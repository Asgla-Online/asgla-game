using System.Collections.Generic;
using System.Linq;
using Asgla.Avatar.Monster;
using Asgla.Avatar.Player;
using Asgla.Controller;
using Asgla.Data;
using Asgla.Effect;
using Asgla.Scenes;
using AsglaUI.UI;
using AssetBundles;
using CharacterCreator2D;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

namespace Asgla {
	public class Main : MonoBehaviour {

		public Part part1; //test
		public Part part2; //test

		public bool Walkable = true;

		public Player PlayerPrefab;
		public Monster MonsterPrefab;

		[SerializeField] private GameObject _loadingOverlay;

		#region Base

		[SerializeField] private List<URPA> _universalRenderPipelineAsset;

		#endregion

		[HideInInspector] public AssetBundleManager Abm;

		public static Main Singleton { get; protected set; }

		public Game Game { get; private set; }

		public Network Network { get; } = new Network();

		public RequestController Request { get; } = new RequestController();

		public AvatarController AvatarManager { get; } = new AvatarController();

		public MapController MapManager { get; } = new MapController();

		public UIController UIManager { get; } = new UIController();

		public GameObject Loading => _loadingOverlay;

		public List<URPA> URPA => _universalRenderPipelineAsset;

		public EffectMain GameAsset { get; private set; }

		public AudioMixerGroup AudioMixer => _audioMixer;

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

			Network.Main = this;
			Request.Main = this;

			AvatarManager.Main = this;
			MapManager.Main = this;
			UIManager.Main = this;

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

		public void SetGame(Game game) {
			Game = game;
		}

		public void SetGameAsset(GameObject go) {
			//if (_gameAssetBundle != null)
			//    Destroy(_gameAssetBundle.gameObject);
			GameAsset = go.GetComponent<EffectMain>();
		}

		public void Login(UIModalBox modal, string token) {
			UIManager.Modal = modal;
			Network.ConnectToServer(token);
		}

		#region Game Information

		public readonly string url_base = "https://asgla.online/";
		public readonly string url_login = "https://asgla.online/api/game/login";
		public readonly string url_bundle = "https://asgla.online/gamebundles";

		[SerializeField] private AudioMixerGroup _audioMixer;

		public readonly int SceneLogin = 0;

		//public readonly int SceneCharacterSelect = 1;
		public readonly int SceneGame = 1;
		//public readonly int SceneRegister = 3;

		#endregion

	}
}