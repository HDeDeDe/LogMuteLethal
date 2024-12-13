using System;
using UnityEngine;
using MonoMod.RuntimeDetour;
using HarmonyLib;

namespace HDeMods {
    internal static class Reveal {
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
        public static void Startup() {
#if DEBUG
            logEv += Thyself;
            infoHook = new Hook(AccessTools.Method(typeof(Debug), nameof(Debug.Log), 
                new Type[] { typeof(object) }), logEv);
            warningHook = new Hook(AccessTools.Method(typeof(Debug), nameof(Debug.LogWarning), 
                new Type[] { typeof(object) }), logEv);
            errorHook = new Hook(AccessTools.Method(typeof(Debug), nameof(Debug.LogError), 
                new Type[] { typeof(object) }), logEv);
            
            logEvContext += ThyselfContext;
            infoHookContext = new Hook(AccessTools.Method(typeof(Debug), nameof(Debug.Log), 
                new Type[] { typeof(object), typeof(UnityEngine.Object) }), logEvContext);
            warningHookContext = new Hook(AccessTools.Method(typeof(Debug), nameof(Debug.LogWarning), 
                new Type[] { typeof(object), typeof(UnityEngine.Object) }), logEvContext);
            errorHookContext = new Hook(AccessTools.Method(typeof(Debug), nameof(Debug.LogError), 
                new Type[] { typeof(object), typeof(UnityEngine.Object) }), logEvContext);
#endif
        }
#if DEBUG
        private static void Thyself(Action<object> orig,object message) {
            System.Reflection.MethodBase method = (new System.Diagnostics.StackTrace()).GetFrame(2).GetMethod();
            
            if (method.ReflectedType?.Name == "LogMessage") MUTE.Log.Debug(new System.Diagnostics.StackTrace());
            else MUTE.Log.Debug(method.ReflectedType?.Name + "." + method.Name);
            
            orig(message);
        }
        
        private static void ThyselfContext(Action<object, UnityEngine.Object> orig,object message, UnityEngine.Object context) {
            System.Reflection.MethodBase method = (new System.Diagnostics.StackTrace()).GetFrame(2).GetMethod();
            
            if (method.ReflectedType?.Name == "LogMessage") MUTE.Log.Debug(new System.Diagnostics.StackTrace());
            else MUTE.Log.Debug(method.ReflectedType?.Name + "." + method.Name);
            
            orig(message, context);
        }
#endif
    }
}