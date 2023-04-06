# MF-Audio-Device-Enumerator

This example shows how audio device watcher/observer and how audio device enumeration can be implemented in Windows.
There are two solutions provided:
1) Using NAudio library.
2) Using Vortice.MediaFoundation library.

Both libraries provide CoreAudioAPI data structures such as IMMDeviceEnumerator, IMMDevice, etc.

PolicyConfigClient is implemented to support setting default system device.

NAudio sample also provides AudioEndpointVolume as part of MMDevice while Vortice does not which enables audio device volume control for it.
