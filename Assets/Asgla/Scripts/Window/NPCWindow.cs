using Asgla.NPC;
using AsglaUI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.Window {
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(CanvasGroup))]
    public class NPCWindow : UIWindow {

        [SerializeField] private TextMeshProUGUI _name = null;

        [SerializeField] private TextMeshProUGUI _description = null;

        [SerializeField] private Button _quest = null;

        [SerializeField] private Button _shop = null;

        private NPCMain _npc = null;

        //[SerializeField] private GameObject _npcImgTemp;

        #region Unity
        protected override void Start() {
            base.Start();
        }
        #endregion

        public override void Show() {
            base.Show();
            //UIUtility.BringToFront(_npcImgTemp);
            //_npcImgTemp.SetActive(true);
        }

        public override void Hide() {
            base.Hide();
            //_npcImgTemp.SetActive(false);
        }

        public void Init(NPCMain npc) {
            _npc = npc;

            _name.text = _npc.Name();
            _description.text = _npc.Description();

            Show();
        }

        public void OnShopClick() => Main.Singleton.Request.Send("ShopLoad", _npc.ShopId());

        public void OnQuestClick() => Main.Singleton.Request.Send("QuestLoad", _npc.NPCId());

    }
}
