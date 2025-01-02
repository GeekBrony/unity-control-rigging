using UnityEngine;

namespace ControlRigging
{
    public static class ObjectExtensions
    {
        public static void Destroy(this Object o)
        {
            if(!o) return;
            if (Application.isEditor && !Application.isPlaying)
            {
                Object.DestroyImmediate(o);
            }
            else
            {
                Object.Destroy(o);
            }
        }
        
        public static void Destroy(this Transform t)
        {
            if(!t) return;
            t.gameObject.Destroy();
        }
        
        public static GameObject CreateChild(this GameObject parent, string objectName) 
        {
            GameObject o = new GameObject(objectName);
            o.transform.SetParent(parent.transform, false);
            return o;
        }
        
        public static GameObject CreateChild(this Transform parent, string objectName) 
        {
            GameObject o = new GameObject(objectName);
            o.transform.SetParent(parent, false);
            return o;
        }
    }
}