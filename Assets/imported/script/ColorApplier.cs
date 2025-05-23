using UnityEngine;
using Fusion;
using System.Diagnostics;

public class ColorApplier : NetworkBehaviour
{
    public string tagColorA;
    public string tagColorB;
    public GameObject target;

    [Networked]
    private Color NetworkColor { get; set; }

    private MeshRenderer targetRenderer;
    private Color lastAppliedColor;

    public override void Spawned()
    {
        if (target == null)
            target = gameObject;

        targetRenderer = target.GetComponent<MeshRenderer>();
        ApplyNetworkColor();
    }

    public void ApplyColorA() => ApplyColorFromTag(tagColorA);
    public void ApplyColorB() => ApplyColorFromTag(tagColorB);

    private void ApplyColorFromTag(string tag)
    {
        if (!HasStateAuthority)
            return;

        GameObject source = GameObject.FindWithTag(tag);
        if (source == null)
        {
            //Debug.LogWarning("Nessun oggetto trovato con il tag: " + tag);
            return;
        }

        var sourceRenderer = source.GetComponent<MeshRenderer>();
        if (sourceRenderer == null)
        {
            //Debug.LogWarning("MeshRenderer mancante sull'oggetto sorgente");
            return;
        }

        NetworkColor = sourceRenderer.material.color;
    }

    public override void FixedUpdateNetwork()
    {
        if (targetRenderer != null && targetRenderer.material.color != NetworkColor)
        {
            ApplyNetworkColor();
        }
    }

    private void ApplyNetworkColor()
    {
        if (targetRenderer == null && target != null)
            targetRenderer = target.GetComponent<MeshRenderer>();

        if (targetRenderer != null)
        {
            targetRenderer.material.color = NetworkColor;
        }
    }
}
