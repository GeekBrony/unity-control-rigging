using UnityEngine;

namespace ControlRigging
{
    /// <summary>
    /// Axis type for constraints.
    /// </summary>
    public enum Axis
    {
        /// <summary>Positive X Axis (1, 0, 0)</summary>
        [InspectorName("+X")] X,
        /// <summary>Negative X Axis (-1, 0, 0)</summary>
        [InspectorName("-X")] XNegative,
        /// <summary>Positive Y Axis (0, 1, 0)</summary>
        [InspectorName("+Y")] Y,
        /// <summary>Negative Y Axis (0, -1, 0)</summary>
        [InspectorName("-Y")] YNegative,
        /// <summary>Positive Z Axis (0, 0, 1)</summary>
        [InspectorName("+Z")] Z,
        /// <summary>Negative Z Axis (0, 0, -1)</summary>
        [InspectorName("-Z")] ZNegative
    }

    public static class AxisUtility
    {
        public static string ToFormattedString(this Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    return "+X";
                case Axis.XNegative:
                    return "-X";
                case Axis.Y:
                    return "+Y";
                case Axis.YNegative:
                    return "-Y";
                case Axis.Z:
                    return "+Z";
                case Axis.ZNegative:
                    return "-Z";
            }

            return ((int) axis).ToString();
        }
        
        public static Vector3 ToDirection(this Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    return new Vector3(1, 0, 0);
                case Axis.XNegative:
                    return new Vector3(-1, 0, 0);
                case Axis.Y:
                    return new Vector3(0, 1, 0);
                case Axis.YNegative:
                    return new Vector3(0, -1, 0);
                case Axis.Z:
                    return new Vector3(0, 0, 1);
                case Axis.ZNegative:
                    return new Vector3(0, 0, -1);
            }

            return Vector3.zero;
        }

        public static Vector3 ToLocalDirection(this Axis axis, Transform transform)
        {
            return transform.TransformDirection(axis.ToDirection());
        }
    }
}