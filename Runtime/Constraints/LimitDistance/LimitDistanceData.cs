using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    [Serializable]
    public struct LimitDistanceData : IAnimationJobData, ILimitDistanceData
    {
        public Transform ConstrainedTransform { get => m_ConstrainedTransform; set => m_ConstrainedTransform = value; }
        [SerializeField]
        private Transform m_ConstrainedTransform;
        
        public Transform TargetTransform { get => m_TargetTransform; set => m_TargetTransform = value; }
        [SerializeField]
        private Transform m_TargetTransform;
        
        [Header("Limit")]
        [SyncSceneToStream, SerializeField]
        [Tooltip("Distance in Unity units (meters)")]
        float m_Distance;
        public float Distance { get => m_Distance; set => m_Distance = value; }
        public string LimitDistanceFloatProp => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_Distance));

        public DistanceLimitBinding.LimitDistanceMode Mode { get => m_Mode; set => m_Mode = value; }
        [SyncSceneToStream, SerializeField]
        private DistanceLimitBinding.LimitDistanceMode m_Mode;

        public Vector3 Offset { get => m_Offset; set => m_Offset = value; }
        [SyncSceneToStream, SerializeField]
        private Vector3 m_Offset;
        
        public bool IsValid()
        {
            return m_ConstrainedTransform && m_TargetTransform && m_Distance >= 0;
        }

        public void SetDefaultValues()
        {
            m_ConstrainedTransform = null;
            m_TargetTransform = null;
            Distance = 0;
            m_Mode = DistanceLimitBinding.LimitDistanceMode.KeepInside;
            Offset = Vector3.zero;
        }
    }

    public interface ILimitDistanceData
    {
        public Transform ConstrainedTransform { get; set; }
        public Transform TargetTransform { get; set; }
        string LimitDistanceFloatProp { get; }
        public DistanceLimitBinding.LimitDistanceMode Mode { get; set; }
        public Vector3 Offset { get; set; }
    }
}