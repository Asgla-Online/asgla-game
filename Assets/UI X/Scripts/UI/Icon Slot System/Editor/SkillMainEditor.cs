using Asgla.Skill;
using UnityEditor;
using UnityEngine;

namespace AsglaUIEditor.UI {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SkillMain), true)]
	public class SkillMainEditor : UISlotBaseEditor {

		private SerializedProperty _IDProperty;

		private SerializedProperty _slotGroupProperty;
		private SerializedProperty onAssignProperty;
		private SerializedProperty onClickProperty;
		private SerializedProperty onUnassignProperty;

		protected override void OnEnable() {
			base.OnEnable();

			_slotGroupProperty = serializedObject.FindProperty("_slotGroup");
			_IDProperty = serializedObject.FindProperty("_id");
			onAssignProperty = serializedObject.FindProperty("onAssign");
			onUnassignProperty = serializedObject.FindProperty("onUnassign");
			onClickProperty = serializedObject.FindProperty("onClick");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(_slotGroupProperty, new GUIContent("Slot Group"));
			EditorGUILayout.PropertyField(_IDProperty, new GUIContent("Slot ID"));
			EditorGUILayout.Separator();
			serializedObject.ApplyModifiedProperties();

			base.OnInspectorGUI();

			EditorGUILayout.Separator();

			serializedObject.Update();
			EditorGUILayout.PropertyField(onAssignProperty, new GUIContent("On Assign"), true);
			EditorGUILayout.PropertyField(onUnassignProperty, new GUIContent("On Unassign"), true);
			EditorGUILayout.PropertyField(onClickProperty, new GUIContent("On Click"), true);
			serializedObject.ApplyModifiedProperties();
		}

	}
}