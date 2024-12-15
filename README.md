Mutes some logs that are constantly spammed but should be entirely ignored. By default, it targets AI functions that spam the console and harmless errors that unity itself complains about.

If anyone can find where inside UnityPlayer.dll the BoxCollider warning is emitted I will give you a cookie.

## Creds
This mod is based on the one .score created for [Risk of Rain 2](https://thunderstore.io/package/score/LogMute/) and borrows code from it.

Thank you to iDeathHD for pointing me in [the right direction](https://github.com/risk-of-thunder/RoR2BepInExPack/blob/dlc2/RoR2BepInExPack/UnityEngineHooks/FrankenMonoPrintStackOverflowException.cs) on how to do byte patching.