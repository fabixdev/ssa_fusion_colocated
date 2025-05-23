using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public GameObject prefab;

    [System.Serializable]
    public class ConfigurablePart
    {
        public string partName;
        public List<ColorOption> colorOptions;
    }

    [System.Serializable]
    public class ColorOption
    {
        public string colorID;
        public Material material;
    }

    public List<ConfigurablePart> configurableParts;
    public Transform spawnParent;

    private void OnValidate()
    {
        if (prefab != null && configurableParts != null)
        {
            //ClonePrefabs();
        }
    }

    [EditorButton("Create Color")]
    private void ClonePrefabs()
    {
        foreach (Transform child in spawnParent)
        {
            DestroyImmediate(child.gameObject);
        }

        foreach (var part in configurableParts)
        {
            GameObject parentObject = new GameObject(part.partName);
            parentObject.transform.SetParent(spawnParent);

            foreach (var colorOption in part.colorOptions)
            {
                GameObject clone = Instantiate(prefab, parentObject.transform);
                Renderer[] renderers = clone.GetComponentsInChildren<Renderer>();

                foreach (Renderer renderer in renderers)
                {
                    Material[] newMaterials = new Material[renderer.materials.Length];

                    for (int j = 0; j < newMaterials.Length; j++)
                    {
                        newMaterials[j] = renderer.materials[j];
                    }

                    newMaterials[0] = colorOption.material;
                    renderer.materials = newMaterials;
                }



                // Assegna il partName al tagName nello script del clone
                Draggable2 draggableScript = clone.GetComponent<Draggable2>();
                if (draggableScript != null)
                {
                    draggableScript.tagName = part.partName;
                }

                UnityEngine.Debug.Log($"Creato clone per {part.partName} con colore {colorOption.colorID}");
            }
        }
    }
}
