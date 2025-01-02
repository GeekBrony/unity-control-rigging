using System;

namespace ControlRigging
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RigComponentMenuAttribute : Attribute
    {
        public string ComponentName;
        
        public RigComponentMenuAttribute(string name)
        {
            this.ComponentName = name;
        }

        public static RigComponentMenuAttribute GetAttribute(Type type)
        {
            return (RigComponentMenuAttribute) GetCustomAttribute(type, typeof(RigComponentMenuAttribute));
        }
        
        public static string GetMenuName(Type type)
        {
            return GetAttribute(type)?.ComponentName;
        }
    }
}