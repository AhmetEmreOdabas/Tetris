using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    GameController m_gameController;
    TouchController m_touchController;

    public Slider m_dragDistanceSlider;
    public Slider m_swipeDistanceSlider;

    private void Start() 
    {
        m_touchController = GameObject.FindWithTag("Touch").GetComponent<TouchController>();
        m_gameController = GameObject.FindWithTag("GameManager").GetComponent<GameController>();

        if(m_dragDistanceSlider != null)
        {
            m_dragDistanceSlider.minValue = 50;
            m_dragDistanceSlider.maxValue = 150; 
            m_dragDistanceSlider.value = 100;
        }

        if (m_swipeDistanceSlider != null)
        {
            m_swipeDistanceSlider.minValue = 20;
            m_swipeDistanceSlider.maxValue = 250;
            m_swipeDistanceSlider.value = 50;
        }

        UpdatePanel();
    }

    public void UpdatePanel()
    {
        if(m_dragDistanceSlider != null)
        {
            if(m_touchController != null)
            {
                m_touchController.m_minDragDistance =(int) m_dragDistanceSlider.value;
            }
        }

        if(m_swipeDistanceSlider != null)
        {
            if (m_touchController != null)
            {
                m_touchController.m_minSwipeDistance =(int) m_swipeDistanceSlider.value;
            }
        }
    }
}
