using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct MagicalOverloadFadeColour
{
    public int m_magicalOverloadTrigger;
    public Color m_colour;
}

public class MagicalOverloadHandler : MonoBehaviour
{
    public List<Sprite> m_images;

    [SerializeField]
    private int m_magicalOverload = 0;
    [SerializeField]
    private List<Image> m_digits;
    [SerializeField]
    [Tooltip(tooltip: "Please make sure these are ordered by trigger value!")]
    private List<MagicalOverloadFadeColour> m_colourTriggers;

    private int m_zeroCharPos = Convert.ToInt32('0');

    public void SetMagicalOverload(int p_magicalOverload)
    {
        //print(gameObject.name + " new MO: " + p_magicalOverload);
        m_magicalOverload = p_magicalOverload;

        //Initalise colour lerp calculation values
        Color p_targetColorA = Color.white;
        Color p_targetColorB = Color.white;
        Color p_targetColorFinal = Color.white; 
        if (m_magicalOverload < 100)
        {
            //Loop over available colour triggers
            for (int i = 0; i < m_colourTriggers.Count; i++)
            {
                //If this colour trigger is less than/equal to magical overload
                if (m_colourTriggers[i].m_magicalOverloadTrigger <= m_magicalOverload)
                {
                    //Set source lerp calc value
                    p_targetColorA = m_colourTriggers[i].m_colour;
                }
                else
                {
                    //Set target lerp calc value
                    p_targetColorB = m_colourTriggers[i].m_colour;
                }
            }

            p_targetColorFinal = Color.Lerp(p_targetColorA, p_targetColorB, (m_magicalOverload / 100.0f));
            //Lerp between found colours
        } else
        {
            p_targetColorFinal = m_colourTriggers[m_colourTriggers.Count - 1].m_colour;
        }

        //Get string representation of magical overload
        string l_translatedMO = m_magicalOverload.ToString();

        //print(gameObject.name + " MO string: " + l_translatedMO);

        //Loop over available digit images
        for(int i = 0; i < l_translatedMO.Length; i++)
        {
            //Set image sprite to sprite representing this digit of the magical overload 
            //This takes advantage of the fact that the ASCII char values of the digits are the same as their int values
            m_digits[i].sprite = m_images[Convert.ToInt32(l_translatedMO[(l_translatedMO.Length-1) - i]) - m_zeroCharPos];
            //Set digit colour
            m_digits[i].color = p_targetColorFinal;
        }

        //Account for prefixing 0s being culled for 100s and 10s digits
        if(m_magicalOverload < 100)
        {
            m_digits[2].color = Color.clear;

            if(m_magicalOverload < 10)
            {
                m_digits[1].color = Color.clear;
            }
        }
    }

    public int GetMagicalOverload()
    {
        return m_magicalOverload;
    }
}
