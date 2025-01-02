using UnityEditor;

namespace ControlRigging
{
    public abstract class GenericMenuSelection<T>
    {
        public SerializedProperty Property;
        public T Value;

        protected abstract void SetValue(T value);
            
        public void Set()
        {
            if (Property == null)
                return;
                
            SetValue(Value);
            Apply();
        }
            
        public bool Apply()
        {
            if (Property == null)
                return false;

            return Property.serializedObject.ApplyModifiedProperties();
        }
    }

    public class StringMenuSelection : GenericMenuSelection<string>
    {
        protected override void SetValue(string value)
        {
            Property.stringValue = value;
        }
    }
}