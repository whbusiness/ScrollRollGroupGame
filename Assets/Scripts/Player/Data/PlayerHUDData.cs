using UnityEngine;

[CreateAssetMenu(fileName = "New Player HUD Data", menuName = "ScrollRoll/Player HUD Data")]
public class PlayerHUDData : ScriptableObject
{
    public const int m_maxLives = 3;
    public int m_currentLives = 3;
    public float m_currentMagicalOverload = 0;

    public float TakeDamage(float p_attackDamage)
    {
        return m_currentMagicalOverload += p_attackDamage;
    }
}