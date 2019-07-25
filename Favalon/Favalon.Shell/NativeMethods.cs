// This is part of Favalon project - https://github.com/kekyo/Favalon
// Copyright (c) 2019 Kouji Matsui
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Favalon
{
    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {

            public short X;
            public short Y;

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public ushort FontFamily;
            public ushort FontWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]   // LF_FACESIZE
            string FaceName;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT_RECORD
        {
            [FieldOffset(0)]
            public ushort EventType;
            [FieldOffset(4)]
            public KEY_EVENT_RECORD KeyEvent;
            [FieldOffset(4)]
            public MOUSE_EVENT_RECORD MouseEvent;
            [FieldOffset(4)]
            public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
            [FieldOffset(4)]
            public MENU_EVENT_RECORD MenuEvent;
            [FieldOffset(4)]
            public FOCUS_EVENT_RECORD FocusEvent;
        };

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct KEY_EVENT_RECORD
        {
            [FieldOffset(0), MarshalAs(UnmanagedType.Bool)]
            public bool bKeyDown;
            [FieldOffset(4), MarshalAs(UnmanagedType.U2)]
            public ushort wRepeatCount;
            [FieldOffset(6), MarshalAs(UnmanagedType.U2)]
            //public VirtualKeys wVirtualKeyCode;
            public ushort wVirtualKeyCode;
            [FieldOffset(8), MarshalAs(UnmanagedType.U2)]
            public ushort wVirtualScanCode;
            [FieldOffset(10)]
            public char UnicodeChar;
            [FieldOffset(12), MarshalAs(UnmanagedType.U4)]
            //public ControlKeyState dwControlKeyState;
            public uint dwControlKeyState;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;
            public uint dwButtonState;
            public uint dwControlKeyState;
            public uint dwEventFlags;
        }

        public struct WINDOW_BUFFER_SIZE_RECORD
        {
            public COORD dwSize;

            public WINDOW_BUFFER_SIZE_RECORD(short x, short y)
            {
                dwSize = new COORD();
                dwSize.X = x;
                dwSize.Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MENU_EVENT_RECORD
        {
            public uint dwCommandId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FOCUS_EVENT_RECORD
        {
            public uint bSetFocus;
        }

        private const ushort FOCUS_EVENT = 0x0010;
        private const ushort KEY_EVENT = 0x0001;
        private const ushort MENU_EVENT = 0x0008;
        private const ushort MOUSE_EVENT = 0x0002;
        private const ushort WINDOW_BUFFER_SIZE_EVENT = 0x0004;

        [DllImport("kernel32.dll", EntryPoint = "PeekConsoleInputW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool PeekConsoleInputW(
            IntPtr hConsoleInput,
            [MarshalAs(UnmanagedType.LPArray), Out] INPUT_RECORD[] lpBuffer,
            uint nLength,
            out uint lpNumberOfEventsRead);

        [DllImport("kernel32.dll", EntryPoint = "PeekConsoleInputW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool PeekConsoleInputW(
            IntPtr hConsoleInput,
            out INPUT_RECORD lpBuffer,
            uint nLength1,
            out uint lpNumberOfEventsRead);

        [DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool ReadConsoleInputW(
            IntPtr hConsoleInput,
            [MarshalAs(UnmanagedType.LPArray), Out] INPUT_RECORD[] lpBuffer,
            uint nLength,
            out uint lpNumberOfEventsRead);

        [DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool ReadConsoleInputW(
            IntPtr hConsoleInput,
            out INPUT_RECORD lpBuffer,
            uint nLength1,
            out uint lpNumberOfEventsRead);

        public const int STD_OUTPUT_HANDLE = -11;
        public const int STD_INPUT_HANDLE = -10;
        public const int STD_ERROR_HANDLE = -12;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int type);

        /*
         * Version of ReadConsoleInput() that works with IME.
         * Works around problems on Windows 8.
         */
        private const int IRSIZE = 10;
        private static readonly INPUT_RECORD[] s_irCache = new INPUT_RECORD[IRSIZE];
        private static uint s_dwIndex = 0;
        private static uint s_dwMax = 0;

        private const bool win8_or_later = true;    // TODO:

#if true
        public static KEY_EVENT_RECORD? ReadConsoleInput(
            IntPtr hInput)
        {
            if (ReadConsoleInputW(hInput, out var record, 1, out var dwEvents))
            {
                if (record.EventType == KEY_EVENT)
                {
                    return record.KeyEvent;
                }
            }

            return null;
        }
#else
        public static bool read_console_input(
            IntPtr hInput,
            INPUT_RECORD[] lpBuffer,
            int nLength,
            out uint lpEvents)
        {
            uint dwEvents;
            uint head;
            uint tail;
            uint i;

            if (nLength == -2)
            {
                lpEvents = 0;
                return (s_dwMax > 0) ? true : false;
            }

            if (!win8_or_later)
            {
                if (nLength == -1)
                    return PeekConsoleInputW(hInput, lpBuffer, 1, out lpEvents);
                else
                {
                    lpEvents = 0;
                    return ReadConsoleInputW(hInput, lpBuffer, 1, out dwEvents);
                }
            }

            if (s_dwMax == 0)
            {
                if (nLength == -1)
                    return PeekConsoleInputW(hInput, lpBuffer, 1, out lpEvents);
                if (!ReadConsoleInputW(hInput, s_irCache, IRSIZE, out dwEvents))
                {
                    lpEvents = 0;
                    return false;
                }

                s_dwIndex = 0;
                s_dwMax = dwEvents;
                if (dwEvents == 0)
                {
                    lpEvents = 0;
                    return true;
                }

                if (s_dwMax > 1)
                {
                    head = 0;
                    tail = s_dwMax - 1;
                    while (head != tail)
                    {
                    if (s_irCache[head].EventType == WINDOW_BUFFER_SIZE_EVENT
                        && s_irCache[head + 1].EventType
                                      == WINDOW_BUFFER_SIZE_EVENT)
                    {
                        /* Remove duplicate event to avoid flicker. */
                        for (i = head; i<tail; ++i)
                        s_irCache[i] = s_irCache[i + 1];
                        --tail;
                        continue;
                    }
                    head++;
                    }
                    s_dwMax = tail + 1;
                }
            }

            lpBuffer = s_irCache[s_dwIndex];
            if (!(nLength == -1 || nLength == -2) && ++s_dwIndex >= s_dwMax)
            s_dwMax = 0;
            lpEvents = 1;
            return true;
        }

        /*
         * Version of PeekConsoleInput() that works with IME.
         */
        public static bool
        peek_console_input(
            IntPtr hInput,
            INPUT_RECORD[] lpBuffer,
            uint nLength,
            out uint lpEvents)
        {
            return read_console_input(hInput, lpBuffer, -1, out lpEvents);
        }
#endif
    }
}
