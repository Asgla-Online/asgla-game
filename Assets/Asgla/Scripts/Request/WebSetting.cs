using System;
using System.Collections.Generic;

namespace Asgla.Request {
    public class WebSetting {

        [Serializable]
        public class LoginWebRequest {
            public string Message;
            public List<Server> servers;
            public User user;
        }

        [Serializable]
        public class Server {
            public string Name;
            public string IP;
            public int Port;
            public int Count;
            public int HighestCount;
            public int Max;
        }

        [Serializable]
        public class User {
            public string Username;
            public string Token;
        }

    }
}
