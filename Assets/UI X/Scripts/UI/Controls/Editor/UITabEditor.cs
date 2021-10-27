using AsglaUI.UI;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUIEditor.UI {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UITab), true)]
	public class UITabEditor : Editor {

		public const string PREFS_KEY = "UITabEditor_";
		private GUIStyle m_FoldoutStyle;
		private SerializedProperty m_GraphicProperty;
		private SerializedProperty m_GroupProperty;
		private SerializedProperty m_ImageAnimationTriggersProperty;
		private SerializedProperty m_ImageColorsProperty;
		private SerializedProperty m_ImageSpriteStateProperty;
		private SerializedProperty m_ImageTargetProperty;
		private SerializedProperty m_ImageTransitionProperty;
		private SerializedProperty m_IsOnProperty;
		private SerializedProperty m_Navigation;
		private SerializedProperty m_OnValueChangedProperty;

		private SerializedProperty m_TargetContentProperty;
		private SerializedProperty m_TextColorsProperty;
		private SerializedProperty m_TextTargetProperty;
		private SerializedProperty m_TextTransitionProperty;
		private SerializedProperty m_TransitionProperty;
		private bool showImageProperties = true;
		private bool showTextProperties = true;

		protected virtual void OnEnable() {
			showImageProperties = EditorPrefs.GetBool(PREFS_KEY + "1", true);
			showTextProperties = EditorPrefs.GetBool(PREFS_KEY + "2", true);

			m_TargetContentProperty = serializedObject.FindProperty("m_TargetContent");
			m_ImageTargetProperty = serializedObject.FindProperty("m_ImageTarget");
			m_ImageTransitionProperty = serializedObject.FindProperty("m_ImageTransition");
			m_ImageColorsProperty = serializedObject.FindProperty("m_ImageColors");
			m_ImageSpriteStateProperty = serializedObject.FindProperty("m_ImageSpriteState");
			m_ImageAnimationTriggersProperty = serializedObject.FindProperty("m_ImageAnimationTriggers");
			m_TextTargetProperty = serializedObject.FindProperty("m_TextTarget");
			m_TextTransitionProperty = serializedObject.FindProperty("m_TextTransition");
			m_TextColorsProperty = serializedObject.FindProperty("m_TextColors");
			m_TransitionProperty = serializedObject.FindProperty("toggleTransition");
			m_GraphicProperty = serializedObject.FindProperty("graphic");
			m_GroupProperty = serializedObject.FindProperty("m_Group");
			m_IsOnProperty = serializedObject.FindProperty("m_IsOn");
			m_OnValueChangedProperty = serializedObject.FindProperty("onValueChanged");
			m_Navigation = serializedObject.FindProperty("m_Navigation");
		}

		public override void OnInspectorGUI() {
			if (m_FoldoutStyle == null) {
				m_FoldoutStyle = new GUIStyle(EditorStyles.foldout);
				m_FoldoutStyle.normal.textColor = Color.black;
				m_FoldoutStyle.fontStyle = FontStyle.Bold;
			}

			EditorGUI.BeginChangeCheck();

			serializedObject.Update();
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(m_TargetContentProperty, new GUIContent("Target Content"));
			EditorGUILayout.Space();
			DrawTargetImageProperties();
			EditorGUILayout.Space();
			DrawTargetTextProperties();
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.Space();
			DrawToggleProperties();

			if (EditorGUI.EndChangeCheck())
				foreach (Object target in targets)
					(target as UITab).OnProperyChange_Editor();
		}

		private void DrawTargetImageProperties() {
			bool newState = EditorGUILayout.Foldout(showImageProperties, "Image Target Properties", m_FoldoutStyle);

			if (newState != showImageProperties) {
				EditorPrefs.SetBool(PREFS_KEY + "1", newState);
				showImageProperties = newState;
			}

			if (showImageProperties) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				EditorGUILayout.PropertyField(m_ImageTargetProperty);

				// Check if image is set
				if (m_ImageTargetProperty.objectReferenceValue != null) {
					Image image = m_ImageTargetProperty.objectReferenceValue as Image;

					EditorGUILayout.PropertyField(m_ImageTransitionProperty);

					// Get the selected transition
					Selectable.Transition transition = (Selectable.Transition) m_ImageTransitionProperty.enumValueIndex;

					if (transition != Selectable.Transition.None) {
						EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
						if (transition == Selectable.Transition.ColorTint) {
							EditorGUILayout.PropertyField(m_ImageColorsProperty);
						} else if (transition == Selectable.Transition.SpriteSwap) {
							EditorGUILayout.PropertyField(m_ImageSpriteStateProperty);
						} else if (transition == Selectable.Transition.Animation) {
							EditorGUILayout.PropertyField(m_ImageAnimationTriggersProperty);

							Animator animator = image.gameObject.GetComponent<Animator>();

							if (animator == null || animator.runtimeAnimatorController == null) {
								Rect controlRect = EditorGUILayout.GetControlRect();
								controlRect.xMin = controlRect.xMin + EditorGUIUtility.labelWidth;

								if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton)) {
									// Generate the animator controller
									AnimatorController animatorController =
										UIAnimatorControllerGenerator.GenerateAnimatorContoller(
											m_ImageAnimationTriggersProperty, target.name);

									if (animatorController != null) {
										if (animator == null)
											animator = image.gameObject.AddComponent<Animator>();
										AnimatorController.SetAnimatorController(animator, animatorController);
									}
								}
							}
						}

						EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
					}
				}

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
		}

		private void DrawTargetTextProperties() {
			bool newState = EditorGUILayout.Foldout(showTextProperties, "Text Target Properties", m_FoldoutStyle);

			if (newState != showTextProperties) {
				EditorPrefs.SetBool(PREFS_KEY + "2", newState);
				showTextProperties = newState;
			}

			if (showTextProperties) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				EditorGUILayout.PropertyField(m_TextTargetProperty);

				// Check if image is set
				if (m_TextTargetProperty.objectReferenceValue != null) {
					EditorGUILayout.PropertyField(m_TextTransitionProperty);

					// Get the selected transition
					UITab.TextTransition transition = (UITab.TextTransition) m_TextTransitionProperty.enumValueIndex;

					if (transition != UITab.TextTransition.None) {
						EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
						if (transition == UITab.TextTransition.ColorTint)
							EditorGUILayout.PropertyField(m_TextColorsProperty);
						EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
					}
				}

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
		}

		private void DrawToggleProperties() {
			EditorGUILayout.LabelField("Toggle Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			serializedObject.Update();
			EditorGUILayout.PropertyField(m_IsOnProperty);
			EditorGUILayout.PropertyField(m_TransitionProperty);
			EditorGUILayout.PropertyField(m_GraphicProperty);
			EditorGUILayout.PropertyField(m_GroupProperty);
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(m_Navigation);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(m_OnValueChangedProperty);
			serializedObject.ApplyModifiedProperties();
		}

	}
}