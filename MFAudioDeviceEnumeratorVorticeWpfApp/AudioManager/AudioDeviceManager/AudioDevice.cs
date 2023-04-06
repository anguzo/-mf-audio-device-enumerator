using System;
using System.Diagnostics;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MFAudioDeviceEnumeratorVorticeWpfApp.AudioManager.AudioDeviceManager.Extensions;
using MFAudioDeviceEnumeratorVorticeWpfApp.AudioManager.AudioDeviceManager.Interop.Utilities;
using SharpGen.Runtime.Win32;
using Vortice.MediaFoundation;
using Vortice.Win32;

namespace MFAudioDeviceEnumeratorVorticeWpfApp.AudioManager.AudioDeviceManager
{
    public class AudioDevice : ObservableObject, IAudioDevice
    {
        private readonly IMMDevice _device;
        private readonly IPropertyStore _deviceProperties;
        private readonly Dispatcher _dispatcher;

        public AudioDevice(IMMDevice device, Dispatcher foregroundDispatcher)
        {
            _device = device;
            Id = _device.Id;
            _deviceProperties = _device.Properties;
            _dispatcher = foregroundDispatcher;
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