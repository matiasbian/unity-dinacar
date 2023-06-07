using System;
using UnityEngine;

namespace HQ
{
    public abstract class Modifier 
    {
        public string label;
        public bool disabled;
        public float h;
        public float spriteX;
        public bool flipX;
        public Sprite sprite;
        public Vector2Int Segments;
        public int frequency;
    }
    [Serializable]
    public class TrackModifier : Modifier
    {
        public float curve;
    }
    [Serializable]
    public class CarModifier : Modifier
    {
        public int speed;
    }
}
