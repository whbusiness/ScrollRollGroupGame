using UnityEngine;

[CreateAssetMenu(fileName = "New Entity Physics Data", menuName = "ScrollRoll/Entity")]
public class EntityPhysicsData : ScriptableObject
{
    public float m_movementSpeed;
    public float m_jumpHeight;
    public float m_jumpDelay;
    public Vector3 m_spawnLocation;
}