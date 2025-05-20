using UnityEngine;
using System.Collections.Generic;

public class MenuHierarchyManager : MonoBehaviour
{
    [Header("Configurazione del Menu")]
    public Transform menuParent;  // Dove posizionare l'intero menu (genitore)
    public GameObject colorPrefab; // Prefab per i colori

    [System.Serializable]
    public class ColorOption
    {
        public string colorID;  // ID del colore (es. "Bianco", "Rosso")
        public Material material;  // Materiale associato al colore
    }

    [System.Serializable]
    public class SubGroup
    {
        public string name;  // Nome del sotto-gruppo (es. "Finestra", "Basso")
        public GameObject bottonerow2; // Qui dovrebbero andare i bottoni del row2
        public List<ColorOption> colorOptions;  // Opzioni di colore per il sotto-gruppo
    }

    [System.Serializable]
    public class Group
    {
        public string name;  // Nome del gruppo (es. "Vetro", "Muri")
        public GameObject bottonerow1; // Qui dovrebbero andare i bottoni del row1
        public List<SubGroup> subGroups;  // Sotto-gruppi (es. "Finestra", "Basso")
    }

    // Lista dei gruppi principali (padre)
    public List<Group> mainGroups;

    // Funzione che genera il menu
    [EditorButton("Create Color")]
    private void CreateMenu()
    {
        GenerateMenu();  // Crea la gerarchia del menu all'inizio
    }

    // Funzione per generare l'intera gerarchia del menu
    public void GenerateMenu()
    {
        // Creazione dei gruppi principali (row1)
        foreach (var group in mainGroups)
        {
            // Crea un gruppo principale (padre) come oggetto di gioco
            GameObject parentGroup = CreateGroup(group.name, menuParent);

            // Crea un bottone per il gruppo principale (row1)
            CreateButton(group.bottonerow1, parentGroup.transform);

            // Creazione dei sotto-gruppi (row2) per il gruppo principale
            CreateSubGroups(group, parentGroup.transform);
        }
    }

    // Funzione per creare un gruppo principale (padre)
    private GameObject CreateGroup(string name, Transform parent)
    {
        // Crea un oggetto di gioco per il gruppo
        GameObject groupObject = new GameObject(name);
        groupObject.transform.SetParent(parent);  // Imposta il parent correttamente
        return groupObject;
    }

    // Funzione per creare un bottone
    private void CreateButton(GameObject buttonPrefab, Transform parent)
    {
        if (buttonPrefab != null)
        {
            GameObject buttonObject = Instantiate(buttonPrefab, parent);
            buttonObject.name = parent.name + "_Button";
        }
    }

    // Funzione per creare i sotto-gruppi (row2) per un gruppo principale
    private void CreateSubGroups(Group parentGroup, Transform parentTransform)
    {
        foreach (var subGroup in parentGroup.subGroups)
        {
            // Crea un oggetto di gioco per il sotto-gruppo (row2)
            GameObject subGroupObject = CreateGroup(subGroup.name, parentTransform);

            // Crea un bottone per il sotto-gruppo (row2)
            CreateButton(subGroup.bottonerow2, subGroupObject.transform);

            // Creazione delle opzioni di colore (row3) per il sotto-gruppo
            CreateColorOptions(subGroup, subGroupObject.transform);
        }
    }

    // Funzione per creare le opzioni di colore (row3) per un sotto-gruppo
    private void CreateColorOptions(SubGroup subGroup, Transform parentTransform)
    {
        foreach (var colorOption in subGroup.colorOptions)
        {
            // Crea un GameObject per ogni colore (row3)
            GameObject colorObject = Instantiate(colorPrefab, parentTransform);
            colorObject.name = colorOption.colorID;  // Imposta il nome del colore

            // Imposta il materiale del colore
            Renderer renderer = colorObject.GetComponent<Renderer>();
            renderer.material = colorOption.material;
        }
    }
}
