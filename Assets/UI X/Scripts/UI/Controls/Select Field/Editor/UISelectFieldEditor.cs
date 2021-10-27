using System;
using System.Collections.Generic;
using AsglaUI.UI;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUIEditor.UI {
	[CustomEditor(typeof(UISelectField), true)]
	public class UISelectFieldEditor : SelectableEditor {

		public const string PREFS_KEY = "UISelectFieldEditor_";
		private SerializedProperty m_AllowScrollRectProperty;
		private SerializedProperty m_AnimTriggerProperty;
		private SerializedProperty m_ColorBlockProperty;

		private SerializedProperty m_DirectionProperty;
		private GUIStyle m_FoldoutStyle;
		private SerializedProperty m_InteractableProperty;
		private SerializedProperty m_LabelTextProperty;
		private SerializedProperty m_ListAnimationDurationProperty;
		private SerializedProperty m_ListAnimationTypeProperty;
		private SerializedProperty m_ListAnimatorControllerProperty;
		private SerializedProperty m_ListBackgroundColorProperty;
		private SerializedProperty m_ListBackgroundSpriteProperty;
		private SerializedProperty m_ListBackgroundSpriteTypeProperty;
		private SerializedProperty m_ListlistAnimationCloseTriggerProperty;
		private SerializedProperty m_ListlistAnimationOpenTriggerProperty;
		private SerializedProperty m_ListMarginsProperty;
		private SerializedProperty m_ListPaddingProperty;
		private SerializedProperty m_ListSeparatorColorProperty;
		private SerializedProperty m_ListSeparatorHeightProperty;
		private SerializedProperty m_ListSeparatorSpriteProperty;
		private SerializedProperty m_ListSeparatorTypeProperty;
		private SerializedProperty m_ListSpacingProperty;
		private SerializedProperty m_ListStartSeparator;
		private SerializedProperty m_NavigationProperty;
		private SerializedProperty m_OnChangeProperty;
		private SerializedProperty m_OptionBackgroundAnimationTriggersProperty;
		private SerializedProperty m_OptionBackgroundAnimatorControllerProperty;
		private SerializedProperty m_OptionBackgroundSpriteColorProperty;
		private SerializedProperty m_OptionBackgroundSpriteProperty;
		private SerializedProperty m_OptionBackgroundSpriteStatesProperty;
		private SerializedProperty m_OptionBackgroundSpriteTypeProperty;
		private SerializedProperty m_OptionBackgroundTransColorsProperty;
		private SerializedProperty m_OptionBackgroundTransitionTypeProperty;
		private SerializedProperty m_OptionColorProperty;
		private SerializedProperty m_OptionFontProperty;
		private SerializedProperty m_OptionFontSizeProperty;
		private SerializedProperty m_OptionFontStyleProperty;
		private SerializedProperty m_OptionHoverOverlayColorBlockProperty;
		private SerializedProperty m_OptionHoverOverlayColorProperty;
		private SerializedProperty m_OptionHoverOverlayProperty;
		private SerializedProperty m_OptionPaddingProperty;
		private SerializedProperty m_OptionPressOverlayColorBlockProperty;
		private SerializedProperty m_OptionPressOverlayColorProperty;
		private SerializedProperty m_OptionPressOverlayProperty;
		private SerializedProperty m_OptionTextEffectColorProperty;
		private SerializedProperty m_OptionTextEffectDistanceProperty;
		private SerializedProperty m_OptionTextEffectTypeProperty;
		private SerializedProperty m_OptionTextEffectUseGraphicAlphaProperty;
		private SerializedProperty m_OptionTextTransitionColorsProperty;
		private SerializedProperty m_OptionTextTransitionTypeProperty;
		private SerializedProperty m_ScrollBarPrefabProperty;
		private SerializedProperty m_ScrollbarSpacingProperty;
		private SerializedProperty m_ScrollDecelerationRateProperty;
		private SerializedProperty m_ScrollElasticityProperty;
		private SerializedProperty m_ScrollInertiaProperty;
		private SerializedProperty m_ScrollListHeightProperty;
		private SerializedProperty m_ScrollMinOptionsProperty;
		private SerializedProperty m_ScrollMovementTypeProperty;
		private SerializedProperty m_ScrollSensitivityProperty;
		private SerializedProperty m_SpriteStateProperty;
		private SerializedProperty m_TargetGraphicProperty;
		private SerializedProperty m_TransitionProperty;
		private bool showListLayout = true;
		private bool showListSeparatorLayout = true;
		private bool showOptionBackgroundLayout = true;
		private bool showOptionHover = true;
		private bool showOptionLayout = true;
		private bool showOptionPress = true;
		private bool showScrollRectLayout = true;
		private bool showSelectLayout = true;

		protected override void OnEnable() {
			base.OnEnable();

			showSelectLayout = EditorPrefs.GetBool(PREFS_KEY + "1", true);
			showListLayout = EditorPrefs.GetBool(PREFS_KEY + "2", true);
			showListSeparatorLayout = EditorPrefs.GetBool(PREFS_KEY + "3", true);
			showOptionLayout = EditorPrefs.GetBool(PREFS_KEY + "4", true);
			showOptionBackgroundLayout = EditorPrefs.GetBool(PREFS_KEY + "5", true);
			showOptionHover = EditorPrefs.GetBool(PREFS_KEY + "6", true);
			showOptionPress = EditorPrefs.GetBool(PREFS_KEY + "7", true);
			showScrollRectLayout = EditorPrefs.GetBool(PREFS_KEY + "8", true);

			m_TargetGraphicProperty = serializedObject.FindProperty("m_TargetGraphic");
			m_InteractableProperty = serializedObject.FindProperty("m_Interactable");
			m_TransitionProperty = serializedObject.FindProperty("m_Transition");
			m_NavigationProperty = serializedObject.FindProperty("m_Navigation");
			m_ColorBlockProperty = serializedObject.FindProperty("colors");
			m_SpriteStateProperty = serializedObject.FindProperty("spriteState");
			m_AnimTriggerProperty = serializedObject.FindProperty("animationTriggers");
			m_DirectionProperty = serializedObject.FindProperty("m_Direction");
			m_LabelTextProperty = serializedObject.FindProperty("m_LabelText");
			m_ListBackgroundSpriteProperty = serializedObject.FindProperty("listBackgroundSprite");
			m_ListBackgroundSpriteTypeProperty = serializedObject.FindProperty("listBackgroundSpriteType");
			m_ListBackgroundColorProperty = serializedObject.FindProperty("listBackgroundColor");
			m_ListMarginsProperty = serializedObject.FindProperty("listMargins");
			m_ListPaddingProperty = serializedObject.FindProperty("listPadding");
			m_ListSpacingProperty = serializedObject.FindProperty("listSpacing");
			m_ListSeparatorSpriteProperty = serializedObject.FindProperty("listSeparatorSprite");
			m_ListSeparatorTypeProperty = serializedObject.FindProperty("listSeparatorType");
			m_ListSeparatorColorProperty = serializedObject.FindProperty("listSeparatorColor");
			m_ListSeparatorHeightProperty = serializedObject.FindProperty("listSeparatorHeight");
			m_ListStartSeparator = serializedObject.FindProperty("startSeparator");
			m_ListAnimationTypeProperty = serializedObject.FindProperty("listAnimationType");
			m_ListAnimationDurationProperty = serializedObject.FindProperty("listAnimationDuration");
			m_ListAnimatorControllerProperty = serializedObject.FindProperty("listAnimatorController");
			m_ListlistAnimationOpenTriggerProperty = serializedObject.FindProperty("listAnimationOpenTrigger");
			m_ListlistAnimationCloseTriggerProperty = serializedObject.FindProperty("listAnimationCloseTrigger");
			m_OptionFontProperty = serializedObject.FindProperty("optionFont");
			m_OptionFontSizeProperty = serializedObject.FindProperty("optionFontSize");
			m_OptionFontStyleProperty = serializedObject.FindProperty("optionFontStyle");
			m_OptionColorProperty = serializedObject.FindProperty("optionColor");
			m_OptionTextTransitionColorsProperty = serializedObject.FindProperty("optionTextTransitionColors");
			m_OptionPaddingProperty = serializedObject.FindProperty("optionPadding");
			m_OptionTextEffectTypeProperty = serializedObject.FindProperty("optionTextEffectType");
			m_OptionTextEffectColorProperty = serializedObject.FindProperty("optionTextEffectColor");
			m_OptionTextEffectDistanceProperty = serializedObject.FindProperty("optionTextEffectDistance");
			m_OptionTextEffectUseGraphicAlphaProperty =
				serializedObject.FindProperty("optionTextEffectUseGraphicAlpha");
			m_OptionTextTransitionTypeProperty = serializedObject.FindProperty("optionTextTransitionType");
			m_OptionBackgroundSpriteProperty = serializedObject.FindProperty("optionBackgroundSprite");
			m_OptionBackgroundSpriteTypeProperty = serializedObject.FindProperty("optionBackgroundSpriteType");
			m_OptionBackgroundSpriteColorProperty = serializedObject.FindProperty("optionBackgroundSpriteColor");
			m_OptionBackgroundTransitionTypeProperty = serializedObject.FindProperty("optionBackgroundTransitionType");
			m_OptionBackgroundTransColorsProperty = serializedObject.FindProperty("optionBackgroundTransColors");
			m_OptionBackgroundSpriteStatesProperty = serializedObject.FindProperty("optionBackgroundSpriteStates");
			m_OptionBackgroundAnimationTriggersProperty =
				serializedObject.FindProperty("optionBackgroundAnimationTriggers");
			m_OptionBackgroundAnimatorControllerProperty =
				serializedObject.FindProperty("optionBackgroundAnimatorController");
			m_OptionHoverOverlayProperty = serializedObject.FindProperty("optionHoverOverlay");
			m_OptionHoverOverlayColorProperty = serializedObject.FindProperty("optionHoverOverlayColor");
			m_OptionHoverOverlayColorBlockProperty = serializedObject.FindProperty("optionHoverOverlayColorBlock");
			m_OptionPressOverlayProperty = serializedObject.FindProperty("optionPressOverlay");
			m_OptionPressOverlayColorProperty = serializedObject.FindProperty("optionPressOverlayColor");
			m_OptionPressOverlayColorBlockProperty = serializedObject.FindProperty("optionPressOverlayColorBlock");

			m_AllowScrollRectProperty = serializedObject.FindProperty("allowScrollRect");
			m_ScrollMovementTypeProperty = serializedObject.FindProperty("scrollMovementType");
			m_ScrollElasticityProperty = serializedObject.FindProperty("scrollElasticity");
			m_ScrollInertiaProperty = serializedObject.FindProperty("scrollInertia");
			m_ScrollDecelerationRateProperty = serializedObject.FindProperty("scrollDecelerationRate");
			m_ScrollSensitivityProperty = serializedObject.FindProperty("scrollSensitivity");
			m_ScrollMinOptionsProperty = serializedObject.FindProperty("scrollMinOptions");
			m_ScrollListHeightProperty = serializedObject.FindProperty("scrollListHeight");
			m_ScrollBarPrefabProperty = serializedObject.FindProperty("scrollBarPrefab");
			m_ScrollbarSpacingProperty = serializedObject.FindProperty("scrollbarSpacing");

			m_OnChangeProperty = serializedObject.FindProperty("onChange");
		}

		public override void OnInspectorGUI() {
			if (m_FoldoutStyle == null) {
				m_FoldoutStyle = new GUIStyle(EditorStyles.foldout);
				m_FoldoutStyle.normal.textColor = Color.black;
				m_FoldoutStyle.fontStyle = FontStyle.Bold;
			}

			UISelectField select = target as UISelectField;
			serializedObject.Update();

			DrawOptionsArea();
			EditorGUILayout.Separator();
			DrawStringPopup("Default option", select.options.ToArray(), select.value, OnDefaultOptionSelected);
			EditorGUILayout.PropertyField(m_DirectionProperty);
			EditorGUILayout.PropertyField(m_InteractableProperty, new GUIContent("Interactable"));
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(m_LabelTextProperty, new GUIContent("Label Text"));
			EditorGUILayout.Separator();
			DrawSelectFieldLayotProperties();
			EditorGUILayout.Separator();
			DrawListLayoutProperties();
			EditorGUILayout.Separator();
			DrawScrollRectProperties();
			EditorGUILayout.Separator();
			DrawListSeparatorLayoutProperties();
			EditorGUILayout.Separator();
			DrawOptionLayoutProperties();
			EditorGUILayout.Separator();
			DrawOptionBackgroundLayoutProperties();
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(m_NavigationProperty);
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(m_OnChangeProperty);

			serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		///     Raises the default option selected event.
		/// </summary>
		/// <param name="value">Value.</param>
		private void OnDefaultOptionSelected(string value) {
			UISelectField select = target as UISelectField;

			Undo.RecordObject(select, "Select Field default option changed.");
			select.SelectOption(value);
			EditorUtility.SetDirty(select);
		}

		/// <summary>
		///     Draws the options area.
		/// </summary>
		private void DrawOptionsArea() {
			UISelectField select = target as UISelectField;

			// Place a label for the options
			EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);

			// Prepare the string to be used in the text area
			string text = "";
			foreach (string s in select.options)
				text += s + "\n";

			string modified = EditorGUILayout.TextArea(text, GUI.skin.textArea, GUILayout.Height(100f));

			// Check if the options have changed
			if (!modified.Equals(text)) {
				Undo.RecordObject(target, "UI Select Field changed.");

				string[] split = modified.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

				select.ClearOptions();

				foreach (string s in split)
					select.AddOption(s);

				if (string.IsNullOrEmpty(select.value) || !select.options.Contains(select.value))
					@select.value = @select.options.Count > 0 ? @select.options[0] : "";

				EditorUtility.SetDirty(target);
			}
		}

		/// <summary>
		///     Draws the select field layot properties.
		/// </summary>
		public void DrawSelectFieldLayotProperties() {
			bool newState = EditorGUILayout.Foldout(showSelectLayout, "Select Field Layout", m_FoldoutStyle);

			if (newState != showSelectLayout) {
				EditorPrefs.SetBool(PREFS_KEY + "1", newState);
				showSelectLayout = newState;
			}

			if (showSelectLayout) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				EditorGUILayout.PropertyField(m_TransitionProperty, new GUIContent("Transition"));

				Graphic graphic = m_TargetGraphicProperty.objectReferenceValue as Graphic;
				Selectable.Transition transition = (Selectable.Transition) m_TransitionProperty.enumValueIndex;

				// Check if the transition requires a graphic
				if (transition == Selectable.Transition.ColorTint || transition == Selectable.Transition.SpriteSwap) {
					EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
					EditorGUILayout.PropertyField(m_TargetGraphicProperty);
					EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				}

				// Check if we have a transition set
				if (transition != Selectable.Transition.None) {
					EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

					if (transition == Selectable.Transition.ColorTint) {
						if (graphic == null) {
							EditorGUILayout.HelpBox(
								"You must have a Graphic target in order to use a color transition.", MessageType.Info);
						} else {
							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField(m_ColorBlockProperty, true);
							if (EditorGUI.EndChangeCheck())
								graphic.canvasRenderer.SetColor(m_ColorBlockProperty
									.FindPropertyRelative("m_NormalColor").colorValue);
						}
					} else if (transition == Selectable.Transition.SpriteSwap) {
						if (graphic as Image == null)
							EditorGUILayout.HelpBox(
								"You must have a Image target in order to use a sprite swap transition.",
								MessageType.Info);
						else
							EditorGUILayout.PropertyField(m_SpriteStateProperty, true);
					} else if (transition == Selectable.Transition.Animation) {
						EditorGUILayout.PropertyField(m_AnimTriggerProperty, true);

						Animator animator = (target as UISelectField).animator;

						if (animator == null || animator.runtimeAnimatorController == null) {
							Rect controlRect = EditorGUILayout.GetControlRect();
							controlRect.xMin = controlRect.xMin + EditorGUIUtility.labelWidth;

							if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton)) {
								// Generate the animator controller
								AnimatorController animatorController =
									UIAnimatorControllerGenerator.GenerateAnimatorContoller(m_AnimTriggerProperty,
										target.name);

								if (animatorController != null) {
									if (animator == null)
										animator = (target as UISelectField).gameObject.AddComponent<Animator>();
									AnimatorController.SetAnimatorController(animator, animatorController);
								}
							}
						}
					}

					EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				}

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
		}

		/// <summary>
		///     Draws the list layout properties.
		/// </summary>
		public void DrawListLayoutProperties() {
			bool newState = EditorGUILayout.Foldout(showListLayout, "List Layout", m_FoldoutStyle);

			if (newState != showListLayout) {
				EditorPrefs.SetBool(PREFS_KEY + "2", newState);
				showListLayout = newState;
			}

			if (showListLayout) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				EditorGUILayout.PropertyField(m_ListBackgroundSpriteProperty, new GUIContent("Sprite"));
				if (m_ListBackgroundSpriteProperty.objectReferenceValue != null) {
					EditorGUILayout.PropertyField(m_ListBackgroundSpriteTypeProperty, new GUIContent("Sprite Type"));
					EditorGUILayout.PropertyField(m_ListBackgroundColorProperty, new GUIContent("Sprite Color"));
				}

				EditorGUILayout.PropertyField(m_ListMarginsProperty, new GUIContent("Margin"), true);
				EditorGUILayout.PropertyField(m_ListPaddingProperty, new GUIContent("Padding"), true);
				EditorGUILayout.PropertyField(m_ListSpacingProperty, new GUIContent("Spacing"), true);
				EditorGUILayout.PropertyField(m_ListAnimationTypeProperty, new GUIContent("Transition"), true);

				UISelectField.ListAnimationType animationType =
					(UISelectField.ListAnimationType) m_ListAnimationTypeProperty.enumValueIndex;

				if (animationType == UISelectField.ListAnimationType.Fade) {
					EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
					EditorGUILayout.PropertyField(m_ListAnimationDurationProperty, new GUIContent("Duration"), true);
					EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				} else if (animationType == UISelectField.ListAnimationType.Animation) {
					EditorGUILayout.PropertyField(m_ListAnimatorControllerProperty,
						new GUIContent("Animator Controller"));
					EditorGUILayout.PropertyField(m_ListlistAnimationOpenTriggerProperty,
						new GUIContent("Open Trigger"));
					EditorGUILayout.PropertyField(m_ListlistAnimationCloseTriggerProperty,
						new GUIContent("Close Trigger"));

					if (m_ListAnimatorControllerProperty.objectReferenceValue == null) {
						Rect controlRect = EditorGUILayout.GetControlRect();
						controlRect.xMin = controlRect.xMin + EditorGUIUtility.labelWidth;

						if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton)) {
							// Prepare the triggers list
							List<string> triggers = new List<string>();
							triggers.Add(!string.IsNullOrEmpty(m_ListlistAnimationOpenTriggerProperty.stringValue)
								? m_ListlistAnimationOpenTriggerProperty.stringValue
								: "Open");
							triggers.Add(!string.IsNullOrEmpty(m_ListlistAnimationCloseTriggerProperty.stringValue)
								? m_ListlistAnimationCloseTriggerProperty.stringValue
								: "Close");

							// Generate the animator controller
							AnimatorController animatorController =
								UIAnimatorControllerGenerator.GenerateAnimatorContoller(triggers,
									target.name + " - List");

							// Apply the controller to the property
							if (animatorController != null)
								m_ListAnimatorControllerProperty.objectReferenceValue = animatorController;
						}
					}
				}

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
		}

		/// <summary>
		///     Draws the scroll rect properties.
		/// </summary>
		public void DrawScrollRectProperties() {
			bool newState = EditorGUILayout.Foldout(showScrollRectLayout, "Scrolling Properties", m_FoldoutStyle);

			if (newState != showScrollRectLayout) {
				EditorPrefs.SetBool(PREFS_KEY + "8", newState);
				showScrollRectLayout = newState;
			}

			if (showScrollRectLayout) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				EditorGUILayout.PropertyField(m_AllowScrollRectProperty, new GUIContent("Allow Scroll Rect"));
				EditorGUILayout.PropertyField(m_ScrollMovementTypeProperty, new GUIContent("Movement Type"));

				if (m_ScrollMovementTypeProperty.enumValueIndex == 1) {
					EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
					EditorGUILayout.PropertyField(m_ScrollElasticityProperty, new GUIContent("Elasticity"));
					EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				}

				EditorGUILayout.PropertyField(m_ScrollInertiaProperty, new GUIContent("Inertia"));

				if (m_ScrollInertiaProperty.boolValue) {
					EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
					EditorGUILayout.PropertyField(m_ScrollDecelerationRateProperty,
						new GUIContent("Deceleration Rate"));
					EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				}

				EditorGUILayout.PropertyField(m_ScrollSensitivityProperty, new GUIContent("Scroll Sensitivity"));
				EditorGUILayout.PropertyField(m_ScrollMinOptionsProperty,
					new GUIContent("Minimum Options", "The number of options required to use the scroll rect."));
				EditorGUILayout.PropertyField(m_ScrollListHeightProperty, new GUIContent("List Height"));
				EditorGUILayout.PropertyField(m_ScrollBarPrefabProperty, new GUIContent("Scroll Bar Prefab"));
				EditorGUILayout.PropertyField(m_ScrollbarSpacingProperty, new GUIContent("Scroll Bar Spacing"));

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
		}

		/// <summary>
		///     Draws the list separator layout properties.
		/// </summary>
		public void DrawListSeparatorLayoutProperties() {
			bool newState = EditorGUILayout.Foldout(showListSeparatorLayout, "List Separator Layout", m_FoldoutStyle);

			if (newState != showListSeparatorLayout) {
				EditorPrefs.SetBool(PREFS_KEY + "3", newState);
				showListSeparatorLayout = newState;
			}

			if (showListSeparatorLayout) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				EditorGUILayout.PropertyField(m_ListSeparatorSpriteProperty, new GUIContent("Sprite"));

				if (m_ListSeparatorSpriteProperty.objectReferenceValue != null) {
					EditorGUILayout.PropertyField(m_ListSeparatorTypeProperty, new GUIContent("Sprite Type"));
					EditorGUILayout.PropertyField(m_ListSeparatorColorProperty, new GUIContent("Sprite Color"));
					EditorGUILayout.PropertyField(m_ListSeparatorHeightProperty, new GUIContent("Override Height"));
					EditorGUILayout.PropertyField(m_ListStartSeparator, new GUIContent("Start Separator"));
				}

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
		}

		/// <summary>
		///     Draws the option layout properties.
		/// </summary>
		public void DrawOptionLayoutProperties() {
			bool newState = EditorGUILayout.Foldout(showOptionLayout, "Option Layout", m_FoldoutStyle);

			if (newState != showOptionLayout) {
				EditorPrefs.SetBool(PREFS_KEY + "4", newState);
				showOptionLayout = newState;
			}

			if (showOptionLayout) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				EditorGUILayout.PropertyField(m_OptionFontProperty, new GUIContent("Font"));
				EditorGUILayout.PropertyField(m_OptionFontSizeProperty, new GUIContent("Font size"));
				EditorGUILayout.PropertyField(m_OptionFontStyleProperty, new GUIContent("Font style"));
				EditorGUILayout.PropertyField(m_OptionColorProperty, new GUIContent("Color Normal"));
				EditorGUILayout.PropertyField(m_OptionPaddingProperty, new GUIContent("Padding"), true);
				EditorGUILayout.PropertyField(m_OptionTextEffectTypeProperty, new GUIContent("Effect Type"));

				UISelectField.OptionTextEffectType textEffect =
					(UISelectField.OptionTextEffectType) m_OptionTextEffectTypeProperty.enumValueIndex;

				if (textEffect != UISelectField.OptionTextEffectType.None) {
					EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
					EditorGUILayout.PropertyField(m_OptionTextEffectColorProperty, new GUIContent("Color"), true);
					EditorGUILayout.PropertyField(m_OptionTextEffectDistanceProperty, new GUIContent("Distance"), true);
					EditorGUILayout.PropertyField(m_OptionTextEffectUseGraphicAlphaProperty,
						new GUIContent("Use graphic alpha"), true);
					EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				}

				EditorGUILayout.PropertyField(m_OptionTextTransitionTypeProperty, new GUIContent("Transition"));

				UISelectField.OptionTextTransitionType textTransition =
					(UISelectField.OptionTextTransitionType) m_OptionTextTransitionTypeProperty.enumValueIndex;

				if (textTransition == UISelectField.OptionTextTransitionType.CrossFade) {
					EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
					EditorGUILayout.PropertyField(m_OptionTextTransitionColorsProperty, true);
					EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				}

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
		}

		/// <summary>
		///     Draws the option background layout properties.
		/// </summary>
		public void DrawOptionBackgroundLayoutProperties() {
			bool newState =
				EditorGUILayout.Foldout(showOptionBackgroundLayout, "Option Background Layout", m_FoldoutStyle);

			if (newState != showOptionBackgroundLayout) {
				EditorPrefs.SetBool(PREFS_KEY + "5", newState);
				showOptionBackgroundLayout = newState;
			}

			if (showOptionBackgroundLayout) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				EditorGUILayout.PropertyField(m_OptionBackgroundSpriteProperty, new GUIContent("Sprite"));

				if (m_OptionBackgroundSpriteProperty.objectReferenceValue != null) {
					EditorGUILayout.PropertyField(m_OptionBackgroundSpriteTypeProperty, new GUIContent("Sprite Type"));
					EditorGUILayout.PropertyField(m_OptionBackgroundSpriteColorProperty,
						new GUIContent("Sprite Color"));
					EditorGUILayout.PropertyField(m_OptionBackgroundTransitionTypeProperty,
						new GUIContent("Transition"));

					Selectable.Transition optionBgTransition =
						(Selectable.Transition) m_OptionBackgroundTransitionTypeProperty.enumValueIndex;

					if (optionBgTransition != Selectable.Transition.None) {
						EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
						if (optionBgTransition == Selectable.Transition.ColorTint) {
							EditorGUILayout.PropertyField(m_OptionBackgroundTransColorsProperty, true);
						} else if (optionBgTransition == Selectable.Transition.SpriteSwap) {
							EditorGUILayout.PropertyField(m_OptionBackgroundSpriteStatesProperty, true);
						} else if (optionBgTransition == Selectable.Transition.Animation) {
							EditorGUILayout.PropertyField(m_OptionBackgroundAnimatorControllerProperty,
								new GUIContent("Animator Controller"));
							EditorGUILayout.PropertyField(m_OptionBackgroundAnimationTriggersProperty, true);

							if (m_OptionBackgroundAnimatorControllerProperty.objectReferenceValue == null) {
								Rect controlRect = EditorGUILayout.GetControlRect();
								controlRect.xMin = controlRect.xMin + EditorGUIUtility.labelWidth;

								if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton)) {
									// Generate the animator controller
									AnimatorController animatorController =
										UIAnimatorControllerGenerator.GenerateAnimatorContoller(
											m_OptionBackgroundAnimationTriggersProperty,
											target.name + " - Option Background");

									// Apply the controller to the property
									if (animatorController != null)
										m_OptionBackgroundAnimatorControllerProperty.objectReferenceValue =
											animatorController;
								}
							}
						}

						EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
					}
				}

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}

			EditorGUILayout.Separator();

			bool newState2 = EditorGUILayout.Foldout(showOptionHover, "Option Hover Overlay", m_FoldoutStyle);

			if (newState2 != showOptionHover) {
				EditorPrefs.SetBool(PREFS_KEY + "6", newState2);
				showOptionHover = newState2;
			}

			if (showOptionHover) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				EditorGUILayout.PropertyField(m_OptionHoverOverlayProperty, new GUIContent("Sprite"));
				EditorGUILayout.PropertyField(m_OptionHoverOverlayColorProperty, new GUIContent("Sprite Color"));
				if (m_OptionHoverOverlayProperty.objectReferenceValue != null) {
					EditorGUILayout.LabelField("Transition", EditorStyles.boldLabel);
					EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
					EditorGUILayout.PropertyField(m_OptionHoverOverlayColorBlockProperty, new GUIContent("Colors"),
						true);
					EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				}

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}

			EditorGUILayout.Separator();

			bool newState3 = EditorGUILayout.Foldout(showOptionPress, "Option Press Overlay", m_FoldoutStyle);

			if (newState3 != showOptionPress) {
				EditorPrefs.SetBool(PREFS_KEY + "7", newState3);
				showOptionPress = newState3;
			}

			if (showOptionPress) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				EditorGUILayout.PropertyField(m_OptionPressOverlayProperty, new GUIContent("Sprite"));
				EditorGUILayout.PropertyField(m_OptionPressOverlayColorProperty, new GUIContent("Sprite Color"));
				if (m_OptionPressOverlayProperty.objectReferenceValue != null) {
					EditorGUILayout.LabelField("Transition", EditorStyles.boldLabel);
					EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
					EditorGUILayout.PropertyField(m_OptionPressOverlayColorBlockProperty, new GUIContent("Colors"),
						true);
					EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				}

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
		}

		/// <summary>
		///     Draws a string popup field.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="list">Array of values.</param>
		/// <param name="selected">The selected value.</param>
		/// <param name="onChange">On change.</param>
		public static void DrawStringPopup(string label, string[] list, string selected, Action<string> onChange) {
			string newValue = string.Empty;
			GUI.changed = false;

			if (list != null && list.Length > 0) {
				int index = 0;

				// Make sure we have a selection
				if (string.IsNullOrEmpty(selected))
					selected = list[0];

				// Find the index of the selection
				else if (!string.IsNullOrEmpty(selected))
					for (int i = 0; i < list.Length; ++i)
						if (selected.Equals(list[i], StringComparison.OrdinalIgnoreCase)) {
							index = i;
							break;
						}

				// Draw the sprite selection popup
				index = string.IsNullOrEmpty(label)
					? EditorGUILayout.Popup(index, list)
					: EditorGUILayout.Popup(label, index, list);

				// Save the selected value
				newValue = list[index];
			}

			// Invoke the event with the selected value
			if (GUI.changed)
				onChange.Invoke(newValue);
		}

	}
}