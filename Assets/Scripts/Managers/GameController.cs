using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // reference to board
    Board m_gameBoard;
    // reference to spawner
    Spawner m_spawner;
    // currently active shape
    Shape m_activeShape;

    SoundManager m_soundManager;

    ScoreManager m_scoreManager;

    GhostShape m_ghostShape;

    Holder m_holder;

    bool m_gameOver = false;

    //Shape move props
    public float m_dropDelay = 1f;  // This determines to drop frequency !!
    float m_dropDelayModded;
    float m_timeToDrop;      // <<<< You can also add start delay from that prop  ...

    //Shape Input Props..
    float m_timeToNextKeyLeftRight;

    [Range(0.02f, 1f)]
    public float m_keyRepeatRateLeftRight = 0.25f;

    float m_timeToNextKeyDown;

    [Range(0.02f, 1f)]
    public float m_keyRepeatRateDown = 0.25f;

    float m_timeToNextKeyRotate;

    [Range(0.02f, 1f)]
    public float m_keyRepeatRateRotate = 0.25f;

    public GameObject m_gameOverPanel;

    public IconToggle m_rotateIconToggle;
    
    bool m_clockwise = true;

    public bool m_isPaused = false;

    public GameObject m_pausePanel;

    public ParticlePlayer m_gameOverFX;

    enum Direction {none, left, right, up, down}

    Direction m_dragDirection = Direction.none;
    Direction m_swipeDirection = Direction.none;

    float m_timeToNextDrag;
    float m_timeToNextSwipe;

    [Range(0.05f,1f)]
    public float m_minTimeToDrag = 0.15f;

    [Range(0.05f, 1f)]
    public float m_minTimeToSwipe = 0.3f;

    bool m_didTap = false;


    private void OnEnable() 
    {
        TouchController.DragEvent += DragHandler;
        TouchController.SwipeEvent += SwipeHandler;
        TouchController.TapEvent += TapHandler;
        
    }

    private void OnDisable() 
    {
        TouchController.DragEvent -= DragHandler;
        TouchController.SwipeEvent -= SwipeHandler;
        TouchController.TapEvent -= TapHandler;
    }

    private void Start() 
    {
        m_dropDelayModded = m_dropDelay;
        m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;
        m_gameBoard = GameObject.FindWithTag("Board").GetComponent<Board>();
        m_spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();
        m_soundManager = GameObject.FindWithTag("Sounds").GetComponent<SoundManager>();
        m_scoreManager = GameObject.FindWithTag("Score").GetComponent<ScoreManager>();
        m_ghostShape = GameObject.FindWithTag("Ghost").GetComponent<GhostShape>();
        m_holder = GameObject.FindWithTag("Holder").GetComponent<Holder>();

        if(!m_gameBoard || !m_spawner)
        {
            Debug.Log("Warning check the GameBoard and Spawner if its exist !!");
        }

        if(m_spawner)
        {
            m_spawner.transform.position = Vectorf.Round(m_spawner.transform.position);

            if(!m_activeShape)
            {
                m_activeShape = m_spawner.SpawnShape();
            }
        }

        if(m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(false);
        }

        if(m_pausePanel)
        {
            m_pausePanel.SetActive(false);
        }
    }

    private void Update() 
    {
        if (!m_gameBoard || !m_spawner || !m_activeShape || m_gameOver || !m_soundManager || !m_scoreManager)
        {
            return;
        }


        PlayerInput();
    }

    private void LateUpdate() 
    {
        if(m_ghostShape)
        {
            m_ghostShape.DrawGhost(m_activeShape,m_gameBoard);
        }
    }

    void MoveRight()
    {
        m_activeShape.MoveRight();
        m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;

        if (!m_gameBoard.IsValidPosition(m_activeShape))
        {
            m_activeShape.MoveLeft();
            PlaySound(m_soundManager.m_errorSound, 0.5f);
        }
        else
        {
            PlaySound(m_soundManager.m_moveSound, 0.5f);
        }
    }

    void MoveLeft()
    {
        m_activeShape.MoveLeft();
        m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;

        if (!m_gameBoard.IsValidPosition(m_activeShape))
        {
            m_activeShape.MoveRight();
            PlaySound(m_soundManager.m_errorSound, 0.5f);
        }
        else
        {
            PlaySound(m_soundManager.m_moveSound, 0.5f);
        }
    }

    void Rotate()
    {
        m_activeShape.RotateClockwise(m_clockwise);
        m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

        if (!m_gameBoard.IsValidPosition(m_activeShape))
        {
            m_activeShape.RotateClockwise(!m_clockwise);
            PlaySound(m_soundManager.m_errorSound, 0.5f);
        }
        else
        {
            PlaySound(m_soundManager.m_moveSound, 0.5f);
        }
    }

    void MoveDown()
    {
        m_timeToDrop = Time.time + m_dropDelayModded;
        m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;

        if (m_activeShape)
        {
            m_activeShape.MoveDown();

            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                if (m_gameBoard.IsOverLimit(m_activeShape))
                {
                    GameOver();
                }
                else
                {
                    LandShape();
                }
            }
        }
    }

    void PlayerInput()
    {
        if ((Input.GetButton("MoveRight") && Time.time > m_timeToNextKeyLeftRight) || Input.GetButtonDown("MoveRight"))
        {
           MoveRight();
        }
        else if ((Input.GetButton("MoveLeft") && Time.time > m_timeToNextKeyLeftRight) || Input.GetButtonDown("MoveLeft"))
        {
            MoveLeft();
        }
        else if (Input.GetButtonDown("Rotate") && Time.time > m_timeToNextKeyRotate)
        {
            Rotate();
        }
        else if((Input.GetButton("MoveDown") && (Time.time > m_timeToNextKeyDown)) || (Time.time > m_timeToDrop))
        {
           MoveDown();
        }
        else if( (m_swipeDirection == Direction.right && Time.time > m_timeToNextSwipe) || (m_dragDirection == Direction.right) && Time.time > m_timeToNextDrag)
        {
            MoveRight();
            m_timeToNextDrag = Time.time + m_minTimeToDrag;
            m_timeToNextSwipe = Time.time + m_minTimeToSwipe;
        }
        else if ((m_swipeDirection == Direction.left && Time.time > m_timeToNextSwipe) || (m_dragDirection == Direction.left) && Time.time > m_timeToNextDrag)
        {
            MoveLeft();
            m_timeToNextDrag = Time.time + m_minTimeToDrag;
            m_timeToNextSwipe = Time.time + m_minTimeToSwipe;
        }
        else if((m_swipeDirection == Direction.up && Time.time > m_timeToNextSwipe) || (m_didTap))
        {
            Rotate();
            m_timeToNextSwipe = Time.time + m_minTimeToSwipe;
            m_didTap = false;
        }
        else if(m_dragDirection == Direction.down && Time.time > m_timeToNextDrag)
        {
            MoveDown();
        }
        else if(Input.GetButtonDown("ToggleRotation"))
        {
            ToggleRotateDirection();
        }
        else if(Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }
        else if(Input.GetButtonDown("Hold"))
        {
            Hold();
        }

        m_dragDirection = Direction.none;
        m_swipeDirection = Direction.none;
        m_didTap = false;
    }

    void LandShape()
    {
        if(m_activeShape)
        {
        m_activeShape.MoveUp();
        m_gameBoard.StoreShapeInGrid(m_activeShape);
        m_activeShape.LandShapeFX();
        if(m_ghostShape)
        {
            m_ghostShape.Reset();
        }
        if(m_holder)
        {
            m_holder.m_canRelease = true;
        }
        m_activeShape = m_spawner.SpawnShape();

        m_timeToNextKeyLeftRight = Time.time;
        m_timeToNextKeyDown = Time.time;
        m_timeToNextKeyRotate = Time.time;

        m_gameBoard.StartCoroutine("ClearAllRows");

        PlaySound(m_soundManager.m_dropSound, 0.65f);

        if(m_gameBoard.m_completedRows > 0)
        {
            m_scoreManager.ScoreLines(m_gameBoard.m_completedRows);

            if(m_scoreManager.m_didLevelUp)
            {
                PlaySound(m_soundManager.m_levelUpVocalClip, 0.75f);
                m_dropDelayModded =Mathf.Clamp(m_dropDelay - (((float) m_scoreManager.m_level -1) * 0.1f), 0.05f, 1f);
            }
            else
            {
                if (m_gameBoard.m_completedRows > 1)
                {
                    AudioClip randomVocal = m_soundManager.GetRandomClip(m_soundManager.m_vocalClips);
                    PlaySound(randomVocal, 0.35f);
                }
            }
            PlaySound(m_soundManager.m_clearRowSounds, 0.75f);
        }
        }
    }

    void GameOver()
    {
        m_activeShape.MoveUp();
        if(m_gameOverFX)
        {
            m_gameOverFX.Play();
        }
        PlaySound(m_soundManager.m_gameOverSound, 1f);
        PlaySound(m_soundManager.m_gameoverVocalClip,0.5f);
        m_gameOver = true;
        if(m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(true);
        }
    }

    void PlaySound(AudioClip clip, float volMultiplier)
    {
        if (m_soundManager.m_fxEnabled && clip)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position,Mathf.Clamp(m_soundManager.m_fxVolume * volMultiplier, 0.05f, 1f) );
        }
    }

    public void ToggleRotateDirection()
    {
        m_clockwise = !m_clockwise;
        if(m_rotateIconToggle)
        {
            m_rotateIconToggle.ToggleIcon(m_clockwise);
        }
    }

    public void TogglePause()
    {
        if(m_gameOver)
        {
            return;
        }

        m_isPaused = !m_isPaused;

        if(m_pausePanel)
        {
            m_pausePanel.SetActive(m_isPaused);

            if(m_soundManager)
            {
                m_soundManager.m_musicSource.volume = (m_isPaused) ? m_soundManager.m_musicVolume * 0.25f : m_soundManager.m_musicVolume;
            }

            Time.timeScale = (m_isPaused) ? 0 : 1;
        }
    }

    public void Hold()
    {
        if(!m_holder)
        {
            
        }
        if(!m_holder.m_heldShape)
        {
            m_holder.Catch(m_activeShape);
            m_activeShape = m_spawner.SpawnShape();
            PlaySound(m_soundManager.m_holdSound,0.5f);
        }
        else if(m_holder.m_canRelease)
        {
            Shape tmp = m_activeShape;
            m_activeShape = m_holder.Release();
            m_activeShape.transform.position = m_spawner.transform.position;
            m_holder.Catch(tmp);
            PlaySound(m_soundManager.m_holdSound, 0.5f);
        }
        else
        {
            PlaySound(m_soundManager.m_errorSound, 0.5f);
        }
        if(m_ghostShape)
        {
            m_ghostShape.Reset();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    void DragHandler(Vector2 dragMovement)
    {
        m_dragDirection = GetDirection(dragMovement);
    }

    void SwipeHandler(Vector2 swipeMovement)
    {
        m_swipeDirection = GetDirection(swipeMovement);
    }

    void TapHandler(Vector2 tapMovement)
    {
       m_didTap = true;
    }


    Direction GetDirection(Vector2 swipeMovement)
    {
        Direction swipeDir = Direction.none;

        if(Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
        {
            swipeDir = (swipeMovement.x >= 0) ? Direction.right: Direction.left;
        }
        else
        {
            swipeDir = (swipeMovement.y >= 0) ? Direction.up : Direction.down;
        }

        return swipeDir;
    }
}
