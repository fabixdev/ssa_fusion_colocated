using UnityEngine;

public class DisableAtStart : MonoBehaviour
{
    private void OnEnable()
    {
        // Disattiva l'oggetto ogni volta che viene abilitato
        gameObject.SetActive(false);
    }
}
