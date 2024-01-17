using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public float rotationSpeed = 10f;        // Speed of rotation in degrees per second
    public float rotationDelay = 2f;         // Delay before starting rotation
    public float returnDelay = 2f;           // Delay before returning to the original position
    public float rotationAmount = 90f;      // Amount of rotation in degrees
    public float returnTransitionDuration = 1f; // Duration of the return transition

    private Quaternion originalRotation;
    private Quaternion finalRotation;
    private bool isRotating = false;

    void Start()
    {
        originalRotation = transform.rotation;
        finalRotation = originalRotation * Quaternion.Euler(Vector3.right * rotationAmount);
    }

    void Update()
    {
        if (isRotating)
        {
            // Rotate the platform around the X-axis up to the specified rotationAmount
            float step = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation, step);
        }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Player is on the platform
            if (!isRotating)
            {
                // Start rotating after a delay
                Invoke("StartRotation", rotationDelay);
            }
        }
    }

    void StartRotation()
    {
        // Start rotating
        isRotating = true;

        // Invoke the method to reset rotation after completing the rotation
        Invoke("ResetRotation", rotationDelay + returnDelay);
    }

    void ResetRotation()
    {
        // Stop rotating and smoothly interpolate back to the original position
        isRotating = false;
        StartCoroutine(LerpRotation(originalRotation, returnTransitionDuration));
    }

    IEnumerator LerpRotation(Quaternion targetRotation, float duration)
    {
        float elapsed = 0f;
        Quaternion startRotation = transform.rotation;

        while (elapsed < duration)
        {
            // Use Quaternion.Slerp for smooth interpolation
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation is exactly the target rotation
        transform.rotation = targetRotation;
    }
}