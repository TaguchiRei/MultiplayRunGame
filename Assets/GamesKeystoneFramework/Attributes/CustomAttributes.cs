using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GamesKeystoneFramework.Attributes
{
    /// <summary>
    /// インスペクター上で閲覧専用にする
    /// </summary>
    public class ReadOnlyInInspectorAttribute : PropertyAttribute
    {
    }

    /// <summary>
    /// インスペクター上で複数要素を持つ構造体をたためるようにする
    /// </summary>
    public class GroupingAttribute : PropertyAttribute
    {
    }

    //エディター専用のアテリビュート
#if UNITY_EDITOR

    #region ReadOnly

    [CustomPropertyDrawer(typeof(ReadOnlyInInspectorAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false; // 編集を無効化
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true; // 元に戻す
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }

    #endregion

    #region Grouping

    [CustomPropertyDrawer(typeof(GroupingAttribute))]
    public class GroupingDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GroupingAttribute grouping = (GroupingAttribute)attribute;

            // 元のGUIカラーを保存
            Color previousColor = GUI.backgroundColor;

            // 背景カラーを変更
            GUI.backgroundColor = Color.white;

            // ボックス風の背景を描画
            GUI.Box(position, GUIContent.none);

            // GUIカラーを元に戻す
            GUI.backgroundColor = previousColor;

            // 変数のフィールドを描画（ちょっと内側に）
            Rect innerRect = new Rect(position.x + 4, position.y + 2, position.width - 8, position.height - 4);
            EditorGUI.PropertyField(innerRect, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true) + 4;
        }
    }
    #endregion
    
#endif
}