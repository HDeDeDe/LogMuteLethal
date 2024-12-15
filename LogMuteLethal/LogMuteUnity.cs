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
            
            if (!NopLocation(currentProc, baseAddress, 0xB9EE00, 0xe8_8b_f0_4b_00_48_8b_9c)) 
                MUTE.Log.Error("Failed to silence Audio Source!");
            if (!NopLocation(currentProc, baseAddress, 0x0EF489, 0xe8_02_ea_f6_00_0f_28_bc)) 
                MUTE.Log.Error("Failed to silence Look Zero!");
            if (!NopLocation(currentProc, baseAddress, 0xB9F62B, 0xe8_60_e8_4b_00_40_38_7d)) 
                MUTE.Log.Error("Failed to silence Filter!");
            if (!NopLocation(currentProc, baseAddress, 0xA34511, 0xe8_7a_99_62_00_80_bd_18)) 
                MUTE.Log.Error("Failed to silence NavMesh!");
            if (!NopLocation(currentProc, baseAddress, 0xA393B9, 0xe8_d2_4a_62_00_32_c0_48)) 
                MUTE.Log.Error("Failed to silence SetDestination!");
        }
        
        private static bool NopLocation(Process currentProc, IntPtr baseAddress, nint offset, ulong instrToReplace) {
            ulong mask = 0xff_ff_ff_ff_ff_00_00_00;
            
            byte* logCall = (byte*)(baseAddress + offset);
            
            if (*(ulong*)logCall != instrToReplace) {
                MUTE.Log.Error("No Match. " + (*(long*)logCall).ToString("X") + " " + ((long)instrToReplace).ToString("X"));
                return false;
            }
            mask ^= instrToReplace;
            
            *(ulong*)logCall = 0x66_48_90_48_90_00_00_00 + mask;
            
            if (FlushInstructionCache(currentProc.Handle, logCall, 5) == 0) return false;
            return true;
        }
    }
}