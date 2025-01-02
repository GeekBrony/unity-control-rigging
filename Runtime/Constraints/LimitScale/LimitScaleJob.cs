using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    public struct LimitScaleJob : IWeightedAnimationJob
    {
        public ReadWriteTransformHandle ConstrainedHandle;

        public Vector3BoolProperty LimitMin;
        public Vector3BoolProperty LimitMax;
        public Vector3Property Minimum;
        public Vector3Property Maximum;
        
        public void ProcessAnimation(AnimationStream stream)
        {
            if(!ConstrainedHandle.IsValid(stream))
                return;

            float weight = jobWeight.Get(stream);
            if (weight <= 0)
            {
                AnimationRuntimeUtils.PassThrough(stream, ConstrainedHandle);
                return;
            }

            Vector3 originalScale = ConstrainedHandle.GetLocalScale(stream);
            Vector3 scale = originalScale;
            Vector3Bool limitMin = LimitMin.Get(stream);
            Vector3Bool limitMax = LimitMax.Get(stream);
            Vector3 min = Minimum.Get(stream);
            Vector3 max = Maximum.Get(stream);
            
            if (limitMin.x && scale.x < min.x)
                scale.x = math.clamp(scale.x, min.x, float.PositiveInfinity);
            
            if (limitMin.y && scale.y < min.y)
                scale.y = math.clamp(scale.y, min.y, float.PositiveInfinity);
            
            if (limitMin.z && scale.z < min.z)
                scale.z = math.clamp(scale.z, min.z, float.PositiveInfinity);
            
            if (limitMax.x && scale.z > max.z)
                scale.x = math.clamp(scale.x, float.NegativeInfinity, max.x);
            
            if (limitMax.y && scale.y > max.y)
                scale.y = math.clamp(scale.y, float.NegativeInfinity, max.y);
            
            if (limitMax.z && scale.z > max.z)
                scale.z = math.clamp(scale.z, float.NegativeInfinity, max.z);
            
            Vector3 finalScale = Vector3.Lerp(originalScale, scale, weight);
            
            ConstrainedHandle.SetLocalScale(stream, finalScale);
        }

        public void ProcessRootMotion(AnimationStream stream)
        {
            
        }

        public FloatProperty jobWeight { get; set; }
    }
    
    public class LimitScaleJobBinder<T> : AnimationJobBinder<LimitScaleJob, T>
        where T : struct, IAnimationJobData, ILimitScaleData
    {
        public override LimitScaleJob Create(Animator animator, ref T data, Component component)
        {
            var job = new LimitScaleJob();

            var constrainedTransform = data.ConstrainedTransform;
            if (constrainedTransform)
            {
                job.ConstrainedHandle = ReadWriteTransformHandle.Bind(animator, constrainedTransform);
            }
            
            job.LimitMin = Vector3BoolProperty.Bind(animator, component, data.LimitMinVector3BoolProp);
            job.LimitMax = Vector3BoolProperty.Bind(animator, component, data.LimitMaxVector3BoolProp);
            job.Minimum = Vector3Property.Bind(animator, component, data.MinimumVector3Prop);
            job.Maximum = Vector3Property.Bind(animator, component, data.MaximumVector3Prop);
            
            return job;
        }
        
        public override void Update(LimitScaleJob job, ref T data)
        {
            
        }

        public override void Destroy(LimitScaleJob job)
        { }
    }
}