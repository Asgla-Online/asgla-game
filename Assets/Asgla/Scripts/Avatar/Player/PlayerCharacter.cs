using Asgla.Utility;
using UnityEngine;

namespace Asgla.Avatar.Player {
	public class PlayerCharacter : AvatarRoot {

		#region Unity

		protected override void OnMouseEnter() {
			base.OnMouseEnter();
			(_avatar as Player).CharacterView().TintColor = CommonColorBuffer.StringToColor("ECECEC");
		}

		protected override void OnMouseExit() {
			base.OnMouseExit();
			(_avatar as Player).CharacterView().TintColor = Color.white;
		}

		#endregion

	}
}