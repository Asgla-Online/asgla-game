using UnityEngine;

namespace CharacterCreator2D {
	public class BodySliderAttribute : PropertyAttribute {

		public float maxVal;
		public float minVal;
		public bool symmetrical;

		public BodySliderAttribute(float minValue, float maxValue, bool symmetrical) {
			this.minVal = minValue;
			this.maxVal = maxValue;
			this.symmetrical = symmetrical;
		}

	}
}