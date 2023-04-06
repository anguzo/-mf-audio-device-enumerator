using System;
using System.ComponentModel;

namespace MFAudioDeviceEnumeratorVorticeWpfApp.AudioManager.AudioDeviceManager
{
    public interface IAudioDevice : INotifyPropertyChanged, IDisposable
    {
        string Id { get; }
        string DisplayName { get; }
        string IconPath { get; }
        string InterfaceName { get; }
        string DeviceDescription { get; }
        void DevicePropertiesChanged();
    }
}