using UnityEngine;

namespace AsglaUI.UI {
	public class UITalentDatabase : ScriptableObject {

		public UITalentInfo[] talents;

		/// <summary>
		///     Get the specified TalentInfo by index.
		/// </summary>
		/// <param name="index">Index.</param>
		public UITalentInfo Get(int index) {
			return talents[index];
		}

		/// <summary>
		///     Gets the specified TalentInfo by ID.
		/// </summary>
		/// <returns>The TalentInfo or NULL if not found.</returns>
		/// <param name="ID">The talent ID.</param>
		public UITalentInfo GetByID(int ID) {
			for (int i = 0; i < talents.Length; i++)
				if (talents[i].ID == ID)
					return talents[i];

			return null;
		}

		#region singleton

		private static UITalentDatabase m_Instance;

		public static UITalentDatabase Instance {
			get{
				if (m_Instance == null)
					m_Instance = Resources.Load("Databases/TalentDatabase") as UITalentDatabase;

				return m_Instance;
			}
		}

		#endregion

	}
}