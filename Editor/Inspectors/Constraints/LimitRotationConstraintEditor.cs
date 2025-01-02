using System;
using System.Collections.Generic;
using System.Linq;
using ControlRigging.Constraints;
using UnityEditor;
using UnityEngine;

namespace ControlRigging
{
    [CustomEditor(typeof(LimitRotationConstraint)), CanEditMultipleObjects]
    public class LimitRotationConstraintEditor : Editor
    {
        public LimitRotationConstraint Constraint => (LimitRotationConstraint) target;
        public IEnumerable<LimitRotationConstraint> Constraints => targets.Cast<LimitRotationConstraint>();

        private void OnSceneGUI()
        {
            DrawGizmos(Constraint);
        }

        private const float HandleDistance = 0.5f;

        void DrawGizmos(LimitRotationConstraint constraint)
        {
            var data = constraint.data;
            if(!data.ConstrainedTransform)
                return;
            
            Vector3 axis = data.ConstrainedAxis.ToDirection();
            Transform bone = data.ConstrainedTransform;
            Vector3 bonePos = bone.position;
            float limit = data.Limit;
            
            Vector3 worldAxis = bone.TransformDirection(axis);
            Vector3 crossAxis = bone.TransformDirection(new Vector3(axis.z, axis.x, axis.y));
            
            Color originalHandleColor = Handles.color;
            
            // Draw the current rotation at this axis.
            Handles.color = Color.green;
            Handles.DrawLine(bonePos, bonePos + (worldAxis * HandleDistance));
            
            // Draw the minimum rotation at this axis.
            Handles.color = Color.red;
            Vector3 minDir = Quaternion.AngleAxis(-limit, crossAxis) * worldAxis;
            Handles.DrawLine(bonePos, bonePos + (minDir * HandleDistance));

            Vector3 lastPos = bonePos + (minDir * HandleDistance);

            // in between minimum and maximum, draw every 1 degree
            Handles.color = new Color(0,1,0,0.2f);
            for (int i = (int) -limit + 1; i < limit; i += 1)
            {
                // draw lighter green every 15 degrees
                if (i % 15 == 0)
                    Handles.color = Color.green;
                else
                    Handles.color = new Color(0,1,0,0.2f);
                
                Vector3 innerDir = Quaternion.AngleAxis(i, crossAxis) * worldAxis;
                Vector3 curPos = bonePos + (innerDir * HandleDistance);
                
                Handles.DrawLine(bonePos, curPos);
                
                Handles.color = Color.green;
                Handles.DrawLine(lastPos, curPos);

                lastPos = curPos;
            }
            
            // Draw the maximum rotation at this axis.
            Handles.color = Color.red;
            Vector3 maxDir = Quaternion.AngleAxis(limit, crossAxis) * worldAxis;
            Handles.DrawLine(bonePos, bonePos + (maxDir * HandleDistance));
            Handles.color = Color.green;
            Handles.DrawLine(lastPos, bonePos + (maxDir * HandleDistance));

            Handles.color = originalHandleColor;
        }
    }
}