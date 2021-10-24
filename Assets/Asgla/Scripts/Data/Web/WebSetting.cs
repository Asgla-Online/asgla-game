using System.Collections.Generic;

namespace Asgla.Data.Web {
	public class WebSetting {

		public class LoginWebRequest {

			public string Message;

			public List<Server> servers;
			public User user;

		}

		public class Server {

			public int Count;
			public int HighestCount;
			public string IP;
			public int Max;
			public string Name;

			public int Port;

		}

		public class User {

			public string Token;
			public string Username;

		}

	}
}