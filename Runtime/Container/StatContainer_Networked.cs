#if MIRROR
using Mirror;

using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
public class StatContainer_Networked : NetworkBehaviour
{
    bool initialized = false;

    private SO_StatContainer _data;
    public SO_StatContainer Data;

    // syncing
    public float lastUpdate;
    public Dictionary<SO_Stat_Parameter_Base, string> SyncMap = new Dictionary<SO_Stat_Parameter_Base, string>();

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (lastUpdate > 1 / 30f)
        {
            Sync();
            lastUpdate = 0;
        }
        else
        {
            lastUpdate += Time.deltaTime;
        }
    }

    [ContextMenu("Sync")]
    public void Sync()
    {
        foreach (var kvp in SyncMap)
        {
            var parameter = kvp.Key.name;
            var serializedData = kvp.Value;
            CmdSyncParameter(parameter, serializedData);
        }
        SyncMap.Clear();
    }

    [Command]
    void CmdSyncParameter(string parameterId, string data)
    {
        UpdateParameter(parameterId, data);
        RpcSyncParameter(parameterId, data);
    }

    void UpdateParameter(string parameterId, string data)
    {
        var parameter = Data.GetParameterByName(parameterId);
        if (parameter != null)
        {
            parameter.Deserialize(data);
        }
    }

    [ClientRpc]
    void RpcSyncParameter(string parameterId, string data)
    {
        UpdateParameter(parameterId, data);
    }

    public void Init()
    {
        Data = Data.CreateRuntimeInstance();

        foreach (var parameter in Data.parameters)
        {
            parameter.onChange += () =>
            {
                var serializedData = parameter.Serialize();
                SyncMap[parameter] = serializedData;
            };
        }
        initialized = true;
    }
}
#endif
