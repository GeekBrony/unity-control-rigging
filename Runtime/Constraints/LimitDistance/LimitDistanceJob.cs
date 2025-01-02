using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    public struct LimitDistanceJob : IWeightedAnimationJob
    {
        public ReadWriteTransformHandle ConstrainedHandle;
        public ReadOnlyTransformHandle TargetHandle;
        public FloatProperty Distance;
        public Vector3 Offset;

        public Vector3 lossyScale;
        
        public DistanceLimitBinding.LimitDistanceMode Mode;
        
        public void ProcessAnimation(AnimationStream stream)
        {
            if(!ConstrainedHandle.IsValid(stream))
                return;
            
            if(!TargetHandle.IsValid(stream))
                return;

            float weight = jobWeight.Get(stream);
            if (weight <= 0)
            {
                AnimationRuntimeUtils.PassThrough(stream, ConstrainedHandle);
                return;
            }
            
            Vector3 localTransformPos = ConstrainedHandle.GetPosition(stream);
            Vector3 targetTransformPos = TargetHandle.GetRotation(stream) *
                                         Vector3.Scale(Offset, TargetHandle.GetLocalScale(stream)) +
                                         TargetHandle.GetPosition(stream);
            
            Vector3 limitedTransformPos = localTransformPos;

            float distLimit = Distance.Get(stream);
            float currentDistance = Vector3.Distance(localTransformPos, targetTransformPos);

            Vector3 direction = (localTransformPos - targetTransformPos).normalized;
            if (direction.magnitude == 0)
                direction = Vector3.up;
            
            if (Mode == DistanceLimitBinding.LimitDistanceMode.KeepInside)
            {
                if (currentDistance > distLimit)
                {
                    limitedTransformPos = targetTransformPos + (direction * distLimit);
                }
                else
                {
                    AnimationRuntimeUtils.PassThrough(stream, ConstrainedHandle);
                    return;
                }
            }
            else if(Mode == DistanceLimitBinding.LimitDistanceMode.KeepOutside)
            {
                if (currentDistance < distLimit)
                {
                    limitedTransformPos = targetTransformPos + (direction * distLimit);
                }
                else
                {
                    AnimationRuntimeUtils.PassThrough(stream, ConstrainedHandle);
                    return;
                }
            }
            
            ConstrainedHandle.SetPosition(stream, Vector3.Lerp(localTransformPos, limitedTransformPos, weight));
        }
        

        public void ProcessRootMotion(AnimationStream stream)
        {
            
        }

        public FloatProperty jobWeight { get; set; }
    }
    
    public class LimitDistanceJobBinder<T> : AnimationJobBinder<LimitDistanceJob, T>
        where T : struct, IAnimationJobData, ILimitDistanceData
    {
        public override LimitDistanceJob Create(Animator animator, ref T data, Component component)
        {
            var job = new LimitDistanceJob();

            var constrainedTransform = data.ConstrainedTransform;
            if (constrainedTransform)
            {
                job.ConstrainedHandle = ReadWriteTransformHandle.Bind(animator, constrainedTransform);
            }
            
            var targetTransform = data.TargetTransform;
            if (targetTransform)
            {
                job.TargetHandle = ReadOnlyTransformHandle.Bind(animator, targetTransform);
                job.lossyScale = data.TargetTransform.lossyScale;
            }
            
            job.Distance = FloatProperty.Bind(animator, component, data.LimitDistanceFloatProp);
            job.Offset = data.Offset;
            job.Mode = data.Mode;
            
            return job;
        }
        
        public override void Update(LimitDistanceJob job, ref T data)
        {
            job.lossyScale = data.TargetTransform.lossyScale;
        }

        public override void Destroy(LimitDistanceJob job)
        { }
    }
}