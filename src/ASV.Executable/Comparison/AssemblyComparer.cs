using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ASV.Core.Enums;

namespace ASV.Executable.Comparison
{
    public static partial class AssemblyComparer
    {
        public static ChangeLevel Compare(Assembly currentAssembly, Assembly previousAssembly)
        {
            Type[] currentTypes = currentAssembly.GetLoadedTypes();
            Type[] previousTypes = previousAssembly.GetLoadedTypes();

            List<Type> removed = previousTypes.Where(c => currentTypes.All(p => p.Name != c.Name)).ToList();

            if (removed.Any(t => t.IsPublic))
            {
                return ChangeLevel.Major;
            }

            ChangeLevel changeLevel = ChangeLevel.None;

            List<Type> existing = currentTypes.Where(c => previousTypes.Any(p => p.Name == c.Name)).ToList();

            foreach (Type type in existing)
            {
                ChangeLevel newLevel = TypeComparer.Compare(type, existing.Single(t => t.Name == type.Name));

                changeLevel = changeLevel.TryChange(newLevel);
            }

            List<Type> added = currentTypes.Where(c => previousTypes.All(p => p.Name != c.Name)).ToList();

            if (added.Any(t => t.IsPublic))
            {
                changeLevel = changeLevel.TryChange(ChangeLevel.Minor);
            }
            else if(added.Any(t => !t.IsPublic))
            {
                changeLevel = changeLevel.TryChange(ChangeLevel.Patch);
            }

            return changeLevel;
        }
    }
}
