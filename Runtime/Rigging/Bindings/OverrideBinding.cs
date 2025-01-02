using System;
using UnityEngine;

namespace ControlRigging
{
    [Serializable]
    public class OverrideBinding : IRigBinding
    {
        [Serializable]
        public struct OverrideArmatureBinding
        {
            public ArmatureBinding bone;
            public ArmatureBinding target;
        }
        
        public OverrideArmatureBinding[] bindings;
        
        [Range(0, 1)]
        public float positionWeight = 1;
        [Range(0, 1)]
        public float rotationWeight = 1;
        [Range(0, 1)]
        public float scaleWeight = 1;
    }
}