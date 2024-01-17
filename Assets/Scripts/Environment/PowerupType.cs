using System;
using UnityEngine;

public enum PowerupType
{
    None,
    Electric,
    Poison,
    Fire,
    Ice
}

[Serializable]
public struct PowerupPreset
{
    public Sprite m_elementSprite;
    public PowerupType m_type;
}