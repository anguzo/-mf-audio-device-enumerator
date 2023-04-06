using System;
using System.ComponentModel;

namespace MFAudioDeviceEnumeratorVorticeWpfApp.AudioManager.AudioDeviceManager
{
    public interface IAudioDevice : INotifyPropertyChanged, IDisposable
    {
        string Id { get; }
        string FriendlyName { get; }
        string DeviceFriendlyName { get; }
        string IconPath { get; }
        void DevicePropertiesChanged();
    }
}