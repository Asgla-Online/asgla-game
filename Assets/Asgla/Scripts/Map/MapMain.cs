using Asgla.Data.Map;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Asgla.Map;

public class MapMain : MonoBehaviour {

    private AreaData _data;

    [SerializeField] private List<MapArea> _areas;

    /*private void Awake() {
        foreach (Transform child in transform) {
            Debug.LogFormat("Child: {0}", child.name);        
        }
    }*/

    public AreaData Data(AreaData data) => _data = data;

    public AreaData Data() => _data;

    public List<MapArea> Areas() => _areas;

    public MapArea AreaByName(string name) => _areas.Where(zone => zone.name == name).FirstOrDefault();

}
