using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class StatContainer_Networked : NetworkBehaviour
{
    bool initialized = false;


    private SO_StatContainer _data;
    public SO_StatContainer Data;

    //syncing
    public float lastUpdate;
    public Dictionary<SO_Stat_Parameter_Base, string> SyncMap = new Dictionary<SO_Stat_Parameter_Base, string>();

    private void Start()
    {

        Init();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (lastUpdate > 1/30)
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
            // Assuming you have a method to send data over the network
            // This is a placeholder for actual network sync logic
            CmdSyncParameter(parameter, serializedData);
        }
        SyncMap.Clear();
    }

    /// <summary>
    /// Called on server when client wants to sync a parameter.
    /// The server then broadcasts to all clients.
    /// </summary>
    [Command]
    void CmdSyncParameter(string parameterId, string data)
    {

        UpdateParameter(parameterId,data);
        // Broadcast to all clients
        RpcSyncParameter(parameterId, data);
    }

    void UpdateParameter(string parameterId, string data)
    {
        // Update server-side container
        var parameter = Data.GetParameterByName(parameterId);
        if (parameter != null)
        {
            parameter.Deserialize(data);
        }
    }

    /// <summary>
    /// Runs on all clients when server broadcasts parameter changes.
    /// </summary>
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
                if (SyncMap.TryGetValue(parameter, out var serializedData))
                {
                    serializedData = parameter.Serialize();
                    SyncMap[parameter] = serializedData;
                }
                else
                {
                    serializedData = parameter.Serialize();
                    SyncMap.Add(parameter, serializedData);
                }
            };
        }
        initialized = true;
    }
}
