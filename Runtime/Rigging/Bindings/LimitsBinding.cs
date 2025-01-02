using System;
using ControlRigging.Constraints;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ControlRigging
{
    [Serializable]
    public class PositionLimitBinding : IRigBinding
    {
        public ArmatureBinding bone;
        
        public Vector3Bool limitMin;
        public Vector3Bool limitMax;
        
        [Space]
        public LimitPositionData.Space space;
        public Vector3 minimum = new Vector3(0, 0, 0);
        public Vector3 maximum = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
    }
    
    [Serializable]
    public class RotationLimitBinding : IRigBinding
    {
        public ArmatureBinding bone;
        
        [Space]
        public Axis axis;
        
        [Range(0,180)]
        public float limit = 180;
    }
    
    [Serializable]
    public class ScaleLimitBinding : IRigBinding
    {
        public ArmatureBinding bone;

        public Vector3Bool limitMin;
        public Vector3Bool limitMax;
        
        [Space]
        public Vector3 minimum = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        public Vector3 maximum = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
    }
    
    [Serializable]
    public class DistanceLimitBinding : IRigBinding
    {
        public ArmatureBinding bone;
        
        public enum LimitDistanceMode
        {
            KeepInside, KeepOutside
        }

        public LimitDistanceMode mode = LimitDistanceMode.KeepInside;
        
        public ArmatureBinding target;

        [Space]
        public float distance = 0;

        public Vector3 offset = Vector3.zero;
    }
}