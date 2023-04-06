using System.Runtime.InteropServices;
using SharpGen.Runtime.Win32;
using Vortice.MediaFoundation;

namespace MFAudioDeviceEnumeratorVorticeWpfApp.AudioManager.AudioDeviceManager.Interop
{
    // W10_TH1: CA286FC3-91FD-42C3-8E9B-CAAFA66242E3
    // W10_TH2: 6BE54BE8-A068-4875-A49D-0C2966473B11
    // Win7-Win8, W10_RS1-Present:
    [Guid("F8679F50-850A-41CF-9C72-430F290290C8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPolicyConfigWin7
    {
        void Unused1();
        void Unused2();
        void Unused3();
        void Unused4();
        void Unused5();
        void Unused6();
        void Unused7();
        void Unused8();

        void GetPropertyValue<T>([MarshalAs(UnmanagedType.LPWStr)] string wszDeviceId, ref PropertyKey pkey,
            ref T pv);

        void SetPropertyValue<T>([MarshalAs(UnmanagedType.LPWStr)] string wszDeviceId, ref PropertyKey pkey,
            ref T pv);

        void SetDefaultEndpoint([MarshalAs(UnmanagedType.LPWStr)] string wszDeviceId, Role eRole);

        void SetEndpointVisibility([MarshalAs(UnmanagedType.LPWStr)] string wszDeviceId,
            [MarshalAs(UnmanagedType.I2)] short isVisible);
    }
}