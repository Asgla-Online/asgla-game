using System.Collections.Generic;
using AsglaUI.UI;
using TMPro;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUIEditor.UI {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UIHighlightTransition))]
	public class UIHighlightTransitionEditor : Editor {

		private SerializedProperty m_ActiveAlphaProperty;
		private SerializedProperty m_ActiveBoolProperty;
		private SerializedProperty m_ActiveColorProperty;
		private SerializedProperty m_ActiveSpriteProperty;
		private SerializedProperty m_ColorMultiplierProperty;
		private SerializedProperty m_DurationProperty;
		private SerializedProperty m_HighlightedAlphaProperty;
		private SerializedProperty m_HighlightedColorProperty;
		private SerializedProperty m_HighlightedSpriteProperty;
		private SerializedProperty m_HighlightedTriggerProperty;
		private SerializedProperty m_NormalAlphaProperty;
		private SerializedProperty m_NormalColorProperty;
		private SerializedProperty m_NormalTriggerProperty;
		private SerializedProperty m_PressedAlphaProperty;
		private SerializedProperty m_PressedColorProperty;
		private SerializedProperty m_PressedSpriteProperty;
		private SerializedProperty m_PressedTriggerProperty;
		private SerializedProperty m_SelectedAlphaProperty;
		private SerializedProperty m_SelectedColorProperty;
		private SerializedProperty m_SelectedSpriteProperty;
		private SerializedProperty m_SelectedTriggerProperty;
		private SerializedProperty m_TargetCanvasGroupProperty;
		private SerializedProperty m_TargetGameObjectProperty;
		private SerializedProperty m_TargetGraphicProperty;
		private SerializedProperty m_TargetToggleProperty;

		private SerializedProperty m_TransitionProperty;
		private SerializedProperty m_UseToggleProperty;

		protected void OnEnable() {
			m_TransitionProperty = serializedObject.FindProperty("m_Transition");
			m_TargetGraphicProperty = serializedObject.FindProperty("m_TargetGraphic");
			m_TargetGameObjectProperty = serializedObject.FindProperty("m_TargetGameObject");
			m_NormalColorProperty = serializedObject.FindProperty("m_NormalColor");
			m_HighlightedColorProperty = serializedObject.FindProperty("m_HighlightedColor");
			m_SelectedColorProperty = serializedObject.FindProperty("m_SelectedColor");
			m_PressedColorProperty = serializedObject.FindProperty("m_PressedColor");
			m_DurationProperty = serializedObject.FindProperty("m_Duration");
			m_ColorMultiplierProperty = serializedObject.FindProperty("m_ColorMultiplier");
			m_HighlightedSpriteProperty = serializedObject.FindProperty("m_HighlightedSprite");
			m_SelectedSpriteProperty = serializedObject.FindProperty("m_SelectedSprite");
			m_PressedSpriteProperty = serializedObject.FindProperty("m_PressedSprite");
			m_NormalTriggerProperty = serializedObject.FindProperty("m_NormalTrigger");
			m_HighlightedTriggerProperty = serializedObject.FindProperty("m_HighlightedTrigger");
			m_SelectedTriggerProperty = serializedObject.FindProperty("m_SelectedTrigger");
			m_PressedTriggerProperty = serializedObject.FindProperty("m_PressedTrigger");
			m_UseToggleProperty = serializedObject.FindProperty("m_UseToggle");
			m_TargetToggleProperty = serializedObject.FindProperty("m_TargetToggle");
			m_ActiveColorProperty = serializedObject.FindProperty("m_ActiveColor");
			m_ActiveSpriteProperty = serializedObject.FindProperty("m_ActiveSprite");
			m_ActiveBoolProperty = serializedObject.FindProperty("m_ActiveBool");
			m_TargetCanvasGroupProperty = serializedObject.FindProperty("m_TargetCanvasGroup");
			m_NormalAlphaProperty = serializedObject.FindProperty("m_NormalAlpha");
			m_HighlightedAlphaProperty = serializedObject.FindProperty("m_HighlightedAlpha");
			m_SelectedAlphaProperty = serializedObject.FindProperty("m_SelectedAlpha");
			m_PressedAlphaProperty = serializedObject.FindProperty("m_PressedAlpha");
			m_ActiveAlphaProperty = serializedObject.FindProperty("m_ActiveAlpha");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			UIHighlightTransition.Transition transition =
				(UIHighlightTransition.Transition) m_TransitionProperty.enumValueIndex;
			Graphic graphic = m_TargetGraphicProperty.objectReferenceValue as Graphic;
			GameObject targetGameObject = m_TargetGameObjectProperty.objectReferenceValue as GameObject;
			CanvasGroup targetCanvasGroup = m_TargetCanvasGroupProperty.objectReferenceValue as CanvasGroup;

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(m_TransitionProperty, new GUIContent("Transition"));
			EditorGUI.indentLevel++;

			// Check if the transition requires a graphic
			switch (transition) {
				case UIHighlightTransition.Transition.ColorTint:
				case UIHighlightTransition.Transition.SpriteSwap:
				case UIHighlightTransition.Transition.TextColor:
					EditorGUILayout.PropertyField(m_TargetGraphicProperty, new GUIContent("Target Graphic"));
					switch (transition) {
						case UIHighlightTransition.Transition.ColorTint:
							if (graphic == null) {
								EditorGUILayout.HelpBox(
									"You must have a Graphic target in order to use a color transition.",
									MessageType.Info);
							} else {
								EditorGUI.BeginChangeCheck();
								EditorGUILayout.PropertyField(m_NormalColorProperty, true);
								if (EditorGUI.EndChangeCheck())
									graphic.canvasRenderer.SetColor(m_NormalColorProperty.colorValue);

								EditorGUILayout.PropertyField(m_HighlightedColorProperty, true);
								EditorGUILayout.PropertyField(m_SelectedColorProperty, true);
								EditorGUILayout.PropertyField(m_PressedColorProperty, true);
								EditorGUILayout.PropertyField(m_ColorMultiplierProperty, true);
								EditorGUILayout.PropertyField(m_DurationProperty, true);
							}

							break;
						case UIHighlightTransition.Transition.TextColor:
							if (graphic == null || graphic is Text == false && graphic is TextMeshProUGUI == false) {
								EditorGUILayout.HelpBox(
									"You must have a Text target in order to use a text color transition.",
									MessageType.Info);
							} else {
								EditorGUI.BeginChangeCheck();
								EditorGUILayout.PropertyField(m_NormalColorProperty, true);
								if (EditorGUI.EndChangeCheck())
									switch (graphic) {
										case Text t:
											t.color = m_NormalColorProperty.colorValue;
											break;
										case TextMeshProUGUI t:
											t.color = m_NormalColorProperty.colorValue;
											break;
									}

								EditorGUILayout.PropertyField(m_HighlightedColorProperty, true);
								EditorGUILayout.PropertyField(m_SelectedColorProperty, true);
								EditorGUILayout.PropertyField(m_PressedColorProperty, true);
								EditorGUILayout.PropertyField(m_DurationProperty, true);
							}

							break;
						case UIHighlightTransition.Transition.SpriteSwap:
							if (graphic == null || graphic is Image == false) {
								EditorGUILayout.HelpBox(
									"You must have a Image target in order to use a sprite swap transition.",
									MessageType.Info);
							} else {
								EditorGUILayout.PropertyField(m_HighlightedSpriteProperty, true);
								EditorGUILayout.PropertyField(m_SelectedSpriteProperty, true);
								EditorGUILayout.PropertyField(m_PressedSpriteProperty, true);
							}

							break;
					}

					break;
				case UIHighlightTransition.Transition.Animation: {
					EditorGUILayout.PropertyField(m_TargetGameObjectProperty, new GUIContent("Target GameObject"));
					if (targetGameObject == null) {
						EditorGUILayout.HelpBox(
							"You must have a Game Object target in order to use a animation transition.",
							MessageType.Info);
					} else {
						EditorGUILayout.PropertyField(m_NormalTriggerProperty, true);
						EditorGUILayout.PropertyField(m_HighlightedTriggerProperty, true);
						EditorGUILayout.PropertyField(m_SelectedTriggerProperty, true);
						EditorGUILayout.PropertyField(m_PressedTriggerProperty, true);

						Animator animator = targetGameObject.GetComponent<Animator>();

						if (animator == null || animator.runtimeAnimatorController == null) {
							Rect controlRect = EditorGUILayout.GetControlRect();
							controlRect.xMin = controlRect.xMin + EditorGUIUtility.labelWidth;

							if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton)) {
								// Generate the animator controller
								AnimatorController animatorController = GenerateAnimatorController();

								if (animatorController != null) {
									if (animator == null)
										animator = targetGameObject.AddComponent<Animator>();
									AnimatorController.SetAnimatorController(animator, animatorController);
								}
							}
						}
					}

					break;
				}
				case UIHighlightTransition.Transition.CanvasGroup:
					EditorGUILayout.PropertyField(m_TargetCanvasGroupProperty, new GUIContent("Target Canvas Group"));
					if (targetCanvasGroup == null) {
						EditorGUILayout.HelpBox("You must have a CanvasGroup target in order to use this transition.",
							MessageType.Info);
					} else {
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(m_NormalAlphaProperty, true);
						if (EditorGUI.EndChangeCheck())
							targetCanvasGroup.alpha = m_NormalAlphaProperty.floatValue;

						EditorGUILayout.PropertyField(m_HighlightedAlphaProperty, true);
						EditorGUILayout.PropertyField(m_SelectedAlphaProperty, true);
						EditorGUILayout.PropertyField(m_PressedAlphaProperty, true);
						EditorGUILayout.PropertyField(m_DurationProperty, true);
					}

					break;
			}

			EditorGUILayout.PropertyField(m_UseToggleProperty, true);

			if (m_UseToggleProperty.boolValue) {
				EditorGUILayout.PropertyField(m_TargetToggleProperty, true);
				switch (transition) {
					case UIHighlightTransition.Transition.ColorTint:
						EditorGUILayout.PropertyField(m_ActiveColorProperty, true);
						break;
					case UIHighlightTransition.Transition.TextColor:
						EditorGUILayout.PropertyField(m_ActiveColorProperty, true);
						break;
					case UIHighlightTransition.Transition.SpriteSwap:
						EditorGUILayout.PropertyField(m_ActiveSpriteProperty, true);
						break;
					case UIHighlightTransition.Transition.Animation:
						EditorGUILayout.PropertyField(m_ActiveBoolProperty, true);
						break;
					case UIHighlightTransition.Transition.CanvasGroup:
						EditorGUILayout.PropertyField(m_ActiveAlphaProperty, true);
						break;
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		private AnimatorController GenerateAnimatorController() {
			// Prepare the triggers list
			List<string> triggers = new List<string>();

			triggers.Add(!string.IsNullOrEmpty(m_NormalTriggerProperty.stringValue)
				? m_NormalTriggerProperty.stringValue
				: "Normal");
			triggers.Add(!string.IsNullOrEmpty(m_HighlightedTriggerProperty.stringValue)
				? m_HighlightedTriggerProperty.stringValue
				: "Highlighted");
			triggers.Add(!string.IsNullOrEmpty(m_SelectedTriggerProperty.stringValue)
				? m_SelectedTriggerProperty.stringValue
				: "Selected");
			triggers.Add(!string.IsNullOrEmpty(m_PressedTriggerProperty.stringValue)
				? m_PressedTriggerProperty.stringValue
				: "Pressed");

			return UIAnimatorControllerGenerator.GenerateAnimatorContoller(triggers,
				m_TargetGameObjectProperty.objectReferenceValue.name);
		}

	}
}