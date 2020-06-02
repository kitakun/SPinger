namespace Kitakun.SPinger.Core.Services
{
    using System;
    using System.Collections.ObjectModel;

    using Kitakun.SPinger.Core.Models;

    public interface IPingService : IDisposable
    {
        event OnServerStateChanges OnServerChanged;

        /// <summary>
        /// Start watching web addresses for availability
        /// </summary>
        /// <param name="elements">Server collections</param>
        void Watch(Collection<PingerTargetModel> elements);
    }
}
