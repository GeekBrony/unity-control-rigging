using Unity.Burst;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    [BurstCompile]
    public struct LimitRotationJob : IWeightedAnimationJob
    {
        public ReadWriteTransformHandle ConstrainedHandle;
        
        public Vector3 Axis;
        public float Limit;
        
        public void ProcessRootMotion(AnimationStream stream) { }
        
        public void ProcessAnimation(AnimationStream stream)
        {
            float weight = jobWeight.Get(stream);
            
            if (!ConstrainedHandle.IsValid(stream))
                return;

            if (weight <= 0)
            {
                AnimationRuntimeUtils.PassThrough(stream, ConstrainedHandle);
                return;
            }
            
            // get original rotation and calculate the axis
            Quaternion originalRotation = ConstrainedHandle.GetLocalRotation(stream);
            Vector3 constrainedAxis = originalRotation * Axis;
            
            // Get the limited axis
            Quaternion rotation = Quaternion.FromToRotation(Axis, constrainedAxis);
            Quaternion limitRot = Quaternion.RotateTowards(Quaternion.identity, rotation, Limit);
            
            // Get the rotation from the off-limits rotation to the in-limits rotation
            Quaternion toLimits = Quaternion.FromToRotation(constrainedAxis, limitRot * Axis);
            
            // subtract the limits from the original rotation
            Quaternion targetRotation = toLimits * originalRotation;
            
            // using the weight, blend the original rotation to the target rotation
            Quaternion finalRotation = Quaternion.Lerp(originalRotation, targetRotation, weight);
            
            // Finally, set the local rotation.
            ConstrainedHandle.SetLocalRotation(stream, finalRotation);
        }
        
        public FloatProperty jobWeight { get; set; }
    }

    public class LimitRotationJobBinder<T> : AnimationJobBinder<LimitRotationJob, T>
        where T : struct, IAnimationJobData, ILimitRotationData
    {
        public override LimitRotationJob Create(Animator animator, ref T data, Component component)
        {
            LimitRotationJob job = new LimitRotationJob();
            
            job.ConstrainedHandle = ReadWriteTransformHandle.Bind(animator, data.ConstrainedTransform);
            job.Axis = data.ConstrainedAxis.ToDirection();
            job.Limit = data.Limit;
            
            return job;
        }

        public override void Destroy(LimitRotationJob job)
        {
            
        }
    }
}