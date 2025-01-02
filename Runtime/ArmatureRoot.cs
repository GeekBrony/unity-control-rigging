using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ControlRigging;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ArmatureRoot : MonoBehaviour
{
    public ArmatureAsset armatureAsset;

    #region Mapped Bones

    [SerializeField]
    List<MappedBone> m_MappedBones = new List<MappedBone>();
    
    public MappedBone GetMappedBone(string boneName)
    {
        var mappedBone = m_MappedBones
            .FirstOrDefault(b => b.HasName(boneName));
        
        return mappedBone;
    }
    
    public void SetMappedBone(string boneName, Transform t)
    {
        MappedBone bone = GetMappedBone(boneName);
        if (bone != null)
        {
            bone.transform = t;
            return;
        }
        
        m_MappedBones.Add(new MappedBone()
        {
            boneName = boneName,
            transform = t
        });
    }

    #endregion
    

    private void Awake()
    {
    }

    [SerializeField, HideInInspector]
    private ArmatureAsset m_ArmatureAsset;
    private void OnValidate()
    {
        if (!armatureAsset || m_ArmatureAsset != armatureAsset)
        {
            m_MappedBones.Clear();
        }
        m_ArmatureAsset = armatureAsset;
        
        ResolveBoneReferences();
    }

    public override string ToString()
    {
        string armatureName = "null";
        if (armatureAsset)
            armatureName = armatureAsset.name;
        
        return $"ArmatureRoot ({armatureName})";
    }
    
    void ResolveBoneReferences()
    {
        if (!armatureAsset)
            return;
        
        // Recurse into the transform hierarchy only once.
        Transform[] transforms = transform.Recurse().ToArray();

        int fullArmatureBoneCount = 0;
        int count = 0;
        
        // Iterate through all the bones in this armature asset
        foreach (var bone in armatureAsset.Bones)
        {
            fullArmatureBoneCount++;
            
            MappedBone mappedBone = GetMappedBone(bone.name);
            if (mappedBone != null && mappedBone.Populated)
                continue;
            
            Transform boneTransform = transforms.FirstOrDefault(t => t.name.Equals(bone.name));
            if (!boneTransform)
                continue;
            
            SetMappedBone(bone.name, boneTransform);
            count++;
        }
        
        /*// Iterate through all the bones in this armature asset
        foreach (var bone in armatureAsset.Bones)
        {
            bool valueExists = m_MappedBones.TryGetValue(bone.name, out Transform boneTransform);
            if (valueExists && boneTransform) continue;
            
            boneTransform = transforms.FirstOrDefault(t => t.name.Equals(bone.name));
            m_MappedBones[bone.name] = boneTransform;
            count++;
        }*/

        if (count > 0)
            Debug.Log($"{ToString()} - Resolved {count} bones ({fullArmatureBoneCount} total in armature).");
        
    }

    public Transform GetBoneTransform(string boneName)
    {
        // Find bone in dictionary
        MappedBone mappedBone = GetMappedBone(boneName);
        if (mappedBone != null && mappedBone.Populated)
            return mappedBone.transform;

        // Can't find in dictionary, search for bone in armature
        if (TryFindBone(boneName, out Transform bone))
        {
            // Found bone, save it to dictionary.
            SetMappedBone(boneName, bone);
            return bone;
        }
        
        // Does not exist in armature at all
        return null;
    }

    public bool TryFindBone(string boneName, out Transform boneTransform)
    {
        boneTransform = transform.Recurse()
            .FirstOrDefault(t => t.name.Equals(boneName));
        return boneTransform != null;
    }
    
    public bool GetBindPos(Transform t, out BindPose bindPose)
    {
        bindPose = new BindPose();
        
        Bone b = armatureAsset.GetBone(t.name);
        if (b == null) 
            return false;

        bindPose = b.bindPos;
        return true;
    }
    
    public void SetToBindPos()
    {
        if (!armatureAsset)
        {
            Debug.LogError("No bind pos. Did you forget to add / create an armature asset?");
            return;
        }
        
        foreach (Transform t in transform.Recurse())
            SetToBindPos(t);
    }
    
    /// <summary>
    /// Set an armature's transform to its bind pose
    /// </summary>
    public void SetToBindPos(Transform t, bool setPosition = true, bool setRotation = true, bool setScale = true)
    {
        if (!t.IsChildOf(transform))
            return;
        
        bool boneFound = GetBindPos(t, out BindPose bindPose);
        if (!boneFound)
            return;
        
#if UNITY_EDITOR
        SerializedObject serializedObject = new SerializedObject(t);
        serializedObject.Update();
        
        if (setPosition)
            serializedObject.FindProperty("m_LocalPosition").vector3Value = bindPose.localPosition;
        
        if (setRotation)
            serializedObject.FindProperty("m_LocalRotation").quaternionValue = bindPose.localRotation;
        
        if (setScale)
            serializedObject.FindProperty("m_LocalScale").vector3Value = bindPose.localScale;
        
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        serializedObject.Dispose();
#endif
        
        if (setPosition)
            t.localPosition = bindPose.localPosition;
        
        if (setRotation)
            t.localRotation = bindPose.localRotation;
        
        if (setScale)
            t.localScale = bindPose.localScale;
    }

#if UNITY_EDITOR
    [MenuItem("CONTEXT/Transform/Set To Bind Pos/All", true)]
    [MenuItem("CONTEXT/Transform/Set To Bind Pos/Position", true)]
    [MenuItem("CONTEXT/Transform/Set To Bind Pos/Rotation", true)]
    [MenuItem("CONTEXT/Transform/Set To Bind Pos/Scale", true)]
    public static bool ValidateSetTransformToBind(MenuCommand command)
    {
        return ((Transform)command.context).GetComponentInParent<ArmatureRoot>();
    }
    
    [MenuItem("CONTEXT/Transform/Set To Bind Pos/All")]
    public static void SetTransformToBind(MenuCommand command)
    {
        Transform t = ((Transform)command.context);
        ArmatureRoot root = t.GetComponentInParent<ArmatureRoot>();
        root.SetToBindPos(t);
    }
    
    [MenuItem("CONTEXT/Transform/Set To Bind Pos/Position")]
    public static void SetTransformToBind_Position(MenuCommand command)
    {
        Transform t = ((Transform)command.context);
        ArmatureRoot root = t.GetComponentInParent<ArmatureRoot>();
        root.SetToBindPos(t, true, false, false);
    }
    
    [MenuItem("CONTEXT/Transform/Set To Bind Pos/Rotation")]
    public static void SetTransformToBind_Rotation(MenuCommand command)
    {
        Transform t = ((Transform)command.context);
        ArmatureRoot root = t.GetComponentInParent<ArmatureRoot>();
        root.SetToBindPos(t, false, true, false);
    }
    
    [MenuItem("CONTEXT/Transform/Set To Bind Pos/Scale")]
    public static void SetTransformToBind_Scale(MenuCommand command)
    {
        Transform t = ((Transform)command.context);
        ArmatureRoot root = t.GetComponentInParent<ArmatureRoot>();
        root.SetToBindPos(t, false, false, true);
    }
#endif
    
    public static IEnumerable<ArmatureRoot> GetAllArmatureRoots(Transform t)
    {
        return !t ? Array.Empty<ArmatureRoot>() : t.GetComponentsInChildren<ArmatureRoot>();
    }
    
    public static ArmatureRoot GetArmature(Transform t, ArmatureAsset armatureAsset, bool includeInactive = false)
    {
        if (!t)
            return null; 
        
        return t.GetComponentsInChildren<ArmatureRoot>(includeInactive)
            .FirstOrDefault(r => r.armatureAsset == armatureAsset);
    }
}