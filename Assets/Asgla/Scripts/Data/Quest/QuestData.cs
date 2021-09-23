using Asgla.Data.Item;
using System;
using System.Collections.Generic;

namespace Asgla.Data.Quest {

    [Serializable]
    public class QuestData {
        public int DatabaseID;

        public string Name;
        public string Description;

        public int Experience;
        public int Gold;

        public List<Reward> Reward;
        public List<Requirement> Requirement;
    }

    [Serializable]
    public class Reward {
        public int DatabaseID;

        public ItemData Item;
    }

    [Serializable]
    public class Requirement {
        public int DatabaseID;
        public int Quantity;

        public ItemData Item;
    }

}
