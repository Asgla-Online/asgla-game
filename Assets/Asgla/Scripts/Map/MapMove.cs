using Asgla.Avatar.Player;
using Asgla.Controller;
using Asgla.Data.Area;
using UnityEngine;

namespace Asgla.Map {
	public class MapMove : MonoBehaviour {

		[Header("Empty/Ignore if type is MoveToArea")] [SerializeField]
		private string _join;

		[Header("Enter, Area2, Area3...")] [SerializeField]
		private string _area;

		[Header("Spawn, Left, Right...")] [SerializeField]
		private string _position;

		[Space] [SerializeField] private AreaMoveType _type;

		private void OnTriggerEnter2D(Collider2D collider) {
			if (!collider.CompareTag("Player"))
				return;

			Player player = collider.GetComponent<Player>();

			player.TargetReset();

			switch (_type) {
				case AreaMoveType.MoveToArea:
					if (string.IsNullOrEmpty(_area) || string.IsNullOrEmpty(_position))
						return;

					MapArea area = Main.Singleton.MapManager.Map.AreaByName(_area);

					Main.Singleton.MapManager.UpdatePlayerArea(player, area, _position);

					if (player.Data().isControlling)
						Main.Singleton.Request.Send("MoveToArea", _area, _position);
					break;
				case AreaMoveType.JoinMap:
					if (!player.Data().isControlling || string.IsNullOrEmpty(_join) || string.IsNullOrEmpty(_area) ||
					    string.IsNullOrEmpty(_position))
						return;

					UIController.CreateLoadingMap();
					Main.Singleton.UIManager.LoadingOverlay.SetLoadingText("LOADING MAP");

					Main.Singleton.Request.Send("Join", _join, _area, _position);
					break;
			}
		}

	}
}