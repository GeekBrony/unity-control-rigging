using System;

namespace ControlRigging
{
    [AttributeUsage(validOn: AttributeTargets.Class 
                             | AttributeTargets.Field 
                             | AttributeTargets.Interface 
                             | AttributeTargets.Struct)]
    public class ExpandChildAttribute : Attribute { }
}