using Asgla.Data.Skill;
using Asgla.Skill;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	public class Demo_Spellbook_SpellRow : MonoBehaviour {

		private void Start() {
			if (UISkillDatabase.Instance == null || !m_IsDemo)
				return;

			SkillData[] spells = UISkillDatabase.Instance.spells;
			SkillData spell = spells[Random.Range(0, spells.Length)];

			if (m_Slot != null) m_Slot.Assign(spell);
			if (m_NameText != null) m_NameText.text = spell.Name;
			if (m_RankText != null) m_RankText.text = Random.Range(1, 6).ToString();
			if (m_DescriptionText != null) m_DescriptionText.text = spell.Description;
		}
#pragma warning disable 0649
		[SerializeField] private SkillMain m_Slot;
		[SerializeField] private Text m_NameText;
		[SerializeField] private Text m_RankText;
		[SerializeField] private Text m_DescriptionText;
		[SerializeField] private bool m_IsDemo;
#pragma warning restore 0649

	}
}