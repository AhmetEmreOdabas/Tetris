using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Shape[] m_allShapes;
    // Shape Queue ...
    public Transform[] m_queuedXform = new Transform[3];
    Shape[] m_queuedShapes = new Shape[3];
    float m_queueScale = 0.5f;

    public ParticlePlayer m_spawnFX;

    private void Awake() 
    {
        InitQueue();
    }

    Shape GetRandomShape()
    {
        int i = Random.Range(0, m_allShapes.Length);
        if(m_allShapes[i])
        {
            return m_allShapes[i];
        }
        else
        {
            Debug.Log("Warning !!  Invalid Shape...");
            return null;
        }
    }

    public Shape SpawnShape()
    {
        Shape shape = null;
        shape = GetQueuedShape();
        shape.transform.position = transform.position;
        StartCoroutine(GrowShape(shape,transform.position,0.25f));
        //shape.transform.localScale = Vector3.one;  i will replace this with growscale routine

        if(m_spawnFX)
        {
            m_spawnFX.Play();
        }

        if(shape)
        {
            return shape;
        }
        else
        {
            Debug.Log("Warning !!  Invalid Shape...");
            return null;
        }
    }

    void InitQueue()
    {
        for (int i = 0; i < m_queuedShapes.Length; i++)
        {
            m_queuedShapes[i] = null;
        }

        FillQueue();
    }

    void FillQueue()
    {
        for (int i = 0; i < m_queuedShapes.Length; i++)
        {
            if(!m_queuedShapes[i])
            {
                m_queuedShapes[i] = Instantiate(GetRandomShape(), transform.position,Quaternion.identity) as Shape;
                m_queuedShapes[i].transform.position = m_queuedXform[i].transform.position + m_queuedShapes[i].m_queueOffset;
                m_queuedShapes[i].transform.localScale = new Vector3(m_queueScale,m_queueScale,m_queueScale);
            }
        }
    }

    Shape GetQueuedShape()
    {
        Shape firsShape = null;

        if(m_queuedShapes[0])
        {
            firsShape = m_queuedShapes[0];
        }

        for (int i = 1; i < m_queuedShapes.Length; i++)
        {
            m_queuedShapes[i-1] = m_queuedShapes[i];
            m_queuedShapes[i-1].transform.position = m_queuedXform[i-1].position + m_queuedShapes[i].m_queueOffset;
        }

        m_queuedShapes[m_queuedShapes.Length -1] = null;

        FillQueue();

        return firsShape;
    }

    IEnumerator GrowShape(Shape shape, Vector3 pos,float growTime)
    {
        float size = 0;
        growTime = Mathf.Clamp(growTime, 0.1f, 2f);
        float sizeDelta = Time.deltaTime / growTime;

        while (size < 1)
        {
            shape.transform.localScale = new Vector3(size,size,size);
            size += sizeDelta;
            shape.transform.position = pos;
            yield return null;
        }

        shape.transform.localScale = Vector3.one;
    }
}
