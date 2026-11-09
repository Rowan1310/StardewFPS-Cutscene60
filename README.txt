StardewFPS - Cutscene 60 FPS (SOURCE PATCH)

Behavior:
- Gameplay FPS remains configurable with the original SMAPI console command: fps <number>
- While a Stardew Valley event/cutscene is active, the target is forced to 60 FPS.
- When the event/cutscene ends, the last selected gameplay FPS is restored.
- If no FPS command has been used, the game’s initial target FPS is preserved for gameplay.

IMPORTANT:
This package contains SOURCE CODE ONLY. It is not an installable mod yet because
StardewFPS.dll has not been compiled or tested in this environment.

Build:
1. Open StardewFPS.vbproj in an environment with the .NET 6 SDK.
2. Restore NuGet packages and build in Release mode.
3. Copy the generated StardewFPS.dll beside manifest.json in a folder named StardewFPS.
4. Install that folder in the game's Mods directory, replacing the original mod folder
   only after keeping a backup of the original.

Compatibility target: Stardew Valley 1.6 / SMAPI Android (the code uses SMAPI's
GameLoop.UpdateTicked event and Stardew Valley's Game1.CurrentEvent / Game1.eventUp).

This source patch is based on the public Lake1059/StardewFPS repository. It has not
been compiled or runtime-tested here; verify it on a backed-up save before relying on it.
