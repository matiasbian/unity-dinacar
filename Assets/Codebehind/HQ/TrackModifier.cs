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
    public class CarModifier
    {
        public string label;
        [HideInInspector]
        public float h;
        public enum Lane { Left, Middle, Right };
        public Lane spriteX;
        public Sprite sprite;
        public int position;
        public int speed;
        public bool disabled;

        public float GetLane () {
            switch (spriteX) {
                case Lane.Left:
                    return -0.67f;
                case Lane.Middle:
                    return -0.01f;
                case Lane.Right:
                    return 0.13f;
                default:
                    return 0;
            }
        }
    }
}
