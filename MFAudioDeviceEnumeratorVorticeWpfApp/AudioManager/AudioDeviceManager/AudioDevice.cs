using System;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Vortice.MediaFoundation;

namespace MFAudioDeviceEnumeratorVorticeWpfApp.AudioManager.AudioDeviceManager
{
    public class AudioDevice : ObservableObject, IAudioDevice
    {
        private readonly IMMDevice _device;
        private readonly Dispatcher _dispatcher;

        public AudioDevice(IMMDevice device, Dispatcher foregroundDispatcher)
        {
            _device = device;
            Id = _device.Id;
            _dispatcher = foregroundDispatcher;
        }

        public string Id { get; }
        public string FriendlyName => _device.FriendlyName;
        public string DeviceFriendlyName => _device.DeviceFriendlyName;
        public string IconPath => _device.IconPath;

        public void DevicePropertiesChanged()
        {
            _dispatcher.Invoke(() =>
            {
                OnPropertyChanged(nameof(FriendlyName));
                OnPropertyChanged(nameof(IconPath));
                OnPropertyChanged(nameof(DeviceFriendlyName));
            });
        }

        #region Dispose and Finalize

        ~AudioDevice()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) _device?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}