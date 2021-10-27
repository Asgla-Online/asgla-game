using System;

namespace AsglaUI.UI {
	[Serializable]
	public class UITooltipLineContent {

		public UITooltipLines.LineStyle LineStyle;
		public string CustomLineStyle;
		public string Content;
		public bool IsSpacer;

		public UITooltipLineContent() {
			LineStyle = UITooltipLines.LineStyle.Default;
			CustomLineStyle = string.Empty;
			Content = string.Empty;
			IsSpacer = false;
		}

	}
}