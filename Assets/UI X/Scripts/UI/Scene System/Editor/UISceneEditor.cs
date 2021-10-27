using System.Collections.Generic;
using AsglaUI.UI;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AsglaUIEditor.UI {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UIScene))]
	public class UISceneEditor : Editor {

		private SerializedProperty m_AnimateInTrigger;
		private SerializedProperty m_AnimateOutTrigger;
		private SerializedProperty m_Content;
		private SerializedProperty m_FirstSelected;
		private SerializedProperty m_Id;
		private SerializedProperty m_IsActivated;
		private SerializedProperty m_OnActivate;
		private SerializedProperty m_OnDeactivate;
		private SerializedProperty m_Prefab;
		private SerializedProperty m_Resource;
		private SerializedProperty m_Transition;
		private SerializedProperty m_TransitionDuration;
		private SerializedProperty m_TransitionEasing;
		private SerializedProperty m_Type;

		protected virtual void OnEnable() {
			m_Id = serializedObject.FindProperty("m_Id");
			m_IsActivated = serializedObject.FindProperty("m_IsActivated");
			m_Type = serializedObject.FindProperty("m_Type");
			m_Content = serializedObject.FindProperty("m_Content");
			m_Prefab = serializedObject.FindProperty("m_Prefab");
			m_Resource = serializedObject.FindProperty("m_Resource");
			m_Transition = serializedObject.FindProperty("m_Transition");
			m_TransitionDuration = serializedObject.FindProperty("m_TransitionDuration");
			m_TransitionEasing = serializedObject.FindProperty("m_TransitionEasing");
			m_AnimateInTrigger = serializedObject.FindProperty("m_AnimateInTrigger");
			m_AnimateOutTrigger = serializedObject.FindProperty("m_AnimateOutTrigger");
			m_OnActivate = serializedObject.FindProperty("onActivate");
			m_OnDeactivate = serializedObject.FindProperty("onDeactivate");
			m_FirstSelected = serializedObject.FindProperty("m_FirstSelected");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.Separator();
			DrawGeneralProperties();
			EditorGUILayout.Separator();
			DrawTransitionProperties();
			EditorGUILayout.Separator();
			DrawNavigationProperties();
			EditorGUILayout.Separator();
			DrawEventsProperties();
			EditorGUILayout.Separator();
			serializedObject.ApplyModifiedProperties();
		}

		protected void DrawNavigationProperties() {
			EditorGUILayout.LabelField("Navigation Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			EditorGUILayout.PropertyField(m_FirstSelected, new GUIContent("First Selected"));

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		protected void DrawGeneralProperties() {
			EditorGUILayout.LabelField("General Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			EditorGUILayout.PropertyField(m_Id, new GUIContent("ID"));
			EditorGUILayout.PropertyField(m_IsActivated,
				new GUIContent("Is Activated", "Whether the scene is active or not."));
			EditorGUILayout.PropertyField(m_Type, new GUIContent("Type"));

			UIScene.Type type = (UIScene.Type) m_Type.enumValueIndex;

			if (type == UIScene.Type.Preloaded)
				EditorGUILayout.PropertyField(m_Content, new GUIContent("Content"));
			else if (type == UIScene.Type.Prefab)
				EditorGUILayout.PropertyField(m_Prefab, new GUIContent("Prefab"));
			else if (type == UIScene.Type.Resource)
				EditorGUILayout.PropertyField(m_Resource, new GUIContent("Resource"));

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		protected void DrawTransitionProperties() {
			EditorGUILayout.LabelField("Transition Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			EditorGUILayout.PropertyField(m_Transition, new GUIContent("Transition"));

			// Get the transition
			UIScene.Transition transition = (UIScene.Transition) m_Transition.enumValueIndex;

			if (transition != UIScene.Transition.None && transition != UIScene.Transition.Animation) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				EditorGUILayout.PropertyField(m_TransitionDuration, new GUIContent("Duration"));
				EditorGUILayout.PropertyField(m_TransitionEasing, new GUIContent("Easing"));
				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			} else if (transition == UIScene.Transition.Animation) {
				EditorGUILayout.PropertyField(m_AnimateInTrigger, new GUIContent("Animate In Trigger"));
				EditorGUILayout.PropertyField(m_AnimateOutTrigger, new GUIContent("Animate Out Trigger"));

				UIScene scene = target as UIScene;
				Animator animator = scene.animator;

				if (animator == null || animator.runtimeAnimatorController == null) {
					Rect controlRect = EditorGUILayout.GetControlRect();
					controlRect.xMin = controlRect.xMin + EditorGUIUtility.labelWidth;

					if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton)) {
						List<string> triggersList = new List<string>();
						triggersList.Add(m_AnimateInTrigger.stringValue);
						triggersList.Add(m_AnimateOutTrigger.stringValue);

						// Generate the animator controller
						AnimatorController animatorController =
							UIAnimatorControllerGenerator.GenerateAnimatorContoller(triggersList, scene.gameObject.name,
								true);

						if (animatorController != null) {
							if (animator == null)
								animator = scene.gameObject.AddComponent<Animator>();
							AnimatorController.SetAnimatorController(animator, animatorController);
						}
					}
				}
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		protected void DrawEventsProperties() {
			EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			EditorGUILayout.PropertyField(m_OnActivate, new GUIContent("On Activate"));
			EditorGUILayout.PropertyField(m_OnDeactivate, new GUIContent("On Deactivate"));

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

	}
}