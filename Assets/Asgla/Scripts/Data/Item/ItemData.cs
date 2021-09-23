using Asgla.Data.Type;
using System;
using UnityEngine;

namespace Asgla.Data.Item {

    [Serializable]
    public class ItemData {

        public int DatabaseID = -1;

        public string Name = null;
        public string Description = null;

        public TypeItemData Type = null;

        public string Bundle = null;
        public string Asset = null;

        public RarityData Rarity = RarityData.Common;

        public int RequiredLevel = -1;

        //public Part Part;

        public Sprite GetIcon => Resources.Load<Sprite>("Sprites/Icon/" + Type.Icon);
    }

}
