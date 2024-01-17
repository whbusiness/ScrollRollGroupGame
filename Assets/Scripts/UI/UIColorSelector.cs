using UnityEngine;
using UnityEngine.UI;

public class UIColorSelector : MonoBehaviour
{
    public Slider m_redSlider;
    public Slider m_greenSlider;
    public Slider m_blueSlider;

    public TMPro.TextMeshProUGUI m_redText;
    public TMPro.TextMeshProUGUI m_greenText;
    public TMPro.TextMeshProUGUI m_blueText;

    public Image m_tempImage;
    public Color32 m_outputColor;
    public Vector4 m_input;

    public GameObject m_targetObject;
    public GameObject m_targetObjectFallback;

    public void UpdateColours()
    {
        m_outputColor = new Color32();
        m_outputColor.r = (byte)m_redSlider.value;
        m_outputColor.g = (byte)m_greenSlider.value;
        m_outputColor.b = (byte)m_blueSlider.value;
        m_tempImage.color = m_outputColor;
        m_targetObject.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_BaseColor", m_outputColor);

        if(m_targetObjectFallback != null)
        {
            m_targetObjectFallback.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_BaseColor", m_outputColor);
        }
    }

    public void SetColour(int p_r, int p_g, int p_b, int p_a)
    {
        //Why does this work? Color32 should work just fine.
        m_input = new Vector4(p_r, p_g, p_b, p_a);
        //m_outputColor = new Color32((byte)m_input.x, (byte)m_input.y, (byte)m_input.z, (byte)m_input.w);
        m_redSlider.value = m_input.x;
        m_greenSlider.value = m_input.y;
        m_blueSlider.value = m_input.z;
        m_tempImage.color = new Color32((byte)m_input.x, (byte)m_input.y, (byte)m_input.z, (byte)m_input.w);
        m_targetObject.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_BaseColor", m_outputColor);

        if (m_targetObjectFallback != null)
        {
            m_targetObjectFallback.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_BaseColor", m_outputColor);
        }
    }

    public void SetObjectToEdit(GameObject p_toEdit)
    {
        m_targetObject = p_toEdit;
    }

    public void UpdateRedText()
    {
        UpdateColours();
        UpdateText(m_redSlider, m_redText);
    }

    public void UpdateGreenText()
    {
        UpdateColours();
        UpdateText(m_greenSlider, m_greenText);
    }

    public void UpdateBlueText()
    {
        UpdateColours();
        UpdateText(m_blueSlider, m_blueText);
    }

    private void UpdateText(Slider p_slider, TMPro.TextMeshProUGUI p_sliderText)
    {
        p_sliderText.text = p_slider.value.ToString();
    }
}
