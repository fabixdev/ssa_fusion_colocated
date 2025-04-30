using Fusion;
using System;
using UnityEngine;

public class OnOffGameObject : NetworkBehaviour
{
    // Networked property to track the active state
    [Networked]
    public NetworkBool IsActive { get; set; }

    // Method to manually check if the state has changed
    public void UpdateState()
    {
        // Only update if the object has been spawned and has authority
        if (Object.HasStateAuthority)
        {
            gameObject.SetActive(IsActive);
        }
    }

    // Method to turn on the GameObject
    public void TurnOn()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: TurnOn called");
        IsActive = true;
    }

    // Method to turn off the GameObject
    public void TurnOff()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: TurnOff called");
        IsActive = false;
    }

    // Public methods to control the GameObject
    public void ActivateGameObject()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: ActivateGameObject called");
        if (Object.HasStateAuthority)
        {
            TurnOn();
        }
    }

    public void DeactivateGameObject()
    {
        UnityEngine.Debug.Log($"{gameObject.name}: DeactivateGameObject called");
        if (Object.HasStateAuthority)
        {
            TurnOff();
        }
    }

    // Initialize the state when spawned
    public override void Spawned()
    {
        base.Spawned();
        UpdateState();  // Ensure the correct initial state when spawned
    }

    // Method to update the state each frame (if needed)
    private void Update()
    {
        // Only update the state if the object has authority
        if (Object.HasStateAuthority)
        {
            if (IsActive != gameObject.activeSelf)
            {
                UpdateState();  // Update GameObject state manually
            }
        }
    }
}
