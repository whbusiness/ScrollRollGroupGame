using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuBackingHandler : MonoBehaviour
{
    [SerializeField]
    private UIAnimTransformModify m_backingModify;
    [SerializeField]
    private UIAnimTransformModify m_PABModify;

    public UIAnimationList m_list;
    public bool m_backingStarted = false;
    public Button m_toSelect;

    public void HandleBacking()
    {
        if (!m_backingStarted)
        {
            m_backingStarted = true;
            m_PABModify.StartReverse();
            m_backingModify.StartRun();
            m_backingModify.GetComponent<UIAnimImageColorFade>().StartRun();
            m_list.RunAll();
            m_toSelect.Select();
        }
    }
}