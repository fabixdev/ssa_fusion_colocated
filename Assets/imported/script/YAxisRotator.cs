using UnityEngine;
using UnityEngine.EventSystems;

public class YAxisRotator : MonoBehaviour
{
    [Tooltip("Transform dell'oggetto handle che l'utente trascina nello spazio (già dotato di logica draggable nativa)")]
    public Transform draggableHandle;

    [Tooltip("Trasform dell'oggetto da ruotare attorno all'asse Y")]
    public Transform targetToRotate;

    [Tooltip("Coordinata X minima consentita per l'handle (corrispondente a 0°)")]
    public float minX = -1f;

    [Tooltip("Coordinata X massima consentita per l'handle (corrispondente a 360°)")]
    public float maxX = 1f;

    // Memorizza posizioni iniziali Y e Z per bloccarle
    private float initialY;
    private float initialZ;

    void Start()
    {
        if (draggableHandle != null)
        {
            initialY = draggableHandle.position.y;
            initialZ = draggableHandle.position.z;
        }
    }

    void Update()
    {
        if (draggableHandle == null || targetToRotate == null)
            return;

        // Prendi la posizione corrente dell'handle in world space
        Vector3 handlePos = draggableHandle.position;

        // Clampa l'asse X tra minX e maxX
        float clampedX = Mathf.Clamp(handlePos.x, minX, maxX);

        // Blocca gli assi Y e Z ai valori iniziali
        Vector3 lockedPos = new Vector3(clampedX, initialY, initialZ);
        draggableHandle.position = lockedPos;

        // Calcola t come interpolazione 0→1 da minX a maxX
        float range = maxX - minX;
        float t = 0f;
        if (range > Mathf.Epsilon)
        {
            t = Mathf.InverseLerp(minX, maxX, clampedX);
        }

        // Calcola angolo Y tra 0 e 360
        float angleY = t * 360f;

        // Applica rotazione attorno all'asse Y (in locale)
        targetToRotate.localRotation = Quaternion.Euler(0f, angleY, 0f);
    }

    /*
     * ISTRUZIONI:
     * 1. Trascina questo script su un GameObject vuoto oppure sullo stesso GameObject del draggableHandle.
     * 2. In Inspector assegna:
     *    - draggableHandle: il Transform dell'handle già dotato di logica drag.
     *    - targetToRotate: il Transform dell'oggetto che vuoi ruotare sull'asse Y.
     *    - minX: la X world-space corrispondente a rotazione 0°.
     *    - maxX: la X world-space corrispondente a rotazione 360°.
     * 3. Al runtime, la posizione iniziale di Y e Z viene salvata in Start().
     * 4. Ogni Update clampa la X in [minX, maxX] e forza Y/Z ai valori iniziali.
     * 5. Traduce la X normalizzata in un angolo Y e lo assegna a targetToRotate.
     */
}
