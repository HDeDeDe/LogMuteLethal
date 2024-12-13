using System.Collections;

internal static class Settings {
//-----------------------------------------------------Customize--------------------------------------------------------
    // ReSharper disable once InconsistentNaming
    public const bool giveMePDBs = true;
    public const bool weave = false;

    public const string pluginName = HDeMods.LogMuteLethalPlugin.PluginName;
    public const string pluginAuthor = HDeMods.LogMuteLethalPlugin.PluginAuthor;
    public const string pluginVersion = HDeMods.LogMuteLethalPlugin.PluginVersion;
    public const string changelog = "../CHANGELOG.md";
    public const string readme = "../README.md";

    public const string icon =
        "../Resources/icon.png";

    public const string riskOfRain2Install =
        @"C:\Program Files (x86)\Steam\steamapps\common\Lethal Company\Lethal Company_Data\Managed";

    public static readonly ArrayList extraFiles = new() {
    };

    public const string manifestWebsiteUrl = "https://github.com/HDeDeDe/LogMuteLethal";

    public const string manifestDescription =
        "Removes some of the annoying debug logs";

    public const string manifestDependencies = "[\n" +
                                               "\t\t\"BepInEx-BepInExPack-5.4.2100\",\n" +
                                               "\t\t\"Evaisa-HookGenPatcher-0.0.5\"\n" +
                                               "\t]";
}