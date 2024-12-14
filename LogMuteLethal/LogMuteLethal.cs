
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using Unity.Netcode;

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
    public static class LogMuteLethal {
        private static ILHook networkVarDirtyHook;
        private static ILHook oneShotHook;
        private static ILHook playerLevelHook;
        
        // Thanks to .score for providing these 2 functions
        private static void RemoveLogFormat(this ILCursor c, string logName) => c.RemoveLog(logName, 2);
        private static void RemoveLog(this ILCursor c, string logName, int count = 1)
        {
            if (!c.TryGotoNext(x => x.MatchCallOrCallvirt<Debug>(logName))) {
                MUTE.Log.Error("Failed to silence " + c.Method.Name);
                return;
            }
            
            for (int i = 0; i < count; i++)
                c.Emit(OpCodes.Pop);
            c.Remove();
            
#if DEBUG
            MUTE.Log.Warning("Silenced " + c.Method.Name);
#endif
        }
        
        internal static void Startup() {
            IL.HoarderBugAI.SetGoTowardsTargetObject += HoarderBugAI_SetGoTowardsTargetObject;
            IL.DepositItemsDesk.Update += DepositItemsDesk_Update;
            playerLevelHook = new ILHook(AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(HUDManager),
                nameof(HUDManager.SetPlayerLevelSmoothly))), HUDManager_SetPlayerLevelSmoothly);
            
            networkVarDirtyHook = new ILHook(AccessTools.Method(typeof(NetworkVariableBase),
                nameof(NetworkVariableBase.SetDirty)), NetworkVariableBase_SetDirty);
            oneShotHook = new ILHook(AccessTools.Method(typeof(AudioSource), nameof(AudioSource.PlayOneShot), 
                    new Type[] {typeof(AudioClip), typeof(float)}), AudioSource_PlayOneShot);
            Reveal.Startup();
        }
        
        private static void HUDManager_SetPlayerLevelSmoothly(ILContext il) {
            ILCursor c = new ILCursor(il);

            c.RemoveLog(nameof(Debug.Log));
        }
        
        private static void HoarderBugAI_SetGoTowardsTargetObject(ILContext il) {
            ILCursor c = new ILCursor(il);
            
            c.RemoveLog(nameof(Debug.Log));
            c.RemoveLog(nameof(Debug.Log));
        }
        
        private static void DepositItemsDesk_Update(ILContext il) {
            ILCursor c = new ILCursor(il);
            
            c.RemoveLog(nameof(Debug.Log));
            c.RemoveLog(nameof(Debug.Log));
        }

        private static void NetworkVariableBase_SetDirty(ILContext il) {
            ILCursor c = new ILCursor(il);
            
            c.RemoveLog(nameof(Debug.LogWarning));
        }
        
        private static void AudioSource_PlayOneShot(ILContext il) {
            ILCursor c = new ILCursor(il);
            
            c.RemoveLog(nameof(Debug.LogWarning));
        }
    }
}