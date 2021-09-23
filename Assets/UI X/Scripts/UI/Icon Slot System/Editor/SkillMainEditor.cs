using Asgla.Skill;
using UnityEditor;
using UnityEngine;

namespace AsglaUIEditor.UI {
    [CanEditMultipleObjects, CustomEditor(typeof(SkillMain), true)]
	public class SkillMainEditor : UISlotBaseEditor {
		
		private SerializedProperty _slotGroupProperty;
		private SerializedProperty _IDProperty;
		private SerializedProperty onAssignProperty;
		private SerializedProperty onUnassignProperty;
		private SerializedProperty onClickProperty;
			
		protected override void OnEnable()
		{
			base.OnEnable();
			
			this._slotGroupProperty = this.serializedObject.FindProperty("_slotGroup");
			this._IDProperty = this.serializedObject.FindProperty("_id");
			this.onAssignProperty = this.serializedObject.FindProperty("onAssign");
			this.onUnassignProperty = this.serializedObject.FindProperty("onUnassign");
			this.onClickProperty = this.serializedObject.FindProperty("onClick");
		}
		
		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(this._slotGroupProperty, new GUIContent("Slot Group"));
			EditorGUILayout.PropertyField(_IDProperty, new GUIContent("Slot ID"));
            EditorGUILayout.Separator();
            this.serializedObject.ApplyModifiedProperties();
			
			base.OnInspectorGUI();

            EditorGUILayout.Separator();

            this.serializedObject.Update();
			EditorGUILayout.PropertyField(this.onAssignProperty, new GUIContent("On Assign"), true);
			EditorGUILayout.PropertyField(this.onUnassignProperty, new GUIContent("On Unassign"), true);
			EditorGUILayout.PropertyField(this.onClickProperty, new GUIContent("On Click"), true);
			this.serializedObject.ApplyModifiedProperties();
		}
	}
}
