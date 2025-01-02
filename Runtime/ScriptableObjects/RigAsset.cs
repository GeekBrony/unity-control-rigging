﻿using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ControlRigging
{
    [CreateAssetMenu(menuName = "Rigging/Rig Asset")]
    public class RigAsset : ScriptableObject
    {
        [SerializeReference]
        public IRigComponent[] Components;
        
        public EffectorBinding[] effectors;
        
        public ArmatureBinding[] transforms;
    }
}