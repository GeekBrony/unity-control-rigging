using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    [DisallowMultipleComponent] 
    [AddComponentMenu("Control Rigging/Limit Rotation")]
    public class LimitRotationConstraint : RigConstraint<
        LimitRotationJob,
        LimitRotationData,
        LimitRotationJobBinder<LimitRotationData>>
    {
    }
}