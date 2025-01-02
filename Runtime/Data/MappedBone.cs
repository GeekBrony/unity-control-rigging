using System;
using UnityEngine;

namespace ControlRigging
{
    [Serializable]
    public class MappedBone
    {
        public string boneName;
        public Transform transform;
        
        public bool HasName(string s) => boneName.Equals(s);
        public bool Populated => transform != null;
    }
}