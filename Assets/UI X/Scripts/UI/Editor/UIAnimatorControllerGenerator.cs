using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AsglaUIEditor.UI {
	public class UIAnimatorControllerGenerator {

		/// <summary>
		///     Generate an the animator contoller.
		/// </summary>
		/// <returns>The animator contoller.</returns>
		/// <param name="triggersProperty">Triggers property.</param>
		/// <param name="preferredName">Preferred name.</param>
		public static AnimatorController GenerateAnimatorContoller(SerializedProperty triggersProperty,
			string preferredName) {
			// Prepare the triggers list
			List<string> triggersList = new List<string>();

			SerializedProperty serializedProperty = triggersProperty.Copy();
			SerializedProperty endProperty = serializedProperty.GetEndProperty();

			while (serializedProperty.NextVisible(true) &&
			       !SerializedProperty.EqualContents(serializedProperty, endProperty))
				triggersList.Add(!string.IsNullOrEmpty(serializedProperty.stringValue)
					? serializedProperty.stringValue
					: serializedProperty.name);

			// Generate the animator controller
			return GenerateAnimatorContoller(triggersList, preferredName);
		}

		/// <summary>
		///     Generates an animator contoller.
		/// </summary>
		/// <returns>The animator contoller.</returns>
		/// <param name="animationTriggers">Animation triggers.</param>
		/// <param name="preferredName">The preferred animator name.</param>
		public static AnimatorController
			GenerateAnimatorContoller(List<string> animationTriggers, string preferredName) {
			return GenerateAnimatorContoller(animationTriggers, preferredName, false);
		}

		/// <summary>
		///     Generates an animator contoller.
		/// </summary>
		/// <returns>The animator contoller.</returns>
		/// <param name="animationTriggers">Animation triggers.</param>
		/// <param name="preferredName">The preferred animator name.</param>
		/// <param name="initialState">If animator should have initial state.</param>
		public static AnimatorController GenerateAnimatorContoller(List<string> animationTriggers,
			string preferredName,
			bool initialState) {
			if (string.IsNullOrEmpty(preferredName))
				preferredName = "New Animator Controller";

			string saveControllerPath = GetSaveControllerPath(preferredName);

			if (string.IsNullOrEmpty(saveControllerPath))
				return null;

			AnimatorController animatorController =
				AnimatorController.CreateAnimatorControllerAtPath(saveControllerPath);

			if (initialState)
				GenerateInitialState(animatorController);

			foreach (string trigger in animationTriggers)
				GenerateTriggerableTransition(trigger, animatorController);

			return animatorController;
		}

		private static string GetSaveControllerPath(string name) {
			string message = string.Format("Create a new animator controller with name '{0}':", name);
			return EditorUtility.SaveFilePanelInProject("New Animator Contoller", name, "controller", message);
		}

		private static AnimationClip GenerateTriggerableTransition(string name, AnimatorController controller) {
			AnimationClip animationClip = AnimatorController.AllocateAnimatorClip(name);
			AssetDatabase.AddObjectToAsset(animationClip, controller);
			AnimatorState animatorState = controller.AddMotion(animationClip);
			controller.AddParameter(name, AnimatorControllerParameterType.Trigger);
			AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
			AnimatorStateTransition animatorStateTransition = stateMachine.AddAnyStateTransition(animatorState);
			animatorStateTransition.AddCondition(AnimatorConditionMode.If, 0f, name);
			return animationClip;
		}

		private static AnimationClip GenerateInitialState(AnimatorController controller) {
			AnimationClip animationClip = AnimatorController.AllocateAnimatorClip("Initial");
			AssetDatabase.AddObjectToAsset(animationClip, controller);
			controller.AddMotion(animationClip);
			return animationClip;
		}

		public static void GenerateBool(string name, AnimatorController controller) {
			foreach (AnimatorControllerParameter param in controller.parameters)
				if (param.name.Equals(name))
					return;

			controller.AddParameter(name, AnimatorControllerParameterType.Bool);
		}

	}
}