using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ScrollingBackground
{
    public Material m_mat;
    public float m_speed;
}

public class ScrollingBG : MonoBehaviour
{
    public List<ScrollingBackground> m_backgrounds;
    private float dist;

    // Update is called once per frame
    void Update()
    {
        dist += Time.deltaTime;
        for(int i = 0; i < m_backgrounds.Count; i++)
        {
            m_backgrounds[i].m_mat.SetTextureOffset("_MainTex", dist * m_backgrounds[i].m_speed * Vector2.right);
        }
    }
}
