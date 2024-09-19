## Regular Patch
```c#
    [HarmonyPatch(typeof(MainMenu), "Start")]
    internal class Patch
    {
        static void Postfix()
        {
            BepinPlugin.Log.LogInfo("Example Patch Executed");
        }
    }
```

## Transpiler
```
using static HarmonyLib.AccessTools;
using static VoidManager.Utilities.HarmonyHelpers;
using static System.Reflection.Emit.OpCodes;
```

```
    [HarmonyPatch(typeof(PLSpaceScrap), "OnCollect")]
    class TranspilerPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> target = new List<CodeInstruction>()
            {
                new CodeInstruction(Ldsfld, Field(typeof(PLEncounterManager), "Instance")),
                new CodeInstruction(Ldfld, Field(typeof(PLLevelSync), "PlayerShip"))
            }; // PLShipInfo plshipInfo = PLEncounterManager.Instance.PlayerShip;

            List<CodeInstruction> patch = new List<CodeInstruction>()
            {
                new CodeInstruction(Ldarg_0), // this
                new CodeInstruction(Call, Method(typeof(PlayerShipReplace), "PatchShip"))
            }; // result: PLShipInfo plshipInfo = PlayerShipReplace.PatchShip(this);

            return PatchBySequence(instructions, target, patch, PatchMode.REPLACE, showDebugOutput: false);
        }
    }
```

### FindSequence
```
int LabelIndex = FindSequence(instructions, targetSequence, CheckMode.NONNULL);
CodeInstruction instruction = instructions.ToList()[LabelIndex];
```
Find sequence returns the numerical value of the next code instruction.

## Access Tools
```
private static FieldInfo Flood_StartingPos = AccessTools.Field(typeof(PLAbyssShipInfo), "Flood_StartingPos");
```

