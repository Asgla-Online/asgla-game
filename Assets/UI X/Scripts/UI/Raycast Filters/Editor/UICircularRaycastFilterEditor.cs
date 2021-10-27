using AsglaUI.UI;
using UnityEditor;
using UnityEngine;

namespace AsglaUIEditor.UI {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UICircularRaycastFilter))]
	public class UICircularRaycastFilterEditor : Editor {

		public const string PREFS_KEY = "UICircleRaycastFilter_DG";
		private bool m_DisplayGeometry = true;

		protected void OnEnable() {
			m_DisplayGeometry = EditorPrefs.GetBool(PREFS_KEY, true);
		}

		protected void OnSceneGUI() {
			if (!m_DisplayGeometry)
				return;

			UICircularRaycastFilter filter = target as UICircularRaycastFilter;
			RectTransform rt = filter.transform as RectTransform;

			if (filter.operationalRadius == 0f)
				return;

			float radius = filter.operationalRadius;
			Vector3 position = rt.TransformPoint(new Vector3(rt.rect.center.x, rt.rect.center.y, 0f) +
			                                     new Vector3(filter.offset.x, filter.offset.y, 0f));

			Canvas canvas = UIUtility.FindInParents<Canvas>(filter.gameObject);
			if (canvas != null)
				radius *= canvas.transform.localScale.x;

			Handles.color = Color.green;
			Handles.CircleHandleCap(0, position, rt.rotation, radius, EventType.Repaint);
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			EditorGUI.BeginChangeCheck();
			m_DisplayGeometry = EditorGUILayout.Toggle("Display Geometry", m_DisplayGeometry);

			if (EditorGUI.EndChangeCheck()) {
				EditorPrefs.SetBool(PREFS_KEY, m_DisplayGeometry);
				SceneView.RepaintAll();
			}
		}

	}
}