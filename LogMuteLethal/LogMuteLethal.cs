using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using Unity.Netcode;


namespace HDeMods {
    public static class LogMuteLethal {
#if DEBUG
        private static Hook infoHook;
        private static Hook warningHook;
        private static Hook errorHook;
        
        private delegate void logDel(Action<object> orig, object message);
        private static logDel logEv;
        
        private static Hook infoHookContext;
        private static Hook warningHookContext;
        private static Hook errorHookContext;
        
        private delegate void logDelContext(Action<object, UnityEngine.Object> orig, object message, UnityEngine.Object context);
        private static logDelContext logEvContext;
#endif
        private static ILHook networkVarDirtyHook;
        
        // Thanks to .score for providing these 2 functions
        private static void RemoveLogFormat(this ILCursor c, string logName) => c.RemoveLog(logName, 2);
        private static void RemoveLog(this ILCursor c, string logName, int count = 1)
        {
            if (!c.TryGotoNext(x => x.MatchCallOrCallvirt<Debug>(logName))) return;
            
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
#if DEBUG
            logEv += RevealThyself;
            infoHook = new Hook(AccessTools.Method(typeof(Debug), nameof(Debug.Log), 
                new Type[] { typeof(object) }), logEv);
            warningHook = new Hook(AccessTools.Method(typeof(Debug), nameof(Debug.LogWarning), 
                new Type[] { typeof(object) }), logEv);
            errorHook = new Hook(AccessTools.Method(typeof(Debug), nameof(Debug.LogError), 
                new Type[] { typeof(object) }), logEv);
            
            logEvContext += RevealThyselfContext;
            infoHookContext = new Hook(AccessTools.Method(typeof(Debug), nameof(Debug.Log), 
                new Type[] { typeof(object), typeof(UnityEngine.Object) }), logEvContext);
            warningHookContext = new Hook(AccessTools.Method(typeof(Debug), nameof(Debug.LogWarning), 
                new Type[] { typeof(object), typeof(UnityEngine.Object) }), logEvContext);
            errorHookContext = new Hook(AccessTools.Method(typeof(Debug), nameof(Debug.LogError), 
                new Type[] { typeof(object), typeof(UnityEngine.Object) }), logEvContext);
#endif
        }
#if DEBUG
        private static void RevealThyself(Action<object> orig,object message) {
            System.Reflection.MethodBase method = (new System.Diagnostics.StackTrace()).GetFrame(2).GetMethod();
            MUTE.Log.Debug(method.ReflectedType?.Name + "." + method.Name);
            orig(message);
        }
        
        private static void RevealThyselfContext(Action<object, UnityEngine.Object> orig,object message, UnityEngine.Object context) {
            System.Reflection.MethodBase method = (new System.Diagnostics.StackTrace()).GetFrame(2).GetMethod();
            MUTE.Log.Debug(method.ReflectedType?.Name + "." + method.Name);
            orig(message, context);
        }
#endif
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