using Asgla.Data.Area;
using UnityEngine;

namespace Asgla.NPC {
	public class NPCMain : MonoBehaviour {

		[SerializeField] private int _id;

		[SerializeField] private NPCAvatar _avatar;

		private AreaLocalNpc _data;

		#region Unity

		private void Start() {
			_avatar.NPC(this);
		}

		#endregion

		public void Data(AreaLocalNpc data) {
			_data = data;
		}

		public void Position(Vector2 vector2) {
			transform.position = vector2;
		}

		public int AreaId() {
			return _id;
			// _data.databaseId;
		}

		public int NPCId() {
			return _data.npc.DatabaseID;
		}

		public string Name() {
			return _data.npc.Name;
		}

		public string Description() {
			return _data.npc.Description;
		}

		public int ShopId() {
			return _data.npc.ShopID;
		}

		public int QuestId() {
			return _data.npc.QuestID;
		}

		public Vector2 Position() {
			return transform.position;
		}

		public GameObject Avatar() {
			return gameObject;
		}

		public AreaLocalNpc Data() {
			return _data;
		}

	}
}