using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPlayerRank : MonoBehaviour
{
    public Text numberText;
    public Text nameText;
    public Text scoreText;

    public void SetItem(int count, string playerName, int score)
    {
        numberText.text = count.ToString();
        nameText.text = playerName;
        scoreText.text = score.ToString();
    }
}
