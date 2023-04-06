using System;
using System.Runtime.InteropServices;
using MFAudioDeviceEnumeratorVorticeWpfApp.AudioManager.AudioDeviceManager.Interop.Utilities;

namespace MFAudioDeviceEnumeratorVorticeWpfApp.AudioManager.AudioDeviceManager.Extensions
{
    internal static class ExceptionExtensions
    {
        internal static bool Is(this Exception ex, HRESULT type)
        {
            switch (type)
            {
                case HRESULT.AUDCLNT_E_DEVICE_INVALIDATED:
                case HRESULT.AUDCLNT_S_NO_SINGLE_PROCESS:
                case HRESULT.ERROR_NOT_FOUND:
                    return (uint)(ex as COMException)?.HResult == (uint)type;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}