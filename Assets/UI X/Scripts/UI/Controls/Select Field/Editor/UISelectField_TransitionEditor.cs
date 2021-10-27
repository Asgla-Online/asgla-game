using AsglaUI.UI;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUIEditor.UI {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UISelectField_Transition), true)]
	public class UISelectField_TransitionEditor : Editor {

		private SerializedProperty m_TargetGameObject;
		private SerializedProperty m_TargetGraphic;
		private SerializedProperty m_Transition;

		protected void OnEnable() {
			m_Transition = serializedObject.FindProperty("m_Transition");
			m_TargetGraphic = serializedObject.FindProperty("m_TargetGraphic");
			m_TargetGameObject = serializedObject.FindProperty("m_TargetGameObject");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(m_Transition, new GUIContent("Transition"));

			Selectable.Transition transition = (Selectable.Transition) m_Transition.enumValueIndex;
			Graphic graphic = m_TargetGraphic.objectReferenceValue as Graphic;

			// Check if the transition requires a graphic
			if (transition == Selectable.Transition.ColorTint || transition == Selectable.Transition.SpriteSwap) {
				EditorGUILayout.PropertyField(m_TargetGraphic, new GUIContent("Target Graphic"));

				if (transition == Selectable.Transition.ColorTint) {
					if (graphic == null) {
						EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.",
							MessageType.Info);
					} else {
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Colors"),
							new GUIContent("Colors"), true);
						if (EditorGUI.EndChangeCheck())
							graphic.canvasRenderer.SetColor(serializedObject.FindProperty("m_Colors")
								.FindPropertyRelative("m_NormalColor").colorValue);
					}
				} else if (transition == Selectable.Transition.SpriteSwap) {
					if (graphic as Image == null)
						EditorGUILayout.HelpBox(
							"You must have a Image target in order to use a sprite swap transition.", MessageType.Info);
					else
						EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SpriteState"),
							new GUIContent("Sprites"), true);
				}
			} else if (transition == Selectable.Transition.Animation) {
				EditorGUILayout.PropertyField(m_TargetGameObject, new GUIContent("Target Game Object"));

				GameObject targetGameObject = m_TargetGameObject.objectReferenceValue as GameObject;

				if (targetGameObject != null) {
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_AnimationTriggers"), true);

					Animator animator = (target as UISelectField_Transition).gameObject.GetComponent<Animator>();

					if (animator == null || animator.runtimeAnimatorController == null) {
						Rect controlRect = EditorGUILayout.GetControlRect();
						controlRect.xMin = controlRect.xMin + EditorGUIUtility.labelWidth;

						if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton)) {
							// Generate the animator controller
							AnimatorController animatorController =
								UIAnimatorControllerGenerator.GenerateAnimatorContoller(
									serializedObject.FindProperty("m_AnimationTriggers"), target.name);

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

	}
}