using UnityEngine;

namespace Asgla.UI.Unit_Frame {
	public class UnitFrameBase : MonoBehaviour {

		[SerializeField] private UnitFrameBar _health;
		[SerializeField] private UnitFrameBar _energy;

		public UnitFrameBar Health => _health;
		public UnitFrameBar Energy => _energy;

	}
}