using System.Collections.Generic;
using AsglaUI.UI;
using UnityEditor;
using UnityEngine;

namespace AsglaUIEditor.UI {
	[CustomEditor(typeof(UIStepBar), true)]
	public class UIStepBarEditor : Editor {

		private SerializedProperty m_BubbleOffset;
		private SerializedProperty m_BubbleRect;
		private SerializedProperty m_BubbleText;

		private SerializedProperty m_CurrentStep;
		private SerializedProperty m_FillImage;
		private SerializedProperty m_SeparatorAutoSize;
		private SerializedProperty m_SeparatorSize;
		private SerializedProperty m_SeparatorSprite;
		private SerializedProperty m_SeparatorSpriteActive;
		private SerializedProperty m_SeparatorSpriteColor;
		private SerializedProperty m_StepsCount;
		private SerializedProperty m_StepsGridPadding;

		protected virtual void OnEnable() {
			m_CurrentStep = serializedObject.FindProperty("m_CurrentStep");
			m_StepsCount = serializedObject.FindProperty("m_StepsCount");
			m_FillImage = serializedObject.FindProperty("m_FillImage");
			m_BubbleRect = serializedObject.FindProperty("m_BubbleRect");
			m_BubbleOffset = serializedObject.FindProperty("m_BubbleOffset");
			m_BubbleText = serializedObject.FindProperty("m_BubbleText");
			m_StepsGridPadding = serializedObject.FindProperty("m_StepsGridPadding");
			m_SeparatorSprite = serializedObject.FindProperty("m_SeparatorSprite");
			m_SeparatorSpriteActive = serializedObject.FindProperty("m_SeparatorSpriteActive");
			m_SeparatorSpriteColor = serializedObject.FindProperty("m_SeparatorSpriteColor");
			m_SeparatorAutoSize = serializedObject.FindProperty("m_SeparatorAutoSize");
			m_SeparatorSize = serializedObject.FindProperty("m_SeparatorSize");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("General Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.IntSlider(m_CurrentStep, 0, m_StepsCount.intValue + 1, new GUIContent("Starting Step"));
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(m_StepsCount, new GUIContent("Step Count"));
			if (EditorGUI.EndChangeCheck()) {
				serializedObject.ApplyModifiedProperties();
				(target as UIStepBar).ValidateOverrideFillList();
				(target as UIStepBar).RebuildSteps_Editor();
				serializedObject.Update();
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Grid Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_StepsGridPadding, new GUIContent("Padding"), true);
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Separator Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_SeparatorSprite, new GUIContent("Normal Sprite"), true);
			if (m_SeparatorSprite.objectReferenceValue != null) {
				EditorGUILayout.PropertyField(m_SeparatorSpriteActive, new GUIContent("Active Sprite"));
				EditorGUILayout.PropertyField(m_SeparatorSpriteColor, new GUIContent("Sprite Color"), true);
				EditorGUILayout.PropertyField(m_SeparatorAutoSize, new GUIContent("Auto Size"), true);
				GUI.enabled = !m_SeparatorAutoSize.boolValue;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(m_SeparatorSize, new GUIContent("Size"), true);
				if (EditorGUI.EndChangeCheck()) {
					serializedObject.ApplyModifiedProperties();
					(target as UIStepBar).RebuildSteps_Editor();
					serializedObject.Update();
				}

				GUI.enabled = true;
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Fill Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_FillImage, new GUIContent("Image"));
			DrawOverrideFillTable();
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Bubble Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_BubbleRect, new GUIContent("Rect Transform"));
			EditorGUILayout.PropertyField(m_BubbleText, new GUIContent("Text"));
			EditorGUILayout.PropertyField(m_BubbleOffset, new GUIContent("Offset"));
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Space();
			serializedObject.ApplyModifiedProperties();
		}

		protected void DrawOverrideFillTable() {
			UIStepBar bar = target as UIStepBar;
			List<UIStepBar.StepFillInfo> list = bar.GetOverrideFillList();

			EditorGUILayout.LabelField("Override Fill Amount");
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			for (int i = 1; i <= m_StepsCount.intValue; i++) {
				// Check if we have override info for the step
				int overrideIndex = list.FindIndex(x => x.index == i);

				// If we have info
				if (overrideIndex >= 0) {
					// Get the info
					UIStepBar.StepFillInfo info = list[overrideIndex];

					EditorGUI.BeginChangeCheck();
					float newAmount = EditorGUILayout.FloatField("Step #" + i, info.amount);
					if (EditorGUI.EndChangeCheck()) {
						Undo.RecordObject(target, "UI Step Bar changed.");

						UIStepBar.StepFillInfo newInfo = new UIStepBar.StepFillInfo();
						newInfo.amount = newAmount;
						newInfo.index = i;
						list[overrideIndex] = newInfo;

						// Validate the override list to remove the zero amount info
						if (newAmount == 0f)
							bar.ValidateOverrideFillList();

						// Update the fill image fillAmount
						bar.UpdateFillImage();

						// Set the object as dirty to be saved on the disk
						EditorUtility.SetDirty(bar);
					}
				} else // We dont have override info for the current step
				{
					EditorGUI.BeginChangeCheck();
					float newAmount = EditorGUILayout.FloatField("Step #" + i, bar.GetStepFillAmount(i));
					if (EditorGUI.EndChangeCheck())
						if (newAmount > 0f) {
							Undo.RecordObject(target, "UI Step Bar changed.");

							UIStepBar.StepFillInfo newInfo = new UIStepBar.StepFillInfo();
							newInfo.amount = newAmount;
							newInfo.index = i;
							list.Add(newInfo);

							// Update the fill image fillAmount
							bar.UpdateFillImage();

							// Set the object as dirty to be saved on the disk
							EditorUtility.SetDirty(bar);
						}
				}
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

	}
}