using Fusion;
using System;
using UnityEngine;

public class DisableChildren : NetworkBehaviour
{
    [Networked] private bool isActive { get; set; }  // Stato attivo/inattivo replicato in rete

    // Metodo chiamato localmente per disabilitare i figli e sincronizzare su altri client
    public void DisableAllChildren()
    {
        if (Object.HasStateAuthority)
        {
            // Se siamo lo State Authority, disabilitiamo i figli localmente e sincronizziamo con gli altri client
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            // Chiamata RPC per sincronizzare la disabilitazione con gli altri client
            RPC_DisableAllChildren();
        }
        else
        {
            // Se non siamo lo State Authority, inviamo una richiesta di disabilitazione
            RPC_RequestDisableAllChildren();
        }
    }

    // Metodo RPC per disabilitare i figli su tutti i client
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DisableAllChildren(RpcInfo info = default)
    {
        // Disabilita i figli su tutti i client
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    // Metodo RPC per inviare la richiesta di disabilitazione al client con autorità di stato
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestDisableAllChildren(RpcInfo info = default)
    {
        if (Object.HasStateAuthority)
        {
            // Se siamo lo State Authority, disabilitiamo i figli localmente e sincronizziamo con gli altri client
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            // Chiamata RPC per sincronizzare la disabilitazione con gli altri client
            RPC_DisableAllChildren();
        }
    }
}
