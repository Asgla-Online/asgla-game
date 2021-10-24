using System;
using System.Globalization;
using UnityEngine;

namespace Asgla.Utility {
	public static class CommonColorBuffer {

		public static string ColorToString(Color32 color) {
			return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
		}

		public static Color32 StringToColor(string colorString) {
			int num = int.Parse(colorString, NumberStyles.HexNumber);

			Color32 result;

			switch (colorString.Length) {
				case 8:
					result = new Color32((byte) ((num >> 24) & 255), (byte) ((num >> 16) & 255),
						(byte) ((num >> 8) & 255),
						(byte) (num & 255));
					break;
				case 6:
					result = new Color32((byte) ((num >> 16) & 255), (byte) ((num >> 8) & 255), (byte) (num & 255),
						255);
					break;
				case 4:
					result = new Color32((byte) (((num >> 12) & 15) * 17), (byte) (((num >> 8) & 15) * 17),
						(byte) (((num >> 4) & 15) * 17), (byte) ((num & 15) * 17));
					break;
				default: {
					if (colorString.Length != 3)
						throw new FormatException("Support only RRGGBBAA, RRGGBB, RGBA, RGB formats");
					result = new Color32((byte) (((num >> 8) & 15) * 17), (byte) (((num >> 4) & 15) * 17),
						(byte) ((num & 15) * 17), 255);
					break;
				}
			}

			return result;
		}

	}
}