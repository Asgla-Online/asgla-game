using Asgla.Data.Type;
using System;
using UnityEngine;

namespace CharacterCreator2D {

    [Serializable]
    public class SlotList {
        /// <summary>
        /// Slot for the armor.
        /// </summary>
        public PartSlot armor;

        /// <summary>
        /// Slot for the boots.
        /// </summary>
        public PartSlot boots;

        /// <summary>
        /// Slot for the ear.
        /// </summary>
        public PartSlot ear;

        /// <summary>
        /// Slot for the eyebrow.
        /// </summary>
        public PartSlot eyebrow;

        /// <summary>
        /// Slot for the eyes.
        /// </summary>
        public PartSlot eyes;

        /// <summary>
        /// Slot for the facial hair.
        /// </summary>
        public PartSlot facialHair;

        /// <summary>
        /// Slot for the gloves.
        /// </summary>
        public PartSlot gloves;

        /// <summary>
        /// Slot for the hair.
        /// </summary>
        public PartSlot hair;

        /// <summary>
        /// Slot for the helmet.
        /// </summary>
        public PartSlot helmet;

        /// <summary>
        /// Slot for the mouth.
        /// </summary>
        public PartSlot mouth;

        /// <summary>
        /// Slot for the nose.
        /// </summary>
        public PartSlot nose;

        /// <summary>
        /// Slot for the pants.
        /// </summary>
        public PartSlot pants;

        /// <summary>
        /// Slot for the skin's details.
        /// </summary>
        public PartSlot skinDetails;

        /// <summary>
        /// Slot for the main hand's weapon.
        /// </summary>
        public PartSlot mainHand;

        /// <summary>
        /// Slot for the off hand's weapon.
        /// </summary>
        public PartSlot offHand;

        /// <summary>
        /// Slot for the cape
        /// </summary>
        public PartSlot cape;

        /// <summary>
        /// Slot for the skirts.
        /// </summary>
        public PartSlot skirts;

        /// <summary>
        /// Slot for the body skin.
        /// </summary>
        public PartSlot bodySkin;

        /// <summary>
        /// Returns PartSlot of a given SlotCategory.
        /// </summary>
        /// <param name="slotCategory">Given SlotCategory.</param>
        /// <returns>PartSlot according to slotCategory.</returns>
        public PartSlot GetSlot(Equipment slotCategory) {
            switch (slotCategory) {
                case Equipment.Armor:
                    return this.armor;
                case Equipment.Boot:
                    return this.boots;
                case Equipment.Ear:
                    return this.ear;
                case Equipment.Eyebrow:
                    return this.eyebrow;
                case Equipment.Eye:
                    return this.eyes;
                case Equipment.FacialHair:
                    return this.facialHair;
                case Equipment.Glove:
                    return this.gloves;
                case Equipment.Hair:
                    return this.hair;
                case Equipment.Helmet:
                    return this.helmet;
                case Equipment.Mouth:
                    return this.mouth;
                case Equipment.Nose:
                    return this.nose;
                case Equipment.Pant:
                    return this.pants;
                case Equipment.SkinDetail:
                    return this.skinDetails;
                case Equipment.MainHand:
                    return this.mainHand;
                case Equipment.OffHand:
                    return this.offHand;
                case Equipment.Cape:
                    return this.cape;
                case Equipment.Skirt:
                    return this.skirts;
                case Equipment.BodySkin:
                    return this.bodySkin;
                default:
                    return null;
            }
        }
    }

    [Serializable]
    public class PartSlot {
        /// <summary>
        /// Slot's material.
        /// </summary>
        public Material material;

        /// <summary>
        /// Assigned Part.
        /// </summary>
        public Part assignedPart;

        /// <summary>
        /// First color.
        /// </summary>
        public Color color1 = Color.gray;

        /// <summary>
        /// Second color.
        /// </summary>
        public Color color2 = Color.gray;

        /// <summary>
        /// Third color.
        /// </summary>
        public Color color3 = Color.gray;
    }

}