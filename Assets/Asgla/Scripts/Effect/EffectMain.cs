using Asgla.Avatar;
using Asgla.Data.Effect;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Asgla.Effect {
    public class EffectMain : MonoBehaviour {

        private static EffectMain _singleton;

        public static EffectMain Singleton => _singleton;

        [SerializeField] private List<EffectData> _effect = new List<EffectData>();

        public void AddAsset(EffectData e) => _effect.Add(e);

        public EffectData GetByName(string name) => _effect.Where(v => v.Name == name).First();

        public void SetAsset(EffectMain data) => _singleton = data;

        public IEnumerator SpawmAsset(GameObject asset, AvatarMain from, AvatarMain target) {
            Renderer renderer = asset.GetComponent<Renderer>();
            ParticleSystem ps = asset.GetComponent<ParticleSystem>();

            float destroy = ps.main.duration + 0.2f;
            bool loop = ps.main.loop;

            if (renderer.sortingLayerName == "Default") {
                renderer.sortingLayerName = "Main";
                renderer.sortingOrder = 3;
            }

            asset.transform.localScale = new Vector3(1, 1, 1);

            Vector2 spawn = loop ? new Vector2(from.Position().x, from.Position().y + 1.3f) : new Vector2(target.Position().x, target.Position().y + 1.3f);

            GameObject effect = Instantiate(asset, spawn, Quaternion.identity, target.Utility().transform);

            if (loop) {
                effect.AddComponent<EffectFollow>().Target(target);
            } else {
                Destroy(effect, destroy);
            }

            yield break;
        }

    }
}
