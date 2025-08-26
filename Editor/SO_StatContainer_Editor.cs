using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(SO_StatContainer))]
public class SO_StatContainer_Editor : Editor
{
    private Type[] allParameterTypes;
    private string[] typeNames;
    private int selectedIndex = 0;

    private void OnEnable()
    {
        // Discover all non-abstract classes inheriting from SO_Item_Parameter_Base
        allParameterTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(SO_Stat_Parameter_Base)) && !type.IsAbstract)
            .ToArray();

        typeNames = allParameterTypes.Select(t => t.Name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SO_StatContainer item = (SO_StatContainer)target;

        EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);

        if (item.parameters != null)
        {
            List<SO_Stat_Parameter_Base> toDelete = new List<SO_Stat_Parameter_Base>();

            foreach (var param in item.parameters)
            {
                if (param == null) continue;

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(param.name, EditorStyles.boldLabel);

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    toDelete.Add(param); // Collect for delayed delete
                }

                EditorGUILayout.EndHorizontal();

                if (!toDelete.Contains(param))
                {
                    Editor paramEditor = CreateEditor(param);
                    paramEditor.OnInspectorGUI();
                }

                EditorGUILayout.EndVertical();
            }

            // Defer deletion to next editor update to avoid GUI drawing conflict
            if (toDelete.Count > 0)
            {
                EditorApplication.delayCall += () =>
                {
                    foreach (var param in toDelete)
                    {
                        if (param != null)
                        {
                            item.parameters.Remove(param);
                            UnityEngine.Object.DestroyImmediate(param, true);
                        }
                    }

                    EditorUtility.SetDirty(item);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(item));
                };
            }
        }


        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Add New Parameter", EditorStyles.boldLabel);

        selectedIndex = EditorGUILayout.Popup("Parameter Type", selectedIndex, typeNames);

        if (GUILayout.Button("Add Parameter"))
        {
            AddParameterToItem(allParameterTypes[selectedIndex]);
        }

        serializedObject.ApplyModifiedProperties();
        DrawDefaultInspector();
    }



    private void AddParameterToItem(Type parameterType)
    {
        SO_StatContainer item = (SO_StatContainer)target;

        var newParam = ScriptableObject.CreateInstance(parameterType) as SO_Stat_Parameter_Base;
        if (newParam == null)
        {
            Debug.LogError("Failed to create parameter of type: " + parameterType.Name);
            return;
        }

        newParam.name = parameterType.Name.Split('_')[parameterType.Name.Split('_').Length - 1];

        string path = AssetDatabase.GetAssetPath(item);
        AssetDatabase.AddObjectToAsset(newParam, path);

        // Defer import and update until next Editor loop to prevent asset corruption
        EditorApplication.delayCall += () =>
        {
            item.parameters.Add(newParam);
            EditorUtility.SetDirty(item);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path);
        };
    }
}
