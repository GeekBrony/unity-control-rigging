using System;
using UnityEngine.Animations.Rigging;

namespace ControlRigging
{
    [Serializable]
    public class EffectorBinding : IRigBinding
    {
        public ArmatureBinding transform;
        public RigEffectorData.Style style;
    }
}