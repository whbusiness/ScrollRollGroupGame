using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class RandomizeSpriteColour : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer m_renderer;
    [SerializeField]
    private GOAnimTransformModify m_anim;
    private Vector3 m_targetPosition;
    public bool m_hasAnyKeyPressed = false;
    public UIMainMenuBackingHandler m_backing;
    public List<Button> m_buttons;
    private MasterInput m_input;

    private int m_buttonPos = 0;
    private Vector3 m_startPos;

    private void Start()
    {
        m_renderer = m_renderer == null ? GetComponent<SpriteRenderer>() : m_renderer;
        m_renderer.color = new Color(Random.value, Random.value, Random.value);
        m_anim = m_anim == null ? GetComponent<GOAnimTransformModify>() : m_anim;

        m_targetPosition = m_anim.GetData().m_targetValue;

        InputSystem.onAnyButtonPress.CallOnce(p_action => {
            if (!m_hasAnyKeyPressed)
            {
                m_hasAnyKeyPressed = true;
                StartScroll();
            }
        });

        m_input = new MasterInput();
        m_input.Enable();

        
        /*
        m_input.UI.Navigate.performed += ctx => {
            Vector2 l_uiMove = ctx.ReadValue<Vector2>();

            transform.position += new Vector3(0, l_uiMove.y, 0);
            m_buttonPos -= (int)l_uiMove.y;

            if(transform.position.y > m_startPos.y)
            {
                transform.position = m_startPos + new Vector3(0, -3, 0);
                m_buttonPos = 3;
            }

            if(transform.position.y < m_startPos.y - 3)
            {
                transform.position = m_startPos;
                m_buttonPos = 0;
            }

            //m_buttons[m_buttonPos].Select();
            //EventSystem.current.SetSelectedGameObject(null);
            //EventSystem.current.SetSelectedGameObject(m_buttons[m_buttonPos].gameObject);
        };
        */
    }

    private void OnEnable()
    {
        m_anim.StartRun();
    }

    private void OnDisable()
    {
        //m_input.Disable();
    }

    private void FixedUpdate()
    {
        if (!m_hasAnyKeyPressed)
        {
            if (m_anim.GetData().m_status != UISoftwareAnimStatus.Running)
            {
                Vector3 l_add = new()
                {
                    x = Mathf.Clamp(m_targetPosition.x + Mathf.PingPong(Time.time, 0.5f), m_targetPosition.x, m_targetPosition.x + 0.5f),
                    y = Mathf.Clamp(m_targetPosition.y + Mathf.PingPong(Time.time, 0.25f), m_targetPosition.y, m_targetPosition.y + 0.5f),
                    z = transform.localPosition.z
                };
                transform.localPosition = l_add;
            }

            
        } else
        {
            for (int i = 0; i < m_buttons.Count; i++)
            {
                if (m_buttons[i].gameObject == EventSystem.current.currentSelectedGameObject)
                {
                    transform.position = m_startPos + new Vector3(0, -i, 0);
                }
            }
        }
    }

    private void StartScroll()
    {
        m_backing.HandleBacking();
        m_startPos = transform.position;
    }
}
