using Fusion;
using System;
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
        if (Object.HasStateAuthority)
        {
            IsActive = !IsActive; // Alterna lo stato
            UnityEngine.Debug.Log($"{gameObject.name}: IsActive set to {IsActive}");
            UpdateGameObjectState();
            RPC_UpdateState(IsActive); // Sincronizza con gli altri client
            GameObject.Find("turntoggles").GetComponent<AudioSource>().Play();
        }
        else
        {
            // Se non siamo lo State Authority, inviamo una richiesta per cambiare lo stato
            RPC_RequestToggle();
            GameObject.Find("turntoggles").GetComponent<AudioSource>().Play();
        }
    }

    public void TurnOn()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: TurnOn called");

        // Solo il client con authority può modificare lo stato
        if (Object.HasStateAuthority)
        {
            if (!IsActive)  // Accende solo se non è già acceso
            {
                IsActive = true;
                UnityEngine.Debug.Log($"{gameObject.name}: IsActive set to true");
                UpdateGameObjectState();
                RPC_UpdateState(true); // Sincronizza con gli altri client
                GameObject.Find("turnons").GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            // Se non siamo lo State Authority, inviamo una richiesta per accendere l'oggetto
            RPC_RequestTurnOn();
            GameObject.Find("turnons").GetComponent<AudioSource>().Play();
        }
    }

    public void TurnOff()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: TurnOff called");

        // Solo il client con authority può modificare lo stato
        if (Object.HasStateAuthority)
        {
            if (IsActive)  // Spegne solo se è acceso
            {
                IsActive = false;
                UnityEngine.Debug.Log($"{gameObject.name}: IsActive set to false");
                UpdateGameObjectState();
                RPC_UpdateState(false); // Sincronizza con gli altri client
                GameObject.Find("turnoffs").GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            // Se non siamo lo State Authority, inviamo una richiesta per spegnere l'oggetto
            RPC_RequestTurnOff();
            GameObject.Find("turnoffs").GetComponent<AudioSource>().Play();
        }
    }

    private void UpdateGameObjectState()
    {
        gameObject.SetActive(IsActive);
        UnityEngine.Debug.Log($"{gameObject.name}: GameObject SetActive({IsActive})");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_UpdateState(bool active)
    {
        // Sincronizza lo stato su tutti i client
        IsActive = active;
        UpdateGameObjectState();
        UnityEngine.Debug.Log($"{gameObject.name}: RPC_UpdateState received, active = {active}");
    }

    // RPC per richiedere al client con authority di eseguire il toggle
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestToggle(RpcInfo info = default)
    {
        if (Object.HasStateAuthority)
        {
            IsActive = !IsActive; // Alterna lo stato
            RPC_UpdateState(IsActive); // Sincronizza con gli altri client
        }
    }

    // RPC per richiedere al client con authority di accendere il GameObject
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestTurnOn(RpcInfo info = default)
    {
        if (Object.HasStateAuthority)
        {
            if (!IsActive)
            {
                IsActive = true;
                RPC_UpdateState(true); // Sincronizza con gli altri client
            }
        }
    }

    // RPC per richiedere al client con authority di spegnere il GameObject
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestTurnOff(RpcInfo info = default)
    {
        if (Object.HasStateAuthority)
        {
            if (IsActive)
            {
                IsActive = false;
                RPC_UpdateState(false); // Sincronizza con gli altri client
            }
        }
    }
}
