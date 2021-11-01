using UnityEngine;

namespace Asgla.Avatar {
	public abstract class AvatarRoot : MonoBehaviour {

		protected AvatarMain _avatar;

		private bool _oneClick;

		private float _timeForDoubleClick;

		public void Avatar(AvatarMain avatar) {
			_avatar = avatar;
		}

		#region Unity

		private void OnMouseDown() {
			const float delay = 0.25f;

			if (!_oneClick) {
				_timeForDoubleClick = Time.time;
				_oneClick = true;
			} else if (Time.time - _timeForDoubleClick > delay) {
				_timeForDoubleClick = Time.time;
			} else {
				_avatar.Unselect();
				_oneClick = false;
				return;
			}

			Main.Singleton.Game.AvatarController.SelectTarget(_avatar);

			/*switch (_avatar) {
			    case Player player:
			        Main.Singleton.SelectPlayer(player);
			        break;
			    case Monster monster:
			        Main.Singleton.SelectMonster(monster);
			        break;
			}*/
		}

		protected virtual void OnMouseEnter() {
			Main.Singleton.walkable = false;
			//_monster.CharacterView().TintColor = CommonColorBuffer.StringToColor("ECECEC");
		}

		protected virtual void OnMouseExit() {
			Main.Singleton.walkable = true;
			//_monster.CharacterView().TintColor = Color.white;
		}

		#endregion

	}
}