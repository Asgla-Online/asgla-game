using System;
using System.Collections.Generic;
using UnityEngine;

namespace AsglaUI.UI {
	public class UITooltipLines {

		public enum LineStyle {

			Default,
			Title,
			Description,
			Custom

		}

		/// <summary>
		///     The lines list.
		/// </summary>
		public Lines lineList = new Lines();

		/// <summary>
		///     Adds a line to the list.
		/// </summary>
		/// <param name="leftContent">Left content.</param>
		/// <param name="rightContent">Right content.</param>
		public void AddLine(string leftContent, string rightContent) {
			lineList.Add(new Line(leftContent, rightContent, true, new RectOffset(), LineStyle.Default, ""));
		}

		/// <summary>
		///     Adds a line to the list.
		/// </summary>
		/// <param name="leftContent">Left content.</param>
		/// <param name="rightContent">Right content.</param>
		/// <param name="padding">Row padding.</param>
		public void AddLine(string leftContent, string rightContent, RectOffset padding) {
			lineList.Add(new Line(leftContent, rightContent, true, padding, LineStyle.Default, ""));
		}

		/// <summary>
		///     Adds a single column line to the list.
		/// </summary>
		/// <param name="content">Content.</param>
		public void AddLine(string content) {
			lineList.Add(new Line(content, string.Empty, true, new RectOffset(), LineStyle.Default, ""));
		}

		/// <summary>
		///     Adds a single column line to the list.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="padding">Row padding.</param>
		public void AddLine(string content, RectOffset padding) {
			lineList.Add(new Line(content, string.Empty, true, padding, LineStyle.Default, ""));
		}

		/// <summary>
		///     Adds a single column line to the list.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="padding">Row padding.</param>
		public void AddLine(string content, RectOffset padding, LineStyle style) {
			lineList.Add(new Line(content, string.Empty, true, padding, style, ""));
		}

		/// <summary>
		///     Adds a single column line to the list.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="padding">Row padding.</param>
		public void AddLine(string content, RectOffset padding, string customStyle) {
			lineList.Add(new Line(content, string.Empty, true, padding, LineStyle.Custom, customStyle));
		}

		/// <summary>
		///     Adds a line to the list.
		/// </summary>
		/// <param name="leftContent">Left content.</param>
		/// <param name="rightContent">Right content.</param>
		/// <param name="padding">Row padding.</param>
		/// <param name="style">Style.</param>
		public void AddLine(string leftContent, string rightContent, RectOffset padding, LineStyle style) {
			// Add the line to the list
			lineList.Add(new Line(leftContent, rightContent, true, padding, style, ""));
		}

		/// <summary>
		///     Adds a line to the list.
		/// </summary>
		/// <param name="leftContent">Left content.</param>
		/// <param name="rightContent">Right content.</param>
		/// <param name="padding">Row padding.</param>
		/// <param name="customStyle">Custom style name.</param>
		public void AddLine(string leftContent, string rightContent, RectOffset padding, string customStyle) {
			// Add the line to the list
			lineList.Add(new Line(leftContent, rightContent, true, padding, LineStyle.Custom, customStyle));
		}

		/// <summary>
		///     Adds a column to the last line or to a new one.
		/// </summary>
		/// <param name="content">Content.</param>
		public void AddColumn(string content) {
			AddColumn(content, LineStyle.Default, "");
		}

		/// <summary>
		///     Adds a column to the last line or to a new one.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="style">The line style.</param>
		public void AddColumn(string content, LineStyle style) {
			AddColumn(content, style, "");
		}

		/// <summary>
		///     Adds a column to the last line or to a new one.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="customStyle">The custom line style name.</param>
		public void AddColumn(string content, string customStyle) {
			AddColumn(content, LineStyle.Custom, customStyle);
		}

		/// <summary>
		///     Adds a column to the last line or to a new one.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="style">The line style.</param>
		/// <param name="customStyle">The custom line style name.</param>
		public void AddColumn(string content, LineStyle style, string customStyle) {
			// Check if the rows list is empty
			if (lineList.Count == 0) {
				// Add the a new row to the list
				lineList.Add(new Line(content, string.Empty, false, new RectOffset(), style, customStyle));
			} else {
				// Find the last row
				Line line = lineList[lineList.Count - 1];

				// Check if the rows is not marked as complete
				if (!line.isComplete) {
					// Add it to the line
					line.right = content;

					// Check if it's complete now
					line.isComplete = true;
				} else {
					// Add the a new line to the list
					lineList.Add(new Line(content, string.Empty, false, new RectOffset(), style, customStyle));
				}
			}
		}

		[Serializable]
		public class Line {

			public string left;
			public string right;
			public bool isComplete;
			public RectOffset padding;
			public LineStyle style;
			public string customStyle;

			public Line(string left,
				string right,
				bool isComplete,
				RectOffset padding,
				LineStyle style,
				string customStyle) {
				this.left = left;
				this.right = right;
				this.isComplete = isComplete;
				this.padding = padding;
				this.style = style;
				this.customStyle = customStyle;
			}

		}

		public class Lines : List<Line> {

		}

	}
}