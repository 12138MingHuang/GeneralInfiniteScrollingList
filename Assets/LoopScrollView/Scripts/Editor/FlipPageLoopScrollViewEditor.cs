using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LoopScrollViewNamespace
{
    [CustomEditor(typeof(FlipPageLoopScrollView))]
    public class FlipPageLoopScrollViewEditor : Editor
    {
        private readonly string[] m_TitlesE = new string[14] { "Direction", "Row Or Column", "Spacing", "Cell GameObject", "IsMaxRange", "isOpenNavIcon", "Sprite", "SelectSprite", "DefaultSprite", "SpriteSpacing", "SpriteSize", "Is Show Arrow", "Up or Left Arrow", "Down or Right Arrow" };
        private readonly string[] m_TitlesC = new string[14] { "方向", "行或列", "间距", "单元格Item", "是否可大范围拖动", "是否打开导航图标", "导航图标", "选中图标", "默认图标", "图标间距", "图标大小", "是否显示箭头", "上或左箭头", "下或右箭头" };
        
        private FlipPageLoopScrollView m_FlipPageLoopScrollView;
        
        public override void OnInspectorGUI()
        {
            m_FlipPageLoopScrollView = (FlipPageLoopScrollView)target;
            m_FlipPageLoopScrollView.direction = (LoopScrollDirection)EditorGUILayout.EnumPopup(GetTitle(0), m_FlipPageLoopScrollView.direction);

            m_FlipPageLoopScrollView.row = EditorGUILayout.IntField(GetTitle(1), m_FlipPageLoopScrollView.row);
            m_FlipPageLoopScrollView.spacing = EditorGUILayout.FloatField(GetTitle(2), m_FlipPageLoopScrollView.spacing);
            m_FlipPageLoopScrollView.cellGameObject = (GameObject)EditorGUILayout.ObjectField(GetTitle(3), m_FlipPageLoopScrollView.cellGameObject, typeof(GameObject), true);

            m_FlipPageLoopScrollView.isMaxRange = EditorGUILayout.ToggleLeft(GetTitle(4), m_FlipPageLoopScrollView.isMaxRange);
            
            m_FlipPageLoopScrollView.isOpenNavIcon = EditorGUILayout.ToggleLeft(GetTitle(5), m_FlipPageLoopScrollView.isOpenNavIcon);
            
            if (m_FlipPageLoopScrollView.isOpenNavIcon)
            {
                m_FlipPageLoopScrollView.objNavigation = (Transform)EditorGUILayout.ObjectField(GetTitle(6), m_FlipPageLoopScrollView.objNavigation, typeof(Transform), true);
                m_FlipPageLoopScrollView.selectIcon = (Sprite)EditorGUILayout.ObjectField(GetTitle(7), m_FlipPageLoopScrollView.selectIcon, typeof(Sprite), true);
                m_FlipPageLoopScrollView.normalIcon = (Sprite)EditorGUILayout.ObjectField(GetTitle(8), m_FlipPageLoopScrollView.normalIcon, typeof(Sprite), true);
                m_FlipPageLoopScrollView.iconSpacing = EditorGUILayout.FloatField(GetTitle(9), m_FlipPageLoopScrollView.iconSpacing);
                m_FlipPageLoopScrollView.iconSize = EditorGUILayout.FloatField(GetTitle(10), m_FlipPageLoopScrollView.iconSize);
            }

            m_FlipPageLoopScrollView.isShowArrow = EditorGUILayout.ToggleLeft(GetTitle(11), m_FlipPageLoopScrollView.isShowArrow);
            if (m_FlipPageLoopScrollView.isShowArrow)
            {
                m_FlipPageLoopScrollView.pointingFirstArrow = (GameObject)EditorGUILayout.ObjectField(GetTitle(12), m_FlipPageLoopScrollView.pointingFirstArrow, typeof(GameObject), true);
                m_FlipPageLoopScrollView.pointingEndArrow = (GameObject)EditorGUILayout.ObjectField(GetTitle(13), m_FlipPageLoopScrollView.pointingEndArrow, typeof(GameObject), true);
            }
        }
        
        private string GetTitle(int index)
        {
            return LoopScrollViewSetting.EditorLanguageType == 0 ? m_TitlesC[index] : m_TitlesE[index];
        }
    }
}
