using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "item", menuName = "Data/Statcontainer")]
public class SO_StatContainer : ScriptableObject
{
    public Dictionary<Type, SO_Stat_Parameter_Base> parametersDictionary; // Dictionary to hold item parameters by name
    public List<SO_Stat_Parameter_Base> parameters; // List of item parameters


    public virtual SO_StatContainer CreateRuntimeInstance()
    {
        SO_StatContainer clone = Clone();
        for (int i = 0; i < parameters.Count; i++)
        {
            // Initialize each parameter
            var parameter = parameters[i];
            var parameterClone = parameter.CreateRuntimeInstance();
            clone.parameters.Add(parameterClone);
            clone.parametersDictionary.Add(parameter.GetType(), parameterClone);
        }
        return clone;
    }

    public T GetParameter<T>() where T : SO_Stat_Parameter_Base
    {
        if (parametersDictionary.TryGetValue(typeof(T), out var parameter))
        {
            return parameter as T;
        }
        return null;
    }

    public SO_Stat_Parameter_Base GetParameterByName(string name)
    {
        foreach (var parameter in parameters)
        {
            if (parameter.name == name)
            {
                return parameter;
            }
        }
        return null;
    }



    public virtual SO_StatContainer Clone()
    {
        // Create a new instance of the actual runtime type
        SO_StatContainer clone = Instantiate(this);
        clone.parametersDictionary = new Dictionary<Type, SO_Stat_Parameter_Base>();
        clone.parameters = new List<SO_Stat_Parameter_Base>();
        return clone;
    }

}
