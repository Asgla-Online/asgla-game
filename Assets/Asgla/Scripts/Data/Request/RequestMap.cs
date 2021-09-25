using Asgla.Data.Map;
using Asgla.Data.Player;

namespace Asgla.Data.Request {
    public class RequestMap {

        public class PlayerUpdate {
            public PlayerData Player;
        }

        public class MoveToArea {
            public int PlayerID;

            public string Area;
            public string Position;
        }

        public class JoinRoom {
            public MapData Map;
        }

        public class LeaveRoomRequest {
            public int PlayerID;
        }

        public class MoveRequest {
            public RequestAvatar.Entity Entity = null;

            public double x;
            public double y;
        }

    }
}
