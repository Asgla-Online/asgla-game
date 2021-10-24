using Asgla.Avatar.Player;

namespace Asgla.Requests {
	public interface IRequest {

		void onRequest(Main main, string json);

	}
}