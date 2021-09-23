using Asgla.Data.Skill;
using Asgla.Skill;
using UnityEngine;

namespace AsglaUI.UI {
    public class Test_SkillMain_AssignAll : MonoBehaviour {
#pragma warning disable 0649
        [SerializeField] private Transform m_Container;
#pragma warning restore 0649

        void Start() {
            if (this.m_Container == null || UISkillDatabase.Instance == null) {
                this.Destruct();
                return;
            }

            SkillMain[] slots = this.m_Container.gameObject.GetComponentsInChildren<SkillMain>();
            SkillData[] spells = UISkillDatabase.Instance.spells;

            if (slots.Length > 0 && spells.Length > 0) {
                foreach (SkillMain slot in slots)
                    slot.Assign(spells[Random.Range(0, spells.Length)]);
            }

            this.Destruct();
        }

        private void Destruct() {
            DestroyImmediate(this);
        }
    }
}
