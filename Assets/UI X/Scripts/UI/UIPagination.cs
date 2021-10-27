using Asgla.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[AddComponentMenu("UI/Pagination", 82)]
	public class UIPagination : MonoBehaviour {

		private int activePage;

		private void Start() {
			if (m_ButtonPrev != null)
				m_ButtonPrev.onClick.AddListener(OnPrevClick);

			if (m_ButtonNext != null)
				m_ButtonNext.onClick.AddListener(OnNextClick);

			// Detect active page
			if (m_PagesContainer != null && m_PagesContainer.childCount > 0)
				for (int i = 0; i < m_PagesContainer.childCount; i++)
					if (m_PagesContainer.GetChild(i).gameObject.activeSelf) {
						activePage = i;
						break;
					}

			// Prepare the pages visibility
			UpdatePagesVisibility();
		}

		private void UpdatePagesVisibility() {
			if (m_PagesContainer == null)
				return;

			if (m_PagesContainer.childCount > 0)
				for (int i = 0; i < m_PagesContainer.childCount; i++)
					m_PagesContainer.GetChild(i).gameObject.SetActive(i == activePage ? true : false);

			// Format and update the label text
			if (m_LabelText != null)
				m_LabelText.text = "<color=#" + CommonColorBuffer.ColorToString(m_LabelActiveColor) + ">" +
				                   (activePage + 1) + "</color> / "
				                   + m_PagesContainer.childCount;
		}

		private void OnPrevClick() {
			if (!isActiveAndEnabled || m_PagesContainer == null)
				return;

			// If we are on the first page, jump to the last one
			if (activePage == 0)
				activePage = m_PagesContainer.childCount - 1;
			else
				activePage -= 1;

			// Activate
			UpdatePagesVisibility();
		}

		private void OnNextClick() {
			if (!isActiveAndEnabled || m_PagesContainer == null)
				return;

			// If we are on the last page, jump to the first one
			if (activePage == m_PagesContainer.childCount - 1)
				activePage = 0;
			else
				activePage += 1;

			// Activate
			UpdatePagesVisibility();
		}

#pragma warning disable 0649
		[SerializeField] private Transform m_PagesContainer;
		[SerializeField] private Button m_ButtonPrev;
		[SerializeField] private Button m_ButtonNext;
		[SerializeField] private Text m_LabelText;
		[SerializeField] private Color m_LabelActiveColor = Color.white;
#pragma warning restore 0649

	}
}