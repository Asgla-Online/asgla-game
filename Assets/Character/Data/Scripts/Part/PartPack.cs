using Asgla.Data.Type;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D {
    [Serializable]
    public class PartPack {
        /// <summary>
        /// PartPack's PartCategory.
        /// </summary>
        [Tooltip("PartPack's PartCategory")]
        public Category category;

        /// <summary>
        /// List of Parts grouped by this PartPacks.
        /// </summary>
        [Tooltip("List of Parts grouped by this PartPacks")]
        public List<Part> parts;

        public PartPack() {
            this.category = Category.Armor;
            this.parts = new List<Part>();
        }
    }
}