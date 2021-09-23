using Asgla.UI.UnitFrame;
using TMPro;
using UnityEngine;

namespace Asgla.Avatar {
    public class AvatarUtility : MonoBehaviour {

        [Header("Text")]
        [SerializeField] private TextMeshPro _title;
        [SerializeField] private TextMeshPro _name;
        [SerializeField] private TextMeshPro _guild;

        [Header("Game Object")]
        [SerializeField] private GameObject _loadFlame;

        [SerializeField] private UnitFrameSmall _smallUnitFrameOne;
        //[SerializeField] private UnitFrameSmall _smallUnitFrameTwo;

        public GameObject LoadFlame => _loadFlame;

        public UnitFrameSmall SmallUnitFrameOne => _smallUnitFrameOne;
        //public UnitFrameSmall SmallUnitFrameTwo => _smallUnitFrameTwo;

        public void SetTitle(string title) => _title.text = title;

        public void SetName(string name) => _name.text = name;

        public void SetGuild(string guild) => _guild.text = guild;

    }
}
