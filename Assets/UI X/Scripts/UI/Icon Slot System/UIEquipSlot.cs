using Asgla.Data.Item;
using Asgla.Data.Type;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AsglaUI.UI {
    [AddComponentMenu("UI/Icon Slots/Equip Slot", 12)]
    public class UIEquipSlot : UISlotBase, IUIItemSlot {
        [Serializable] public class OnAssignEvent : UnityEvent<UIEquipSlot> { }
        [Serializable] public class OnAssignWithSourceEvent : UnityEvent<UIEquipSlot, Object> { }
        [Serializable] public class OnUnassignEvent : UnityEvent<UIEquipSlot> { }

        [SerializeField] private Category Category = Category.Armor;

        /// <summary>
        /// Gets or sets the equip type of the slot.
        /// </summary>
        /// <value>The type of the equip.</value>
        public Category equipType {
            get { return this.Category; }
            set { this.Category = value; }
        }

        /// <summary>
        /// The assigned item info.
        /// </summary>
        private ItemData m_ItemInfo;

        /// <summary>
        /// The assign event delegate.
        /// </summary>
        public OnAssignEvent onAssign = new OnAssignEvent();

        /// <summary>
        /// The assign with source event delegate.
        /// </summary>
        public OnAssignWithSourceEvent onAssignWithSource = new OnAssignWithSourceEvent();

        /// <summary>
        /// The unassign event delegate.
        /// </summary>
        public OnUnassignEvent onUnassign = new OnUnassignEvent();

        /// <summary>
        /// Gets the item info of the item assigned to this slot.
        /// </summary>
        /// <returns>The spell info.</returns>
        public ItemData GetItemInfo() {
            return this.m_ItemInfo;
        }

        /// <summary>
        /// Determines whether this slot is assigned.
        /// </summary>
        /// <returns><c>true</c> if this instance is assigned; otherwise, <c>false</c>.</returns>
        public override bool IsAssigned() {
            return (this.m_ItemInfo != null);
        }

        /// <summary>
        /// Assign the slot by new item info while refering to the source.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <param name="source">The source slot (Could be null).</param>
        /// <returns><c>true</c> if this instance is assigned; otherwise, <c>false</c>.</returns>
        public bool Assign(ItemData itemInfo, Object source) {
            if (itemInfo == null)
                return false;

            // Check if the equipment type matches the target slot
            if (!this.CheckEquipType(itemInfo))
                return false;

            // Make sure we unassign first, so the event is called before new assignment
            this.Unassign();

            // Use the base class assign to set the icon
            this.Assign(itemInfo.GetIcon);

            // Set the spell info
            this.m_ItemInfo = itemInfo;

            // Invoke the on assign event
            if (this.onAssign != null)
                this.onAssign.Invoke(this);

            // Invoke the on assign event
            if (this.onAssignWithSource != null)
                this.onAssignWithSource.Invoke(this, source);

            // Success
            return true;
        }

        /// <summary>
        /// Assign the slot by item info.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        public bool Assign(ItemData itemInfo) {
            return this.Assign(itemInfo, null);
        }

        /// <summary>
        /// Assign the slot by the passed source slot.
        /// </summary>
        /// <param name="source">Source.</param>
        public override bool Assign(Object source) {
            if (source is IUIItemSlot) {
                IUIItemSlot sourceSlot = source as IUIItemSlot;

                if (sourceSlot != null) {
                    // Check if the equipment type matches the target slot
                    if (!this.CheckEquipType(sourceSlot.GetItemInfo()))
                        return false;

                    return this.Assign(sourceSlot.GetItemInfo(), source);
                }
            }

            // Default
            return false;
        }

        /// <summary>
        /// Checks if the given item can assigned to this slot.
        /// </summary>
        /// <returns><c>true</c>, if equip type was checked, <c>false</c> otherwise.</returns>
        /// <param name="info">Info.</param>
        public virtual bool CheckEquipType(ItemData info) {
            if (info == null)
                return false;

            if (info.Type.Category != this.equipType)
                return false;

            return true;
        }

        /// <summary>
        /// Unassign this slot.
        /// </summary>
        public override void Unassign() {
            // Remove the icon
            base.Unassign();

            // Clear the spell info
            this.m_ItemInfo = null;

            // Invoke the on unassign event
            if (this.onUnassign != null)
                this.onUnassign.Invoke(this);
        }

        /// <summary>
        /// Determines whether this slot can swap with the specified target slot.
        /// </summary>
        /// <returns><c>true</c> if this instance can swap with the specified target; otherwise, <c>false</c>.</returns>
        /// <param name="target">Target.</param>
        public override bool CanSwapWith(Object target) {
            if (target is IUIItemSlot) {
                // Check if the equip slot accpets this item
                if (target is UIEquipSlot) {
                    return (target as UIEquipSlot).CheckEquipType(this.GetItemInfo());
                }

                // It's an item slot
                return true;
            }

            // Default
            return false;
        }

        // <summary>
        /// Performs a slot swap.
        /// </summary>
        /// <returns><c>true</c>, if slot swap was performed, <c>false</c> otherwise.</returns>
        /// <param name="sourceSlot">Source slot.</param>
        public override bool PerformSlotSwap(Object sourceObject) {
            // Get the source slot
            IUIItemSlot sourceSlot = (sourceObject as IUIItemSlot);

            // Get the source item info
            ItemData sourceItemInfo = sourceSlot.GetItemInfo();

            // Assign the source slot by this slot
            bool assign1 = sourceSlot.Assign(this.GetItemInfo(), this);

            // Assign this slot by the source slot
            bool assign2 = this.Assign(sourceItemInfo, sourceObject);

            // Return the status
            return (assign1 && assign2);
        }

        /// <summary>
        /// Raises the tooltip event.
        /// </summary>
        /// <param name="show">If set to <c>true</c> show.</param>
        public override void OnTooltip(bool show) {
            UITooltip.InstantiateIfNecessary(this.gameObject);

            // Handle unassigned
            if (!this.IsAssigned()) {
                // If we are showing the tooltip
                if (show) {
                    UITooltip.AddTitle(UIEquipSlot.EquipTypeToString(this.Category));
                    UITooltip.SetHorizontalFitMode(ContentSizeFitter.FitMode.PreferredSize);
                    UITooltip.AnchorToRect(this.transform as RectTransform);
                    UITooltip.Show();
                }
                else UITooltip.Hide();
            }
            else {
                // Make sure we have spell info, otherwise game might crash
                if (this.m_ItemInfo == null)
                    return;

                // If we are showing the tooltip
                if (show) {
                    UIItemSlot.PrepareTooltip(this.m_ItemInfo);
                    UITooltip.AnchorToRect(this.transform as RectTransform);
                    UITooltip.Show();
                }
                else UITooltip.Hide();
            }
        }

        /// <summary>
		/// This method is raised when the slot is denied to be thrown away and returned to it's source.
		/// </summary>
        protected override void OnThrowAwayDenied() {
            if (!this.IsAssigned())
                return;

            // Find free inventory slot
            List<UIItemSlot> itemSlots = UIItemSlot.GetSlotsInGroup(UIItemSlot_Group.Inventory);

            if (itemSlots.Count > 0) {
                // Get the first free one
                foreach (UIItemSlot slot in itemSlots) {
                    if (!slot.IsAssigned()) {
                        // Assign this equip slot to the item slot
                        slot.Assign(this);

                        // Unassing this equip slot
                        this.Unassign();
                        break;
                    }
                }
            }
        }

        #region Static Methods
        /// <summary>
        /// Equip type to string convertion.
        /// </summary>
        /// <returns>The string.</returns>
        public static string EquipTypeToString(Category type) {
            string str = "Undefined";
            switch (type) {
                case Category.Weapon: str = "Weapon"; break;
                case Category.Helmet: str = "Helm"; break;
                case Category.Armor: str = "Armor"; break;
                case Category.Cape: str = "Cape"; break;
            }

            return str;
        }
        /*public static string EquipTypeToString(UIEquipmentType type) {
            string str = "Undefined";

            switch (type) {
                case UIEquipmentType.Weapon_MainHand: str = "Main Hand"; break;
                case UIEquipmentType.Weapon_OffHand: str = "Off Hand"; break;
                case UIEquipmentType.Head_Hair: str = "Hair"; break;
                case UIEquipmentType.Head_Face: str = "Face"; break;
                case UIEquipmentType.Armor: str = "Armor"; break;
            }

            return str;
        }*/

        /// <summary>
        /// Gets all the equip slots.
        /// </summary>
        /// <returns>The slots.</returns>
        public static List<UIEquipSlot> GetSlots() {
            List<UIEquipSlot> slots = new List<UIEquipSlot>();
            UIEquipSlot[] sl = Resources.FindObjectsOfTypeAll<UIEquipSlot>();

            foreach (UIEquipSlot s in sl) {
                // Check if the slow is active in the hierarchy
                if (s.gameObject.activeInHierarchy)
                    slots.Add(s);
            }

            return slots;
        }

        /// <summary>
        /// Gets the first equip slot found with the specified Equipment Type.
        /// </summary>
        /// <returns>The slot.</returns>
        /// <param name="equip">The slot Equipment Type.</param>
        //public static UIEquipSlot GetSlotWithType(UIEquipmentType type) {
        public static UIEquipSlot GetSlotWithType(Category equip) {
            UIEquipSlot[] sl = Resources.FindObjectsOfTypeAll<UIEquipSlot>();

            foreach (UIEquipSlot s in sl) {
                // Check if the slow is active in the hierarchy
                if (s.gameObject.activeInHierarchy && s.equipType == equip)
                    return s;
            }

            // Default
            return null;
        }

        /// <summary>
        /// Gets all the equip slots with the specified Equipment Type.
        /// </summary>
        /// <returns>The slots.</returns>
        /// <param name="equip">The slot Equipment Type.</param>
        //public static List<UIEquipSlot> GetSlotsWithType(UIEquipmentType type) {
        public static List<UIEquipSlot> GetSlotsWithType(Category equip) {
            List<UIEquipSlot> slots = new List<UIEquipSlot>();
            UIEquipSlot[] sl = Resources.FindObjectsOfTypeAll<UIEquipSlot>();

            foreach (UIEquipSlot s in sl) {
                // Check if the slow is active in the hierarchy
                if (s.gameObject.activeInHierarchy && s.equipType == equip)
                    slots.Add(s);
            }

            return slots;
        }
        #endregion
    }
}
