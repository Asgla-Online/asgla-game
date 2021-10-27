using System;
using UnityEngine;

namespace AsglaUI.UI {
	[Serializable]
	public struct ColorBlockExtended {

		//
		// Properties
		//
		[SerializeField] private Color m_NormalColor;
		[SerializeField] private Color m_HighlightedColor;
		[SerializeField] private Color m_PressedColor;
		[SerializeField] private Color m_ActiveColor;
		[SerializeField] private Color m_ActiveHighlightedColor;
		[SerializeField] private Color m_ActivePressedColor;
		[SerializeField] private Color m_DisabledColor;
		[Range(1f, 5f)] [SerializeField] private float m_ColorMultiplier;

		[SerializeField] private float m_FadeDuration;

		//
		// Static Properties
		//
		public static ColorBlockExtended defaultColorBlock =>
			new ColorBlockExtended {
				m_NormalColor = new Color32(128, 128, 128, 128),
				m_HighlightedColor = new Color32(128, 128, 128, 178),
				m_PressedColor = new Color32(88, 88, 88, 178),
				m_ActiveColor = new Color32(128, 128, 128, 128),
				m_ActiveHighlightedColor = new Color32(128, 128, 128, 178),
				m_ActivePressedColor = new Color32(88, 88, 88, 178),
				m_DisabledColor = new Color32(64, 64, 64, 128),
				m_ColorMultiplier = 1f,
				m_FadeDuration = 0.1f
			};

		public Color normalColor {
			get => m_NormalColor;
			set => m_NormalColor = value;
		}

		public Color highlightedColor {
			get => m_HighlightedColor;
			set => m_HighlightedColor = value;
		}

		public Color pressedColor {
			get => m_PressedColor;
			set => m_PressedColor = value;
		}

		public Color disabledColor {
			get => m_DisabledColor;
			set => m_DisabledColor = value;
		}

		public Color activeColor {
			get => m_ActiveColor;
			set => m_ActiveColor = value;
		}

		public Color activeHighlightedColor {
			get => m_ActiveHighlightedColor;
			set => m_ActiveHighlightedColor = value;
		}

		public Color activePressedColor {
			get => m_ActivePressedColor;
			set => m_ActivePressedColor = value;
		}

		public float colorMultiplier {
			get => m_ColorMultiplier;
			set => m_ColorMultiplier = value;
		}

		public float fadeDuration {
			get => m_FadeDuration;
			set => m_FadeDuration = value;
		}

	}
}