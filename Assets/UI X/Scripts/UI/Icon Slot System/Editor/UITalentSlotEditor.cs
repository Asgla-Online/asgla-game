using AsglaUI.UI;
using UnityEditor;
using UnityEngine;

namespace AsglaUIEditor.UI {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UITalentSlot), true)]
	public class UITalentSlotEditor : UISlotBaseEditor {

		private SerializedProperty m_pointsActiveColorProperty;
		private SerializedProperty m_pointsMaxColorProperty;
		private SerializedProperty m_pointsMinColorProperty;

		private SerializedProperty m_PointsTextProperty;

		protected override void OnEnable() {
			base.OnEnable();
			m_PointsTextProperty = serializedObject.FindProperty("m_PointsText");
			m_pointsMinColorProperty = serializedObject.FindProperty("m_pointsMinColor");
			m_pointsMaxColorProperty = serializedObject.FindProperty("m_pointsMaxColor");
			m_pointsActiveColorProperty = serializedObject.FindProperty("m_pointsActiveColor");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.Separator();
			DrawPointsProperties();
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.Separator();
			base.OnInspectorGUI();
			EditorGUILayout.Separator();
		}

		protected void DrawPointsProperties() {
			EditorGUILayout.LabelField("Points Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			EditorGUILayout.PropertyField(m_PointsTextProperty, new GUIContent("Text Component"));
			EditorGUILayout.PropertyField(m_pointsMinColorProperty, new GUIContent("Minimum Color"));
			EditorGUILayout.PropertyField(m_pointsMaxColorProperty, new GUIContent("Maximum Color"));
			EditorGUILayout.PropertyField(m_pointsActiveColorProperty, new GUIContent("Active Color"));

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

	}
}