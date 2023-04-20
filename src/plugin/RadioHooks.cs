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
                    int increaseAmount = (self.eatObjects[i].chunk.owner as Creature).State.meatLeft;
                    myself.foodPoints += increaseAmount>0?increaseAmount:1;
                    myself.eatenFoodPoints += increaseAmount>0?increaseAmount:1;
                    /*for (int j = 0; j < self.bodyChunks.Length; j++) {
                        self.bodyChunks[j].rad += (self.eatObjects[i].chunk.owner as Creature).State.meatLeft;
                    }
                    for (int j = 0; j < self.bodyChunkConnections.Length; j++) {
                        self.bodyChunkConnections[j].distance += (self.eatObjects[i].chunk.owner as Creature).State.meatLeft;
                    }*/
                }
            }
        };
        On.DaddyAI.ReactToNoise += (orig, self, source, noise) => {
            if (self.daddy is RadioLongLegs) {   //Issue; it won't execute for the last LongLegs in the pack. Needs refactoring
                (self as RadioAI).radioAI.pack.num++;
                Debug.Log("Reacted to noise, num is: " + (self as RadioAI).radioAI.pack.num);
                self.reactTarget = Custom.MakeWorldCoordinate(new IntVector2((int)(noise.pos.x / 20f), (int)(noise.pos.y / 20f)), self.daddy.room.abstractRoom.index);
                self.reactNoiseTime = 120;
                self.newIdlePosCounter = 0;//300;
                self.creature.abstractAI.SetDestination(self.reactTarget);
                self.pathFinder.ForceNextDestination();
                if ((self as RadioAI).radioAI.pack.num < (self as RadioAI).radioAI.pack.members.Count) {
                    ((self as RadioAI).radioAI.pack.members[(self as RadioAI).radioAI.pack.num].abstRadiodll.realizedCreature.graphicsModule as RadioGraphics).ReactToNoise(source, noise);
                    ((self as RadioAI).radioAI.pack.members[(self as RadioAI).radioAI.pack.num].abstRadiodll.abstractAI.RealAI as RadioAI).ReactToNoise(source, noise);
                }
                else {
                    Debug.Log("Set num back to -1");
                    (self as RadioAI).radioAI.pack.num = -1;
                }
            }
            else {
                orig(self, source, noise);
            }
        };
    }
}