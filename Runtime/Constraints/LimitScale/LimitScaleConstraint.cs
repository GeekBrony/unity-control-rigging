using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Control Rigging/Limit Scale")]
    public class LimitScaleConstraint : RigConstraint<
        LimitScaleJob, 
        LimitScaleData, 
        LimitScaleJobBinder<LimitScaleData>>
    {
    }
}