using System;

namespace ControlRigging
{
    [Serializable]
    public struct ArmatureBinding
    {
        public ArmatureAsset armature;
        public string boneName;
        
        public Bone Bone
        {
            get
            {
                if (!armature)
                    return null;
                
                return armature.GetBone(boneName);
            }
        }
        
        public bool IsValid => Bone != null;
    }
}