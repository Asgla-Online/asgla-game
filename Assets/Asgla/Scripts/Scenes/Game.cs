using System;
using System.Collections.Generic;
using Asgla.Controller;
using Asgla.Controller.Game;
using Asgla.Data.Area;
using Asgla.Skill;
using Asgla.UI;
using Asgla.UI.Action_Bar;
using Asgla.UI.Loading;
using Asgla.UI.Quest.Track;
using Asgla.UI.Unit_Frame;
using Asgla.UI.Window;
using AsglaUI.UI;
using Cinemachine;
using UnityEngine;

namespace Asgla.Scenes {
	public class Game : MonoBehaviour {

		[SerializeField] private QuestTracker questTrack;

		[NonSerialized] public Main Main;

		public QuestTracker QuestTrack => questTrack;

		public UnitFrameBig UnitFramePlayer => unitFramePlayer;

		public UnitFrameBig UnitFrameTarget => unitFrameTarget;

		public ActionBar ActionBar => actionBar;

		public UICastBar CastBar => castBar;

		public Chat Chat => chat;

		public Notification NotificationTop => notificationTop;

		public Notification NotificationMiddle => notificationMiddle;

		public void Logout() {
			Main.Singleton.Network.Connection.Close();
		}

		#region Camera

		[SerializeField] private Camera cameraGame;

		public Camera CameraGame => cameraGame;

		[field: HideInInspector] public CinemachineVirtualCamera CinemachineVirtual { get; private set; }

		[field: HideInInspector] public CinemachineConfiner CinemachineConfiner { get; private set; }

		#endregion

		#region Controller Game

		public AvatarController AvatarController { get; } = new AvatarController();

		public AreaController AreaController { get; } = new AreaController();

		public QuestController QuestController { get; } = new QuestController();

		#endregion

		#region UI

		[Header("Unit Frame")] [SerializeField]
		private UnitFrameBig unitFramePlayer;

		[SerializeField] private UnitFrameBig unitFrameTarget;

		[Header("Window")] [SerializeField] private InventoryWindow windowInventory;

		[SerializeField] private ItemPreviewWindow windowItemPreview;
		[SerializeField] private NpcWindow windowNpc;
		[SerializeField] private QuestWindow windowQuest;
		[SerializeField] private RespawnWindow windowRespawn;
		[SerializeField] private SettingWindow windowSetting;
		[SerializeField] private ShopWindow windowShop;

		[Header("Bar")] [SerializeField] private ActionBar actionBar;

		[SerializeField] private UICastBar castBar;

		[SerializeField] private Chat chat;

		[Header("Notification")] [SerializeField]
		private Notification notificationTop;

		[SerializeField] private Notification notificationMiddle;

		#endregion

		#region Unity

		private void Awake() {
			Main.Singleton.SetGame(this);

			AvatarController.Main = Main;
			AreaController.Main = Main;
			QuestController.Main = Main;

			Transform cinemachine = cameraGame.transform.GetChild(0);

			//Debug.Log(cineMachine.name);

			CinemachineVirtual = cinemachine.GetComponent<CinemachineVirtualCamera>();
			CinemachineConfiner = cinemachine.GetComponent<CinemachineConfiner>();

			AvatarController.Players = new List<AreaAvatar>();
			AvatarController.Monsters = new List<AreaAvatar>();

			unitFrameTarget.gameObject.SetActive(false); //testing
		}

		private void Start() {
			LoadingAssetOverlay loadingAsset = UIController.CreateLoadingAsset();

			if (loadingAsset == null)
				Main.UIManager.LoadingOverlay.SetLoadingText(
					"Error(Null Asset) loading asset, please contact Asgla Team.");

			loadingAsset.LoadAsset();
			//Main.Request.Send("JoinFirst");
		}

		private void Update() {
			/*RaycastHit2D hit;

			Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
			if (hit = Physics2D.Raycast(ray.origin, new Vector2(0, 0)))
			    Debug.Log(hit.collider.name);*/

			//TODO: Replace? this look bad
			if (!Chat.ChatInput.isFocused) {
				if (Input.GetKeyDown(KeyCode.Alpha1))
					ActionBar.OnSkillClick(SkillMain.GetSlot(1, SkillMain_Group.Skill));
				if (Input.GetKeyDown(KeyCode.Alpha2))
					ActionBar.OnSkillClick(SkillMain.GetSlot(2, SkillMain_Group.Skill));
				if (Input.GetKeyDown(KeyCode.Alpha3))
					ActionBar.OnSkillClick(SkillMain.GetSlot(3, SkillMain_Group.Skill));
				if (Input.GetKeyDown(KeyCode.Alpha4))
					ActionBar.OnSkillClick(SkillMain.GetSlot(4, SkillMain_Group.Skill));
				if (Input.GetKeyDown(KeyCode.Alpha5))
					ActionBar.OnSkillClick(SkillMain.GetSlot(5, SkillMain_Group.Skill));
				if (Input.GetKeyDown(KeyCode.Return))
					Chat.ChatInput.Select();
			}
		}

		#endregion

		#region Window

		public InventoryWindow WindowInventory => windowInventory;

		public ItemPreviewWindow WindowItemPreview => windowItemPreview;

		public NpcWindow WindowNpc => windowNpc;

		public RespawnWindow WindowRespawn => windowRespawn;

		public QuestWindow WindowQuest => windowQuest;

		public SettingWindow WindowSetting => windowSetting;

		public ShopWindow WindowShop => windowShop;

		#endregion

	}
}