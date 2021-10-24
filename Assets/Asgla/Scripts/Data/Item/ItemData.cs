using Asgla.Data.Type;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Asgla.Data.Item {

    [Serializable]
    public class ItemData {

        public int databaseId = -1;

        public string name;
        public string description;

        public TypeItemData Type = null;

        public string bundle;
        public string asset;

        public RarityData rarity = RarityData.Common;

        public int requiredLevel = -1;

        //public Part Part;

        public Sprite GetIcon => Resources.Load<Sprite>("Sprites/Icon/" + Type.Icon);
        
    }

}
