using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using UnityEngine;
using static System.Reflection.BindingFlags;
using static Mono.Cecil.Cil.OpCodes;
using System.Runtime.CompilerServices;
using Noise;
using MoreSlugcats;
using RWCustom;
using Random = UnityEngine.Random;
using System.Linq;

namespace MoreDlls;

public class RadioHooks
{
    public static ConditionalWeakTable<DaddyLongLegs, DLLValues> customStuff = new();
    internal static void Apply()
    {
        On.DaddyLongLegs.Eat += (orig, self, eu) => {
            orig(self, eu);
            for (int i = self.eatObjects.Count-1; i >= 0; i --) {
                if (self.eatObjects[i].progression > 1f && self.eatObjects[i].chunk.owner is Creature && self is RadioLongLegs) {
                    RadioLongLegs myself = self as RadioLongLegs;
                    myself.foodPoints += (self.eatObjects[i].chunk.owner as Creature).State.meatLeft;
                    myself.eatenFoodPoints += (self.eatObjects[i].chunk.owner as Creature).State.meatLeft;
                    /*for (int j = 0; j < self.bodyChunks.Length; j++) {
                        self.bodyChunks[j].rad += (self.eatObjects[i].chunk.owner as Creature).State.meatLeft;
                    }
                    for (int j = 0; j < self.bodyChunkConnections.Length; j++) {
                        self.bodyChunkConnections[j].distance += (self.eatObjects[i].chunk.owner as Creature).State.meatLeft;
                    }*/
                }
            }
        };
    }
}