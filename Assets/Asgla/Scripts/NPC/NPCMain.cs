using Asgla.Data.Map;
using UnityEngine;

namespace Asgla.NPC {
	public class NPCMain : MonoBehaviour {

		[SerializeField] private int _id;

		[SerializeField] private NPCAvatar _avatar;

		private MapAreaNPC _data;

		#region Unity

		private void Start() {
			_avatar.NPC(this);
		}

		#endregion

		public void Data(MapAreaNPC data) {
			_data = data;
		}

		public void Position(Vector2 vector2) {
			transform.position = vector2;
		}

		public int AreaId() {
			return _id;
			// _data.DatabaseID;
		}

		public int NPCId() {
			return _data.NPC.DatabaseID;
		}

		public string Name() {
			return _data.NPC.Name;
		}

		public string Description() {
			return _data.NPC.Description;
		}

		public int ShopId() {
			return _data.NPC.ShopID;
		}

		public int QuestId() {
			return _data.NPC.QuestID;
		}

		public Vector2 Position() {
			return transform.position;
		}

		public GameObject Avatar() {
			return gameObject;
		}

		public MapAreaNPC Data() {
			return _data;
		}

	}
}