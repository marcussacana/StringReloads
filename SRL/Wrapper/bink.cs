using System;
using System.Runtime.InteropServices;
using static SRL.Wrapper.Tools;

namespace SRL.Wrapper {

    /// <summary>
    /// This is a wrapper to the bink2w32.dll or bink2w64.dll
    /// </summary>
    public static class BINK {

        public static IntPtr RealHandler;
        public static void LoadRetail() {
            if (RealHandler != IntPtr.Zero)
                return;

            try {
                StringReloader.ProcessReal(IntPtr.Zero);
            } catch { }

            RealHandler = LoadLibrary(Environment.Is64BitProcess ? "bink2w64.dll" : "bink2w32.dll");

            if (RealHandler == IntPtr.Zero)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED


            _BinkFreeGlobals0 = GetDelegate<NULL_0>(RealHandler, "_BinkFreeGlobals@0");

            _BinkLoadUnload1 =    GetDelegate<NULL_1>(RealHandler, "_BinkLoadUnload@1");
            _BinkSetError1 =      GetDelegate<NULL_1>(RealHandler, "_BinkSetError@1");
            _BinkNextFrame1 =     GetDelegate<NULL_1>(RealHandler, "_BinkNextFrame@1");
            _BinkClose1 =         GetDelegate<NULL_1>(RealHandler, "_BinkClose@1");
            _BinkService1 =       GetDelegate<NULL_1>(RealHandler, "_BinkService@1");
            _BinkGetPalette1 =    GetDelegate<NULL_1>(RealHandler, "_BinkGetPalette@1");
            _BinkCloseTrack1 =    GetDelegate<NULL_1>(RealHandler, "_BinkCloseTrack@1");
            _BinkSetIO1 =         GetDelegate<NULL_1>(RealHandler, "_BinkSetIO@1");
            _BinkSetSimulate1 =   GetDelegate<NULL_1>(RealHandler, "_BinkSetSimulate@1");
            _BinkSetIOSize1 =     GetDelegate<NULL_1>(RealHandler, "_BinkSetIOSize@1");
            _BinkBufferClose1 =   GetDelegate<NULL_1>(RealHandler, "_BinkBufferClose@1");
            _BinkRestoreCursor1 = GetDelegate<NULL_1>(RealHandler, "_BinkRestoreCursor@1");

            _BinkLoadUnloadConverter2 =  GetDelegate<NULL_2>(RealHandler, "_BinkLoadUnloadConverter@2");
            _BinkGetFrameBuffersInfo2 =  GetDelegate<NULL_2>(RealHandler, "_BinkGetFrameBuffersInfo@2");
            _BinkRegisterFrameBuffers2 = GetDelegate<NULL_2>(RealHandler, "_BinkRegisterFrameBuffers@2");
            _BinkGetSummary2 =           GetDelegate<NULL_2>(RealHandler, "_BinkGetSummary@2");
            _BinkSetSoundTrack2 =        GetDelegate<NULL_2>(RealHandler, "_BinkSetSoundTrack@2");
            _BinkSetFrameRate2 =         GetDelegate<NULL_2>(RealHandler, "_BinkSetFrameRate@2");
            _BinkSetMemory2 =            GetDelegate<NULL_2>(RealHandler, "_BinkSetMemory@2");
            _BinkGoto3 =                 GetDelegate<NULL_3>(RealHandler, "_BinkGoto@3");

            _BinkSetVolume3 =           GetDelegate<NULL_3>(RealHandler, "_BinkSetVolume@3");
            _BinkSetPan3 =              GetDelegate<NULL_3>(RealHandler, "_BinkSetPan@3");
            _BinkGetRealtime3 =         GetDelegate<NULL_3>(RealHandler, "_BinkGetRealtime@3");
            _BinkBufferSetResolution3 = GetDelegate<NULL_3>(RealHandler, "_BinkBufferSetResolution@3");
            _BinkBufferCheckWinPos3 =   GetDelegate<NULL_3>(RealHandler, "_BinkBufferCheckWinPos@3");
            _BinkBufferBlit3 =          GetDelegate<NULL_3>(RealHandler, "_BinkBufferBlit@3");

            _BinkSetMixBins4 =       GetDelegate<NULL_4>(RealHandler, "_BinkSetMixBins@4");
            _BinkSetMixBinVolumes5 = GetDelegate<NULL_5>(RealHandler, "_BinkSetMixBinVolumes@5");


            _BinkLogoAddress0 =    GetDelegate<RET_0>(RealHandler, "_BinkLogoAddress@0");
            _BinkGetError0 =       GetDelegate<RET_0>(RealHandler, "_BinkGetError@0");
            _BinkBufferGetError0 = GetDelegate<RET_0>(RealHandler, "_BinkBufferGetError@0");

            _BinkDoFrame1 =              GetDelegate<RET_1>(RealHandler, "_BinkDoFrame@1");
            _BinkWait1 =                 GetDelegate<RET_1>(RealHandler, "_BinkWait@1");
            _BinkShouldSkip1 =           GetDelegate<RET_1>(RealHandler, "_BinkShouldSkip@1");
            _BinkOpenDirectSound1 =      GetDelegate<RET_1>(RealHandler, "_BinkOpenDirectSound@1");
            _BinkOpenWaveOut1 =          GetDelegate<RET_1>(RealHandler, "_BinkOpenWaveOut@1");
            _BinkOpenXAudio1 =           GetDelegate<RET_1>(RealHandler, "_BinkOpenXAudio@1");
            _BinkOpenMiles1 =            GetDelegate<RET_1>(RealHandler, "_BinkOpenMiles@1");
            _BinkOpenSoundManager1 =     GetDelegate<RET_1>(RealHandler, "_BinkOpenSoundManager@1");
            _BinkOpenSDLMixer1 =         GetDelegate<RET_1>(RealHandler, "_BinkOpenSDLMixer@1");
            _BinkOpenAX1 =               GetDelegate<RET_1>(RealHandler, "_BinkOpenAX@1");
            _BinkOpenMusyXSound1 =       GetDelegate<RET_1>(RealHandler, "_BinkOpenMusyXSound@1");
            _BinkOpenRAD_IOP1 =          GetDelegate<RET_1>(RealHandler, "_BinkOpenRAD_IOP@1");
            _BinkOpenLibAudio1 =         GetDelegate<RET_1>(RealHandler, "_BinkOpenLibAudio@1");
            _BinkOpenPSPSound1 =         GetDelegate<RET_1>(RealHandler, "_BinkOpenPSPSound@1");
            _BinkOpenNDSSound1 =         GetDelegate<RET_1>(RealHandler, "_BinkOpenNDSSound@1");
            _BinkGDSurfaceType1 =        GetDelegate<RET_1>(RealHandler, "_BinkGDSurfaceType@1");
            _BinkIsSoftwareCursor1 =     GetDelegate<RET_1>(RealHandler, "_BinkIsSoftwareCursor@1");
            _BinkDDSurfaceType1 =        GetDelegate<RET_1>(RealHandler, "_BinkDDSurfaceType@1");
            _BinkDX8SurfaceType1 =       GetDelegate<RET_1>(RealHandler, "_BinkDX8SurfaceType@1");
            _BinkDX9SurfaceType1 =       GetDelegate<RET_1>(RealHandler, "_BinkDX9SurfaceType@1");
            _BinkBufferLock1 =           GetDelegate<RET_1>(RealHandler, "_BinkBufferLock@1");
            _BinkBufferUnlock1 =         GetDelegate<RET_1>(RealHandler, "_BinkBufferUnlock@1");
            _BinkBufferGetDescription1 = GetDelegate<RET_1>(RealHandler, "_BinkBufferGetDescription@1");
            _BinkWaitStopAsyncThread1 =  GetDelegate<RET_1>(RealHandler, "_BinkWaitStopAsyncThread@1");

            _BinkMacOpen2 =                 GetDelegate<RET_2>(RealHandler, "_BinkMacOpen@2");
            _BinkNDSOpen2 =                 GetDelegate<RET_2>(RealHandler, "_BinkNDSOpen@2");
            _BinkOpen2 =                    GetDelegate<RET_2>(RealHandler, "_BinkOpen@2");
            _BinkPause2 =                   GetDelegate<RET_2>(RealHandler, "_BinkPause@2");
            _BinkGetRects2 =                GetDelegate<RET_2>(RealHandler, "_BinkGetRects@2");
            _BinkSetVideoOnOff2 =           GetDelegate<RET_2>(RealHandler, "_BinkSetVideoOnOff@2");
            _BinkSetSoundOnOff2 =           GetDelegate<RET_2>(RealHandler, "_BinkSetSoundOnOff@2");
            _BinkControlBackgroundIO2 =     GetDelegate<RET_2>(RealHandler, "_BinkControlBackgroundIO@2");
            _BinkOpenTrack2 =               GetDelegate<RET_2>(RealHandler, "_BinkOpenTrack@2");
            _BinkGetTrackData2 =            GetDelegate<RET_2>(RealHandler, "_BinkGetTrackData@2");
            _BinkGetTrackType2 =            GetDelegate<RET_2>(RealHandler, "_BinkGetTrackType@2");
            _BinkGetTrackMaxSize2 =         GetDelegate<RET_2>(RealHandler, "_BinkGetTrackMaxSize@2");
            _BinkGetTrackID2 =              GetDelegate<RET_2>(RealHandler, "_BinkGetTrackID@2");
            _BinkSetSoundSystem2 =          GetDelegate<RET_2>(RealHandler, "_BinkSetSoundSystem@2");
            _BinkControlPlatformFeatures2 = GetDelegate<RET_2>(RealHandler, "_BinkControlPlatformFeatures@2");
            _BinkBufferSetHWND2 =           GetDelegate<RET_2>(RealHandler, "_BinkBufferSetHWND@2");
            _BinkIsSoftwareCursor2 =        GetDelegate<RET_2>(RealHandler, "_BinkIsSoftwareCursor@2");
            _BinkBufferSetDirectDraw2 =     GetDelegate<RET_2>(RealHandler, "_BinkBufferSetDirectDraw@2");
            _BinkBufferClear2 =             GetDelegate<RET_2>(RealHandler, "_BinkBufferClear@2");

            _BinkGetKeyFrame3 =     GetDelegate<RET_3>(RealHandler, "_BinkGetKeyFrame@3");
            _BinkBufferSetOffset3 = GetDelegate<RET_3>(RealHandler, "_BinkBufferSetOffset@3");
            _BinkBufferSetScale3 =  GetDelegate<RET_3>(RealHandler, "_BinkBufferSetScale@3");

            _BinkOpenXAudio24 = GetDelegate<RET_4>(RealHandler, "_BinkOpenXAudio2@4");
            _BinkBufferOpen4 =  GetDelegate<RET_4>(RealHandler, "_BinkBufferOpen@4");

            _BinkRequestStopAsyncThreadsMulti8 = GetDelegate<RET_8>(RealHandler, "_BinkRequestStopAsyncThreadsMulti@8");
            _BinkWaitStopAsyncThreadsMulti8 =    GetDelegate<RET_8>(RealHandler, "_BinkWaitStopAsyncThreadsMulti@8");
            _BinkDoFrameAsyncWait8 =             GetDelegate<RET_8>(RealHandler, "_BinkDoFrameAsyncWait@8");
            _BinkSetSoundSystem8 =               GetDelegate<RET_8>(RealHandler, "_BinkSetSoundSystem@8");
            _BinkStartAsyncThread8 =             GetDelegate<RET_8>(RealHandler, "_BinkStartAsyncThread@8");

            _BinkCheckCursor5 =        GetDelegate<RET_5>(RealHandler, "_BinkCheckCursor@5");
            _BinkCopyToBuffer7 =       GetDelegate<RET_7>(RealHandler, "_BinkCopyToBuffer@7");
            _BinkCopyToBufferRect11 =  GetDelegate<RET_11>(RealHandler, "_BinkCopyToBufferRect@11");

            //Bink2 - Undocumented
            _BinkClose4 =              GetDelegate<NULL_4>(RealHandler, "_BinkClose@4");
            _BinkUtilCPUs0 =           GetDelegate<RET_0>(RealHandler, "_BinkUtilCPUs@0");
            _BinkNextFrame4 =          GetDelegate<RET_4>(RealHandler, "_BinkNextFrame@4");
            _BinkShouldSkip4 =         GetDelegate<RET_4>(RealHandler, "_BinkShouldSkip@4");
            _BinkWait4 =               GetDelegate<RET_4>(RealHandler, "_BinkWait@4");
            _BinkOpen8 =               GetDelegate<RET_8>(RealHandler, "_BinkOpen@8");
            _BinkDoFrameAsyncMulti12 = GetDelegate<RET_12>(RealHandler, "_BinkDoFrameAsyncMulti@12");
            _BinkSetVolume12 =         GetDelegate<NULL_12>(RealHandler, "_BinkSetVolume@12");
            _BinkCopyToBuffer28 =      GetDelegate<RET_28>(RealHandler, "_BinkCopyToBuffer@28");
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkLoadUnload@1")]
        public static void BinkLoadUnload(IntPtr inout) {
            LoadRetail();

            _BinkLoadUnload1(inout);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkLoadUnloadConverter@2")]
        public static void BinkLoadUnloadConverter(IntPtr surfaces, IntPtr inout) {
            LoadRetail();

            _BinkLoadUnloadConverter2(surfaces, inout);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkMacOpen@2")]
        public static IntPtr BinkMacOpen(IntPtr /*FSSpec*/ fsp, IntPtr flags) {
            LoadRetail();

            return _BinkMacOpen2(fsp, flags);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkNDSOpen@2")]

        public static IntPtr BinkNDSOpen(IntPtr /*FSFileID*/ fid, IntPtr flags) {
            LoadRetail();

            return _BinkNDSOpen2(fid, flags);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkLogoAddress@0")]
        public static IntPtr BinkLogoAddress() {
            LoadRetail();

            return _BinkLogoAddress0();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetError@1")]
        public static void BinkSetError(IntPtr err) {
            LoadRetail();

            _BinkSetError1(err);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkGetError@0")]
        public static IntPtr BinkGetError() {
            LoadRetail();

            return _BinkGetError0();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpen@2")]
        public static IntPtr BinkOpen(IntPtr name, IntPtr flags) {
            LoadRetail();

            return _BinkOpen2(name, flags);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkGetFrameBuffersInfo@2")]
        public static void BinkGetFrameBuffersInfo(IntPtr bink, IntPtr fbset) {
            LoadRetail();

            _BinkGetFrameBuffersInfo2(bink, fbset);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkRegisterFrameBuffers@2")]
        public static void BinkRegisterFrameBuffers(IntPtr bink, IntPtr fbset) {
            LoadRetail();

            _BinkRegisterFrameBuffers2(bink, fbset);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkDoFrame@1")]
        public static IntPtr BinkDoFrame(IntPtr bnk) {
            LoadRetail();

            return _BinkDoFrame1(bnk);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkNextFrame@1")]
        public static void BinkNextFrame(IntPtr bnk) {
            LoadRetail();

            _BinkNextFrame1(bnk);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkWait@1")]
        public static IntPtr BinkWait(IntPtr bnk) {
            LoadRetail();

            return _BinkWait1(bnk);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkClose@1")]
        public static void BinkClose(IntPtr bnk) {
            LoadRetail();

            _BinkClose1(bnk);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkPause@2")]
        public static IntPtr BinkPause(IntPtr bnk, IntPtr pause) {
            LoadRetail();

            return _BinkPause2(bnk, pause);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkCopyToBuffer@7")]
        public static IntPtr BinkCopyToBuffer(IntPtr bnk, IntPtr dest, IntPtr destpitch, IntPtr destheight, IntPtr destx, IntPtr desty, IntPtr flags) {
            LoadRetail();

            return _BinkCopyToBuffer7(bnk, dest, destpitch, destheight, destx, desty, flags);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkCopyToBufferRect@11")]
        public static IntPtr BinkCopyToBufferRect(IntPtr bnk, IntPtr dest, IntPtr destpitch, IntPtr destheight, IntPtr destx, IntPtr desty, IntPtr srcx, IntPtr srcy, IntPtr srcw, IntPtr srch, IntPtr flags) {
            LoadRetail();

            return _BinkCopyToBufferRect11(bnk, dest, destpitch, destheight, destx, desty, srcx, srcy, srcw, srch, flags);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkGetRects@2")]
        public static IntPtr BinkGetRects(IntPtr bnk, IntPtr flags) {
            LoadRetail();

            return _BinkGetRects2(bnk, flags);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkGoto@3")]
        public static void BinkGoto(IntPtr bnk, IntPtr frame, IntPtr flags) {
            LoadRetail();

            _BinkGoto3(bnk, frame, flags);
        } // use 1 for the first frame

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkGetKeyFrame@3")]
        public static IntPtr BinkGetKeyFrame(IntPtr bnk, IntPtr frame, IntPtr flags) {
            LoadRetail();

            return _BinkGetKeyFrame3(bnk, frame, flags);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetVideoOnOff@2")]
        public static IntPtr BinkSetVideoOnOff(IntPtr bnk, IntPtr onoff) {
            LoadRetail();

            return _BinkSetVideoOnOff2(bnk, onoff);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetSoundOnOff@2")]
        public static IntPtr BinkSetSoundOnOff(IntPtr bnk, IntPtr onoff) {
            LoadRetail();

            return _BinkSetSoundOnOff2(bnk, onoff);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetVolume@3")]
        public static void BinkSetVolume(IntPtr bnk, IntPtr trackid, IntPtr volume) {
            LoadRetail();

            _BinkSetVolume3(bnk, trackid, volume);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetPan@3")]
        public static void BinkSetPan(IntPtr bnk, IntPtr trackid, IntPtr pan) {
            LoadRetail();

            _BinkSetPan3(bnk, trackid, pan);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetMixBins@4")]
        public static void BinkSetMixBins(IntPtr bnk, IntPtr trackid, IntPtr mix_bins, IntPtr total) {
            LoadRetail();

            _BinkSetMixBins4(bnk, trackid, mix_bins, total);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetMixBinVolumes@5")]
        public static void BinkSetMixBinVolumes(IntPtr bnk, IntPtr trackid, IntPtr vol_mix_bins, IntPtr volumes, IntPtr total) {
            LoadRetail();

            _BinkSetMixBinVolumes5(bnk, trackid, vol_mix_bins, volumes, total);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkService@1")]
        public static void BinkService(IntPtr bink) {
            LoadRetail();

            _BinkService1(bink);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkShouldSkip@1")]
        public static IntPtr BinkShouldSkip(IntPtr bink) {
            LoadRetail();

            return _BinkShouldSkip1(bink);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkGetPalette@1")]
        public static void BinkGetPalette(IntPtr out_pal) {
            LoadRetail();

            _BinkGetPalette1(out_pal);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkControlBackgroundIO@2")]
        public static IntPtr BinkControlBackgroundIO(IntPtr bink, IntPtr control) {
            LoadRetail();

            return _BinkControlBackgroundIO2(bink, control);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpenTrack@2")]
        public static IntPtr BinkOpenTrack(IntPtr bnk, IntPtr trackindex) {
            LoadRetail();

            return _BinkOpenTrack2(bnk, trackindex);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkCloseTrack@1")]
        public static void BinkCloseTrack(IntPtr bnkt) {
            LoadRetail();

            _BinkCloseTrack1(bnkt);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkGetTrackData@2")]
        public static IntPtr BinkGetTrackData(IntPtr bnkt, IntPtr dest) {
            LoadRetail();

            return _BinkGetTrackData2(bnkt, dest);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkGetTrackType@2")]
        public static IntPtr BinkGetTrackType(IntPtr bnk, IntPtr trackindex) {
            LoadRetail();

           return _BinkGetTrackType2(bnk, trackindex);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkGetTrackMaxSize@2")]
        public static IntPtr BinkGetTrackMaxSize(IntPtr bnk, IntPtr trackindex) {
            LoadRetail();

            return _BinkGetTrackMaxSize2(bnk, trackindex);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkGetTrackID@2")]
        public static IntPtr BinkGetTrackID(IntPtr bnk, IntPtr trackindex) {
            LoadRetail();

            return _BinkGetTrackID2(bnk, trackindex);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkGetSummary@2")]
        public static void BinkGetSummary(IntPtr bnk, IntPtr sum) {
            LoadRetail();

            _BinkGetSummary2(bnk, sum);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkGetRealtime@3")]
        public static void BinkGetRealtime(IntPtr bink, IntPtr run, IntPtr frames) {
            LoadRetail();

            _BinkGetRealtime3(bink, run, frames);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetSoundTrack@2")]
        public static void BinkSetSoundTrack(IntPtr total_tracks, IntPtr tracks) {
            LoadRetail();

            _BinkSetSoundTrack2(total_tracks, tracks);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetIO@1")]
        public static void BinkSetIO(IntPtr io) {
            LoadRetail();

            _BinkSetIO1(io);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetFrameRate@2")]
        public static void BinkSetFrameRate(IntPtr forcerate, IntPtr forceratediv) {
            LoadRetail();

            _BinkSetFrameRate2(forcerate, forceratediv);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetSimulate@1")]
        public static void BinkSetSimulate(IntPtr sim) {
            LoadRetail();

            _BinkSetSimulate1(sim);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetIOSize@1")]
        public static void BinkSetIOSize(IntPtr iosize) {
            LoadRetail();

            _BinkSetIOSize1(iosize);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetSoundSystem@2")]
        public static IntPtr BinkSetSoundSystem(IntPtr open, IntPtr param) {
            LoadRetail();

            return _BinkSetSoundSystem2(open, param);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkControlPlatformFeatures@2")]
        public static IntPtr BinkControlPlatformFeatures(IntPtr use, IntPtr dont_use) {
            LoadRetail();

            return _BinkControlPlatformFeatures2(use, dont_use);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpenDirectSound@1")]
        public static IntPtr BinkOpenDirectSound(IntPtr param) { // don't call directly
            LoadRetail();

            return _BinkOpenDirectSound1(param);
        } 

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpenWaveOut@1")]
        public static IntPtr BinkOpenWaveOut(IntPtr param) { // don't call directly
            LoadRetail();

            return _BinkOpenWaveOut1(param);
        } 

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpenXAudio@1")]
        public static IntPtr BinkOpenXAudio(IntPtr param) { // don't call directly
            LoadRetail();

            return _BinkOpenXAudio1(param);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpenMiles@1")]
        public static IntPtr BinkOpenMiles(IntPtr param) { // don't call directly
            LoadRetail();

            return _BinkOpenMiles1(param);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpenSoundManager@1")]
        public static IntPtr BinkOpenSoundManager(IntPtr param) { // don't call directly
            LoadRetail();

            return _BinkOpenSoundManager1(param);
        } 

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpenSDLMixer@1")]
        public static IntPtr BinkOpenSDLMixer(IntPtr param) { // don't call directly
            LoadRetail();

            return _BinkOpenSDLMixer1(param);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpenAX@1")]
        public static IntPtr BinkOpenAX(IntPtr param) { // don't call directly
            LoadRetail();

            return _BinkOpenAX1(param);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpenMusyXSound@1")]
        public static IntPtr BinkOpenMusyXSound(IntPtr param) { // don't call directly
            LoadRetail();

            return _BinkOpenMusyXSound1(param);
        }


        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpenRAD_IOP@1")]
        public static IntPtr BinkOpenRAD_IOP(IntPtr param) { // don't call directly
            LoadRetail();

            return _BinkOpenRAD_IOP1(param);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkFreeGlobals@0")]
        public static void BinkFreeGlobals() {
            LoadRetail();

            _BinkFreeGlobals0();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpenLibAudio@1")]
        public static IntPtr BinkOpenLibAudio(IntPtr param) {
            LoadRetail();

            return _BinkOpenLibAudio1(param);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpenPSPSound@1")]
        public static IntPtr BinkOpenPSPSound(IntPtr param) {
            LoadRetail();

            return _BinkOpenPSPSound1(param);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpenNDSSound@1")]
        public static IntPtr BinkOpenNDSSound(IntPtr param) { // don't call directly
            LoadRetail();

            return _BinkOpenNDSSound1(param);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkDX8SurfaceType@1")]
        public static IntPtr BinkDX8SurfaceType(IntPtr lpD3Ds) {
            LoadRetail();

            return _BinkDX8SurfaceType1(lpD3Ds);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkDX9SurfaceType@1")]
        public static IntPtr BinkDX9SurfaceType(IntPtr lpD3Ds) {
            LoadRetail();

            return _BinkDX9SurfaceType1(lpD3Ds);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkGDSurfaceType@1")]
        public static IntPtr BinkGDSurfaceType(IntPtr /*GDHandle*/ gd) {
            LoadRetail();

            return _BinkGDSurfaceType1(gd);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkIsSoftwareCursor@1")]
        public static IntPtr BinkIsSoftwareCursor(IntPtr /*GDHandle*/ gd) {
            LoadRetail();

            return _BinkIsSoftwareCursor1(gd);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkBufferOpen@4")]
        public static IntPtr BinkBufferOpen(IntPtr /*HWND*/ wnd, IntPtr width, IntPtr height, IntPtr bufferflags) {
            LoadRetail();

            return _BinkBufferOpen4(wnd, width, height, bufferflags);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkBufferSetHWND@2")]
        public static IntPtr BinkBufferSetHWND(IntPtr buf, IntPtr /*HWND*/ newwnd) {
            LoadRetail();

            return _BinkBufferSetHWND2(buf, newwnd);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkDDSurfaceType@1")]
        public static IntPtr BinkDDSurfaceType(IntPtr lpDDS) {
            LoadRetail();

            return _BinkDDSurfaceType1(lpDDS);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkIsSoftwareCursor@2")]
        public static IntPtr BinkIsSoftwareCursor(IntPtr lpDDSP, IntPtr /*HCURSOR*/ cur) {
            LoadRetail();

            return _BinkIsSoftwareCursor2(lpDDSP, cur);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkCheckCursor@5")]
        public static IntPtr BinkCheckCursor(IntPtr /*HWND*/ wnd, IntPtr x, IntPtr y, IntPtr w, IntPtr h) {
            LoadRetail();

            return _BinkCheckCursor5(wnd, x, y, w, h);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkBufferSetDirectDraw@2")]
        public static IntPtr BinkBufferSetDirectDraw(IntPtr lpDirectDraw, IntPtr lpPrimary) {
            LoadRetail();

            return _BinkBufferSetDirectDraw2(lpDirectDraw, lpPrimary);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkBufferClose@11")]
        public static void BinkBufferClose(IntPtr buf) {
            LoadRetail();

            _BinkBufferClose1(buf);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkBufferLock@1")]
        public static IntPtr BinkBufferLock(IntPtr buf) {
            LoadRetail();

            return _BinkBufferLock1(buf);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkBufferUnlock@1")]
        public static IntPtr BinkBufferUnlock(IntPtr buf) {
            LoadRetail();

            return _BinkBufferUnlock1(buf);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkBufferSetResolution@3")]
        public static void BinkBufferSetResolution(IntPtr w, IntPtr h, IntPtr bits) {
            LoadRetail();

            _BinkBufferSetResolution3(w, h, bits);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkBufferCheckWinPos@3")]
        public static void BinkBufferCheckWinPos(IntPtr buf, IntPtr NewWindowX, IntPtr NewWindowY) {
            LoadRetail();

            _BinkBufferCheckWinPos3(buf, NewWindowX, NewWindowY);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkBufferSetOffset@3")]
        public static IntPtr BinkBufferSetOffset(IntPtr buf, IntPtr destx, IntPtr desty) {
            LoadRetail();

            return _BinkBufferSetOffset3(buf, destx, desty);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkBufferBlit@3")]
        public static void BinkBufferBlit(IntPtr buf, IntPtr rects, IntPtr numrects) {
            LoadRetail();

            _BinkBufferBlit3(buf, rects, numrects);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkBufferSetScale@3")]
        public static IntPtr BinkBufferSetScale(IntPtr buf, IntPtr w, IntPtr h) {
            LoadRetail();

            return _BinkBufferSetScale3(buf, w, h);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkBufferGetDescription@1")]
        public static IntPtr BinkBufferGetDescription(IntPtr buf) {
            LoadRetail();

            return _BinkBufferGetDescription1(buf);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkBufferGetError@0")]
        public static IntPtr BinkBufferGetError() {
            LoadRetail();

            return _BinkBufferGetError0();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkRestoreCursor@1")]
        public static void BinkRestoreCursor(IntPtr checkcount) {
            LoadRetail();

            _BinkRestoreCursor1(checkcount);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkBufferClear@2")]
        public static IntPtr BinkBufferClear(IntPtr buf, IntPtr RGB) {
            LoadRetail();

            return _BinkBufferClear2(buf, RGB);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetMemory@2")]
        public static void BinkSetMemory(IntPtr a, IntPtr f) {
            LoadRetail();

            _BinkSetMemory2(a, f);
        }

        //Bink2 - Undocumented 

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkUtilCPUs@0")]
        public static IntPtr BinkUtilCPUs() {
            LoadRetail();

            return _BinkUtilCPUs0();
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkWaitStopAsyncThread@1")]
        public static IntPtr BinkWaitStopAsyncThread(IntPtr A) {
            LoadRetail();

            return _BinkWaitStopAsyncThread1(A);
        }


        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkNextFrame@4")]
        public static IntPtr BinkNextFrame(IntPtr A, IntPtr B, IntPtr C, IntPtr D) {
            LoadRetail();

            return _BinkNextFrame4(A, B, C, D);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpenXAudio2@4")]
        public static IntPtr BinkOpenXAudio2(IntPtr A, IntPtr B, IntPtr C, IntPtr D) {
            LoadRetail();

            return _BinkOpenXAudio24(A, B, C, D);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkShouldSkip@4")]
        public static IntPtr BinkShouldSkip(IntPtr A, IntPtr B, IntPtr C, IntPtr D) {
            LoadRetail();

            return _BinkShouldSkip4(A, B, C, D);
        }


        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkWait@4")]
        public static IntPtr BinkWait(IntPtr A, IntPtr B, IntPtr C, IntPtr D) {
            LoadRetail();

            return _BinkWait4(A, B, C, D);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkClose@4")]
        public static void BinkClose(IntPtr A, IntPtr B, IntPtr C, IntPtr D) {
            LoadRetail();

            _BinkClose4(A, B, C, D);
        }


        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkWaitStopAsyncThreadsMulti@8")]
        public static IntPtr BinkWaitStopAsyncThreadsMulti(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H) {
            LoadRetail();

            return _BinkWaitStopAsyncThreadsMulti8(A, B, C, D, E, F, G, H);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkRequestStopAsyncThreadsMulti@8")]
        public static IntPtr BinkRequestStopAsyncThreadsMulti(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H) {
            LoadRetail();

            return _BinkRequestStopAsyncThreadsMulti8(A, B, C, D, E, F, G, H);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkDoFrameAsyncWait@8")]
        public static IntPtr BinkDoFrameAsyncWait(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H) {
            LoadRetail();

            return _BinkDoFrameAsyncWait8(A, B, C, D, E, F, G, H);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetSoundSystem@8")]
        public static IntPtr BinkSetSoundSystem(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H) {
            LoadRetail();

            return _BinkSetSoundSystem8(A, B, C, D, E, F, G, H);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkStartAsyncThread@8")]
        public static IntPtr BinkStartAsyncThread(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H) {
            LoadRetail();

            return _BinkStartAsyncThread8(A, B, C, D, E, F, G, H);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkOpen@8")]
        public static IntPtr BinkOpen(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H) {
            LoadRetail();

            return _BinkOpen8(A, B, C, D, E, F, G, H);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkDoFrameAsyncMulti@12")]
        public static IntPtr BinkDoFrameAsyncMulti(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H, IntPtr I, IntPtr J, IntPtr K, IntPtr L) {
            LoadRetail();

            return _BinkDoFrameAsyncMulti12(A, B, C, D, E, F, G, H, I, J, K, L);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkCopyToBuffer@28")]
        public static IntPtr BinkCopyToBuffer(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H, IntPtr I, IntPtr J, IntPtr K, IntPtr L, IntPtr M, IntPtr N, IntPtr O, IntPtr P, IntPtr Q, IntPtr R, IntPtr S, IntPtr T, IntPtr U, IntPtr V, IntPtr W, IntPtr X, IntPtr Y, IntPtr Z, IntPtr AA, IntPtr AB) {
            LoadRetail();

            return _BinkCopyToBuffer28(A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, AA, AB);
        }

        [DllExport(CallingConvention = CallingConvention.Winapi, ExportName = "_BinkSetVolume@12")]
        public static void BinkSetVolume(IntPtr A, IntPtr B, IntPtr C, IntPtr D, IntPtr E, IntPtr F, IntPtr G, IntPtr H, IntPtr I, IntPtr J, IntPtr K, IntPtr L) {
            LoadRetail();

            _BinkSetVolume12(A, B, C, D, E, F, G, H, I, J, K, L);
        }

        static NULL_0 _BinkFreeGlobals0;

        static NULL_1 _BinkLoadUnload1;
        static NULL_1 _BinkSetError1;
        static NULL_1 _BinkNextFrame1;
        static NULL_1 _BinkClose1;
        static NULL_1 _BinkService1;
        static NULL_1 _BinkGetPalette1;
        static NULL_1 _BinkCloseTrack1;
        static NULL_1 _BinkSetIO1;
        static NULL_1 _BinkSetSimulate1;
        static NULL_1 _BinkSetIOSize1;
        static NULL_1 _BinkBufferClose1;
        static NULL_1 _BinkRestoreCursor1;

        static NULL_2 _BinkLoadUnloadConverter2;
        static NULL_2 _BinkGetFrameBuffersInfo2;
        static NULL_2 _BinkRegisterFrameBuffers2;
        static NULL_2 _BinkGetSummary2;
        static NULL_2 _BinkSetSoundTrack2;
        static NULL_2 _BinkSetFrameRate2;
        static NULL_2 _BinkSetMemory2;

        static NULL_3 _BinkGoto3;
        static NULL_3 _BinkSetVolume3;
        static NULL_3 _BinkSetPan3;
        static NULL_3 _BinkGetRealtime3;
        static NULL_3 _BinkBufferSetResolution3;
        static NULL_3 _BinkBufferCheckWinPos3;
        static NULL_3 _BinkBufferBlit3;

        static NULL_4 _BinkSetMixBins4;
        static NULL_4 _BinkClose4;
        static NULL_5 _BinkSetMixBinVolumes5;
        static NULL_12 _BinkSetVolume12;

        static RET_0 _BinkLogoAddress0;
        static RET_0 _BinkGetError0;
        static RET_0 _BinkBufferGetError0;
        static RET_0 _BinkUtilCPUs0;

        static RET_1 _BinkDoFrame1;
        static RET_1 _BinkWait1;
        static RET_1 _BinkShouldSkip1;
        static RET_1 _BinkOpenDirectSound1;
        static RET_1 _BinkOpenWaveOut1;
        static RET_1 _BinkOpenXAudio1;
        static RET_1 _BinkOpenMiles1;
        static RET_1 _BinkOpenSoundManager1;
        static RET_1 _BinkOpenSDLMixer1;
        static RET_1 _BinkOpenAX1;
        static RET_1 _BinkOpenMusyXSound1;
        static RET_1 _BinkOpenRAD_IOP1;
        static RET_1 _BinkOpenLibAudio1;
        static RET_1 _BinkOpenPSPSound1;
        static RET_1 _BinkOpenNDSSound1;
        static RET_1 _BinkGDSurfaceType1;
        static RET_1 _BinkIsSoftwareCursor1;
        static RET_1 _BinkDDSurfaceType1;
        static RET_1 _BinkBufferLock1;
        static RET_1 _BinkBufferUnlock1;
        static RET_1 _BinkBufferGetDescription1;
        static RET_1 _BinkDX8SurfaceType1;
        static RET_1 _BinkDX9SurfaceType1;
        static RET_1 _BinkWaitStopAsyncThread1;

        static RET_2 _BinkMacOpen2;
        static RET_2 _BinkNDSOpen2;
        static RET_2 _BinkOpen2;
        static RET_2 _BinkPause2;
        static RET_2 _BinkGetRects2;
        static RET_2 _BinkSetVideoOnOff2;
        static RET_2 _BinkSetSoundOnOff2;
        static RET_2 _BinkControlBackgroundIO2;
        static RET_2 _BinkOpenTrack2;
        static RET_2 _BinkGetTrackData2;
        static RET_2 _BinkGetTrackType2;
        static RET_2 _BinkGetTrackMaxSize2;
        static RET_2 _BinkGetTrackID2;
        static RET_2 _BinkSetSoundSystem2;
        static RET_2 _BinkControlPlatformFeatures2;
        static RET_2 _BinkBufferSetHWND2;
        static RET_2 _BinkIsSoftwareCursor2;
        static RET_2 _BinkBufferSetDirectDraw2;
        static RET_2 _BinkBufferClear2;

        static RET_3 _BinkGetKeyFrame3;
        static RET_3 _BinkBufferSetOffset3;
        static RET_3 _BinkBufferSetScale3;

        static RET_4 _BinkOpenXAudio24;
        static RET_4 _BinkBufferOpen4;
        static RET_4 _BinkNextFrame4;
        static RET_4 _BinkWait4;
        static RET_4 _BinkShouldSkip4;

        static RET_8 _BinkSetSoundSystem8;
        static RET_8 _BinkRequestStopAsyncThreadsMulti8;
        static RET_8 _BinkWaitStopAsyncThreadsMulti8;
        static RET_8 _BinkDoFrameAsyncWait8;
        static RET_8 _BinkStartAsyncThread8;
        static RET_8 _BinkOpen8;

        static RET_5 _BinkCheckCursor5;
        static RET_7 _BinkCopyToBuffer7;
        static RET_11 _BinkCopyToBufferRect11;
        static RET_12 _BinkDoFrameAsyncMulti12;
        static RET_28 _BinkCopyToBuffer28;
    }
}
