using Asgla.Data.Skill;
using Asgla.Utility;
using TMPro;
using UnityEngine;

//TODO: namespace

namespace Asgla.UI {
	public class PopupText : MonoBehaviour {

		private const float DISAPPEAR_TIMER_MAX = 1f;

		private float disappearTimer;

		private Vector3 moveVector;

		private Color textColor;

		private TextMeshPro textMesh;

		private void Awake() {
			textMesh = transform.GetComponent<TextMeshPro>();
		}

		private void FixedUpdate() {
			transform.position += moveVector * Time.deltaTime;
			moveVector -= moveVector * 8f * Time.deltaTime;

			if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f) {
				// First half of the popup lifetime
				float increaseScaleAmount = 1f;
				transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
			} else {
				// Second half of the popup lifetime
				float decreaseScaleAmount = 1f;
				transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
			}

			disappearTimer -= Time.deltaTime;
			if (disappearTimer < 0) {
				// Start disappearing
				float disappearSpeed = 3f;
				textColor.a -= disappearSpeed * Time.deltaTime;
				textMesh.color = textColor;
				if (textColor.a < 0)
					Destroy(gameObject);
			}
		}

		public void Setup(string damage, SkillDamageType type, int sortingOrder) {
			textMesh.text = damage;

			switch (type) {
				case SkillDamageType.CRIT:
					textMesh.fontSize = 4;
					textColor = CommonColorBuffer.StringToColor("ff3d3d");

					moveVector = new Vector3(.7f, 6f); //Random.Range(20f, 25f);
					break;
				case SkillDamageType.DODGE:
					textMesh.fontSize = 2;
					textColor = CommonColorBuffer.StringToColor("ff913d");

					moveVector = new Vector3(.7f, 6f);
					break;
				case SkillDamageType.HIT:
					textMesh.fontSize = 2;
					textColor = CommonColorBuffer.StringToColor("473dff");

					moveVector = new Vector3(.7f, 6f);
					break;
				case SkillDamageType.MISS:
					textMesh.fontSize = 3;
					textColor = CommonColorBuffer.StringToColor("ff3d9b");

					moveVector = new Vector3(.7f, 6f);
					break;
			}

			textMesh.color = textColor;
			disappearTimer = DISAPPEAR_TIMER_MAX;

			textMesh.sortingOrder = sortingOrder;
		}

	}
}