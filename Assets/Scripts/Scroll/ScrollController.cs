using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScrollController : MonoBehaviour
{
    public MatchTimerController m_timer;
    public PhysicMaterial m_defaultPhysicMaterial;
    public float m_scrollRollPower = 25.0f;
    public float m_scrollIdlePower = 1.0f;

    [SerializeField]
    private Transform m_endCapL;
    [SerializeField]
    private Transform m_endCapR;
    [SerializeField]
    private Transform m_binder;
    [SerializeField]
    private Transform m_body;

    [SerializeField]
    private MeshCollider m_bodyCollider;

    public ElementType m_currentElement;
    public ElementType m_nextElement;

    private MeshRenderer m_endCapRendererL;
    private MeshRenderer m_endCapRendererR;
    private MeshRenderer m_binderRenderer;
    private MeshRenderer m_bodyRenderer;

    [SerializeField]
    public List<ElementType> m_possibleElements;
    [SerializeField]
    private List<ScrollBiome> m_biomes;
    [SerializeField]
    private PlatformSpawner m_platformSpawner;
    private Vector3 m_position;

    [SerializeField]
    private Rigidbody m_rb;
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip changingEnvironmentAudioClip;
    [SerializeField]
    private AudioClip scrollingAudioClip;
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (m_endCapL != null && m_endCapRendererL == null && !m_endCapL.TryGetComponent(out m_endCapRendererL)) Debug.LogError("Scroll End Cap has no MeshRenderer!");
        if (m_endCapR != null && m_endCapRendererR == null && !m_endCapR.TryGetComponent(out m_endCapRendererR)) Debug.LogError("Scroll End Cap has no MeshRenderer!");
        if (m_binder != null && m_binderRenderer == null && !m_binder.TryGetComponent(out m_binderRenderer)) Debug.LogError("Scroll Binder has no MeshRenderer!");
        if (m_body != null && m_bodyRenderer == null && !m_body.TryGetComponent(out m_bodyRenderer)) Debug.LogError("Scroll Body has no MeshRenderer!");

        if (m_body != null && m_bodyCollider == null && !m_body.TryGetComponent(out m_bodyCollider)) Debug.LogError("Scroll Body has no Mesh Collider!");
    
        foreach(ScrollBiome l_biome in m_biomes)
        {
            l_biome.m_preset.m_biomeStart = 0;
            l_biome.m_preset.m_biomeEnd = 0;
            l_biome.m_preset.m_biomeTimeStart = 0;
            l_biome.m_preset.m_biomePreTimeStart = 0;
            l_biome.m_preset.m_biomePostTimeStart = 0;
        }

        m_position = transform.position;
        m_platformSpawner = FindObjectOfType<PlatformSpawner>();
    }

    public void FixedUpdate()
    {
        transform.position = m_position;
    }

    public void Roll(float p_amount)
    {
        m_rb.AddTorque(Vector3.left * p_amount, ForceMode.Acceleration);
    }

    public void RollIdle()
    {
        Roll(m_scrollIdlePower);
    }

    public void RollPower()
    {
        Roll(m_scrollRollPower);
    }

    public ScrollBiome SetNextBiome()
    {
        m_nextElement = m_possibleElements[Random.Range(0, m_possibleElements.Count)];
        return m_biomes[(int)m_nextElement];
    }

    public void GoToNextBiome()
    {
        SetBiome(m_nextElement);
    }

    private void SetBiome(ElementType p_type)
    {
        PlayScrollEnvironmentChangeAudio();
        print(m_currentElement);
        print(p_type);
        m_currentElement = p_type;

        if (m_possibleElements.Contains(p_type))
        {
            m_possibleElements.Remove(p_type);
        }
        else
        {
            return;
        }

        var l_biomePreset = m_biomes[(int)p_type].m_preset;

        m_endCapRendererL.sharedMaterial = l_biomePreset.m_endCapMaterial;
        m_endCapRendererR.sharedMaterial = l_biomePreset.m_endCapMaterial;
        m_binderRenderer.sharedMaterial = l_biomePreset.m_binderMaterial;
        m_bodyRenderer.sharedMaterial = l_biomePreset.m_bodyMaterial;

        if (l_biomePreset.m_physicMaterial != null)
        {
            m_bodyCollider.material = l_biomePreset.m_physicMaterial;
        } else
        {
            m_bodyCollider.material = m_defaultPhysicMaterial;
        }
        m_platformSpawner.SpawnPlatforms();
    }

    public void PlayScrollEnvironmentChangeAudio()
    {
            audioSource.Stop();
            audioSource.clip = changingEnvironmentAudioClip;
            audioSource.time = 0f;
            audioSource.Play();
            Invoke(nameof(SwitchAudioClip), 4f);
    }

    void SwitchAudioClip()
    {
        audioSource.Stop();
        audioSource.clip = scrollingAudioClip;
        audioSource.time = 0f;
        audioSource.Play();
    }
}
