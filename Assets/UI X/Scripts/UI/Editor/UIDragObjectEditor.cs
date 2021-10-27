using AsglaUI.UI;
using UnityEditor;
using UnityEngine;

namespace AsglaUIEditor.UI {
	[CustomEditor(typeof(UIDragObject), true)]
	public class UIDragObjectEditor : Editor {

		public override void OnInspectorGUI() {
			serializedObject.Update();

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Target"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Horizontal"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Vertical"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Inertia", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Inertia"), new GUIContent("Enable"));
			if (serializedObject.FindProperty("m_Inertia").boolValue) {
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_DampeningRate"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_InertiaRounding"));
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Constrain Within Canvas", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ConstrainWithinCanvas"),
				new GUIContent("Enable"));
			if (serializedObject.FindProperty("m_ConstrainWithinCanvas").boolValue) {
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ConstrainDrag"),
					new GUIContent("Constrain Drag"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ConstrainInertia"),
					new GUIContent("Constrain Inertia"));
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("onBeginDrag"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("onEndDrag"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("onDrag"), true);

			serializedObject.ApplyModifiedProperties();
		}

	}
}