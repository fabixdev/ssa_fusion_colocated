using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Draggable2 : NetworkBehaviour
{
    public string tagName = "";
    private string excludedTag = "sphereremoved";

    private void Start()
    {
        if (!string.IsNullOrEmpty(tagName))
        {
            if (!gameObject.CompareTag(excludedTag))
            {
                gameObject.tag = tagName;
                UnityEngine.Debug.Log("Tag assigned to GameObject: " + tagName);
            }
        }
        else
        {
            UnityEngine.Debug.Log("Tag assignment failed. Ensure tagName is not empty.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        MeshRenderer sourceMeshRenderer = GetComponent<MeshRenderer>();

        if (sourceMeshRenderer == null)
        {
            UnityEngine.Debug.Log("The GameObject does not have a MeshRenderer.");
            return;
        }

        if (other.gameObject.CompareTag(tagName) && !other.gameObject.CompareTag(excludedTag))
        {
            UnityEngine.Debug.Log("Collided with object having tag: " + tagName);

            GameObject.Find("soundconfirm").GetComponent<AudioSource>().Play();

            MeshRenderer targetMeshRenderer = other.gameObject.GetComponent<MeshRenderer>();
            NetworkObject netObj = other.gameObject.GetComponent<NetworkObject>();

            if (targetMeshRenderer != null && netObj != null)
            {
                string materialName = sourceMeshRenderer.material.name;

                if (HasStateAuthority)
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
            MeshRenderer sourceMeshRenderer = GetComponent<MeshRenderer>();

            if (targetMeshRenderer != null && sourceMeshRenderer != null)
            {
                Material sourceMaterial = FindMaterialByName(sourceMeshRenderer.materials, materialName);
                if (sourceMaterial != null)
                {
                    targetMeshRenderer.material = sourceMaterial;
                    UnityEngine.Debug.Log("Material " + materialName + " assigned to " + targetObj.name);
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_TransferMaterialToAllWithTag(string materialName)
    {
        MeshRenderer sourceMeshRenderer = GetComponent<MeshRenderer>();
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
            if (mat.name.Contains(name))
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
