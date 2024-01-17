using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 5f;
    public float leftBoundary = -5f;  // Adjust as needed
    public float rightBoundary = 5f;  // Adjust as needed

    private int direction = 1; // 1 for right, -1 for left

    void Update()
    {
        // Calculate the movement in the X-axis
        float moveX = speed * direction * Time.deltaTime;

        // Calculate the new X position with boundaries
        float newX = Mathf.Clamp(transform.position.x + moveX, leftBoundary, rightBoundary);

        // Move the cube to the new position
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // Check if the cube has reached a boundary, and reverse direction
        if (newX == leftBoundary || newX == rightBoundary)
        {
            direction *= -1;
        }
    }
}