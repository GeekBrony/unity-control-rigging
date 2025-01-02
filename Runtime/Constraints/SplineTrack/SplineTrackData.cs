using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Splines;

namespace ControlRigging.Constraints
{
    [Serializable]
    public struct SplineTrackData : IAnimationJobData, ISplineTrackData
    {
        [Header("Transform")]
        [SerializeField]
        Transform m_ConstrainedTransform;

        public Transform ConstrainedTransform
        {
            get => m_ConstrainedTransform;
            set => m_ConstrainedTransform = value;
        }

        [Header("Spline")]
        [SerializeField]
        SplineContainer m_Spline;

        public SplineContainer Spline
        {
            get => m_Spline;
            set => m_Spline = value;
        }

        [SyncSceneToStream, SerializeField] 
        float m_Time;
        public string TimeFloatProperty => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_Time));

        [Header("Offset")]
        [SyncSceneToStream, SerializeField]
        private Vector3 m_PositionOffset;

        public string PositionOffsetVectorProperty =>
            ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_PositionOffset));

        /// <summary>The weight for which position has an effect on constrained Transform. This is a value in between 0 and 1.</summary>
        public Vector3 PositionOffset
        {
            get => m_PositionOffset;
            set => m_PositionOffset = value;
        }

        [SyncSceneToStream, SerializeField] 
        private Vector3 m_RotationOffset;

        public string RotationOffsetVectorProperty =>
            ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_RotationOffset));

        /// <summary>The weight for which position has an effect on constrained Transform. This is a value in between 0 and 1.</summary>
        public Vector3 RotationOffset
        {
            get => m_RotationOffset;
            set => m_RotationOffset = value;
        }

        [Header("Weights")] 
        [SyncSceneToStream, SerializeField, Range(0f, 1f)]
        float m_PositionWeight;

        public string PositionWeightFloatProperty =>
            ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_PositionWeight));

        /// <summary>The weight for which position has an effect on constrained Transform. This is a value in between 0 and 1.</summary>
        public float PositionWeight
        {
            get => m_PositionWeight;
            set => m_PositionWeight = Mathf.Clamp01(value);
        }

        [SyncSceneToStream, SerializeField, Range(0f, 1f)]
        float m_RotationWeight;

        public string RotationWeightFloatProperty =>
            ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_RotationWeight));

        /// <summary>The weight for which rotation has an effect on constrained Transform. This is a value in between 0 and 1.</summary>
        public float RotationWeight
        {
            get => m_RotationWeight;
            set => m_RotationWeight = Mathf.Clamp01(value);
        }

        public bool IsValid()
        {
            return Spline != null && Spline.Splines.Count > 0;
        }

        public void SetDefaultValues()
        {
            m_Spline = null;
            m_ConstrainedTransform = null;
            PositionWeight = 1f;
            RotationWeight = 1f;
            PositionOffset = Vector3.zero;
            RotationOffset = Vector3.zero;
            m_Time = 0;
        }
    }

    public interface ISplineTrackData
    {
        Transform ConstrainedTransform { get; }
        SplineContainer Spline { get; }

        /// <summary>The path to the position offset property in the constraint component.</summary>
        string PositionOffsetVectorProperty { get; }

        /// <summary>The path to the rotation offset property in the constraint component.</summary>
        string RotationOffsetVectorProperty { get; }

        /// <summary>The path to the time property in the constraint component.</summary>
        string TimeFloatProperty { get; }

        /// <summary>The path to the position weight property in the constraint component.</summary>
        string PositionWeightFloatProperty { get; }

        /// <summary>The path to the rotation weight property in the constraint component.</summary>
        string RotationWeightFloatProperty { get; }
    }
}