using Asgla.Avatar;
using BestHTTP.JSON.LitJson;
using UnityEngine;
using static Asgla.Data.Request.RequestAvatar;

// ReSharper disable InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global

namespace Asgla.Requests.Unity {
	public class Move : IRequest {
		
		public Entity entity;

		public double x;
		public double y;

		public void onRequest(Main main, string json) {
			Move move = JsonMapper.ToObject<Move>(json);
			
			AvatarMain avatar = move.entity.Avatar;

			if (avatar is null) {
				return;
			}

			avatar.Move(new Vector2((float)move.x, (float)move.y));
		}

	}
}