namespace Asgla.Avatar.Monster {
	public class MonsterRoot : AvatarRoot {

		#region Unity

		protected override void OnMouseEnter() {
			Main.Singleton.Walkable = false;
		}

		protected override void OnMouseExit() {
			Main.Singleton.Walkable = true;
		}

		#endregion

	}
}