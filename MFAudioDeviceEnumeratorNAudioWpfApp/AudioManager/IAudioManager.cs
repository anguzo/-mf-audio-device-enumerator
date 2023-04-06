using System;
using System.Collections.ObjectModel;
using MFAudioDeviceEnumeratorNAudioWpfApp.AudioManager.AudioDeviceManager;

namespace MFAudioDeviceEnumeratorNAudioWpfApp.AudioManager
{
    public interface IAudioManager : IDisposable
    {
        ObservableCollection<IAudioDevice> PlaybackDevices { get; }
        ObservableCollection<IAudioDevice> RecordingDevices { get; }
        IAudioDevice PlaybackDevice { get; set; }
        string PlaybackDeviceId { get; }
        IAudioDevice RecordingDevice { get; set; }
        string RecordingDeviceId { get; }
        event EventHandler DefaultPlaybackDeviceChanged;
        event EventHandler DefaultRecordingDeviceChanged;
        void IncrementPlaybackDeviceVolume();
        void DecrementPlaybackDeviceVolume();
        void IncrementRecordingDeviceVolume();
        void DecrementRecordingDeviceVolume();
    }
}