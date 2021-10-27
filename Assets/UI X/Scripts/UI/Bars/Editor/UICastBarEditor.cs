using AsglaUI.UI;
using UnityEditor;
using UnityEngine;

namespace AsglaUIEditor.UI {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UICastBar), true)]
	public class UICastBarEditor : Editor {

		private SerializedProperty m_BrindToFront;
		private SerializedProperty m_DisplayTime;
		private SerializedProperty m_FillImage;
		private SerializedProperty m_FullTimeFormat;
		private SerializedProperty m_FullTimeLabel;
		private SerializedProperty m_HideDelay;
		private SerializedProperty m_IconFrame;
		private SerializedProperty m_IconImage;
		private SerializedProperty m_InTransition;
		private SerializedProperty m_InTransitionDuration;
		private SerializedProperty m_NormalColors;
		private SerializedProperty m_OnFinishColors;
		private SerializedProperty m_OnInterruptColors;
		private SerializedProperty m_OutTransition;
		private SerializedProperty m_OutTransitionDuration;

		private SerializedProperty m_ProgressBar;
		private SerializedProperty m_TimeFormat;
		private SerializedProperty m_TimeLabel;
		private SerializedProperty m_TitleLabel;
		private SerializedProperty m_UseSpellIcon;

		protected void OnEnable() {
			m_ProgressBar = serializedObject.FindProperty("m_ProgressBar");
			m_TitleLabel = serializedObject.FindProperty("m_TitleLabel");
			m_TimeLabel = serializedObject.FindProperty("m_TimeLabel");
			m_DisplayTime = serializedObject.FindProperty("m_DisplayTime");
			m_TimeFormat = serializedObject.FindProperty("m_TimeFormat");
			m_FullTimeLabel = serializedObject.FindProperty("m_FullTimeLabel");
			m_FullTimeFormat = serializedObject.FindProperty("m_FullTimeFormat");
			m_UseSpellIcon = serializedObject.FindProperty("m_UseSpellIcon");
			m_IconFrame = serializedObject.FindProperty("m_IconFrame");
			m_IconImage = serializedObject.FindProperty("m_IconImage");
			m_NormalColors = serializedObject.FindProperty("m_NormalColors");
			m_OnInterruptColors = serializedObject.FindProperty("m_OnInterruptColors");
			m_OnFinishColors = serializedObject.FindProperty("m_OnFinishColors");
			m_FillImage = serializedObject.FindProperty("m_FillImage");

			m_InTransition = serializedObject.FindProperty("m_InTransition");
			m_InTransitionDuration = serializedObject.FindProperty("m_InTransitionDuration");
			m_OutTransition = serializedObject.FindProperty("m_OutTransition");
			m_OutTransitionDuration = serializedObject.FindProperty("m_OutTransitionDuration");
			m_HideDelay = serializedObject.FindProperty("m_HideDelay");
			m_BrindToFront = serializedObject.FindProperty("m_BrindToFront");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			EditorGUILayout.Separator();

			EditorGUILayout.PropertyField(m_ProgressBar, new GUIContent("Progress Bar"));
			EditorGUILayout.PropertyField(m_FillImage, new GUIContent("Fill Image"));

			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Spell Title", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_TitleLabel, new GUIContent("Text"));
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Time Text", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_TimeLabel, new GUIContent("Text"));
			EditorGUILayout.PropertyField(m_DisplayTime, new GUIContent("Display"));
			EditorGUILayout.PropertyField(m_TimeFormat, new GUIContent("Format"));
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Full Time Text", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_FullTimeLabel, new GUIContent("Text"));
			EditorGUILayout.PropertyField(m_FullTimeFormat, new GUIContent("Format"));
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Spell Icon", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_UseSpellIcon, new GUIContent("Use Icon"));
			if (m_UseSpellIcon.boolValue) {
				EditorGUILayout.PropertyField(m_IconFrame, new GUIContent("Frame"));
				EditorGUILayout.PropertyField(m_IconImage, new GUIContent("Image"));
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_NormalColors, new GUIContent("Normal"), true);
			EditorGUILayout.PropertyField(m_OnInterruptColors, new GUIContent("On Interrupt"), true);
			EditorGUILayout.PropertyField(m_OnFinishColors, new GUIContent("On Finish"), true);
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Show Transition", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_InTransition, new GUIContent("Transition"));
			if (m_InTransition.enumValueIndex == 1)
				EditorGUILayout.PropertyField(m_InTransitionDuration, new GUIContent("Duration"));
			EditorGUILayout.PropertyField(m_BrindToFront, new GUIContent("Bring to Front"));
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Hide Transition", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_OutTransition, new GUIContent("Transition"));
			if (m_OutTransition.enumValueIndex == 1)
				EditorGUILayout.PropertyField(m_OutTransitionDuration, new GUIContent("Duration"));
			EditorGUILayout.PropertyField(m_HideDelay, new GUIContent("Hide Delay"));
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Separator();

			if (GUILayout.Button("Test Interrupt", EditorStyles.miniButton))
				(target as UICastBar).Interrupt();

			EditorGUILayout.Separator();

			serializedObject.ApplyModifiedProperties();
		}

	}
}