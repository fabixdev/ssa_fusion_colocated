//Switch manager based on whether a game object is on or off

using System.Diagnostics;
using UnityEngine;

public class ToggleGameObjects : MonoBehaviour
{

    public GameObject GameDaToogle;
    public GameObject switchon;
    public GameObject switchoff;

    void Update()
    {
        if (GameDaToogle == null || switchon == null || switchoff == null)
        {
            return;
        }

        if (GameDaToogle.activeSelf)
        {
            switchon.SetActive(true);
            switchoff.SetActive(false);
        }
        else
        {
            switchon.SetActive(false);
            switchoff.SetActive(true);
        }
    }
}
