using System;
using System.Collections;
using System.Collections.Generic;
using ControlRigging;
using UnityEngine;

[CreateAssetMenu(menuName= "Rigging/Armature Asset", fileName = "Armature")]
public class ArmatureAsset : ScriptableObject
{
    [SerializeField]
    ArmatureAsset m_Parent;
    public ArmatureAsset Parent
    {
        get => m_Parent;
        set => m_Parent = value;
    }

    public ArmatureAsset GetParent()
    {
        if (m_Parent == this)
            return null;
        
        return m_Parent;
    }
    
    public IEnumerable<ArmatureAsset> GetParentsRecursive()
    {
        ArmatureAsset nextParent = GetParent();
        yield return nextParent;
        
        while (nextParent && nextParent.m_Parent)
        {
            nextParent = nextParent.GetParent();
            if(nextParent)
                yield return nextParent;
        }
    }
    
    [SerializeField]
    Bone[] m_Bones;
    public Bone[] Bones { 
        get => m_Bones;
        set => m_Bones = value;
    }
    
    public IEnumerable<Bone> GetBonesRecursive()
    {
        // Enumerate over all bones that are in this armature.
        foreach (var b in m_Bones)
            yield return b;
        
        // Recurse to the parent armature, if exists
        var parent = GetParent();
        if (!parent) yield break;
        
        foreach (var b in parent.GetBonesRecursive())
            yield return b;
    }
    
    public Bone GetBone(string boneName)
    {
        // Try to find the bone in current armature first
        foreach (var b in m_Bones)
        {
            if (!b.name.Equals(boneName))
                continue;

            return b;
        }
        
        // If the bone does not exist in the current armature,
        // recurse to an existing parent armature
        var parent = GetParent();
        if (parent)
            return parent.GetBone(boneName);
        
        // Nothing found
        return null;
    }

    public bool IsChildOf(ArmatureAsset asset)
    {
        if (asset == this)
            return false;
        
        foreach (var nextParent in GetParentsRecursive())
        {
            if(nextParent == asset)
                return true;
        }

        return false;
    }
    
    public ArmatureAsset GetRootAsset()
    {
        ArmatureAsset nextParent = GetParent();
        while (nextParent != null && nextParent.m_Parent)
             nextParent = nextParent.GetParent();
        
        return nextParent != null ? nextParent : this;
    }

    public static ArmatureAsset Create(Transform transform, bool includeRoot = false)
    {
        ArmatureAsset asset = CreateInstance<ArmatureAsset>();
        asset.Import(transform, includeRoot);
        asset.name = $"Armature_{transform.name}";
        return asset;
    }
    
    public void Import(Transform source, bool includeRoot = false)
    {
        if (!source)
            return;
        
        var parent = GetParent();
        List<Bone> bones = new List<Bone>();
        foreach (Transform t in source.Recurse(includeRoot))
        {
            Bone bone = new Bone(t);
            
            if (parent)
            {
                Bone parentBone = parent.GetBone(t.name);
                if (parentBone != null)
                {
                    if(parentBone.name.Equals(bone.name) && parentBone.bindPos == bone.bindPos)
                        continue;
                }
            }
            
            bones.Add(bone);
        }

        m_Bones = bones.ToArray();
    }
}