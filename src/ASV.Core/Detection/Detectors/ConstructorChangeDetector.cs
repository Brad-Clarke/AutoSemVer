﻿using ASV.Core.Detection.Factory;
using ASV.Core.Enums;
using ASV.Core.Extensions;
using ASV.Core.Helpers;
using ASV.Core.Tracking;
using DeltaWare.SDK.Core.Helpers;
using System.Reflection;

namespace ASV.Core.Detection.Detectors
{
    internal sealed class ConstructorChangeDetector : IChangeDetector<ConstructorInfo>
    {
        private readonly IChangeTracker _changeTracker;

        private readonly IChangeDetector<ParameterInfo> _parameterChangeDetector;

        public ConstructorChangeDetector(IChangeTracker changeTracker, IChangeDetectorFactory changeDetectorFactory)
        {
            _changeTracker = changeTracker;

            _parameterChangeDetector = changeDetectorFactory.Build<ParameterInfo>();
        }

        public ChangeLevel DetectChanges(ConstructorInfo current, ConstructorInfo previous)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            changeLevel = changeLevel.TryChange(CompareAttributes(current, previous));
            changeLevel = changeLevel.TryChange(CompareParameters(current, previous));

            return changeLevel;
        }

        public bool Match(ConstructorInfo left, ConstructorInfo right)
            => DeepReflectionComparer.Compare(left, right);

        private ChangeLevel CompareAttributes(ConstructorInfo current, ConstructorInfo original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetCustomAttributes()?.ToArray() ?? Array.Empty<Attribute>(), original.GetCustomAttributes()?.ToArray() ?? Array.Empty<Attribute>())
                .OnCompare((left, right) => left.GetType().ToFriendlyName() == right.GetType().ToFriendlyName())
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Constructor {original.ToFriendlyName()} had the Attribute {removed.GetType().ToFriendlyName()} Removed.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(current.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Field {current.ToFriendlyName()} had the Attribute {added.GetType().ToFriendlyName()} Added.", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(current.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }

        private ChangeLevel CompareParameters(ConstructorInfo current, ConstructorInfo original)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            CollectionHelper.Compare(current.GetParameters(), original.GetParameters())
                .OnCompare((left, right) => _parameterChangeDetector.Match(left, right))
                .ForEachRemoved(removed =>
                {
                    _changeTracker.Track($"Constructor {current.ToFriendlyName()} had the Parameter Removed {removed.GetType().ToFriendlyName()}.", ChangeType.Removal);

                    changeLevel = changeLevel.TryChange(current.IsPublic ? ChangeLevel.Major : ChangeLevel.Patch);
                })
                .ForEachExisting((currentExisting, originalExisting) =>
                {
                    ChangeLevel newLevel = _parameterChangeDetector.DetectChanges(currentExisting, originalExisting);

                    changeLevel = changeLevel.TryChange(newLevel);
                })
                .ForEachAdded(added =>
                {
                    _changeTracker.Track($"Constructor {original.ToFriendlyName()} had the Parameter Added {added.GetType().ToFriendlyName()}.", ChangeType.Addition);

                    changeLevel = changeLevel.TryChange(current.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch);
                });

            return changeLevel;
        }
    }
}
