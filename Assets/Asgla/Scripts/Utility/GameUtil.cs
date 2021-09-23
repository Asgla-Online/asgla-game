using Asgla.Avatar;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Asgla.Utility {
    public class GameUtil {

        public static List<AvatarMain> FindTarget(int quantity, double scale, double range, AvatarMain from, HashSet<AvatarMain> near, HashSet<AvatarMain> ignore) {
            return near.Where(a => AvatarDistance(scale, a.Position(), from.Position()) <= range && a != from && !ignore.Contains(a))
                .OrderBy(a => AvatarDistance(scale, a.Position(), from.Position()))
                .Take(quantity)
                .ToList();
        }

        /*public static List<AvatarMain> FindTarget(int quantity, double scale, double range, AvatarMain avatar, HashSet<AvatarMain> near, HashSet<AvatarMain> ignore) {

                HashSet<AvatarMain> found = new HashSet<AvatarMain>();

                int i = 0;

                AvatarMain nearest = null;
                double nearestDistance = int.MaxValue;

                foreach (AvatarMain a in near) {
                    Debug.LogFormat("findTarget 1: {0} {1}", near.Count, found.Count);

                    if (ignore.Contains(a))
                        continue;

                    if (i > quantity)
                        break;

                    Debug.LogFormat("findTarget 2: {0} {1}", near.Count, found.Count);

                    foreach (AvatarMain a1 in near) {
                        double distance = AvatarDistance(scale, a1.Position(), avatar.Position());
                        if (distance <= range && distance < nearestDistance && !found.Contains(a1)) {
                            nearest = a1;
                            nearestDistance = distance;
                        }
                    }

                    found.Add(nearest);

                    Debug.LogFormat("findTarget 3: {0} {1}", near.Count, found.Count);

                    i++;
                }

                Debug.LogFormat("findTarget 4: {0} {1}", near.Count, found.Count);

                return found;
        }*/

        public static double AvatarDistance(double scale, Vector2 a, Vector2 b) {
            Debug.LogWarningFormat("AvatarDistance {0}, {1}, {2}", scale, a, b);
            double v0 = (double)(b.x / scale - a.x / scale);
            double v1 = (double)(b.y / scale - a.y / scale);
            return (double)Math.Sqrt(v0 * v0 + v1 * v1);
        }

        // Get Mouse Position in World with Z = 0f
        public static Vector3 GetMouseWorldPosition() {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }

        public static Vector3 GetMouseWorldPositionWithZ() {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera) {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }

        // Is Mouse over a UI Element? Used for ignoring World clicks through UI
        public static bool IsPointerOverUI() {
            if (EventSystem.current.IsPointerOverGameObject()) {
                return true;
            } else {
                PointerEventData pe = new PointerEventData(EventSystem.current);
                pe.position = Input.mousePosition;

                List<RaycastResult> hits = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pe, hits);

                return hits.Count > 0;
            }
        }

        // Returns 00-FF, value 0->255
        /*public static string Dec_to_Hex(int value) {
            return value.ToString("X2");
        }

        // Returns 0-255
        public static int Hex_to_Dec(string hex) {
            return Convert.ToInt32(hex, 16);
        }

        // Returns a hex string based on a number between 0->1
        public static string Dec01_to_Hex(float value) {
            return Dec_to_Hex((int)Mathf.Round(value * 255f));
        }

        // Returns a float between 0->1
        public static float Hex_to_Dec01(string hex) {
            return Hex_to_Dec(hex) / 255f;
        }

        // Get Hex Color FF00FF
        public static string GetStringFromColor(Color color) {
            string red = Dec01_to_Hex(color.r);
            string green = Dec01_to_Hex(color.g);
            string blue = Dec01_to_Hex(color.b);
            return red + green + blue;
        }

        // Get Hex Color FF00FFAA
        public static string GetStringFromColorWithAlpha(Color color) {
            string alpha = Dec01_to_Hex(color.a);
            return GetStringFromColor(color) + alpha;
        }

        // Sets out values to Hex String 'FF'
        public static void GetStringFromColor(Color color, out string red, out string green, out string blue, out string alpha) {
            red = Dec01_to_Hex(color.r);
            green = Dec01_to_Hex(color.g);
            blue = Dec01_to_Hex(color.b);
            alpha = Dec01_to_Hex(color.a);
        }

        // Get Hex Color FF00FF
        public static string GetStringFromColor(float r, float g, float b) {
            string red = Dec01_to_Hex(r);
            string green = Dec01_to_Hex(g);
            string blue = Dec01_to_Hex(b);
            return red + green + blue;
        }

        // Get Hex Color FF00FFAA
        public static string GetStringFromColor(float r, float g, float b, float a) {
            string alpha = Dec01_to_Hex(a);
            return GetStringFromColor(r, g, b) + alpha;
        }

        // Get Color from Hex string FF00FFAA
        public static Color GetColorFromString(string color) {
            float red = Hex_to_Dec01(color.Substring(0, 2));
            float green = Hex_to_Dec01(color.Substring(2, 2));
            float blue = Hex_to_Dec01(color.Substring(4, 2));
            float alpha = 1f;
            if (color.Length >= 8) {
                // Color string contains alpha
                alpha = Hex_to_Dec01(color.Substring(6, 2));
            }
            return new Color(red, green, blue, alpha);
        }*/

    }
}
