using System.Collections.Generic;
using AsglaUI.UI;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUIEditor.UI {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UIPressTransition))]
	public class UIPressTransitionEditor : Editor {

		private SerializedProperty m_ColorMultiplierProperty;
		private SerializedProperty m_DurationProperty;
		private SerializedProperty m_NormalColorProperty;
		private SerializedProperty m_NormalTriggerProperty;
		private SerializedProperty m_PressedColorProperty;
		private SerializedProperty m_PressedSpriteProperty;
		private SerializedProperty m_PressedTriggerProperty;
		private SerializedProperty m_TargetGameObjectProperty;
		private SerializedProperty m_TargetGraphicProperty;
		private SerializedProperty m_TransitionProperty;

		protected void OnEnable() {
			m_TransitionProperty = serializedObject.FindProperty("m_Transition");
			m_TargetGraphicProperty = serializedObject.FindProperty("m_TargetGraphic");
			m_TargetGameObjectProperty = serializedObject.FindProperty("m_TargetGameObject");
			m_NormalColorProperty = serializedObject.FindProperty("m_NormalColor");
			m_PressedColorProperty = serializedObject.FindProperty("m_PressedColor");
			m_DurationProperty = serializedObject.FindProperty("m_Duration");
			m_ColorMultiplierProperty = serializedObject.FindProperty("m_ColorMultiplier");
			m_PressedSpriteProperty = serializedObject.FindProperty("m_PressedSprite");
			m_NormalTriggerProperty = serializedObject.FindProperty("m_NormalTrigger");
			m_PressedTriggerProperty = serializedObject.FindProperty("m_PressedTrigger");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			UIHighlightTransition.Transition transition =
				(UIHighlightTransition.Transition) m_TransitionProperty.enumValueIndex;
			Graphic graphic = m_TargetGraphicProperty.objectReferenceValue as Graphic;
			GameObject targetGameObject = m_TargetGameObjectProperty.objectReferenceValue as GameObject;

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(m_TransitionProperty, new GUIContent("Transition"));
			EditorGUI.indentLevel++;

			// Check if the transition requires a graphic
			if (transition == UIHighlightTransition.Transition.ColorTint ||
			    transition == UIHighlightTransition.Transition.SpriteSwap ||
			    transition == UIHighlightTransition.Transition.TextColor) {
				EditorGUILayout.PropertyField(m_TargetGraphicProperty, new GUIContent("Target Graphic"));

				if (transition == UIHighlightTransition.Transition.ColorTint) {
					if (graphic == null) {
						EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.",
							MessageType.Info);
					} else {
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(m_NormalColorProperty, true);
						if (EditorGUI.EndChangeCheck())
							graphic.canvasRenderer.SetColor(m_NormalColorProperty.colorValue);

						EditorGUILayout.PropertyField(m_PressedColorProperty, true);
						EditorGUILayout.PropertyField(m_ColorMultiplierProperty, true);
						EditorGUILayout.PropertyField(m_DurationProperty, true);
					}
				} else if (transition == UIHighlightTransition.Transition.TextColor) {
					if (graphic is Text == false) {
						EditorGUILayout.HelpBox("You must have a Text target in order to use a text color transition.",
							MessageType.Info);
					} else {
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(m_NormalColorProperty, true);
						if (EditorGUI.EndChangeCheck())
							(graphic as Text).color = m_NormalColorProperty.colorValue;

						EditorGUILayout.PropertyField(m_PressedColorProperty, true);
						EditorGUILayout.PropertyField(m_DurationProperty, true);
					}
				} else if (transition == UIHighlightTransition.Transition.SpriteSwap) {
					if (graphic as Image == null)
						EditorGUILayout.HelpBox(
							"You must have a Image target in order to use a sprite swap transition.", MessageType.Info);
					else
						EditorGUILayout.PropertyField(m_PressedSpriteProperty, true);
				}
			} else if (transition == UIHighlightTransition.Transition.Animation) {
				EditorGUILayout.PropertyField(m_TargetGameObjectProperty, new GUIContent("Target GameObject"));

				if (targetGameObject == null) {
					EditorGUILayout.HelpBox(
						"You must have a Game Object target in order to use a animation transition.", MessageType.Info);
				} else {
					EditorGUILayout.PropertyField(m_NormalTriggerProperty, true);
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
			}

			serializedObject.ApplyModifiedProperties();
		}

		private AnimatorController GenerateAnimatorController() {
			// Prepare the triggers list
			List<string> triggers = new List<string>();

			triggers.Add(!string.IsNullOrEmpty(m_NormalTriggerProperty.stringValue)
				? m_NormalTriggerProperty.stringValue
				: "Normal");
			triggers.Add(!string.IsNullOrEmpty(m_PressedTriggerProperty.stringValue)
				? m_PressedTriggerProperty.stringValue
				: "Highlighted");

			return UIAnimatorControllerGenerator.GenerateAnimatorContoller(triggers,
				m_TargetGameObjectProperty.objectReferenceValue.name);
		}

	}
}