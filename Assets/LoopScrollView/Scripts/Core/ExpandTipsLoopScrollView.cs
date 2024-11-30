using System;
using System.Collections;
using System.Collections.Generic;
using LoopScrollViewNamespace;
using UnityEngine;
using UnityEngine.UI;

namespace LoopScrollViewNamespace
{
    public class ExpandTipsLoopScrollView : LoopScrollView
    {
        public GameObject expandTips;
        public GameObject arrow;
        public float tipsSpacing;
        
        private float m_ExpandTipsHeight;
        private bool m_IsExpand;
        private string lastClickCellName = null;
        
        private Dictionary<GameObject, bool> m_isAddedListener = new Dictionary<GameObject, bool>();
        
        #region 初始化

        public override void Init(Action<GameObject, int> callBack, Action<GameObject, int> onClickCallBack)
        {
            base.Init(callBack, onClickCallBack);
            
            LoopScrollViewUtils.SetActive(expandTips, false);
            
            RectTransform rectTrans = expandTips.GetComponent<RectTransform>();
            rectTrans.pivot = new Vector2(0, 1);
            rectTrans.anchorMin = new Vector2(0, 1);
            rectTrans.anchorMax = new Vector2(0, 1);
            rectTrans.anchoredPosition3D = new Vector3(0, 0, 0);
            rectTrans.sizeDelta = new Vector2(contentWidth, rectTrans.sizeDelta.y);
            
            m_ExpandTipsHeight = expandTips.GetComponent<RectTransform>().rect.height;

            if (arrow != null)
            {
                RectTransform arrowRectTrans = arrow.GetComponent<RectTransform>();
                arrowRectTrans.pivot = new Vector2(0.5f, 0.5f);
                arrowRectTrans.anchorMin = new Vector2(0f, 0.5f);
                arrowRectTrans.anchorMax = new Vector2(0f, 0.5f);
            }
        }

        #endregion


        #region 显示列表

        /// <summary>
        /// 显示列表
        /// </summary>
        /// <param name="num">显示数量</param>
        public override void ShowList(int num)
        {
            base.ShowList(num);

            m_IsExpand = false;
            isClearList = true;
            lastClickCellName = null;
            LoopScrollViewUtils.SetActive(expandTips, false);
        }

        #endregion

        #region 事件

        public override void OnClickCell(GameObject cell)
        {
            if (lastClickCellName == null || cell.name == lastClickCellName || !m_IsExpand)
            {
                m_IsExpand = !m_IsExpand;
            }
            lastClickCellName = cell.name;
            
            float index = float.Parse(cell.name);
            int curRow = Mathf.FloorToInt(index / row) + 1;
            
            // Tips框显示
            LoopScrollViewUtils.SetActive(expandTips, m_IsExpand);
            expandTips.transform.localPosition =
                new Vector3(0, -((spacing + cellObjectHeight) * curRow + tipsSpacing), 0);
            
            //计算content尺寸
            float contentHeight = m_IsExpand ? base.contentHeight + m_ExpandTipsHeight + tipsSpacing : base.contentHeight;
            contentHeight = contentHeight < planeHeight ? planeHeight : contentHeight;
            contentRectTrans.sizeDelta = new Vector2(contentWidth, contentHeight);

            minIndex = -1;

            for (int i = 0; i < cellInfos.Length; i++)
            {
                CellInfo cellInfo = cellInfos[i];

                // Y坐标
                float pos = 0;
                // 计算每排里面的cell的坐标
                float rowPos = 0;

                pos = cellObjectHeight * Mathf.FloorToInt(i / row) + spacing * (Mathf.FloorToInt(i / row) + 1);
                rowPos = cellObjectWidth * (i % row) + spacing * (i % row);
                pos += (i / row >= curRow && m_IsExpand) ? m_ExpandTipsHeight + tipsSpacing * 2 - spacing : 0;
                cellInfo.pos =new Vector3(rowPos, -pos, 0f);

                if (IsOutRange(-pos))
                {
                    if (cellInfo.obj != null)
                    {
                        SetPoolsObj(cellInfo.obj);
                        cellInfo.obj = null;
                    }
                }
                else
                {
                    //记录显示范围中的首位index和末尾index
                    minIndex = minIndex == -1 ? i : minIndex; // 首位index
                    maxIndex = i; // 末尾index

                    GameObject cellObj = cellInfo.obj == null ? GetPoolsObj() : cellInfo.obj;
                    cellObj.GetComponent<RectTransform>().anchoredPosition3D = cellInfo.pos;
                    cellInfo.obj = cellObj;
                }
                
                cellInfos[i] = cellInfo;
            }
            
            if(arrow!= null)
            {
                RectTransform arrowRectTrans = arrow.GetComponent<RectTransform>();
                arrowRectTrans.anchoredPosition3D = new Vector3(cell.transform.localPosition.x + cellObjectWidth / 2,
                    arrowRectTrans.anchoredPosition3D.y, 0);
            }
            
            Func(funcOnClickCallBack, cell);
        }

        #endregion
        
        #region 对象池

        /// <summary>
        /// 获取对象池中的对象
        /// </summary>
        /// <returns> 对象 </returns>
        protected override GameObject GetPoolsObj()
        {
            GameObject cell = base.GetPoolsObj();

            if (!m_isAddedListener.ContainsKey(cell))
            {
                Button button = cell.GetComponent<Button>() == null
                    ? cell.AddComponent<Button>()
                    : cell.GetComponent<Button>();

                button.onClick.AddListener(() =>
                {
                    OnClickCell(cell);
                });
                
                m_isAddedListener[cell] = true;
            }
            
            return cell;
        }

        #endregion
    }
}