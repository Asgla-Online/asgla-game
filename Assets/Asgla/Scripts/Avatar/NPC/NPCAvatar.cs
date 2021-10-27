using Asgla.Utility;
using UnityEngine;

namespace Asgla.Avatar.NPC {
	public class NpcAvatar : MonoBehaviour {

		private NpcMain _npc;

		private SpriteRenderer _sprite;

		public void Npc(NpcMain npc) {
			_npc = npc;
		}

		#region Unity

		private void Awake() {
			_sprite = GetComponent<SpriteRenderer>(); //ye.. ik but fk
		}

		private void OnMouseDown() {
			_sprite.color = CommonColorBuffer.StringToColor("D9D9D9");
			Main.Singleton.Game.WindowNPC.Init(_npc);
		}

		private void OnMouseEnter() {
			Main.Singleton.Walkable = false;
			_sprite.color = CommonColorBuffer.StringToColor("ECECEC");
		}

		private void OnMouseExit() {
			Main.Singleton.Walkable = true;
			_sprite.color = Color.white;
		}

		private void OnMouseUpAsButton() {
			_sprite.color = Color.white;
		}

		#endregion

	}
}