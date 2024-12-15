using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using Unity.Netcode;

namespace HDeMods {
    internal static class LogMuteLethal {
        private static ILHook networkVarDirtyHook;
        private static ILHook oneShotHook;
        private static ILHook playerLevelHook;
        
        // Thanks to .score for providing these 2 functions
        private static bool RemoveLogFormat(this ILCursor c, string logName) => c.RemoveLog(logName, 2);
        private static bool RemoveLog(this ILCursor c, string logName, int count = 1)
        {
            if (!c.TryGotoNext(x => x.MatchCallOrCallvirt<Debug>(logName))) {
                MUTE.Log.Error("Failed to silence " + c.Method.Name);
                return false;
            }
            
            for (int i = 0; i < count; i++)
                c.Emit(OpCodes.Pop);
            c.Remove();
            
#if DEBUG
            MUTE.Log.Warning("Silenced " + c.Method.Name);
#endif
            return true;
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

            LogMuteUnity.Startup();
            LogMuteReveal.Startup();
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