using UnityEngine;

public class GroundFlag : MonoBehaviour
{
    [SerializeField]
    private bool m_isGround;

    public bool Check()
    {
        return m_isGround;
    }
}