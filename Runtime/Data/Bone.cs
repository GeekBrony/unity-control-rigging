using System;
using System.Collections.Generic;
using UnityEngine;

namespace ControlRigging
{
    [Serializable]
    public class Bone
    {
        public string name;
        public BindPose bindPos;

        public Bone(Transform transform)
        {
            this.name = transform.name;
            this.bindPos = new BindPose(transform);
        }
    }
}