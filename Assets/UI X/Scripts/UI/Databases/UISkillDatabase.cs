using Asgla.Data.Skill;
using UnityEngine;

namespace AsglaUI.UI {
	public class UISkillDatabase : ScriptableObject {

		public SkillData[] spells;

		/// <summary>
		///     Get the specified SpellInfo by index.
		/// </summary>
		/// <param name="index">Index.</param>
		public SkillData Get(int index) {
			return spells[index];
		}

		/// <summary>
		///     Gets the specified SpellInfo by ID.
		/// </summary>
		/// <returns>The SpellInfo or NULL if not found.</returns>
		/// <param name="ID">The spell ID.</param>
		public SkillData GetByID(int ID) {
			for (int i = 0; i < spells.Length; i++)
				if (spells[i].DatabaseID == ID)
					return spells[i];

			return null;
		}

		#region singleton

		private static UISkillDatabase m_Instance;

		public static UISkillDatabase Instance {
			get{
				if (m_Instance == null)
					m_Instance = Resources.Load("Databases/SpellDatabase") as UISkillDatabase;

				return m_Instance;
			}
		}

		#endregion

	}
}