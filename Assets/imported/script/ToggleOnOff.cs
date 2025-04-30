using Fusion;
using System.Diagnostics;
using UnityEngine;

public class ToggleOnOff : NetworkBehaviour
{
    [Networked]
    public bool IsActive { get; set; }

    private void Start()
    {
        UpdateGameObjectState();
    }

    public override void Spawned()
    {
        base.Spawned();
        UpdateGameObjectState();
    }

    public void Toggle()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: Toggle called");

        // Solo il client con authority può modificare lo stato
        if (HasStateAuthority)
        {
            IsActive = !IsActive; // Alterna lo stato
            UnityEngine.Debug.Log($"{gameObject.name}: IsActive set to {IsActive}");
            UpdateGameObjectState();
            RPC_UpdateState(IsActive); // Sincronizza con gli altri client
        }
    }

    public void TurnOn()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: TurnOn called");

        // Solo il client con authority può modificare lo stato
        if (HasStateAuthority)
        {
            if (!IsActive)  // Accende solo se non è già acceso
            {
                IsActive = true;
                UnityEngine.Debug.Log($"{gameObject.name}: IsActive set to true");
                UpdateGameObjectState();
                RPC_UpdateState(true); // Sincronizza con gli altri client
            }
        }
    }

    public void TurnOff()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: TurnOff called");

        // Solo il client con authority può modificare lo stato
        if (HasStateAuthority)
        {
            if (IsActive)  // Spegne solo se è acceso
            {
                IsActive = false;
                UnityEngine.Debug.Log($"{gameObject.name}: IsActive set to false");
                UpdateGameObjectState();
                RPC_UpdateState(false); // Sincronizza con gli altri client
            }
        }
    }

    private void UpdateGameObjectState()
    {
        gameObject.SetActive(IsActive);
        UnityEngine.Debug.Log($"{gameObject.name}: GameObject SetActive({IsActive})");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateState(bool active)
    {
        IsActive = active;
        UpdateGameObjectState();
        UnityEngine.Debug.Log($"{gameObject.name}: RPC_UpdateState received, active = {active}");
    }
}
