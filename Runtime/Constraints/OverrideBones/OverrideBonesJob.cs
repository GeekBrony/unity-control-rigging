using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

namespace ControlRigging.Constraints
{
    public struct CopyTransformHandle
    {
        /// <summary>The Transform handle for the constrained object Transform.</summary>
        public ReadWriteTransformHandle ConstrainedHandle;

        /// <summary>The Transform handle for the override source object Transform.</summary>
        public ReadOnlyTransformHandle CopiedHandle;
    }

    [BurstCompile]
    public struct OverrideBonesJob : IWeightedAnimationJob
    {
        public NativeArray<CopyTransformHandle> Handles;

        public FloatProperty jobWeight { get; set; }
        public FloatProperty PositionWeight;
        public FloatProperty RotationWeight;
        public FloatProperty ScaleWeight;

        // no root motion necessary
        public void ProcessRootMotion(AnimationStream stream)
        {
        }

        public void ProcessAnimation(AnimationStream stream)
        {
            float weight = jobWeight.Get(stream);
            float posWeight = PositionWeight.Get(stream) * weight;
            float rotWeight = RotationWeight.Get(stream) * weight;
            float scaleWeight = ScaleWeight.Get(stream) * weight;

            var numHandles = Handles.Length;
            for (var i = 0; i < numHandles; ++i)
            {
                var handle = Handles[i];

                var constrainedHandle = handle.ConstrainedHandle;
                if (!constrainedHandle.IsValid(stream))
                    continue;

                if (weight <= 0)
                {
                    AnimationRuntimeUtils.PassThrough(stream, constrainedHandle);
                    continue;
                }

                var trackedHandle = handle.CopiedHandle;
                if (!trackedHandle.IsValid(stream))
                    continue;

                Vector3 currentPos = constrainedHandle.GetPosition(stream);
                Quaternion currentRot = constrainedHandle.GetRotation(stream);
                Vector3 currentScale = constrainedHandle.GetLocalScale(stream);

                Vector3 copiedPos = trackedHandle.GetPosition(stream);
                Quaternion copiedRot = trackedHandle.GetRotation(stream);
                Vector3 copiedScale = trackedHandle.GetLocalScale(stream);

                Vector3 targetPos = Vector3.Lerp(currentPos, copiedPos, posWeight);
                Quaternion targetRot = Quaternion.Lerp(currentRot, copiedRot, rotWeight);
                Vector3 targetScale = Vector3.Lerp(currentScale, copiedScale, scaleWeight);

                constrainedHandle.SetGlobalTR(stream, targetPos, targetRot);
                
                // LIMITATION: Scale still scales the child transforms, need to
                constrainedHandle.SetLocalScale(stream, targetScale);
            }
        }
    }

    /// <summary>
    /// The CopyTransforms job binder.
    /// </summary>
    /// <typeparam name="T">The constraint data type</typeparam>
    public class OverrideBonesJobBinder<T> : AnimationJobBinder<OverrideBonesJob, T>
        where T : struct, IAnimationJobData, ICopyTransformsData
    {
        /// <inheritdoc />
        public override OverrideBonesJob Create(Animator animator, ref T data, Component component)
        {
            var job = new OverrideBonesJob();
            //var cacheBuilder = new AnimationJobCacheBuilder();

            int length = data.Transforms.Count;
            job.Handles = new NativeArray<CopyTransformHandle>(length, Allocator.Persistent);

            for (int i = 0; i < job.Handles.Length; ++i)
            {
                var handle = new CopyTransformHandle();

                CopyTransformDataSingle tData = data.Transforms[i];
                handle.ConstrainedHandle = ReadWriteTransformHandle.Bind(animator, tData.ConstrainedTransform);
                if (tData.CopiedTransform != null)
                    handle.CopiedHandle = ReadOnlyTransformHandle.Bind(animator, tData.CopiedTransform);

                job.Handles[i] = handle;
            }

            job.PositionWeight = FloatProperty.Bind(animator, component, data.positionWeightFloatProperty);
            job.RotationWeight = FloatProperty.Bind(animator, component, data.rotationWeightFloatProperty);
            job.ScaleWeight = FloatProperty.Bind(animator, component, data.scaleWeightFloatProperty);

            return job;
        }

        /// <inheritdoc />
        public override void Destroy(OverrideBonesJob job)
        {
            job.Handles.Dispose();
        }

        /// <inheritdoc />
        public override void Update(OverrideBonesJob job, ref T data)
        {
        }
    }
}