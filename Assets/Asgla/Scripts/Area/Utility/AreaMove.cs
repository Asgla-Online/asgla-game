using System;
using Asgla.Avatar.Player;
using Asgla.Controller;
using Asgla.Data.Area;
using UnityEngine;

namespace Asgla.Area.Utility {
	public class AreaMove : MonoBehaviour {

		[Header("Empty/Ignore if type is MoveToArea")] [SerializeField]
		private string join;

		[Header("Enter, Area2, Area3...")] [SerializeField]
		private string area;

		[Header("Spawn, Left, Right...")] [SerializeField]
		private string position;

		[Space] [SerializeField] private AreaMoveType type;

		private void OnTriggerEnter2D(Collider2D trigger) {
			if (!trigger.CompareTag("Player"))
				return;

			Player player = trigger.GetComponent<Player>();

			player.TargetReset();

			switch (type) {
				case AreaMoveType.MoveToArea:
					if (string.IsNullOrEmpty(area) || string.IsNullOrEmpty(position))
						return;

					AreaLocal areaLocal = Main.Singleton.MapManager.Map.AreaByName(area);

					Main.Singleton.MapManager.UpdatePlayerArea(player, areaLocal, position);

					if (player.Data().isControlling)
						Main.Singleton.Request.Send("MoveToArea", area, position);
					break;
				case AreaMoveType.JoinMap:
					if (!player.Data().isControlling || string.IsNullOrEmpty(join) || string.IsNullOrEmpty(area) ||
					    string.IsNullOrEmpty(position))
						return;

					UIController.CreateLoadingMap();
					Main.Singleton.UIManager.LoadingOverlay.SetLoadingText("LOADING MAP");

					Main.Singleton.Request.Send("Join", join, area, position);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

	}
}