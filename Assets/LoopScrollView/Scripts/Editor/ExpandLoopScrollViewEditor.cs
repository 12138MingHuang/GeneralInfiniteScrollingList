using UnityEditor;
using UnityEngine;

namespace LoopScrollViewNamespace
{
    [CustomEditor(typeof(ExpandLoopScrollView))]
    public class ExpandLoopScrollViewEditor : Editor
    {
        private readonly string[] m_TitlesE = new string[10] { "Direction", "Row Or Column", "Spacing", "Cell GameObject", "ExpandCell", "isDefaultExpand", "BackgroundScale", "Is Show Arrow", "Up or Left Arrow", "Down or Right Arrow" };
        private readonly string[] m_TitlesC = new string[10] { "方向", "行或列", "间距", "单元格Item", "展开的单元Item", "是否默认展开", "背景缩放", "是否显示箭头", "上或左箭头", "下或右箭头" };
        
        private ExpandLoopScrollView m_ExpandLoopScrollView;

        public override void OnInspectorGUI()
        {
            m_ExpandLoopScrollView = (ExpandLoopScrollView)target;
            
            m_ExpandLoopScrollView.direction = (LoopScrollDirection)EditorGUILayout.EnumPopup(GetTitle(0), m_ExpandLoopScrollView.direction);
            m_ExpandLoopScrollView.row = EditorGUILayout.IntField(GetTitle(1), m_ExpandLoopScrollView.row);
            m_ExpandLoopScrollView.spacing = EditorGUILayout.FloatField(GetTitle(2), m_ExpandLoopScrollView.spacing);
            m_ExpandLoopScrollView.expandButton = (GameObject)EditorGUILayout.ObjectField(GetTitle(3), m_ExpandLoopScrollView.expandButton, typeof(GameObject), true);
            m_ExpandLoopScrollView.cellGameObject = (GameObject)EditorGUILayout.ObjectField(GetTitle(4), m_ExpandLoopScrollView.cellGameObject, typeof(GameObject), true);
            m_ExpandLoopScrollView.isExpand = EditorGUILayout.Toggle(GetTitle(5), m_ExpandLoopScrollView.isExpand);
            // m_ExpandLoopScrollView.backgroundMargin = EditorGUILayout.FloatField(GetTitle(6), m_ExpandLoopScrollView.backgroundMargin);
            
            m_ExpandLoopScrollView.isShowArrow = EditorGUILayout.Toggle(GetTitle(7), m_ExpandLoopScrollView.isShowArrow);
            if (m_ExpandLoopScrollView.isShowArrow)
            {
                m_ExpandLoopScrollView.pointingFirstArrow = (GameObject)EditorGUILayout.ObjectField(GetTitle(8), m_ExpandLoopScrollView.pointingFirstArrow, typeof(GameObject), true);
                m_ExpandLoopScrollView.pointingEndArrow = (GameObject)EditorGUILayout.ObjectField(GetTitle(9), m_ExpandLoopScrollView.pointingEndArrow, typeof(GameObject), true);
            }
        }

        private string GetTitle(int index)
        {
            return LoopScrollViewSetting.EditorLanguageType == 0 ? m_TitlesC[index] : m_TitlesE[index];
        }
    }
}