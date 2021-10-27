using AsglaUI.UI;
using UnityEditor;
using UnityEngine;

namespace AsglaUIEditor.UI {
	[CustomEditor(typeof(UITooltip), true)]
	public class UITooltipEditor : Editor {

		private SerializedProperty m_AnchoredOffsetProperty;
		private SerializedProperty m_AnchorGraphicOffsetProperty;
		private SerializedProperty m_AnchorGraphicProperty;
		private SerializedProperty m_AnchoringProperty;

		private SerializedProperty m_DefaultWidthProperty;
		private SerializedProperty m_followMouseProperty;
		private SerializedProperty m_OffsetProperty;
		private SerializedProperty m_TransitionDurationProperty;
		private SerializedProperty m_TransitionEasingProperty;

		private SerializedProperty m_TransitionProperty;
		//private SerializedProperty m_OnAnchorEventProperty;

		protected virtual void OnEnable() {
			m_DefaultWidthProperty = serializedObject.FindProperty("m_DefaultWidth");
			m_AnchoringProperty = serializedObject.FindProperty("m_Anchoring");
			m_AnchorGraphicProperty = serializedObject.FindProperty("m_AnchorGraphic");
			m_AnchorGraphicOffsetProperty = serializedObject.FindProperty("m_AnchorGraphicOffset");
			m_followMouseProperty = serializedObject.FindProperty("m_followMouse");
			m_OffsetProperty = serializedObject.FindProperty("m_Offset");
			m_AnchoredOffsetProperty = serializedObject.FindProperty("m_AnchoredOffset");
			m_TransitionProperty = serializedObject.FindProperty("m_Transition");
			m_TransitionEasingProperty = serializedObject.FindProperty("m_TransitionEasing");
			m_TransitionDurationProperty = serializedObject.FindProperty("m_TransitionDuration");
			//this.m_OnAnchorEventProperty = this.serializedObject.FindProperty("onAnchorEvent");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.Separator();
			DrawGeneralProperties();
			EditorGUILayout.Separator();
			DrawAnchorProperties();
			EditorGUILayout.Separator();
			DrawTransitionProperties();
			EditorGUILayout.Separator();
			//EditorGUILayout.PropertyField(this.m_OnAnchorEventProperty, new GUIContent("On Anchor Event"), true);
			//EditorGUILayout.Separator();
			serializedObject.ApplyModifiedProperties();
		}

		protected void DrawGeneralProperties() {
			EditorGUILayout.LabelField("General Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUIUtility.labelWidth = 150f;
			EditorGUILayout.PropertyField(m_followMouseProperty, new GUIContent("Follow Mouse"));
			EditorGUILayout.PropertyField(m_OffsetProperty, new GUIContent("Offset"));
			EditorGUILayout.PropertyField(m_AnchoredOffsetProperty, new GUIContent("Anchored Offset"));
			EditorGUILayout.PropertyField(m_DefaultWidthProperty, new GUIContent("Default Width"));
			EditorGUILayout.PropertyField(m_AnchoringProperty, new GUIContent("Anchoring"));
			EditorGUIUtility.labelWidth = 120f;
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		protected void DrawTransitionProperties() {
			EditorGUILayout.LabelField("Transition Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			EditorGUILayout.PropertyField(m_TransitionProperty, new GUIContent("Transition"));

			UITooltip.Transition transition = (UITooltip.Transition) m_TransitionProperty.enumValueIndex;

			if (transition != UITooltip.Transition.None) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				if (transition == UITooltip.Transition.Fade) {
					EditorGUILayout.PropertyField(m_TransitionEasingProperty, new GUIContent("Easing"));
					EditorGUILayout.PropertyField(m_TransitionDurationProperty, new GUIContent("Duration"));
				}

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		protected void DrawAnchorProperties() {
			EditorGUILayout.LabelField("Anchor Graphic Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_AnchorGraphicProperty, new GUIContent("Graphic"));
			EditorGUILayout.PropertyField(m_AnchorGraphicOffsetProperty, new GUIContent("Offset"));
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

	}
}