using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

namespace ControlRigging.Constraints
{
    [Serializable]
    public struct CopyTransformDataSingle
    {
        public Transform ConstrainedTransform;

        [FormerlySerializedAs("TrackedTransform")] [SyncSceneToStream]
        public Transform CopiedTransform;
    }

    /// <summary>
    /// This interface defines the data mapping for CopyTransforms.
    /// </summary>
    public interface ICopyTransformsData
    {
        List<CopyTransformDataSingle> Transforms { get; }

        /// <summary>The path to the position weight property in the constraint component.</summary>
        string positionWeightFloatProperty { get; }

        /// <summary>The path to the rotation weight property in the constraint component.</summary>
        string rotationWeightFloatProperty { get; }

        /// <summary>The path to the scale weight property in the constraint component.</summary>
        string scaleWeightFloatProperty { get; }
    }

    [Serializable]
    public struct OverrideBonesData : IAnimationJobData, ICopyTransformsData
    {
        [SerializeField] List<CopyTransformDataSingle> m_Transforms;

        public List<CopyTransformDataSingle> Transforms
        {
            get => m_Transforms;
            set => m_Transforms = value;
        }

        [SyncSceneToStream, SerializeField, Range(0f, 1f)]
        float m_PositionWeight;

        [SyncSceneToStream, SerializeField, Range(0f, 1f)]
        float m_RotationWeight;

        [SyncSceneToStream, SerializeField, Range(0f, 1f)]
        float m_ScaleWeight;

        /// <summary>The weight for which override position has an effect on constrained Transform. This is a value in between 0 and 1.</summary>
        public float PositionWeight
        {
            get => m_PositionWeight;
            set => m_PositionWeight = Mathf.Clamp01(value);
        }

        /// <summary>The weight for which override rotation has an effect on constrained Transform. This is a value in between 0 and 1.</summary>
        public float RotationWeight
        {
            get => m_RotationWeight;
            set => m_RotationWeight = Mathf.Clamp01(value);
        }

        /// <summary>The weight for which override scale has an effect on constrained Transform. This is a value in between 0 and 1.</summary>
        public float ScaleWeight
        {
            get => m_ScaleWeight;
            set => m_ScaleWeight = Mathf.Clamp01(value);
        }

        public string positionWeightFloatProperty =>
            ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_PositionWeight));

        public string rotationWeightFloatProperty =>
            ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_RotationWeight));

        public string scaleWeightFloatProperty =>
            ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_ScaleWeight));

        public bool IsValid()
        {
            for (int i = 0; i < m_Transforms.Count; ++i)
            {
                var t = m_Transforms[i];
                if (!t.CopiedTransform || !t.ConstrainedTransform)
                    return false;
            }

            return true;
        }

        public void SetDefaultValues()
        {
            m_Transforms = new List<CopyTransformDataSingle>()
            {
                new()
            };
            PositionWeight = 1f;
            RotationWeight = 1f;
            ScaleWeight = 1f;
        }
    }
}