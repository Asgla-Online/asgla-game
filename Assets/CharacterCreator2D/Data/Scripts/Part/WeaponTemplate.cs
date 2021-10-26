using UnityEngine;

namespace CharacterCreator2D {
	public class WeaponTemplate : PartTemplate {

		/// <summary>
		/// Weapon category.
		/// </summary>
		[Tooltip("Weapon category")] [ReadOnly]
		public WeaponCategory weaponCategory;

	}
}