namespace ASV.Core.Enums
{
    public enum ChangeLevel
    {
        None = 0,
        Patch = 1,
        Minor = 2,
        Major = 3
    }

    public static class ChangeTypeExtensions
    {
        public static ChangeLevel TryChange(this ChangeLevel currentLevel, ChangeLevel newLevel)
        {
            return currentLevel >= newLevel ? currentLevel : newLevel;
        }
    }
}
