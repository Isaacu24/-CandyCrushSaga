using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.Playables;
using UnityEngine.UI;

enum PLAYSTATE
{
    REST,
    DESTROY,
    FALL,
    FILLUP
}

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager instance = null;

    public Candy candy;

    private List<List<Candy>> candies = new List<List<Candy>>();

    [SerializeField]
    MoveScore moveScore;

    Vector3 mousePosition;

    Camera camera;

    Candy selectCandy;

    int minY;

    bool isDestroy;
    float t;

    int combo;

    PLAYSTATE playState;

    [SerializeField]
    GameObject[] comboPanels = new GameObject[3];

    private void Awake()
    {
        //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
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
    }

    void Start()
    {
        playState = PLAYSTATE.DESTROY;

        camera = GameObject.Find("Camera").GetComponent<Camera>();

        minY = 10;

        for (int x = 0; x < 10; x++)
        {
            List<Candy> Arr = new List<Candy>();
            candies.Add(Arr);

            for (int y = 0; y < 20; y++)
            {
                Candy iCandy = Instantiate(candy);
                iCandy.transform.position = new Vector3(-0.1f, 11.0f);
                //iCandy.transform.position = new Vector3(-0.1f, 3.5f);
                iCandy.transform.position += new Vector3(x * 0.75f, -y * 0.75f);

                iCandy.CandyX = x;
                iCandy.CandyY = y;

                candies[x].Add(iCandy);

                iCandy.gameObject.transform.SetParent(this.transform);
            }

            for (int i = 0; i < 3; i++)
            {
                Image img = comboPanels[i].GetComponent<Image>();
                Color color = img.color;
                color.a = 0.0f;
                img.color = color;

                comboPanels[i].SetActive(false);
            }
        }
    }


    void Update()
    {
        switch (playState)
        {
            case PLAYSTATE.REST:
                if (0 >= moveScore.MoveCount)
                {
                    //게임 종료
                    return;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    SolveCandy();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    MoveCandy();
                }
                break;
            case PLAYSTATE.DESTROY:
                t += Time.deltaTime;

                if (0.5f < t)
                {
                    t = 0.0f;
                    DestroyCandy();
                }
                break;
            case PLAYSTATE.FALL:
                FallCandy();
                break;
            case PLAYSTATE.FILLUP:
                t += Time.deltaTime;

                if (0.5f < t)
                {
                    t = 0.0f;
                    FillUpCandy();
                }
                break;
            //case PLAYSTATE.COMBO:
            //    break;
        }
    }

    public List<List<Candy>> CandyList
    {
        get { return candies; }
        set { candies = value; }
    }

    public bool IsDestroy
    {
        get { return isDestroy; }
        set { isDestroy = value; }
    }

    void DestroyCandy()
    {
        isDestroy = false;

        for (int x = 0; x < candies.Count; ++x)
        {
            for (int y = 0; y < candies[x].Count; ++y)
            {
                if (minY > y)
                {
                    continue;
                }

                if (null != candies[x][y])
                {
                    candies[x][y].BFSCandy();
                }
            }
        }

        if (false == isDestroy)
        {
            playState = PLAYSTATE.REST;
        }

        else
        {
            playState = PLAYSTATE.FALL;
            ++combo;
        }
    }

    //가로
    public void WidthDestroy(Candy _Candy)
    {
        _Candy.IsStripe = false;

        for (int x = 0; x < candies.Count; ++x)
        {
            if (null != candies[x][_Candy.CandyY])
            {
                if (true == candies[x][_Candy.CandyY].IsStripe
                    && candies[x][_Candy.CandyY] != _Candy)
                {
                    if (true == candies[x][_Candy.CandyY].Direction)
                    {
                        LengthDestroy(candies[x][_Candy.CandyY]);
                    }

                    if (false == candies[x][_Candy.CandyY].Direction)
                    {
                        WidthDestroy(candies[x][_Candy.CandyY]);
                    }
                }

                candies[x][_Candy.CandyY].Destroy();
            }
        }
    }

    //세로
    public void LengthDestroy(Candy _Candy)
    {
        _Candy.IsStripe = false;

        for (int y = 0; y < candies[_Candy.CandyX].Count; ++y)
        {
            if (minY > y)
            {
                continue;
            }

            if (null != candies[_Candy.CandyX][y])
            {
                if (true == candies[_Candy.CandyX][y].IsStripe
                    && candies[_Candy.CandyX][y] != _Candy)
                {
                    if (false == candies[_Candy.CandyX][y].Direction)
                    {
                        WidthDestroy(candies[_Candy.CandyX][y]);
                    }

                    if (true == candies[_Candy.CandyX][y].Direction)
                    {
                        LengthDestroy(candies[_Candy.CandyX][y]);
                    }
                }

                candies[_Candy.CandyX][y].Destroy();
            }
        }
    }

    void FallCandy()
    {
        for (int x = candies.Count - 1; x >= 0; --x)
        {
            for (int y = candies[x].Count - 1; y >= 0; --y)
            {
                if (null == candies[x][y])
                {
                    continue;
                }

                int maxY = candies[x].Count;
                int underY = y + 1;

                Vector3 pos = candies[x][y].transform.position;

                bool isFall = false;

                while (true)
                {
                    if (underY < maxY
                        && null == candies[x][underY])
                    {
                        isFall = true;
                        pos += new Vector3(0.0f, -0.75f);

                        ++underY;
                    }

                    else
                    {
                        break;
                    }
                }

                if (true == isFall)
                {
                    //Debug.Log("x: " + x);
                    //Debug.Log("y: " + underY);

                    candies[x][y].FallCandy(pos);
                    candies[x][underY - 1] = candies[x][y];
                    candies[x][underY - 1].CandyX = x;
                    candies[x][underY - 1].CandyY = underY - 1;
                    candies[x][y] = null;
                }
            }
        }

        playState = PLAYSTATE.FILLUP;
    }

    void FillUpCandy()
    {
        for (int x = 0; x < candies.Count; ++x)
        {
            for (int y = 0; y < candies[x].Count; ++y)
            {
                if (null == candies[x][y])
                {
                    candies[x][y] = Instantiate(candy);
                    Candy nullCandy = candies[x][y];

                    nullCandy.transform.position = new Vector3(-0.1f, 11.0f);
                    nullCandy.transform.position += new Vector3(x * 0.75f, -y * 0.75f);

                    nullCandy.CandyX = x;
                    nullCandy.CandyY = y;

                    nullCandy.gameObject.transform.SetParent(this.transform);

                }
            }
        }

        playState = PLAYSTATE.DESTROY;
    }

    void SolveCandy()
    {
        mousePosition = Input.mousePosition;
        mousePosition = camera.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0.0f;

        for (int x = 0; x < candies.Count; ++x)
        {
            for (int y = 0; y < candies[x].Count; ++y)
            {
                if (null == candies[x][y])
                {
                    continue;
                }

                Transform candyTr = candies[x][y].transform;

                Vector3 range = candyTr.position + new Vector3(0.5f, 0.5f);
                Vector3 nRange = candyTr.position + new Vector3(-0.5f, -0.5f);

                if (false == range.x > mousePosition.x
                    || false == nRange.x < mousePosition.x)
                {
                    continue;
                }

                if (false == range.y > mousePosition.y
                    || false == nRange.y < mousePosition.y)
                {
                    continue;
                }

                selectCandy = candies[x][y];

                Debug.Log("x: " + selectCandy.CandyX);
                Debug.Log("y: " + selectCandy.CandyY);
            }
        }
    }

    void MoveCandy()
    {
        if (null == selectCandy)
        {
            return;
        }

        Vector3 curMousePosition = Input.mousePosition;
        curMousePosition = camera.ScreenToWorldPoint(curMousePosition);
        curMousePosition.z = 0.0f;

        Vector3 mouseDir = curMousePosition - mousePosition; 

        selectCandy.IsSelect = true;

        if (0.0f < mouseDir.x
            && mouseDir.y < mouseDir.x) //오른쪽 
        {
            if (candies.Count - 1 > selectCandy.CandyX + 1)
            {
                SwapCandy(selectCandy, candies[selectCandy.CandyX + 1][selectCandy.CandyY]);
            }
        }

        else if (0.0f >= mouseDir.x
            && mouseDir.y > mouseDir.x) //왼쪽 
        {
            if (0 < selectCandy.CandyX - 1)
            {
                SwapCandy(selectCandy, candies[selectCandy.CandyX - 1][selectCandy.CandyY]);
            }
        }

        //Y는 아래가 양의 방향
        else if (0.0f < mouseDir.y
            && mouseDir.x < mouseDir.y) //위쪽 
        {
            if (0 < selectCandy.CandyY - 1)
            {
                SwapCandy(selectCandy, candies[selectCandy.CandyX][selectCandy.CandyY - 1]);
            }
        }

        else if (0.0f >= mouseDir.y
            && mouseDir.x > mouseDir.y) //아래쪽 
        {
            if (candies[selectCandy.CandyX].Count - 1 > selectCandy.CandyY + 1)
            {
                SwapCandy(selectCandy, candies[selectCandy.CandyX][selectCandy.CandyY + 1]);
            }
        }
    }

    void SwapCandy(Candy _LeftCandy, Candy _RightCandy)
    {
        _RightCandy.IsSelect = true;

        if (null == _LeftCandy
            || null == _RightCandy)
        {
            return;
        }

        --moveScore.MoveCount;

        int tempX = _RightCandy.CandyX;
        int tempY = _RightCandy.CandyY;

        Vector3 tempPos = _RightCandy.transform.position;

        candies[_LeftCandy.CandyX][_LeftCandy.CandyY] = _RightCandy;
        candies[_RightCandy.CandyX][_RightCandy.CandyY] = _LeftCandy;

        //오른쪽에서 왼쪽
        _RightCandy.CandyX = _LeftCandy.CandyX;
        _RightCandy.CandyY = _LeftCandy.CandyY;

        //왼쪽에서 오른쪽 
        _LeftCandy.CandyX = tempX;
        _LeftCandy.CandyY = tempY;

        _RightCandy.transform.position =  _LeftCandy.transform.localPosition;
        _LeftCandy.transform.position = tempPos;

        playState = PLAYSTATE.DESTROY;
    }
}
