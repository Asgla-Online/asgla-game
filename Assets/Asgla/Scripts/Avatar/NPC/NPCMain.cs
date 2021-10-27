using Asgla.Data.Area;
using UnityEngine;

namespace Asgla.Avatar.NPC {
	public class NpcMain : MonoBehaviour {

		[SerializeField] private int id;

		[SerializeField] private NpcAvatar avatar;

		private AreaLocalNpc _data;

		#region Unity

		private void Start() {
			avatar.Npc(this);
		}

		#endregion

		public void Data(AreaLocalNpc data) {
			_data = data;
		}

		public void Position(Vector2 vector2) {
			transform.position = vector2;
		}

		public int AreaId() {
			return id;
			// _data.databaseId;
		}

		public int NpcId() {
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