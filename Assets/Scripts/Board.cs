using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Board : MonoBehaviour
{
    [SerializeField]
    Transform slots;
    [SerializeField]
    Coins[] coins;
    [SerializeField]
    GameOver gameOver;
    [SerializeField]
    GameObject turnR,turnG;
    public TextMeshProUGUI txtScoreR, txtScoreG, txtTimerR, txtTimerG,txtSkipR,txtSkipG;
    public GameObject handPointer;

    List<Circle> circles;
    Coins selectedCoin;
    int scoreR, scoreG,timer,skipR=3,skipG=3;
    char turn;
    bool secondStrike,tutEnd;

    const int turnTime = 20;

    private void Start()
    {
        circles = new List<Circle>();
        for(int i = 0; i < slots.childCount; i++)
        {
            Transform slot = slots.GetChild(i);
            Circle circle=slot.gameObject.AddComponent<Circle>();
            circles.Add(circle);
            circle.x = Convert.ToInt32(slot.name);
            circle.y = 0;
            circle.board = this;
            for(int j = 0; j < slot.childCount; j++)
            {
                Transform subSlot = slot.GetChild(j);
                Circle subCircle = subSlot.gameObject.AddComponent<Circle>();
                circles.Add(subCircle);
                subCircle.x = circle.x;
                subCircle.y = Convert.ToInt32(subSlot.name);
                subCircle.board = this;
            }
        }

        foreach (Coins coin in coins)
            coin.board = this;
        if (UnityEngine.Random.Range(0, 2) == 0)
            turn = 'r';
        else
            turn = 'g';
        SetTurn();

        timer = turnTime;
        StartCoroutine(Timer());
        if (turn == 'r')
            StartCoroutine(ShowTut(0,-1,1,true));
        else
            StartCoroutine(ShowTut(0, 1, 1,true));
    }

    IEnumerator ShowTut(int x , int y, float time,bool isCoin)
    {
        handPointer.SetActive(false);
        yield return new WaitForSeconds(time);
        if (isCoin)
        {
            Coins c = GetCoinFrom(x, y);
            handPointer.transform.parent = c.transform;
        }else
        {
            Circle c = GetCircleFrom(x, y);
            handPointer.transform.parent = c.transform;
        }
        handPointer.transform.localPosition = Vector3.zero;
        handPointer.SetActive(true);
    }

    IEnumerator Timer()
    {
        if (!tutEnd)
        {
            txtTimerR.text = null;
            txtTimerG.text = null;
            yield return new WaitUntil(() => tutEnd);
        }

        if (turn == 'r')
            txtTimerR.text = timer + " sec";
        else
            txtTimerG.text = timer + " sec";

        yield return new WaitForSeconds(1);
        timer--;

        if(timer==0)
        {
            secondStrike = false;
            foreach (Circle c in circles)
                c.Deactivate();
            timer = turnTime;
            if (turn == 'r')
            {
                turn = 'g';
                skipR--;
                txtSkipR.text = skipR + " Skip Left";
                if (skipR == 0)
                    gameOver.Fail('r');
                txtTimerR.text = "0 sec";
                txtTimerG.text = timer + " sec";
            }
            else
            {
                turn = 'r';
                skipG--;
                txtSkipG.text = skipG + " Skip Left";
                if (skipG == 0)
                    gameOver.Fail('g');
                txtTimerG.text = "0 sec";
                txtTimerR.text = timer + " sec";
            }
            SetTurn(); 
        }
        if(skipR>0&&skipG>0)
            StartCoroutine(Timer());
    }

    public void CoinClicked(Coins coin)
    {
        if (coin.team != turn || secondStrike) return;
        selectedCoin = coin;
        int x = coin.x;
        int y = coin.y;
        char team = coin.team;

        foreach (Circle c in circles)
            c.Deactivate();
        GetCircleFrom(x, y).Activate(team);

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                if ((Mathf.Abs(x % 2) == 1 || Mathf.Abs(y % 2) == 1) && !(Mathf.Abs(x % 2) == 1 && Mathf.Abs(y % 2) == 1))
                    if (i != 0 && j != 0)
                        continue;
                if (GetCoinFrom(x + i, y + j) == null)
                {
                    Circle circle = GetCircleFrom(x + i, y + j);
                    if (circle)
                    {
                        circle.Activate(team);
                        if (!tutEnd)
                            StartCoroutine(ShowTut(x + i, y + j,0,false));
                    }
                }
                else if(GetCoinFrom(x + i, y + j).team!=team && GetCoinFrom(x + i + i, y + j + j) == null)
                {
                    Circle circle = GetCircleFrom(x + i + i, y + j + j);
                    if (circle)
                    {
                        circle.Activate(team);
                        if (!tutEnd)
                        {
                            StartCoroutine(ShowTut(x + i + i, y + j + j, 0,false));
                            tutEnd = true;
                        }
                    }
                }
            }
        }
    }

    public void CircleClicked(Circle circle)
    {
        selectedCoin.x = circle.x;
        selectedCoin.y = circle.y;

        foreach (Circle c in circles)
            c.Deactivate();

        timer = turnTime;
        if (turn == 'r')
        {
            turn = 'g';
            txtTimerR.text = "0 sec";
            txtTimerG.text = timer + " sec";
        }
        else
        {
            turn = 'r';
            txtTimerG.text = "0 sec";
            txtTimerR.text = timer + " sec";
        }

        SetTurn();

        secondStrike = false;
        if (!tutEnd)
        {
            if (turn == 'r')
                StartCoroutine(ShowTut(0, -1, 0.5f, true));
            else
                StartCoroutine(ShowTut(0, 1, 0.5f, true));
        }
        else if(handPointer.activeInHierarchy)
        {
            handPointer.transform.parent = null;
            handPointer.SetActive(false);
        }
    }

    public void CheckSecondStrike()
    {
        int x = selectedCoin.x;
        int y = selectedCoin.y;
        char team = selectedCoin.team;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                if ((Mathf.Abs(x % 2) == 1 || Mathf.Abs(y % 2) == 1) && !(Mathf.Abs(x % 2) == 1 && Mathf.Abs(y % 2) == 1))
                    if (i != 0 && j != 0)
                        continue;
                if (GetCoinFrom(x + i, y + j) != null && GetCoinFrom(x + i, y + j).team != team && GetCoinFrom(x + i + i, y + j + j) == null)
                {
                    Circle circle = GetCircleFrom(x + i + i, y + j + j);
                    if (circle)
                    {
                        circle.Activate(team); 
                        if (turn == 'r')
                        {
                            txtTimerR.text = "0 sec";
                        }
                        else
                        {
                            txtTimerG.text = "0 sec";
                        }
                        turn = team;
                        SetTurn();
                        secondStrike = true;
                    }
                }
            }
        }
    }

    public void AddScore(char team)
    {
        if (team == 'r')
        {
            scoreG++;
            txtScoreG.text = "Score : "+scoreG;
        }
        else
        {
            scoreR++;
            txtScoreR.text = "Score : " + scoreR;
        }

        if (scoreG == 16 || scoreR == 16)
            gameOver.Fail(team);
    }

    void SetTurn()
    {
        turnG.SetActive(false);
        turnR.SetActive(false);
        if (turn == 'r')
            turnR.SetActive(true);
        else
            turnG.SetActive(true);
    }

    public Circle GetCircleFrom(int x, int y)
    {
        foreach(Circle circle in circles)
        {
            if(circle.x == x && circle.y == y)
            {
                return circle;
            }
        }
        return null;
    }
    public Coins GetCoinFrom(int x, int y)
    {
        foreach (Coins coin in coins)
        {
            if (coin!=null && coin.x == x && coin.y == y && coin.GetComponent<CircleCollider2D>().enabled)
            {
                return coin;
            }
        }
        return null;
    }

    public bool IsSelectedCoin(Coins coin)
    {
        return selectedCoin == coin;
    }
}
