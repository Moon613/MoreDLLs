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
    internal static void Apply()
    {
        
        //When adding new creatures, make sure to continue the or statement change in these hooks
        On.ArenaCreatureSpawner.IsMajorCreature += (orig, type) => type == CreatureTemplateType.RadioDaddyLongLegs || orig(type);

        new Hook(typeof(DaddyLongLegs).GetMethod("get_SizeClass", Public | NonPublic | Instance), (Func<DaddyLongLegs, bool> orig, DaddyLongLegs self) => self.Template.type == CreatureTemplateType.RadioDaddyLongLegs || orig(self));

        IL.DaddyLongLegs.Act += il =>
        {
            var c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchLdcR4(.6f)))
            {
                c.Emit(Ldarg_0);
                c.EmitDelegate((float num, DaddyLongLegs self) => self.safariControlled && self.Template.type == CreatureTemplateType.RadioDaddyLongLegs ? .25f : num);
            }
            else
                Debug.Log("Couldn't ILHook DaddyLongLegs.Act!");
        };
        
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
            Debug.Log("The relatioship with the source is: " + source.creatureRep?.dynamicRelationship?.currentRelationship.type.ToString());
            if (self.daddy is RadioLongLegs && source.creatureRep != null && source.creatureRep.dynamicRelationship?.currentRelationship.type == CreatureTemplate.Relationship.Type.Eats) {
                (self as RadioAI).radioAI.pack.num++;
                self.reactTarget = Custom.MakeWorldCoordinate(new IntVector2((int)(noise.pos.x / 20f), (int)(noise.pos.y / 20f)), self.daddy.room.abstractRoom.index);
                self.reactNoiseTime = (int)Custom.Dist(self.daddy.mainBodyChunk.pos, source.pos);
                self.newIdlePosCounter = 300;
                self.creature.abstractAI.SetDestination(self.reactTarget);
                self.pathFinder.ForceNextDestination();
                if ((self as RadioAI).radioAI.pack.num < (self as RadioAI).radioAI.pack.members.Count) {
                    Debug.Log("Reacted to noise, num is: " + (self as RadioAI).radioAI.pack.num);
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
        
        On.DaddyAI.IUseARelationshipTracker_UpdateDynamicRelationship += (orig, self, dRelation) => {
            CreatureTemplate.Relationship relationship = orig(self, dRelation);
            var trackedCreature = dRelation?.trackerRep?.representedCreature?.realizedCreature;
            if (self.daddy.Template.type == CreatureTemplateType.RadioDaddyLongLegs) {
                if (trackedCreature.Template.type == CreatureTemplateType.RadioDaddyLongLegs) {
                    return relationship;
                }
                if ((self as RadioAI).radioAI.pack.members.Count <= 2 || trackedCreature.abstractCreature.creatureTemplate.canFly) {
                    relationship.type = CreatureTemplate.Relationship.Type.Afraid;
                    relationship.intensity = 1f;
                    Debug.Log("Set relation to Afraid");
                }
                else if ((self as RadioAI).radioAI.pack.members.Count > 2 && relationship.type != CreatureTemplate.Relationship.Type.Eats && !trackedCreature.abstractCreature.creatureTemplate.canFly) {
                    relationship.type = CreatureTemplate.Relationship.Type.Eats;
                    Debug.Log("Set relation to Eats");
                }
            }
            return relationship;
        };
    }
}