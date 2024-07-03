using UnityEngine;

namespace MFramework
{
    public static class ColorExtension
    {
        public static Color32 ToColor32(this Color col)
        {
            return new Color32((byte)(col.r * 255f), (byte)(col.g * 255f), (byte)(col.b * 255f), (byte)(col.a * 255f));
        }
    }
}