using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Control Rigging/Limit Position")]
    public class LimitPositionConstraint : RigConstraint<
        LimitPositionJob, 
        LimitPositionData, 
        LimitPositionJobBinder<LimitPositionData>>
    {
    }
}