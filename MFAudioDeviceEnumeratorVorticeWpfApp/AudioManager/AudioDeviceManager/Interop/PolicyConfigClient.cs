using System.Runtime.InteropServices;
using Vortice.MediaFoundation;

namespace MFAudioDeviceEnumeratorVorticeWpfApp.AudioManager.AudioDeviceManager.Interop
{
    [ComImport]
    [Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")]
    internal class PolicyConfigClient
    {
    }

    internal class AutoPolicyConfigClientWin7
    {
        private readonly IPolicyConfigWin7 _policyClient = (IPolicyConfigWin7)new PolicyConfigClient();

        public void SetEndpointVisibility(string deviceId, bool isVisible)
        {
            _policyClient.SetEndpointVisibility(deviceId, isVisible ? (short)1 : (short)0);
        }

        public void SetDefaultEndpoint(string deviceId, Role role = Role.Multimedia)
        {
            _policyClient.SetDefaultEndpoint(deviceId, role);
        }
    }
}