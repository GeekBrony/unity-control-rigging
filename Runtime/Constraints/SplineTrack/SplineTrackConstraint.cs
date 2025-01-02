using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Control Rigging/Spline Track Constraint")]
    public class SplineTrackConstraint : RigConstraint<
        SplineTrackJob,
        SplineTrackData,
        SplineTrackJobBinder<SplineTrackData>>
    { }
}
