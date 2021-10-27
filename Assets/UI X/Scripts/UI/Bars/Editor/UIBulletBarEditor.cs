using AsglaUI.UI;
using UnityEditor;
using UnityEngine;

namespace AsglaUIEditor.UI {
	[CustomEditor(typeof(UIBulletBar), true)]
	public class UIBulletBarEditor : Editor {

		private SerializedProperty m_ActivePosition;
		private SerializedProperty m_AngleMax;
		private SerializedProperty m_AngleMin;
		private SerializedProperty m_BarType;
		private SerializedProperty m_BulletCount;
		private SerializedProperty m_BulletSize;
		private SerializedProperty m_BulletSprite;
		private SerializedProperty m_BulletSpriteActive;
		private SerializedProperty m_BulletSpriteActiveColor;
		private SerializedProperty m_BulletSpriteColor;
		private SerializedProperty m_Distance;
		private SerializedProperty m_FillAmount;
		private SerializedProperty m_FixedSize;
		private SerializedProperty m_InvertFill;
		private SerializedProperty m_SpriteRotation;

		protected virtual void OnEnable() {
			m_BarType = serializedObject.FindProperty("m_BarType");
			m_FixedSize = serializedObject.FindProperty("m_FixedSize");
			m_BulletSize = serializedObject.FindProperty("m_BulletSize");
			m_BulletSprite = serializedObject.FindProperty("m_BulletSprite");
			m_BulletSpriteColor = serializedObject.FindProperty("m_BulletSpriteColor");
			m_SpriteRotation = serializedObject.FindProperty("m_SpriteRotation");
			m_BulletSpriteActive = serializedObject.FindProperty("m_BulletSpriteActive");
			m_BulletSpriteActiveColor = serializedObject.FindProperty("m_BulletSpriteActiveColor");
			m_ActivePosition = serializedObject.FindProperty("m_ActivePosition");
			m_AngleMin = serializedObject.FindProperty("m_AngleMin");
			m_AngleMax = serializedObject.FindProperty("m_AngleMax");
			m_BulletCount = serializedObject.FindProperty("m_BulletCount");
			m_Distance = serializedObject.FindProperty("m_Distance");
			m_FillAmount = serializedObject.FindProperty("m_FillAmount");
			m_InvertFill = serializedObject.FindProperty("m_InvertFill");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("General Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_BarType, new GUIContent("Type"));
			EditorGUILayout.PropertyField(m_BulletCount, new GUIContent("Bullet Count"));
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Bullet Sprites", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_BulletSprite, new GUIContent("Background Sprite"));
			EditorGUILayout.PropertyField(m_BulletSpriteColor, new GUIContent("Background Color"));
			EditorGUILayout.PropertyField(m_BulletSpriteActive, new GUIContent("Fill Sprite"));
			EditorGUILayout.PropertyField(m_BulletSpriteActiveColor, new GUIContent("Fill Color"));
			EditorGUILayout.PropertyField(m_FixedSize, new GUIContent("Fixed Size"));

			if (m_FixedSize.boolValue)
				EditorGUILayout.PropertyField(m_BulletSize, new GUIContent("Size"));

			EditorGUILayout.PropertyField(m_SpriteRotation, new GUIContent("Rotation"));
			EditorGUILayout.PropertyField(m_ActivePosition, new GUIContent("Fill Offset"));
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Space();

			UIBulletBar.BarType barType = (UIBulletBar.BarType) m_BarType.enumValueIndex;

			if (barType == UIBulletBar.BarType.Radial) {
				EditorGUILayout.LabelField("Radial Bar Properties", EditorStyles.boldLabel);
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				EditorGUILayout.PropertyField(m_AngleMin, new GUIContent("Min Angle"));
				EditorGUILayout.PropertyField(m_AngleMax, new GUIContent("Max Angle"));
				EditorGUILayout.PropertyField(m_Distance, new GUIContent("Radius"));
				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

				EditorGUILayout.Space();
			}

			EditorGUILayout.LabelField("Fill Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			EditorGUILayout.PropertyField(m_FillAmount, new GUIContent("Fill Amount"));
			EditorGUILayout.PropertyField(m_InvertFill, new GUIContent("Invert Fill"));
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;

			EditorGUILayout.Space();
			serializedObject.ApplyModifiedProperties();

			if (GUILayout.Button("Generate Bullets"))
				foreach (Object target in targets)
					(target as UIBulletBar).ConstructBullets();

			EditorGUILayout.Space();
		}

	}
}