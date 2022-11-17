using ASV.Core.Enums;
using System.Reflection;

namespace ASV.Core.Detection.Detectors
{
    internal sealed class EventChangeDetector : IChangeDetector<EventInfo>
    {
        public ChangeLevel DetectChanges(EventInfo current, EventInfo original)
        {
            return ChangeLevel.None;
        }

        public bool Match(EventInfo left, EventInfo right)
        {
            return left.Name == right.Name;
        }
    }
}
