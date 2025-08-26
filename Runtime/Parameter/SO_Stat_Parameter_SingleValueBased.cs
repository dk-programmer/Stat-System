using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public abstract class SO_Stat_Parameter_SingleValueBased : SO_Stat_Parameter_Base
{
    [SerializeField]
    private float _value;
    public float Value
    {
        get => _value;
        set
        {
            float oldValue = _value;
            _value = Mathf.Clamp(value, minValue, maxValue);
            if(oldValue != _value)
            {
                onChange?.Invoke();
            }
        }
    }
    public float minValue = 0f;
    public float maxValue = 100f;
    public float defaultValue = 100f;


    public List<SO_StatEffect_Base> statEffects;

    protected virtual void OnValidate()
    {
        // Clamp default value to valid range
        defaultValue = Mathf.Clamp(defaultValue, minValue, maxValue);

        // If the current Value isn’t initialized, or needs to follow defaultValue, update it
        if (!Application.isPlaying)
        {
            Value = defaultValue;
        }
    }

    public override void UpdateStatEffects(float delta)
    {
        //TODO
        base.UpdateStatEffects(delta);
    }

    public override SO_Stat_Parameter_Base CreateRuntimeInstance()
    {
        return Clone();
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public override void Deserialize(string data)
    {
        if (string.IsNullOrEmpty(data)) return;

        // Overwrite this object's fields with the JSON data
        // This will NOT overwrite event handlers or methods
        JsonUtility.FromJsonOverwrite(data, this);
    }
}
