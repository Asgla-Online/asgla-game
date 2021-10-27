using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[AddComponentMenu("UI/Bars/Step Bar")]
	public class UIStepBar : MonoBehaviour {

		/// <summary>
		///     Gets or sets the current step.
		/// </summary>
		/// <value>The value.</value>
		public int step {
			get => m_CurrentStep;
			set => GoToStep(value);
		}

		protected virtual void Start() {
			UpdateBubble();
		}

		/// <summary>
		///     Raises the rect transform dimensions change event.
		/// </summary>
		protected virtual void OnRectTransformDimensionsChange() {
			if (!IsActive())
				return;

			UpdateGridProperties();
		}

		/// <summary>
		///     Determines whether this instance is active.
		/// </summary>
		/// <returns><c>true</c> if this instance is active; otherwise, <c>false</c>.</returns>
		public virtual bool IsActive() {
			return enabled && gameObject.activeInHierarchy;
		}

		/// <summary>
		///     Gets the fill override values list.
		/// </summary>
		/// <returns>The override fill list.</returns>
		public List<StepFillInfo> GetOverrideFillList() {
			return m_OverrideFillList;
		}

		/// <summary>
		///     Sets the fill override values list.
		/// </summary>
		/// <param name="list">List.</param>
		public void SetOverrideFillList(List<StepFillInfo> list) {
			m_OverrideFillList = list;
		}

		/// <summary>
		///     Validates the fill override values list.
		/// </summary>
		public void ValidateOverrideFillList() {
			// Create a temporary list
			List<StepFillInfo> list = new List<StepFillInfo>();

			// Copy the current list to array
			StepFillInfo[] tempArr = m_OverrideFillList.ToArray();

			// Loop
			foreach (StepFillInfo info in tempArr)
				// Add all the valid step infos to the temporary list
				if (info.index > 1 && info.index <= m_StepsCount && info.amount > 0f)
					list.Add(info);

			// Set the temporary list as the override list
			m_OverrideFillList = list;
		}

		/// <summary>
		///     Goes to the specified step.
		/// </summary>
		/// <param name="step">Step.</param>
		public void GoToStep(int step) {
			// Validate the step
			if (step < 0) step = 0;
			if (step > m_StepsCount) step = m_StepsCount + 1;

			// Set the step
			m_CurrentStep = step;

			// Update the steps properties
			UpdateStepsProperties();

			// Update the fill amount
			UpdateFillImage();

			// Update the bubble
			UpdateBubble();
		}

		/// <summary>
		///     Updates the fill image.
		/// </summary>
		public void UpdateFillImage() {
			if (m_FillImage == null)
				return;

			int overrideIndex = m_OverrideFillList.FindIndex(x => x.index == m_CurrentStep);

			// Apply fill amount
			m_FillImage.fillAmount = overrideIndex >= 0
				? m_OverrideFillList[overrideIndex].amount
				: GetStepFillAmount(m_CurrentStep);
		}

		/// <summary>
		///     Updates the bubble.
		/// </summary>
		public void UpdateBubble() {
			if (m_BubbleRect == null)
				return;

			// Determine if the bubble should be visible
			if (m_CurrentStep > 0 && m_CurrentStep <= m_StepsCount) {
				// Activate if required
				if (!m_BubbleRect.gameObject.activeSelf)
					m_BubbleRect.gameObject.SetActive(true);

				// Get the step game object
				GameObject stepObject = m_StepsGameObjects[m_CurrentStep];

				if (stepObject != null) {
					RectTransform stepRect = stepObject.transform as RectTransform;

					if (stepRect.anchoredPosition.x != 0f)
						// Update the bubble position based on the location of the step
						m_BubbleRect.anchoredPosition =
							new Vector2(m_BubbleOffset.x + (stepRect.anchoredPosition.x + stepRect.rect.width / 2f),
								m_BubbleOffset.y);
				}

				// Update the bubble text
				if (m_BubbleText != null)
					m_BubbleText.text = m_CurrentStep.ToString();
			} else {
				// Deactivate if required
				if (m_BubbleRect.gameObject.activeSelf)
					m_BubbleRect.gameObject.SetActive(false);
			}
		}

		/// <summary>
		///     Gets the step fill amount.
		/// </summary>
		/// <returns>The step fill amount.</returns>
		/// <param name="step">Step.</param>
		public float GetStepFillAmount(int step) {
			return 1f / (m_StepsCount + 1) * step;
		}

		/// <summary>
		///     Creates the steps grid.
		/// </summary>
		protected void CreateStepsGrid() {
			if (m_StepsGridGameObject != null)
				return;

			// Create new game object
			m_StepsGridGameObject = new GameObject("Steps Grid", typeof(RectTransform), typeof(GridLayoutGroup));
			m_StepsGridGameObject.layer = gameObject.layer;
			m_StepsGridGameObject.transform.SetParent(transform, false);
			m_StepsGridGameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			m_StepsGridGameObject.transform.localPosition = Vector3.zero;
			m_StepsGridGameObject.transform.SetAsLastSibling();

			// Get the rect transform
			m_StepsGridRect = m_StepsGridGameObject.GetComponent<RectTransform>();
			m_StepsGridRect.sizeDelta = new Vector2(0f, 0f);
			m_StepsGridRect.anchorMin = new Vector2(0f, 0f);
			m_StepsGridRect.anchorMax = new Vector2(1f, 1f);
			m_StepsGridRect.pivot = new Vector2(0f, 1f);
			m_StepsGridRect.anchoredPosition = new Vector2(0f, 0f);

			// Get the grid layout group
			m_StepsGrid = m_StepsGridGameObject.GetComponent<GridLayoutGroup>();

			// Set the bubble as last sibling
			if (m_BubbleRect != null)
				m_BubbleRect.SetAsLastSibling();

			// Clear the steps game objects list
			m_StepsGameObjects.Clear();
		}

		/// <summary>
		///     Updates the grid properties.
		/// </summary>
		public void UpdateGridProperties() {
			if (m_StepsGrid == null)
				return;

			int seps = m_StepsCount + 2;

			// Grid Padding
			if (!m_StepsGrid.padding.Equals(m_StepsGridPadding))
				m_StepsGrid.padding = m_StepsGridPadding;

			// Auto sizing
			if (m_SeparatorAutoSize && m_SeparatorSprite != null)
				m_SeparatorSize = new Vector2(m_SeparatorSprite.rect.width, m_SeparatorSprite.rect.height);

			if (!m_StepsGrid.cellSize.Equals(m_SeparatorSize))
				m_StepsGrid.cellSize = m_SeparatorSize;

			// Grid spacing
			float spacingX =
				Mathf.Floor((m_StepsGridRect.rect.width - m_StepsGridPadding.horizontal - seps * m_SeparatorSize.x) /
				            (seps - 1) / 2f) * 2f;

			if (m_StepsGrid.spacing.x != spacingX)
				m_StepsGrid.spacing = new Vector2(spacingX, 0f);
		}

		/// <summary>
		///     Rebuilds the steps.
		/// </summary>
		public void RebuildSteps() {
			if (m_StepsGridGameObject == null)
				return;

			// Check if we already have the steps
			if (m_StepsGameObjects.Count == m_StepsCount + 2)
				return;

			// Destroy the steps
			DestroySteps();

			int seps = m_StepsCount + 2;

			// Create the steps
			for (int i = 0; i < seps; i++) {
				GameObject step = new GameObject("Step " + i, typeof(RectTransform));
				step.layer = gameObject.layer;
				step.transform.localScale = new Vector3(1f, 1f, 1f);
				step.transform.localPosition = Vector3.zero;
				step.transform.SetParent(m_StepsGridGameObject.transform, false);

				if (i > 0 && i < seps - 1)
					step.AddComponent<Image>();

				// Add to the list
				m_StepsGameObjects.Add(step);
			}
		}

		/// <summary>
		///     Updates the steps properties.
		/// </summary>
		protected void UpdateStepsProperties() {
			// Loop through the options
			foreach (GameObject stepObject in m_StepsGameObjects) {
				int index = m_StepsGameObjects.IndexOf(stepObject) + 1;
				bool active = index <= m_CurrentStep;

				// Image
				Image image = stepObject.GetComponent<Image>();

				if (image != null) {
					image.sprite = m_SeparatorSprite;
					image.overrideSprite = active ? m_SeparatorSpriteActive : null;
					image.color = m_SeparatorSpriteColor;
					image.rectTransform.pivot = new Vector2(0f, 1f);
				}
			}
		}

		/// <summary>
		///     Destroies the steps.
		/// </summary>
		protected void DestroySteps() {
			if (Application.isPlaying) {
				foreach (GameObject g in m_StepsGameObjects)
					Destroy(g);
			} else {
#if UNITY_EDITOR
				GameObject[] objects = m_StepsGameObjects.ToArray();

				EditorApplication.delayCall += () => {
					foreach (GameObject g in objects)
						DestroyImmediate(g);
				};
#endif
			}

			// Clear the list
			m_StepsGameObjects.Clear();
		}

		[Serializable]
		public struct StepFillInfo {

			public int index;
			public float amount;

		}

#pragma warning disable 0649
		[SerializeField] private List<GameObject> m_StepsGameObjects = new List<GameObject>();
		[SerializeField] private List<StepFillInfo> m_OverrideFillList = new List<StepFillInfo>();

		[SerializeField] private GameObject m_StepsGridGameObject;
		[SerializeField] private RectTransform m_StepsGridRect;
		[SerializeField] private GridLayoutGroup m_StepsGrid;

		[SerializeField] private int m_CurrentStep;
		[SerializeField] private int m_StepsCount = 1;
		[SerializeField] private RectOffset m_StepsGridPadding = new RectOffset();
		[SerializeField] private Sprite m_SeparatorSprite;
		[SerializeField] private Sprite m_SeparatorSpriteActive;
		[SerializeField] private Color m_SeparatorSpriteColor = Color.white;
		[SerializeField] private bool m_SeparatorAutoSize = true;
		[SerializeField] private Vector2 m_SeparatorSize = Vector2.zero;
		[SerializeField] private Image m_FillImage;
		[SerializeField] private RectTransform m_BubbleRect;
		[SerializeField] private Vector2 m_BubbleOffset = Vector2.zero;
		[SerializeField] private Text m_BubbleText;
#pragma warning restore 0649

#if UNITY_EDITOR
		/// <summary>
		///     Editor only!
		/// </summary>
		public void RebuildSteps_Editor() {
			// Create the steps grid if required
			CreateStepsGrid();

			// Update the grid properties
			UpdateGridProperties();

			// Rebuild the steps if required
			RebuildSteps();

			// Update the steps properties
			UpdateStepsProperties();

			// Update the bubble
			UpdateBubble();
		}

		/// <summary>
		///     Raises the validate event.
		/// </summary>
		protected virtual void OnValidate() {
			// Validate the step integers
			if (m_CurrentStep < 0) m_CurrentStep = 0;
			if (m_StepsCount < 1) m_StepsCount = 1;
			if (m_CurrentStep > m_StepsCount) m_CurrentStep = m_StepsCount + 1;

			// Create the steps grid if required
			//this.CreateStepsGrid();

			// Update the grid properties
			UpdateGridProperties();

			// Rebuild the steps if required
			//this.RebuildSteps();

			// Update the steps properties
			UpdateStepsProperties();

			// Validate the fill image
			if (m_FillImage != null) {
				// Validate the image type
				if (m_FillImage.type != Image.Type.Filled)
					m_FillImage.type = Image.Type.Filled;

				// Validate the fill method
				if (m_FillImage.fillMethod != Image.FillMethod.Horizontal)
					m_FillImage.fillMethod = Image.FillMethod.Horizontal;

				// Update the fill amount
				UpdateFillImage();
			}

			// Update the bubble
			UpdateBubble();
		}
#endif

	}
}