using System.Collections.Generic;
using System.Linq;
using Asgla.Data.Quest;
using AsglaUI.UI;
using UnityEngine;

namespace Asgla.UI.Quest.Track {
	[ExecuteInEditMode]
	public class QuestTracker : MonoBehaviour {

		[Header("Quest")] [SerializeField] private QuestTrackObjective _objective;

		[SerializeField] private QuestTrackProgress _progress;

		[SerializeField] private Transform _objectiveContent;

		[SerializeField] private List<QuestTrackProgress> _progresss = new List<QuestTrackProgress>();

		public QuestTrackProgress Get(int databaseId) {
			return _progresss.Where(objective => objective.Quest().DatabaseID == databaseId).FirstOrDefault();
		}

		public void Add(QuestData quest) {
			_progresss.Add(Instantiate(_progress.gameObject, _objectiveContent)
				.GetComponent<QuestTrackProgress>().Init(quest, _objective));
		}

		public void Remove(int databaseId) {
			QuestTrackProgress progress = _progresss.Where(objective => objective.Quest().DatabaseID == databaseId)
				.FirstOrDefault();
			if (progress != null) {
				_progresss.Remove(progress);
				Destroy(progress.gameObject);
			}
		}

		public void OnToggleStateChange(bool state) {
			Debug.Log(state);
			if (state) {
				if (_toggleContent != null)
					_toggleContent.SetActive(true);

				if (_arrowFlippable != null) {
					_arrowFlippable.horizontal = _arrowInvertFlip ? false : true;
					(_arrowFlippable.transform as RectTransform).anchoredPosition = _activeOffset;
				}
			} else {
				if (_toggleContent != null)
					_toggleContent.SetActive(false);

				if (_arrowFlippable != null) {
					_arrowFlippable.horizontal = _arrowInvertFlip ? true : false;
					(_arrowFlippable.transform as RectTransform).anchoredPosition = _inactiveOffset;
				}
			}
		}

		#region Toggle

		[Header("Toggle")] [SerializeField] private GameObject _toggleContent;

		[SerializeField] private UIFlippable _arrowFlippable;

		[SerializeField] private bool _arrowInvertFlip;

		[SerializeField] private Vector2 _activeOffset = Vector2.zero;

		[SerializeField] private Vector2 _inactiveOffset = Vector2.zero;

		#endregion

	}
}