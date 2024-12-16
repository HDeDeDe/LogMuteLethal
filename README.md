Mutes some logs that are constantly spammed but should be entirely ignored. By default, it targets AI functions that spam the console and harmless errors that unity itself complains about.

If anyone can find where inside UnityPlayer.dll the BoxCollider warning is emitted I will give you a cookie.

If there are any other log messages you think should be muted, feel free to either ping me on the Lethal Company Modding Discord or leave an issue on the [github](https://github.com/HDeDeDe/LogMuteLethal).

## Creds
This mod is based on the one .score created for [Risk of Rain 2](https://thunderstore.io/package/score/LogMute/) and borrows code from it.

Thank you to iDeathHD for pointing me in [the right direction](https://github.com/risk-of-thunder/RoR2BepInExPack/blob/dlc2/RoR2BepInExPack/UnityEngineHooks/FrankenMonoPrintStackOverflowException.cs) on how to do byte patching.

Thank you to 脱税(Windows10ce) for pointing out that "Null Reference Exception" is mono's way of saying "Access Violation".

Thank you to Risk of Rain 2 for getting me into unity modding.