using System;
using System.ComponentModel;

namespace MFAudioDeviceEnumeratorNAudioWpfApp.AudioManager.AudioDeviceManager
{
    public interface IAudioDevice : INotifyPropertyChanged, IDisposable
    {
        string Id { get; }
        string DisplayName { get; }
        string IconPath { get; }
        string InterfaceName { get; }
        string DeviceDescription { get; }
        bool IsMuted { get; set; }
        float Volume { get; set; }
        void DevicePropertiesChanged();
    }
}