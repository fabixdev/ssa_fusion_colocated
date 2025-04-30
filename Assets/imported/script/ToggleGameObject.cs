using Fusion;
using System.Diagnostics;
using UnityEngine;

public class ToggleGameObject : NetworkBehaviour
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

        // Solo il client con authority puň modificare lo stato
        if (HasStateAuthority)
        {
            IsActive = !IsActive;
            UnityEngine.Debug.Log($"{gameObject.name}: IsActive set to {IsActive}");
            UpdateGameObjectState();
            RPC_UpdateState(IsActive);
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