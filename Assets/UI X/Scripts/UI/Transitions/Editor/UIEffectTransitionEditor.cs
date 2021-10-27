using AsglaUI.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUIEditor.UI {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UIEffectTransition))]
	public class UIEffectTransitionEditor : Editor {

		private SerializedProperty m_ActiveColorProperty;
		private SerializedProperty m_DurationProperty;
		private SerializedProperty m_HighlightedColorProperty;
		private SerializedProperty m_NormalColorProperty;
		private SerializedProperty m_PressedColorProperty;
		private SerializedProperty m_SelectedColorProperty;
		private SerializedProperty m_TargetEffectProperty;
		private SerializedProperty m_TargetToggleProperty;
		private SerializedProperty m_UseToggleProperty;

		protected void OnEnable() {
			m_TargetEffectProperty = serializedObject.FindProperty("m_TargetEffect");
			m_NormalColorProperty = serializedObject.FindProperty("m_NormalColor");
			m_HighlightedColorProperty = serializedObject.FindProperty("m_HighlightedColor");
			m_SelectedColorProperty = serializedObject.FindProperty("m_SelectedColor");
			m_PressedColorProperty = serializedObject.FindProperty("m_PressedColor");
			m_DurationProperty = serializedObject.FindProperty("m_Duration");
			m_UseToggleProperty = serializedObject.FindProperty("m_UseToggle");
			m_TargetToggleProperty = serializedObject.FindProperty("m_TargetToggle");
			m_ActiveColorProperty = serializedObject.FindProperty("m_ActiveColor");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			BaseMeshEffect effect = m_TargetEffectProperty.objectReferenceValue as BaseMeshEffect;

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(m_TargetEffectProperty, new GUIContent("Target Effect"));
			EditorGUI.indentLevel++;

			if (effect == null || effect is Shadow == false && effect is Outline == false) {
				EditorGUILayout.HelpBox(
					"You must have Shadow or Outline effect target in order to use this transition.", MessageType.Info);
			} else {
				EditorGUILayout.PropertyField(m_NormalColorProperty, true);
				EditorGUILayout.PropertyField(m_HighlightedColorProperty, true);
				EditorGUILayout.PropertyField(m_SelectedColorProperty, true);
				EditorGUILayout.PropertyField(m_PressedColorProperty, true);
				EditorGUILayout.PropertyField(m_DurationProperty, true);
			}

			EditorGUILayout.PropertyField(m_UseToggleProperty, true);

			if (m_UseToggleProperty.boolValue) {
				EditorGUILayout.PropertyField(m_TargetToggleProperty, true);
				EditorGUILayout.PropertyField(m_ActiveColorProperty, true);
			}

			serializedObject.ApplyModifiedProperties();
		}

	}
}