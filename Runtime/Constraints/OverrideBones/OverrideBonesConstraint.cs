using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    [DisallowMultipleComponent] 
    [AddComponentMenu("Control Rigging/Copy Transforms Constraint")]
    public class OverrideBonesConstraint : RigConstraint<
        OverrideBonesJob,
        OverrideBonesData,
        OverrideBonesJobBinder<OverrideBonesData>>
    {
    }
}

