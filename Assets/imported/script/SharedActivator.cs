using Fusion;
using System;
using UnityEngine;

public class SharedActivator : NetworkBehaviour
{
    [Networked] private bool isActive { get; set; }  // Stato attivo/inattivo replicato in rete

    public override void Spawned()
    {
        // Imposta lo stato iniziale del GameObject appena l'oggetto di rete è stato spawnato
        gameObject.SetActive(isActive);
    }

    public override void FixedUpdateNetwork()
    {
        // Sincronizza lo stato del GameObject in base alla proprietà di rete aggiornata
        if (gameObject.activeSelf != isActive)
        {
            gameObject.SetActive(isActive);
        }
    }

    // Metodi pubblici chiamabili localmente per attivare/disattivare l'oggetto di rete
    public void ActivateGameObject()
    {
        if (Object.HasStateAuthority)
        {
            // Se siamo lo State Authority, aggiorniamo subito la proprietà di rete
            isActive = true;
            // Notifica agli altri client che lo stato è cambiato
            RPC_ToggleActive(true);
        }
        else
        {
            // Se non siamo lo State Authority, invochiamo l'RPC per chiedere di attivare l'oggetto
            RPC_RequestActivate();
        }
    }

    public void DeactivateGameObject()
    {
        if (Object.HasStateAuthority)
        {
            isActive = false;
            // Notifica agli altri client che lo stato è cambiato
            RPC_ToggleActive(false);
        }
        else
        {
            // Se non siamo lo State Authority, invochiamo l'RPC per chiedere di disattivare l'oggetto
            RPC_RequestDeactivate();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestActivate(RpcInfo info = default)
    {
        // Solo lo State Authority può cambiare lo stato
        if (Object.HasStateAuthority)
        {
            isActive = true;
            RPC_ToggleActive(true);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestDeactivate(RpcInfo info = default)
    {
        // Solo lo State Authority può cambiare lo stato
        if (Object.HasStateAuthority)
        {
            isActive = false;
            RPC_ToggleActive(false);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ToggleActive(bool state, RpcInfo info = default)
    {
        // Aggiorna lo stato del GameObject in base al valore passato dalla RPC
        isActive = state;
        gameObject.SetActive(isActive);
    }
}