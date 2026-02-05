using System.Collections.Generic;

namespace ControlRigging
{
    public interface IArmatureProvider
    {
        public IEnumerable<ArmatureRoot> GetArmatures();
    }
}