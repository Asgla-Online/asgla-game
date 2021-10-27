using System;
using UnityEngine;

namespace AsglaUI.UI {
	[Serializable]
	public struct SpriteStateExtended {

		//
		// Properties
		//
		[SerializeField] private Sprite m_HighlightedSprite;
		[SerializeField] private Sprite m_PressedSprite;
		[SerializeField] private Sprite m_ActiveSprite;
		[SerializeField] private Sprite m_ActiveHighlightedSprite;
		[SerializeField] private Sprite m_ActivePressedSprite;
		[SerializeField] private Sprite m_DisabledSprite;

		public Sprite highlightedSprite {
			get => m_HighlightedSprite;
			set => m_HighlightedSprite = value;
		}

		public Sprite pressedSprite {
			get => m_PressedSprite;
			set => m_PressedSprite = value;
		}

		public Sprite activeSprite {
			get => m_ActiveSprite;
			set => m_ActiveSprite = value;
		}

		public Sprite activeHighlightedSprite {
			get => m_ActiveHighlightedSprite;
			set => m_ActiveHighlightedSprite = value;
		}

		public Sprite activePressedSprite {
			get => m_ActivePressedSprite;
			set => m_ActivePressedSprite = value;
		}

		public Sprite disabledSprite {
			get => m_DisabledSprite;
			set => m_DisabledSprite = value;
		}

	}
}