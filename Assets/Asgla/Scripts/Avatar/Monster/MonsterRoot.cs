namespace Asgla.Avatar.Monster {
	public class MonsterRoot : AvatarRoot {

		#region Unity

		protected override void OnMouseEnter() {
			Main.Singleton.walkable = false;
		}

		protected override void OnMouseExit() {
			Main.Singleton.walkable = true;
		}

		#endregion

	}
}