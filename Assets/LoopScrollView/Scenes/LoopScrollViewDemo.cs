using System.Collections;
using System.Collections.Generic;
using CircularScrollView;
using UnityEngine;
using LoopScrollViewNamespace;
using UnityEngine.UI;

public class LoopScrollViewDemo : MonoBehaviour
{
    public LoopScrollView HorizontalScroll;
    public LoopScrollView VerticalScroll;
    
    public ExpandLoopScrollView expandScroll;

    public void Start()
    {
        StartScrollView();
    }
    
    private void StartScrollView()
    {
        HorizontalScroll.Init(NormalCallBack);
        HorizontalScroll.ShowList(50);
        
        VerticalScroll.Init(NormalCallBack);
        VerticalScroll.ShowList(50);
        
        expandScroll.Init(ExpandCallBack);
        expandScroll.ShowList("3|2|5|8");
    }
    
    private void NormalCallBack(GameObject cell, int index)
    {
        cell.transform.Find("Text1").GetComponent<Text>().text = index.ToString();
        cell.transform.Find("Text2").GetComponent<Text>().text = index.ToString();
    }
    
    private void ExpandCallBack(GameObject cell, GameObject childCell, int index, int childIndex)
    {
        cell.transform.Find("Text1").GetComponent<Text>().text = "Click Me : " + index.ToString();
        if (childCell != null)
        {
            childCell.transform.Find("Text1").GetComponent<Text>().text = childIndex.ToString();
        }
    }
}
