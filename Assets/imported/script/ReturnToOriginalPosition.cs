using System.Collections;
using UnityEngine;

public class ReturnToOriginalPosition : MonoBehaviour
{
    [Header("Initial state")]
    public Vector3 originalPosition;
    public Quaternion originalRotation;

    [Header("References")]
    public Rigidbody rb;

    [Header("Behaviour")]
    public float delay = 2f;          // Secondi da attendere prima del ritorno
    public float minVelocity = 0.01f; // Soglia per considerare l'oggetto “fermo”

    private bool isReturning = false;
    private float timer = 0f;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. Controlla se l'oggetto si sta muovendo
        if (rb.velocity.magnitude > minVelocity)      // <-- correzione qui
        {
            // Se l'oggetto si muove, annulla un eventuale ritorno in corso
            if (isReturning)
            {
                StopCoroutine(ReturnToPosition());
                isReturning = false;
            }
            timer = 0f;
        }
        else
        {
            // 2. Se è fermo ma non è nella posizione originale, avvia il timer
            if (!isReturning && Vector3.Distance(transform.position, originalPosition) > 0.001f)
            {
                timer += Time.deltaTime;
                if (timer >= delay)
                {
                    isReturning = true;
                    StartCoroutine(ReturnToPosition());
                }
            }
        }
    }

    private IEnumerator ReturnToPosition()
    {
        float duration = 1f; // Durata dell’interpolazione
        float elapsedTime = 0f;

        Vector3 startingPos = transform.position;
        Quaternion startingRot = transform.rotation;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startingPos, originalPosition, t);
            transform.rotation = Quaternion.Lerp(startingRot, originalRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Assicura che i valori finali siano esatti
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        // Ripristina lo stato
        isReturning = false;
        timer = 0f;
    }
}
