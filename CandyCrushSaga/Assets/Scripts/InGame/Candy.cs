using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.ParticleSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

using Random = UnityEngine.Random;

enum CANDYTYPE
{
    RED,
    BLUE,
    GREEN,
    PURPLE,
    YELLOW,
    ORANGE
}

public class Candy : MonoBehaviour
{
    int x;
    int y;

    [SerializeField]
    private Sprite[] sprites = new Sprite[5];

    [SerializeField]
    private ParticleSystem particle;

    private SpriteRenderer spriteRenderer;
    StateComponent stateComponent;

    CANDYTYPE candyType = CANDYTYPE.RED;

    float deathTime = 0.0f;

    Vector3 fallStartPos = new Vector3(0.0f, 0.0f);
    Vector3 fallPos = new Vector3(0.0f, 0.0f);

    float speed;

    bool isSelect;

    bool isStripe;

    //true ¼¼·Î, false °¡·Î
    bool direction;

    void Start()
    {
        particle = Instantiate(particle);
        particle.Stop();
        particle.gameObject.SetActive(false);

        transform.localScale = new Vector3(0.75f, 0.75f, 1.0f);

        spriteRenderer = GetComponent<SpriteRenderer>();

        candyType = (CANDYTYPE)Random.RandomRange(0, 6);
        spriteRenderer.sprite = sprites[(int)candyType];

        stateComponent = new StateComponent();
        stateComponent.CreateState("Idle", IdleUpdate);
        stateComponent.CreateState("Destroy", DestroyUpdate, DestroyStart);
        stateComponent.CreateState("Fall", FallUpdate);
        stateComponent.ChangeState("Idle");
    }

    void Update()
    {
        stateComponent.StateUpdate();

        Debug.Log(stateComponent.CurStateInfo.StateName);
    }

    void IdleUpdate()
    {

    }

    void DestroyStart()
    {
        
    }

    void DestroyUpdate()
    {
        GamePlayManager.instance.CandyList[x][y] = null;

        if (null != Score.instance)
        {
            Score.instance.PlusScore(100);
        }

        if (null != SoundManager.instance)
        {
            SoundManager.instance.LandCandy();
        }

        particle.gameObject.SetActive(true);
        particle.transform.position = transform.position;
        particle.Play();

        Destroy(gameObject);
    }

    void FallUpdate()
    {
        speed += Time.deltaTime * 1.5f;
        gameObject.transform.position = Vector3.Lerp(fallStartPos, fallPos, speed);

        if (1.0f < speed)
        {
            speed = 0.0f;
            stateComponent.ChangeState("Idle");
        }
    }

    public bool IsSelect
    {
        get { return isSelect; }
        set { isSelect = value; }
    }

    public bool IsStripe
    {
        get { return isStripe; }
        set { isStripe = value; }
    }
    public bool Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    public int CandyX
    {
        get { return x; }
        set { x = value; }
    }

    public int CandyY
    {
        get { return y; }
        set { y = value; }
    }

