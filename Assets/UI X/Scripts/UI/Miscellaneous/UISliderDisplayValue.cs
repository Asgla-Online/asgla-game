using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	public class UISliderDisplayValue : MonoBehaviour {

		public enum DisplayValue {

			Raw,
			Percentage

		}

		protected void Awake() {
			if (m_slider == null) m_slider = gameObject.GetComponent<Slider>();
		}

		protected void OnEnable() {
			if (m_slider != null) {
				m_slider.onValueChanged.AddListener(SetValue);
				SetValue(m_slider.value);
			}
		}

		protected void OnDisable() {
			if (m_slider != null)
				m_slider.onValueChanged.RemoveListener(SetValue);
		}

		public void SetValue(float value) {
			if (m_Text != null) {
				if (m_Display == DisplayValue.Percentage)
					m_Text.text = (value * 100f).ToString(m_Format) + m_Append;
				else
					m_Text.text = value.ToString(m_Format) + m_Append;
			}
		}

#pragma warning disable 0649
		[SerializeField] private Slider m_slider;
		[SerializeField] private Text m_Text;
		[SerializeField] private DisplayValue m_Display = DisplayValue.Percentage;
		[SerializeField] private string m_Format = "0";
		[SerializeField] private string m_Append = "%";
#pragma warning restore 0649

	}
}