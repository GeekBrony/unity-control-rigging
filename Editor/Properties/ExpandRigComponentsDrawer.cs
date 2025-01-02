using UnityEditor;
using UnityEngine;

namespace ControlRigging
{
    [CustomPropertyDrawer(typeof(IRigComponent), true)]
    [CustomPropertyDrawer(typeof(IRigBinding), true)]
    [CustomPropertyDrawer(typeof(IRigBinding[]), true)]
    class ExpandRigComponentsDrawer : ExpandChildDrawer
    {
        
    }
}