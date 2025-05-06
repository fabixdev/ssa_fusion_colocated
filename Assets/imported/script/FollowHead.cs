using UnityEngine;

public class FollowHead : MonoBehaviour
{
    public Transform targetTransform;       // La trasform da seguire
    public Vector3 offset = Vector3.zero;   // Offset opzionale per la posizione

    void Update()
    {
        if (targetTransform != null)
        {
            // Segue la posizione con offset
            transform.position = targetTransform.position + offset;

            // Mantiene rotazione X e Z, ma copia la Y dal target
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.y = targetTransform.eulerAngles.y;
            transform.eulerAngles = currentRotation;
        }
    }
}
