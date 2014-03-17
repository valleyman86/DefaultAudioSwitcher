using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DefaultAudioSwitcher
{
    class EndPointController
    {
        [DllImport("AudioEndPointController.dll", EntryPoint = "SetDefaultAudioPlaybackDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetDefaultAudioPlaybackDevice([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string devID);

        [DllImport("AudioEndPointController.dll", EntryPoint = "GetDeviceID", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.LPWStr)]
        public static extern string GetDeviceID(int deviceCount);

        [DllImport("AudioEndPointController.dll", EntryPoint = "GetDeviceIDasLPWSTR", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.LPWStr)]
        public static extern string GetDeviceIDasLPWSTR(int deviceCount);

        [DllImport("AudioEndPointController.dll", EntryPoint = "GetDeviceFriendlyName", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAsAttribute(UnmanagedType.LPWStr)]
        public static extern string GetDeviceFriendlyName(int deviceCount);

        [DllImport("AudioEndPointController.dll", EntryPoint = "GetDeviceCount", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetDeviceCount();
    }
}
