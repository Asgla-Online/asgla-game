using AsglaUI.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUIEditor.UI {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UIProgressBar), true)]
	public class UIProgressBarEditor : Editor {

		private UIProgressBar bar;
		private SerializedProperty m_FillAmount;
		private SerializedProperty m_FillSizing;
		private SerializedProperty m_MaxWidth;
		private SerializedProperty m_MinWidth;
		private SerializedProperty m_OnChange;
		private SerializedProperty m_Sprites;
		private SerializedProperty m_Steps;
		private SerializedProperty m_TargetImage;
		private SerializedProperty m_TargetTransform;

		private SerializedProperty m_Type;

		protected void OnEnable() {
			bar = target as UIProgressBar;

			m_Type = serializedObject.FindProperty("m_Type");
			m_TargetImage = serializedObject.FindProperty("m_TargetImage");
			m_Sprites = serializedObject.FindProperty("m_Sprites");
			m_TargetTransform = serializedObject.FindProperty("m_TargetTransform");
			m_FillSizing = serializedObject.FindProperty("m_FillSizing");
			m_MinWidth = serializedObject.FindProperty("m_MinWidth");
			m_MaxWidth = serializedObject.FindProperty("m_MaxWidth");
			m_FillAmount = serializedObject.FindProperty("m_FillAmount");
			m_Steps = serializedObject.FindProperty("m_Steps");
			m_OnChange = serializedObject.FindProperty("onChange");
		}

		public override void OnInspectorGUI() {
			bool amountChanged = false;

			serializedObject.Update();

			EditorGUILayout.Separator();

			EditorGUILayout.LabelField("Fill Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_Type, new GUIContent("Fill Type"));
			// Normal type
			if (m_Type.enumValueIndex == 0) {
				EditorGUILayout.PropertyField(m_TargetImage, new GUIContent("Fill Target"));
				if (m_TargetImage.objectReferenceValue != null &&
				    (m_TargetImage.objectReferenceValue as Image).type != Image.Type.Filled)
					EditorGUILayout.HelpBox("The target image must be of type Filled.", MessageType.Info);
			} else if (m_Type.enumValueIndex == 1) {
				EditorGUILayout.PropertyField(m_TargetTransform, new GUIContent("Fill Target"));
				EditorGUILayout.PropertyField(m_FillSizing, new GUIContent("Fill Sizing"));
				if (m_FillSizing.enumValueIndex == 1) {
					EditorGUILayout.PropertyField(m_MinWidth, new GUIContent("Min Width"));
					EditorGUILayout.PropertyField(m_MaxWidth, new GUIContent("Max Width"));
				}
			} else if (m_Type.enumValueIndex == 2) {
				EditorGUILayout.PropertyField(m_TargetImage, new GUIContent("Fill Target"));
				EditorGUILayout.PropertyField(m_Sprites, new GUIContent("Sprites"), true);
			}

			EditorGUILayout.PropertyField(m_Steps, new GUIContent("Steps"));
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Separator();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(m_FillAmount, new GUIContent("Fill Amount"));
			if (EditorGUI.EndChangeCheck())
				amountChanged = true;

			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(m_OnChange, true);

			serializedObject.ApplyModifiedProperties();

			if (amountChanged) {
				bar.UpdateBarFill();
				bar.onChange.Invoke(m_FillAmount.floatValue);
			}
		}

	}
}