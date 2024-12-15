using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

/*
 * e8 8b f0 4b 00 AudioSource   -0x00B9EE00
 * e8 02 ea f6 00 LookZero      -0x000EF489
 * e8 60 e8 4b 00 Filter        -0x00B9F62B
 * e8 7a 99 62 00 NavMesh       -0x00A34511
 * e8 d2 4a 62 00 SetDest       -0x00A393B9
 *
 * 66 48 90 48 90 NOP
 */

namespace HDeMods {

    // Thank you to iDeathHD for pointing me in the right direction for byte patching
    internal static unsafe class LogMuteUnity {
        
        [DllImport("kernel32", ExactSpelling = true)]
        private static extern int FlushInstructionCache(nint handle, void* baseAddr, nuint size);
        
        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress,
            UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
        
        public static void Startup() {
            Process currentProc = Process.GetCurrentProcess();
            IntPtr baseAddress = currentProc.Modules.Cast<ProcessModule>()
                .Single(m => m.ModuleName == "UnityPlayer.dll")
                .BaseAddress;
            
            if (!NopLocation(currentProc, baseAddress, 0xB9EE00, 0xe8_8b_f0_4b_00)) 
                MUTE.Log.Error("Failed to silence Audio Source!");
            if (!NopLocation(currentProc, baseAddress, 0x0EF489, 0xe8_02_ea_f6_00)) 
                MUTE.Log.Error("Failed to silence Look Zero!");
            if (!NopLocation(currentProc, baseAddress, 0xB9F62B, 0xe8_60_e8_4b_00)) 
                MUTE.Log.Error("Failed to silence Filter!");
            if (!NopLocation(currentProc, baseAddress, 0xA34511, 0xe8_7a_99_62_00)) 
                MUTE.Log.Error("Failed to silence NavMesh!");
            if (!NopLocation(currentProc, baseAddress, 0xA393B9, 0xe8_d2_4a_62_00)) 
                MUTE.Log.Error("Failed to silence SetDestination!");
        }
        
        private static bool NopLocation(Process currentProc, IntPtr baseAddress, int offset, ulong instrToReplace) {
            byte* logCall = *(byte**)(baseAddress + offset);
            MUTE.Log.Debug(instrToReplace.ToString("X"));
            MUTE.Log.Debug(offset.ToString("X"));
            MUTE.Log.Debug(((IntPtr)logCall).ToString("X"));
            
            if (*(ulong*)logCall != instrToReplace) return false;
            *(ulong*)logCall = 0x6648904890;
            
            if (FlushInstructionCache(currentProc.Handle, logCall, 5) == 0) return false;
            return true;
        }
    }
}