using Asgla.Avatar.NPC;
using AsglaUI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI.Window {
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	public class NpcWindow : UIWindow {

		[SerializeField] private TextMeshProUGUI npcName;

		[SerializeField] private TextMeshProUGUI description;

		[SerializeField] private Button quest;

		[SerializeField] private Button shop;

		private NpcMain _npc;

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

		public void Init(NpcMain npc) {
			_npc = npc;

			npcName.text = _npc.Name();
			description.text = _npc.Description();

			Show();
		}

		public void OnShopClick() {
			Main.Singleton.Request.Send("ShopLoad", _npc.ShopId());
		}

		public void OnQuestClick() {
			Main.Singleton.Request.Send("QuestLoad", _npc.NpcId());
		}

	}
}