    public void BFSCandy()
    {
        int maxX = GamePlayManager.instance.CandyList.Count;
        int maxY = GamePlayManager.instance.CandyList[x].Count;

        int[] Dir = new int[2];
        Dir[0] = 1;
        Dir[1] = -1;

        Queue<Candy> candies = new Queue<Candy>();
        List<Candy> destroyCandies = new List<Candy>();

        candies.Enqueue(this);

        while (0 != candies.Count)
        {
            Candy curCandy = candies.Dequeue();

            int indexX = curCandy.CandyX;

            for (int i = 0; i < 2; ++i)
            {
                indexX += Dir[i];

                if (0 > indexX || maxX - 1 < indexX)
                {
                    continue;
                }

                Candy candy = GamePlayManager.instance.CandyList[indexX][y];

                if (null == candy)
                {
                    continue;
                }

                if (candyType == candy.candyType
                    && "Idle" == candy.stateComponent.CurStateInfo.StateName)
                {
                    candy.stateComponent.ChangeState("Destroy");
                    candies.Enqueue(candy);
                    destroyCandies.Add(candy);
                }
            }
        }

        if (3 > destroyCandies.Count)
        {
            for (int i = 0; i < destroyCandies.Count; i++)
            {
                destroyCandies[i].stateComponent.ChangeState("Idle");
            }
        }

        else if (3 <= destroyCandies.Count)
        {
            GamePlayManager.instance.IsDestroy = true;

            for (int i = 0; i < destroyCandies.Count; ++i)
            {
                if (true == destroyCandies[i].isStripe)
                {
                    if (true == destroyCandies[i].direction)
                    {
                        GamePlayManager.instance.LengthDestroy(destroyCandies[i]);
                    }

                    else if (false == destroyCandies[i].direction)
                    {
                        GamePlayManager.instance.WidthDestroy(destroyCandies[i]);
                    }

                    break;
                }

                if (3 < destroyCandies.Count)
                {
                    if (true == destroyCandies[i].isSelect)
                    {
                        destroyCandies[i].stateComponent.ChangeState("Idle");
                        destroyCandies[i].ChangeWidthStripeCandy();

                        break;
                    }

                    else if (destroyCandies.Count - 1 == i)
                    {
                        destroyCandies[i].stateComponent.ChangeState("Idle");
                        destroyCandies[i].ChangeWidthStripeCandy();
                    }
                }
            }
        }

        destroyCandies.Clear();

        candies.Enqueue(this);

        while (0 != candies.Count)
        {
            Candy curCandy = candies.Dequeue();

            int indexY = curCandy.CandyY;

            for (int i = 0; i < 2; i++)
            {
                indexY += Dir[i];

                if (0 > indexY || maxY - 1 < indexY)
                {
                    continue;
                }

                Candy candy = GamePlayManager.instance.CandyList[x][indexY];

                if (null == candy)
                {
                    continue;
                }

                if (candyType == candy.candyType
                    && "Idle" == candy.stateComponent.CurStateInfo.StateName)
                {
                    candy.stateComponent.ChangeState("Destroy");
                    candies.Enqueue(candy);
                    destroyCandies.Add(candy);
                }
            }
        }

        if (3 > destroyCandies.Count)
        {
            for (int i = 0; i < destroyCandies.Count; i++)
            {
                destroyCandies[i].stateComponent.ChangeState("Idle");
            }
        }

        else if (3 <= destroyCandies.Count)
        {
            GamePlayManager.instance.IsDestroy = true;

            for (int i = 0; i < destroyCandies.Count; ++i)
            {
                if (true == destroyCandies[i].isStripe)
                {
                    if (true == destroyCandies[i].direction)
                    {
                        GamePlayManager.instance.LengthDestroy(destroyCandies[i]);
                    }

                    else if (false == destroyCandies[i].direction)
                    {
                        GamePlayManager.instance.WidthDestroy(destroyCandies[i]);
                    }

                    break;
                }

                if (3 < destroyCandies.Count)
                {
                    if (true == destroyCandies[i].isSelect)
                    {
                        destroyCandies[i].stateComponent.ChangeState("Idle");
                        destroyCandies[i].ChangeLengthStripeCandy();

                        break;
                    }

                    else if (destroyCandies.Count - 1 == i)
                    {
                        destroyCandies[i].stateComponent.ChangeState("Idle");
                        destroyCandies[i].ChangeLengthStripeCandy();
                    }
                }
            }
        }

        isSelect = false;
        destroyCandies.Clear();
    }

    public void FallCandy(Vector3 _FallPos)
    {
        fallStartPos = gameObject.transform.position;
        fallPos = _FallPos;

        stateComponent.ChangeState("Fall");
    }

    //ÁÙ¹«´Ì Äµµð
    void ChangeLengthStripeCandy()
    {
        SpriteRenderer candySprite = GetComponent<SpriteRenderer>();
        isStripe = true;
        direction = true;

        if (null != candySprite)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Textrue/Candies");

            switch (candyType)
            {
                case CANDYTYPE.RED:
                    candySprite.sprite = sprites[71];
                    break;
                case CANDYTYPE.BLUE:
                    candySprite.sprite = sprites[69];
                    break;
                case CANDYTYPE.GREEN:
                    candySprite.sprite = sprites[79];
                    break;
                case CANDYTYPE.PURPLE:
                    candySprite.sprite = sprites[61];
                    break;
                case CANDYTYPE.YELLOW:
                    candySprite.sprite = sprites[15];
                    break;
                case CANDYTYPE.ORANGE:
                    candySprite.sprite = sprites[84];
                    break;
            }
        }
    }

    void ChangeWidthStripeCandy()
    {
        SpriteRenderer candySprite = GetComponent<SpriteRenderer>();
        isStripe = true;
        direction = false;

        if (null != candySprite)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Textrue/Candies");

            switch (candyType)
            {
                case CANDYTYPE.RED:
                    candySprite.sprite = sprites[80];
                    break;
                case CANDYTYPE.BLUE:
                    candySprite.sprite = sprites[70];
                    break;
                case CANDYTYPE.GREEN:
                    candySprite.sprite = sprites[81];
                    break;
                case CANDYTYPE.PURPLE:
                    candySprite.sprite = sprites[68];
                    break;
                case CANDYTYPE.YELLOW:
                    candySprite.sprite = sprites[74];
                    break;
                case CANDYTYPE.ORANGE:
                    candySprite.sprite = sprites[82];
                    break;
            }
        }
    }

    public void Destroy()
    {
        stateComponent.ChangeState("Destroy");
    }
}