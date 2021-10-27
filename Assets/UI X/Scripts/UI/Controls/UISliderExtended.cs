using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[AddComponentMenu("UI/Slider Extended", 58)]
	public class UISliderExtended : Slider {

		public enum OptionTransition {

			None,
			ColorTint

		}

		public enum TextEffectType {

			None,
			Shadow,
			Outline

		}

		public enum TransitionTarget {

			Image,
			Text

		}

		/// <summary>
		///     Gets or sets the options list (Rebuilds the options on set).
		/// </summary>
		/// <value>The options.</value>
		public List<string> options {
			get => m_Options;
			set{
				m_Options = value;
				RebuildOptions();
				ValidateOptions();
			}
		}

		/// <summary>
		///     Gets the selected option game object.
		/// </summary>
		/// <value>The selected option game object.</value>
		public GameObject selectedOptionGameObject { get; private set; }

		/// <summary>
		///     Gets the index of the selected option.
		/// </summary>
		/// <value>The index of the selected option.</value>
		public int selectedOptionIndex {
			get{
				int optionIndex = Mathf.RoundToInt(value);

				// Validate the index
				if (optionIndex < 0 || optionIndex >= m_Options.Count)
					return 0;

				return optionIndex;
			}
		}

		/// <summary>
		///     Gets or sets the options grid padding.
		/// </summary>
		/// <value>The options padding.</value>
		public RectOffset optionsPadding {
			get => m_OptionsPadding;
			set => m_OptionsPadding = value;
		}

		/// <summary>
		///     Raises the enable event.
		/// </summary>
		protected override void OnEnable() {
			base.OnEnable();
			ValidateOptions();

			// Add the listener for the value change
			onValueChanged.AddListener(OnValueChanged);
		}

		/// <summary>
		///     Raises the disable event.
		/// </summary>
		protected override void OnDisable() {
			base.OnDisable();

			// Remove the listener for the value change
			onValueChanged.RemoveListener(OnValueChanged);
		}

		/// <summary>
		///     Raises the rect transform dimensions change event.
		/// </summary>
		protected override void OnRectTransformDimensionsChange() {
			base.OnRectTransformDimensionsChange();

			if (!IsActive())
				return;

			UpdateGridProperties();
		}

#if UNITY_EDITOR
		/// <summary>
		///     Raises the validate event.
		/// </summary>
		protected override void OnValidate() {
			base.OnValidate();
			ValidateOptions();

			if (m_OptionTextFont == null)
				m_OptionTextFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		}
#endif

		/// <summary>
		///     Determines whether this slider has options.
		/// </summary>
		/// <returns><c>true</c> if this instance has options; otherwise, <c>false</c>.</returns>
		public bool HasOptions() {
			return m_Options != null && m_Options.Count > 0;
		}

		/// <summary>
		///     Raises the value changed event.
		/// </summary>
		/// <param name="value">Value.</param>
		public void OnValueChanged(float value) {
			if (!IsActive() || !HasOptions())
				return;

			// Transition
			if (m_OptionTransition == OptionTransition.ColorTint) {
				// Transition out the current selected option
				Graphic currentTarget =
					m_OptionTransitionTarget == TransitionTarget.Text
						? selectedOptionGameObject.GetComponentInChildren<Text>()
						: selectedOptionGameObject.GetComponent<Image>() as Graphic;

				// Transition the current target to normal state
				StartColorTween(currentTarget, m_OptionTransitionColorNormal * m_OptionTransitionMultiplier,
					m_OptionTransitionDuration);

				// Get the new value option index
				int newOptionIndex = Mathf.RoundToInt(value);

				// Validate the index
				if (newOptionIndex < 0 || newOptionIndex >= m_Options.Count)
					newOptionIndex = 0;

				// Get the new selected option game object
				GameObject newOptionGameObject = m_OptionGameObjects[newOptionIndex];

				if (newOptionGameObject != null) {
					Graphic newTarget =
						m_OptionTransitionTarget == TransitionTarget.Text
							? newOptionGameObject.GetComponentInChildren<Text>()
							: newOptionGameObject.GetComponent<Image>() as Graphic;

					// Transition the new target to active state
					StartColorTween(newTarget, m_OptionTransitionColorActive * m_OptionTransitionMultiplier,
						m_OptionTransitionDuration);
				}

				// Save the new option game object
				selectedOptionGameObject = newOptionGameObject;
			}
		}

		/// <summary>
		///     Starts a color tween.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="targetColor">Target color.</param>
		/// <param name="duration">Duration.</param>
		private void StartColorTween(Graphic target, Color targetColor, float duration) {
			if (target == null)
				return;

			if (!Application.isPlaying || duration == 0f)
				target.canvasRenderer.SetColor(targetColor);
			else
				target.CrossFadeColor(targetColor, duration, true, true);
		}

		/// <summary>
		///     Validates the options.
		/// </summary>
		protected void ValidateOptions() {
			if (!IsActive())
				return;

			if (!HasOptions()) {
				// Destroy the options container if we have one
				if (m_OptionsContGameObject != null) {
					if (Application.isPlaying)
						Destroy(m_OptionsContGameObject);
					else
						DestroyImmediate(m_OptionsContGameObject);
				}

				return;
			}

			// Make sure we have the options container
			if (m_OptionsContGameObject == null)
				CreateOptionsContainer();

			// Make sure we use whole numbers when using options
			if (!wholeNumbers)
				wholeNumbers = true;

			// Make sure the max value is the options count, when using options
			minValue = 0f;
			maxValue = m_Options.Count - 1f;

			// Update the grid properties
			UpdateGridProperties();

			// Update the options properties
			UpdateOptionsProperties();
		}

		/// <summary>
		///     Updates the grid properties.
		/// </summary>
		public void UpdateGridProperties() {
			if (m_OptionsContGrid == null)
				return;

			// Grid Padding
			if (!m_OptionsContGrid.padding.Equals(m_OptionsPadding))
				m_OptionsContGrid.padding = m_OptionsPadding;

			// Grid Cell Size
			Vector2 cellSize = m_OptionSprite != null
				? new Vector2(m_OptionSprite.rect.width, m_OptionSprite.rect.height)
				: Vector2.zero;

			if (!m_OptionsContGrid.cellSize.Equals(cellSize))
				m_OptionsContGrid.cellSize = cellSize;

			// Grid spacing
			float spacingX =
				(m_OptionsContRect.rect.width - (m_OptionsPadding.left + (float) m_OptionsPadding.right) -
				 m_Options.Count * cellSize.x) / (m_Options.Count - 1f);

			if (m_OptionsContGrid.spacing.x != spacingX)
				m_OptionsContGrid.spacing = new Vector2(spacingX, 0f);
		}

		/// <summary>
		///     Updates the options properties.
		/// </summary>
		public void UpdateOptionsProperties() {
			if (!HasOptions())
				return;

			// Loop through the options
			int i = 0;
			foreach (GameObject optionObject in m_OptionGameObjects) {
				bool selected = Mathf.RoundToInt(value) == i;

				// Save as current
				if (selected)
					selectedOptionGameObject = optionObject;

				// Image
				Image image = optionObject.GetComponent<Image>();
				if (image != null) {
					image.sprite = m_OptionSprite;
					image.rectTransform.pivot = new Vector2(0f, 1f);

					if (m_OptionTransition == OptionTransition.ColorTint &&
					    m_OptionTransitionTarget == TransitionTarget.Image)
						image.canvasRenderer.SetColor(selected
							? m_OptionTransitionColorActive
							: m_OptionTransitionColorNormal);
					else
						image.canvasRenderer.SetColor(Color.white);
				}

				// Text
				Text text = optionObject.GetComponentInChildren<Text>();
				if (text != null) {
					// Update the text
					text.font = m_OptionTextFont;
					text.fontStyle = m_OptionTextStyle;
					text.fontSize = m_OptionTextSize;
					text.color = m_OptionTextColor;

					if (m_OptionTransition == OptionTransition.ColorTint &&
					    m_OptionTransitionTarget == TransitionTarget.Text)
						text.canvasRenderer.SetColor(selected
							? m_OptionTransitionColorActive
							: m_OptionTransitionColorNormal);
					else
						text.canvasRenderer.SetColor(Color.white);

					// Update the text offset
					(text.transform as RectTransform).anchoredPosition = m_OptionTextOffset;

					// Update the text effects
					UpdateTextEffect(text.gameObject);
				}

				// Increase the indexer
				i++;
			}
		}

		/// <summary>
		///     Rebuilds the options.
		/// </summary>
		protected void RebuildOptions() {
			if (!HasOptions())
				return;

			// Make sure we have the options container
			if (m_OptionsContGameObject == null)
				CreateOptionsContainer();

			// Clear out the current options
			DestroyOptions();

			// Loop through the options	
			int i = 0;
			foreach (string option in m_Options) {
				GameObject optionObject = new GameObject("Option " + i, typeof(RectTransform), typeof(Image));
				optionObject.layer = gameObject.layer;
				optionObject.transform.SetParent(m_OptionsContGameObject.transform, false);
				optionObject.transform.localScale = Vector3.one;
				optionObject.transform.localPosition = Vector3.zero;

				// Create the text game object
				GameObject textObject = new GameObject("Text", typeof(RectTransform));
				textObject.layer = gameObject.layer;
				textObject.transform.SetParent(optionObject.transform, false);
				textObject.transform.localScale = Vector3.one;
				textObject.transform.localPosition = Vector3.zero;

				// Add the text component and set the text
				Text text = textObject.AddComponent<Text>();
				text.text = option;

				// Add content size fitter
				ContentSizeFitter fitter = textObject.AddComponent<ContentSizeFitter>();
				fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
				fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

				// Add to the game objects list
				m_OptionGameObjects.Add(optionObject);

				// Increase the indexer
				i++;
			}

			// Update the option properties
			UpdateOptionsProperties();
		}

		/// <summary>
		///     Adds the option text effect.
		/// </summary>
		/// <param name="gObject">G object.</param>
		private void AddTextEffect(GameObject gObject) {
			// Add new text effect
			if (m_OptionTextEffect == TextEffectType.Shadow) {
				Shadow s = gObject.AddComponent<Shadow>();
				s.effectColor = m_OptionTextEffectColor;
				s.effectDistance = m_OptionTextEffectDistance;
				s.useGraphicAlpha = m_OptionTextEffectUseGraphicAlpha;
			} else if (m_OptionTextEffect == TextEffectType.Outline) {
				Outline o = gObject.AddComponent<Outline>();
				o.effectColor = m_OptionTextEffectColor;
				o.effectDistance = m_OptionTextEffectDistance;
				o.useGraphicAlpha = m_OptionTextEffectUseGraphicAlpha;
			}
		}

		/// <summary>
		///     Updates the option text effect.
		/// </summary>
		/// <param name="gObject">G object.</param>
		private void UpdateTextEffect(GameObject gObject) {
			// Update text text effect
			if (m_OptionTextEffect == TextEffectType.Shadow) {
				Shadow s = gObject.GetComponent<Shadow>();
				if (s == null) s = gObject.AddComponent<Shadow>();
				s.effectColor = m_OptionTextEffectColor;
				s.effectDistance = m_OptionTextEffectDistance;
				s.useGraphicAlpha = m_OptionTextEffectUseGraphicAlpha;
			} else if (m_OptionTextEffect == TextEffectType.Outline) {
				Outline o = gObject.GetComponent<Outline>();
				if (o == null) o = gObject.AddComponent<Outline>();
				o.effectColor = m_OptionTextEffectColor;
				o.effectDistance = m_OptionTextEffectDistance;
				o.useGraphicAlpha = m_OptionTextEffectUseGraphicAlpha;
			}
		}

		/// <summary>
		///     Rebuilds the options text effects.
		/// </summary>
		public void RebuildTextEffects() {
			// Loop through the options
			foreach (GameObject optionObject in m_OptionGameObjects) {
				Text text = optionObject.GetComponentInChildren<Text>();

				if (text != null) {
					Shadow s = text.gameObject.GetComponent<Shadow>();
					Outline o = text.gameObject.GetComponent<Outline>();

					// Destroy any effect we find
					if (Application.isPlaying) {
						if (s != null) Destroy(s);
						if (o != null) Destroy(o);
					} else {
						if (s != null) DestroyImmediate(s);
						if (o != null) DestroyImmediate(o);
					}

					// Re-add the effect
					AddTextEffect(text.gameObject);
				}
			}
		}

		/// <summary>
		///     Destroies the current options.
		/// </summary>
		protected void DestroyOptions() {
			// Clear out the optins
			foreach (GameObject g in m_OptionGameObjects)
				if (Application.isPlaying) Destroy(g);
				else DestroyImmediate(g);

			// Clear the list
			m_OptionGameObjects.Clear();
		}

		/// <summary>
		///     Creates the options container.
		/// </summary>
		protected void CreateOptionsContainer() {
			// Create new game object
			m_OptionsContGameObject = new GameObject("Options Grid", typeof(RectTransform), typeof(GridLayoutGroup));
			m_OptionsContGameObject.layer = gameObject.layer;
			m_OptionsContGameObject.transform.SetParent(transform, false);
			m_OptionsContGameObject.transform.SetAsFirstSibling();
			m_OptionsContGameObject.transform.localScale = Vector3.one;
			m_OptionsContGameObject.transform.localPosition = Vector3.zero;

			// Get the rect transform
			m_OptionsContRect = m_OptionsContGameObject.GetComponent<RectTransform>();
			m_OptionsContRect.sizeDelta = new Vector2(0f, 0f);
			m_OptionsContRect.anchorMin = new Vector2(0f, 0f);
			m_OptionsContRect.anchorMax = new Vector2(1f, 1f);
			m_OptionsContRect.pivot = new Vector2(0f, 1f);
			m_OptionsContRect.anchoredPosition = new Vector2(0f, 0f);

			// Get the grid layout group
			m_OptionsContGrid = m_OptionsContGameObject.GetComponent<GridLayoutGroup>();
		}

#pragma warning disable 0649
		[SerializeField] private List<string> m_Options = new List<string>();
		[SerializeField] private List<GameObject> m_OptionGameObjects = new List<GameObject>();
		[SerializeField] private GameObject m_OptionsContGameObject;
		[SerializeField] private RectTransform m_OptionsContRect;
		[SerializeField] private GridLayoutGroup m_OptionsContGrid;

		[SerializeField] private RectOffset m_OptionsPadding = new RectOffset();
		[SerializeField] private Sprite m_OptionSprite;
		[SerializeField] private Font m_OptionTextFont;
		[SerializeField] private FontStyle m_OptionTextStyle = FontData.defaultFontData.fontStyle;
		[SerializeField] private int m_OptionTextSize = FontData.defaultFontData.fontSize;
		[SerializeField] private Color m_OptionTextColor = Color.white;
		[SerializeField] private TextEffectType m_OptionTextEffect = TextEffectType.None;
		[SerializeField] private Color m_OptionTextEffectColor = new Color(0f, 0f, 0f, 128f);
		[SerializeField] private Vector2 m_OptionTextEffectDistance = new Vector2(1f, -1f);
		[SerializeField] private bool m_OptionTextEffectUseGraphicAlpha = true;
		[SerializeField] private Vector2 m_OptionTextOffset = Vector2.zero;
		[SerializeField] private OptionTransition m_OptionTransition = OptionTransition.None;
		[SerializeField] private TransitionTarget m_OptionTransitionTarget = TransitionTarget.Text;
		[SerializeField] private Color m_OptionTransitionColorNormal = ColorBlock.defaultColorBlock.normalColor;
		[SerializeField] private Color m_OptionTransitionColorActive = ColorBlock.defaultColorBlock.highlightedColor;
		[SerializeField] [Range(1f, 6f)] private float m_OptionTransitionMultiplier = 1f;
		[SerializeField] private float m_OptionTransitionDuration = 0.1f;
#pragma warning restore 0649

	}
}