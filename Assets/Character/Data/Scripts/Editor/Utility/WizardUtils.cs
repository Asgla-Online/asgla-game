using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CharacterCreator2D;

namespace CharacterEditor2D {
    public static class WizardUtils {
        public const string PartTemplateFolder = "Assets/Character/Data/Part Templates";
        public const string PaletteFolder = "Assets/Character/Color Palettes";
        public const string PartFolder = "Assets/Asgla/Game/items"; //Assets/Asgla/Game/Items
        public const string SetupDataPath = "Assets/Character/Data/Resources/CC2D_SetupData.asset";
        //public const string PartListPath = "Assets/Character/Data/Resources/CC2D_PartList.asset";

        private static GUIStyle _bgstyle = createBGStyle();
        public static GUIStyle BGStyle {
            get { return _bgstyle; }
        }

        private static GUIStyle createBGStyle() {
            GUIStyle val = new GUIStyle();
            val.normal.background = EditorUtils.CreateTexture(1, 1, new Color(0.6f, 0.6f, 0.6f, 1.0f));
            return val;
        }

        private static GUIStyle _boldtxtstyle = createBoldTextStyle();
        public static GUIStyle BoldTextStyle {
            get { return _boldtxtstyle; }
        }

        private static GUIStyle createBoldTextStyle() {
            GUIStyle val = new GUIStyle();
            val.fontStyle = FontStyle.Bold;
            return val;
        }
    }
}