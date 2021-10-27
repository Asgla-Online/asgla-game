using UnityEngine;

namespace AsglaUI.UI {
	public class ColorScheme : ScriptableObject {

		[Header("Image Colors")] [SerializeField]
		private Color m_ImagePrimary = Color.white;

		[SerializeField] private Color m_ImageSecondary = Color.white;
		[SerializeField] private Color m_ImageLight = Color.white;
		[SerializeField] private Color m_ImageDark = Color.white;
		[SerializeField] private Color m_ImageEffect = Color.white;
		[SerializeField] private Color m_ImageEffect2 = Color.white;
		[SerializeField] private Color m_ImageEffect3 = Color.white;
		[SerializeField] private Color m_ImageBordersPrimary = Color.white;
		[HideInInspector] [SerializeField] private Color m_ImageBordersSecondary = Color.white;

		[Header("Button Colors")] [SerializeField]
		private Color m_ButtonForeground = Color.white;

		[SerializeField] private Color m_ButtonEffect = Color.white;

		[Header("Window Colors")] [SerializeField]
		private Color m_WindowHeader = Color.white;

		[Header("Animation Colors")] [SerializeField]
		private Color m_Animation = Color.white;

		/// <summary>
		///     Gets or sets the Primary image color shade.
		/// </summary>
		public Color imagePrimary {
			get => m_ImagePrimary;
			set => m_ImagePrimary = value;
		}

		/// <summary>
		///     Gets or sets the Secondary image color shade.
		/// </summary>
		public Color imageSecondary {
			get => m_ImageSecondary;
			set => m_ImageSecondary = value;
		}

		/// <summary>
		///     Gets or sets the light image color shade.
		/// </summary>
		public Color imageLight {
			get => m_ImageLight;
			set => m_ImageLight = value;
		}

		/// <summary>
		///     Gets or sets the dark image color shade.
		/// </summary>
		public Color imageDark {
			get => m_ImageDark;
			set => m_ImageDark = value;
		}

		/// <summary>
		///     Gets or sets the Effect image color shade.
		/// </summary>
		public Color imageEffect {
			get => m_ImageEffect;
			set => m_ImageEffect = value;
		}

		/// <summary>
		///     Gets or sets the Effect 2 image color shade.
		/// </summary>
		public Color imageEffect2 {
			get => m_ImageEffect2;
			set => m_ImageEffect2 = value;
		}

		/// <summary>
		///     Gets or sets the Effect 3 image color shade.
		/// </summary>
		public Color imageEffect3 {
			get => m_ImageEffect3;
			set => m_ImageEffect3 = value;
		}

		/// <summary>
		///     Gets or sets the Primary Borders image color shade.
		/// </summary>
		public Color imageBordersPrimary {
			get => m_ImageBordersPrimary;
			set => m_ImageBordersPrimary = value;
		}

		/// <summary>
		///     Gets or sets the Secondary Borders image color shade.
		/// </summary>
		public Color imageBordersSecondary {
			get => m_ImageBordersSecondary;
			set => m_ImageBordersSecondary = value;
		}

		/// <summary>
		///     Gets or sets the button foreground color.
		/// </summary>
		public Color buttonForeground {
			get => m_ButtonForeground;
			set => m_ButtonForeground = value;
		}

		/// <summary>
		///     Gets or sets the button effect color.
		/// </summary>
		public Color buttonEffect {
			get => m_ButtonEffect;
			set => m_ButtonEffect = value;
		}

		/// <summary>
		///     Gets or sets the window header color.
		/// </summary>
		public Color windowHeader {
			get => m_WindowHeader;
			set => m_WindowHeader = value;
		}

		/// <summary>
		///     Gets or sets the animation color.
		/// </summary>
		public Color animation {
			get => m_Animation;
			set => m_Animation = value;
		}

		/// <summary>
		///     Applies the color scheme.
		/// </summary>
		public void ApplyColorScheme() {
			// Get all the color scheme elements in the scene
			ColorSchemeElement[] elements = FindObjectsOfType<ColorSchemeElement>();

			foreach (ColorSchemeElement element in elements)
				ApplyToElement(element);

			ColorSchemeElement_SelectField[] selectElements = FindObjectsOfType<ColorSchemeElement_SelectField>();

			foreach (ColorSchemeElement_SelectField element in selectElements)
				ApplyToElement(element);

			// Set the color scheme as active
			if (ColorSchemeManager.Instance != null)
				ColorSchemeManager.Instance.activeColorScheme = this;
		}

		/// <summary>
		///     Gets a color shade.
		/// </summary>
		/// <param name="shade">The shade.</param>
		/// <returns>The color.</returns>
		public Color GetColorShade(ColorSchemeShade shade) {
			Color newColor = Color.white;

			switch (shade) {
				case ColorSchemeShade.Primary:
					newColor = m_ImagePrimary;
					break;
				case ColorSchemeShade.Secondary:
					newColor = m_ImageSecondary;
					break;
				case ColorSchemeShade.Light:
					newColor = m_ImageLight;
					break;
				case ColorSchemeShade.Dark:
					newColor = m_ImageDark;
					break;
				case ColorSchemeShade.Effect:
					newColor = m_ImageEffect;
					break;
				case ColorSchemeShade.Effect2:
					newColor = m_ImageEffect2;
					break;
				case ColorSchemeShade.Effect3:
					newColor = m_ImageEffect3;
					break;
				case ColorSchemeShade.BordersPrimary:
					newColor = m_ImageBordersPrimary;
					break;
				case ColorSchemeShade.BordersSecondary:
					newColor = m_ImageBordersSecondary;
					break;
				case ColorSchemeShade.ButtonPrimary:
					newColor = m_ButtonForeground;
					break;
				case ColorSchemeShade.ButtonSecondary:
					newColor = m_ButtonEffect;
					break;
				case ColorSchemeShade.WindowHeader:
					newColor = m_WindowHeader;
					break;
				case ColorSchemeShade.Animation:
					newColor = m_Animation;
					break;
			}

			return newColor;
		}

		/// <summary>
		///     Applies the color scheme to the specified element.
		/// </summary>
		/// <param name="element"></param>
		public void ApplyToElement(IColorSchemeElement element) {
			if (element == null)
				return;

			// Get the color
			Color newColor = GetColorShade(element.shade);

			// Apply
			element.Apply(newColor);
		}

	}
}