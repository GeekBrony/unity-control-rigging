using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Splines;

namespace ControlRigging.Constraints
{
    [BurstCompile]
    public struct SplineTrackJob : IWeightedAnimationJob
    {
        public ReadWriteTransformHandle ConstrainedTransform;

        [ReadOnly] public NativeSpline Spline;
        public ReadOnlyTransformHandle SplineTransform;

        public Vector3Property PositionOffset;
        public Vector3Property RotationOffset;

        public FloatProperty Time;
        public FloatProperty jobWeight { get; set; }
        public FloatProperty PositionWeight;
        public FloatProperty RotationWeight;

        public void ProcessRootMotion(AnimationStream stream)
        {
        }

        public void ProcessAnimation(AnimationStream stream)
        {
            float weight = jobWeight.Get(stream);
            float posWeight = PositionWeight.Get(stream) * weight;
            float rotWeight = RotationWeight.Get(stream) * weight;

            if (!ConstrainedTransform.IsValid(stream))
                return;

            if (weight <= 0 || !SplineTransform.IsValid(stream))
            {
                AnimationRuntimeUtils.PassThrough(stream, ConstrainedTransform);
                return;
            }

            float time = Time.Get(stream);
            float rt = time.Repeat(1);
            
            if (isDirty)
            {
                AnimationRuntimeUtils.PassThrough(stream, ConstrainedTransform);
                isDirty = false;
                return;
            }
            
            Evaluate(stream, rt, out Matrix4x4 m);

            Vector3 posOffset = PositionOffset.Get(stream);
            Quaternion rotOffset = Quaternion.Euler(RotationOffset.Get(stream));

            Matrix4x4 offsetMatrix = Matrix4x4.TRS(posOffset, rotOffset, Vector3.one);
            m *= offsetMatrix;

            Vector3 currentPos = ConstrainedTransform.GetPosition(stream);
            Quaternion currentRot = ConstrainedTransform.GetRotation(stream);

            Vector3 targetPos = Vector3.Lerp(currentPos, m.GetPosition(), posWeight);
            Quaternion targetRot = Quaternion.Lerp(currentRot, m.rotation * currentRot, rotWeight);

            ConstrainedTransform.SetGlobalTR(stream, targetPos, targetRot);
        }

        private bool Evaluate(AnimationStream stream, float time, out Matrix4x4 matrix)
        {
            bool valid = Spline.Evaluate(time, out float3 position, out float3 tangent, out float3 upVector);
            
            Quaternion rotation = quaternion.LookRotation(tangent, upVector);

            Matrix4x4 m = Matrix4x4.TRS(
                SplineTransform.GetPosition(stream),
                SplineTransform.GetRotation(stream),
                SplineTransform.GetLocalScale(stream)
            );
            Matrix4x4 m2 = Matrix4x4.TRS(position, rotation, Vector3.one);
            matrix = m * m2;

            return valid;
        }

        public bool isDirty;
    }

    /// <summary>
    /// The SplineTrack job binder.
    /// </summary>
    /// <typeparam name="T">The constraint data type</typeparam>
    public class SplineTrackJobBinder<T> : AnimationJobBinder<SplineTrackJob, T>
        where T : struct, IAnimationJobData, ISplineTrackData
    {
        /// <inheritdoc />
        public override SplineTrackJob Create(Animator animator, ref T data, Component component)
        {
            var job = new SplineTrackJob();

            if (data.ConstrainedTransform)
            {
                job.ConstrainedTransform = ReadWriteTransformHandle.Bind(animator, data.ConstrainedTransform);
            }

            if (data.Spline)
            {
                job.Spline = new NativeSpline(data.Spline.Spline, Allocator.Persistent);
                job.SplineTransform = ReadOnlyTransformHandle.Bind(animator, data.Spline.transform);
            }

            job.PositionOffset = Vector3Property.Bind(animator, component, data.PositionOffsetVectorProperty);
            job.RotationOffset = Vector3Property.Bind(animator, component, data.RotationOffsetVectorProperty);

            job.Time = FloatProperty.Bind(animator, component, data.TimeFloatProperty);

            job.PositionWeight = FloatProperty.Bind(animator, component, data.PositionWeightFloatProperty);
            job.RotationWeight = FloatProperty.Bind(animator, component, data.RotationWeightFloatProperty);

            return job;
        }

        /// <inheritdoc />
        public override void Destroy(SplineTrackJob job)
        {
            job.Spline.Dispose();
        }

        /// <inheritdoc />
        public override void Update(SplineTrackJob job, ref T data)
        {
        }
    }
}