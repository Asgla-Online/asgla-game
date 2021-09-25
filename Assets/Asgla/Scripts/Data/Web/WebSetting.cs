using System.Collections.Generic;

namespace Asgla.Data.Web {
    public class WebSetting {

        public class LoginWebRequest {
            public string Message;

            public List<Server> servers;
            public User user;
        }

        public class Server {
            public string Name;
            public string IP;

            public int Port;
            public int Count;
            public int HighestCount;
            public int Max;
        }

        public class User {
            public string Username;
            public string Token;
        }

    }
}
