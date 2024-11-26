using UnityEditor;
using UnityEngine;

namespace LoopScrollViewNamespace
{
    [CustomEditor(typeof(LoopScrollView), true)]
    public class LoopScrollViewEditor : Editor
    {
        private readonly string[] m_TitlesE = new string[7] { "Direction", "Row Or Column", "Spacing", "Cell GameObject", "Is Show Arrow", "Up or Left Arrow", "Down or Right Arrow" };
        private readonly string[] m_TitlesC = new string[7] { "方向", "行或列", "间距", "单元格item", "是否显示箭头", "上或左箭头", "下或右箭头" };

        // 无限滚动列表
        private LoopScrollView m_LoopScrollView;
        
        public override void OnInspectorGUI()
        {
            m_LoopScrollView = target as LoopScrollView;

            if (m_LoopScrollView == null)
            {
                EditorGUILayout.LabelField("LoopScrollView is null");
                return;
            }

            m_LoopScrollView.direction = (LoopScrollDirection)EditorGUILayout.EnumPopup(GetTitle(0), m_LoopScrollView.direction);
            m_LoopScrollView.row = EditorGUILayout.IntField(GetTitle(1), m_LoopScrollView.row);
            m_LoopScrollView.spacing = EditorGUILayout.FloatField(GetTitle(2), m_LoopScrollView.spacing);
            m_LoopScrollView.cellGameObject = (GameObject)EditorGUILayout.ObjectField(GetTitle(3), m_LoopScrollView.cellGameObject, typeof(GameObject), true);
            m_LoopScrollView.isShowArrow = EditorGUILayout.ToggleLeft(GetTitle(4), m_LoopScrollView.isShowArrow);

            if (m_LoopScrollView.isShowArrow)
            {
                m_LoopScrollView.pointingFirstArrow = (GameObject)EditorGUILayout.ObjectField(GetTitle(5), m_LoopScrollView.pointingFirstArrow, typeof(GameObject), true);
                m_LoopScrollView.pointingEndArrow = (GameObject)EditorGUILayout.ObjectField(GetTitle(6), m_LoopScrollView.pointingEndArrow, typeof(GameObject), true);
            }
        }
        
        private string GetTitle(int index)
        {
            return LoopScrollViewSetting.EditorLanguageType == 0 ? m_TitlesC[index] : m_TitlesE[index];
        }
    }
}
