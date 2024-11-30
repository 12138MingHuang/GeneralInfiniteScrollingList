using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LoopScrollViewNamespace;
using UnityEngine.UI;

public class LoopScrollViewDemo : MonoBehaviour
{
    public LoopScrollView HorizontalScroll;
    public LoopScrollView VerticalScroll;
    
    public ExpandLoopScrollView VerticalExpandScroll;
    public ExpandLoopScrollView HorizontalExpandScroll;
    
    public ExpandTipsLoopScrollView VerticalExpandTipsScroll;
    public GameObject expandTips;
    
    public FlipPageLoopScrollView HorizontalFlipPageScroll;

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
        
        VerticalExpandScroll.Init(ExpandCallBack);
        VerticalExpandScroll.ShowList("3|2|5|8");
        
        HorizontalExpandScroll.Init(ExpandCallBack);
        HorizontalExpandScroll.ShowList("3|2|50|8");
        
        VerticalExpandTipsScroll.Init(ExpandTipsCallBack, OnClickExpandTipsCallBack);
        VerticalExpandTipsScroll.ShowList(30);
        
        HorizontalFlipPageScroll.Init(FlipPageCallBack);
        HorizontalFlipPageScroll.ShowList(10);
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
    
    private void ExpandTipsCallBack(GameObject cell, int index)
    {
        cell.transform.Find("Text1").GetComponent<Text>().text = "Click Me : " + index.ToString();
    }

    private void OnClickExpandTipsCallBack(GameObject cell, int index)
    {
        expandTips.transform.Find("Text").GetComponent<Text>().text = string.Format("我是{0}号", index);
    }
    
    private void FlipPageCallBack(GameObject cell, int index)
    {
        cell.transform.Find("Text1").GetComponent<Text>().text = "Drag Me : " + index.ToString();
    }
}
