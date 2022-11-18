using ASV.Core.Enums;
using ASV.Core.Helpers;
using System.Reflection;

namespace ASV.Core.Detection.Detectors
{
    internal sealed class EventChangeDetector : IChangeDetector<EventInfo>
    {
        public ChangeLevel DetectChanges(EventInfo current, EventInfo previous)
        {
            return ChangeLevel.None;
        }

        public bool Match(EventInfo left, EventInfo right)
            => DeepReflectionComparer.Compare(left, right);
    }
}
