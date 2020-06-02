namespace Kitakun.SPinger.Core.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class PingerTargetModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Is web address available right now
        /// </summary>
        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                if (_isAvailable != value)
                {
                    _isAvailable = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isAvailable;

        /// <summary>
        /// Web address
        /// </summary>
        public string Address
        {
            get => _address;
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _address;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
