using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace LoopScrollViewNamespace
{
    public class ExpandLoopScrollView : LoopScrollView
    {
        public GameObject expandButton;
        public float backgroundMargin;
        public bool isExpand = false;

        private float m_ExpandButtonX;
        private float m_ExpandButtonY;
        private float m_ExpandButtonWidth;
        private float m_ExpandButtonHeight;

        private Vector2 m_BackgroundOriginSize;
        
        //展开信息
        struct ExpandInfo
        {
            public GameObject button;
            public bool isExpand;
            public CellInfo[] cellInfos;
            public float size;
            public int cellCount;
        }

        private ExpandInfo[] m_ExpandInfos = null;

        //用于判断是否重复添加点击事件
        private Dictionary<GameObject, bool> m_IsAddedListener = new Dictionary<GameObject, bool>();

        //使用 new 是一种明确的语法，用于表明有意隐藏基类成员，避免隐藏基类成员时产生警告c冲突。
        protected new Action<GameObject, GameObject, int, int> funcCallBackFunc;
        protected new Action<GameObject, GameObject, int, int> funcOnClickCallBack;
        
        #region 初始化
        public void Init(Action<GameObject, GameObject, int, int> callBack)
        {
            Init(callBack, null);
        }

        public void Init(Action<GameObject, GameObject, int, int> callBack,
            Action<GameObject, GameObject, int, int> onClickCallBack,
            Action<int, bool, GameObject> onButtonClickCallBack)
        {
            funcOnButtonClickCallBack = onButtonClickCallBack;
            Init(callBack, onClickCallBack);
        }

        public void Init(Action<GameObject, GameObject, int, int> callBack,
            Action<GameObject, GameObject, int, int> onClickCallBack)
        {
            base.Init(null, null);
            
            funcCallBackFunc = callBack;

            //Button处理
            if (expandButton == null)
            {
                if (content.transform.Find("Button") != null)
                {
                    expandButton = content.transform.Find("Button").gameObject;
                }
            }

            if (expandButton != null)
            {
                RectTransform rectTrans = expandButton.transform.GetComponent<RectTransform>();
                m_ExpandButtonX = rectTrans.anchoredPosition.x;
                m_ExpandButtonY = rectTrans.anchoredPosition.y;

                SetPoolsButtonObj(expandButton);
                
                m_ExpandButtonWidth = rectTrans.rect.width;
                m_ExpandButtonHeight = rectTrans.rect.height;

                Transform background = expandButton.transform.Find("background");
                if (background != null)
                {
                    m_BackgroundOriginSize = background.GetComponent<RectTransform>().sizeDelta;
                }
            }
        }
        #endregion

        #region 显示或刷新列表

        public override void ShowList(string numStr)
        {
            //清除所有Cell (非首次调Showlist时执行)
            ClearCell();

            int totalCount = 0;
            int beforeCellCount = 0;
            
            string[] numArray = numStr.Split('|');
            int buttonCount = numArray.Length;

            bool isReset = false;
            if(isInited && m_ExpandInfos.Length == buttonCount)
            {
                isReset = false;
            }
            else
            {
                m_ExpandInfos = new ExpandInfo[buttonCount];
                isReset = true;
            }

            for (int i = 0; i < buttonCount; i++)
            {
                // Button处理
                GameObject button = GetPoolsButtonObj();
                button.name = i.ToString();
                Button buttonComponent = button.GetComponent<Button>();
                if(!m_IsAddedListener.ContainsKey(button) && buttonComponent != null)
                {
                    m_IsAddedListener[button] = true;
                    buttonComponent.onClick.AddListener(() =>
                    {
                        OnClickExpand(button);
                    });
                    button.transform.SetSiblingIndex(0);
                }

                float pos = 0; // 坐标（isVertical ? 记录Y : 记录X）
                
                //计算每个按钮的坐标
                if(direction == LoopScrollDirection.Vertical)
                {
                    pos = m_ExpandButtonHeight * i + spacing * (i + 1);
                    pos += i > 0 ? (cellObjectHeight + spacing) * Mathf.CeilToInt((float)beforeCellCount / row) : 0;
                    button.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(m_ExpandButtonX, -pos, 0);
                }
                else
                {
                    pos = m_ExpandButtonWidth * i + spacing * (i + 1);
                    pos += i > 0 ? (cellObjectWidth + spacing) * Mathf.CeilToInt((float)beforeCellCount / row) : 0;
                    button.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(pos, m_ExpandButtonY, 0);
                }
                
                int count = int.Parse(numArray[i]);
                totalCount += count;
                
                //存储数据
                ExpandInfo expandInfo = isReset ? new ExpandInfo() : m_ExpandInfos[i];
                expandInfo.button = button;
                expandInfo.cellCount = count;
                expandInfo.cellInfos = new CellInfo[count];
                expandInfo.isExpand = isReset ? isExpand : expandInfo.isExpand;
                expandInfo.size = direction == LoopScrollDirection.Vertical
                    ? (cellObjectHeight + spacing) * Mathf.CeilToInt((float)count / row) 
                    : (cellObjectWidth + spacing) * Mathf.CeilToInt((float)count / row); // 计算需要展开的尺寸
                
                //遍历每个按钮下的cell
                for (int j = 0; j < count; j++)
                {
                    if(!expandInfo.isExpand) break;
                    
                    CellInfo cellInfo = new CellInfo();
                    float rowPos = 0; // 计算每排里面的cell坐标
                    
                    //计算每个cell的坐标
                    if (direction == LoopScrollDirection.Vertical)
                    {
                        pos = cellObjectHeight * Mathf.FloorToInt(j / row) + spacing * (Mathf.FloorToInt(j / row) + 1);
                        pos += (m_ExpandButtonHeight + spacing) * (i + 1);
                        pos += (cellObjectHeight + spacing) * Mathf.CeilToInt((float)beforeCellCount / row);
                        rowPos = cellObjectWidth * (j % row) + spacing * (j % row);
                        cellInfo.pos = new Vector3(rowPos, -pos, 0);
                    }
                    else
                    {
                        pos = cellObjectWidth * Mathf.FloorToInt(j / row) + spacing * (Mathf.FloorToInt(j / row) + 1);
                        pos += (m_ExpandButtonWidth + spacing) * (i + 1);
                        pos += (cellObjectWidth + spacing) * Mathf.CeilToInt((float)beforeCellCount / row);
                        rowPos = cellObjectHeight * (j % row) + spacing * (j % row);
                        cellInfo.pos = new Vector3(pos, -rowPos, 0);
                    }
                    
                    //计算是否超出范围
                    float cellPos = direction == LoopScrollDirection.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                    if (IsOutRange(cellPos))
                    {
                        cellInfo.obj = null;
                        expandInfo.cellInfos[j] = cellInfo;
                        continue;
                    }
                    
                    //获取cell对象
                    GameObject cell = GetPoolsObj();
                    cell.transform.GetComponent<RectTransform>().anchoredPosition3D = cellInfo.pos;
                    cell.gameObject.name = i + "_" + j.ToString();
                    
                    Button cellButtonComponent = cell.GetComponent<Button>();
                    if(!m_IsAddedListener.ContainsKey(cell) && cellButtonComponent != null)
                    {
                        m_IsAddedListener[cell] = true;
                        cellButtonComponent.onClick.AddListener(()=>
                        {
                            OnClickCell(cell);
                        });
                    }
                    
                    //存数据
                    cellInfo.obj = cell;
                    expandInfo.cellInfos[j] = cellInfo;
                    
                    //回调函数
                    Func(funcCallBackFunc, button, cell, expandInfo.isExpand);
                }
                
                beforeCellCount += expandInfo.isExpand ? count : 0;
                m_ExpandInfos[i] = expandInfo;
                Func(funcCallBackFunc, button, null, expandInfo.isExpand);
            }

            if (!isInited)
            {
                //计算Content的尺寸
                if (direction == LoopScrollDirection.Vertical)
                {
                    float contentSize =
                        isExpand ? (spacing + cellObjectHeight) * Mathf.CeilToInt((float)totalCount / row) : 0;
                    contentSize += (spacing + m_ExpandButtonHeight) * buttonCount;
                    contentRectTrans.sizeDelta = new Vector2(contentRectTrans.sizeDelta.x, contentSize);
                }
                else
                {
                    float contentSize =
                        isExpand ? (spacing + cellObjectWidth) * Mathf.CeilToInt((float)totalCount / row) : 0;
                    contentSize += (spacing + m_ExpandButtonWidth) * buttonCount;
                    contentRectTrans.sizeDelta = new Vector2(contentSize, contentRectTrans.sizeDelta.y);
                }
            }
            
            isInited = true;
        }
        #endregion

        #region 事件

        /// <summary>
        /// 点击Cell事件
        /// </summary>
        /// <param name="cell"> 点击的Cell </param>
        public override void OnClickCell(GameObject cell)
        {
            int index = int.Parse(cell.name.Split('_')[0]);
            Func(funcOnClickCallBack, m_ExpandInfos[index].button, cell, m_ExpandInfos[index].isExpand);
        }

        /// <summary>
        /// 点击展开按钮
        /// </summary>
        /// <param name="button"> 按钮 </param>
        private void OnClickExpand(GameObject button)
        {
            int index = int.Parse(button.name) + 1;
            OnClickExpand(index);
            if (funcOnButtonClickCallBack != null)
            {
                funcOnButtonClickCallBack(index, m_ExpandInfos[index - 1].isExpand, button);
            }
        }

        public override void OnClickExpand(int index)
        {
            base.OnClickExpand(index);
            
            index = index - 1;
            m_ExpandInfos[index].isExpand = !m_ExpandInfos[index].isExpand;
            
            //计算Content 的尺寸
            Vector2 size = contentRectTrans.sizeDelta;
            if(direction == LoopScrollDirection.Vertical)
            {
                float height = m_ExpandInfos[index].isExpand ? size.y + m_ExpandInfos[index].size : size.y - m_ExpandInfos[index].size;
                contentRectTrans.sizeDelta = new Vector2(size.x, height);
            }
            else
            {
                float width = m_ExpandInfos[index].isExpand ? size.x + m_ExpandInfos[index].size : size.x - m_ExpandInfos[index].size;
                contentRectTrans.sizeDelta = new Vector2(width, size.y);
            }
            
            int beforeCellCount = 0;
            float pos = 0;
            float rowPos = 0;
            
            //重新计算坐标并显示处理
            for (int i = 0; i < m_ExpandInfos.Length; i++)
            {
                int count = m_ExpandInfos[i].cellCount;
                if (i >= index)
                {
                    //计算按钮位置
                    GameObject button = m_ExpandInfos[i].button;
                    if(direction == LoopScrollDirection.Vertical)
                    {
                        pos = m_ExpandButtonHeight * i + spacing * (i + 1);
                        pos += (cellObjectHeight + spacing) * Mathf.CeilToInt((float)beforeCellCount / row);
                        button.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(m_ExpandButtonX, -pos, 0);
                    }
                    else
                    {
                        pos = m_ExpandButtonWidth * i + spacing * (i + 1);
                        pos += (cellObjectWidth + spacing) * Mathf.CeilToInt((float)beforeCellCount / row);
                        button.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(pos, m_ExpandButtonY, 0);
                    }
                    
                    ExpandInfo expandInfo = m_ExpandInfos[i];
                    for (int j = 0; j < count; j++)
                    {
                        //按钮收 的状态
                        if (!expandInfo.isExpand)
                        {
                            if(expandInfo.cellInfos[j].obj != null)
                            {
                                SetPoolsObj(expandInfo.cellInfos[j].obj);
                                m_ExpandInfos[i].cellInfos[j].obj = null;
                            }
                            continue;
                        }
                        
                        CellInfo cellInfo = expandInfo.cellInfos[j];
                        
                        //计算每个cell的坐标
                        if(direction == LoopScrollDirection.Vertical)
                        {
                            pos = cellObjectHeight * Mathf.FloorToInt(j / row) +
                                  spacing * (Mathf.FloorToInt(j / row) + 1);
                            pos += (m_ExpandButtonHeight + spacing) * (i + 1);
                            pos += (cellObjectHeight + spacing) * Mathf.CeilToInt((float)beforeCellCount / row);
                            rowPos = cellObjectWidth * (j % row) + spacing * (j % row);
                            cellInfo.pos = new Vector3(rowPos, -pos, 0);
                        }
                        else
                        {
                            pos = cellObjectWidth * Mathf.FloorToInt(j / row) +
                                  spacing * (Mathf.FloorToInt(j / row) + 1);
                            pos += (m_ExpandButtonWidth + spacing) * (i + 1);
                            pos += (cellObjectWidth + spacing) * Mathf.CeilToInt((float)beforeCellCount / row);
                            rowPos = cellObjectHeight * (j % row) + spacing * (j % row);
                            cellInfo.pos = new Vector3(pos, -rowPos, 0);
                        }
                        
                        //计算是否超出范围
                        float cellPos = direction == LoopScrollDirection.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                        if (IsOutRange(cellPos))
                        {
                            SetPoolsObj(cellInfo.obj);
                            cellInfo.obj = null;
                            m_ExpandInfos[i].cellInfos[j] = cellInfo;
                            continue;
                        }
                        
                        GameObject cell = cellInfo.obj != null ? cellInfo.obj : GetPoolsObj();
                        cell.transform.GetComponent<RectTransform>().anchoredPosition3D = cellInfo.pos;
                        cell.gameObject.name = i + "_" + j.ToString();
                        
                        //回调
                        if (cellInfo.obj == null)
                        {
                            Func(funcCallBackFunc, button, cell, expandInfo.isExpand);
                        }
                        
                        //添加按钮事件
                        Button cellButtonComponent = cell.GetComponent<Button>();
                        if(!m_IsAddedListener.ContainsKey(cell) && cellButtonComponent != null)
                        {
                            m_IsAddedListener[cell] = true;
                            cellButtonComponent.onClick.AddListener(()=>
                            {
                                OnClickCell(cell);
                            });
                        }
                        
                        //存数据
                        cellInfo.obj = cell;
                        m_ExpandInfos[i].cellInfos[j] = cellInfo;
                    }
                }

                if (m_ExpandInfos[i].isExpand)
                {
                    beforeCellCount += count;
                }
            }
            
            //展开时候的背景图
            ExpandBackground(m_ExpandInfos[index]);
            Func(funcCallBackFunc, m_ExpandInfos[index].button, null, m_ExpandInfos[index].isExpand);
        }

        /// <summary>
        /// 展开时候的背景图
        /// </summary>
        /// <param name="expandInfo"> 展开信息 </param>
        private void ExpandBackground(ExpandInfo expandInfo)
        {
            //收展的list尺寸变化
            if (expandInfo.isExpand == false)
            {
                Transform background = expandInfo.button.transform.Find("background");
                if (background != null)
                {
                    RectTransform backgroundRect = background.GetComponent<RectTransform>();
                    backgroundRect.sizeDelta = m_BackgroundOriginSize;
                }
            }
            else
            {
                Transform background = expandInfo.button.transform.Find("background");
                if (background != null)
                {
                    RectTransform backgroundRect = background.GetComponent<RectTransform>();
                    float total_h = expandInfo.size;
                    if (direction == LoopScrollDirection.Vertical)
                    {
                        if (total_h > 3f)
                        {
                            backgroundRect.sizeDelta = new Vector2(m_BackgroundOriginSize.x,
                                m_BackgroundOriginSize.y + total_h + backgroundMargin);
                        }
                        else
                        {
                            backgroundRect.sizeDelta = new Vector2(m_BackgroundOriginSize.x,
                                m_BackgroundOriginSize.y);
                        }
                    }
                    else
                    {
                        backgroundRect.sizeDelta = new Vector2(m_BackgroundOriginSize.x + total_h + backgroundMargin,
                            m_BackgroundOriginSize.y);
                    }
                }
            }
        }

        protected override void ScrollRectListener(Vector2 value)
        {
            Vector3 contentP = contentRectTrans.anchoredPosition3D;
            
            if (m_ExpandInfos == null) return;

            for (int i = 0; i < m_ExpandInfos.Length; i++)
            {
                ExpandInfo expandInfo = m_ExpandInfos[i];
                if (!expandInfo.isExpand) continue;

                int count = expandInfo.cellCount;
                for (int j = 0; j < count; j++)
                {
                    CellInfo cellInfo = expandInfo.cellInfos[j];
                    float rangPos = direction == LoopScrollDirection.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                    if (IsOutRange(rangPos))
                    {
                        SetPoolsObj(cellInfo.obj);
                        m_ExpandInfos[i].cellInfos[j].obj = null;
                    }
                    else
                    {
                        if (cellInfo.obj == null)
                        {
                            GameObject cell = GetPoolsObj();
                            cell.transform.GetComponent<RectTransform>().anchoredPosition3D = cellInfo.pos;
                            cell.name = i + "_" + j.ToString();
                            
                            Button cellButtonComponent = cell.GetComponent<Button>();
                            if (!m_IsAddedListener.ContainsKey(cell) && cellButtonComponent != null)
                            {
                                m_IsAddedListener[cell] = true;
                                cellButtonComponent.onClick.AddListener(() =>
                                {
                                    OnClickCell(cell);
                                });
                            }
                            
                            cellInfo.obj = cell;
                            m_ExpandInfos[i].cellInfos[j] = cellInfo;
                            Func(funcCallBackFunc, expandInfo.button, cell, expandInfo.isExpand);
                        }
                    }
                }
            }
        }


        private void Func(Action<GameObject, GameObject, int, int> Func, GameObject button, GameObject selectObject,
            bool isShow)
        {
            string[] objName = { "1", "-2" };
            if (selectObject != null)
            {
                objName = selectObject.name.Split('_');
            }

            int buttonName = int.Parse(button.name) + 1;
            int num = int.Parse(objName[1]) + 1;

            if (Func != null)
            {
                if(selectObject != null)
                {
                    // Func.Invoke(button, selectObject, buttonName, num, isShow);
                    Func.Invoke(button, selectObject, buttonName, num);
                }
                else
                {
                    // Func.Invoke(button, null, buttonName, -1, isShow);
                    Func.Invoke(button, null, buttonName, -1);
                }
            }
        }
        
        #endregion
        
        #region 拓展按钮对象池
        
        private Stack<GameObject> buttonPoolsObj = new Stack<GameObject>();
        
        /// <summary>
        /// 获取按钮对象池中的按钮
        /// </summary>
        /// <returns> 按钮对象 </returns>
        public GameObject GetPoolsButtonObj()
        {
            GameObject button = null;
            if (buttonPoolsObj.Count > 0)
            {
                button = buttonPoolsObj.Pop();
            }
            if (button == null)
            {
                button = Instantiate(expandButton);
            }
            button.transform.SetParent(content.transform);
            // TODO: 按钮位置需要重新设置
            button.transform.localPosition = Vector3.zero;
            button.transform.localScale = Vector3.one;
            LoopScrollViewUtils.SetActive(button, true);
            
            return button;
        }
        
        /// <summary>
        /// 存入按钮对象池中的按钮
        /// </summary>
        /// <param name="button"> 按钮对象 </param>
        private void SetPoolsButtonObj(GameObject button)
        {
            if (button != null)
            {
                buttonPoolsObj.Push(button);
                LoopScrollViewUtils.SetActive(button, false);
            }
        }

        /// <summary>
        /// 清除所有Cell 扔到对象池
        /// </summary>
        private void ClearCell()
        {
            if (!isInited)
            {
                return;
            }

            for (int i = 0; i < m_ExpandInfos.Length; i++)
            {
                if(m_ExpandInfos[i].button != null)
                {
                    SetPoolsButtonObj(m_ExpandInfos[i].button);
                    m_ExpandInfos[i].button = null;
                }

                for (int j = 0; j < m_ExpandInfos[i].cellInfos.Length; j++)
                {
                    if (m_ExpandInfos[i].cellInfos[j].obj != null)
                    {
                        SetPoolsObj(m_ExpandInfos[i].cellInfos[j].obj);
                        m_ExpandInfos[i].cellInfos[j].obj = null;
                    }
                }
            }
        }
        #endregion
    }
}