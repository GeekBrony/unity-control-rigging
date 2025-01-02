using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    public struct LimitPositionJob : IWeightedAnimationJob
    {
        public ReadWriteTransformHandle ConstrainedHandle;
        public Vector3 StartPosition;
        public LimitPositionData.Space Space;

        public Vector3BoolProperty LimitMin;
        public Vector3BoolProperty LimitMax;
        public Vector3Property Minimum;
        public Vector3Property Maximum;
        
        public void ProcessRootMotion(AnimationStream stream) { }
        
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

            Vector3 originalPosition = Vector3.zero;
            switch (Space)
            {
                case LimitPositionData.Space.Local:
                    originalPosition = ConstrainedHandle.GetLocalPosition(stream);
                    break;
                case LimitPositionData.Space.World:
                    originalPosition = ConstrainedHandle.GetPosition(stream);
                    break;
                case LimitPositionData.Space.Pivot:
                    originalPosition = ConstrainedHandle.GetPosition(stream) - StartPosition;
                    break;
            }
            
            Vector3 position = originalPosition;
            Vector3Bool limitMin = LimitMin.Get(stream);
            Vector3Bool limitMax = LimitMax.Get(stream);
            Vector3 min = Minimum.Get(stream);
            Vector3 max = Maximum.Get(stream);
            
            if (limitMin.x && position.x < min.x)
                position.x = math.clamp(position.x, min.x, float.PositiveInfinity);
            
            if (limitMin.y && position.y < min.y)
                position.y = math.clamp(position.y, min.y, float.PositiveInfinity);
            
            if (limitMin.z && position.z < min.z)
                position.z = math.clamp(position.z, min.z, float.PositiveInfinity);
            
            if (limitMax.x && position.z > max.z)
                position.x = math.clamp(position.x, float.NegativeInfinity, max.x);
            
            if (limitMax.y && position.y > max.y)
                position.y = math.clamp(position.y, float.NegativeInfinity, max.y);
            
            if (limitMax.z && position.z > max.z)
                position.z = math.clamp(position.z, float.NegativeInfinity, max.z);

            if (Space == LimitPositionData.Space.Pivot)
                position += StartPosition;
            
            Vector3 finalPosition = Vector3.Lerp(originalPosition, position, weight);

            switch (Space)
            {
                case LimitPositionData.Space.Local:
                    ConstrainedHandle.SetLocalPosition(stream, finalPosition);
                    break;
                case LimitPositionData.Space.World:
                case LimitPositionData.Space.Pivot:
                    ConstrainedHandle.SetPosition(stream, finalPosition);
                    break;
            }
        }
        
        public FloatProperty jobWeight { get; set; }
    }

    public class LimitPositionJobBinder<T> : AnimationJobBinder<LimitPositionJob, T>
        where T : struct, IAnimationJobData, ILimitPositionData
    {
        public override LimitPositionJob Create(Animator animator, ref T data, Component component)
        {
            var job = new LimitPositionJob();

            var constrainedTransform = data.ConstrainedTransform;
            if (constrainedTransform)
            {
                job.ConstrainedHandle = ReadWriteTransformHandle.Bind(animator, constrainedTransform);
                job.StartPosition = constrainedTransform.position;
            }
            
            job.Space = data.space;
            
            job.LimitMin = Vector3BoolProperty.Bind(animator, component, data.LimitMinVector3BoolProp);
            job.LimitMax = Vector3BoolProperty.Bind(animator, component, data.LimitMaxVector3BoolProp);
            job.Minimum = Vector3Property.Bind(animator, component, data.MinimumVector3Prop);
            job.Maximum = Vector3Property.Bind(animator, component, data.MaximumVector3Prop);
            
            return job;
        }
        
        public override void Update(LimitPositionJob job, ref T data)
        {
            job.Space = data.space;
        }

        public override void Destroy(LimitPositionJob job)
        { }

    }
}