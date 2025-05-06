using UnityEngine;

public class DisableAtStart : MonoBehaviour
{
    void Start()
    {
        // Disattiva l'oggetto ogni volta che viene abilitato
        gameObject.SetActive(false);
    }

    public void Enables()
    {
        // Disattiva l'oggetto ogni volta che viene abilitato
        gameObject.SetActive(true);
    }
}
