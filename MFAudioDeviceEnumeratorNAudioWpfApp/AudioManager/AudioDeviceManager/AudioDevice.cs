using System;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MFAudioDeviceEnumeratorNAudioWpfApp.AudioManager.AudioDeviceManager.Extensions;
using MFAudioDeviceEnumeratorNAudioWpfApp.AudioManager.AudioDeviceManager.Interop.Utilities;
using NAudio.CoreAudioApi;

namespace MFAudioDeviceEnumeratorNAudioWpfApp.AudioManager.AudioDeviceManager
{
    public class AudioDevice : ObservableObject, IAudioDevice
    {
        private readonly MMDevice _device;
        private readonly AudioEndpointVolume _deviceVolume;
        private readonly Dispatcher _dispatcher;

        public AudioDevice(MMDevice device, Dispatcher foregroundDispatcher)
        {
            _device = device;
            _device.GetPropertyInformation();
            Id = _device.ID;
            _dispatcher = foregroundDispatcher;

            if (_device.State == DeviceState.Active)
            {
                _deviceVolume = device.AudioEndpointVolume;
                _deviceVolume.OnVolumeNotification += DeviceVolumeOnVolumeNotification;
            }
        }

        public string Id { get; }
        public string FriendlyName => _device.FriendlyName;
        public string DeviceFriendlyName => _device.DeviceFriendlyName;
        public string IconPath => _device.IconPath;

        public float Volume
        {
            get => _deviceVolume.MasterVolumeLevelScalar;
            set
            {
                try
                {
                    _deviceVolume.MasterVolumeLevelScalar = value;
                }
                catch (Exception ex) when (ex.Is(HRESULT.AUDCLNT_E_DEVICE_INVALIDATED))
                {
                    // Expected in some cases.
                }
            }
        }

        public bool IsMuted
        {
            get => _deviceVolume.Mute;
            set
            {
                try
                {
                    _deviceVolume.Mute = value;
                }
                catch (Exception ex) when (ex.Is(HRESULT.AUDCLNT_E_DEVICE_INVALIDATED))
                {
                    // Expected in some cases.
                }
            }
        }

        public void DevicePropertiesChanged()
        {
            _dispatcher.Invoke(() =>
            {
                OnPropertyChanged(nameof(FriendlyName));
                OnPropertyChanged(nameof(IconPath));
                OnPropertyChanged(nameof(DeviceFriendlyName));
            });
        }

        private void DeviceVolumeOnVolumeNotification(AudioVolumeNotificationData data)
        {
            _dispatcher.Invoke(() =>
            {
                OnPropertyChanged(nameof(Volume));
                OnPropertyChanged(nameof(IsMuted));
            });
        }

        #region Dispose and Finalize

        ~AudioDevice()
        {
            Dispose(false);
        }

        private void ReleaseUnmanagedResources()
        {
            _deviceVolume.OnVolumeNotification -= DeviceVolumeOnVolumeNotification;
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                _deviceVolume?.Dispose();
                _device?.Dispose();
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