using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Transform m_emptySprite;
    public int m_boardHeight = 30;
    public int m_boardWidth = 10;
    public int m_header = 8;

    Transform[,] m_grid;

    public int m_completedRows = 0;
    public ParticlePlayer[] m_rowGlowFX = new ParticlePlayer[4];

    private void Awake() 
    {
        m_grid = new Transform[m_boardWidth,m_boardHeight];
    }

    private void Start() 
    {
        DrawEmptyCells();
    }

    void DrawEmptyCells()
    {
        if(m_emptySprite)
        {
            for (int y = 0; y < m_boardHeight - m_header; y++)
            {
                for (int x = 0; x < m_boardWidth; x++)
                {
                    Transform clone;
                    clone = Instantiate(m_emptySprite, new Vector3(x, y, 0), Quaternion.identity) as Transform;
                    clone.name = "Board Space (x = " + x.ToString() + " , y = " + y.ToString() + ")";
                    clone.transform.parent = transform;
                }
            }
        }
        else
        {
            Debug.Log("WARNING !!  You need to assign m_emptySprite Transform to Board gameobject.");
        }
    }

    bool IsWithinBoard(int x,int y)
    {
        return(x >= 0 && x < m_boardWidth && y >= 0);
    }

    public bool IsValidPosition(Shape shape)
    {
        foreach (Transform child in shape.transform)
        {
            Vector2 pos = Vectorf.Round(child.position);

            if(!IsWithinBoard((int) pos.x , (int) pos.y))
            {
                return false;
            }
            if(IsOccupied((int) pos.x , (int) pos.y, shape))
            {
                return false;
            }
        }

        return true;
    }

    bool IsOccupied(int x, int y, Shape shape)
    {
        return (m_grid[x,y] != null && m_grid[x,y].parent != shape.transform);
    }

    public void StoreShapeInGrid(Shape shape)
    {
        if(shape == null)
        {
            return;
        }

        foreach (Transform child in shape.transform)
        {
            Vector2 pos = Vectorf.Round(child.position);
            m_grid[(int) pos.x , (int) pos.y] = child;
        }
    }

    bool IsComplete(int y)
    {
        for (int x = 0; x < m_boardWidth; x++)
        {
            if(m_grid[x,y] == null)
            {
                return false;
            }
        }

        return true;
    }

    void ClearRow(int y)
    {
        for (int x = 0; x < m_boardWidth; x++)
        {
            if(m_grid[x,y] != null)
            {
                Destroy(m_grid[x,y].gameObject);
            }
            m_grid[x,y] = null;
        }
    }

    void ShiftOneRowDown(int y)
    {
        for (int x = 0; x < m_boardWidth; x++)
        {
            if(m_grid[x,y] != null)
            {
                m_grid[x,y-1] = m_grid[x,y];
                m_grid[x,y] = null;
                m_grid[x,y-1].position += new Vector3(0,-1,0);
            }
        }
    }

    void ShiftRowsDown(int startY)
    {
        for (int i = startY; i < m_boardHeight; i++)
        {
            ShiftOneRowDown(i);
        }
    }

    public IEnumerator ClearAllRows()
    {
        m_completedRows = 0;

        for (int y = 0; y < m_boardHeight; y++)
        {
            if(IsComplete(y))
            {
                ClearRowFX(m_completedRows, y);
                m_completedRows++;
            }
        }

        yield return new WaitForSeconds(0.5f);

        for (int y = 0; y < m_boardHeight; y++)
        {
            if(IsComplete(y))
            {
                ClearRow(y);
                ShiftRowsDown(y + 1);
                yield return new WaitForSeconds(0.15f);
                y--;
            }
        }
    }

    public bool IsOverLimit(Shape shape)
    {
        foreach (Transform child in shape.transform)
        {
            if(child.transform.position.y >= (m_boardHeight - m_header -1))
            {
                return true;
            }
        }

        return false;
    }

    void ClearRowFX(int idx,int y)
    {
        if(m_rowGlowFX[idx])
        {
            m_rowGlowFX[idx].transform.position = new Vector3(0,y,-2);
            m_rowGlowFX[idx].Play();
        }
    }
}
