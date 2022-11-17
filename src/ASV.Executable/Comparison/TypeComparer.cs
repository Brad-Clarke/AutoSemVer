using System;
using System.Linq;
using ASV.Core.Enums;

namespace ASV.Executable.Comparison
{
    public static class TypeComparer
    {
        public static ChangeLevel Compare(Type currentType, Type previousType)
        {
            return CompareMethods(currentType, previousType);
        }

        private static ChangeLevel CompareMethods(Type currentType, Type previousType)
        {
            var current = currentType.GetMethods();
            var previous = previousType.GetMethods();

            var removed = previous.Where(c => current.All(p => p.Name != c.Name)).ToList();
            var existing = current.Where(c => previous.Any(p => p.Name == c.Name)).ToList();
            var added = current.Where(c => previous.All(p => p.Name != c.Name)).ToList();

            //if (removed.Any())
            //{
            //    return ChangeLevel.Breaking;
            //}



            return ChangeLevel.None;
        }

        private static ChangeLevel CompareConstructors()
        {
            return ChangeLevel.None;
        }

        private static ChangeLevel CompareProperties()
        {
            return ChangeLevel.None;
        }

        private static ChangeLevel CompareFields()
        {
            return ChangeLevel.None;
        }

        private static ChangeLevel CompareEvents()
        {
            return ChangeLevel.None;
        }

        private static ChangeLevel CompareAttributes()
        {
            return ChangeLevel.None;
        }

        private static ChangeLevel CompareNestedTypes()
        {
            return ChangeLevel.None;
        }
    }
}
