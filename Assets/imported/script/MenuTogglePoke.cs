using UnityEngine;
using Fusion;
using System.Diagnostics;
using System;

public class MenuTogglePoke : NetworkBehaviour
{
    [Header("Contenitore da mostrare/nascondere")]
    public GameObject contentContainer;

    [Networked]
    private NetworkBool isOpenNet { get; set; }

    private bool lastOpen;

    private void Start()
    {
        if (contentContainer != null)
            contentContainer.SetActive(false);
    }



    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestToggle(RpcInfo info = default)
    {
        ToggleState();
    }


    private void ToggleState()
    {
        bool newState = !isOpenNet;
        isOpenNet = newState;
        UpdateView(newState);  // Aggiorna subito la UI

        var siblings = transform.parent.GetComponentsInChildren<MenuTogglePoke>();
        foreach (var sib in siblings)
        {
            if (sib != this)
            {
                sib.SetState(false);
                sib.UpdateView(false);
            }
        }
    }

    public void OnPokePressed()
    {
        if (Object.HasStateAuthority)
        {
            ToggleState();
        }
        else
        {
            RPC_RequestToggle();
        }
    }

    private void SetState(bool open)
    {
        if (isOpenNet != open)
        {
            isOpenNet = open;
        }
    }

    private void UpdateView(bool open)
    {
        if (contentContainer != null)
            contentContainer.SetActive(open);
    }

    public override void Render()
    {
        if (lastOpen != isOpenNet)
        {
            lastOpen = isOpenNet;
            UpdateView(isOpenNet);
        }
    }

    public void ForceClose()
    {
        isOpenNet = false;
        if (contentContainer != null)
            contentContainer.SetActive(false);
    }

}
