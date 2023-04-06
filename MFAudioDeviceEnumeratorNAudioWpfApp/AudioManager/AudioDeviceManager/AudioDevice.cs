using System;
using System.Diagnostics;
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
        private readonly PropertyStore _deviceProperties;
        private readonly AudioEndpointVolume _deviceVolume;
        private readonly Dispatcher _dispatcher;

        public AudioDevice(MMDevice device, Dispatcher foregroundDispatcher)
        {
            _device = device;
            Id = _device.ID;
            _deviceProperties = _device.Properties;
            _dispatcher = foregroundDispatcher;

            if (_device.State == DeviceState.Active)
            {
                _deviceVolume = device.AudioEndpointVolume;
                _deviceVolume.OnVolumeNotification += DeviceVolumeOnVolumeNotification;
            }
        }

        public string Id { get; }

        public string DisplayName
        {
            get
            {
                try
                {
                    return _deviceProperties?[PropertyKeys.PKEY_Device_FriendlyName].Value as string;
                }
                catch (Exception ex) when (ex.Is(HRESULT.AUDCLNT_E_DEVICE_INVALIDATED))
                {
                    // Expected in some cases.
                    return "";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return "";
                }
            }
        }

        public string IconPath
        {
            get
            {
                try
                {
                    return _deviceProperties?[PropertyKeys.PKEY_Device_IconPath].Value as string;
                }
                catch (Exception ex) when (ex.Is(HRESULT.AUDCLNT_E_DEVICE_INVALIDATED))
                {
                    // Expected in some cases.
                    return "";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return "";
                }
            }
        }

        public string InterfaceName
        {
            get
            {
                try
                {
                    return _deviceProperties?[PropertyKeys.PKEY_DeviceInterface_FriendlyName].Value as string;
                }
                catch (Exception ex) when (ex.Is(HRESULT.AUDCLNT_E_DEVICE_INVALIDATED))
                {
                    // Expected in some cases.
                    return "";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return "";
                }
            }
        }

        public string DeviceDescription
        {
            get
            {
                try
                {
                    return _deviceProperties?[PropertyKeys.PKEY_Device_DeviceDesc].Value as string;
                }
                catch (Exception ex) when (ex.Is(HRESULT.AUDCLNT_E_DEVICE_INVALIDATED))
                {
                    // Expected in some cases.
                    return "";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return "";
                }
            }
        }

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
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(IconPath));
                OnPropertyChanged(nameof(InterfaceName));
                OnPropertyChanged(nameof(DeviceDescription));
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