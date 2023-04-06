using System;
using System.ComponentModel;

namespace MFAudioDeviceEnumeratorNAudioWpfApp.AudioManager.AudioDeviceManager
{
    public interface IAudioDevice : INotifyPropertyChanged, IDisposable
    {
        string Id { get; }
        string FriendlyName { get; }
        string DeviceFriendlyName { get; }
        string IconPath { get; }
        bool IsMuted { get; set; }
        float Volume { get; set; }
        void DevicePropertiesChanged();
    }
}