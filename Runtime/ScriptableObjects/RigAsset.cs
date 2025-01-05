using UnityEngine;
using UnityEngine.Serialization;

namespace ControlRigging
{
    [CreateAssetMenu(menuName = "Rigging/Rig Asset")]
    public class RigAsset : ScriptableObject
    {
        [FormerlySerializedAs("Components")]
        [SerializeReference]
        public IRigComponent[] components;
        public EffectorBinding[] effectors;
        public ArmatureBinding[] transforms;
    }
}