using System;
using UnityEngine;

namespace ControlRigging
{
    [Serializable]
    public struct CachedBinding
    {
        public ArmatureBinding binding;
        
        private ArmatureRoot _cachedArmature;
        public ArmatureRoot Root => _cachedArmature;

        public void SetToBindPos()
        {
            if(!IsArmatureBound())
                return;
            
            Root.SetToBindPos(Transform);
        }

        public bool IsArmatureBound()
        {
            return Root &&
                   Root.armatureAsset == binding.armature;
        }
        
        public bool Validate(Transform root)
        {
            if (!IsArmatureBound())
                return Bind(root);

            return true;
        }
        
        public bool Bind(Transform root)
        {
            if (!binding.IsValid)
                return false;
            
            var armature = ArmatureRoot.GetArmature(root, binding.armature);
            if (!armature) return false;
                    
            _cachedArmature = armature;
            return true;
        }
        
        public Transform Transform
        {
            get
            {
                if (!IsArmatureBound())
                    return null;
            
                return _cachedArmature.GetBoneTransform(binding.boneName);
            }
        }
    }
}