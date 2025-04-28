using Fusion;
using UnityEngine;

public class OnOffGameObject : NetworkBehaviour
{
    // Metodo RPC per accendere il GameObject
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RpcTurnOn()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: RpcTurnOn called");
        gameObject.SetActive(true);
    }

    // Metodo RPC per spegnere il GameObject
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RpcTurnOff()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: RpcTurnOff called");
        gameObject.SetActive(false);
    }

    // Chiamate locali per attivare/disattivare tramite input
    private void Update()
    {
        if (!HasInputAuthority) return; // Solo il client con autorità può invocare gli input

        if (Input.GetKeyDown(KeyCode.Y)) // Premi 'Y' per attivare
        {
            UnityEngine.Debug.Log($"{gameObject.name}: 'Y' key pressed, calling RpcTurnOn");
            RpcTurnOn();
        }

        if (Input.GetKeyDown(KeyCode.U)) // Premi 'U' per disattivare
        {
            UnityEngine.Debug.Log($"{gameObject.name}: 'U' key pressed, calling RpcTurnOff");
            RpcTurnOff();
        }
    }
}
