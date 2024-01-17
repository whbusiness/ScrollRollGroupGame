using System;
using UnityEngine;

[Serializable]
public struct ScrollBiome
{
    public ScrollPreset m_preset;
    public ElementType m_targetElement;
}

[CreateAssetMenu(fileName = "New Scroll Preset", menuName = "ScrollRoll/Scroll Preset")]
public class ScrollPreset : ScriptableObject
{
    public string m_presetName;
    public Sprite m_sprite;
    public Material m_binderMaterial;
    public Material m_endCapMaterial;
    public Material m_bodyMaterial;
    public PhysicMaterial m_physicMaterial;

    public float m_biomeStart = 0;
    public float m_biomeEnd = 0;

    public float m_biomePreTimeStart;
    public float m_biomeTimeStart; //This is also the biomePreTimeEnd
    public float m_biomePostTimeStart; //This is also the biomeTimeEnd
}