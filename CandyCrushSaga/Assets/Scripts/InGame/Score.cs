using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static Score instance = null;

    int score;

    Text scoreText;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            if (instance != this)
            {
                Destroy(this);
            }
        }

        scoreText = gameObject.transform.GetChild(1).GetComponent<Text>();
    }

    void Update()
    {
        
    }

    public void PlusScore(int _Value)
    {
        score += _Value;
        scoreText.text = score.ToString();
    }
}
