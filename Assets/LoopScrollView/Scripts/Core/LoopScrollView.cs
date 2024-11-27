using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LoopScrollViewNamespace
{
    /// <summary>
    /// 工具类：为循环滚动列表提供一些辅助功能
    /// </summary>
    public class LoopScrollViewUtils
    {
        /// <summary>
        /// 设置物体的激活状态
        /// </summary>
        /// <param name="obj">目标游戏物体</param>
        /// <param name="isActive">激活状态（true为激活，false为隐藏）</param>
        public static void SetActive(GameObject obj, bool isActive)
        {
            if (obj == null) return; // 检查对象是否为null
            obj.SetActive(isActive);
        }
    }
    
    /// <summary>
    /// 滚动方向枚举：指定列表的滚动方向
    /// </summary>
    public enum LoopScrollDirection
    {
        /// <summary>
        /// 水平方向滚动
        /// </summary>
        Horizontal,
        
        /// <summary>
        /// 垂直方向滚动
        /// </summary>
        Vertical
    }
    
    /// <summary>
    /// 循环滚动组件
    /// </summary>
    [DisallowMultipleComponent]
    public class LoopScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        /// <summary>
        /// 起始箭头，指示列表滚动的首部
        /// </summary>
        public GameObject pointingFirstArrow;

        /// <summary>
        /// 末尾箭头，指示列表滚动的尾部
        /// </summary>
        public GameObject pointingEndArrow;
        
        /// <summary>
        /// 列表滚动方向，默认值为水平滚动
        /// </summary>
        public LoopScrollDirection direction = LoopScrollDirection.Horizontal;
        
        /// <summary>
        /// 是否显示指示箭头
        /// </summary>
        public bool isShowArrow = true;

        /// <summary>
        /// 每行或每列的Cell数量（适用于网格布局）
        /// </summary>
        public int row = 1;

        /// <summary>
        /// Cell间的间隔（像素单位）
        /// </summary>
        public float spacing = 0f;

        /// <summary>
        /// 列表中单个Cell的预制体
        /// </summary>
        public GameObject cellGameObject;

        /// <summary>
        /// Cell更新回调函数，用于动态更新内容
        /// </summary>
        protected Action<GameObject, int> funcCallBackFunc;

        /// <summary>
        /// Cell点击回调函数，用于处理用户交互
        /// </summary>
        protected Action<GameObject, int> funcOnClickCallBack;

        /// <summary>
        /// 按钮点击回调函数，用于特定交互场景
        /// </summary>
        protected Action<int ,bool, GameObject> funcOnButtonClickCallBack;

        /// <summary>
        /// 列表所在的RectTransform组件
        /// </summary>
        protected RectTransform rectTrans;

        /// <summary>
        /// 可滚动区域的宽度
        /// </summary>
        protected float planeWidth;

        /// <summary>
        /// 可滚动区域的高度
        /// </summary>
        protected float planeHeight;
        
        /// <summary>
        /// Content区域的宽度
        /// </summary>
        protected float contentWidth;

        /// <summary>
        /// Content区域的高度
        /// </summary>
        protected float contentHeight;
        
        /// <summary>
        /// 单个Cell的宽度
        /// </summary>
        protected float cellObjectWidth;

        /// <summary>
        /// 单个Cell的高度
        /// </summary>
        protected float cellObjectHeight;
        
        /// <summary>
        /// 滚动区域的Content对象（所有Cell的父对象）
        /// </summary>
        protected GameObject content;

        /// <summary>
        /// Content的RectTransform组件
        /// </summary>
        protected RectTransform contentRectTrans;
        
        /// <summary>
        /// 存储Cell的位置信息与实例对象
        /// </summary>
        protected struct CellInfo
        {
            public Vector3 pos;  // Cell的位置
            public GameObject obj;  // Cell的游戏对象实例
        }
        
        /// <summary>
        /// 存储所有Cell的信息
        /// </summary>
        protected CellInfo[] cellInfos;
        
        /// <summary>
        /// 是否已完成初始化
        /// </summary>
        private bool m_IsInited = false;
        
        /// <summary>
        /// 列表的ScrollRect组件
        /// </summary>
        protected ScrollRect scrollRect;

        /// <summary>
        /// 当前列表的最大Cell数量
        /// </summary>
        protected int maxCount = -1;

        /// <summary>
        /// 当前视图范围内的最小Cell索引
        /// </summary>
        protected int minIndex = -1;

        /// <summary>
        /// 当前视图范围内的最大Cell索引
        /// </summary>
        protected int maxIndex = -1;
        
        /// <summary>
        /// 标志位：是否需要清空列表
        /// </summary>
        protected bool isClearList = false;

        #region 初始化

        /// <summary>
        /// 初始化滚动列表
        /// </summary>
        /// <param name="callBack">Cell的内容更新回调</param>
        public virtual void Init(Action<GameObject, int> callBack)
        {
            Init(callBack, null);
        }
        /// <summary>
        /// 初始化滚动列表，支持点击事件回调
        /// </summary>
        /// <param name="callBack">Cell的内容更新回调</param>
        /// <param name="onClickCallBack">Cell的点击事件回调</param>
        /// <param name="onButtonClickCallBack">按钮的点击事件回调</param>
        public virtual void Init(Action<GameObject, int> callBack, Action<GameObject, int> onClickCallBack, Action<int, bool, GameObject> onButtonClickCallBack)
        {
            if (onButtonClickCallBack != null)
            {
                funcOnButtonClickCallBack = onButtonClickCallBack;
            }
            Init(callBack, onClickCallBack);
        }
        /// <summary>
        /// 核心初始化逻辑：配置内容区域、回调函数并绑定事件
        /// </summary>
        /// <param name="callBack">内容更新回调</param>
        /// <param name="onClickCallBack">点击事件回调</param>
        public virtual void Init(Action<GameObject, int> callBack, Action<GameObject, int> onClickCallBack)
        {
            DisposeAll(); // 清理之前的设置
            
            funcCallBackFunc = callBack;
            
            if(onClickCallBack != null)
            {
                funcOnClickCallBack = onClickCallBack;
            }
            
            if(m_IsInited) return; // 防止重复初始化

            content = GetComponent<ScrollRect>().content.gameObject;

            if (cellGameObject == null)
            {
                // 获取默认的第一个子物体作为Cell模板
                cellGameObject = content.transform.GetChild(0).gameObject;
            }
            
            //Cell处理
            SetPoolsObj(cellGameObject);
            
            RectTransform cellRectTrans = cellGameObject.GetComponent<RectTransform>();
            cellRectTrans.pivot = new Vector2(0f, 1f);
            //检查 Anchor 是否正确
            CheckAnchor(cellRectTrans);
            cellRectTrans.anchoredPosition = Vector2.zero;
            
            //记录Cell信息
            cellObjectHeight = cellRectTrans.rect.height;
            cellObjectWidth = cellRectTrans.rect.width;
            
            //记录Plane信息
            rectTrans = GetComponent<RectTransform>();
            Rect planeRect = rectTrans.rect;
            planeHeight = planeRect.height;
            planeWidth = planeRect.width;

            //记录 Content 信息
            contentRectTrans = content.GetComponent<RectTransform>();
            Rect contentRect = contentRectTrans.rect;
            contentHeight = contentRect.height;
            contentWidth = contentRect.width;
            
            contentRectTrans.pivot = new Vector2(0f, 1f);
            CheckAnchor(contentRectTrans);
            
            scrollRect = GetComponent<ScrollRect>();
            scrollRect.onValueChanged.RemoveAllListeners();
            //添加滑动事件
            scrollRect.onValueChanged.AddListener(ScrollRectListener);
            // 设置箭头的可见性
            if(pointingFirstArrow != null || pointingEndArrow != null)
            {
                if (!isShowArrow)
                {
                    pointingFirstArrow.SetActive(false);
                    pointingEndArrow.SetActive(false);
                }
                else
                {
                    pointingFirstArrow.SetActive(true);
                    pointingEndArrow.SetActive(true);
                }
                scrollRect.onValueChanged.AddListener(OnDragListener);
                OnDragListener(Vector2.zero);
            }
            
            m_IsInited = true; // 设置初始化标志位
        }

        /// <summary>
        /// 检查并修正RectTransform的Anchor设置
        /// </summary>
        /// <param name="rectTrans">需要检查的RectTransform</param>
        private void CheckAnchor(RectTransform rectTrans)
        {
            if (direction == LoopScrollDirection.Vertical)
            {
                if (!(rectTrans.anchorMin == new Vector2(0f, 1f) && rectTrans.anchorMax == new Vector2(0f, 1f) ||
                      rectTrans.anchorMin == new Vector2(0f, 1f) && rectTrans.anchorMax == new Vector2(1f, 1f)))
                {
                    rectTrans.anchorMin = new Vector2(0f, 1f);
                    rectTrans.anchorMax = new Vector2(1f, 1f);
                }
            }
            else
            {
                if (!(rectTrans.anchorMin == new Vector2(0f, 1f) && rectTrans.anchorMax == new Vector2(0f, 1f) ||
                      rectTrans.anchorMin == new Vector2(0f, 0f) && rectTrans.anchorMax == new Vector2(0f, 1f)))
                {
                    rectTrans.anchorMin = new Vector2(0f, 0f);
                    rectTrans.anchorMax = new Vector2(0f, 1f);
                }
            }
        }
        
        #endregion

        #region 刷新或显示列表
        
        /// <summary>
        /// 显示列表
        /// </summary>
        /// <param name="numStr">列表数量</param>
        public virtual void ShowList(string numStr) { }

        /// <summary>
        /// 显示列表：根据数量动态加载并显示Cell
        /// </summary>
        /// <param name="num">需要显示的Cell总数量</param>
        public virtual void ShowList(int num)
        {
            minIndex = -1;
            maxIndex = -1;
            
            // 计算Content尺寸
            if(direction == LoopScrollDirection.Vertical)
            {
                float contentSize = (spacing + cellObjectHeight) * Mathf.CeilToInt((float)num / row);
                contentHeight = contentSize;
                contentWidth = contentRectTrans.sizeDelta.x;
                contentSize = contentSize < rectTrans.rect.height ? rectTrans.rect.height : contentSize;
                contentRectTrans.sizeDelta = new Vector2(contentWidth, contentSize);
                if (num != maxCount)
                {
                    contentRectTrans.anchoredPosition = new Vector2(contentRectTrans.anchoredPosition.x, 0f);
                }
            }
            else
            {
                float contentSize = (spacing + cellObjectWidth) * Mathf.CeilToInt((float)num / row);
                contentWidth = contentSize;
                contentHeight = contentRectTrans.sizeDelta.x;
                contentSize = contentSize < rectTrans.rect.width ? rectTrans.rect.width : contentSize;
                contentRectTrans.sizeDelta = new Vector2(contentSize, contentHeight);
                if (num != maxCount)
                {
                    contentRectTrans.anchoredPosition = new Vector2(0f, contentRectTrans.anchoredPosition.y);
                }
            }
            
            //计算开始索引
            int lastEndIndex = 0;
            
            //过多的物体放进对象池（首次调用showList时候无效）
            if (m_IsInited)
            {
                lastEndIndex = num - maxCount > 0 ? maxCount : num;
                lastEndIndex = isClearList ? 0 : lastEndIndex;
                
                int count = isClearList ? cellInfos.Length : maxCount;
                for (int i = lastEndIndex; i < count; i++)
                {
                    if(cellInfos[i].obj != null)
                    {
                        SetPoolsObj(cellInfos[i].obj);
                        cellInfos[i].obj = null;
                    }
                }
            }
            
            CellInfo[] tempCellInfos = cellInfos;
            cellInfos = new CellInfo[num];
            
            //计算每个cell坐标并存储，显示范围内的cell
            for (int i = 0; i < num; i++)
            {
                //存储已有的数据 ( 首次调 ShowList函数时 则无效 )
                if (maxCount != -1 && i <lastEndIndex)
                {
                    CellInfo tempCellInfo = tempCellInfos[i];
                    //计算是否超出范围
                    float rangePos = direction == LoopScrollDirection.Vertical ? tempCellInfo.pos.y : tempCellInfo.pos.x;
                    if (!IsOutRange(rangePos))
                    {
                        //记录显示范围中的首位index和末尾index
                        minIndex = minIndex == -1 ? i : minIndex; //首位index
                        maxIndex = i; //末尾index
                        
                        if (tempCellInfo.obj == null)
                        {
                            tempCellInfo.obj = GetPoolsObj();
                        }
                        tempCellInfo.obj.transform.GetComponent<RectTransform>().anchoredPosition = tempCellInfo.pos;
                        tempCellInfo.obj.name = i.ToString();
                        tempCellInfo.obj.SetActive(true);
                        
                        Func(funcCallBackFunc, tempCellInfo.obj);
                    }
                    else
                    {
                        SetPoolsObj(tempCellInfo.obj);
                        tempCellInfo.obj = null;
                    }
                    cellInfos[i] = tempCellInfo;
                    continue;
                }
                
                CellInfo cellInfo = new CellInfo();

                float pos = 0;  //坐标( isVertical ? 记录Y : 记录X )
                float rowPos = 0; //计算每排里面的cell 坐标
                
                //计算每个cell坐标
                if (direction == LoopScrollDirection.Vertical)
                {
                    pos = cellObjectHeight * Mathf.FloorToInt(i / row) + spacing * Mathf.FloorToInt(i / row);
                    rowPos = cellObjectWidth * (i % row) + spacing * (i % row);
                    cellInfo.pos = new Vector3(rowPos, -pos, 0);
                }
                else
                {
                    pos = cellObjectWidth * Mathf.FloorToInt(i / row) + spacing * Mathf.FloorToInt(i / row);
                    rowPos = cellObjectHeight * (i % row) + spacing * (i % row);
                    cellInfo.pos = new Vector3(pos, -rowPos, 0);
                }
                
                //计算是否超出范围
                float cellPos = direction == LoopScrollDirection.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                if (IsOutRange(cellPos))
                {
                    cellInfo.obj = null;
                    cellInfos[i] = cellInfo;
                    continue;
                }
                
                //记录显示范围中的 首位index 和 末尾index
                minIndex = minIndex == -1 ? i : minIndex; //首位index
                maxIndex = i; // 末尾index
                
                //取出或者创建cell
                GameObject cell = GetPoolsObj();
                cell.transform.GetComponent<RectTransform>().anchoredPosition = cellInfo.pos;
                cell.gameObject.name = i.ToString();
                
                //存数据
                cellInfo.obj = cell;
                cellInfos[i] = cellInfo;

                //回调函数
                Func(funcCallBackFunc, cell);
            }
            
            maxCount = num;
            m_IsInited = true;
            
            OnDragListener(Vector2.zero);
            
        }

        /// <summary>
        /// 更新滚动区域的大小
        /// </summary>
        public void UpdateSize()
        {
            Rect rect = GetComponent<RectTransform>().rect;
            planeHeight = rect.height;
            planeWidth = rect.width;
        }

        /// <summary>
        /// 实时刷新列表
        /// </summary>
        public virtual void UpdateList()
        {
            for (int i = 0; i < cellInfos.Length; i++)
            {
                CellInfo cellInfo = cellInfos[i];
                if (cellInfo.obj != null)
                {
                    float rangePos = direction == LoopScrollDirection.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                    if (!IsOutRange(rangePos))
                    {
                        Func(funcCallBackFunc, cellInfo.obj, true);
                    }
                }
            }
        }

        /// <summary>
        /// 刷新某一项cell
        /// </summary>
        /// <param name="index">cell索引</param>
        public void UpdateCell(int index)
        {
            // TODO: CellInfo cellInfo = cellInfos[index - 1];
            CellInfo cellInfo = cellInfos[index];
            if (cellInfo.obj != null)
            {
                float rangePos = direction == LoopScrollDirection.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                if (!IsOutRange(rangePos))
                {
                    Func(funcCallBackFunc, cellInfo.obj);
                }
            }
        }
        
        #endregion

        #region 事件
        
        /// <summary>
        /// 滑动事件
        /// </summary>
        /// <param name="value">滑动值</param>
        protected virtual void ScrollRectListener(Vector2 value)
        {
            UpdateCheck();
        }

        private void UpdateCheck()
        {
            if(cellInfos == null) return;
            
            //检查超出范围
            for (int i = 0; i < cellInfos.Length; i++)
            {
                CellInfo cellInfo = cellInfos[i];
                GameObject obj = cellInfo.obj;
                Vector3 pos = cellInfo.pos;

                float rangePos = direction == LoopScrollDirection.Vertical ? pos.y : pos.x;
                
                //判断是否超出显示范围
                if (IsOutRange(rangePos))
                {
                    //把超出范围的cell放入对象池
                    if (obj != null)
                    {
                        SetPoolsObj(obj);
                        cellInfos[i].obj = null;
                    }
                }
                else
                {
                    if (obj == null)
                    {
                        //优先从对象池中取出 cell,如果没有取到，则创建新的 cell
                        GameObject cell = GetPoolsObj();
                        cell.transform.localPosition = pos;
                        cell.gameObject.name = i.ToString();
                        cellInfos[i].obj = cell;
                        funcCallBackFunc(cell, i);

                        Func(funcCallBackFunc, cell);
                    }
                }
            }
        }

        /// <summary>
        /// 是否超出显示范围
        /// </summary>
        /// <param name="pos">位置</param>
        /// <returns>是否超出</returns>
        protected bool IsOutRange(float pos)
        {
            Vector3 listPos = contentRectTrans.anchoredPosition;
            if (direction == LoopScrollDirection.Vertical)
            {
                if(pos + listPos.y > cellObjectHeight || pos + listPos.y < -rectTrans.rect.height)
                {
                    return true;
                }
            }
            else
            {
                if(pos + listPos.x < -cellObjectWidth || pos + listPos.x > rectTrans.rect.width)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="func">回调函数</param>
        /// <param name="selectObject">对象</param>
        /// <param name="isUpdate">是否更新</param>
        protected void Func(Action<GameObject, int> func, GameObject selectObject, bool isUpdate = false)
        {
            int num = int.Parse(selectObject.name) + 1;
            func?.Invoke(selectObject, num);
        }

        /// <summary>
        /// 滑动监听
        /// </summary>
        /// <param name="value">滑动值</param>
        protected void OnDragListener(Vector2 value)
        {
            if(!isShowArrow) return;
            // 当前滑动的位置，0表示最上（左），1表示最下（右）
            float normalizedPos = direction == LoopScrollDirection.Vertical ? scrollRect.verticalNormalizedPosition : scrollRect.horizontalNormalizedPosition;
            
            if (direction == LoopScrollDirection.Vertical)
            {
                if (contentHeight - rectTrans.rect.height < 10)
                {
                    LoopScrollViewUtils.SetActive(pointingFirstArrow, false);
                    LoopScrollViewUtils.SetActive(pointingEndArrow, false);
                    return;
                }
            }
            else
            {
                if (contentWidth - rectTrans.rect.width < 10)
                {
                    LoopScrollViewUtils.SetActive(pointingFirstArrow, false);
                    LoopScrollViewUtils.SetActive(pointingEndArrow, false);
                    return;
                }
            }
            
            if (direction == LoopScrollDirection.Vertical)
            {
                switch (normalizedPos)
                {
                    case >= 0.9f:
                        LoopScrollViewUtils.SetActive(pointingFirstArrow, false);
                        LoopScrollViewUtils.SetActive(pointingEndArrow, true);
                        break;
                    case <= 0.1f:
                        LoopScrollViewUtils.SetActive(pointingFirstArrow, true);
                        LoopScrollViewUtils.SetActive(pointingEndArrow, false);
                        break;
                    default:
                        LoopScrollViewUtils.SetActive(pointingFirstArrow, true);
                        LoopScrollViewUtils.SetActive(pointingEndArrow, true);
                        break;
                }
            }
            else
            {
                switch (normalizedPos)
                {
                    case >= 0.9f:
                        LoopScrollViewUtils.SetActive(pointingFirstArrow, true);
                        LoopScrollViewUtils.SetActive(pointingEndArrow, false);
                        break;
                    case <= 0.1f:
                        LoopScrollViewUtils.SetActive(pointingFirstArrow, false);
                        LoopScrollViewUtils.SetActive(pointingEndArrow, true);
                        break;
                    default:
                        LoopScrollViewUtils.SetActive(pointingFirstArrow, true);
                        LoopScrollViewUtils.SetActive(pointingEndArrow, true);
                        break;
                }
            }
            
        }

        /// <summary>
        /// 点击cell
        /// </summary>
        /// <param name="cell">所点击的cell对象</param>
        public virtual void OnClickCell(GameObject cell) { }
        
        /// <summary>
        /// 点击展开按钮,ExpandCircularScrollView 函数
        /// </summary>
        /// <param name="index">索引</param>
        public virtual void OnClickExpand(int index){ }
        
        /// <summary>
        /// 设置翻页到指定索引,FlipCircularScrollView 函数
        /// </summary>
        /// <param name="index">索引</param>
        public virtual void SetToPageIndex(int index) { }
        
        public virtual void OnBeginDrag(PointerEventData eventData) { }
        public void OnEndDrag(PointerEventData eventData) { }
        public virtual void OnDrag(PointerEventData eventData) { }
        
        #endregion

        #region 循环列表对象池
        
        /// <summary>
        /// 对象池子
        /// </summary>
        protected Stack<GameObject> poolsObj = new Stack<GameObject>();

        /// <summary>
        /// 取出对象
        /// </summary>
        /// <returns>cell对象</returns>
        protected virtual GameObject GetPoolsObj()
        {
            GameObject cell = null;
            if (poolsObj.Count > 0)
            {
                cell = poolsObj.Pop();
            }
            
            if(cell == null)
            {
                cell = Instantiate(cellGameObject);
            }
            cell.transform.SetParent(content.transform);
            cell.transform.localPosition = Vector3.zero;
            cell.transform.localScale = Vector3.one;
            LoopScrollViewUtils.SetActive(cell, true);
            
            return cell;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="cell">要回收的对象</param>
        protected virtual void SetPoolsObj(GameObject cell)
        {
            if(cell == null) return;
            
            poolsObj.Push(cell);
            LoopScrollViewUtils.SetActive(cell, false);
        }
        
        
        
        #endregion

        private void OnDestroy()
        {
            DisposeAll();
        }

        public void DisposeAll()
        {
            if(funcCallBackFunc != null)
            {
                funcCallBackFunc = null;
            }
            
            if(funcOnClickCallBack != null)
            {
                funcOnClickCallBack = null;
            }
        }
    }
}
