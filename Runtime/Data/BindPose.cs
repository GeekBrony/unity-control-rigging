using System;
using Unity.Mathematics;
using UnityEngine;

namespace ControlRigging
{
    [Serializable]
    public struct BindPose
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;

        public BindPose(Transform t)
        {
            localPosition = t.localPosition;
            localRotation = t.localRotation;
            localScale = t.localScale;
        }

        private const double EqualityTolerance = 0.00001;
        public bool Equals(BindPose other)
        {
            return (localPosition - other.localPosition).magnitude < EqualityTolerance && 
                   (localRotation.eulerAngles - other.localRotation.eulerAngles).magnitude < EqualityTolerance && 
                   (localScale - other.localScale).magnitude < EqualityTolerance;
        }

        public override bool Equals(object obj)
        {
            return obj is BindPose other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(localPosition, localRotation, localScale);
        }

        public static bool operator ==(BindPose left, BindPose right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BindPose left, BindPose right)
        {
            return !left.Equals(right);
        }
    }
}