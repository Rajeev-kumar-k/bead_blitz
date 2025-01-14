using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Coins : MonoBehaviour, IPointerClickHandler
{
    public char team;
    public int x, y;

    [HideInInspector]
    public Board board;

    bool strike;

    private void Start()
    {
        StartCoroutine(EnableCollider());
    }

    IEnumerator EnableCollider()
    {
        yield return new WaitUntil(() => board!=null && transform.position == board.GetCircleFrom(x, y).transform.position);
        GetComponent<CircleCollider2D>().enabled = true;
    }

    private void Update()
    {
        if (strike)
        {
            Vector3 toPos = board.txtScoreR.transform.position;
            if(team=='r')
                toPos = board.txtScoreG.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, toPos, Time.deltaTime * 8);
            if (transform.position == toPos)
            {
                board.AddScore(team);
                Destroy(gameObject);
            }
        }
        else
        {
            Vector3 toPos = board.GetCircleFrom(x, y).transform.position;
            transform.position = Vector3.MoveTowards(transform.position, toPos, Time.deltaTime * 4);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        board.CoinClicked(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!board.IsSelectedCoin(this))
        {
            GetComponent<CircleCollider2D>().enabled = false;
            strike = true;
            board.CheckSecondStrike();
        }
    }
}