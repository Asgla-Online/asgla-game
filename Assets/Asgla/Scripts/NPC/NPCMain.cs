using Asgla.Data.Map;
using Asgla.Data.NPC;
using UnityEngine;

namespace Asgla.NPC {
    public class NPCMain : MonoBehaviour {

        [SerializeField] private int _id;

        [SerializeField] private NPCAvatar _avatar;

        private MapAreaNPC _data;

        #region Unity
        private void Start() {
            _avatar.NPC(this);
        }
        #endregion

        public void Data(MapAreaNPC data) => _data = data;

        public void Position(Vector2 vector2) => transform.position = vector2;

        public int AreaId() => _id;// _data.DatabaseID;

        public int NPCId() => _data.NPC.DatabaseID;

        public string Name() => _data.NPC.Name;

        public string Description() => _data.NPC.Description;

        public int ShopId() => _data.NPC.ShopID;

        public int QuestId() => _data.NPC.QuestID;

        public Vector2 Position() => transform.position;

        public GameObject Avatar() => gameObject;

        public MapAreaNPC Data() => _data;

    }
}
