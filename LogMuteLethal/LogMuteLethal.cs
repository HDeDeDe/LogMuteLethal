
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using Unity.Netcode;


namespace HDeMods {
    public static class LogMuteLethal {
        private static ILHook networkVarDirtyHook;
        
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
        }
        
        internal static void Startup() {
            IL.HUDManager.SetPlayerLevelSmoothly += HUDManager_SetPlayerLevelSmoothly;
            IL.HoarderBugAI.SetGoTowardsTargetObject += HoarderBugAI_SetGoTowardsTargetObject;
            IL.DepositItemsDesk.Update += DepositItemsDesk_Update;

            networkVarDirtyHook = new ILHook(AccessTools.Method(typeof(NetworkVariableBase),
                nameof(NetworkVariableBase.SetDirty)), NetworkVariableBase_SetDirty);
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
    }
}