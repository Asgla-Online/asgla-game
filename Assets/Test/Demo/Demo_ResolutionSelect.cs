using UnityEngine;

namespace AsglaUI.UI {
	public class Demo_ResolutionSelect : MonoBehaviour {

#pragma warning disable 0649
		[SerializeField] private UISelectField m_SelectField;
#pragma warning restore 0649

		protected void Start() {
			if (m_SelectField == null)
				return;

			// Clear the options
			m_SelectField.ClearOptions();

			// Add the supported resolutions
			Resolution[] resolutions = Screen.resolutions;

			foreach (Resolution res in resolutions)
				// Add new resolution option
				m_SelectField.AddOption(res.width + "x" + res.height + " @ " + res.refreshRate + "Hz");

			Resolution currentResolution = Screen.currentResolution;

			// Set the current resolution as selected
			m_SelectField.SelectOption(currentResolution.width + "x" + currentResolution.height + " @ " +
			                           currentResolution.refreshRate + "Hz");
		}

		protected void OnEnable() {
			if (m_SelectField == null)
				return;

			m_SelectField.onChange.AddListener(OnSelectedOption);
		}

		protected void OnDisable() {
			if (m_SelectField == null)
				return;

			m_SelectField.onChange.RemoveListener(OnSelectedOption);
		}

		protected void OnSelectedOption(int index, string option) {
			Resolution res = Screen.resolutions[index];

			if (res.Equals(Screen.currentResolution))
				return;

			Screen.SetResolution(res.width, res.height, true, res.refreshRate);
		}

	}
}