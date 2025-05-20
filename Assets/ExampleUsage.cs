using System.Diagnostics;
using UnityEngine;

public class ExampleUsage : MonoBehaviour
{
    [EditorButton("Create Room")]
    private void CreateRoom()
    {
        UnityEngine.Debug.Log("Room Created!");
    }

    [EditorButton("Join Room")]
    private void JoinRoom()
    {
        UnityEngine.Debug.Log("Joined Room!");
    }
}
