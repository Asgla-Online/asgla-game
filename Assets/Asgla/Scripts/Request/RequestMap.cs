using Asgla.Data.Map;
using Asgla.Data.Player;
using System;

using static Asgla.Request.RequestAvatar;

namespace Asgla.Request {
    public class RequestMap {

        [Serializable]
        public class PlayerUpdate {
            public PlayerData Player;
        }

        [Serializable]
        public class MoveToArea {
            public int PlayerID;
            public string Area;
            public string Position;
        }

        [Serializable]
        public class JoinRoom {
            public MapData Map;
        }

        [Serializable]
        public class LeaveRoomRequest {
            public int PlayerID;
        }

        [Serializable]
        public class MoveRequest {
            public Entity Entity = null;

            public double x;
            public double y;
        }

    }
}
