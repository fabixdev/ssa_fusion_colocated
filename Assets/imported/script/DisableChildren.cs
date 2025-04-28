using Fusion;
using UnityEngine;

public class DisableChildren : NetworkBehaviour
{
    // Metodo chiamato localmente per disabilitare i figli e sincronizzare su altri client
    public void DisableAllChildren()
    {
        // Disabilita i figli localmente
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        // Chiama l'RPC per sincronizzare con gli altri client
        RPC_DisableAllChildren();
    }

    // RPC per disabilitare i figli su altri client
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_DisableAllChildren()
    {
        // Evita di rieseguire la disattivazione se già fatta localmente
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
