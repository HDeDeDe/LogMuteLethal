using System.Diagnostics.CodeAnalysis;
using BepInEx;
using BepInEx.Logging;

namespace HDeMods {
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class LogMuteLethalPlugin : BaseUnityPlugin {
        public const string PluginGUID = "com." + PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "HDeDeDe";
        public const string PluginName = "LogMuteLethal";
        public const string PluginVersion = "1.0.0";

        public static LogMuteLethalPlugin instance;

        private void Awake() {
            if (instance != null) {
                MUTE.Log.Error("There can be only 1 instance of " + PluginName + "!");
                Destroy(this);
                return;
            }
            
            MUTE.Log.Init(Logger);
            instance = this;
            LogMuteLethal.Startup();
        }
    }

    namespace MUTE {
    internal static class Log {
        private static ManualLogSource logMe;

        internal static void Init(ManualLogSource logSource) {
            logMe = logSource;
        }

        internal static void Debug(object data) => logMe!.LogDebug(data);
        internal static void Error(object data) => logMe!.LogError(data);
        internal static void Fatal(object data) => logMe!.LogFatal(data);
        internal static void Info(object data) => logMe!.LogInfo(data);
        internal static void Message(object data) => logMe!.LogMessage(data);
        internal static void Warning(object data) => logMe!.LogWarning(data);
    }
    }
}