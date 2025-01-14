using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI txtR, txtG;

    public void Fail(char team)
    {
        gameObject.SetActive(true);
        if (team == 'g')
        {
            txtR.text = "You Won";
            txtG.text = "You Loss";
        }
        else
        {
            txtR.text = "You Loss";
            txtG.text = "You Won";
        }
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
    }
}
