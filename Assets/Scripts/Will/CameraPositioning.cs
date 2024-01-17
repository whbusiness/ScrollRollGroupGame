using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositioning : MonoBehaviour
{
    public Vector3 LeftTargetPoint, RightTargetPoint, CentreTargetPoint;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime;
    //Main Camera transform
    public Transform m_cam;

    private void Start()
    {
        //if(m_cam == null) m_cam = Camera.main.transform;
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_cam != null)
        {
            if (other.gameObject.name == "LeftTrigger" && CameraZoomer.ZoomIn)
            {
                m_cam.position = Vector3.SmoothDamp(m_cam.position, LeftTargetPoint, ref velocity, smoothTime);
            }
            if (other.gameObject.name == "RightTrigger" && CameraZoomer.ZoomIn)
            {
                m_cam.position = Vector3.SmoothDamp(m_cam.position, RightTargetPoint, ref velocity, smoothTime);
            }
            if (other.gameObject.name == "CentreTrigger" && CameraZoomer.ZoomIn)
            {
                m_cam.position = Vector3.SmoothDamp(m_cam.position, CentreTargetPoint, ref velocity, smoothTime);
            }
            if (!CameraZoomer.ZoomIn)
            {
                m_cam.position = Vector3.SmoothDamp(m_cam.position, CentreTargetPoint, ref velocity, smoothTime);
            }
        }
    }
}
