using System;
using System.Collections.Generic;
using System.Linq;
using ControlRigging.Constraints;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

namespace ControlRigging
{
    [RequireComponent(typeof(ArmatureRoot), typeof(Rig))]
    public class RigRoot : MonoBehaviour
    {
        public Rig rig;
        
        public RigAsset asset;
        public Transform constraintRoot;
        public bool generated;
        
        public bool showDebugMenu;
        public bool hideConstraints = true;

        public RigBuilder rigBuilder;
        public List<Transform> effectorTransforms;
        
        public bool ValidateHierarchy()
        {
            Transform parent = transform.parent;
            var armatureRoots = ArmatureRoot.GetAllArmatureRoots(parent);
            if (!armatureRoots.Any() || parent.GetComponentInParent<ArmatureRoot>() || 
                transform == transform.root)
            {
                Debug.LogError("The rig hierarchy is invalid. Unable to continue.\n\n"+
                               "Verify that all of the following are true:\n" +
                               "\t- The Rig Root is next to an Armature Root in the hierarchy\n"+
                               "\t- The Rig Root is NOT a child of an Armature Root\n" +
                               "\t- The Rig Root is NOT a root transform in the scene\n");
                
                // Unity's Animation system works with a hierarchy underneath an Animator,
                // so invalid hierarchies will not work with Animation Rigging.
                
                // The root of the Rig should be at the same level as the Armature root.
                // In other words, the root should not be a child of an Armature hierarchy,
                // but rather siblings (next) to it.
                
                // example:
                // CharacterPrefab
                //  - Armature.Root
                //      - Bone1
                //          - Bone2
                //              - Bone3
                //  - Rig.Root
                //      - Bone1.Target
                //      - Bone2.Target
                //      - Bone3.Target
                
                // For more information, refer to Unity's Animation Rigging documentation:
                // https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.2/manual/RiggingWorkflow.html
                
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Find or create the RigBuilder component on the parent of the RigRoot.
        /// </summary>
        public bool CreateRigBuilder()
        {
            if(rigBuilder)
                return true;
            
            Transform parent = transform.parent;
            if(!parent)
                return false;
            
            RigBuilder builder = parent.GetComponentInParent<RigBuilder>();
            if (!builder)
                builder = parent.gameObject.AddComponent<RigBuilder>();

            rigBuilder = builder;
            return true;
        }
        
        public void ClearRig()
        {
            if(!generated)
                return;

        #if UNITY_EDITOR
            foreach (var effectorBinding in asset.effectors)
            {
                Transform t = GetBone(effectorBinding.transform);
                if (!t) continue;
                
                if (rig.ContainsEffector(t))
                    rig.RemoveEffector(t);
            }
        #endif
            
            foreach (var bone in asset.transforms)
            {
                Transform t = GetBone(bone);
                if(!t) continue;
                var rt = t.GetComponent<RigTransform>();
                if (!rt) continue;

                rt.Destroy();
            }

            ClearConstraints();

            if (constraintRoot)
                constraintRoot.Destroy();

            generated = false;
        }
        
        public void GenerateRig()
        {
            if(generated)
                return;
            
            // Ensure the hierarchy is valid before generating.
            if(!ValidateHierarchy())
                return;
            
        #if UNITY_EDITOR
            // Only available in editor
            foreach (var effectorBinding in asset.effectors)
            {
                Transform t = GetBone(effectorBinding.transform);
                if (!t) continue;

                if (rig.ContainsEffector(t))
                    rig.RemoveEffector(t);
                
                rig.AddEffector(t, effectorBinding.style);
            }
        #endif

            foreach (var bone in asset.transforms)
            {
                Transform t = GetBone(bone);
                if(!t) continue;
                
                // add rig transform if it doesn't exist yet
                var rt = t.GetComponent<RigTransform>();
                if (!rt)
                    t.gameObject.AddComponent<RigTransform>();
            }
            
            GenerateConstraints();

            // if for whatever reason this fails, display an error.
            if (!CreateRigBuilder())
            {
                Debug.LogError($"Error finding/creating RigBuilder on {transform.name}'s parent.");
                return;
            }

            if (rigBuilder.layers.Count(l => l.rig == rig) == 0)
            {
                rigBuilder.layers.Add(new(rig));
            }

            rigBuilder.layers.RemoveAll(l => !l.rig);
            
            if (rigBuilder.transform != transform.root)
            {
                var parentT = rigBuilder.transform.parent;
                foreach (RigBuilder b in parentT.GetComponentsInParent<RigBuilder>())
                {
                    if(b.layers.Any(l => l.rig == rig))
                        continue;
                    
                    b.layers.Add(new RigLayer(rig));
                }
            }

            generated = true;
        }

        public Dictionary<IRigComponent, GameObject> Components = new Dictionary<IRigComponent, GameObject>();
        void GenerateConstraints()
        {
            EnsureConstraintRootCreated();
            
            // Iterate through all the components in the RigAsset
            foreach (var component in asset.components)
            {
                // If this component isn't enabled, go to next component
                if(!component.Enabled)
                    continue;

                // Get the name of the component, fallback to the type name
                string componentName = component.Name;
                if (string.IsNullOrWhiteSpace(componentName))
                    componentName = component.GetType().Name;
                
                // Create a new child GameObject in the constraint root GameObject.
                var go = CreateChildObject(constraintRoot, componentName);
                
                // Tell the component to generate on this object.
                component.Generate(this, go);
                
                // Add the component to the list
                Components.Add(component, go);
            }
        }

        void ClearConstraints()
        {
            // Iterate through all currently generated components.
            var components = Components.ToArray();
            foreach (var pair in components)
            {
                IRigComponent component = pair.Key;
                GameObject go = pair.Value;
                if(!go)
                    continue;
                
                // Tell the component to clean up this object.
                component.Clear(this, go);

                // Remove the component from the list
                Components.Remove(component);
            }
            
            // Clear the generated components list
            Components.Clear();
        }

        public ArmatureRoot GetArmatureInHierarchy(ArmatureAsset asset)
        {
            if (asset == null)
                return null;
            
            var parent = transform.parent;
            var armatureRoots = parent.GetComponentsInChildren<ArmatureRoot>();
            foreach (var root in armatureRoots)
            {
                if (root.armatureAsset == asset || root.armatureAsset.IsChildOf(asset))
                    return root;
            }

            return null;
        }
        
        public Transform GetBone(ArmatureBinding binding)
        {
            ArmatureRoot armatureRoot = GetArmatureInHierarchy(binding.armature);
            if (!armatureRoot)
                return null;
            
            return armatureRoot.GetBoneTransform(binding.boneName);
        }

        private const string ConstraintRootTransformName = "_Constraints_Auto";
        void EnsureConstraintRootCreated()
        {
            if (constraintRoot)
                return;

            constraintRoot = CreateChildObject(transform, ConstraintRootTransformName).transform;
            constraintRoot.SetAsFirstSibling();
            
            constraintRoot.gameObject.hideFlags = !hideConstraints ? 
                HideFlags.None : HideFlags.HideInHierarchy;
        }

        private GameObject CreateChildObject(Transform parent, string objectName) 
        {
            GameObject o = new GameObject(objectName);
            o.transform.SetParent(parent, false);
            return o;
        }

        private void OnValidate()
        {
            if (!rig)
            {
                rig = GetComponent<Rig>();
            }
            
            if (constraintRoot)
            {
                constraintRoot.gameObject.hideFlags = !hideConstraints ? 
                    HideFlags.None : HideFlags.HideInHierarchy;
            }
        }

        /// <summary>
        /// Iterate through the given list of ArmatureRoots and find all the RigRoot components.
        /// </summary>
        /// <param name="armatures">An IEnumerable of ArmatureRoot</param>
        /// <returns>An IEnumerable of RigRoot</returns>
        public static IEnumerable<RigRoot> GetAllRigRoots(IEnumerable<ArmatureRoot> armatures)
        {
            foreach (var armature in armatures)
            {
                var rigRoot = armature.GetComponent<RigRoot>();
                if (rigRoot)
                    yield return rigRoot;
            }
        }
    }
}