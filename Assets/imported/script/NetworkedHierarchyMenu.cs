using Fusion;
using UnityEngine;

public class NetworkedHierarchyMenu : NetworkBehaviour
{
    [Header("Gruppi da accendere")]
    public GameObject[] row2Groups;   // Group_A, Group_B…
    public GameObject[] row3Groups;   // SubGroup_C, SubGroup_D…

    /*────────── VARIABILI DI RETE (solo host può scriverle) ─────────*/
    [Networked] private int mainIdx { get; set; } = -1;
    [Networked] private int subIdx { get; set; } = -1;

    /*────────── CACHE LOCALE (tutti possono scrivere) ─────────*/
    private int localMain = -1;
    private int localSub = -1;

    /*===================== SELEZIONE LOCALE =====================*/
    public void Select(int level, int idx)
    {
        // 1️⃣  Feedback immediato → aggiorna cache locale + UI
        ApplyLocal(level, idx);

        // 2️⃣  Se non siamo host, chiediamo all’host di confermare
        if (!HasStateAuthority)
            RPC_RequestSelection(level, idx);
        else
            RPC_BroadcastSelection(level, idx); // host replica agli altri
    }

    /*========================== RPC ============================*/
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestSelection(int level, int idx, RpcInfo info = default)
    {
        // l'host aggiorna le variabili di rete
        if (level == 1) mainIdx = idx;
        else subIdx = idx;

        RPC_BroadcastSelection(level, idx);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_BroadcastSelection(int level, int idx, RpcInfo info = default)
    {
        ApplyLocal(level, idx); // tutti i client applicano alla loro UI
    }

    /*====================== LOGICA LOCALE ======================*/
    private void ApplyLocal(int level, int idx)
    {
        if (level == 1)
        {
            localMain = idx;
            localSub = -1;  // reset riga 3
        }
        else if (level == 2)
        {
            localSub = idx;
        }

        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        /* riga 2 */
        for (int i = 0; i < row2Groups.Length; i++)
            row2Groups[i].SetActive(i == localMain);

        /* riga 3 */
        int flatIdx = (localMain >= 0 && localSub >= 0)
                      ? (localMain * 10 + localSub)   // mapping semplice
                      : -1;

        for (int i = 0; i < row3Groups.Length; i++)
            row3Groups[i].SetActive(i == flatIdx);
    }
}
