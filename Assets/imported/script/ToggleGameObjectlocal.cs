using UnityEngine;

public class ToggleGameObjectlocal : MonoBehaviour
{
    // Metodo per accendere il GameObject
    public void TurnOn()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: TurnOn called");
        gameObject.SetActive(true);
    }

    // Metodo per spegnere il GameObject
    public void TurnOff()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: TurnOff called");
        gameObject.SetActive(false);
    }

    // Metodo per cambiare lo stato del GameObject
    public void Toggle()
    {
        if (gameObject.activeSelf)
        {
            UnityEngine.Debug.Log($"{gameObject.name}: GameObject is active, turning off");
            TurnOff();
        }
        else
        {
            UnityEngine.Debug.Log($"{gameObject.name}: GameObject is inactive, turning on");
            TurnOn();
        }
    }
}
