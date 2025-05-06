using Fusion;
using Oculus.Interaction;
using System.Diagnostics;
using UnityEngine;

public class PokeMenuNode : MonoBehaviour
{
    public int level = 1;            // 1 = riga1, 2 = riga2
    public int indexInLevel = 0;     // lo assegni tu da Inspector
    public NetworkedHierarchyMenu manager; // drag manuale

    private void Awake()
    {
        // ci agganciamo al WhenSelect già presente
        var wrapper = GetComponent<InteractableUnityEventWrapper>();
        if
            (wrapper) wrapper.WhenSelect.AddListener(SelectSelf);
        else
            UnityEngine.Debug.LogError($"{name} manca InteractableUnityEventWrapper");
    }

    private void SelectSelf() => manager.Select(level, indexInLevel);
}
