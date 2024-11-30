using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LoopScrollViewNamespace
{
    public class FlipPageLoopScrollView : LoopScrollView
    {
        /// <summary>
        /// 每页显示的项目
        /// </summary>
        public int onePageCount = 1;
        
        /// <summary>
        /// 滑动速度
        /// </summary>
        public float slideSpeed = 5f;

        public bool isMaxRange = false;
        
        /// <summary>
        /// 是否打开导航图标
        /// </summary>
        public bool isOpenNavIcon = false;
        
        //开启导航图标时 使用以下变量
        public float iconSize =20f;
        public float iconSpacing = 50f;
        public Sprite selectIcon = null;
        public Sprite normalIcon = null;
        public Transform objNavigation;
        
        /// <summary>
        /// 总页码
        /// </summary>
        private int m_AllPageCount;

        /// <summary>
        /// 当前位置索引
        /// </summary>
        private int m_NowIndex;
        /// <summary>
        /// 上一个位置索引
        /// </summary>
        private int m_LastIndex;
        /// <summary>
        /// 是否拖拽中
        /// </summary>
        private bool m_IsDrag = false;
        /// <summary>
        /// 滑动的目标位置
        /// </summary>
        private float m_TargetPos = 0;
        /// <summary>
        /// 总页数索引比例0-1
        /// </summary>
        private List<float> m_ListPageValue = new List<float> { 0 };
        
        private List<Image> m_NavigationList = new List<Image>();

        protected new Action<int> funcOnClickCallBack;

        #region 显示列表

        public override void ShowList(int num)
        {
            base.ShowList(num);
            ListPageValueInit();
        }

        /// <summary>
        /// 初始化列表页码索引比例
        /// </summary>
        private void ListPageValueInit()
        {
            m_NavigationList.Clear();
            m_AllPageCount = maxCount - onePageCount;
            if (maxCount != 0)
            {
                for (float i = 0; i < m_AllPageCount; i++)
                {
                    m_ListPageValue.Add(i / m_AllPageCount);
                }
            }

            if (isOpenNavIcon && maxCount > 1)
            {
                if (objNavigation == null)
                {
                    objNavigation = transform.Find("objNavigation");
                }

                float[] posArray = new float[maxCount];
                if (maxCount == 1)
                {
                    posArray[0] = 0;
                }
                else
                {
                    float startX = -maxCount / 2f * iconSpacing;
                    for (int i = 0; i < maxCount; i++)
                    {
                        posArray[i] = startX + i * iconSpacing;
                    }
                }

                for (int i = 0; i < maxCount; i++)
                {
                    GameObject icon = null;
                    if (objNavigation.Find($"icon{i}") != null)
                    {
                        icon = objNavigation.Find($"icon{i}").gameObject;
                    }
                    else
                    {
                        icon = new GameObject($"icon{i}");
                    }
                    icon.transform.SetParent(objNavigation);
                    Image img = null;
                    if (icon.GetComponent<Image>() == null)
                    {
                        img = icon.AddComponent<Image>();
                    }
                    else
                    {
                        img = icon.GetComponent<Image>();
                    }

                    if (i == 0)
                    {
                        img.sprite = selectIcon;
                    }
                    else
                    {
                        img.sprite = normalIcon;
                    }
                    img.rectTransform.sizeDelta = new Vector2(iconSize, iconSize);
                    icon.transform.localPosition = new Vector3(posArray[i], 0, 0);
                    icon.transform.localScale = Vector3.one;
                    m_NavigationList.Add(img);
                }
            }

            if (funcOnClickCallBack != null)
            {
                Func(funcOnClickCallBack, m_NowIndex);
            }
        }

        /// <summary>
        /// 翻页到某一页
        /// </summary>
        /// <param name="index"> 页码索引 </param>
        public override void SetToPageIndex(int index)
        {
            m_IsDrag = false;
            m_NowIndex = index - 1;
            m_TargetPos = m_ListPageValue[m_NowIndex];

            for (int i = 0; i < m_NavigationList.Count; i++)
            {
                if (i == m_NowIndex)
                {
                    m_NavigationList[m_NowIndex].sprite = selectIcon;
                }
                else
                {
                    m_NavigationList[i].sprite = normalIcon;
                }
            }
            if(funcOnClickCallBack != null)
            {
                Func(funcOnClickCallBack, m_NowIndex);
            }
        }
        #endregion

        private void Update()
        {
            if (!m_IsDrag)
            {
                if (scrollRect == null) return;

                if (direction == LoopScrollDirection.Vertical)
                {
                    scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition,
                        m_TargetPos, Time.deltaTime * slideSpeed);
                }
                else
                {
                    scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition,
                        m_TargetPos, Time.deltaTime * slideSpeed);
                }
            }
        }
        
        #region 事件

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            m_IsDrag = true;
        }
        
        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            m_IsDrag = false;
            if (m_ListPageValue.Count == 1) return;
            float tempPos = 0;
            if(direction == LoopScrollDirection.Vertical)
            {
                tempPos = scrollRect.verticalNormalizedPosition;
            }
            else
            {
                tempPos = scrollRect.horizontalNormalizedPosition;
            }

            if (isMaxRange)
            {
                //获取拖动的值
                int index = 0;
                //拖动的绝对值
                float offset = Mathf.Abs(m_ListPageValue[index] - tempPos);
                for (int i = 1; i < m_ListPageValue.Count; i++)
                {
                    float temp = Mathf.Abs(tempPos - m_ListPageValue[i]);
                    if (temp < offset)
                    {
                        index = i;
                        offset = temp;
                    }
                }
                m_TargetPos = m_ListPageValue[index];
                m_NowIndex = index;
                if(m_NowIndex != m_LastIndex && funcOnClickCallBack != null)
                {
                    Func(funcOnClickCallBack, m_NowIndex);
                }
                m_LastIndex = m_NowIndex;
            }
            else
            {
                float currPos = m_ListPageValue[m_NowIndex];
                if(tempPos > currPos)
                {
                    m_NowIndex++;
                }
                else if (tempPos < currPos)
                {
                    m_NowIndex--;
                }

                if (m_NowIndex < 0)
                {
                    m_NowIndex = 0;
                }
                
                if(m_NowIndex > m_ListPageValue.Count - 1)
                {
                    m_NowIndex = m_ListPageValue.Count - 1;
                }
                m_TargetPos = m_ListPageValue[m_NowIndex];
                
                if(m_NowIndex != m_LastIndex && funcOnClickCallBack != null)
                {
                    Func(funcOnClickCallBack, m_NowIndex);
                }
                m_LastIndex = m_NowIndex;
            }

            if (isOpenNavIcon)
            {
                for (int i = 0; i < m_NavigationList.Count; i++)
                {
                    Image img = m_NavigationList[i];
                    if(i == m_NowIndex)
                    {
                        img.sprite = selectIcon;
                    }
                    else
                    {
                        img.sprite = normalIcon;
                    }
                }
            }
        }

        /// <summary>
        /// 翻页时候的回调
        /// </summary>
        /// <param name="func"> 回调函数 </param>
        /// <param name="index"> 页码索引 </param>
        private void Func(Action<int> func, int index)
        {
            func?.Invoke(index+1);
        }

        #endregion
    }
}
