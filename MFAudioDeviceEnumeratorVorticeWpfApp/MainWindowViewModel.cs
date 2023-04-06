using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MFAudioDeviceEnumeratorVorticeWpfApp.AudioManager;
using MFAudioDeviceEnumeratorVorticeWpfApp.AudioManager.AudioDeviceManager;

namespace MFAudioDeviceEnumeratorVorticeWpfApp
{
    public class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            AudioManager = new AudioManager.AudioManager();
            SetupPlaybackDeviceLogic();
            SetupRecordingDeviceLogic();
        }

        public IAudioManager AudioManager { get; }
        public IAudioDevice SelectedPlaybackDevice => AudioManager.PlaybackDevice;
        public IAudioDevice SelectedRecordingDevice => AudioManager.RecordingDevice;
        public ICommand SelectPlaybackDeviceCommand { get; private set; }
        public ICommand SelectRecordingDeviceCommand { get; private set; }
        public ObservableCollection<IAudioDevice> PlaybackDevices => AudioManager.PlaybackDevices;
        public ObservableCollection<IAudioDevice> RecordingDevices => AudioManager.RecordingDevices;

        private void AudioManagerOnDefaultPlaybackDeviceChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(SelectedPlaybackDevice));
        }

        private void AudioManagerOnDefaultRecordingDeviceChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(SelectedRecordingDevice));
        }

        private void SetupPlaybackDeviceLogic()
        {
            AudioManager.DefaultPlaybackDeviceChanged += AudioManagerOnDefaultPlaybackDeviceChanged;
            SelectPlaybackDeviceCommand = new RelayCommand<IAudioDevice>(device =>
            {
                AudioManager.PlaybackDevice = device;
            });
        }

        private void SetupRecordingDeviceLogic()
        {
            AudioManager.DefaultRecordingDeviceChanged += AudioManagerOnDefaultRecordingDeviceChanged;
            SelectRecordingDeviceCommand = new RelayCommand<IAudioDevice>(device =>
            {
                AudioManager.RecordingDevice = device;
            });
        }
    }
}