using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    [AddComponentMenu("Control Rigging/Limit Distance")]
    public class LimitDistanceConstraint
        : RigConstraint<
        LimitDistanceJob, 
        LimitDistanceData, 
        LimitDistanceJobBinder<LimitDistanceData>>
    {
        public void OnDrawGizmosSelected()
        {
            var tTransform = data.TargetTransform;
            if (tTransform)
            {
                Vector3 targetTransformPos = tTransform.rotation *
                                             Vector3.Scale(data.Offset, tTransform.localScale) +
                                             tTransform.position;
                
                Gizmos.DrawWireSphere(targetTransformPos, data.Distance);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (data.Distance < 0)
                data.Distance = 0;
        }
    }
}