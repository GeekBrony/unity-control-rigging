using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    [Serializable]
    public struct LimitRotationData : IAnimationJobData, ILimitRotationData
    {
        [SerializeField] Transform m_ConstrainedTransform;
        [Space, SerializeField] private Axis m_ConstrainedAxis;
        
        [Range(0, 180), SerializeField] 
        float m_Limit;
        
        public Transform ConstrainedTransform { get => m_ConstrainedTransform; set => m_ConstrainedTransform = value; }
        public Axis ConstrainedAxis { get => m_ConstrainedAxis; set => m_ConstrainedAxis = value; }
        public float Limit { get => m_Limit; set => m_Limit = value; }
        
        public bool IsValid()
        {
            return m_ConstrainedTransform;
        }

        public void SetDefaultValues()
        {
            m_ConstrainedTransform = null;
            m_Limit = 180;
        }
    }

    public interface ILimitRotationData
    {
        Transform ConstrainedTransform { get; set; }
        float Limit { get; set; }
        Axis ConstrainedAxis { get; set; }
    }
}