using System.Collections.Generic;
using System.Linq;
using Asgla.Data.Map;
using Asgla.Map;
using UnityEngine;

public class MapMain : MonoBehaviour {

	[SerializeField] private List<MapArea> _areas;

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

	public List<MapArea> Areas() {
		return _areas;
	}

	public MapArea AreaByName(string name) {
		return _areas.Where(zone => zone.name == name).FirstOrDefault();
	}

}