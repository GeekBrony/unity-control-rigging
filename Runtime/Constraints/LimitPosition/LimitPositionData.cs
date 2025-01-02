using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    [Serializable]
    public struct LimitPositionData : IAnimationJobData, ILimitPositionData
    {
        public Transform ConstrainedTransform { get => m_ConstrainedTransform; set => m_ConstrainedTransform = value; }
        
        [SerializeField]
        private Transform m_ConstrainedTransform;
        
        [Serializable]
        public enum Space { World, Local, Pivot }
        
        [Space, SerializeField]
        Space m_Space;
        public Space space { get => m_Space; set => m_Space = value; }
        
        [Space, SyncSceneToStream, SerializeField]
        Vector3Bool m_LimitMin;
        public Vector3Bool LimitMin { get => m_LimitMin; set => m_LimitMin = value; }
        [SyncSceneToStream, SerializeField]
        Vector3Bool m_LimitMax;
        public Vector3Bool LimitMax { get => m_LimitMax; set => m_LimitMax = value; }
        
        [Header("Limits")]
        [SyncSceneToStream, SerializeField]
        Vector3 m_Minimum;
        public Vector3 Minimum { get => m_Minimum; set => m_Minimum = value; }
        [SyncSceneToStream, SerializeField]
        Vector3 m_Maximum;
        public Vector3 Maximum { get => m_Maximum; set => m_Maximum = value; }
        
        public string LimitMinVector3BoolProp => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_LimitMin));
        public string LimitMaxVector3BoolProp => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_LimitMax));
        public string MinimumVector3Prop => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_Minimum));
        public string MaximumVector3Prop => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_Maximum));

        public bool IsValid()
        {
            return m_ConstrainedTransform;
        }

        public void SetDefaultValues()
        {
            m_ConstrainedTransform = null;
            m_Space = Space.Pivot;
            m_Minimum = Vector3.zero;
            m_Maximum = Vector3.zero;
            m_LimitMin = new Vector3Bool(false);
            m_LimitMax = new Vector3Bool(false);
        }
    }

    public interface ILimitPositionData
    {
        Transform ConstrainedTransform { get; set; }
        
        public LimitPositionData.Space space { get; set; }

        public Vector3Bool LimitMin { get; set; }
        string LimitMinVector3BoolProp { get; }
        public Vector3 Minimum { get; set; }
        string MinimumVector3Prop { get; }
        
        public Vector3Bool LimitMax { get; set; }
        string LimitMaxVector3BoolProp { get; }
        public Vector3 Maximum { get; set; }
        string MaximumVector3Prop { get; }
    }
}