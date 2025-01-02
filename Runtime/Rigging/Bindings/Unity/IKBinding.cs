using System;
using UnityEngine;

namespace ControlRigging
{
    [Serializable]
    public class IKBinding : IRigBinding
    {
        [Header("Constrained Bones")]
        public ArmatureBinding rootBone;
        public ArmatureBinding midBone;
        public ArmatureBinding tipBone;
            
        [Header("Target Bones")]
        public ArmatureBinding targetBone;
        public ArmatureBinding hintBone;
    }
}