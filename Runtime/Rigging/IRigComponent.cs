using UnityEngine;

namespace ControlRigging
{
    public interface IRigComponent
    {
        /// <summary>
        /// Is this component enabled?
        /// </summary>
        bool Enabled { get; set; }
        
        /// <summary>
        /// The name of this component.
        /// </summary>
        string Name { get; set; }
        
        /// <summary>
        /// Function that generates the components necessary on the GameObject.
        /// </summary>
        /// <param name="rig"></param>
        /// <param name="gameObject"></param>
        void Generate(RigRoot rig, GameObject gameObject);
        
        /// <summary>
        /// Function that clears (deletes) the components from the GameObject
        /// </summary>
        /// <param name="rig"></param>
        /// <param name="gameObject"></param>
        void Clear(RigRoot rig, GameObject gameObject);
    }
}