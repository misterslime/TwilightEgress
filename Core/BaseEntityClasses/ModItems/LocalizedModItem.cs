namespace Cascade.Core.BaseEntityClasses.ModItems
{
    /// <summary>
    /// A simple abstract class, created for the purpose of not having to add <see cref="ILocalizedModType"/> to every item we add.
    /// This class simply inherits that interface and allows you to create <see cref="ModItem"/> classes as normal.
    /// </summary>
    public abstract class LocalizedModItem : ModItem, ILocalizedModType
    {
        public virtual string LocalizationPath => "";

        public new string LocalizationCategory => LocalizationPath;
    }
}
