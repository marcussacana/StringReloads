using System;
using System.Runtime.InteropServices;
using SRLWrapper.Wrapper.Base;
using static SRLWrapper.Wrapper.Base.Wrapper;

namespace SRLWrapper.Wrapper
{
    public unsafe static class WINMM
    {
        public static void* RealHandler;
        public static void LoadRetail()
        {
            if (RealHandler != null)
                return;

            RealHandler = LoadLibrary("winmm.dll");

            dPlaySoundW = GetDelegate<RET_3>(RealHandler, "PlaySoundW", true);
            dtimeSetEvent = GetDelegate<RET_5>(RealHandler, "timeSetEvent", true);
            dtimeKillEvent = GetDelegate<RET_1>(RealHandler, "timeKillEvent", true);
            djoyGetNumDevs = GetDelegate<RET_0>(RealHandler, "joyGetNumDevs", true);
            djoyConfigChanged = GetDelegate<RET_1>(RealHandler, "joyConfigChanged", true);
            djoyGetDevCapsA = GetDelegate<RET_3>(RealHandler, "joyGetDevCapsA", true);
            djoyGetDevCapsW = GetDelegate<RET_3>(RealHandler, "joyGetDevCapsW", true);
            djoyGetPos = GetDelegate<RET_2>(RealHandler, "joyGetPos", true);
            djoyGetPosEx = GetDelegate<RET_2>(RealHandler, "joyGetPosEx", true);
            djoyGetThreshold = GetDelegate<RET_2>(RealHandler, "joyGetThreshold", true);
            djoyReleaseCapture = GetDelegate<RET_1>(RealHandler, "joyReleaseCapture", true);
            djoySetCapture = GetDelegate<RET_4>(RealHandler, "joySetCapture", true);
            djoySetThreshold = GetDelegate<RET_2>(RealHandler, "joySetThreshold", true);
            dNotifyCallbackData = GetDelegate<RET_5>(RealHandler, "NotifyCallbackData", true);
            dWOW32DriverCallback = GetDelegate<RET_7>(RealHandler, "WOW32DriverCallback", true);
            dWOW32ResolveMultiMediaHandle = GetDelegate<RET_6>(RealHandler, "WOW32ResolveMultiMediaHandle", true);
            daux32Message = GetDelegate<RET_5>(RealHandler, "aux32Message", true);
            djoy32Message = GetDelegate<RET_5>(RealHandler, "joy32Message", true);
            dmid32Message = GetDelegate<RET_5>(RealHandler, "mid32Message", true);
            dmod32Message = GetDelegate<RET_5>(RealHandler, "mod32Message", true);
            dmxd32Message = GetDelegate<RET_5>(RealHandler, "mxd32Message", true);
            dtid32Message = GetDelegate<RET_5>(RealHandler, "tid32Message", true);
            dwid32Message = GetDelegate<RET_5>(RealHandler, "wid32Message", true);
            dwod32Message = GetDelegate<RET_5>(RealHandler, "wod32Message", true);
            dmciExecute = GetDelegate<RET_1>(RealHandler, "mciExecute", true);
            dmciGetErrorStringA = GetDelegate<RET_3>(RealHandler, "mciGetErrorStringA", true);
            dmciGetErrorStringW = GetDelegate<RET_3>(RealHandler, "mciGetErrorStringW", true);
            dmciSendCommandA = GetDelegate<RET_4>(RealHandler, "mciSendCommandA", true);
            dmciSendCommandW = GetDelegate<RET_4>(RealHandler, "mciSendCommandW", true);
            dmciSendStringA = GetDelegate<RET_4>(RealHandler, "mciSendStringA", true);
            dmciSendStringW = GetDelegate<RET_4>(RealHandler, "mciSendStringW", true);
            dmciFreeCommandResource = GetDelegate<RET_1>(RealHandler, "mciFreeCommandResource", true);
            dmciLoadCommandResource = GetDelegate<RET_3>(RealHandler, "mciLoadCommandResource", true);
            dmciDriverNotify = GetDelegate<RET_3>(RealHandler, "mciDriverNotify", true);
            dmciDriverYield = GetDelegate<RET_1>(RealHandler, "mciDriverYield", true);
            dmciGetCreatorTask = GetDelegate<RET_1>(RealHandler, "mciGetCreatorTask", true);
            dmciGetDeviceIDA = GetDelegate<RET_1>(RealHandler, "mciGetDeviceIDA", true);
            dmciGetDeviceIDFromElementIDA = GetDelegate<RET_2>(RealHandler, "mciGetDeviceIDFromElementIDA", true);
            dmciGetDeviceIDFromElementIDW = GetDelegate<RET_2>(RealHandler, "mciGetDeviceIDFromElementIDW", true);
            dmciGetDeviceIDW = GetDelegate<RET_1>(RealHandler, "mciGetDeviceIDW", true);
            dmciGetDriverData = GetDelegate<RET_1>(RealHandler, "mciGetDriverData", true);
            dmciGetYieldProc = GetDelegate<RET_2>(RealHandler, "mciGetYieldProc", true);
            dmciSetDriverData = GetDelegate<RET_2>(RealHandler, "mciSetDriverData", true);
            dmciSetYieldProc = GetDelegate<RET_3>(RealHandler, "mciSetYieldProc", true);
            dPlaySoundA = GetDelegate<RET_3>(RealHandler, "PlaySoundA", true);
            dsndPlaySoundA = GetDelegate<RET_2>(RealHandler, "sndPlaySoundA", true);
            dsndPlaySoundW = GetDelegate<RET_2>(RealHandler, "sndPlaySoundW", true);
            dDriverCallback = GetDelegate<RET_7>(RealHandler, "DriverCallback", true);
            dWOWAppExit = GetDelegate<RET_1>(RealHandler, "WOWAppExit", true);
            dmmsystemGetVersion = GetDelegate<RET_0>(RealHandler, "mmsystemGetVersion", true);
            dtimeBeginPeriod = GetDelegate<RET_1>(RealHandler, "timeBeginPeriod", true);
            dtimeEndPeriod = GetDelegate<RET_1>(RealHandler, "timeEndPeriod", true);
            dCloseDriver = GetDelegate<RET_3>(RealHandler, "CloseDriver", true);
            dDefDriverProc = GetDelegate<RET_5>(RealHandler, "DefDriverProc", true);
            dDrvGetModuleHandle = GetDelegate<RET_1>(RealHandler, "DrvGetModuleHandle", true);
            dGetDriverModuleHandle = GetDelegate<RET_1>(RealHandler, "GetDriverModuleHandle", true);
            dOpenDriver = GetDelegate<RET_3>(RealHandler, "OpenDriver", true);
            dSendDriverMessage = GetDelegate<RET_4>(RealHandler, "SendDriverMessage", true);
            dauxGetDevCapsA = GetDelegate<RET_3>(RealHandler, "auxGetDevCapsA", true);
            dauxGetDevCapsW = GetDelegate<RET_3>(RealHandler, "auxGetDevCapsW", true);
            dauxGetVolume = GetDelegate<RET_2>(RealHandler, "auxGetVolume", true);
            dauxOutMessage = GetDelegate<RET_4>(RealHandler, "auxOutMessage", true);
            dauxSetVolume = GetDelegate<RET_2>(RealHandler, "auxSetVolume", true);
            dmidiConnect = GetDelegate<RET_3>(RealHandler, "midiConnect", true);
            dmidiDisconnect = GetDelegate<RET_3>(RealHandler, "midiDisconnect", true);
            dmidiInAddBuffer = GetDelegate<RET_3>(RealHandler, "midiInAddBuffer", true);
            dmidiInClose = GetDelegate<RET_1>(RealHandler, "midiInClose", true);
            dmidiInGetDevCapsA = GetDelegate<RET_3>(RealHandler, "midiInGetDevCapsA", true);
            dmidiInGetDevCapsW = GetDelegate<RET_3>(RealHandler, "midiInGetDevCapsW", true);
            dmidiInGetErrorTextA = GetDelegate<RET_3>(RealHandler, "midiInGetErrorTextA", true);
            dmidiInGetErrorTextW = GetDelegate<RET_3>(RealHandler, "midiInGetErrorTextW", true);
            dmidiInGetID = GetDelegate<RET_2>(RealHandler, "midiInGetID", true);
            dmidiInMessage = GetDelegate<RET_4>(RealHandler, "midiInMessage", true);
            dmidiInOpen = GetDelegate<RET_5>(RealHandler, "midiInOpen", true);
            dmidiInPrepareHeader = GetDelegate<RET_3>(RealHandler, "midiInPrepareHeader", true);
            dmidiInReset = GetDelegate<RET_1>(RealHandler, "midiInReset", true);
            dmidiInStart = GetDelegate<RET_1>(RealHandler, "midiInStart", true);
            dmidiInStop = GetDelegate<RET_1>(RealHandler, "midiInStop", true);
            dmidiInUnprepareHeader = GetDelegate<RET_3>(RealHandler, "midiInUnprepareHeader", true);
            dmidiOutCacheDrumPatches = GetDelegate<RET_4>(RealHandler, "midiOutCacheDrumPatches", true);
            dmidiOutCachePatches = GetDelegate<RET_4>(RealHandler, "midiOutCachePatches", true);
            dmidiOutClose = GetDelegate<RET_1>(RealHandler, "midiOutClose", true);
            dmidiOutGetDevCapsA = GetDelegate<RET_3>(RealHandler, "midiOutGetDevCapsA", true);
            dmidiOutGetDevCapsW = GetDelegate<RET_3>(RealHandler, "midiOutGetDevCapsW", true);
            dmidiOutGetErrorTextA = GetDelegate<RET_3>(RealHandler, "midiOutGetErrorTextA", true);
            dmidiOutGetErrorTextW = GetDelegate<RET_3>(RealHandler, "midiOutGetErrorTextW", true);
            dmidiOutGetID = GetDelegate<RET_2>(RealHandler, "midiOutGetID", true);
            dmidiOutGetVolume = GetDelegate<RET_2>(RealHandler, "midiOutGetVolume", true);
            dmidiOutLongMsg = GetDelegate<RET_3>(RealHandler, "midiOutLongMsg", true);
            dmidiOutMessage = GetDelegate<RET_4>(RealHandler, "midiOutMessage", true);
            dmidiOutOpen = GetDelegate<RET_5>(RealHandler, "midiOutOpen", true);
            dmidiOutPrepareHeader = GetDelegate<RET_3>(RealHandler, "midiOutPrepareHeader", true);
            dmidiOutReset = GetDelegate<RET_1>(RealHandler, "midiOutReset", true);
            dmidiOutSetVolume = GetDelegate<RET_2>(RealHandler, "midiOutSetVolume", true);
            dmidiOutShortMsg = GetDelegate<RET_2>(RealHandler, "midiOutShortMsg", true);
            dmidiOutUnprepareHeader = GetDelegate<RET_3>(RealHandler, "midiOutUnprepareHeader", true);
            dmidiStreamClose = GetDelegate<RET_1>(RealHandler, "midiStreamClose", true);
            dmidiStreamOpen = GetDelegate<RET_6>(RealHandler, "midiStreamOpen", true);
            dmidiStreamOut = GetDelegate<RET_3>(RealHandler, "midiStreamOut", true);
            dmidiStreamPause = GetDelegate<RET_1>(RealHandler, "midiStreamPause", true);
            dmidiStreamPosition = GetDelegate<RET_3>(RealHandler, "midiStreamPosition", true);
            dmidiStreamProperty = GetDelegate<RET_3>(RealHandler, "midiStreamProperty", true);
            dmidiStreamRestart = GetDelegate<RET_1>(RealHandler, "midiStreamRestart", true);
            dmidiStreamStop = GetDelegate<RET_1>(RealHandler, "midiStreamStop", true);
            dmixerClose = GetDelegate<RET_1>(RealHandler, "mixerClose", true);
            dmixerGetControlDetailsA = GetDelegate<RET_3>(RealHandler, "mixerGetControlDetailsA", true);
            dmixerGetControlDetailsW = GetDelegate<RET_3>(RealHandler, "mixerGetControlDetailsW", true);
            dmixerGetDevCapsA = GetDelegate<RET_3>(RealHandler, "mixerGetDevCapsA", true);
            dmixerGetDevCapsW = GetDelegate<RET_3>(RealHandler, "mixerGetDevCapsW", true);
            dmixerGetID = GetDelegate<RET_3>(RealHandler, "mixerGetID", true);
            dmixerGetLineControlsA = GetDelegate<RET_3>(RealHandler, "mixerGetLineControlsA", true);
            dmixerGetLineControlsW = GetDelegate<RET_3>(RealHandler, "mixerGetLineControlsW", true);
            dmixerGetLineInfoA = GetDelegate<RET_3>(RealHandler, "mixerGetLineInfoA", true);
            dmixerGetLineInfoW = GetDelegate<RET_3>(RealHandler, "mixerGetLineInfoW", true);
            dmixerMessage = GetDelegate<RET_4>(RealHandler, "mixerMessage", true);
            dmixerOpen = GetDelegate<RET_5>(RealHandler, "mixerOpen", true);
            dmixerSetControlDetails = GetDelegate<RET_3>(RealHandler, "mixerSetControlDetails", true);
            dmmDrvInstall = GetDelegate<RET_4>(RealHandler, "mmDrvInstall", true);
            dmmTaskCreate = GetDelegate<RET_3>(RealHandler, "mmTaskCreate", true);
            dmmTaskSignal = GetDelegate<RET_1>(RealHandler, "mmTaskSignal", true);
            dmmioAdvance = GetDelegate<RET_3>(RealHandler, "mmioAdvance", true);
            dmmioAscend = GetDelegate<RET_3>(RealHandler, "mmioAscend", true);
            dmmioClose = GetDelegate<RET_2>(RealHandler, "mmioClose", true);
            dmmioCreateChunk = GetDelegate<RET_3>(RealHandler, "mmioCreateChunk", true);
            dmmioDescend = GetDelegate<RET_4>(RealHandler, "mmioDescend", true);
            dmmioFlush = GetDelegate<RET_2>(RealHandler, "mmioFlush", true);
            dmmioGetInfo = GetDelegate<RET_3>(RealHandler, "mmioGetInfo", true);
            dmmioInstallIOProcA = GetDelegate<RET_3>(RealHandler, "mmioInstallIOProcA", true);
            dmmioInstallIOProcW = GetDelegate<RET_3>(RealHandler, "mmioInstallIOProcW", true);
            dmmioOpenA = GetDelegate<RET_3>(RealHandler, "mmioOpenA", true);
            dmmioOpenW = GetDelegate<RET_3>(RealHandler, "mmioOpenW", true);
            dmmioRead = GetDelegate<RET_3>(RealHandler, "mmioRead", true);
            dmmioRenameA = GetDelegate<RET_4>(RealHandler, "mmioRenameA", true);
            dmmioRenameW = GetDelegate<RET_4>(RealHandler, "mmioRenameW", true);
            dmmioSeek = GetDelegate<RET_3>(RealHandler, "mmioSeek", true);
            dmmioSendMessage = GetDelegate<RET_4>(RealHandler, "mmioSendMessage", true);
            dmmioSetBuffer = GetDelegate<RET_4>(RealHandler, "mmioSetBuffer", true);
            dmmioSetInfo = GetDelegate<RET_3>(RealHandler, "mmioSetInfo", true);
            dmmioStringToFOURCCA = GetDelegate<RET_2>(RealHandler, "mmioStringToFOURCCA", true);
            dmmioStringToFOURCCW = GetDelegate<RET_2>(RealHandler, "mmioStringToFOURCCW", true);
            dmmioWrite = GetDelegate<RET_3>(RealHandler, "mmioWrite", true);
            dtimeGetTime = GetDelegate<RET_0>(RealHandler, "timeGetTime", true);
            dtimeGetDevCaps = GetDelegate<RET_2>(RealHandler, "timeGetDevCaps", true);
            dtimeGetSystemTime = GetDelegate<RET_2>(RealHandler, "timeGetSystemTime", true);
            dwaveInAddBuffer = GetDelegate<RET_3>(RealHandler, "waveInAddBuffer", true);
            dwaveInClose = GetDelegate<RET_1>(RealHandler, "waveInClose", true);
            dwaveInGetDevCapsA = GetDelegate<RET_3>(RealHandler, "waveInGetDevCapsA", true);
            dwaveInGetDevCapsW = GetDelegate<RET_3>(RealHandler, "waveInGetDevCapsW", true);
            dwaveInGetNumDevs = GetDelegate<RET_0>(RealHandler, "waveInGetNumDevs", true);
            dwaveInGetErrorTextA = GetDelegate<RET_3>(RealHandler, "waveInGetErrorTextA", true);
            dwaveInGetErrorTextW = GetDelegate<RET_3>(RealHandler, "waveInGetErrorTextW", true);
            dwaveInGetID = GetDelegate<RET_2>(RealHandler, "waveInGetID", true);
            dwaveInGetPosition = GetDelegate<RET_3>(RealHandler, "waveInGetPosition", true);
            dwaveInMessage = GetDelegate<RET_4>(RealHandler, "waveInMessage", true);
            dwaveInOpen = GetDelegate<RET_6>(RealHandler, "waveInOpen", true);
            dwaveInPrepareHeader = GetDelegate<RET_3>(RealHandler, "waveInPrepareHeader", true);
            dwaveInReset = GetDelegate<RET_1>(RealHandler, "waveInReset", true);
            dwaveInStart = GetDelegate<RET_1>(RealHandler, "waveInStart", true);
            dwaveInStop = GetDelegate<RET_1>(RealHandler, "waveInStop", true);
            dwaveInUnprepareHeader = GetDelegate<RET_3>(RealHandler, "waveInUnprepareHeader", true);
            dwaveOutBreakLoop = GetDelegate<RET_1>(RealHandler, "waveOutBreakLoop", true);
            dwaveOutClose = GetDelegate<RET_1>(RealHandler, "waveOutClose", true);
            dwaveOutGetDevCapsA = GetDelegate<RET_3>(RealHandler, "waveOutGetDevCapsA", true);
            dwaveOutGetDevCapsW = GetDelegate<RET_3>(RealHandler, "waveOutGetDevCapsW", true);
            dwaveOutGetNumDevs = GetDelegate<RET_0>(RealHandler, "waveOutGetNumDevs", true);
            dwaveOutGetErrorTextA = GetDelegate<RET_3>(RealHandler, "waveOutGetErrorTextA", true);
            dwaveOutGetErrorTextW = GetDelegate<RET_3>(RealHandler, "waveOutGetErrorTextW", true);
            dwaveOutGetID = GetDelegate<RET_2>(RealHandler, "waveOutGetID", true);
            dwaveOutGetPitch = GetDelegate<RET_2>(RealHandler, "waveOutGetPitch", true);
            dwaveOutGetPlaybackRate = GetDelegate<RET_2>(RealHandler, "waveOutGetPlaybackRate", true);
            dwaveOutGetPosition = GetDelegate<RET_3>(RealHandler, "waveOutGetPosition", true);
            dwaveOutGetVolume = GetDelegate<RET_2>(RealHandler, "waveOutGetVolume", true);
            dwaveOutMessage = GetDelegate<RET_4>(RealHandler, "waveOutMessage", true);
            dwaveOutOpen = GetDelegate<RET_6>(RealHandler, "waveOutOpen", true);
            dwaveOutPause = GetDelegate<RET_1>(RealHandler, "waveOutPause", true);
            dwaveOutPrepareHeader = GetDelegate<RET_3>(RealHandler, "waveOutPrepareHeader", true);
            dwaveOutReset = GetDelegate<RET_1>(RealHandler, "waveOutReset", true);
            dwaveOutRestart = GetDelegate<RET_1>(RealHandler, "waveOutRestart", true);
            dwaveOutSetPitch = GetDelegate<RET_2>(RealHandler, "waveOutSetPitch", true);
            dwaveOutSetPlaybackRate = GetDelegate<RET_2>(RealHandler, "waveOutSetPlaybackRate", true);
            dwaveOutSetVolume = GetDelegate<RET_2>(RealHandler, "waveOutSetVolume", true);
            dwaveOutUnprepareHeader = GetDelegate<RET_3>(RealHandler, "waveOutUnprepareHeader", true);
            dwaveOutWrite = GetDelegate<RET_3>(RealHandler, "waveOutWrite", true);

            InitializeSRL();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr PlaySoundW(IntPtr pszSound, IntPtr hmod, IntPtr fdwSound)
        {
			LoadRetail();
            return dPlaySoundW(pszSound, hmod, fdwSound);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr timeGetTime()
        {
            LoadRetail();
            return dtimeGetTime();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr timeSetEvent(IntPtr uDelay, IntPtr uResolution, IntPtr fptc, IntPtr dwUser, IntPtr fuEvent)
        {
			LoadRetail();
            return dtimeSetEvent(uDelay, uResolution, fptc, dwUser, fuEvent);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr timeKillEvent(IntPtr uTimerID)
        {
			LoadRetail();
            return dtimeKillEvent(uTimerID);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr joyGetNumDevs()
        {
			LoadRetail();
            return djoyGetNumDevs();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr joyConfigChanged(IntPtr dwFlags)
        {
			LoadRetail();
            return djoyConfigChanged(dwFlags);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr joyGetDevCapsA(IntPtr uJoyID, IntPtr pjc, IntPtr cbjc)
        {
			LoadRetail();
            return djoyGetDevCapsA(uJoyID, pjc, cbjc);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr joyGetDevCapsW(IntPtr uJoyID, IntPtr pjc, IntPtr cbjc)
        {
			LoadRetail();
            return djoyGetDevCapsW(uJoyID, pjc, cbjc);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr joyGetPos(IntPtr uJoyID, IntPtr pji)
        {
			LoadRetail();
            return djoyGetPos(uJoyID, pji);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr joyGetPosEx(IntPtr uJoyID, IntPtr pji)
        {
			LoadRetail();
            return djoyGetPosEx(uJoyID, pji);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr joyGetThreshold(IntPtr uJoyID, IntPtr puThreshold)
        {
			LoadRetail();
            return djoyGetThreshold(uJoyID, puThreshold);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr joyReleaseCapture(IntPtr uJoyID)
        {
			LoadRetail();
            return djoyReleaseCapture(uJoyID);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr joySetCapture(IntPtr hwnd, IntPtr uJoyID, IntPtr uPeriod, IntPtr fChanged)
        {
			LoadRetail();
            return djoySetCapture(hwnd, uJoyID, uPeriod, fChanged);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr joySetThreshold(IntPtr uJoyID, IntPtr uThreshold)
        {
			LoadRetail();
            return djoySetThreshold(uJoyID, uThreshold);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr NotifyCallbackData(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5)
        {
			LoadRetail();
            return dNotifyCallbackData(a1, a2, a3, a4, a5);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr WOW32DriverCallback(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr lParam, IntPtr a7)
        {
			LoadRetail();
            return dWOW32DriverCallback(a1, a2, a3, a4, a5, lParam, a7);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr WOW32ResolveMultiMediaHandle(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6)
        {
			LoadRetail();
            return dWOW32ResolveMultiMediaHandle(a1, a2, a3, a4, a5, a6);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr aux32Message(IntPtr uDeviceID, IntPtr uMsg, IntPtr a3, IntPtr dw1, IntPtr dw2)
        {
			LoadRetail();
            return daux32Message(uDeviceID, uMsg, a3, dw1, dw2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr joy32Message(IntPtr uJoyID, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr Size)
        {
			LoadRetail();
            return djoy32Message(uJoyID, a2, a3, a4, Size);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mid32Message(IntPtr uDeviceID, IntPtr uMsg, IntPtr hmi, IntPtr dw1, IntPtr Size)
        {
			LoadRetail();
            return dmid32Message(uDeviceID, uMsg, hmi, dw1, Size);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mod32Message(IntPtr a1, IntPtr uMsg, IntPtr hmo, IntPtr dwMsg, IntPtr dw2)
        {
			LoadRetail();
            return dmod32Message(a1, uMsg, hmo, dwMsg, dw2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mxd32Message(IntPtr a1, IntPtr uMsg, IntPtr hmx, IntPtr a4, IntPtr uMxId)
        {
			LoadRetail();
            return dmxd32Message(a1, uMsg, hmx, a4, uMxId);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr tid32Message(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr uPeriod, IntPtr a5)
        {
			LoadRetail();
            return dtid32Message(a1, a2, a3, uPeriod, a5);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr wid32Message(IntPtr uDeviceID, IntPtr uMsg, IntPtr hwi, IntPtr dw1, IntPtr fdwOpen)
        {
			LoadRetail();
            return dwid32Message(uDeviceID, uMsg, hwi, dw1, fdwOpen);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr wod32Message(IntPtr a1, IntPtr uMsg, IntPtr hwo, IntPtr dwVolume, IntPtr a5)
        {
			LoadRetail();
            return dwod32Message(a1, uMsg, hwo, dwVolume, a5);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciExecute(IntPtr MultiByteString)
        {
			LoadRetail();
            return dmciExecute(MultiByteString);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciGetErrorStringA(IntPtr mcierr, IntPtr pszText, IntPtr cchText)
        {
			LoadRetail();
            return dmciGetErrorStringA(mcierr, pszText, cchText);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciGetErrorStringW(IntPtr mcierr, IntPtr pszText, IntPtr cchText)
        {
			LoadRetail();
            return dmciGetErrorStringW(mcierr, pszText, cchText);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciSendCommandA(IntPtr mciId, IntPtr uMsg, IntPtr dwParam1, IntPtr dwParam2)
        {
			LoadRetail();
            return dmciSendCommandA(mciId, uMsg, dwParam1, dwParam2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciSendCommandW(IntPtr mciId, IntPtr uMsg, IntPtr dwParam1, IntPtr dwParam2)
        {
			LoadRetail();
            return dmciSendCommandW(mciId, uMsg, dwParam1, dwParam2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciSendStringA(IntPtr lpstrCommand, IntPtr lpstrReturnString, IntPtr uReturnLength, IntPtr hwndCallback)
        {
			LoadRetail();
            return dmciSendStringA(lpstrCommand, lpstrReturnString, uReturnLength, hwndCallback);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciSendStringW(IntPtr lpstrCommand, IntPtr lpstrReturnString, IntPtr uReturnLength, IntPtr hwndCallback)
        {
			LoadRetail();
            return dmciSendStringW(lpstrCommand, lpstrReturnString, uReturnLength, hwndCallback);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciFreeCommandResource(IntPtr wTable)
        {
			LoadRetail();
            return dmciFreeCommandResource(wTable);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciLoadCommandResource(IntPtr hInstance, IntPtr lpResName, IntPtr wType)
        {
			LoadRetail();
            return dmciLoadCommandResource(hInstance, lpResName, wType);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciDriverNotify(IntPtr hwndCallback, IntPtr wDeviceID, IntPtr uStatus)
        {
			LoadRetail();
            return dmciDriverNotify(hwndCallback, wDeviceID, uStatus);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciDriverYield(IntPtr wDeviceID)
        {
			LoadRetail();
            return dmciDriverYield(wDeviceID);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciGetCreatorTask(IntPtr mciId)
        {
			LoadRetail();
            return dmciGetCreatorTask(mciId);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciGetDeviceIDA(IntPtr pszDevice)
        {
			LoadRetail();
            return dmciGetDeviceIDA(pszDevice);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciGetDeviceIDFromElementIDA(IntPtr dwElementID, IntPtr lpstrType)
        {
			LoadRetail();
            return dmciGetDeviceIDFromElementIDA(dwElementID, lpstrType);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciGetDeviceIDFromElementIDW(IntPtr dwElementID, IntPtr lpstrType)
        {
			LoadRetail();
            return dmciGetDeviceIDFromElementIDW(dwElementID, lpstrType);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciGetDeviceIDW(IntPtr pszDevice)
        {
			LoadRetail();
            return dmciGetDeviceIDW(pszDevice);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciGetDriverData(IntPtr wDeviceID)
        {
			LoadRetail();
            return dmciGetDriverData(wDeviceID);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciGetYieldProc(IntPtr mciId, IntPtr pdwYieldData)
        {
			LoadRetail();
            return dmciGetYieldProc(mciId, pdwYieldData);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciSetDriverData(IntPtr wDeviceID, IntPtr dwData)
        {
			LoadRetail();
            return dmciSetDriverData(wDeviceID, dwData);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mciSetYieldProc(IntPtr mciId, IntPtr fpYieldProc, IntPtr dwYieldData)
        {
			LoadRetail();
            return dmciSetYieldProc(mciId, fpYieldProc, dwYieldData);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr PlaySoundA(IntPtr pszSound, IntPtr hmod, IntPtr fdwSound)
        {
			LoadRetail();
            return dPlaySoundA(pszSound, hmod, fdwSound);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr sndPlaySoundA(IntPtr pszSound, IntPtr fuSound)
        {
			LoadRetail();
            return dsndPlaySoundA(pszSound, fuSound);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr sndPlaySoundW(IntPtr pszSound, IntPtr fuSound)
        {
			LoadRetail();
            return dsndPlaySoundW(pszSound, fuSound);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DriverCallback(IntPtr dwCallback, IntPtr dwFlags, IntPtr hDevice, IntPtr dwMsg, IntPtr dwUser, IntPtr dwParam1, IntPtr dwParam2)
        {
			LoadRetail();
            return dDriverCallback(dwCallback, dwFlags, hDevice, dwMsg, dwUser, dwParam1, dwParam2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr WOWAppExit(IntPtr a1)
        {
			LoadRetail();
            return dWOWAppExit(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmsystemGetVersion()
        {
			LoadRetail();
            return dmmsystemGetVersion();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr timeBeginPeriod(IntPtr uPeriod)
        {
			LoadRetail();
            return dtimeBeginPeriod(uPeriod);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr timeEndPeriod(IntPtr uPeriod)
        {
			LoadRetail();
            return dtimeEndPeriod(uPeriod);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CloseDriver(IntPtr hDriver, IntPtr lParam1, IntPtr lParam2)
        {
			LoadRetail();
            return dCloseDriver(hDriver, lParam1, lParam2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DefDriverProc(IntPtr dwDriverIdentifier, IntPtr hdrvr, IntPtr uMsg, IntPtr lParam1, IntPtr lParam2)
        {
			LoadRetail();
            return dDefDriverProc(dwDriverIdentifier, hdrvr, uMsg, lParam1, lParam2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DrvGetModuleHandle(IntPtr hDriver)
        {
			LoadRetail();
            return dDrvGetModuleHandle(hDriver);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr GetDriverModuleHandle(IntPtr hDriver)
        {
			LoadRetail();
            return dGetDriverModuleHandle(hDriver);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr OpenDriver(IntPtr szDriverName, IntPtr szSectionName, IntPtr lParam2)
        {
			LoadRetail();
            return dOpenDriver(szDriverName, szSectionName, lParam2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr SendDriverMessage(IntPtr hDriver, IntPtr message, IntPtr lParam1, IntPtr lParam2)
        {
			LoadRetail();
            return dSendDriverMessage(hDriver, message, lParam1, lParam2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr auxGetDevCapsA(IntPtr uDeviceID, IntPtr pac, IntPtr cbac)
        {
			LoadRetail();
            return dauxGetDevCapsA(uDeviceID, pac, cbac);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr auxGetDevCapsW(IntPtr uDeviceID, IntPtr pac, IntPtr cbac)
        {
			LoadRetail();
            return dauxGetDevCapsW(uDeviceID, pac, cbac);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr auxGetVolume(IntPtr uDeviceID, IntPtr pdwVolume)
        {
			LoadRetail();
            return dauxGetVolume(uDeviceID, pdwVolume);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr auxOutMessage(IntPtr uDeviceID, IntPtr uMsg, IntPtr dw1, IntPtr dw2)
        {
			LoadRetail();
            return dauxOutMessage(uDeviceID, uMsg, dw1, dw2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr auxSetVolume(IntPtr uDeviceID, IntPtr dwVolume)
        {
			LoadRetail();
            return dauxSetVolume(uDeviceID, dwVolume);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiConnect(IntPtr hmi, IntPtr hmo, IntPtr pReserved)
        {
			LoadRetail();
            return dmidiConnect(hmi, hmo, pReserved);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiDisconnect(IntPtr hmi, IntPtr hmo, IntPtr pReserved)
        {
			LoadRetail();
            return dmidiDisconnect(hmi, hmo, pReserved);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiInAddBuffer(IntPtr hmi, IntPtr pmh, IntPtr cbmh)
        {
			LoadRetail();
            return dmidiInAddBuffer(hmi, pmh, cbmh);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiInClose(IntPtr hmi)
        {
			LoadRetail();
            return dmidiInClose(hmi);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiInGetDevCapsA(IntPtr uDeviceID, IntPtr pmic, IntPtr cbmic)
        {
			LoadRetail();
            return dmidiInGetDevCapsA(uDeviceID, pmic, cbmic);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiInGetDevCapsW(IntPtr uDeviceID, IntPtr pmic, IntPtr cbmic)
        {
			LoadRetail();
            return dmidiInGetDevCapsW(uDeviceID, pmic, cbmic);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiInGetErrorTextA(IntPtr mmrError, IntPtr pszText, IntPtr cchText)
        {
			LoadRetail();
            return dmidiInGetErrorTextA(mmrError, pszText, cchText);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiInGetErrorTextW(IntPtr mmrError, IntPtr pszText, IntPtr cchText)
        {
			LoadRetail();
            return dmidiInGetErrorTextW(mmrError, pszText, cchText);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiInGetID(IntPtr hmi, IntPtr puDeviceID)
        {
			LoadRetail();
            return dmidiInGetID(hmi, puDeviceID);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiInMessage(IntPtr hmi, IntPtr uMsg, IntPtr dw1, IntPtr dw2)
        {
			LoadRetail();
            return dmidiInMessage(hmi, uMsg, dw1, dw2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiInOpen(IntPtr phmi, IntPtr uDeviceID, IntPtr dwCallback, IntPtr dwInstance, IntPtr fdwOpen)
        {
			LoadRetail();
            return dmidiInOpen(phmi, uDeviceID, dwCallback, dwInstance, fdwOpen);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiInPrepareHeader(IntPtr hmi, IntPtr pmh, IntPtr cbmh)
        {
			LoadRetail();
            return dmidiInPrepareHeader(hmi, pmh, cbmh);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiInReset(IntPtr hmi)
        {
			LoadRetail();
            return dmidiInReset(hmi);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiInStart(IntPtr hmi)
        {
			LoadRetail();
            return dmidiInStart(hmi);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiInStop(IntPtr hmi)
        {
			LoadRetail();
            return dmidiInStop(hmi);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiInUnprepareHeader(IntPtr hmi, IntPtr pmh, IntPtr cbmh)
        {
			LoadRetail();
            return dmidiInUnprepareHeader(hmi, pmh, cbmh);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutCacheDrumPatches(IntPtr hmo, IntPtr uPatch, IntPtr pwkya, IntPtr fuCache)
        {
			LoadRetail();
            return dmidiOutCacheDrumPatches(hmo, uPatch, pwkya, fuCache);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutCachePatches(IntPtr hmo, IntPtr uBank, IntPtr pwpa, IntPtr fuCache)
        {
			LoadRetail();
            return dmidiOutCachePatches(hmo, uBank, pwpa, fuCache);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutClose(IntPtr hmo)
        {
			LoadRetail();
            return dmidiOutClose(hmo);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutGetDevCapsA(IntPtr uDeviceID, IntPtr pmoc, IntPtr cbmoc)
        {
			LoadRetail();
            return dmidiOutGetDevCapsA(uDeviceID, pmoc, cbmoc);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutGetDevCapsW(IntPtr uDeviceID, IntPtr pmoc, IntPtr cbmoc)
        {
			LoadRetail();
            return dmidiOutGetDevCapsW(uDeviceID, pmoc, cbmoc);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutGetErrorTextA(IntPtr mmrError, IntPtr pszText, IntPtr cchText)
        {
			LoadRetail();
            return dmidiOutGetErrorTextA(mmrError, pszText, cchText);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutGetErrorTextW(IntPtr mmrError, IntPtr pszText, IntPtr cchText)
        {
			LoadRetail();
            return dmidiOutGetErrorTextW(mmrError, pszText, cchText);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutGetID(IntPtr hmo, IntPtr puDeviceID)
        {
			LoadRetail();
            return dmidiOutGetID(hmo, puDeviceID);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutGetVolume(IntPtr hmo, IntPtr pdwVolume)
        {
			LoadRetail();
            return dmidiOutGetVolume(hmo, pdwVolume);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutLongMsg(IntPtr hmo, IntPtr pmh, IntPtr cbmh)
        {
			LoadRetail();
            return dmidiOutLongMsg(hmo, pmh, cbmh);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutMessage(IntPtr hmo, IntPtr uMsg, IntPtr dw1, IntPtr dw2)
        {
			LoadRetail();
            return dmidiOutMessage(hmo, uMsg, dw1, dw2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutOpen(IntPtr phmo, IntPtr uDeviceID, IntPtr dwCallback, IntPtr dwInstance, IntPtr fdwOpen)
        {
			LoadRetail();
            return dmidiOutOpen(phmo, uDeviceID, dwCallback, dwInstance, fdwOpen);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutPrepareHeader(IntPtr hmo, IntPtr pmh, IntPtr cbmh)
        {
			LoadRetail();
            return dmidiOutPrepareHeader(hmo, pmh, cbmh);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutReset(IntPtr hmo)
        {
			LoadRetail();
            return dmidiOutReset(hmo);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutSetVolume(IntPtr hmo, IntPtr dwVolume)
        {
			LoadRetail();
            return dmidiOutSetVolume(hmo, dwVolume);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutShortMsg(IntPtr hmo, IntPtr dwMsg)
        {
			LoadRetail();
            return dmidiOutShortMsg(hmo, dwMsg);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiOutUnprepareHeader(IntPtr hmo, IntPtr pmh, IntPtr cbmh)
        {
			LoadRetail();
            return dmidiOutUnprepareHeader(hmo, pmh, cbmh);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiStreamClose(IntPtr hms)
        {
			LoadRetail();
            return dmidiStreamClose(hms);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiStreamOpen(IntPtr phms, IntPtr puDeviceID, IntPtr cMidi, IntPtr dwCallback, IntPtr dwInstance, IntPtr fdwOpen)
        {
			LoadRetail();
            return dmidiStreamOpen(phms, puDeviceID, cMidi, dwCallback, dwInstance, fdwOpen);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiStreamOut(IntPtr hms, IntPtr pmh, IntPtr cbmh)
        {
			LoadRetail();
            return dmidiStreamOut(hms, pmh, cbmh);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiStreamPause(IntPtr hms)
        {
			LoadRetail();
            return dmidiStreamPause(hms);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiStreamPosition(IntPtr hms, IntPtr lpmmt, IntPtr cbmmt)
        {
			LoadRetail();
            return dmidiStreamPosition(hms, lpmmt, cbmmt);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiStreamProperty(IntPtr hms, IntPtr lppropdata, IntPtr dwProperty)
        {
			LoadRetail();
            return dmidiStreamProperty(hms, lppropdata, dwProperty);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiStreamRestart(IntPtr hms)
        {
			LoadRetail();
            return dmidiStreamRestart(hms);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr midiStreamStop(IntPtr hms)
        {
			LoadRetail();
            return dmidiStreamStop(hms);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mixerClose(IntPtr hmx)
        {
			LoadRetail();
            return dmixerClose(hmx);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mixerGetControlDetailsA(IntPtr hmxobj, IntPtr pmxcd, IntPtr fdwDetails)
        {
			LoadRetail();
            return dmixerGetControlDetailsA(hmxobj, pmxcd, fdwDetails);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mixerGetControlDetailsW(IntPtr hmxobj, IntPtr pmxcd, IntPtr fdwDetails)
        {
			LoadRetail();
            return dmixerGetControlDetailsW(hmxobj, pmxcd, fdwDetails);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mixerGetDevCapsA(IntPtr uMxId, IntPtr pmxcaps, IntPtr cbmxcaps)
        {
			LoadRetail();
            return dmixerGetDevCapsA(uMxId, pmxcaps, cbmxcaps);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mixerGetDevCapsW(IntPtr uMxId, IntPtr pmxcaps, IntPtr cbmxcaps)
        {
			LoadRetail();
            return dmixerGetDevCapsW(uMxId, pmxcaps, cbmxcaps);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mixerGetID(IntPtr hmxobj, IntPtr puMxId, IntPtr fdwId)
        {
			LoadRetail();
            return dmixerGetID(hmxobj, puMxId, fdwId);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mixerGetLineControlsA(IntPtr hmxobj, IntPtr pmxlc, IntPtr fdwControls)
        {
			LoadRetail();
            return dmixerGetLineControlsA(hmxobj, pmxlc, fdwControls);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mixerGetLineControlsW(IntPtr hmxobj, IntPtr pmxlc, IntPtr fdwControls)
        {
			LoadRetail();
            return dmixerGetLineControlsW(hmxobj, pmxlc, fdwControls);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mixerGetLineInfoA(IntPtr hmxobj, IntPtr pmxl, IntPtr fdwInfo)
        {
			LoadRetail();
            return dmixerGetLineInfoA(hmxobj, pmxl, fdwInfo);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mixerGetLineInfoW(IntPtr hmxobj, IntPtr pmxl, IntPtr fdwInfo)
        {
			LoadRetail();
            return dmixerGetLineInfoW(hmxobj, pmxl, fdwInfo);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mixerMessage(IntPtr hmx, IntPtr uMsg, IntPtr dwParam1, IntPtr dwParam2)
        {
			LoadRetail();
            return dmixerMessage(hmx, uMsg, dwParam1, dwParam2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mixerOpen(IntPtr phmx, IntPtr uMxId, IntPtr dwCallback, IntPtr dwInstance, IntPtr fdwOpen)
        {
			LoadRetail();
            return dmixerOpen(phmx, uMxId, dwCallback, dwInstance, fdwOpen);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mixerSetControlDetails(IntPtr hmxobj, IntPtr pmxcd, IntPtr fdwDetails)
        {
			LoadRetail();
            return dmixerSetControlDetails(hmxobj, pmxcd, fdwDetails);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmDrvInstall(IntPtr hDriver, IntPtr wszDrvEntry, IntPtr drvMessage, IntPtr wFlags)
        {
			LoadRetail();
            return dmmDrvInstall(hDriver, wszDrvEntry, drvMessage, wFlags);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmTaskCreate(IntPtr lpfn, IntPtr lph, IntPtr dwInst)
        {
			LoadRetail();
            return dmmTaskCreate(lpfn, lph, dwInst);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmTaskSignal(IntPtr h)
        {
			LoadRetail();
            return dmmTaskSignal(h);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioAdvance(IntPtr hmmio, IntPtr pmmioinfo, IntPtr fuAdvance)
        {
			LoadRetail();
            return dmmioAdvance(hmmio, pmmioinfo, fuAdvance);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioAscend(IntPtr hmmio, IntPtr pmmcki, IntPtr fuAscend)
        {
			LoadRetail();
            return dmmioAscend(hmmio, pmmcki, fuAscend);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioClose(IntPtr hmmio, IntPtr fuClose)
        {
			LoadRetail();
            return dmmioClose(hmmio, fuClose);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioCreateChunk(IntPtr hmmio, IntPtr pmmcki, IntPtr fuCreate)
        {
			LoadRetail();
            return dmmioCreateChunk(hmmio, pmmcki, fuCreate);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioDescend(IntPtr hmmio, IntPtr pmmcki, IntPtr pmmckiParent, IntPtr fuDescend)
        {
			LoadRetail();
            return dmmioDescend(hmmio, pmmcki, pmmckiParent, fuDescend);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioFlush(IntPtr hmmio, IntPtr fuFlush)
        {
			LoadRetail();
            return dmmioFlush(hmmio, fuFlush);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioGetInfo(IntPtr hmmio, IntPtr pmmioinfo, IntPtr fuInfo)
        {
			LoadRetail();
            return dmmioGetInfo(hmmio, pmmioinfo, fuInfo);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioInstallIOProcA(IntPtr fccIOProc, IntPtr pIOProc, IntPtr dwFlags)
        {
			LoadRetail();
            return dmmioInstallIOProcA(fccIOProc, pIOProc, dwFlags);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioInstallIOProcW(IntPtr fccIOProc, IntPtr pIOProc, IntPtr dwFlags)
        {
			LoadRetail();
            return dmmioInstallIOProcW(fccIOProc, pIOProc, dwFlags);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioOpenA(IntPtr pszFileName, IntPtr pmmioinfo, IntPtr fdwOpen)
        {
			LoadRetail();
            return dmmioOpenA(pszFileName, pmmioinfo, fdwOpen);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioOpenW(IntPtr pszFileName, IntPtr pmmioinfo, IntPtr fdwOpen)
        {
			LoadRetail();
            return dmmioOpenW(pszFileName, pmmioinfo, fdwOpen);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioRead(IntPtr hmmio, IntPtr pch, IntPtr cch)
        {
			LoadRetail();
            return dmmioRead(hmmio, pch, cch);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioRenameA(IntPtr pszFileName, IntPtr pszNewFileName, IntPtr pmmioinfo, IntPtr fdwRename)
        {
			LoadRetail();
            return dmmioRenameA(pszFileName, pszNewFileName, pmmioinfo, fdwRename);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioRenameW(IntPtr pszFileName, IntPtr pszNewFileName, IntPtr pmmioinfo, IntPtr fdwRename)
        {
			LoadRetail();
            return dmmioRenameW(pszFileName, pszNewFileName, pmmioinfo, fdwRename);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioSeek(IntPtr hmmio, IntPtr lOffset, IntPtr iOrigin)
        {
			LoadRetail();
            return dmmioSeek(hmmio, lOffset, iOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioSendMessage(IntPtr hmmio, IntPtr uMsg, IntPtr lParam1, IntPtr lParam2)
        {
			LoadRetail();
            return dmmioSendMessage(hmmio, uMsg, lParam1, lParam2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioSetBuffer(IntPtr hmmio, IntPtr pchBuffer, IntPtr cchBuffer, IntPtr fuBuffer)
        {
			LoadRetail();
            return dmmioSetBuffer(hmmio, pchBuffer, cchBuffer, fuBuffer);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioSetInfo(IntPtr hmmio, IntPtr pmmioinfo, IntPtr fuInfo)
        {
			LoadRetail();
            return dmmioSetInfo(hmmio, pmmioinfo, fuInfo);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioStringToFOURCCA(IntPtr sz, IntPtr uFlags)
        {
			LoadRetail();
            return dmmioStringToFOURCCA(sz, uFlags);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioStringToFOURCCW(IntPtr sz, IntPtr uFlags)
        {
			LoadRetail();
            return dmmioStringToFOURCCW(sz, uFlags);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr mmioWrite(IntPtr hmmio, IntPtr pch, IntPtr cch)
        {
			LoadRetail();
            return dmmioWrite(hmmio, pch, cch);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr timeGetDevCaps(IntPtr ptc, IntPtr cbtc)
        {
			LoadRetail();
            return dtimeGetDevCaps(ptc, cbtc);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr timeGetSystemTime(IntPtr pmmt, IntPtr cbmmt)
        {
			LoadRetail();
            return dtimeGetSystemTime(pmmt, cbmmt);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInAddBuffer(IntPtr hwi, IntPtr pwh, IntPtr cbwh)
        {
			LoadRetail();
            return dwaveInAddBuffer(hwi, pwh, cbwh);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInClose(IntPtr hwi)
        {
			LoadRetail();
            return dwaveInClose(hwi);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInGetDevCapsA(IntPtr uDeviceID, IntPtr pwic, IntPtr cbwic)
        {
			LoadRetail();
            return dwaveInGetDevCapsA(uDeviceID, pwic, cbwic);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInGetDevCapsW(IntPtr uDeviceID, IntPtr pwic, IntPtr cbwic)
        {
			LoadRetail();
            return dwaveInGetDevCapsW(uDeviceID, pwic, cbwic);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInGetErrorTextA(IntPtr mmrError, IntPtr pszText, IntPtr cchText)
        {
			LoadRetail();
            return dwaveInGetErrorTextA(mmrError, pszText, cchText);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInGetErrorTextW(IntPtr mmrError, IntPtr pszText, IntPtr cchText)
        {
			LoadRetail();
            return dwaveInGetErrorTextW(mmrError, pszText, cchText);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInGetID(IntPtr hwi, IntPtr puDeviceID)
        {
			LoadRetail();
            return dwaveInGetID(hwi, puDeviceID);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInGetPosition(IntPtr hwi, IntPtr pmmt, IntPtr cbmmt)
        {
			LoadRetail();
            return dwaveInGetPosition(hwi, pmmt, cbmmt);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInMessage(IntPtr hwi, IntPtr uMsg, IntPtr dw1, IntPtr dw2)
        {
			LoadRetail();
            return dwaveInMessage(hwi, uMsg, dw1, dw2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInOpen(IntPtr phwi, IntPtr uDeviceID, IntPtr pwfx, IntPtr dwCallback, IntPtr dwInstance, IntPtr fdwOpen)
        {
			LoadRetail();
            return dwaveInOpen(phwi, uDeviceID, pwfx, dwCallback, dwInstance, fdwOpen);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInPrepareHeader(IntPtr hwi, IntPtr pwh, IntPtr cbwh)
        {
			LoadRetail();
            return dwaveInPrepareHeader(hwi, pwh, cbwh);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInReset(IntPtr hwi)
        {
			LoadRetail();
            return dwaveInReset(hwi);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInStart(IntPtr hwi)
        {
			LoadRetail();
            return dwaveInStart(hwi);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInStop(IntPtr hwi)
        {
			LoadRetail();
            return dwaveInStop(hwi);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInUnprepareHeader(IntPtr hwi, IntPtr pwh, IntPtr cbwh)
        {
			LoadRetail();
            return dwaveInUnprepareHeader(hwi, pwh, cbwh);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutBreakLoop(IntPtr hwo)
        {
			LoadRetail();
            return dwaveOutBreakLoop(hwo);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutClose(IntPtr hwo)
        {
			LoadRetail();
            return dwaveOutClose(hwo);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutGetDevCapsA(IntPtr uDeviceID, IntPtr pwoc, IntPtr cbwoc)
        {
			LoadRetail();
            return dwaveOutGetDevCapsA(uDeviceID, pwoc, cbwoc);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutGetDevCapsW(IntPtr uDeviceID, IntPtr pwoc, IntPtr cbwoc)
        {
			LoadRetail();
            return dwaveOutGetDevCapsW(uDeviceID, pwoc, cbwoc);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutGetNumDevs()
        {
            LoadRetail();
            return dwaveOutGetNumDevs();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveInGetNumDevs()
        {
            LoadRetail();
            return dwaveInGetNumDevs();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutGetErrorTextA(IntPtr mmrError, IntPtr pszText, IntPtr cchText)
        {
			LoadRetail();
            return dwaveOutGetErrorTextA(mmrError, pszText, cchText);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutGetErrorTextW(IntPtr mmrError, IntPtr pszText, IntPtr cchText)
        {
			LoadRetail();
            return dwaveOutGetErrorTextW(mmrError, pszText, cchText);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutGetID(IntPtr hwo, IntPtr puDeviceID)
        {
			LoadRetail();
            return dwaveOutGetID(hwo, puDeviceID);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutGetPitch(IntPtr hwo, IntPtr pdwPitch)
        {
			LoadRetail();
            return dwaveOutGetPitch(hwo, pdwPitch);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutGetPlaybackRate(IntPtr hwo, IntPtr pdwRate)
        {
			LoadRetail();
            return dwaveOutGetPlaybackRate(hwo, pdwRate);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutGetPosition(IntPtr hwo, IntPtr pmmt, IntPtr cbmmt)
        {
			LoadRetail();
            return dwaveOutGetPosition(hwo, pmmt, cbmmt);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutGetVolume(IntPtr hwo, IntPtr pdwVolume)
        {
			LoadRetail();
            return dwaveOutGetVolume(hwo, pdwVolume);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutMessage(IntPtr hwo, IntPtr uMsg, IntPtr dw1, IntPtr dw2)
        {
			LoadRetail();
            return dwaveOutMessage(hwo, uMsg, dw1, dw2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutOpen(IntPtr phwo, IntPtr uDeviceID, IntPtr pwfx, IntPtr dwCallback, IntPtr dwInstance, IntPtr fdwOpen)
        {
			LoadRetail();
            return dwaveOutOpen(phwo, uDeviceID, pwfx, dwCallback, dwInstance, fdwOpen);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutPause(IntPtr hwo)
        {
			LoadRetail();
            return dwaveOutPause(hwo);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutPrepareHeader(IntPtr hwo, IntPtr pwh, IntPtr cbwh)
        {
			LoadRetail();
            return dwaveOutPrepareHeader(hwo, pwh, cbwh);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutReset(IntPtr hwo)
        {
			LoadRetail();
            return dwaveOutReset(hwo);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutRestart(IntPtr hwo)
        {
			LoadRetail();
            return dwaveOutRestart(hwo);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutSetPitch(IntPtr hwo, IntPtr dwPitch)
        {
			LoadRetail();
            return dwaveOutSetPitch(hwo, dwPitch);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutSetPlaybackRate(IntPtr hwo, IntPtr dwRate)
        {
			LoadRetail();
            return dwaveOutSetPlaybackRate(hwo, dwRate);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutSetVolume(IntPtr hwo, IntPtr dwVolume)
        {
			LoadRetail();
            return dwaveOutSetVolume(hwo, dwVolume);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutUnprepareHeader(IntPtr hwo, IntPtr pwh, IntPtr cbwh)
        {
			LoadRetail();
            return dwaveOutUnprepareHeader(hwo, pwh, cbwh);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr waveOutWrite(IntPtr hwo, IntPtr pwh, IntPtr cbwh)
        {
			LoadRetail();
            return dwaveOutWrite(hwo, pwh, cbwh);
        }


        static RET_3 dPlaySoundW;
        static RET_5 dtimeSetEvent;
        static RET_1 dtimeKillEvent;
        static RET_0 djoyGetNumDevs;
        static RET_1 djoyConfigChanged;
        static RET_3 djoyGetDevCapsA;
        static RET_3 djoyGetDevCapsW;
        static RET_2 djoyGetPos;
        static RET_2 djoyGetPosEx;
        static RET_2 djoyGetThreshold;
        static RET_1 djoyReleaseCapture;
        static RET_4 djoySetCapture;
        static RET_2 djoySetThreshold;
        static RET_5 dNotifyCallbackData;
        static RET_7 dWOW32DriverCallback;
        static RET_6 dWOW32ResolveMultiMediaHandle;
        static RET_5 daux32Message;
        static RET_5 djoy32Message;
        static RET_5 dmid32Message;
        static RET_5 dmod32Message;
        static RET_5 dmxd32Message;
        static RET_5 dtid32Message;
        static RET_5 dwid32Message;
        static RET_5 dwod32Message;
        static RET_1 dmciExecute;
        static RET_3 dmciGetErrorStringA;
        static RET_3 dmciGetErrorStringW;
        static RET_4 dmciSendCommandA;
        static RET_4 dmciSendCommandW;
        static RET_4 dmciSendStringA;
        static RET_4 dmciSendStringW;
        static RET_1 dmciFreeCommandResource;
        static RET_3 dmciLoadCommandResource;
        static RET_3 dmciDriverNotify;
        static RET_1 dmciDriverYield;
        static RET_1 dmciGetCreatorTask;
        static RET_1 dmciGetDeviceIDA;
        static RET_2 dmciGetDeviceIDFromElementIDA;
        static RET_2 dmciGetDeviceIDFromElementIDW;
        static RET_1 dmciGetDeviceIDW;
        static RET_1 dmciGetDriverData;
        static RET_2 dmciGetYieldProc;
        static RET_2 dmciSetDriverData;
        static RET_3 dmciSetYieldProc;
        static RET_3 dPlaySoundA;
        static RET_2 dsndPlaySoundA;
        static RET_2 dsndPlaySoundW;
        static RET_7 dDriverCallback;
        static RET_1 dWOWAppExit;
        static RET_0 dmmsystemGetVersion;
        static RET_1 dtimeBeginPeriod;
        static RET_1 dtimeEndPeriod;
        static RET_3 dCloseDriver;
        static RET_5 dDefDriverProc;
        static RET_1 dDrvGetModuleHandle;
        static RET_1 dGetDriverModuleHandle;
        static RET_3 dOpenDriver;
        static RET_4 dSendDriverMessage;
        static RET_3 dauxGetDevCapsA;
        static RET_3 dauxGetDevCapsW;
        static RET_2 dauxGetVolume;
        static RET_4 dauxOutMessage;
        static RET_2 dauxSetVolume;
        static RET_3 dmidiConnect;
        static RET_3 dmidiDisconnect;
        static RET_3 dmidiInAddBuffer;
        static RET_1 dmidiInClose;
        static RET_3 dmidiInGetDevCapsA;
        static RET_3 dmidiInGetDevCapsW;
        static RET_3 dmidiInGetErrorTextA;
        static RET_3 dmidiInGetErrorTextW;
        static RET_2 dmidiInGetID;
        static RET_4 dmidiInMessage;
        static RET_5 dmidiInOpen;
        static RET_3 dmidiInPrepareHeader;
        static RET_1 dmidiInReset;
        static RET_1 dmidiInStart;
        static RET_1 dmidiInStop;
        static RET_3 dmidiInUnprepareHeader;
        static RET_4 dmidiOutCacheDrumPatches;
        static RET_4 dmidiOutCachePatches;
        static RET_1 dmidiOutClose;
        static RET_3 dmidiOutGetDevCapsA;
        static RET_3 dmidiOutGetDevCapsW;
        static RET_3 dmidiOutGetErrorTextA;
        static RET_3 dmidiOutGetErrorTextW;
        static RET_2 dmidiOutGetID;
        static RET_2 dmidiOutGetVolume;
        static RET_3 dmidiOutLongMsg;
        static RET_4 dmidiOutMessage;
        static RET_5 dmidiOutOpen;
        static RET_3 dmidiOutPrepareHeader;
        static RET_1 dmidiOutReset;
        static RET_2 dmidiOutSetVolume;
        static RET_2 dmidiOutShortMsg;
        static RET_3 dmidiOutUnprepareHeader;
        static RET_1 dmidiStreamClose;
        static RET_6 dmidiStreamOpen;
        static RET_3 dmidiStreamOut;
        static RET_1 dmidiStreamPause;
        static RET_3 dmidiStreamPosition;
        static RET_3 dmidiStreamProperty;
        static RET_1 dmidiStreamRestart;
        static RET_1 dmidiStreamStop;
        static RET_1 dmixerClose;
        static RET_3 dmixerGetControlDetailsA;
        static RET_3 dmixerGetControlDetailsW;
        static RET_3 dmixerGetDevCapsA;
        static RET_3 dmixerGetDevCapsW;
        static RET_3 dmixerGetID;
        static RET_3 dmixerGetLineControlsA;
        static RET_3 dmixerGetLineControlsW;
        static RET_3 dmixerGetLineInfoA;
        static RET_3 dmixerGetLineInfoW;
        static RET_4 dmixerMessage;
        static RET_5 dmixerOpen;
        static RET_3 dmixerSetControlDetails;
        static RET_4 dmmDrvInstall;
        static RET_3 dmmTaskCreate;
        static RET_1 dmmTaskSignal;
        static RET_3 dmmioAdvance;
        static RET_3 dmmioAscend;
        static RET_2 dmmioClose;
        static RET_3 dmmioCreateChunk;
        static RET_4 dmmioDescend;
        static RET_2 dmmioFlush;
        static RET_3 dmmioGetInfo;
        static RET_3 dmmioInstallIOProcA;
        static RET_3 dmmioInstallIOProcW;
        static RET_3 dmmioOpenA;
        static RET_3 dmmioOpenW;
        static RET_3 dmmioRead;
        static RET_4 dmmioRenameA;
        static RET_4 dmmioRenameW;
        static RET_3 dmmioSeek;
        static RET_4 dmmioSendMessage;
        static RET_4 dmmioSetBuffer;
        static RET_3 dmmioSetInfo;
        static RET_2 dmmioStringToFOURCCA;
        static RET_2 dmmioStringToFOURCCW;
        static RET_3 dmmioWrite;
        static RET_0 dtimeGetTime;
        static RET_2 dtimeGetDevCaps;
        static RET_2 dtimeGetSystemTime;
        static RET_3 dwaveInAddBuffer;
        static RET_1 dwaveInClose;
        static RET_3 dwaveInGetDevCapsA;
        static RET_3 dwaveInGetDevCapsW;
        static RET_0 dwaveInGetNumDevs;
        static RET_3 dwaveInGetErrorTextA;
        static RET_3 dwaveInGetErrorTextW;
        static RET_2 dwaveInGetID;
        static RET_3 dwaveInGetPosition;
        static RET_4 dwaveInMessage;
        static RET_6 dwaveInOpen;
        static RET_3 dwaveInPrepareHeader;
        static RET_1 dwaveInReset;
        static RET_1 dwaveInStart;
        static RET_1 dwaveInStop;
        static RET_3 dwaveInUnprepareHeader;
        static RET_1 dwaveOutBreakLoop;
        static RET_1 dwaveOutClose;
        static RET_3 dwaveOutGetDevCapsA;
        static RET_3 dwaveOutGetDevCapsW;
        static RET_0 dwaveOutGetNumDevs;
        static RET_3 dwaveOutGetErrorTextA;
        static RET_3 dwaveOutGetErrorTextW;
        static RET_2 dwaveOutGetID;
        static RET_2 dwaveOutGetPitch;
        static RET_2 dwaveOutGetPlaybackRate;
        static RET_3 dwaveOutGetPosition;
        static RET_2 dwaveOutGetVolume;
        static RET_4 dwaveOutMessage;
        static RET_6 dwaveOutOpen;
        static RET_1 dwaveOutPause;
        static RET_3 dwaveOutPrepareHeader;
        static RET_1 dwaveOutReset;
        static RET_1 dwaveOutRestart;
        static RET_2 dwaveOutSetPitch;
        static RET_2 dwaveOutSetPlaybackRate;
        static RET_2 dwaveOutSetVolume;
        static RET_3 dwaveOutUnprepareHeader;
        static RET_3 dwaveOutWrite;

    }
}
