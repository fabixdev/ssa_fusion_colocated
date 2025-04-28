using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class DraggableSphere : NetworkBehaviour
{
    public GameObject OriginalColoreSphere;
    public string tagName = "";
    public string excludedTag = "";
    public AudioSource audioSource;

    private void Start()
    {
        if (OriginalColoreSphere != null && !string.IsNullOrEmpty(tagName))
        {
            if (!OriginalColoreSphere.CompareTag(excludedTag))
            {
                OriginalColoreSphere.tag = tagName;
                UnityEngine.Debug.Log("Tag assigned to OriginalColoreSphere: " + tagName);
            }
            else
            {
                UnityEngine.Debug.Log("OriginalColoreSphere has the excluded tag and its tag was not changed.");
            }
        }
        else
        {
            UnityEngine.Debug.Log("Tag assignment failed. Ensure OriginalColoreSphere is set and tagName is not empty.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (OriginalColoreSphere == null) return;

        MeshRenderer sourceMeshRenderer = OriginalColoreSphere.GetComponent<MeshRenderer>();

        if (sourceMeshRenderer == null)
        {
            UnityEngine.Debug.Log("The OriginalColoreSphere does not have a MeshRenderer.");
            return;
        }

        if (other.gameObject.CompareTag(tagName) && !other.gameObject.CompareTag(excludedTag))
        {
            UnityEngine.Debug.Log("Collided with object having tag: " + tagName);

            if (audioSource != null) audioSource.Play();

            MeshRenderer targetMeshRenderer = other.gameObject.GetComponent<MeshRenderer>();
            NetworkObject netObj = other.gameObject.GetComponent<NetworkObject>();

            if (targetMeshRenderer != null && netObj != null)
            {
                string materialName = sourceMeshRenderer.material.name;

                if (Object.HasStateAuthority)
                {
                    RequestMaterialTransfer(netObj.Id, materialName);
                }
                else
                {
                    RPC_RequestMaterialTransfer(netObj.Id, materialName);
                }
            }
            else
            {
                UnityEngine.Debug.Log("Missing MeshRenderer or NetworkObject on collided object.");
            }
        }
        else
        {
            UnityEngine.Debug.Log("Collided object does not have the specified tag or is excluded: " + other.gameObject.tag);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestMaterialTransfer(NetworkId targetId, string materialName)
    {
        RequestMaterialTransfer(targetId, materialName);
    }

    private void RequestMaterialTransfer(NetworkId targetId, string materialName)
    {
        RPC_TransferMaterial(targetId, materialName);
        RPC_TransferMaterialToAllWithTag(materialName);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_TransferMaterial(NetworkId targetId, string materialName)
    {
        GameObject targetObj = FindObjectById(targetId);

        if (targetObj != null && !targetObj.CompareTag(excludedTag))
        {
            MeshRenderer targetMeshRenderer = targetObj.GetComponent<MeshRenderer>();
            MeshRenderer sourceMeshRenderer = OriginalColoreSphere.GetComponent<MeshRenderer>();

            if (targetMeshRenderer != null && sourceMeshRenderer != null)
            {
                Material sourceMaterial = FindMaterialByName(sourceMeshRenderer.materials, materialName);
                if (sourceMaterial != null)
                {
                    targetMeshRenderer.material = sourceMaterial;
                    UnityEngine.Debug.Log($"Material {materialName} assigned to {targetObj.name}");
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_TransferMaterialToAllWithTag(string materialName)
    {
        MeshRenderer sourceMeshRenderer = OriginalColoreSphere.GetComponent<MeshRenderer>();
        Material sourceMaterial = FindMaterialByName(sourceMeshRenderer.materials, materialName);

        if (sourceMaterial != null)
        {
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tagName);
            foreach (GameObject obj in taggedObjects)
            {
                if (!obj.CompareTag(excludedTag))
                {
                    MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                    if (mr != null)
                    {
                        mr.material = sourceMaterial;
                        UnityEngine.Debug.Log("Material " + materialName + " assigned to object with tag: " + obj.name);
                    }
                }
            }
        }
    }

    private Material FindMaterialByName(Material[] materials, string name)
    {
        foreach (var mat in materials)
        {
            if (mat.name.Contains(name)) // Sometimes Unity adds suffix like " (Instance)"
                return mat;
        }
        return null;
    }

    private GameObject FindObjectById(NetworkId id)
    {
        foreach (var netObj in FindObjectsOfType<NetworkObject>())
        {
            if (netObj.Id == id)
                return netObj.gameObject;
        }
        return null;
    }
}
