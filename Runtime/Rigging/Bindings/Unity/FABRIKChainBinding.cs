using System;
using UnityEngine;

namespace ControlRigging
{
    [Serializable]
    public class FABRIKChainBinding : IRigBinding
    {
        [Header("Bones")]
        public ArmatureBinding rootBone;
        public ArmatureBinding tipBone;
        [Space]
        public ArmatureBinding targetBone;

        [Header("Configuration")]
        [Range(1,50)]
        public int maxIterations = 15;
        [Range(0, 0.01f)]
        public float tolerance = 0.0001f;
        [Space]
        [Range(0,1)]
        public float chainRotationWeight = 1;
        [Range(0,1)]
        public float tipRotationWeight = 1;
    }
}