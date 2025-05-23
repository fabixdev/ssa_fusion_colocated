using UnityEngine;
using Fusion;
using System;

public class AnchorManager : NetworkBehaviour
{
    [Header("Oggetto da spawnare")]
    public GameObject anchoredPrefab;

    [Header("Posizione sopra al tavolo")]
    public Vector3 spawnPosition = new Vector3(0, 1, 0);

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            Runner.Spawn(anchoredPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
