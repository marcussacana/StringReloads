using System;
using System.Runtime.InteropServices;
using SRLWrapper.Wrapper.Base;
using static SRLWrapper.Wrapper.Base.Wrapper;

namespace SRLWrapper.Wrapper
{
    /// <summary>
    /// This is a wrapper to the MF.dll
    /// </summary>
    public unsafe static class MF
    {
        public static void* RealHandler;
        static MF()
        {
            if (RealHandler != null)
                return;

            RealHandler = LoadLibrary("MF.dll");

            if (RealHandler == null)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED

            dMFGetSystemId = GetDelegate<RET_1>(RealHandler, "MFGetSystemId", true);
            dMFCreateProtectedEnvironmentAccess = GetDelegate<RET_1>(RealHandler, "MFCreateProtectedEnvironmentAccess", true);
            dDllGetActivationFactory = GetDelegate<RET_2>(RealHandler, "DllGetActivationFactory", true);
            dDllCanUnloadNow = GetDelegate<RET_0>(RealHandler, "DllCanUnloadNow", true);
            dMFCreateRemoteDesktopPlugin = GetDelegate<RET_1>(RealHandler, "MFCreateRemoteDesktopPlugin", true);
            dMFCreateVideoRendererActivate = GetDelegate<RET_2>(RealHandler, "MFCreateVideoRendererActivate", true);
            dMFCreateASFProfileFromPresentationDescriptor = GetDelegate<RET_2>(RealHandler, "MFCreateASFProfileFromPresentationDescriptor", true);
            dDllGetClassObject = GetDelegate<RET_3>(RealHandler, "DllGetClassObject", true);
            dMFCreate3GPMediaSink = GetDelegate<RET_4>(RealHandler, "MFCreate3GPMediaSink", true);
            dMFCreateAC3MediaSink = GetDelegate<RET_3>(RealHandler, "MFCreateAC3MediaSink", true);
            dMFCreateADTSMediaSink = GetDelegate<RET_3>(RealHandler, "MFCreateADTSMediaSink", true);
            dMFCreateASFByteStreamPlugin = GetDelegate<RET_2>(RealHandler, "MFCreateASFByteStreamPlugin", true);
            dMFCreateASFContentInfo = GetDelegate<RET_1>(RealHandler, "MFCreateASFContentInfo", true);
            dMFCreateASFIndexerByteStream = GetDelegate<RET_3>(RealHandler, "MFCreateASFIndexerByteStream", true);
            dMFCreateASFIndexer = GetDelegate<RET_1>(RealHandler, "MFCreateASFIndexer", true);
            dMFCreateASFMediaSinkActivate = GetDelegate<RET_3>(RealHandler, "MFCreateASFMediaSinkActivate", true);
            dMFCreateASFMediaSink = GetDelegate<RET_2>(RealHandler, "MFCreateASFMediaSink", true);
            dMFCreateASFMultiplexer = GetDelegate<RET_1>(RealHandler, "MFCreateASFMultiplexer", true);
            dMFCreateASFProfile = GetDelegate<RET_1>(RealHandler, "MFCreateASFProfile", true);
            dMFCreateASFSplitter = GetDelegate<RET_1>(RealHandler, "MFCreateASFSplitter", true);
            dMFCreateASFStreamSelector = GetDelegate<RET_2>(RealHandler, "MFCreateASFStreamSelector", true);
            dMFCreateASFStreamingMediaSinkActivate = GetDelegate<RET_3>(RealHandler, "MFCreateASFStreamingMediaSinkActivate", true);
            dMFCreateASFStreamingMediaSink = GetDelegate<RET_2>(RealHandler, "MFCreateASFStreamingMediaSink", true);
            dMFCreateByteCacheFile = GetDelegate<RET_0>(RealHandler, "MFCreateByteCacheFile", true);
            dMFCreateCacheManager = GetDelegate<RET_0>(RealHandler, "MFCreateCacheManager", true);
            dMFCreateCredentialCache = GetDelegate<RET_1>(RealHandler, "MFCreateCredentialCache", true);
            dMFCreateDrmNetNDSchemePlugin = GetDelegate<RET_2>(RealHandler, "MFCreateDrmNetNDSchemePlugin", true);
            dMFCreateFMPEG4MediaSink = GetDelegate<RET_4>(RealHandler, "MFCreateFMPEG4MediaSink", true);
            dMFCreateFileBlockMap = GetDelegate<RET_0>(RealHandler, "MFCreateFileBlockMap", true);
            dMFCreateHttpSchemePlugin = GetDelegate<RET_2>(RealHandler, "MFCreateHttpSchemePlugin", true);
            dMFCreateLPCMByteStreamPlugin = GetDelegate<RET_2>(RealHandler, "MFCreateLPCMByteStreamPlugin", true);
            dMFCreateMP3ByteStreamPlugin = GetDelegate<RET_2>(RealHandler, "MFCreateMP3ByteStreamPlugin", true);
            dMFCreateMP3MediaSink = GetDelegate<RET_2>(RealHandler, "MFCreateMP3MediaSink", true);
            dMFCreateMPEG4MediaSink = GetDelegate<RET_4>(RealHandler, "MFCreateMPEG4MediaSink", true);
            dMFCreateMuxSink = GetDelegate<RET_7>(RealHandler, "MFCreateMuxSink", true);
            dMFCreateNSCByteStreamPlugin = GetDelegate<RET_2>(RealHandler, "MFCreateNSCByteStreamPlugin", true);
            dMFCreateNetSchemePlugin = GetDelegate<RET_2>(RealHandler, "MFCreateNetSchemePlugin", true);
            dMFCreatePresentationDescriptorFromASFProfile = GetDelegate<RET_2>(RealHandler, "MFCreatePresentationDescriptorFromASFProfile", true);
            dMFCreateProxyLocator = GetDelegate<RET_3>(RealHandler, "MFCreateProxyLocator", true);
            dMFCreateSAMIByteStreamPlugin = GetDelegate<RET_2>(RealHandler, "MFCreateSAMIByteStreamPlugin", true);
            dMFCreateSecureHttpSchemePlugin = GetDelegate<RET_2>(RealHandler, "MFCreateSecureHttpSchemePlugin", true);
            dMFCreateSourceResolver = GetDelegate<RET_1>(RealHandler, "MFCreateSourceResolver", true);
            dMFCreateUrlmonSchemePlugin = GetDelegate<RET_2>(RealHandler, "MFCreateUrlmonSchemePlugin", true);
            dMFGetSupportedMimeTypes = GetDelegate<RET_1>(RealHandler, "MFGetSupportedMimeTypes", true);
            dMFGetSupportedSchemes = GetDelegate<RET_1>(RealHandler, "MFGetSupportedSchemes", true);
            dMFShutdownObject = GetDelegate<RET_1>(RealHandler, "MFShutdownObject", true);
            dMFCreateVideoRenderer = GetDelegate<RET_2>(RealHandler, "MFCreateVideoRenderer", true);
            dMFRR_CreateActivate = GetDelegate<RET_2>(RealHandler, "MFRR_CreateActivate", true);
            dMFLoadSignedLibrary = GetDelegate<RET_2>(RealHandler, "MFLoadSignedLibrary", true);
            dMFGetLocalId = GetDelegate<RET_4>(RealHandler, "MFGetLocalId", true);

            InitializeSRL();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFGetSystemId(IntPtr a1)
        {
            return dMFGetSystemId(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateProtectedEnvironmentAccess(IntPtr a1)
        {
            return dMFCreateProtectedEnvironmentAccess(a1);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DllGetActivationFactory(IntPtr a1, IntPtr a2)
        {
            return dDllGetActivationFactory(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DllCanUnloadNow()
        {
            return dDllCanUnloadNow();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateRemoteDesktopPlugin(IntPtr ppPlugin)
        {
            return dMFCreateRemoteDesktopPlugin(ppPlugin);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateVideoRendererActivate(IntPtr hwndVideo, IntPtr ppActivate)
        {
            return dMFCreateVideoRendererActivate(hwndVideo, ppActivate);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateASFProfileFromPresentationDescriptor(IntPtr pIPD, IntPtr ppIProfile)
        {
            return dMFCreateASFProfileFromPresentationDescriptor(pIPD, ppIProfile);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr DllGetClassObject(IntPtr rclsid, IntPtr riid, IntPtr ppv)
        {
            return dDllGetClassObject(rclsid, riid, ppv);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreate3GPMediaSink(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4)
        {
            return dMFCreate3GPMediaSink(a1, a2, a3, a4);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateAC3MediaSink(IntPtr a1, IntPtr a2, IntPtr a3)
        {
            return dMFCreateAC3MediaSink(a1, a2, a3);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateADTSMediaSink(IntPtr a1, IntPtr a2, IntPtr a3)
        {
            return dMFCreateADTSMediaSink(a1, a2, a3);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateASFByteStreamPlugin(IntPtr riid, IntPtr ppv)
        {
            return dMFCreateASFByteStreamPlugin(riid, ppv);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateASFContentInfo(IntPtr ppIContentInfo)
        {
            return dMFCreateASFContentInfo(ppIContentInfo);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateASFIndexerByteStream(IntPtr pIContentByteStream, IntPtr cbIndexStartOffset, IntPtr pIIndexByteStream)
        {
            return dMFCreateASFIndexerByteStream(pIContentByteStream, cbIndexStartOffset, pIIndexByteStream);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateASFIndexer(IntPtr ppIIndexer)
        {
            return dMFCreateASFIndexer(ppIIndexer);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateASFMediaSinkActivate(IntPtr pwszFileName, IntPtr pContentInfo, IntPtr ppIActivate)
        {
            return dMFCreateASFMediaSinkActivate(pwszFileName, pContentInfo, ppIActivate);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateASFMediaSink(IntPtr pIByteStream, IntPtr ppIMediaSink)
        {
            return dMFCreateASFMediaSink(pIByteStream, ppIMediaSink);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateASFMultiplexer(IntPtr ppIMultiplexer)
        {
            return dMFCreateASFMultiplexer(ppIMultiplexer);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateASFProfile(IntPtr ppIProfile)
        {
            return dMFCreateASFProfile(ppIProfile);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateASFSplitter(IntPtr ppISplitter)
        {
            return dMFCreateASFSplitter(ppISplitter);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateASFStreamSelector(IntPtr pIASFProfile, IntPtr ppSelector)
        {
            return dMFCreateASFStreamSelector(pIASFProfile, ppSelector);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateASFStreamingMediaSinkActivate(IntPtr pByteStreamActivate, IntPtr pContentInfo, IntPtr ppIActivate)
        {
            return dMFCreateASFStreamingMediaSinkActivate(pByteStreamActivate, pContentInfo, ppIActivate);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateASFStreamingMediaSink(IntPtr pIByteStream, IntPtr ppIMediaSink)
        {
            return dMFCreateASFStreamingMediaSink(pIByteStream, ppIMediaSink);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateByteCacheFile()
        {
            return dMFCreateByteCacheFile();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateCacheManager()
        {
            return dMFCreateCacheManager();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateCredentialCache(IntPtr ppCache)
        {
            return dMFCreateCredentialCache(ppCache);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateDrmNetNDSchemePlugin(IntPtr riid, IntPtr ppv)
        {
            return dMFCreateDrmNetNDSchemePlugin(riid, ppv);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateFMPEG4MediaSink(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4)
        {
            return dMFCreateFMPEG4MediaSink(a1, a2, a3, a4);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateFileBlockMap()
        {
            return dMFCreateFileBlockMap();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateHttpSchemePlugin(IntPtr riid, IntPtr ppv)
        {
            return dMFCreateHttpSchemePlugin(riid, ppv);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateLPCMByteStreamPlugin(IntPtr riid, IntPtr ppv)
        {
            return dMFCreateLPCMByteStreamPlugin(riid, ppv);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateMP3ByteStreamPlugin(IntPtr riid, IntPtr ppv)
        {
            return dMFCreateMP3ByteStreamPlugin(riid, ppv);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateMP3MediaSink(IntPtr a1, IntPtr a2)
        {
            return dMFCreateMP3MediaSink(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateMPEG4MediaSink(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4)
        {
            return dMFCreateMPEG4MediaSink(a1, a2, a3, a4);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateMuxSink(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7)
        {
            return dMFCreateMuxSink(a1, a2, a3, a4, a5, a6, a7);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateNSCByteStreamPlugin(IntPtr riid, IntPtr ppv)
        {
            return dMFCreateNSCByteStreamPlugin(riid, ppv);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateNetSchemePlugin(IntPtr riid, IntPtr ppvHandler)
        {
            return dMFCreateNetSchemePlugin(riid, ppvHandler);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreatePresentationDescriptorFromASFProfile(IntPtr pIProfile, IntPtr ppIPD)
        {
            return dMFCreatePresentationDescriptorFromASFProfile(pIProfile, ppIPD);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateProxyLocator(IntPtr pszProtocol, IntPtr pProxyConfig, IntPtr ppProxyLocator)
        {
            return dMFCreateProxyLocator(pszProtocol, pProxyConfig, ppProxyLocator);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateSAMIByteStreamPlugin(IntPtr riid, IntPtr ppv)
        {
            return dMFCreateSAMIByteStreamPlugin(riid, ppv);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateSecureHttpSchemePlugin(IntPtr riid, IntPtr ppv)
        {
            return dMFCreateSecureHttpSchemePlugin(riid, ppv);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateSourceResolver(IntPtr ppISourceResolver)
        {
            return dMFCreateSourceResolver(ppISourceResolver);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateUrlmonSchemePlugin(IntPtr riid, IntPtr ppv)
        {
            return dMFCreateUrlmonSchemePlugin(riid, ppv);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFGetSupportedMimeTypes(IntPtr pPropVarMimeTypeArray)
        {
            return dMFGetSupportedMimeTypes(pPropVarMimeTypeArray);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFGetSupportedSchemes(IntPtr pPropVarSchemeArray)
        {
            return dMFGetSupportedSchemes(pPropVarSchemeArray);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFShutdownObject(IntPtr pUnk)
        {
            return dMFShutdownObject(pUnk);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFCreateVideoRenderer(IntPtr riidRenderer, IntPtr ppVideoRenderer)
        {
            return dMFCreateVideoRenderer(riidRenderer, ppVideoRenderer);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFRR_CreateActivate(IntPtr a1, IntPtr a2)
        {
            return dMFRR_CreateActivate(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr MFLoadSignedLibrary(IntPtr a1, IntPtr a2)
        {
            return dMFLoadSignedLibrary(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.ThisCall)]
        public static IntPtr MFGetLocalId(IntPtr This, IntPtr a2, IntPtr a3, IntPtr a4)
        {
            return dMFGetLocalId(This, a2, a3, a4);
        }


        static RET_1 dMFGetSystemId;
        static RET_1 dMFCreateProtectedEnvironmentAccess;
        static RET_2 dDllGetActivationFactory;
        static RET_0 dDllCanUnloadNow;
        static RET_1 dMFCreateRemoteDesktopPlugin;
        static RET_2 dMFCreateVideoRendererActivate;
        static RET_2 dMFCreateASFProfileFromPresentationDescriptor;
        static RET_3 dDllGetClassObject;
        static RET_4 dMFCreate3GPMediaSink;
        static RET_3 dMFCreateAC3MediaSink;
        static RET_3 dMFCreateADTSMediaSink;
        static RET_2 dMFCreateASFByteStreamPlugin;
        static RET_1 dMFCreateASFContentInfo;
        static RET_3 dMFCreateASFIndexerByteStream;
        static RET_1 dMFCreateASFIndexer;
        static RET_3 dMFCreateASFMediaSinkActivate;
        static RET_2 dMFCreateASFMediaSink;
        static RET_1 dMFCreateASFMultiplexer;
        static RET_1 dMFCreateASFProfile;
        static RET_1 dMFCreateASFSplitter;
        static RET_2 dMFCreateASFStreamSelector;
        static RET_3 dMFCreateASFStreamingMediaSinkActivate;
        static RET_2 dMFCreateASFStreamingMediaSink;
        static RET_0 dMFCreateByteCacheFile;
        static RET_0 dMFCreateCacheManager;
        static RET_1 dMFCreateCredentialCache;
        static RET_2 dMFCreateDrmNetNDSchemePlugin;
        static RET_4 dMFCreateFMPEG4MediaSink;
        static RET_0 dMFCreateFileBlockMap;
        static RET_2 dMFCreateHttpSchemePlugin;
        static RET_2 dMFCreateLPCMByteStreamPlugin;
        static RET_2 dMFCreateMP3ByteStreamPlugin;
        static RET_2 dMFCreateMP3MediaSink;
        static RET_4 dMFCreateMPEG4MediaSink;
        static RET_7 dMFCreateMuxSink;
        static RET_2 dMFCreateNSCByteStreamPlugin;
        static RET_2 dMFCreateNetSchemePlugin;
        static RET_2 dMFCreatePresentationDescriptorFromASFProfile;
        static RET_3 dMFCreateProxyLocator;
        static RET_2 dMFCreateSAMIByteStreamPlugin;
        static RET_2 dMFCreateSecureHttpSchemePlugin;
        static RET_1 dMFCreateSourceResolver;
        static RET_2 dMFCreateUrlmonSchemePlugin;
        static RET_1 dMFGetSupportedMimeTypes;
        static RET_1 dMFGetSupportedSchemes;
        static RET_1 dMFShutdownObject;
        static RET_2 dMFCreateVideoRenderer;
        static RET_2 dMFRR_CreateActivate;
        static RET_2 dMFLoadSignedLibrary;
        static RET_4 dMFGetLocalId;

    }
}
