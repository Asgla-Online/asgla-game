using Asgla.Avatar.Monster;
using Asgla.Avatar.Player;
using Asgla.Controller;
using Asgla.Data;
using Asgla.Effect;
using AsglaUI.UI;
using AssetBundles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

namespace Asgla {
    public class Main : MonoBehaviour {

        public static Main Singleton { get; protected set; }

        public bool Walkable = true;

        [HideInInspector] public AssetBundleManager Abm;

        private Game _game = null;

        private readonly Network _network = new Network();
        private readonly RequestController _request = new RequestController();

        private readonly AvatarController _avatarManager = new AvatarController();
        private readonly MapController _mapManager = new MapController();
        private readonly UIController _uiManager = new UIController();

        public Player PlayerPrefab;
        public Monster MonsterPrefab;

        [SerializeField] private GameObject _loadingOverlay = null;

        #region Base
        [SerializeField] private List<URPA> _universalRenderPipelineAsset = null;
        #endregion

        #region Game Information
        public readonly string url_base = "https://asgla.online/";
        public readonly string url_login = "https://asgla.online/api/game/login";
        public readonly string url_bundle = "https://asgla.online/gamebundles";

        private EffectMain _gameAssetBundle = null;
        [SerializeField] private AudioMixerGroup _audioMixer = null;

        public readonly int SceneLogin = 0;
        //public readonly int SceneCharacterSelect = 1;
        public readonly int SceneGame = 1;
        //public readonly int SceneRegister = 3;
        #endregion

        #region Unity
        private void Awake() {
            Application.runInBackground = true;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //Application.targetFrameRate = 60;

            Debug.LogFormat("Build target {0}", Application.platform);

            if (Singleton != null) {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            Singleton = this;

            _network.Main = this;
            _request.Main = this;

            _avatarManager.Main = this;
            _mapManager.Main = this;
            _uiManager.Main = this;

            GraphicsSettings.renderPipelineAsset = _universalRenderPipelineAsset.First().asset;

            //Volume
            float volume = PlayerPrefs.HasKey("volumeMain") ? PlayerPrefs.GetFloat("volumeMain") : 0.15f;

            _audioMixer.audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("volumeMain", volume);

            PlayerPrefs.Save();

            #if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
            #else
            Debug.unityLogger.logEnabled = false;
            #endif

            /*if (Caching.ClearCache()) {
                Debug.Log("Successfully cleaned the cache.");
            } else {
                Debug.Log("Cache is being used.");
            }*/
        }
        #endregion

        public Game Game => _game;

        public Network Network => _network;

        public RequestController Request => _request;

        public AvatarController AvatarManager => _avatarManager;

        public MapController MapManager => _mapManager;

        public UIController UIManager => _uiManager;

        public GameObject Loading => _loadingOverlay;

        public List<URPA> URPA => _universalRenderPipelineAsset;

        public EffectMain GameAsset => _gameAssetBundle;

        public AudioMixerGroup AudioMixer => _audioMixer;

        public void SetGame(Game game) => _game = game;

        public void SetGameAsset(GameObject go) {
            //if (_gameAssetBundle != null)
            //    Destroy(_gameAssetBundle.gameObject);
            _gameAssetBundle = go.GetComponent<EffectMain>();
        }

        public void Login(UIModalBox modal, string token) {
            UIManager.Modal = modal;
            Network.ConnectToServer(token);
        }

    }
}