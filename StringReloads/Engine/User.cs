using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace StringReloads.Engine
{
    public static class User
    {
        public enum MBButtons : uint {
            AbortRetryIgnore  = 0x00000002,
            CancelTryContinue = 0x00000006,
            Help              = 0x00004000,
            Ok                = 0x00000000,
            OkCancel          = 0x00000001,
            RetryCancel       = 0x00000005,
            YesNo             = 0x00000004,
            YesNoCancel       = 0x00000003
        }
        public enum MBIcon : uint {
            Exclamation = 0x00000030,
            Warning     = 0x00000030,
            Information = 0x00000040,
            Asterisk    = 0x00000040,
            Question    = 0x00000020,
            Stop        = 0x00000010,
            Error       = 0x00000010,
            Hand        = 0x00000010
        }

        public enum MBDefaultButton {
            Button1 = 0x00000000,
            Button2 = 0x00000100,
            Button3 = 0x00000200,
            Button4 = 0x00000300
        }

        public enum MBModal {
            /// <summary>
            /// The user must respond to the message box before continuing work in the window identified by the hWnd parameter. However, the user can move to the windows of other threads and work in those windows.
            /// Depending on the hierarchy of windows in the application, the user may be able to move to other windows within the thread. All child windows of the parent of the message box are automatically disabled, but pop-up windows are not.
            /// <see cref="App"/> is the default if neither <see cref="System"/> nor <see cref="Task"/> is specified.
            /// </summary>
            App = 0x00000000,
            /// <summary>
            /// Same as <see cref="App"/> except that the message box has the WS_EX_TOPMOST style. Use system-modal message boxes to notify the user of serious, potentially damaging errors that require immediate attention (for example, running out of memory). This flag has no effect on the user's ability to interact with windows other than those associated with hWnd
            /// </summary>
            System = 0x00001000,
            /// <summary>
            /// Same as <see cref="App"/> except that all the top-level windows belonging to the current thread are disabled if the hWnd parameter is NULL.
            /// Use this flag when the calling application or library does not have a window handle available but still needs to prevent input to other windows in the calling thread without suspending other threads.
            /// </summary>
            Task = 0x00002000
        }

        public enum MBResult : uint {
            Ok       = 1,
            Cancel   = 2,
            Abort    = 3,
            Retry    = 4,
            Ignore   = 5,
            Yes      = 6,
            No       = 7,
            TryAgain = 10,
            Continue = 11
        }

        const uint MB_TOPMOST       = 0x00040000;
        const uint MB_SETFOREGROUND = 0x00010000;

        public static MBResult ShowMessageBox(string Text, string Title) {
            return ShowMessageBox(Text, Title, 0, 0);
        }
        public static MBResult ShowMessageBox(string Text, string Title, MBButtons Buttons, MBIcon Icon) {
            return ShowMessageBox(Text, Title, Buttons, Icon, MBDefaultButton.Button1, MBModal.Task, true, false);
        }
        public static MBResult ShowMessageBox(string Text, string Title, MBButtons Buttons, MBIcon Icon, MBDefaultButton DefaultButton, MBModal Modal, bool TopMost, bool SetForeground) {
            return (MBResult)MessageBoxW(0, Text, Title, (uint)Buttons | (uint)Icon | (uint)DefaultButton | (uint)Modal | (TopMost ? MB_TOPMOST : 0 ) | (SetForeground ? MB_SETFOREGROUND : 0));   
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern uint MessageBoxW(int hWnd, string Text, string Title, uint type);
    }
}
