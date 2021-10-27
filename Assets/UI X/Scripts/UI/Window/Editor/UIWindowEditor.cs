using AsglaUI.UI;
using UnityEditor;
using UnityEngine;

namespace AsglaUIEditor.UI {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UIWindow))]
	public class UIWindowEditor : Editor {

		private SerializedProperty m_CustomWindowIdProperty;
		private SerializedProperty m_EscapeKeyActionProperty;
		private SerializedProperty m_FocusAllowReparent;
		private SerializedProperty m_StartingStateProperty;
		private SerializedProperty m_TransitionDurationProperty;
		private SerializedProperty m_TransitionEasingProperty;
		private SerializedProperty m_TransitionProperty;
		private SerializedProperty m_UseBlackOverlayProperty;

		private SerializedProperty m_WindowIdProperty;
		private SerializedProperty onTransitionBeginProperty;
		private SerializedProperty onTransitionCompleteProperty;

		protected virtual void OnEnable() {
			m_WindowIdProperty = serializedObject.FindProperty("m_WindowId");
			m_CustomWindowIdProperty = serializedObject.FindProperty("m_CustomWindowId");
			m_StartingStateProperty = serializedObject.FindProperty("m_StartingState");
			m_EscapeKeyActionProperty = serializedObject.FindProperty("m_EscapeKeyAction");
			m_UseBlackOverlayProperty = serializedObject.FindProperty("m_UseBlackOverlay");
			m_FocusAllowReparent = serializedObject.FindProperty("m_FocusAllowReparent");
			m_TransitionProperty = serializedObject.FindProperty("m_Transition");
			m_TransitionEasingProperty = serializedObject.FindProperty("m_TransitionEasing");
			m_TransitionDurationProperty = serializedObject.FindProperty("m_TransitionDuration");
			onTransitionBeginProperty = serializedObject.FindProperty("onTransitionBegin");
			onTransitionCompleteProperty = serializedObject.FindProperty("onTransitionComplete");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.Separator();
			DrawGeneralProperties();
			EditorGUILayout.Separator();
			DrawFocusProperties();
			EditorGUILayout.Separator();
			DrawTransitionProperties();
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(onTransitionBeginProperty, new GUIContent("On Transition Begin"), true);
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(onTransitionCompleteProperty, new GUIContent("On Transition Complete"), true);

			serializedObject.ApplyModifiedProperties();
		}

		protected void DrawGeneralProperties() {
			UIWindowID id = (UIWindowID) m_WindowIdProperty.enumValueIndex;

			EditorGUILayout.LabelField("General Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			EditorGUILayout.PropertyField(m_WindowIdProperty, new GUIContent("ID"));
			if (id == UIWindowID.Custom)
				EditorGUILayout.PropertyField(m_CustomWindowIdProperty, new GUIContent("Custom ID"));
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(m_StartingStateProperty, new GUIContent("Starting State"));
			if (EditorGUI.EndChangeCheck())
				foreach (Object target in targets)
					(target as UIWindow).ApplyVisualState(
						(UIWindow.VisualState) m_StartingStateProperty.enumValueIndex);
			EditorGUILayout.PropertyField(m_EscapeKeyActionProperty, new GUIContent("Escape Key Action"));
			EditorGUILayout.PropertyField(m_UseBlackOverlayProperty, new GUIContent("Use Black Overlay"));

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		protected void DrawFocusProperties() {
			EditorGUILayout.LabelField("Focus Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			EditorGUILayout.PropertyField(m_FocusAllowReparent, new GUIContent("Allow Re-parenting"));

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		protected void DrawTransitionProperties() {
			EditorGUILayout.LabelField("Transition Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			EditorGUILayout.PropertyField(m_TransitionProperty, new GUIContent("Transition"));

			// Get the transition
			UIWindow.Transition transition = (UIWindow.Transition) m_TransitionProperty.enumValueIndex;

			if (transition == UIWindow.Transition.Fade) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				EditorGUILayout.PropertyField(m_TransitionEasingProperty, new GUIContent("Easing"));
				EditorGUILayout.PropertyField(m_TransitionDurationProperty, new GUIContent("Duration"));
				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

	}
}