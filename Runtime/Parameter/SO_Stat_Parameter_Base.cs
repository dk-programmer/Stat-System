using System;
using Unity.VisualScripting;
using UnityEngine;

public class SO_Stat_Parameter_Base : ScriptableObject
{

    public Action onChange;
    /// <summary>
    /// Create a runtime instance of the parameter. Used to be overridden by derived classes to instantiate specific parameter types at runtime.
    /// </summary>
    /// <returns></returns>
    public virtual SO_Stat_Parameter_Base CreateRuntimeInstance()
    {
        return this;
    }

    /// <summary>
    /// Clone the current instance of the parameter.
    /// </summary>
    /// <returns></returns>
    public virtual SO_Stat_Parameter_Base Clone()
    {
        SO_Stat_Parameter_Base clone = Instantiate(this);
        return clone;
    }

    public virtual void UpdateStatEffects(float delta)
    {

    }

    public virtual string Serialize()
    {
        return null;
    }

    public virtual void Deserialize(string data)
    {
        // Implement deserialization logic if needed
    }
}
