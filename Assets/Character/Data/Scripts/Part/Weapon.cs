using UnityEngine;

namespace CharacterCreator2D {
    public class Weapon : Part {
        /// <summary>
        /// Weapon category.
        /// </summary>
        [Tooltip("Weapon category")]
        public Asgla.Data.Type.Weapon weaponCategory;

        /// <summary>
        /// Muzzle flash position.
        /// </summary>
        [Tooltip("Muzzle flash position")]
        public Vector3 muzzlePosition;
    }
}