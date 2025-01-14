using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Circle : MonoBehaviour, IPointerClickHandler
{
    public int x, y;

    [HideInInspector]
    public Board board;

    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void Activate(char team)
    {
        Color color;
        if (team == 'r')
            color = Color.red;
        else
            color = Color.green;
        color.a = 0.5f;
        image.color = color;
    }

    public void Deactivate()
    {
        Color color=Color.green;
        color.a = 0;
        image.color = color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(image.color.a>0)
            board.CircleClicked(this);
    }
}