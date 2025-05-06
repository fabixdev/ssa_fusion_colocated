using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform targetTransform;   // Da cui leggere posizione e rotazione
    public GameObject targetObject;     // A cui applicare
    public Vector3 offset = Vector3.zero; // Offset solo sulla posizione

    [ContextMenu("Esegui NewPositionAndZRotation (Debug)")]
    public void NewPosition()
    {
        if (targetTransform != null && targetObject != null)
        {
            // Applica la posizione con offset
            targetObject.transform.position = targetTransform.position + offset;

            // Applica solo la rotazione Z, lasciando X e Y invariati
            Vector3 currentRotation = targetObject.transform.eulerAngles;
            currentRotation.y = targetTransform.eulerAngles.y;
            targetObject.transform.eulerAngles = currentRotation;
        }
        else
        {
            UnityEngine.Debug.Log("targetTransform o targetObject non assegnato!");
        }
    }
}
