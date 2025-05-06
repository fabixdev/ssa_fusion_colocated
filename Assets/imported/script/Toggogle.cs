using Fusion;
using System.Diagnostics;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Network‑synchronised toggle that shows/hides a **single** GameObject, con log chiaro
/// in italiano che indica l'azione ("ACCESO → SPENGO" oppure "SPENTO → ACCENDO").
/// </summary>
public class Toggogle : NetworkBehaviour, IPointerClickHandler
{
    [Header("Visual da mostrare / nascondere")]
    [SerializeField] private GameObject visual;

    [Networked] public bool IsActive { get; set; }

    // ──────────────────────────────────────────────────────────────────────────
    // Life‑cycle
    // ──────────────────────────────────────────────────────────────────────────
    public override void Spawned()
    {
        if (visual == null)
            visual = gameObject; // se non assegnato, usa il GameObject stesso

        UpdateVisual();
    }

    public override void Render() => UpdateVisual();

    // ──────────────────────────────────────────────────────────────────────────
    // Input
    // ──────────────────────────────────────────────────────────────────────────
    public void OnPointerClick(PointerEventData eventData) => Toggle();

    // ──────────────────────────────────────────────────────────────────────────
    // Toggle logic
    // ──────────────────────────────────────────────────────────────────────────
    public void Toggle()
    {
        if (Object.HasStateAuthority)
        {
            bool eraAcceso = IsActive;
            IsActive = !IsActive;
            UnityEngine.Debug.Log($"{gameObject.name}: {(eraAcceso ? "ACCESO → SPENGO" : "SPENTO → ACCENDO")}  (IsActive = {IsActive})");
        }
        else
        {
            UnityEngine.Debug.Log($"{gameObject.name}: client chiede toggle al server (IsActive = {IsActive})");
            RPC_RequestToggle();
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // RPC
    // ──────────────────────────────────────────────────────────────────────────
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestToggle()
    {
        bool eraAcceso = IsActive;
        IsActive = !IsActive;
        UnityEngine.Debug.Log($"{gameObject.name}: server RPC_RequestToggle ⇒ {(eraAcceso ? "ACCESO → SPENGO" : "SPENTO → ACCENDO")}  (IsActive = {IsActive})");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────
    private void UpdateVisual()
    {
        if (visual != null && visual.activeSelf != IsActive)
            visual.SetActive(IsActive);
    }
}
