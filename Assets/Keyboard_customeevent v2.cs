using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Keyboard_customeevent : MonoBehaviour
{
    [Tooltip("Set here the key name - default is 'a'")]
    public string keyname = "a";
    [Tooltip("Set here the object and the Event at keypress")]
    public UnityEvent keypressCustomEvent; // the events to be run

    private InputAction keyboardAction;

    private void Start()
    {
        keyboardAction = new InputAction("keyboardAction", binding: $"<Keyboard>/{keyname}");
        keyboardAction.started += OnKeyPress;
        keyboardAction.Enable();
    }

    private void OnKeyPress(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Fired key: " + keyname);
            if (keypressCustomEvent != null)
            {
                keypressCustomEvent.Invoke();
            }
        }
    }

    private void OnDisable()
    {
        keyboardAction.Disable();
        keyboardAction.Dispose();
    }
}
