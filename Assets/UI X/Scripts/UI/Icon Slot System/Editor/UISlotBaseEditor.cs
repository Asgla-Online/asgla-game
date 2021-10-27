using System.Collections.Generic;
using AsglaUI.UI;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUIEditor.UI {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UISlotBase), true)]
	public class UISlotBaseEditor : Editor {

		private SerializedProperty hoverHighlightColorProperty;
		private SerializedProperty hoverHighlightTriggerProperty;
		private SerializedProperty hoverNormalColorProperty;
		private SerializedProperty hoverNormalTriggerProperty;
		private SerializedProperty hoverOverrideSpriteProperty;
		private SerializedProperty hoverTargetGraphicProperty;
		private SerializedProperty hoverTransitionDurationProperty;
		private SerializedProperty hoverTransitionProperty;
		private SerializedProperty m_AllowThrowAwayProperty;
		private SerializedProperty m_CloneTargetProperty;
		private SerializedProperty m_DragAndDropEnabledProperty;
		private SerializedProperty m_DragKeyModifierProperty;

		private SerializedProperty m_IconGraphicProperty;
		private SerializedProperty m_IsStaticProperty;
		private SerializedProperty m_TooltipDelayProperty;
		private SerializedProperty m_TooltipEnabledProperty;
		private SerializedProperty pressForceHoverNormalProperty;
		private SerializedProperty pressNormalColorProperty;
		private SerializedProperty pressNormalTriggerProperty;
		private SerializedProperty pressOverrideSpriteProperty;
		private SerializedProperty pressPressColorProperty;
		private SerializedProperty pressPressTriggerProperty;
		private SerializedProperty pressTargetGraphicProperty;
		private SerializedProperty pressTransitionDurationProperty;
		private SerializedProperty pressTransitionInstaOutProperty;
		private SerializedProperty pressTransitionProperty;

		protected virtual void OnEnable() {
			m_IconGraphicProperty = serializedObject.FindProperty("iconGraphic");
			m_CloneTargetProperty = serializedObject.FindProperty("m_CloneTarget");
			m_DragAndDropEnabledProperty = serializedObject.FindProperty("m_DragAndDropEnabled");
			m_IsStaticProperty = serializedObject.FindProperty("m_IsStatic");
			m_AllowThrowAwayProperty = serializedObject.FindProperty("m_AllowThrowAway");
			m_DragKeyModifierProperty = serializedObject.FindProperty("m_DragKeyModifier");
			m_TooltipEnabledProperty = serializedObject.FindProperty("m_TooltipEnabled");
			m_TooltipDelayProperty = serializedObject.FindProperty("m_TooltipDelay");
			hoverTransitionProperty = serializedObject.FindProperty("hoverTransition");
			hoverTargetGraphicProperty = serializedObject.FindProperty("hoverTargetGraphic");
			hoverNormalColorProperty = serializedObject.FindProperty("hoverNormalColor");
			hoverHighlightColorProperty = serializedObject.FindProperty("hoverHighlightColor");
			hoverTransitionDurationProperty = serializedObject.FindProperty("hoverTransitionDuration");
			hoverOverrideSpriteProperty = serializedObject.FindProperty("hoverOverrideSprite");
			hoverNormalTriggerProperty = serializedObject.FindProperty("hoverNormalTrigger");
			hoverHighlightTriggerProperty = serializedObject.FindProperty("hoverHighlightTrigger");
			pressTransitionProperty = serializedObject.FindProperty("pressTransition");
			pressTargetGraphicProperty = serializedObject.FindProperty("pressTargetGraphic");
			pressNormalColorProperty = serializedObject.FindProperty("pressNormalColor");
			pressPressColorProperty = serializedObject.FindProperty("pressPressColor");
			pressTransitionDurationProperty = serializedObject.FindProperty("pressTransitionDuration");
			pressTransitionInstaOutProperty = serializedObject.FindProperty("m_PressTransitionInstaOut");
			pressOverrideSpriteProperty = serializedObject.FindProperty("pressOverrideSprite");
			pressNormalTriggerProperty = serializedObject.FindProperty("pressNormalTrigger");
			pressPressTriggerProperty = serializedObject.FindProperty("pressPressTrigger");
			pressForceHoverNormalProperty = serializedObject.FindProperty("m_PressForceHoverNormal");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			DrawIconGraphicProperties();
			EditorGUILayout.Separator();
			DrawDragAndDropProperties();
			EditorGUILayout.Separator();
			DrawTooltipProperties();
			EditorGUILayout.Separator();
			DrawHoverProperties();
			EditorGUILayout.Separator();
			DrawPressProperties();
			serializedObject.ApplyModifiedProperties();
		}

		protected void DrawIconGraphicProperties() {
			EditorGUILayout.LabelField("Icon Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			EditorGUILayout.PropertyField(m_IconGraphicProperty, new GUIContent("Icon Graphic"));
			EditorGUILayout.PropertyField(m_CloneTargetProperty, new GUIContent("Clone Target"));

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		protected void DrawDragAndDropProperties() {
			EditorGUILayout.LabelField("Drag & Drop Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUIUtility.labelWidth = 150f;

			EditorGUILayout.PropertyField(m_DragAndDropEnabledProperty, new GUIContent("Enabled"));
			if (m_DragAndDropEnabledProperty.boolValue) {
				EditorGUILayout.PropertyField(m_DragKeyModifierProperty, new GUIContent("Drag Key Modifier"));
				EditorGUILayout.PropertyField(m_IsStaticProperty, new GUIContent("Is Static"));
				EditorGUILayout.PropertyField(m_AllowThrowAwayProperty, new GUIContent("Allow Throw Away"));
			}

			EditorGUIUtility.labelWidth = 120f;
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		protected void DrawTooltipProperties() {
			EditorGUILayout.LabelField("Tooltip Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUIUtility.labelWidth = 150f;

			EditorGUILayout.PropertyField(m_TooltipEnabledProperty, new GUIContent("Enabled"));
			if (m_TooltipEnabledProperty.boolValue)
				EditorGUILayout.PropertyField(m_TooltipDelayProperty, new GUIContent("Display Delay"));

			EditorGUIUtility.labelWidth = 120f;
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		protected void DrawHoverProperties() {
			EditorGUILayout.LabelField("Hovered State Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			EditorGUILayout.PropertyField(hoverTargetGraphicProperty, new GUIContent("Target Graphic"));
			EditorGUILayout.PropertyField(hoverTransitionProperty, new GUIContent("Transition"));

			Graphic graphic = hoverTargetGraphicProperty.objectReferenceValue as Graphic;
			UISlotBase.Transition transition = (UISlotBase.Transition) hoverTransitionProperty.enumValueIndex;

			if (transition != UISlotBase.Transition.None) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				if (transition == UISlotBase.Transition.ColorTint) {
					if (graphic == null) {
						EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.",
							MessageType.Info);
					} else {
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(hoverNormalColorProperty, new GUIContent("Normal"));
						if (EditorGUI.EndChangeCheck())
							graphic.canvasRenderer.SetColor(hoverNormalColorProperty.colorValue);

						EditorGUILayout.PropertyField(hoverHighlightColorProperty, new GUIContent("Highlighted"));
						EditorGUILayout.PropertyField(hoverTransitionDurationProperty, new GUIContent("Duration"));
					}
				} else if (transition == UISlotBase.Transition.SpriteSwap) {
					if (graphic as Image == null)
						EditorGUILayout.HelpBox(
							"You must have a Image target in order to use a sprite swap transition.", MessageType.Info);
					else
						EditorGUILayout.PropertyField(hoverOverrideSpriteProperty, new GUIContent("Override Sprite"));
				} else if (transition == UISlotBase.Transition.Animation) {
					if (graphic == null) {
						EditorGUILayout.HelpBox(
							"You must have a Graphic target in order to use a animation transition.", MessageType.Info);
					} else {
						EditorGUILayout.PropertyField(hoverNormalTriggerProperty, new GUIContent("Normal"));
						EditorGUILayout.PropertyField(hoverHighlightTriggerProperty, new GUIContent("Highlighted"));

						Animator animator = graphic.gameObject.GetComponent<Animator>();

						if (animator == null || animator.runtimeAnimatorController == null) {
							Rect controlRect = EditorGUILayout.GetControlRect();
							controlRect.xMin = controlRect.xMin + EditorGUIUtility.labelWidth;

							if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton)) {
								// Generate the animator controller
								AnimatorController animatorController = GenerateHoverAnimatorController();

								if (animatorController != null) {
									if (animator == null)
										animator = graphic.gameObject.AddComponent<Animator>();
									AnimatorController.SetAnimatorController(animator, animatorController);
								}
							}
						}
					}
				}

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		protected void DrawPressProperties() {
			EditorGUILayout.LabelField("Pressed State Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			EditorGUILayout.PropertyField(pressTargetGraphicProperty, new GUIContent("Target Graphic"));
			EditorGUILayout.PropertyField(pressTransitionProperty, new GUIContent("Transition"));

			Graphic graphic = pressTargetGraphicProperty.objectReferenceValue as Graphic;
			UISlotBase.Transition transition = (UISlotBase.Transition) pressTransitionProperty.enumValueIndex;

			if (transition != UISlotBase.Transition.None) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				if (transition == UISlotBase.Transition.ColorTint) {
					if (graphic == null) {
						EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.",
							MessageType.Info);
					} else {
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(pressNormalColorProperty, new GUIContent("Normal"));
						if (EditorGUI.EndChangeCheck())
							graphic.canvasRenderer.SetColor(pressNormalColorProperty.colorValue);

						EditorGUILayout.PropertyField(pressPressColorProperty, new GUIContent("Pressed"));
						EditorGUILayout.PropertyField(pressTransitionDurationProperty, new GUIContent("Duration"));
						EditorGUIUtility.labelWidth = 150f;
						EditorGUILayout.PropertyField(pressTransitionInstaOutProperty, new GUIContent("Instant Out"));
						EditorGUIUtility.labelWidth = 120f;
					}
				} else if (transition == UISlotBase.Transition.SpriteSwap) {
					if (graphic as Image == null)
						EditorGUILayout.HelpBox(
							"You must have a Image target in order to use a sprite swap transition.", MessageType.Info);
					else
						EditorGUILayout.PropertyField(pressOverrideSpriteProperty, new GUIContent("Override Sprite"));
				} else if (transition == UISlotBase.Transition.Animation) {
					if (graphic == null) {
						EditorGUILayout.HelpBox(
							"You must have a Graphic target in order to use a animation transition.", MessageType.Info);
					} else {
						EditorGUILayout.PropertyField(pressNormalTriggerProperty, new GUIContent("Normal"));
						EditorGUILayout.PropertyField(pressPressTriggerProperty, new GUIContent("Pressed"));

						Animator animator = graphic.gameObject.GetComponent<Animator>();

						if (animator == null || animator.runtimeAnimatorController == null) {
							Rect controlRect = EditorGUILayout.GetControlRect();
							controlRect.xMin = controlRect.xMin + EditorGUIUtility.labelWidth;

							if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton)) {
								// Generate the animator controller
								AnimatorController animatorController = GeneratePressAnimatorController();

								if (animatorController != null) {
									if (animator == null)
										animator = graphic.gameObject.AddComponent<Animator>();
									AnimatorController.SetAnimatorController(animator, animatorController);
								}
							}
						}
					}
				}

				EditorGUIUtility.labelWidth = 150f;
				EditorGUILayout.PropertyField(pressForceHoverNormalProperty, new GUIContent("Force Hover Normal"));
				EditorGUIUtility.labelWidth = 120f;
				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		protected AnimatorController GenerateHoverAnimatorController() {
			// Prepare the triggers list
			List<string> triggers = new List<string>();

			triggers.Add(!string.IsNullOrEmpty(hoverNormalTriggerProperty.stringValue)
				? hoverNormalTriggerProperty.stringValue
				: "Normal");
			triggers.Add(!string.IsNullOrEmpty(hoverHighlightTriggerProperty.stringValue)
				? hoverHighlightTriggerProperty.stringValue
				: "Highlighted");

			return UIAnimatorControllerGenerator.GenerateAnimatorContoller(triggers,
				hoverTargetGraphicProperty.objectReferenceValue.name);
		}

		protected AnimatorController GeneratePressAnimatorController() {
			// Prepare the triggers list
			List<string> triggers = new List<string>();

			triggers.Add(!string.IsNullOrEmpty(pressNormalTriggerProperty.stringValue)
				? pressNormalTriggerProperty.stringValue
				: "Normal");
			triggers.Add(!string.IsNullOrEmpty(pressPressTriggerProperty.stringValue)
				? pressPressTriggerProperty.stringValue
				: "Pressed");

			return UIAnimatorControllerGenerator.GenerateAnimatorContoller(triggers,
				pressTargetGraphicProperty.objectReferenceValue.name);
		}

	}
}