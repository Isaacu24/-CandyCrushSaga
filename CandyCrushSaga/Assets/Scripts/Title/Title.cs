using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    public GameObject sign;
    public Image button;

    bool isDown = false;

    void Start()
    {
        button.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    void Update()
    {
        if (1.0f > button.color.a)
        {
            Color color = button.color;
            color.a += Time.deltaTime * 0.5f;
            button.color = color;
        }

        if (false == isDown)
        {
            sign.transform.localPosition += Vector3.down * 300.0f * Time.deltaTime;

            if (290.0f >= sign.transform.localPosition.y)
            {
                isDown = true;
            }
        }
    }
}
