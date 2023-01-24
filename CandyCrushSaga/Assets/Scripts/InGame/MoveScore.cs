using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveScore : MonoBehaviour
{
    int moveCount;

    Text moveText;

    public int MoveCount
    {
        get { return moveCount; }
        set 
        { 
            moveCount = value;
            moveText.text = moveCount.ToString();
        }
    }

    void Start()
    {
        moveText = gameObject.transform.GetChild(1).GetComponent<Text>();

        moveCount = 45;
        moveText.text = moveCount.ToString();
    }

    void Update()
    {
        
    }
}
