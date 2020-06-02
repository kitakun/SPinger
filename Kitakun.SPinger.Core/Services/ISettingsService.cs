namespace Kitakun.SPinger.Core.Services
{
    /// <summary>
    /// Application settings store service
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Load stored data
        /// </summary>
        string[] Load();

        /// <summary>
        /// Save stored data
        /// </summary>
        void Save(string[] data);
    }
}
