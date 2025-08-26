using UnityEngine;

public class StatContainer : MonoBehaviour
{
    public SO_StatContainer Data;

    private void Start()
    {
        Data = Data.CreateRuntimeInstance();
    }
}
