using System.Linq;
using Asgla.Data;
using AsglaUI.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Asgla.UI.Window {

	[DisallowMultipleComponent]
	[RequireComponent(typeof(CanvasGroup))]
	public class SettingWindow : UIWindow {

		[SerializeField] private UISelectField resolutionSelect;
		[SerializeField] private UISelectField graphicSelect;
		[SerializeField] private Slider volumeSlider;
		[SerializeField] private Toggle fullScreen;

		protected override void Start() {
			base.Start();

			//Resolution select
			resolutionSelect.ClearOptions();

			foreach (Resolution res in Screen.resolutions)
				resolutionSelect.AddOption(res.width + "x" + res.height + " @ " + res.refreshRate + "Hz");

			Resolution currentRes = Screen.currentResolution;
			resolutionSelect.SelectOption(currentRes.width + "x" + currentRes.height + " @ " + currentRes.refreshRate +
			                              "Hz");

			fullScreen.isOn = Screen.fullScreen;

			//Graphic select
			graphicSelect.ClearOptions();

			foreach (URPA asset in Main.Singleton.URPA)
				graphicSelect.AddOption(asset.name);

			volumeSlider.value = PlayerPrefs.GetFloat("volumeMain");

			//Resolution currentGra = Screen.currentResolution;
			//_graphicSelect.SelectOption(currentGra.width + "x" + currentGra.height + " @ " + currentGra.refreshRate + "Hz");

			//FullScreen(Screen.fullScreen);
		}

		public void Volume(float volume) {
			PlayerPrefs.SetFloat("volumeMain", volume);
			volume = Mathf.Log10(volume) * 20;
			Main.Singleton.AudioMixer.audioMixer.SetFloat("volume", volume);
			PlayerPrefs.Save();
		}

		public void FullScreen(bool isFull) {
			Screen.fullScreen = isFull;
		}

		public void Resolution(int index, string option) {
			Resolution res = Screen.resolutions[index];

			if (res.Equals(Screen.currentResolution))
				return;

			Screen.SetResolution(res.width, res.height, Screen.fullScreen, res.refreshRate);
		}

		public void Graphic(int index, string option) {
			URPA urpa = Main.Singleton.URPA.Where(u => u.name == option).FirstOrDefault();

			if (urpa != null)
				GraphicsSettings.renderPipelineAsset = urpa.asset;
		}

	}

}