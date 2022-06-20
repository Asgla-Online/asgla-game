using System.Collections.Generic;
using System.Linq;
using Asgla.Avatar.Monster;
using Asgla.Avatar.Player;
using Asgla.Controller;
using Asgla.Data;
using Asgla.Data.Web;
using Asgla.Effect;
using Asgla.Scenes;
using CharacterCreator2D;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

namespace Asgla {
	public class Main : MonoBehaviour {

		public BodyType _BodyType;

		public bool walkable = true;

		public Player playerPrefab;
		public Monster monsterPrefab;

		[SerializeField] public List<GraphicQuality> graphicQualities;

		public LoginWebRequest Login = null;

		public EffectMain GameAsset { get; private set; }

		public AudioMixerGroup AudioMixer => audioMixer;

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
			UIManager.Main = this;

			GraphicsSettings.renderPipelineAsset = graphicQualities.First().asset;

			//Volume
			float volume = PlayerPrefs.HasKey("volumeMain") ? PlayerPrefs.GetFloat("volumeMain") : 0.15f;

			audioMixer.audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
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
			Game.Main = this;
		}

		public void SetGameAsset(GameObject go) {
			//if (_gameAssetBundle != null)
			//    Destroy(_gameAssetBundle.gameObject);
			GameAsset = go.GetComponent<EffectMain>();
		}

		#region Controller

		public static Main Singleton { get; private set; }

		public Game Game { get; private set; }

		public Network Network { get; } = new Network();

		public RequestController Request { get; } = new RequestController();

		public UIController UIManager { get; } = new UIController();

		#endregion

		#region Game Information

		public const string URLBase = "http://localhost/";
		public const string URLLogin = "http://localhost/login.php";
		public const string URLBundle = "http://localhost/gamebundles";

		[SerializeField] private AudioMixerGroup audioMixer;

		public const int SceneLogin = 0;
		public const int SceneServers = 1;
		public const int SceneGame = 2;

		#endregion

	}
}