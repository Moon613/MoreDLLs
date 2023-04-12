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
        
        //When adding new creatures, make sure to continue the or statement change in these hooks
        /*On.ArenaCreatureSpawner.IsMajorCreature += (orig, type) => type == CreatureTemplateType.ExplosiveDaddyLongLegs || type == CreatureTemplateType.ZapDaddyLongLegs || orig(type);

        new Hook(typeof(DaddyLongLegs).GetMethod("get_SizeClass", Public | NonPublic | Instance), (Func<DaddyLongLegs, bool> orig, DaddyLongLegs self) => self.Template.type == CreatureTemplateType.RadioDaddyLongLegs ||orig(self));

        IL.DaddyLongLegs.Act += il =>
        {
            var c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchLdcR4(.6f)))
            {
                c.Emit(Ldarg_0);
                c.EmitDelegate((float num, DaddyLongLegs self) => self.safariControlled && self.Template.type == CreatureTemplateType.ExplosiveDaddyLongLegs || self.Template.type == CreatureTemplateType.ZapDaddyLongLegs ? .25f : num);
            }
            else
                Debug.Log("Couldn't ILHook DaddyLongLegs.Act!");
        };*/

        /*On.DaddyLongLegs.ctor += (orig, self, abstractCreature, world) =>
        {
            orig(self, abstractCreature, world);
            if (self.Template.type == CreatureTemplateType.RadioDaddyLongLegs)
            {
                var state = Random.state;
                Random.InitState(self.abstractCreature.ID.RandomSeed);
                Random.state = state;
                customStuff.Add(self, new DLLValues());
                customStuff.TryGetValue(self, out var something);
                float randNumColor = Random.Range(0,101);
                //Use this line for testing the albino color
                //randNumColor = 99f;
                if (randNumColor >= 96)
                {
                    something.albino = true;
                }
                if (self.SizeClass)
                {
                    something.initialRColor = Random.Range(0.9f, 0.96f);
                    something.initialGColor = Random.Range(0.7f, 0.816f);
                    something.initialBColor = Random.Range(0.37f, 0.419f);
                    self.effectColor = new Color(something.initialRColor, something.initialGColor, something.initialBColor);
                    self.eyeColor = self.effectColor;
                }
                
                float size = 0.4f;
                int amountOfChunks = Random.Range(2, 5);
                //Resizing
                for (int i = 0; i < self.bodyChunks.Length; i++) {
                    self.bodyChunks[i].rad *= size;
                    self.bodyChunks[i].mass *= 0.15f * size;
                    if (i != 0 && i != 1 i >= amountOfChunks && self.bodyChunks[i] != self.mainBodyChunk) {
                        self.bodyChunks[i] = null;
                    }
                }
                for (int i = 0; i < self.bodyChunkConnections.Length; i++) {
                    self.bodyChunkConnections[i].distance *= size;
                    if (Array.IndexOf(self.bodyChunks, self.bodyChunkConnections[i].chunk1) >= amountOfChunks || Array.IndexOf(self.bodyChunks, self.bodyChunkConnections[i].chunk2) >= amountOfChunks) {
                        self.bodyChunkConnections[i] = null;
                    }
                }
                self.bodyChunks = self.bodyChunks.Where(c => c != null).ToArray();
                self.bodyChunkConnections = self.bodyChunkConnections.Where(c => c != null).ToArray();
                
            }
        };*/

        /*On.DaddyGraphics.ApplyPalette += (orig, self, sLeaser, rCam, palette) =>
        {
            if (self.daddy.Template.type == CreatureTemplateType.RadioDaddyLongLegs)
            {
                customStuff.TryGetValue(self.daddy, out var something);
                if (something.albino)
                {
                    float baseRed = Random.Range(0.84f, 1f);
                    palette.blackColor = new Color(baseRed, (Mathf.Exp(4f*(baseRed-0.025f)))/(Mathf.Exp(4f))+0.1f, (Mathf.Exp(4.1f*(baseRed-0.025f)))/(Mathf.Exp(4f))+0.005f);
                }
                else if (!something.albino)
                {
                    float baseRed = 0f, baseGreen = 0f, baseBlue = 0f;
                    baseRed = Random.Range(0.26f, 0.266f);
                    baseGreen = Random.Range(0.16f, 0.1716f);
                    baseBlue = Random.Range(0.037f, 0.0419f);
                    palette.blackColor = new Color(baseRed, baseGreen, baseBlue);
                }
            }
        orig(self, sLeaser, rCam, palette);
        };*/

        /*On.DaddyLongLegs.Update += (orig, self, eu) =>
        {
            orig(self, eu);
            customStuff.TryGetValue(self, out var something);
            if (self.Template.type == CreatureTemplateType.RadioDaddyLongLegs)
            {
                if (something.explosionCooldown > 0f)
                {
                    something.explosionCooldown -= 0.1f;
                }
                if (something.grabDecisionCooldown > 0)
                {
                    something.grabDecisionCooldown -= 1;
                }
                self.eyeColor = new Color(0.1f*Mathf.Cos(something.t/2f)+0.86f - something.initialRColor/15f, 0.06f*Mathf.Cos(something.t/2)+0.74f - something.initialGColor/15f, 0.03f*Mathf.Cos(something.t/3.5f)+0.39f - something.initialBColor/15f);
                something.t += 0.1f;
            }
        };*/
    }
}