// NetworkedPokeMenu.cs  — versione “CS0200‑free”
using System.Collections.Generic;
using System.Diagnostics;
using Fusion;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

#region ───── STRUCT DATI (nomi prefab) ─────
[System.Serializable] public struct L3 { public string prefabName; }

[System.Serializable]
public struct L2
{
    public string prefabName;
    public L3[] children;
}

[System.Serializable]
public struct L1
{
    public string prefabName;
    public L2[] children;
}
#endregion

[RequireComponent(typeof(NetworkObject))]
public class NetworkedPokeMenu : NetworkBehaviour
{
    [Header("Nomi prefab in Resources/PokePrefabs")]
    public L1[] level1;

    [Header("Anchor di spawn")]
    public Transform row1Anchor;
    public Transform row2Anchor;
    public Transform row3Anchor;

    /*──── indici di stato sincronizzati ────*/
    [Networked] private int mainIdx { get; set; } = -1;
    [Networked] private int subIdx { get; set; } = -1;

    /*──── pool prefab ────*/
    private readonly Dictionary<string, Stack<GameObject>> pool = new();

    /*──────── SPAWNED ────────*/
    public override void Spawned() => BuildRow1();

    /*──────── RPC ────────*/
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_OpenRow2(int idx, RpcInfo info = default)
    {
        mainIdx = idx;
        subIdx = -1;
        RefreshRow2();
        ClearRow(row3Anchor);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_OpenRow3(int idx, RpcInfo info = default)
    {
        subIdx = idx;
        RefreshRow3();
    }

    /*──────── BUILD ROW 1 ────────*/
    private void BuildRow1()
    {
        for (int i = 0; i < level1.Length; i++)
        {
            var go = GetFromPool(level1[i].prefabName, row1Anchor);
            AttachPoke(go, () => RPC_OpenRow2(i));
        }
    }

    /*──────── REFRESH ROW 2 ────────*/
    private void RefreshRow2()
    {
        ClearRow(row2Anchor);

        var kids = level1[mainIdx].children;
        for (int i = 0; i < kids.Length; i++)
        {
            var go = GetFromPool(kids[i].prefabName, row2Anchor);
            int k = i;
            AttachPoke(go, () => RPC_OpenRow3(k));
        }
        row2Anchor.gameObject.SetActive(kids.Length > 0);
    }

    /*──────── REFRESH ROW 3 ────────*/
    private void RefreshRow3()
    {
        ClearRow(row3Anchor);

        if (subIdx < 0) return;
        var kids = level1[mainIdx].children[subIdx].children;
        for (int i = 0; i < kids.Length; i++)
            GetFromPool(kids[i].prefabName, row3Anchor);

        row3Anchor.gameObject.SetActive(kids.Length > 0);
    }

    /*──────── POOL UTILS ────────*/
    private GameObject GetFromPool(string prefabName, Transform parent)
    {
        if (!pool.TryGetValue(prefabName, out var stack))
        {
            stack = new Stack<GameObject>();
            pool[prefabName] = stack;
        }

        GameObject go;
        if (stack.Count > 0)
        {
            go = stack.Pop();
            go.transform.SetParent(parent, false);
            go.SetActive(true);
        }
        else
        {
            var prefab = Resources.Load<GameObject>($"PokePrefabs/{prefabName}");
            if (prefab == null)
            {
                UnityEngine.Debug.LogError($"Prefab '{prefabName}' non trovato in Resources/PokePrefabs/");
                return null;
            }
            go = Instantiate(prefab, parent);
            go.name = prefabName;                      // coerenza nel pool
        }
        return go;
    }

    private void ClearRow(Transform row)
    {
        for (int i = row.childCount - 1; i >= 0; i--)
        {
            var c = row.GetChild(i).gameObject;
            c.SetActive(false);
            pool[c.name].Push(c);
        }
    }

    /*──────── BIND POKE ────────*/
    private void AttachPoke(GameObject go, System.Action onSelect)
    {
        if (go == null) return;

        var wrapper = go.GetComponent<InteractableUnityEventWrapper>();
        if (wrapper == null)
            wrapper = go.AddComponent<InteractableUnityEventWrapper>();

        if (wrapper.WhenSelect == null)
            UnityEngine.Debug.LogWarning($"'{go.name}' ha WhenSelect null: verifica prefab.");

        wrapper.WhenSelect?.RemoveAllListeners();
        wrapper.WhenSelect?.AddListener(() => onSelect());
    }
}
