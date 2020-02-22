using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public Transform chainPoint;
    public Transform[] plane;

    public void SetPipe()
    {
        if (GameManager.instance.listPipe.Count < 6)
            return;
        for (int i = 0; i < GameManager.instance.lv; i++)
        {
            int rd = Random.Range(0, plane.Length);
            plane[rd].GetComponent<Renderer>().material.color = new Color32(255, 0, 0, 255);
            plane[rd].tag = "Fail";
        }
    }
}
