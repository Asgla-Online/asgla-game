using System;
using System.Collections.Generic;
using System.Linq;
using Asgla.Avatar.Player;
using Asgla.Data.Area;
using Asgla.NPC;
using Asgla.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Asgla.Map {
	public class MapArea : MonoBehaviour {

		public OnPlayerScaleUpdateEvent OnPlayerScaleUpdate = new OnPlayerScaleUpdateEvent();

		private readonly List<NPCMain> _npcs = new List<NPCMain>();

		private readonly List<Transform> _zones = new List<Transform>();

		private Transform _audio;

		private AreaLocal _local;
		private GameObject _monsters;

		private Transform _npc;

		private GameObject _players;

		private Transform _zone;

		public void Data(AreaLocal local) {
			_local = local;
		}

		public AreaLocal Data() {
			return _local;
		}

		public string Name() {
			return _local.name;
		}

		public double Scale() {
			return _local.scale;
		}

		public double Speed() {
			return _local.speed;
		}

		public bool IsSafe() {
			return _local.isSafe;
		}

		public Transform Players() {
			return _players.transform;
		}

		public Transform Monsters() {
			return _monsters.transform;
		}

		public Transform Zone() {
			return _zone;
		}

		public List<Transform> Zones() {
			return _zones;
		}

		public Transform NPC() {
			return _npc;
		}

		public List<NPCMain> NPCs() {
			return _npcs;
		}

		public Transform ZoneByName(string name) {
			return _zones.Where(zone => zone.name == name).FirstOrDefault();
		}

		public NPCMain NpcById(int id) {
			return _npcs.Where(npc => npc.AreaId() == id).FirstOrDefault();
		}

		[Serializable]
		public class OnPlayerScaleUpdateEvent : UnityEvent<float> {

		} //TODO: Replace with c# event..

		#region Unity

		private void Awake() {
			//Debug.LogFormat("<color=blue>[MapArea]</color> Awake");

			_zone = transform.Find("Zone");

			if (_zone != null) {
				foreach (Transform child in _zone)
					_zones.Add(child);

				_players = new GameObject("Players");
				_players.transform.position = Vector3.zero;
				_players.transform.parent = transform;

				_monsters = new GameObject("Monsters");
				_monsters.transform.position = Vector3.zero;
				_monsters.transform.parent = transform;

				_npc = transform.Find("NPCs");

				switch (_npc) {
					case null:
						Debug.LogFormat("<color=blue>[MapArea]</color> NPC Transform not found");
						break;
					default:
						_npcs.AddRange(from Transform npcT in _npc let npc = npcT.GetComponent<NPCMain>() select npc);
						break;
				}

				_audio = transform.Find("Audios");

				switch (_audio) {
					case null:
						Debug.LogFormat("<color=blue>[MapArea]</color> Audio Transform not found");
						break;
					default:
						foreach (Transform audio in _audio)
							if (audio.gameObject.GetComponent<AudioSource>() !=
							    null) //Debug.LogFormat("<color=blue>[MapArea]</color> AudioSource {0} {1}", audio.name, Main.Singleton.AudioMixer);
								audio.gameObject.GetComponent<AudioSource>().outputAudioMixerGroup =
									Main.Singleton.AudioMixer;
						break;
				}
			} else {
				Debug.LogFormat("<color=blue>[MapArea]</color> Zone Transform not found");
			}
		}

		private void OnDestroy() {
			OnPlayerScaleUpdate.RemoveAllListeners();
		}

		private void OnTriggerEnter2D(Collider2D collider) {
			if (collider.CompareTag("Player")) {
				//Debug.LogFormat("<color=blue>[MapArea]</color> TriggerEnter {0}", collider.gameObject.name);

				Player player = collider.GetComponent<Player>();

				OnPlayerScaleUpdate.Invoke((float) _local.scale);

				if (player.Data()
					.isControlling) //Debug.LogFormat("<color=blue>[MapArea]</color> Controlling", player.Name());
					Main.Singleton.Game.CinemachineConfiner.m_BoundingShape2D = GetComponent<Collider2D>();
			}
		}

		private void OnMouseDown() {
			if (GameUtil.IsPointerOverUI() || !Main.Singleton.Walkable)
				return;

			Vector2 move = Main.Singleton.Game.Camera.ScreenToWorldPoint(Input.mousePosition);

			Main.Singleton.AvatarManager.Player.MoveByClick(move);
		}

		#endregion

	}
}