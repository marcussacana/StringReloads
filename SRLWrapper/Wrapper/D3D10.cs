using System;
using System.Runtime.InteropServices;
using SRLWrapper.Wrapper.Base;
using static SRLWrapper.Wrapper.Base.Wrapper;

namespace SRLWrapper.Wrapper
{
    public unsafe static class D3D10
    {
        public static void* RealHandler;
        public static void LoadRetail()
        {
            if (RealHandler != null)
                return;

            RealHandler = LoadLibrary("d3d10.dll");

            if (RealHandler == null)
                Environment.Exit(0x505);//ERROR_DELAY_LOAD_FAILED     


            dD3D10CreateDevice = GetDelegate<RET_6>(RealHandler, "D3D10CreateDevice", false);
            dD3D10CreateDeviceAndSwapChain = GetDelegate<RET_8>(RealHandler, "D3D10CreateDeviceAndSwapChain", false);
            dD3D10CreateBlob = GetDelegate<RET_2>(RealHandler, "D3D10CreateBlob", false);
            dD3D10CompileShader = GetDelegate<RET_10>(RealHandler, "D3D10CompileShader", false);
            dD3D10GetPixelShaderProfile = GetDelegate<RET_1>(RealHandler, "D3D10GetPixelShaderProfile", false);
            dD3D10GetVertexShaderProfile = GetDelegate<RET_1>(RealHandler, "D3D10GetVertexShaderProfile", false);
            dD3D10GetGeometryShaderProfile = GetDelegate<RET_1>(RealHandler, "D3D10GetGeometryShaderProfile", false);
            dD3D10GetShaderDebugInfo = GetDelegate<RET_3>(RealHandler, "D3D10GetShaderDebugInfo", false);
            dD3D10PreprocessShader = GetDelegate<RET_7>(RealHandler, "D3D10PreprocessShader", false);
            dD3D10GetInputSignatureBlob = GetDelegate<RET_3>(RealHandler, "D3D10GetInputSignatureBlob", false);
            dD3D10GetOutputSignatureBlob = GetDelegate<RET_3>(RealHandler, "D3D10GetOutputSignatureBlob", false);
            dD3D10GetInputAndOutputSignatureBlob = GetDelegate<RET_3>(RealHandler, "D3D10GetInputAndOutputSignatureBlob", false);
            dD3D10CreateEffectFromMemory = GetDelegate<RET_6>(RealHandler, "D3D10CreateEffectFromMemory", false);
            dD3D10CreateEffectPoolFromMemory = GetDelegate<RET_5>(RealHandler, "D3D10CreateEffectPoolFromMemory", false);
            dD3D10CompileEffectFromMemory = GetDelegate<RET_9>(RealHandler, "D3D10CompileEffectFromMemory", false);
            dD3D10ReflectShader = GetDelegate<RET_3>(RealHandler, "D3D10ReflectShader", false);
            dD3D10DisassembleShader = GetDelegate<RET_5>(RealHandler, "D3D10DisassembleShader", false);
            dD3D10DisassembleEffect = GetDelegate<RET_3>(RealHandler, "D3D10DisassembleEffect", false);
            dD3D10CreateStateBlock = GetDelegate<RET_3>(RealHandler, "D3D10CreateStateBlock", false);
            dD3D10StateBlockMaskUnion = GetDelegate<RET_3>(RealHandler, "D3D10StateBlockMaskUnion", false);
            dD3D10StateBlockMaskIntersect = GetDelegate<RET_3>(RealHandler, "D3D10StateBlockMaskIntersect", false);
            dD3D10StateBlockMaskDifference = GetDelegate<RET_3>(RealHandler, "D3D10StateBlockMaskDifference", false);
            dD3D10StateBlockMaskEnableCapture = GetDelegate<RET_4>(RealHandler, "D3D10StateBlockMaskEnableCapture", false);
            dD3D10StateBlockMaskDisableCapture = GetDelegate<RET_4>(RealHandler, "D3D10StateBlockMaskDisableCapture", false);
            dD3D10StateBlockMaskEnableAll = GetDelegate<RET_1>(RealHandler, "D3D10StateBlockMaskEnableAll", false);
            dD3D10StateBlockMaskDisableAll = GetDelegate<RET_1>(RealHandler, "D3D10StateBlockMaskDisableAll", false);
            dD3D10StateBlockMaskGetSetting = GetDelegate<RET_3>(RealHandler, "D3D10StateBlockMaskGetSetting", false);

            InitializeSRL();
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10CreateDevice(IntPtr pAdapter, IntPtr DriverType, IntPtr Software, IntPtr Flags, IntPtr SDKVersion, IntPtr ppDevice)
        {
            LoadRetail();
            return dD3D10CreateDevice(pAdapter, DriverType, Software, Flags, SDKVersion, ppDevice);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10CreateDeviceAndSwapChain(IntPtr pAdapter, IntPtr DriverType, IntPtr Software, IntPtr Flags, IntPtr SDKVersion, IntPtr pSwapChainDesc, IntPtr ppSwapChain, IntPtr ppDevice)
        {
            LoadRetail();
            return dD3D10CreateDeviceAndSwapChain(pAdapter, DriverType, Software, Flags, SDKVersion, pSwapChainDesc, ppSwapChain, ppDevice);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10CreateBlob(IntPtr NumBytes, IntPtr ppBuffer)
        {
            LoadRetail();
            return dD3D10CreateBlob(NumBytes, ppBuffer);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10CompileShader(IntPtr pSrcData, IntPtr SrcDataLen, IntPtr pFileName, IntPtr pDefines, IntPtr pInclude, IntPtr pFunctionName, IntPtr pProfile, IntPtr Flags, IntPtr ppShader, IntPtr ppErrorMsgs)
        {
            LoadRetail();
            return dD3D10CompileShader(pSrcData, SrcDataLen, pFileName, pDefines, pInclude, pFunctionName, pProfile, Flags, ppShader, ppErrorMsgs);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10GetPixelShaderProfile(IntPtr pDevice)
        {
            LoadRetail();
            return dD3D10GetPixelShaderProfile(pDevice);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10GetVertexShaderProfile(IntPtr pDevice)
        {
            LoadRetail();
            return dD3D10GetVertexShaderProfile(pDevice);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10GetGeometryShaderProfile(IntPtr pDevice)
        {
            LoadRetail();
            return dD3D10GetGeometryShaderProfile(pDevice);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10GetShaderDebugInfo(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr ppDebugInfo)
        {
            LoadRetail();
            return dD3D10GetShaderDebugInfo(pShaderBytecode, BytecodeLength, ppDebugInfo);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10PreprocessShader(IntPtr pSrcData, IntPtr SrcDataSize, IntPtr pFileName, IntPtr pDefines, IntPtr pInclude, IntPtr ppShaderText, IntPtr ppErrorMsgs)
        {
            LoadRetail();
            return dD3D10PreprocessShader(pSrcData, SrcDataSize, pFileName, pDefines, pInclude, ppShaderText, ppErrorMsgs);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10GetInputSignatureBlob(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr ppSignatureBlob)
        {
            LoadRetail();
            return dD3D10GetInputSignatureBlob(pShaderBytecode, BytecodeLength, ppSignatureBlob);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10GetOutputSignatureBlob(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr ppSignatureBlob)
        {
            LoadRetail();
            return dD3D10GetOutputSignatureBlob(pShaderBytecode, BytecodeLength, ppSignatureBlob);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10GetInputAndOutputSignatureBlob(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr ppSignatureBlob)
        {
            LoadRetail();
            return dD3D10GetInputAndOutputSignatureBlob(pShaderBytecode, BytecodeLength, ppSignatureBlob);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10CreateEffectFromMemory(IntPtr pData, IntPtr DataLength, IntPtr FXFlags, IntPtr pDevice, IntPtr pEffectPool, IntPtr ppEffect)
        {
            LoadRetail();
            return dD3D10CreateEffectFromMemory(pData, DataLength, FXFlags, pDevice, pEffectPool, ppEffect);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10CreateEffectPoolFromMemory(IntPtr pData, IntPtr DataLength, IntPtr FXFlags, IntPtr pDevice, IntPtr ppEffectPool)
        {
            LoadRetail();
            return dD3D10CreateEffectPoolFromMemory(pData, DataLength, FXFlags, pDevice, ppEffectPool);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10CompileEffectFromMemory(IntPtr pData, IntPtr DataLength, IntPtr pSrcFileName, IntPtr pDefines, IntPtr pInclude, IntPtr HLSLFlags, IntPtr FXFlags, IntPtr ppCompiledEffect, IntPtr ppErrors)
        {
            LoadRetail();
            return dD3D10CompileEffectFromMemory(pData, DataLength, pSrcFileName, pDefines, pInclude, HLSLFlags, FXFlags, ppCompiledEffect, ppErrors);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10ReflectShader(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr ppReflector)
        {
            LoadRetail();
            return dD3D10ReflectShader(pShaderBytecode, BytecodeLength, ppReflector);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10DisassembleShader(IntPtr pShader, IntPtr BytecodeLength, IntPtr EnableColorCode, IntPtr pComments, IntPtr ppDisassembly)
        {
            LoadRetail();
            return dD3D10DisassembleShader(pShader, BytecodeLength, EnableColorCode, pComments, ppDisassembly);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10DisassembleEffect(IntPtr pEffect, IntPtr EnableColorCode, IntPtr ppDisassembly)
        {
            LoadRetail();
            return dD3D10DisassembleEffect(pEffect, EnableColorCode, ppDisassembly);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10CreateStateBlock(IntPtr pDevice, IntPtr pStateBlockMask, IntPtr ppStateBlock)
        {
            LoadRetail();
            return dD3D10CreateStateBlock(pDevice, pStateBlockMask, ppStateBlock);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10StateBlockMaskUnion(IntPtr pA, IntPtr pB, IntPtr pResult)
        {
            LoadRetail();
            return dD3D10StateBlockMaskUnion(pA, pB, pResult);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10StateBlockMaskIntersect(IntPtr pA, IntPtr pB, IntPtr pResult)
        {
            LoadRetail();
            return dD3D10StateBlockMaskIntersect(pA, pB, pResult);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10StateBlockMaskDifference(IntPtr pA, IntPtr pB, IntPtr pResult)
        {
            LoadRetail();
            return dD3D10StateBlockMaskDifference(pA, pB, pResult);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10StateBlockMaskEnableCapture(IntPtr pMask, IntPtr StateType, IntPtr RangeStart, IntPtr RangeLength)
        {
            LoadRetail();
            return dD3D10StateBlockMaskEnableCapture(pMask, StateType, RangeStart, RangeLength);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10StateBlockMaskDisableCapture(IntPtr pMask, IntPtr StateType, IntPtr RangeStart, IntPtr RangeLength)
        {
            LoadRetail();
            return dD3D10StateBlockMaskDisableCapture(pMask, StateType, RangeStart, RangeLength);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10StateBlockMaskEnableAll(IntPtr pMask)
        {
            LoadRetail();
            return dD3D10StateBlockMaskEnableAll(pMask);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10StateBlockMaskDisableAll(IntPtr pMask)
        {
            LoadRetail();
            return dD3D10StateBlockMaskDisableAll(pMask);
        }

        [DllExport(CallingConvention = CallingConvention.StdCall)]
        public static IntPtr D3D10StateBlockMaskGetSetting(IntPtr pMask, IntPtr StateType, IntPtr Entry)
        {
            LoadRetail();
            return dD3D10StateBlockMaskGetSetting(pMask, StateType, Entry);
        }


        static RET_6 dD3D10CreateDevice;
        static RET_8 dD3D10CreateDeviceAndSwapChain;
        static RET_2 dD3D10CreateBlob;
        static RET_10 dD3D10CompileShader;
        static RET_1 dD3D10GetPixelShaderProfile;
        static RET_1 dD3D10GetVertexShaderProfile;
        static RET_1 dD3D10GetGeometryShaderProfile;
        static RET_3 dD3D10GetShaderDebugInfo;
        static RET_7 dD3D10PreprocessShader;
        static RET_3 dD3D10GetInputSignatureBlob;
        static RET_3 dD3D10GetOutputSignatureBlob;
        static RET_3 dD3D10GetInputAndOutputSignatureBlob;
        static RET_6 dD3D10CreateEffectFromMemory;
        static RET_5 dD3D10CreateEffectPoolFromMemory;
        static RET_9 dD3D10CompileEffectFromMemory;
        static RET_3 dD3D10ReflectShader;
        static RET_5 dD3D10DisassembleShader;
        static RET_3 dD3D10DisassembleEffect;
        static RET_3 dD3D10CreateStateBlock;
        static RET_3 dD3D10StateBlockMaskUnion;
        static RET_3 dD3D10StateBlockMaskIntersect;
        static RET_3 dD3D10StateBlockMaskDifference;
        static RET_4 dD3D10StateBlockMaskEnableCapture;
        static RET_4 dD3D10StateBlockMaskDisableCapture;
        static RET_1 dD3D10StateBlockMaskEnableAll;
        static RET_1 dD3D10StateBlockMaskDisableAll;
        static RET_3 dD3D10StateBlockMaskGetSetting;

    }
}
