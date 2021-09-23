using Asgla.Avatar;
using UnityEngine;

namespace Asgla.Effect {
    public class EffectFollow : MonoBehaviour {

        private IAvatar _target;

        public void Target(IAvatar target) => _target = target;

        private void FixedUpdate() {
            transform.LookAt(_target.Avatar().transform);

            Vector2 pos = new Vector2(_target.Position().x, _target.Position().y + 1.3f);

            transform.position = Vector3.MoveTowards(transform.position, pos, 10 * Time.fixedDeltaTime);

            //Debug.LogFormat("EffectFollow {0} - {1}, {2}", _target.Id(), transform.position, _target.Avatar().transform.position);

            if ((Vector2) transform.position == pos) {
                //Debug.Log("EffectFollow Destroy");
                Destroy(gameObject);
            }
        }

    }
}
