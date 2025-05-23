using UnityEngine;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [Header("Setup")]
    public Transform menuParent;
    public GameObject bottonePrefab;
    public GameObject colorPrefab;

    [System.Serializable]
    public class ColorOption
    {
        public string colorID;
        public Material material;
    }

    [System.Serializable]
    public class SubGroup
    {
        public string name;
        public List<ColorOption> colors;
    }

    [System.Serializable]
    public class Group
    {
        public string name;
        public List<SubGroup> subGroups;
    }

    public List<Group> mainGroups;

    [ContextMenu("Create Menu")]
    public void CreateMenu()
    {
        // Pulisce menuParent
        foreach (Transform child in menuParent)
            Destroy(child.gameObject);

        foreach (var group in mainGroups)
        {
            GameObject groupButton = Instantiate(bottonePrefab, menuParent);
            groupButton.name = group.name + "_Button";

            var btnText = groupButton.GetComponentInChildren<UnityEngine.UI.Text>();
            if (btnText != null)
                btnText.text = group.name;

            // Creo un contenitore semplice per i sottogruppi
            GameObject groupContainer = new GameObject(group.name + "_Container");
            groupContainer.transform.SetParent(groupButton.transform, false);



            // **Assegno contentContainer al MenuTogglePoke del groupButton**
            var groupToggle = groupButton.GetComponent<MenuTogglePoke>();
            if (groupToggle != null)
            {
                groupToggle.contentContainer = groupContainer;
                groupToggle.ForceClose();  // chiude all’inizio     si ma da capire
            }


            foreach (var subGroup in group.subGroups)
            {
                GameObject subGroupButton = Instantiate(bottonePrefab, groupContainer.transform);
                subGroupButton.name = subGroup.name + "_Button";

                var subBtnText = subGroupButton.GetComponentInChildren<UnityEngine.UI.Text>();
                if (subBtnText != null)
                    subBtnText.text = subGroup.name;


                // Qui solo il contenitore "Colors" con RectTransform e scala 1
                GameObject colorContainer = new GameObject("Colors");
                colorContainer.transform.SetParent(subGroupButton.transform, false);

                // **Assegno contentContainer al MenuTogglePoke del subGroupButton**
                var subToggle = subGroupButton.GetComponent<MenuTogglePoke>();
                if (subToggle != null)
                {
                    subToggle.contentContainer = colorContainer;
                    subToggle.ForceClose(); //da capire 
                }


                foreach (var colorOption in subGroup.colors)
                {
                    GameObject colorGO = Instantiate(colorPrefab, colorContainer.transform);
                    colorGO.name = colorOption.colorID;

                    var renderer = colorGO.GetComponent<Renderer>();
                    if (renderer != null)
                        renderer.material = colorOption.material;

                    var draggable = colorGO.GetComponent<Draggable2>();
                    if (draggable != null)
                        draggable.tagName = subGroup.name;
                }
            }
        }
    }
}
