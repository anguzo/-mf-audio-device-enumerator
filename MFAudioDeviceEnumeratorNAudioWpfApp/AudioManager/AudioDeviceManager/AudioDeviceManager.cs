using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;
using MFAudioDeviceEnumeratorNAudioWpfApp.AudioManager.AudioDeviceManager.Extensions;
using MFAudioDeviceEnumeratorNAudioWpfApp.AudioManager.AudioDeviceManager.Interop;
using MFAudioDeviceEnumeratorNAudioWpfApp.AudioManager.AudioDeviceManager.Interop.Utilities;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace MFAudioDeviceEnumeratorNAudioWpfApp.AudioManager.AudioDeviceManager
{
    public class AudioDeviceManager : IAudioDeviceManager
    {
        private const Role DEFAULT_ROLE = Role.Multimedia;
        private static AutoPolicyConfigClientWin7 _policyConfigClient;

        private static PropertyKey PKEY_AudioEndPoint_Interface =
            new PropertyKey(Guid.Parse("{a45c254e-df1c-4efd-8020-67d146a850e0}"), 2);

        private readonly ConcurrentDictionary<string, IAudioDevice> _deviceMap =
            new ConcurrentDictionary<string, IAudioDevice>();

        private readonly Dispatcher _dispatcher;
        private readonly MMDeviceEnumerator _enumerator;

        public AudioDeviceManager(AudioDeviceKind kind)
        {
            Kind = kind;
            _dispatcher = Dispatcher.CurrentDispatcher;
            _enumerator = new MMDeviceEnumerator();
            _enumerator.RegisterEndpointNotificationCallback(this);
            var devices = _enumerator.EnumerateAudioEndPoints(Flow, DeviceState.Active);
            foreach (var device in devices) ((IMMNotificationClient)this).OnDeviceAdded(device.ID);
        }

        private DataFlow Flow => Kind == AudioDeviceKind.Playback ? DataFlow.Render : DataFlow.Capture;

        public event EventHandler DefaultChanged;

        public AudioDeviceKind Kind { get; }

        public IAudioDevice Default
        {
            get
            {
                try
                {
                    return GetDefaultDevice();
                }
                catch (Exception ex) when (ex.Is(HRESULT.ERROR_NOT_FOUND))
                {
                    // Expected.
                    return null;
                }
            }
            set => SetDefaultDevice(value);
        }

        public ObservableCollection<IAudioDevice> Devices { get; } =
            new ObservableCollection<IAudioDevice>();

        public void SetDefaultDevice(IAudioDevice device, Role role = DEFAULT_ROLE)
        {
            _policyConfigClient = _policyConfigClient ?? new AutoPolicyConfigClientWin7();

            // Racing with the system, the device may not be valid anymore.
            try
            {
                _policyConfigClient.SetDefaultEndpoint(device.Id, role);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public IAudioDevice GetDefaultDevice(Role role = DEFAULT_ROLE)
        {
            try
            {
                var rawDevice = _enumerator.GetDefaultAudioEndpoint(Flow, role);
                TryFind(rawDevice.ID, out var device);
                return device;
            }
            catch (Exception ex) when (ex.Is(HRESULT.ERROR_NOT_FOUND))
            {
                // Expected.
                return null;
            }
        }

        private void Add(IAudioDevice device)
        {
            if (_deviceMap.TryAdd(device.Id, device))
                _dispatcher.Invoke(() => { Devices.Add(device); });
        }

        private void Remove(string deviceId)
        {
            if (_deviceMap.TryRemove(deviceId, out var device))
                _dispatcher.Invoke(() => { Devices.Remove(device); });
        }

        private bool TryFind(string deviceId, out IAudioDevice found)
        {
            if (deviceId == null)
            {
                found = null;
                return false;
            }

            return _deviceMap.TryGetValue(deviceId, out found);
        }

        #region IMMNotificationClient

        void IMMNotificationClient.OnDeviceAdded(string pwstrDeviceId)
        {
            try
            {
                var device = _enumerator.GetDevice(pwstrDeviceId);
                if (device.DataFlow == Flow &&
                    device.State == DeviceState.Active)
                {
                    var newDevice = new AudioDevice(device, _dispatcher);

                    Add(newDevice);
                }
            }
            catch (Exception ex)
            {
                // Catch Exception here because IMMDevice::Activate can return E_POINTER/NullReferenceException, as well as other exceptions listed here:
                // https://docs.microsoft.com/en-us/dotnet/framework/interop/how-to-map-hresults-and-exceptions
                Debug.WriteLine(ex);
            }
        }

        void IMMNotificationClient.OnDeviceRemoved(string pwstrDeviceId)
        {
            try
            {
                Remove(pwstrDeviceId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        void IMMNotificationClient.OnDefaultDeviceChanged(DataFlow flow, Role role, string pwstrDefaultDeviceId)
        {
            if (flow == Flow && role == DEFAULT_ROLE) DefaultChanged?.Invoke(this, EventArgs.Empty);
        }

        void IMMNotificationClient.OnDeviceStateChanged(string pwstrDeviceId, DeviceState dwNewState)
        {
            switch (dwNewState)
            {
                case DeviceState.Active:
                    ((IMMNotificationClient)this).OnDeviceAdded(pwstrDeviceId);
                    break;
                case DeviceState.Disabled:
                case DeviceState.NotPresent:
                case DeviceState.Unplugged:
                    ((IMMNotificationClient)this).OnDeviceRemoved(pwstrDeviceId);
                    break;
                default:
                    Debug.WriteLine($"Unknown DEVICE_STATE: {dwNewState}");
                    break;
            }
        }

        void IMMNotificationClient.OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
            if (!TryFind(pwstrDeviceId, out var device)) return;
            if (!PKEY_AudioEndPoint_Interface.Equals(key)) return;
            // We're racing with the system, the device may not be resolvable anymore.
            try
            {
                device.DevicePropertiesChanged();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region Dispose and Finalize

        ~AudioDeviceManager()
        {
            Dispose(false);
        }

        private void ReleaseUnmanagedResources()
        {
            _enumerator?.UnregisterEndpointNotificationCallback(this);
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing) _enumerator?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}