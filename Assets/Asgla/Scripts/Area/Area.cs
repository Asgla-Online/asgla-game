using System.Collections.Generic;
using System.Linq;
using Asgla.Data.Area;
using UnityEngine;

namespace Asgla.Area {
	public class Area : MonoBehaviour {

		[SerializeField] private List<AreaLocal> areas;

		private AreaData _data;

		/*private void Awake() {
	    foreach (Transform child in transform) {
	        Debug.LogFormat("Child: {0}", child.name);        
	    }
		}*/

		public AreaData Data(AreaData data) {
			return _data = data;
		}

		public AreaData Data() {
			return _data;
		}

		public List<AreaLocal> Areas() {
			return areas;
		}

		public AreaLocal AreaByName(string areaName) {
			return areas.FirstOrDefault(zone => zone.name == areaName);
		}

	}
}