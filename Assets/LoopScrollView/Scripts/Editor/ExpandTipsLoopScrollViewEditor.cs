using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LoopScrollViewNamespace
{
    [CustomEditor(typeof(ExpandTipsLoopScrollView))]
    public class ExpandTipsLoopScrollViewEditor : Editor
    {
    private readonly string[] m_TitlesE = new string[10] { "Direction", "Row Or Column", "Spacing", "Cell GameObject", "ExpandTips", "Arrow", "BackgroundScale", "Is Show Arrow", "Up or Left Arrow", "Down or Right Arrow" };
        private readonly string[] m_TitlesC = new string[10] { "方向", "行或列", "间距", "单元格Item", "展开的Tips", "箭头", "背景缩放", "是否显示箭头", "上或左箭头", "下或右箭头" };
        
        private ExpandTipsLoopScrollView m_ExpandTipsLoopScrollView;

        public override void OnInspectorGUI()
        {
            m_ExpandTipsLoopScrollView = (ExpandTipsLoopScrollView)target;
            
            m_ExpandTipsLoopScrollView.direction = (LoopScrollDirection)EditorGUILayout.EnumPopup(GetTitle(0), m_ExpandTipsLoopScrollView.direction);
            m_ExpandTipsLoopScrollView.row = EditorGUILayout.IntField(GetTitle(1), m_ExpandTipsLoopScrollView.row);
            m_ExpandTipsLoopScrollView.spacing = EditorGUILayout.FloatField(GetTitle(2), m_ExpandTipsLoopScrollView.spacing);
            m_ExpandTipsLoopScrollView.cellGameObject = (GameObject)EditorGUILayout.ObjectField(GetTitle(3), m_ExpandTipsLoopScrollView.cellGameObject, typeof(GameObject), true);
            m_ExpandTipsLoopScrollView.expandTips = (GameObject)EditorGUILayout.ObjectField(GetTitle(4), m_ExpandTipsLoopScrollView.expandTips, typeof(GameObject), true);
            m_ExpandTipsLoopScrollView.arrow = (GameObject)EditorGUILayout.ObjectField(GetTitle(5), m_ExpandTipsLoopScrollView.arrow, typeof(GameObject), true);
            // m_ExpandLoopScrollView.backgroundMargin = EditorGUILayout.FloatField(GetTitle(6), m_ExpandLoopScrollView.backgroundMargin);
            
            m_ExpandTipsLoopScrollView.isShowArrow = EditorGUILayout.Toggle(GetTitle(7), m_ExpandTipsLoopScrollView.isShowArrow);
            if (m_ExpandTipsLoopScrollView.isShowArrow)
            {
                m_ExpandTipsLoopScrollView.pointingFirstArrow = (GameObject)EditorGUILayout.ObjectField(GetTitle(8), m_ExpandTipsLoopScrollView.pointingFirstArrow, typeof(GameObject), true);
                m_ExpandTipsLoopScrollView.pointingEndArrow = (GameObject)EditorGUILayout.ObjectField(GetTitle(9), m_ExpandTipsLoopScrollView.pointingEndArrow, typeof(GameObject), true);
            }
        }

        private string GetTitle(int index)
        {
            return LoopScrollViewSetting.EditorLanguageType == 0 ? m_TitlesC[index] : m_TitlesE[index];
        }
    }
}