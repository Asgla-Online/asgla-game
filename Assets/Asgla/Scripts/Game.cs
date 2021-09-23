using Asgla.Data.Map;
using Asgla.Quest;
using Asgla.Skill;
using Asgla.UI;
using Asgla.UI.ActionBar;
using Asgla.UI.Loading;
using Asgla.UI.Quest.Track;
using Asgla.UI.UnitFrame;
using Asgla.Window;
using AsglaUI.UI;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace Asgla {
    public class Game : MonoBehaviour {

        #region Camera
        [SerializeField] private Camera _camera = null;

        [HideInInspector] private CinemachineVirtualCamera _cinemachineVirtual = null;

        [HideInInspector] private CinemachineConfiner _cinemachineConfiner = null;
        #endregion

        private QuestMain _quest = null;

        [SerializeField] private QuestTracker _questTrack = null;

        #region UI
        [Header("Unit Frame")]
        [SerializeField] private UnitFrameBig _unitFramePlayer = null;
        [SerializeField] private UnitFrameBig _unitFrameTarget = null;

        [Header("Window")]
        [SerializeField] private InventoryWindow _windowInventory = null;
        [SerializeField] private ItemPreviewWindow _windowItemPreview = null;
        [SerializeField] private NPCWindow _windowNPC = null;
        [SerializeField] private QuestWindow _windowQuest = null;
        [SerializeField] private RespawnWindow _windowRespawn = null;
        [SerializeField] private SettingWindow _windowSetting = null;
        [SerializeField] private ShopWindow _windowShop = null;

        [Header("Bar")]
        [SerializeField] private ActionBar _actionBar = null;
        [SerializeField] private UICastBar _castBar = null;

        [SerializeField] private Chat _chat = null;

        [Header("Notification")]
        [SerializeField] private Notification _notificationTop = null;
        [SerializeField] private Notification _notificationMiddle = null;
        #endregion

        #region Unity
        private void Awake() {
            Transform cinemachine = _camera.transform.GetChild(0);

            //Debug.Log(cinemachine.name);

            _cinemachineVirtual = cinemachine.GetComponent<CinemachineVirtualCamera>();
            _cinemachineConfiner = cinemachine.GetComponent<CinemachineConfiner>();

            Main.Singleton.SetGame(this);

            Main.Singleton.AvatarManager.Players = new List<MapAvatar>();
            Main.Singleton.AvatarManager.Monsters = new List<MapAvatar>();

            _quest = new QuestMain(this);

            _unitFrameTarget.gameObject.SetActive(false); //testing
        }

        private void Start() {
            LoadingAssetOverlay loadingAsset = Main.Singleton.UIManager.CreateLoadingAsset();

            if (loadingAsset == null)
                Main.Singleton.UIManager.LoadingOverlay.SetLoadingText("Error(Null Asset) loading asset, please contact Asgla Team.");

            loadingAsset.LoadAsset();
            //Main.Singleton.Request.Send("JoinFirst");
        }

        private void Update() {
            /*RaycastHit2D hit;

            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
            if (hit = Physics2D.Raycast(ray.origin, new Vector2(0, 0)))
                Debug.Log(hit.collider.name);*/

            //TODO: Replace? this look bad
            if (!Chat.ChatInput.isFocused) {
                if (Input.GetKeyDown(KeyCode.Alpha1)) {
                    ActionBar.OnSkillClick(SkillMain.GetSlot(1, SkillMain_Group.Skill));
                }
                if (Input.GetKeyDown(KeyCode.Alpha2)) {
                    ActionBar.OnSkillClick(SkillMain.GetSlot(2, SkillMain_Group.Skill));
                }
                if (Input.GetKeyDown(KeyCode.Alpha3)) {
                    ActionBar.OnSkillClick(SkillMain.GetSlot(3, SkillMain_Group.Skill));
                }
                if (Input.GetKeyDown(KeyCode.Alpha4)) {
                    ActionBar.OnSkillClick(SkillMain.GetSlot(4, SkillMain_Group.Skill));
                }
                if (Input.GetKeyDown(KeyCode.Alpha5)) {
                    ActionBar.OnSkillClick(SkillMain.GetSlot(5, SkillMain_Group.Skill));
                }
                if (Input.GetKeyDown(KeyCode.Return)) {
                    Chat.ChatInput.Select();
                }
            }
        }
        #endregion

        public Camera Camera => _camera;

        public CinemachineVirtualCamera CinemachineVirtual => _cinemachineVirtual;

        public CinemachineConfiner CinemachineConfiner => _cinemachineConfiner;

        public QuestMain Quest => _quest;

        public QuestTracker QuestTrack => _questTrack;

        public UnitFrameBig UnitFramePlayer => _unitFramePlayer;

        public UnitFrameBig UnitFrameTarget => _unitFrameTarget;

        #region Window
        public InventoryWindow WindowInventory => _windowInventory;

        public ItemPreviewWindow WindowItemPreview => _windowItemPreview;

        public NPCWindow WindowNPC => _windowNPC;

        public RespawnWindow WindowRespawn => _windowRespawn;

        public QuestWindow WindowQuest => _windowQuest;

        public SettingWindow WindowSetting => _windowSetting;

        public ShopWindow WindowShop => _windowShop;
        #endregion

        public ActionBar ActionBar => _actionBar;

        public UICastBar CastBar => _castBar;

        public Chat Chat => _chat;

        public Notification NotificationTop => _notificationTop;

        public Notification NotificationMiddle => _notificationMiddle;

        public void Logout() {
            Main.Singleton.Network.Connection.Close();
        }

    }
}