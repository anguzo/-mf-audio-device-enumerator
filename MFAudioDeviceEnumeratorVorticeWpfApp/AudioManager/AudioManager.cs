using System;
using System.Collections.ObjectModel;
using MFAudioDeviceEnumeratorVorticeWpfApp.AudioManager.AudioDeviceManager;

namespace MFAudioDeviceEnumeratorVorticeWpfApp.AudioManager
{
    public class AudioManager : IAudioManager
    {
        private readonly IAudioDeviceManager _playbackAudioDeviceManager;
        private readonly IAudioDeviceManager _recordingAudioDeviceManager;

        public AudioManager()
        {
            _playbackAudioDeviceManager =
                new AudioDeviceManager.AudioDeviceManager(AudioDeviceKind.Playback);
            _recordingAudioDeviceManager =
                new AudioDeviceManager.AudioDeviceManager(AudioDeviceKind.Recording);
        }

        public event EventHandler DefaultPlaybackDeviceChanged
        {
            add => _playbackAudioDeviceManager.DefaultChanged += value;
            remove => _playbackAudioDeviceManager.DefaultChanged -= value;
        }

        public event EventHandler DefaultRecordingDeviceChanged
        {
            add => _recordingAudioDeviceManager.DefaultChanged += value;
            remove => _recordingAudioDeviceManager.DefaultChanged -= value;
        }

        public ObservableCollection<IAudioDevice> PlaybackDevices => _playbackAudioDeviceManager.Devices;
        public ObservableCollection<IAudioDevice> RecordingDevices => _recordingAudioDeviceManager.Devices;

        public IAudioDevice PlaybackDevice
        {
            get => _playbackAudioDeviceManager.Default;
            set => _playbackAudioDeviceManager.Default = value;
        }

        public string PlaybackDeviceId => PlaybackDevice?.Id;


        public IAudioDevice RecordingDevice
        {
            get => _recordingAudioDeviceManager.Default;
            set => _recordingAudioDeviceManager.Default = value;
        }

        public string RecordingDeviceId => RecordingDevice?.Id;

        #region Dispose and Finalize

        ~AudioManager()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _playbackAudioDeviceManager?.Dispose();
                _recordingAudioDeviceManager?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}