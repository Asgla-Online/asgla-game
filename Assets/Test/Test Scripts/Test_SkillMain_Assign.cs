using Asgla.Skill;
using UnityEngine;

namespace AsglaUI.UI {
    public class Test_SkillMain_Assign : MonoBehaviour {
        #pragma warning disable 0649
        [SerializeField] private SkillMain slot;
        [SerializeField] private int assignSpell;
        #pragma warning restore 0649

        void Awake() {
            if (this.slot == null)
                this.slot = this.GetComponent<SkillMain>();
        }

        void Start() {
            if (this.slot == null || UISkillDatabase.Instance == null) {
                this.Destruct();
                return;
            }

            this.slot.Assign(UISkillDatabase.Instance.GetByID(this.assignSpell));
            this.Destruct();
        }

        private void Destruct() {
            DestroyImmediate(this);
        }
    }
}
