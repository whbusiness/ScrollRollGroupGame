using UnityEngine;

public class UIShowHideControls : MonoBehaviour
{
    [SerializeField] GameObject m_controlsPanel;

    public void OnShowHide()
    {
        m_controlsPanel.SetActive(!m_controlsPanel.activeSelf);
    }
}
