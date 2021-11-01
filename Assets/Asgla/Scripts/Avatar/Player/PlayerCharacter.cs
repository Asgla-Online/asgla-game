using Asgla.Utility;
using UnityEngine;

namespace Asgla.Avatar.Player {
	public class PlayerCharacter : AvatarRoot {

		#region Unity

		protected override void OnMouseEnter() {
			base.OnMouseEnter();
			((Player) _avatar).CharacterView().TintColor = CommonColorBuffer.StringToColor("ECECEC");
		}

		protected override void OnMouseExit() {
			base.OnMouseExit();
			((Player) _avatar).CharacterView().TintColor = Color.white;
		}

		#endregion

	}
}