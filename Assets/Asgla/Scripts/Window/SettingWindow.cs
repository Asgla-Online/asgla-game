using Asgla.Data;
using AsglaUI.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Asgla.Window {

    [DisallowMultipleComponent, RequireComponent(typeof(CanvasGroup))]
    public class SettingWindow : UIWindow {

        [SerializeField] private UISelectField _resolutionSelect = null;
        [SerializeField] private UISelectField _graphicSelect = null;
        [SerializeField] private Slider _volumeSlider = null;
        [SerializeField] private Toggle _fullScreen = null;

        protected override void Start() {
            base.Start();

            //Resolution select
            _resolutionSelect.ClearOptions();

            foreach (Resolution res in Screen.resolutions) {
                _resolutionSelect.AddOption(res.width + "x" + res.height + " @ " + res.refreshRate + "Hz");
            }

            Resolution currentRes = Screen.currentResolution;
            _resolutionSelect.SelectOption(currentRes.width + "x" + currentRes.height + " @ " + currentRes.refreshRate + "Hz");

            _fullScreen.isOn = Screen.fullScreen;

            //Graphic select
            _graphicSelect.ClearOptions();

            foreach (URPA asset in Main.Singleton.URPA) {
                _graphicSelect.AddOption(asset.name);
            }

            _volumeSlider.value = PlayerPrefs.GetFloat("volumeMain");

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

        public void FullScreen(bool IsFull) {
            Screen.fullScreen = IsFull;
        }

        public void Resolution(int index, string option) {
            Resolution res = Screen.resolutions[index];

            if (res.Equals(Screen.currentResolution))
                return;

            Screen.SetResolution(res.width, res.height, Screen.fullScreen, res.refreshRate);
        }

        public void Graphic(int index, string option) {
            URPA urpa = Main.Singleton.URPA.Where(u => u.name == option).FirstOrDefault();

            if (urpa != null) {
                GraphicsSettings.renderPipelineAsset = urpa.asset;
            }
        }

    }

}