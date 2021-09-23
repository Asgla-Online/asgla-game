using UnityEngine;

namespace Asgla.UI.UnitFrame {
    public class UnitFrameBase : MonoBehaviour {

        [SerializeField] private UnitFrameBar _health;
        [SerializeField] private UnitFrameBar _energy;

        public UnitFrameBar Health => _health;
        public UnitFrameBar Energy => _energy;

    }
}
