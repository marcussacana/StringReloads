using System;
using System.Runtime.InteropServices;
using static SRL.Wrapper.Tools;

namespace SRL.Wrapper
{

    public static class VORBISFILE
    {
        public static IntPtr RealHandler;

        public static void LoadRetail()
        {
            if (RealHandler != IntPtr.Zero)
                return;

            RealHandler = LoadLibrary("vorbisfile.dll");
            if (RealHandler == IntPtr.Zero)
                Environment.Exit(0x505);

            Real_ov_clear = GetDelegate<RET_1>(RealHandler, "ov_clear", false);
            Real_ov_open_callbacks = GetDelegate<RET_8>(RealHandler, "ov_open_callbacks", false);
            Real_ov_open = GetDelegate<RET_4>(RealHandler, "ov_open", false);
            Real_ov_test_callbacks = GetDelegate<RET_8>(RealHandler, "ov_test_callbacks", false);
            Real_ov_test = GetDelegate<RET_4>(RealHandler, "ov_test", false);
            Real_ov_test_open = GetDelegate<RET_1>(RealHandler, "ov_test_open", false);
            Real_ov_streams = GetDelegate<RET_1>(RealHandler, "ov_streams", false);
            Real_ov_seekable = GetDelegate<RET_1>(RealHandler, "ov_seekable", false);
            Real_ov_bitrate = GetDelegate<RET_2>(RealHandler, "ov_bitrate", false);
            Real_ov_bitrate_instant = GetDelegate<RET_1>(RealHandler, "ov_bitrate_instant", false);
            Real_ov_serialnumber = GetDelegate<RET_2>(RealHandler, "ov_serialnumber", false);
            Real_ov_raw_total = GetDelegate<RET_2>(RealHandler, "ov_raw_total", false);
            Real_ov_pcm_total = GetDelegate<RET_2>(RealHandler, "ov_pcm_total", false);
            Real_ov_time_total = GetDelegate<RET_2>(RealHandler, "ov_time_total", false);
            Real_ov_raw_seek = GetDelegate<RET_2>(RealHandler, "ov_raw_seek", false);
            Real_ov_pcm_seek_page = GetDelegate<RET_2>(RealHandler, "ov_pcm_seek_page", false);
            Real_ov_pcm_seek = GetDelegate<RET_2>(RealHandler, "ov_pcm_seek", false);
            Real_ov_time_seek = GetDelegate<RET_2>(RealHandler, "ov_time_seek", false);
            Real_ov_time_seek_page = GetDelegate<RET_2>(RealHandler, "ov_time_seek_page", false);
            Real_ov_raw_tell = GetDelegate<RET_1>(RealHandler, "ov_raw_tell", false);
            Real_ov_pcm_tell = GetDelegate<RET_1>(RealHandler, "ov_pcm_tell", false);
            Real_ov_time_tell = GetDelegate<RET_1>(RealHandler, "ov_time_tell", false);
            Real_ov_info = GetDelegate<RET_2>(RealHandler, "ov_info", false);
            Real_ov_comment = GetDelegate<RET_2>(RealHandler, "ov_comment", false);
            Real_ov_read = GetDelegate<RET_7>(RealHandler, "ov_read", false);

            InitializeSRL();
        }


        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_clear(IntPtr a1)
        {
            LoadRetail();
            return Real_ov_clear(a1);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_open_callbacks(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7, IntPtr a8)
        {
            LoadRetail();
            return Real_ov_open_callbacks(a1, a2, a3, a4, a5, a6, a7, a8);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_open(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4)
        {
            LoadRetail();
            return Real_ov_open(a1, a2, a3, a4);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_test_callbacks(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7, IntPtr a8)
        {
            LoadRetail();
            return Real_ov_test_callbacks(a1, a2, a3, a4, a5, a6, a7, a8);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_test(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4)
        {
            LoadRetail();
            return Real_ov_test(a1, a2, a3, a4);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_test_open(IntPtr a1)
        {
            LoadRetail();
            return Real_ov_test_open(a1);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_streams(IntPtr a1)
        {
            LoadRetail();
            return Real_ov_streams(a1);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_seekable(IntPtr a1)
        {
            LoadRetail();
            return Real_ov_seekable(a1);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_bitrate(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return Real_ov_bitrate(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_bitrate_instant(IntPtr a1)
        {
            LoadRetail();
            return Real_ov_bitrate_instant(a1);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_serialnumber(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return Real_ov_serialnumber(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_raw_total(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return Real_ov_raw_total(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_pcm_total(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return Real_ov_pcm_total(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_time_total(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return Real_ov_time_total(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_raw_seek(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return Real_ov_raw_seek(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_pcm_seek_page(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return Real_ov_pcm_seek_page(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_pcm_seek(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return Real_ov_pcm_seek(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_time_seek(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return Real_ov_time_seek(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_time_seek_page(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return Real_ov_time_seek_page(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_raw_tell(IntPtr a1)
        {
            LoadRetail();
            return Real_ov_raw_tell(a1);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_pcm_tell(IntPtr a1)
        {
            LoadRetail();
            return Real_ov_pcm_tell(a1);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_time_tell(IntPtr a1)
        {
            LoadRetail();
            return Real_ov_time_tell(a1);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_info(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return Real_ov_info(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_comment(IntPtr a1, IntPtr a2)
        {
            LoadRetail();
            return Real_ov_comment(a1, a2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ov_read(IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7)
        {
            LoadRetail();
            return Real_ov_read(a1, a2, a3, a4, a5, a6, a7);
        }

        static RET_1 Real_ov_clear;
        static RET_8 Real_ov_open_callbacks;
        static RET_4 Real_ov_open;
        static RET_8 Real_ov_test_callbacks;
        static RET_4 Real_ov_test;
        static RET_1 Real_ov_test_open;
        static RET_1 Real_ov_streams;
        static RET_1 Real_ov_seekable;
        static RET_2 Real_ov_bitrate;
        static RET_1 Real_ov_bitrate_instant;
        static RET_2 Real_ov_serialnumber;
        static RET_2 Real_ov_raw_total;
        static RET_2 Real_ov_pcm_total;
        static RET_2 Real_ov_time_total;
        static RET_2 Real_ov_raw_seek;
        static RET_2 Real_ov_pcm_seek_page;
        static RET_2 Real_ov_pcm_seek;
        static RET_2 Real_ov_time_seek;
        static RET_2 Real_ov_time_seek_page;
        static RET_1 Real_ov_raw_tell;
        static RET_1 Real_ov_pcm_tell;
        static RET_1 Real_ov_time_tell;
        static RET_2 Real_ov_info;
        static RET_2 Real_ov_comment;
        static RET_7 Real_ov_read;
    }

}
