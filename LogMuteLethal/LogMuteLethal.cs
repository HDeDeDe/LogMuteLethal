using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine;
using HarmonyLib;
using MonoMod.RuntimeDetour;

/*  These are the bastards that need to be killed:
 *
 *  NetworkVariableBase.SetDirty
 * 
 *  [Error  : Unity Log] RuntimeNavMeshBuilder: Source mesh %s is skipped because it does not allow read access
 *
 *  [Warning: Unity Log] BoxColliders does not support negative scale or size.
 *  The effective box size has been forced positive and is likely to give unexpected collision geometry.
 *  If you absolutely need to use negative scaling you can use the convex MeshCollider. Scene hierarchy path "%s"
 *
 *  [Warning: Unity Log] Audio source failed to initialize audio spatializer. An audio spatializer is specified in the audio project settings, but the associated plugin was not found or initialized properly. Please make sure that the selected spatializer is compatible with the target.
 *
 *  [Warning: Unity Log] Only custom filters can be played. Please add a custom filter or an audioclip to the audiosource (%s).
 *
 *  [Error  : Unity Log] "SetDestination" can only be called on an active agent that has been placed on a NavMesh.
 *
 *  [Info   : Unity Log] Look rotation viewing vector is zero
 */

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
    }
}