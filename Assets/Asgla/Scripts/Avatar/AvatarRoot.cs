
using Asgla;
using Asgla.Avatar;
using Asgla.Avatar.Monster;
using Asgla.Avatar.Player;
using UnityEngine;

namespace Assets.Asgla.Scripts.Avatar {

    public abstract class AvatarRoot : MonoBehaviour {

        protected AvatarMain _avatar;

        private bool one_click = false;

        private float time_for_double_click;

        #region Unity
        private void OnMouseDown() {
            float delay = 0.25f;

            if (!one_click) {
                time_for_double_click = Time.time;
                one_click = true;
            } else if ((Time.time - time_for_double_click) > delay) {
                time_for_double_click = Time.time;
            } else {
                _avatar.Unselect();
                one_click = false;
                return;
            }

            Main.Singleton.AvatarManager.SelectTarget(_avatar);

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
            Main.Singleton.Walkable = false;
            //_monster.CharacterView().TintColor = CommonColorBuffer.StringToColor("ECECEC");
        }

        protected virtual void OnMouseExit() {
            Main.Singleton.Walkable = true;
            //_monster.CharacterView().TintColor = Color.white;
        }
        #endregion

        public void Avatar(AvatarMain avatar) => _avatar = avatar;

    }

}
