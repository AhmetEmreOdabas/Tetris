using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchController : MonoBehaviour
{
    public delegate void TouchEventHandler(Vector2 swipe);

    public static event TouchEventHandler DragEvent;
    public static event TouchEventHandler SwipeEvent;
    public static event TouchEventHandler TapEvent;

    Vector2 m_touchMovement;

    [Range(50, 150)]
    public int m_minDragDistance = 100;
    [Range(20,250)]
    public int m_minSwipeDistance = 200;

    float m_tapTimeMax = 0;
    public float m_tapTimeWindow = 0.1f;

    void OnTap()
    {
        if(TapEvent != null)
        {
            TapEvent(m_touchMovement);
        }
    }

    void OnDrag()
    {
        if(DragEvent != null)
        {
            DragEvent(m_touchMovement);
        }
    }

    void OnSwipeEnd()
    {
        if (SwipeEvent != null)
        {
            SwipeEvent(m_touchMovement);
        }
    }

    private void Update() 
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            if(touch.phase == TouchPhase.Began)
            {
                m_touchMovement = Vector2.zero;
                m_tapTimeMax = Time.time + m_tapTimeWindow;
            }
            else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                m_touchMovement += touch.deltaPosition;

                if(m_touchMovement.magnitude > m_minDragDistance)
                {
                    OnDrag();
                }
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                if(m_touchMovement.magnitude > m_minSwipeDistance)
                {
                    OnSwipeEnd();                
                }
                else if(Time.time < m_tapTimeMax)
                {
                    OnTap();  
                }
            }
        }
    }
}
