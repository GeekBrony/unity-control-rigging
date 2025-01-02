using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    [DisallowMultipleComponent] 
    [AddComponentMenu("Control Rigging/Offset Position")]
    public class OffsetPositionConstraint : RigConstraint<
        OffsetPositionJob,
        OffsetPositionData,
        OffsetPositionJobBinder<OffsetPositionData>>
    {
    }

    public class OffsetPositionJobBinder<T> : AnimationJobBinder<OffsetPositionJob, T>
        where T : struct, IAnimationJobData, IOffsetPositionData
    {
        public override OffsetPositionJob Create(Animator animator, ref T data, Component component)
        {
            var job = new OffsetPositionJob();
            if (data.ConstrainedTransform)
            {
                job.ConstrainedHandle = ReadWriteTransformHandle.Bind(animator, data.ConstrainedTransform);
            }
            
            job.Offset = Vector3Property.Bind(animator, component, data.OffsetVector3Property);

            return job;
        }

        public override void Destroy(OffsetPositionJob job)
        {
            
        }
    }


    public struct OffsetPositionJob : IWeightedAnimationJob
    {
        public ReadWriteTransformHandle ConstrainedHandle;
        public Vector3Property Offset;
        
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
            
            Vector3 localPos = ConstrainedHandle.GetPosition(stream);
            Vector3 offset = Offset.Get(stream);
            Vector3 offsetPos = localPos + (offset);
            ConstrainedHandle.SetPosition(stream, Vector3.Lerp(localPos, offsetPos, weight));
        }

        public void ProcessRootMotion(AnimationStream stream)
        {
        }

        public FloatProperty jobWeight { get; set; }
    }

    public interface IOffsetPositionData
    {
        Transform ConstrainedTransform { get; set; }
        Vector3 Offset { get; set; }
        string OffsetVector3Property { get; }
        Space Space { get; set; }
    }
    
    [Serializable]
    public struct OffsetPositionData : IAnimationJobData, IOffsetPositionData
    {
        [SerializeField]
        Transform m_ConstrainedTransform;
        
        public Transform ConstrainedTransform
        {
            get => m_ConstrainedTransform;
            set => m_ConstrainedTransform = value;
        }
        
        [SerializeField]
        Vector3 m_Offset;
        
        public Vector3 Offset
        {
            get => m_Offset;
            set => m_Offset = value;
        }

        public string OffsetVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_Offset));
        
        [SerializeField]
        Space m_Space;

        public Space Space
        {
            get => m_Space;
            set => m_Space = value;
        }
        
        public bool IsValid()
        {
            return m_ConstrainedTransform != null;
        }

        public void SetDefaultValues()
        {
            m_ConstrainedTransform = null;
            m_Offset = Vector3.zero;
            m_Space = Space.Self;
        }
    }
}