using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GrammarFixer.Helpers
{
    public static class ClipboardHelper
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        private const byte VK_CONTROL = 0x11;
        private const byte VK_MENU = 0x12; 
        private const byte VK_SHIFT = 0x10;
        private const byte VK_C = 0x43;
        private const byte VK_V = 0x56;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        public static void CopySelectedText()
        {


            keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, 0);  
            keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, 0); 


            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0); 
            
            Thread.Sleep(50); 


            keybd_event(VK_CONTROL, 0, 0, 0);       
            keybd_event(VK_C, 0, 0, 0);             
            Thread.Sleep(10);                       
            keybd_event(VK_C, 0, KEYEVENTF_KEYUP, 0); 
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0); 
        }

        public static void PasteText()
        {

            keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);

            Thread.Sleep(50);


            keybd_event(VK_CONTROL, 0, 0, 0);
            keybd_event(VK_V, 0, 0, 0);
            Thread.Sleep(10);
            keybd_event(VK_V, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
        }
    }
}