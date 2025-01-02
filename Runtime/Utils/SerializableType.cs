using System;
using UnityEngine;

namespace ControlRigging.Utils
{
    /// <summary>
    /// A serializable System.Type
    /// </summary>
    [Serializable]
    public class SerializableType
    {
        [SerializeField]
        private string m_Name;
        public string Name => m_Name;

        [SerializeField]
        private string m_AssemblyQualifiedName;
        public string AssemblyQualifiedName => m_AssemblyQualifiedName;

        [SerializeField]
        private string m_AssemblyName;
        public string AssemblyName => m_AssemblyName;

        private Type m_Type;
        public Type Type
        {
            get
            {
                if (m_Type == null)
                    m_Type = Type.GetType(m_AssemblyQualifiedName);
                        
                return m_Type;
            }
        }

        public SerializableType(Type type)
        {
            m_Type = type;
            m_Name = type.Name;
            m_AssemblyQualifiedName = type.AssemblyQualifiedName;
            m_AssemblyName = type.Assembly.FullName;
        }
    }
}