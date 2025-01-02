using System;
using UnityEngine;

namespace ControlRigging
{
    /// <summary>
    /// The base class to extend for a custom Rig component.
    /// </summary>
    [Serializable]
    public abstract class CustomRigComponent : IRigComponent
    {
        public bool Enabled { get; set; } = true;

        [SerializeField, HideInInspector]
        private string m_Name;
        public string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        public void Generate(RigRoot rig, GameObject gameObject)
        {
            if(!Enabled)
                return;
            
            OnGenerateRig(rig, gameObject);
        }
        
        protected abstract void OnGenerateRig(RigRoot rig, GameObject gameObject);
        
        public void Clear(RigRoot rig, GameObject gameObject)
        {
            if(!Enabled)
                return;
            
            OnDestroyRig(rig, gameObject);
        }
        
        protected abstract void OnDestroyRig(RigRoot rig, GameObject gameObject);
    }
    

}