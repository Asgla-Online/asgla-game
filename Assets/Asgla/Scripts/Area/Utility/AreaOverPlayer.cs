using UnityEngine;

namespace Asgla.Area.Utility {

	/*
	 * MapOverPlayer
	 * Update map object Z position
	 * Order in scene.
	 */

	[ExecuteInEditMode]
	public class AreaOverPlayer : MonoBehaviour {

#if UNITY_EDITOR
		private void Update() {
			//Debug.LogFormat(gameObject.name);
			//Debug.Log(transform.localPosition);
			//Debug.Log(transform.position);
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
		}
#endif

	}

}