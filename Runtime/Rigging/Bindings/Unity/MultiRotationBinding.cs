using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ControlRigging
{
    [Serializable]
    public class MultiRotationBinding : IRigBinding
    {
        public ArmatureBinding bone;
        public Vector3Bool constrainedAxes = new (true, true, true);
            
        [Serializable]
        public class MultiRotationSourceBinding
        {
            public ArmatureBinding target;
            [Range(0, 1)]
            public float weight = 1;
        }
        public MultiRotationSourceBinding[] targets;
    }
}