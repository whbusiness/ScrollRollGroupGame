using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform m_toFollow;
    public string m_ipCamOwner;

    // Update is called once per frame
    void Update()
    {
        if(m_toFollow != null) transform.position = new Vector3(m_toFollow.position.x, m_toFollow.position.y, transform.position.z);
    }
}
