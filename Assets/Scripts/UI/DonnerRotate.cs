using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonnerRotate : MonoBehaviour
{
    public bool m_doRotate = true;
    public float m_rotateSpeed = 1.0f;
    public Vector3 m_rotationAxis = Vector3.up;

    public void FixedUpdate()
    {
        if (m_doRotate)
        {
            transform.Rotate(m_rotationAxis * m_rotateSpeed);
        }
    }
}
