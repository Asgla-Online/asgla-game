using UnityEngine;

namespace AsglaUI.UI
{
    public interface IColorSchemeElement
    {
        ColorSchemeShade shade { get; set; }
        void Apply(Color color);
    }
}
