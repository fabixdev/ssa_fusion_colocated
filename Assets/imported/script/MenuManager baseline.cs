using Oculus.Interaction;  // Importa il namespace di Oculus Interaction
using UnityEngine;
using System.Collections.Generic;
using Fusion;  // Importa Fusion per la gestione della rete

public class MenuManagerbaseline : NetworkBehaviour
{
    [Header("Configurazione del Menu")]
    public Transform menuParent;  // Dove posizionare l'intero menu (genitore)
    public GameObject colorPrefab; // Prefab per i colori
    public GameObject bottonepoke; // Qui dovrebbero andare i bottoni poke


    [System.Serializable]
    public class ColorOption
    {
        //row3 (blocco colori)
        public string colorID;  // ID del colore (es. "Bianco", "Rosso")
        public Material material;  // Materiale associato al colore
    }

    [System.Serializable]
    public class SubGroup
    {
        // row2 (sotto-gruppo)
        public string nametag;  // Nome del sotto-gruppo (es. "Finestra", "Basso")
        public List<ColorOption> colorOptions;  // Opzioni di colore per il sotto-gruppo (row3)
    }

    [System.Serializable]
    public class Group
    {
        // row1 (gruppo principale)
        public string namerow;  // Nome del gruppo (es. "Vetro", "Muri")
        public List<SubGroup> subGroups;  // Sotto-gruppi (es. "Finestra", "Basso")
    }

    // Lista dei gruppi principali (padre)
    public List<Group> mainGroups;


    // Crea il menu dal debug dello script in inspector
    [ContextMenu("Create Menu")]
    private void CreateMenu()
    {
        GenerateMenu();  // Crea la gerarchia del menu all'inizio
    }

    // Funzione per generare l'intera gerarchia del menu
    public void GenerateMenu()
    {
        foreach (var group in mainGroups)
        {
            GameObject parentGroup = CreateGroup(group.namerow, menuParent);

            // Crea un bottone per il gruppo principale (row1)
            CreateButton(bottonepoke, parentGroup.transform, 1, mainGroups.IndexOf(group));

            // Creazione dei sotto-gruppi (row2) per il gruppo principale
            CreateSubGroups(group, parentGroup.transform);
        }
    }

    private GameObject CreateGroup(string name, Transform parent)
    {
        GameObject groupObject = new GameObject(name);
        groupObject.transform.SetParent(parent);
        return groupObject;
    }

    // Funzione per creare un bottone e associare la selezione
    private void CreateButton(GameObject buttonPrefab, Transform parent, int level, int idx)
    {
        if (buttonPrefab != null)
        {
            GameObject buttonObject = Instantiate(buttonPrefab, parent);
            buttonObject.name = parent.name + "_Button";


        }
    }

    private void CreateSubGroups(Group parentGroup, Transform parentTransform)
    {
        for (int i = 0; i < parentGroup.subGroups.Count; i++)
        {
            var subGroup = parentGroup.subGroups[i];

            GameObject subGroupObject = CreateGroup(subGroup.nametag, parentTransform);
            CreateButton(bottonepoke, subGroupObject.transform, 2, i);

            CreateColorOptions(subGroup, subGroupObject.transform);
        }
    }

    private void CreateColorOptions(SubGroup subGroup, Transform parentTransform)
    {
        foreach (var colorOption in subGroup.colorOptions)
        {
            GameObject colorObject = Instantiate(colorPrefab, parentTransform);
            colorObject.name = colorOption.colorID;
            Renderer renderer = colorObject.GetComponent<Renderer>();
            renderer.material = colorOption.material;

            //funzione per mettere il tag name nello script interno al prefab no toccare
            Draggable2 draggableScript = colorObject.GetComponent<Draggable2>();
            if (draggableScript != null)
            {
                draggableScript.tagName = subGroup.nametag;
            }
        }
    }
}