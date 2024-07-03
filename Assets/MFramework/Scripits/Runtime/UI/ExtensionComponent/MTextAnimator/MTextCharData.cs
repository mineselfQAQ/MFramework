using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public class MTextCharData
    {
        public int index = 0;
        public Vector3[] vertices;
        public Vector3[] oVertices;
        public Color32[] colors32;
        public Color32[] oColors32;

        public Vector3 center;
        public float scale = 1.0f;
        public float angle_rad = 0.0f;
        public Vector3 rot_center = -Vector3.one;
        public Vector3 translation = Vector3.zero;
        public Vector3 shake_translation = Vector3.zero;
    }
}