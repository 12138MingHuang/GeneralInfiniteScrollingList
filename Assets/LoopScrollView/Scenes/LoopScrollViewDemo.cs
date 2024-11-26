using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LoopScrollViewNamespace;
using UnityEngine.UI;

public class LoopScrollViewDemo : MonoBehaviour
{
    public LoopScrollView HorizontalScroll;

    public void Start()
    {
        StartScrollView();
    }
    
    private void StartScrollView()
    {
        HorizontalScroll.Init(NormalCallBack);
        HorizontalScroll.ShowList(50);
    }
    
    private void NormalCallBack(GameObject cell, int index)
    {
        cell.transform.Find("Text1").GetComponent<Text>().text = index.ToString();
        cell.transform.Find("Text2").GetComponent<Text>().text = index.ToString();
    }
}
