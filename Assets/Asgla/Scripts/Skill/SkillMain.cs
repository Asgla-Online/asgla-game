using Asgla.Data.Skill;
using AsglaUI.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Asgla.Skill {
    public class SkillMain : UISlotBase, IUISkillSlot, IUISlotHasCooldown {

        [Serializable] public class OnAssignEvent : UnityEvent<SkillMain> { }
        [Serializable] public class OnUnassignEvent : UnityEvent<SkillMain> { }
        [Serializable] public class OnClickEvent : UnityEvent<SkillMain> { }

        [SerializeField, Tooltip("Placing the slot in a group will make the slot accessible via the static method GetSlot.")]
        private SkillMain_Group _slotGroup = SkillMain_Group.None;

        [SerializeField]
        private int _id = 0;

        public SkillMain_Group slotGroup {
            get { return this._slotGroup; }
            set { this._slotGroup = value; }
        }

        public int ID {
            get { return this._id; }
            set { this._id = value; }
        }

        public OnAssignEvent onAssign = new OnAssignEvent();

        public OnUnassignEvent onUnassign = new OnUnassignEvent();

        public OnClickEvent onClick = new OnClickEvent();

        private SkillData _skillData;

        public SkillData GetSkillData() {
            return this._skillData;
        }

        private UISlotCooldown _cooldown;

        public UISlotCooldown cooldownComponent {
            get { return this._cooldown; }
        }

        protected override void OnEnable() {
            base.OnEnable();

            #if UNITY_EDITOR
            if (!this.IsInPrefabStage()) {
                // Check for duplicate id
                List<SkillMain> slots = GetSlotsInGroup(this._slotGroup);
                SkillMain duplicate = slots.Find(x => x.ID == this._id && !x.Equals(this));

                if (duplicate != null) {
                    int oldId = this._id;
                    this.AutoAssignID();
                    Debug.LogWarning("Spell Slot with duplicate ID: " + oldId + " in Group: " + this._slotGroup + ", generating and assigning new ID: " + this._id + ".");
                }
            }
            #endif
        }

        #if UNITY_EDITOR
        private bool IsInPrefabStage() {
            return UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
        }
        #endif

        public override bool IsAssigned() {
            return (this._skillData != null);
        }

        public bool Assign(SkillData spellInfo) {
            if (spellInfo == null)
                return false;

            // Make sure we unassign first, so the event is called before new assignment
            this.Unassign();

            // Use the base class assign to set the icon
            this.Assign(spellInfo.GetIcon);

            // Set the spell info
            this._skillData = spellInfo;

            // Invoke the on assign event
            if (this.onAssign != null)
                this.onAssign.Invoke(this);

            // Notify the cooldown component
            if (this._cooldown != null)
                this._cooldown.OnAssignSpell();

            // Success
            return true;
        }

        public override bool Assign(Object source) {
            if (source is IUISkillSlot) {
                IUISkillSlot sourceSlot = source as IUISkillSlot;

                if (sourceSlot != null)
                    return this.Assign(sourceSlot.GetSkillData());
            }

            // Default
            return false;
        }

        public override void Unassign() {
            // Remove the icon
            base.Unassign();

            // Clear the spell info
            this._skillData = null;

            // Invoke the on unassign event
            if (this.onUnassign != null)
                this.onUnassign.Invoke(this);

            // Notify the cooldown component
            if (this._cooldown != null)
                this._cooldown.OnUnassignSpell();
        }

        public override bool CanSwapWith(Object target) {
            return (target is IUISkillSlot);
        }

        public override bool PerformSlotSwap(Object sourceObject) {
            // Get the source slot
            IUISkillSlot sourceSlot = (sourceObject as IUISkillSlot);

            // Get the source spell info
            SkillData sourceSpellInfo = sourceSlot.GetSkillData();

            // Assign the source slot by this one
            bool assign1 = sourceSlot.Assign(this.GetSkillData());

            // Assign this slot by the source slot
            bool assign2 = this.Assign(sourceSpellInfo);

            // Return the status
            return (assign1 && assign2);
        }

        public override void OnPointerClick(PointerEventData eventData) {
            // Run the event on the base
            base.OnPointerClick(eventData);

            // Make sure the slot is assigned
            if (!this.IsAssigned())
                return;

            // Invoke the click event
            if (this.onClick != null)
                this.onClick.Invoke(this);
        }

        /// <summary>
        /// Raises the tooltip event.
        /// </summary>
        /// <param name="show">If set to <c>true</c> show.</param>
        public override void OnTooltip(bool show) {
            // Make sure we have spell info, otherwise game might crash
            if (this._skillData == null)
                return;

            // If we are showing the tooltip
            if (show) {
                UITooltip.InstantiateIfNecessary(this.gameObject);

                // Prepare the tooltip lines
                PrepareTooltip(this._skillData);

                // Anchor to this slot
                UITooltip.AnchorToRect(this.transform as RectTransform);

                // Show the tooltip
                UITooltip.Show();
            } else {
                // Hide the tooltip
                UITooltip.Hide();
            }
        }

        /// <summary>
        /// Sets the cooldown component.
        /// </summary>
        /// <param name="cooldown">Cooldown.</param>
        public void SetCooldownComponent(UISlotCooldown cooldown) {
            this._cooldown = cooldown;
        }

        /// <summary>
        /// Automatically generate and assign slot ID.
        /// </summary>
        [ContextMenu("Auto Assign ID")]
        public void AutoAssignID() {
            // Get the active slots in the slot's group
            List<SkillMain> slots = GetSlotsInGroup(this._slotGroup);

            if (slots.Count > 0) {
                slots.Reverse();
                this._id = slots[0].ID + 1;
            } else {
                // If we have no slots
                this._id = 1;
            }

            slots.Clear();
        }

        #region Static Methods
        public static void PrepareTooltip(SkillData spellInfo) {
            // Make sure we have spell info, otherwise game might crash
            if (spellInfo == null)
                return;

            // Set the tooltip width
            if (UITooltipManager.Instance != null)
                UITooltip.SetWidth(UITooltipManager.Instance.spellTooltipWidth);

            // Set the spell name as title
            UITooltip.AddLine(spellInfo.Name, "SpellTitle");

            // Spacer
            UITooltip.AddSpacer();

            // Prepare some attributes
            if (spellInfo.FlagType == SkillFlag.PASSIVE) {
                UITooltip.AddLine("Passive", "SkillAttribute");
            } else {
                // Power consumption
                if (spellInfo.Energy > 0f) {
                    //if (spellInfo.Flags.Has(UISpellInfo_Flags.PowerCostInPct))
                    UITooltip.AddLineColumn(spellInfo.Energy.ToString("0") + "% Energy", "SkillAttribute");
                    //else
                    UITooltip.AddLineColumn(spellInfo.Energy.ToString("0") + " Energy", "SkillAttribute");
                }

                // Range
                if (spellInfo.Range > 0f) {
                    if (spellInfo.Range == 1f)
                        UITooltip.AddLineColumn("Melee range", "SkillAttribute");
                    else
                        UITooltip.AddLineColumn(spellInfo.Range.ToString("0") + " range", "SkillAttribute");
                }

                // Cast time
                if (spellInfo.CastTime == 0f)
                    UITooltip.AddLineColumn("Instant", "SkillAttribute");
                else
                    UITooltip.AddLineColumn(spellInfo.CastTime.ToString("0.0") + " sec cast", "SkillAttribute");

                // Cooldown
                if (spellInfo.Cooldown > 0f)
                    UITooltip.AddLineColumn(spellInfo.Cooldown.ToString("0.0") + " sec cooldown", "SkillAttribute");
            }

            // Set the spell description if not empty
            if (!string.IsNullOrEmpty(spellInfo.Description)) {
                UITooltip.AddSpacer();
                UITooltip.AddLine(spellInfo.Description, "SkillDescription");
            }
        }

        /// <summary>
        /// Gets all the spell slots.
        /// </summary>
        /// <returns>The slots.</returns>
        public static List<SkillMain> GetSlots() {
            List<SkillMain> slots = new List<SkillMain>();
            SkillMain[] sl = Resources.FindObjectsOfTypeAll<SkillMain>();

            foreach (SkillMain s in sl) {
                // Check if the slow is active in the hierarchy
                if (s.gameObject.activeInHierarchy)
                    slots.Add(s);
            }

            return slots;
        }

        /// <summary>
        /// Gets all the slots with the specified ID.
        /// </summary>
        /// <returns>The slots.</returns>
        /// <param name="ID">The slot ID.</param>
        public static List<SkillMain> GetSlotsWithID(int ID) {
            List<SkillMain> slots = new List<SkillMain>();
            SkillMain[] sl = Resources.FindObjectsOfTypeAll<SkillMain>();

            foreach (SkillMain s in sl) {
                // Check if the slow is active in the hierarchy
                if (s.gameObject.activeInHierarchy && s.ID == ID)
                    slots.Add(s);
            }

            return slots;
        }

        /// <summary>
        /// Gets all the slots in the specified group.
        /// </summary>
        /// <returns>The slots.</returns>
        /// <param name="group">The spell slot group.</param>
        public static List<SkillMain> GetSlotsInGroup(SkillMain_Group group) {
            List<SkillMain> slots = new List<SkillMain>();
            SkillMain[] sl = Resources.FindObjectsOfTypeAll<SkillMain>();

            foreach (SkillMain s in sl) {
                // Check if the slow is active in the hierarchy
                if (s.gameObject.activeInHierarchy && s.slotGroup == group)
                    slots.Add(s);
            }

            // Sort the slots by id
            slots.Sort(delegate (SkillMain a, SkillMain b) {
                return a.ID.CompareTo(b.ID);
            });

            return slots;
        }

        /// <summary>
        /// Gets the slot with the specified ID and Group.
        /// </summary>
        /// <returns>The slot.</returns>
        /// <param name="ID">The slot ID.</param>
        /// <param name="group">The slot Group.</param>
        public static SkillMain GetSlot(int ID, SkillMain_Group group) {
            SkillMain[] sl = Resources.FindObjectsOfTypeAll<SkillMain>();

            foreach (SkillMain s in sl) {
                // Check if the slow is active in the hierarchy
                if (s.gameObject.activeInHierarchy && s.ID == ID && s.slotGroup == group)
                    return s;
            }

            return null;
        }
        #endregion

    }
}
