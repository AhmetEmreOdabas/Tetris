using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text m_lineText;
    public Text m_levelText;
    public Text m_scoreText;

    public ParticlePlayer m_levelUpFX;

   int m_score;
   int m_lines;
   public int m_level;

   public int m_linesPerLevel = 5;
   public bool m_didLevelUp = false;

   const int m_minLines = 1;
   const int m_maxLines = 4;

   private void Start() 
   {
       Reset();
   }

   public void ScoreLines(int n)
   {
       m_didLevelUp = false;

       n = Mathf.Clamp(n,m_minLines,m_maxLines);

       switch (n)
       {
           case 1:
                 m_score += 40 * m_level;
                break;
            case 2:
                m_score += 100 * m_level;
                break;
            case 3:
                m_score += 300 * m_level;
                break;
            case 4:
                m_score += 1200 * m_level;
                break;
       }

       m_lines -= n;
       if(m_lines <= 0)
       {
           LevelUp();
       }

       UpdateUIText();
   }

   public void Reset()
   {
       m_level = 1;
       m_lines = m_linesPerLevel * m_level;
       UpdateUIText();
   }

   void UpdateUIText()
   {
         if(m_lineText)
        {
           m_lineText.text = m_lines.ToString();
        }
        if (m_levelText)
        {
            m_levelText.text = m_level.ToString();
        }
        if (m_scoreText)
        {
            m_scoreText.text = PadZero(m_score,6);
        }
   }

   string PadZero(int n, int padDigits)
   {
       string nStr = n.ToString();

       while (nStr.Length < padDigits)
       {
            nStr = "0" + nStr;   
       }

       return nStr;
   }

   public void LevelUp()
   {
       m_level++;
       m_lines = m_linesPerLevel * m_level;
       m_didLevelUp = true;

       if(m_levelUpFX)
       {
           m_levelUpFX.Play();
       }
   }
}
