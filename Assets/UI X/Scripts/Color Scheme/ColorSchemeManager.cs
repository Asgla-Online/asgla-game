using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AsglaUI.UI {
	public class ColorSchemeManager : ScriptableObject {

		[SerializeField] private ColorScheme m_ActiveColorScheme;

		/// <summary>
		///     Gets or sets the active color scheme.
		/// </summary>
		public ColorScheme activeColorScheme {
			get{ return m_ActiveColorScheme; }
			set{
				m_ActiveColorScheme = value;
#if UNITY_EDITOR
				EditorUtility.SetDirty(this);
#endif
			}
		}

		#region singleton

		private static ColorSchemeManager m_Instance;

		public static ColorSchemeManager Instance {
			get{
				if (m_Instance == null)
					m_Instance = Resources.Load("ColorSchemeManager") as ColorSchemeManager;

				return m_Instance;
			}
		}

		#endregion

	}
}