using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ControlRigging
{
    [Serializable]
    public class AimBinding : IRigBinding
    {
        public ArmatureBinding bone;
        
        [Serializable]
        public class AimTargetBinding
        {
            public ArmatureBinding target;
            [Range(0, 1)]
            public float weight = 1;
        }
        
        public AimTargetBinding[] targets;
        
        public Axis aimAxis = Axis.XNegative;
        public Axis upAxis = Axis.Y;

        public MultiAimConstraintData.WorldUpType worldUpType = MultiAimConstraintData.WorldUpType.None;
        public MultiAimConstraintData.Axis worldUpAxis;
        public bool useWorldUpBinding = false;
        public ArmatureBinding worldUpBinding;
        
        public Vector3Bool constrainedAxes = new (true, true, true);
        [Range(-180, 180)]
        public float minLimit = -180;
        [Range(-180, 180)]
        public float maxLimit = 180;
    }
}