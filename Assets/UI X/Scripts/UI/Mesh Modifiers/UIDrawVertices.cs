using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[ExecuteInEditMode]
	public class UIDrawVertices : MonoBehaviour, IMeshModifier {

		[SerializeField] private Color color = Color.green;
		private Mesh mesh;

		public void OnDrawGizmos() {
			if (mesh == null) return;

			Gizmos.color = color;
			Gizmos.DrawWireMesh(mesh, transform.position);
		}

#if UNITY_EDITOR
		protected void OnValidate() {
			GetComponent<Graphic>().SetVerticesDirty();
		}
#endif

		public void ModifyMesh(VertexHelper vertexHelper) {
			if (mesh == null) mesh = new Mesh();

			vertexHelper.FillMesh(mesh);
		}

		public void ModifyMesh(Mesh mesh) {
		}

	}
}