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
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace MoreDlls;

static class ZapHooks
{
    public static ConditionalWeakTable<DaddyLongLegs, DLLValues> customStuff = new();
    internal static void Apply()
    {
        
        //When adding new creatures, make sure to continue the or statement change in these hooks
        On.ArenaCreatureSpawner.IsMajorCreature += (orig, type) => type == CreatureTemplateType.ZapDaddyLongLegs || orig(type);

        new Hook(typeof(DaddyLongLegs).GetMethod("get_SizeClass", Public | NonPublic | Instance), (Func<DaddyLongLegs, bool> orig, DaddyLongLegs self) => self.Template.type == CreatureTemplateType.ZapDaddyLongLegs || orig(self));

        IL.DaddyLongLegs.Act += il =>
        {
            var c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchLdcR4(.6f)))
            {
                c.Emit(Ldarg_0);
                c.EmitDelegate((float num, DaddyLongLegs self) => self.safariControlled && self.Template.type == CreatureTemplateType.ZapDaddyLongLegs ? .25f : num);
            }
            else
                Debug.Log("Couldn't ILHook DaddyLongLegs.Act!");
        };
                
        On.DaddyTentacle.CollideWithCreature += (orig, self, tChunk, creatureChunk) =>
        {
            orig(self, tChunk, creatureChunk);
            if (self.daddy.Template.type == CreatureTemplateType.ZapDaddyLongLegs)
            {
                Creature? creatureTouched = creatureChunk.owner as Creature;
                customStuff.TryGetValue(self.daddy, out var something);
                if(!(creatureTouched.dead) && (creatureTouched.stun == 0) && (something.grabDecisionCooldown <= 0) && (Random.Range(0f, 80f) >= 79f))
                {
                    if (something.explosionCooldown <= 0f)
                    {
                        //Debug.Log("ATTENTION!");
                        //Debug.Log(something.albino);
                        Vector2 vector = Vector2.Lerp(creatureChunk.pos, creatureChunk.lastPos, 0.35f);
                        creatureTouched.Violence(creatureChunk, vector, creatureTouched.firstChunk, null, Creature.DamageType.Electric, 0.1f, (!(creatureTouched is Player)) ? (320f * Mathf.Lerp(creatureTouched.Template.baseStunResistance, 1f, 0.5f)) : 140f);
		                self.room.AddObject(new CreatureSpasmer(creatureTouched, false, creatureTouched.stun));
                        self.room.PlaySound(StaticElectricityEnums.StaticElectricity, creatureChunk, false, 0.7f, 1.7f);
                        self.room.AddObject(new Explosion.ExplosionLight(creatureChunk.pos, 200f, 1f, 4, new Color(0.7f, 1f, 1f)));
                        for (int i = 0; i < 15; i++)
                        {
                            Vector2 a = Custom.DegToVec(360f * Random.value);
                            self.room.AddObject(new MouseSpark(creatureChunk.pos + a * 9f, creatureChunk.vel + a * 36f * Random.value, 20f, new Color(0.7f, 1f, 1f)));
                        }
                        something.grabDecisionCooldown = Random.Range(45, 76) + Mathf.RoundToInt(something.explosionCooldown);
                    }
                }
            }
        };

        On.DaddyTentacle.Update += (orig, self) =>
        {
            orig(self);
            if (self.daddy.Template.type == CreatureTemplateType.ZapDaddyLongLegs)
            {
                if (Random.Range(0,501) == 500)
                {
                    self.room.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, new Vector2(self.BasePos.x, self.BasePos.y));
                    for (int i = 0; i < 15; i++)
                    {
                        Vector2 a = Custom.DegToVec(360f * Random.value);
                        self.room.AddObject(new Spark((self as Tentacle).Tip.pos + a * 9f, a * Mathf.Lerp(6f, 18f, Random.value), Color.white, null, 20, 30));
                    }
                }
                customStuff.TryGetValue(self.daddy, out var something);
                if (Random.Range(0,(MoreDlls.staticOptions.poleZapFrequency.Value*10)+1) == MoreDlls.staticOptions.poleZapFrequency.Value*10 && MoreDlls.staticOptions.allowPoleZaps.Value && self != null)
                {
                    if (self.room.GetTile((self as Tentacle).Tip.pos).horizontalBeam)
                    {
                        Vector2 tenticlePos = (self as Tentacle).Tip.pos;
                        Vector2 a = Custom.DegToVec(360f * Random.value);
                        //Debug.Log("Values before for loop");
                        //Debug.Log(something.maxZapxReached);
                        //Debug.Log(something.minZapxReached);
                        for (int i = 1; i <= MoreDlls.staticOptions.poleShockRangeX.Value+1; i++)
                        {
                            //Debug.Log("Values while in loop");
                            //Debug.Log(i);
                            //Debug.Log(something.maxZapxReached);
                            //Debug.Log(something.minZapxReached);
                            if (!something.maxZapxReached)
                            {
                                if (!self.room.GetTile(tenticlePos + new Vector2(i,0)).horizontalBeam || i == MoreDlls.staticOptions.poleShockRangeX.Value+1)
                                {
                                    something.maxZapx = tenticlePos.x + i;
                                    something.maxZapxReached = true;
                                }
                                else if (i%100 == 0f)
                                {
                                    self.room.PlaySound(StaticElectricityEnums.StaticElectricity, tenticlePos + new Vector2(i,0), 0.15f, 0.75f);
                                    self.room.AddObject(new Spark((tenticlePos + new Vector2(i,0)) + a * 9f, a * Mathf.Lerp(6f, 18f, Random.value), Color.white, null, 20, 30));
                                    self.room.AddObject(new Spark((tenticlePos + new Vector2(i,0)) + a * 9f, a * Mathf.Lerp(6f, 18f, Random.value), Color.white, null, 20, 30));
                                }
                            }
                            //Debug.Log("Before min for loop");
                            if (!something.minZapxReached)
                            {
                                //Debug.Log("Made it in min loop");
                                if (!self.room.GetTile(tenticlePos - new Vector2(i,0)).horizontalBeam || i == MoreDlls.staticOptions.poleShockRangeX.Value+1)
                                {
                                    something.minZapx = tenticlePos.x + -i;
                                    something.minZapxReached = true;
                                    //Debug.Log(something.minZapx);
                                }
                                else if (i%100 == 0f)
                                {
                                    self.room.PlaySound(StaticElectricityEnums.StaticElectricity, tenticlePos + new Vector2(-i,0), 0.15f, 0.75f);
                                    self.room.AddObject(new Spark((tenticlePos + new Vector2(-i,0)) + a * 9f, a * Mathf.Lerp(6f, 18f, Random.value), Color.white, null, 20, 30));
                                    self.room.AddObject(new Spark((tenticlePos + new Vector2(-i,0)) + a * 9f, a * Mathf.Lerp(6f, 18f, Random.value), Color.white, null, 20, 30));
                                }
                            }
                        }
                        foreach (var creature in self.room.abstractRoom.creatures)
                        {
                            if (creature.realizedCreature != self.daddy)
                            {
                                //Debug.Log("Is creature in shock range X?");
                                //Debug.Log(something.minZapx < creature.realizedCreature.mainBodyChunk.pos.x && creature.realizedCreature.mainBodyChunk.pos.x < something.maxZapx);
                                //Debug.Log("Is creature in shock range Y?");
                                //Debug.Log(tenticlePos.y-10 <= creature.realizedCreature.mainBodyChunk.pos.y && creature.realizedCreature.mainBodyChunk.pos.y <= tenticlePos.y+10);
                                if ((something.minZapx < creature.realizedCreature.mainBodyChunk.pos.x && creature.realizedCreature.mainBodyChunk.pos.x < something.maxZapx) && (tenticlePos.y-10 <= creature.realizedCreature.mainBodyChunk.pos.y && creature.realizedCreature.mainBodyChunk.pos.y <= tenticlePos.y+10))
                                {
                                    creature.realizedCreature.stun = 50;
                                    self.room.AddObject(new Explosion.ExplosionLight(creature.realizedCreature.mainBodyChunk.pos, 100f, 1f, 2, new Color(0.7f, 1f, 0.9f)));
                                    //Debug.Log("The Zappening Worked!");
                                    //Debug.Log(creature.realizedCreature);
                                }
                                //Debug.Log("Creature Chunk Here");
                                //Debug.Log("Creature position");
                                //Debug.Log(creature.realizedCreature.mainBodyChunk.pos.x);
                                //Debug.Log(creature.realizedCreature.mainBodyChunk.pos.y);
                            }
                        }
                        //Debug.Log("Constraints here (The first two numbers, x & y respactivly, should fit between the groups of two numbers here, first 2 for x, second for y)");
                        //Debug.Log(something.maxZapx);
                        //Debug.Log(something.minZapx);
                        //Debug.Log(tenticlePos.y-5);
                        //Debug.Log(tenticlePos.y+5);
                        something.maxZapxReached = false;
                        something.minZapxReached = false;
                        something.maxZapx = 0;
                        something.minZapx = 0;
                    }
                    if (self.room.GetTile((self as Tentacle).Tip.pos).verticalBeam)
                    {
                        Vector2 tenticlePos = (self as Tentacle).Tip.pos;
                        Vector2 a = Custom.DegToVec(360f * Random.value);
                        if (true)
                        {
                            for (int i = 1; i <= MoreDlls.staticOptions.poleShockRangeY.Value+1; i++)
                            {
                                if (!something.maxZapyReached)
                                {
                                    if (!self.room.GetTile(tenticlePos + new Vector2(0,i)).verticalBeam || i == MoreDlls.staticOptions.poleShockRangeY.Value+1)
                                    {
                                        something.maxZapyReached = true;
                                        something.maxZapy = tenticlePos.y + i;
                                    }
                                    else if (i%57 == 0f)
                                    {
                                        self.room.PlaySound(StaticElectricityEnums.StaticElectricity, tenticlePos + new Vector2(0,i), 0.15f, 0.75f);
                                        self.room.AddObject(new Spark((tenticlePos + new Vector2(0,i)) + a * 9f, a * Mathf.Lerp(6f, 18f, Random.value), Color.white, null, 20, 30));
                                        self.room.AddObject(new Spark((tenticlePos + new Vector2(0,i)) + a * 9f, a * Mathf.Lerp(6f, 18f, Random.value), Color.white, null, 20, 30));
                                    }
                                }
                                if (!something.minZapyReached)
                                {
                                    if (!self.room.GetTile(tenticlePos + new Vector2(0,-i)).verticalBeam || i == MoreDlls.staticOptions.poleShockRangeY.Value+1)
                                    {
                                        something.minZapyReached = true;
                                        something.minZapy = tenticlePos.y + -i;
                                    }
                                    else if (i%57 == 0f)
                                    {
                                        self.room.PlaySound(StaticElectricityEnums.StaticElectricity, tenticlePos + new Vector2(0,-i), 0.15f, 0.75f);
                                        self.room.AddObject(new Spark((tenticlePos + new Vector2(0,-i)) + a * 9f, a * Mathf.Lerp(6f, 18f, Random.value), Color.white, null, 20, 30));
                                        self.room.AddObject(new Spark((tenticlePos + new Vector2(0,-i)) + a * 9f, a * Mathf.Lerp(6f, 18f, Random.value), Color.white, null, 20, 30));
                                    }
                                }
                            }
                            foreach (var creature in self.room.abstractRoom.creatures)
                            {
                                if (creature.realizedCreature != self.daddy)
                                {
                                    if ((something.minZapy < creature.realizedCreature.mainBodyChunk.pos.y && creature.realizedCreature.mainBodyChunk.pos.y < something.maxZapy) && (tenticlePos.x-10 <= creature.realizedCreature.mainBodyChunk.pos.x && creature.realizedCreature.mainBodyChunk.pos.x <= tenticlePos.x+10))
                                    {
                                        //self.room.AddObject(new CreatureSpasmer(creature.realizedCreature, false, creature.realizedCreature.stun));
                                        creature.realizedCreature.stun = 50;
                                        self.room.AddObject(new Explosion.ExplosionLight(creature.realizedCreature.mainBodyChunk.pos, 200f, 1f, 4, new Color(0.7f, 1f, 1f)));
                                        //Debug.Log("The Zappening Worked!");
                                        //Debug.Log(creature.realizedCreature);
                                    }
                                    //Debug.Log("Creature Chunk Here");
                                    //Debug.Log(creature.realizedCreature.mainBodyChunk.pos.x);
                                    //Debug.Log(creature.realizedCreature.mainBodyChunk.pos.y);
                                }
                            }
                            something.maxZapyReached = false;
                            something.minZapyReached = false;
                            something.maxZapy = 0;
                            something.minZapy = 0;
                        }
                    }
                }
            }
        };

        On.DaddyLongLegs.ctor += (orig, self, abstractCreature, world) =>
        {
            orig(self, abstractCreature, world);
            if (self.Template.type == CreatureTemplateType.ZapDaddyLongLegs)
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
                    something.initialGColor = Random.Range(0.87f, 0.97f);
                    something.initialBColor = Random.Range(0.83f, 0.94f);
                    self.effectColor = new Color(0.098f, something.initialGColor, something.initialBColor);
                    self.eyeColor = self.effectColor;
                }
            }
        };

        On.DaddyGraphics.ApplyPalette += (orig, self, sLeaser, rCam, palette) =>
        {
            if (self.daddy.Template.type == CreatureTemplateType.ZapDaddyLongLegs)
            {
                customStuff.TryGetValue(self.daddy, out var something);
                if (something.albino)
                {
                    float baseBlue = Random.Range(0.84f, 1f);
                    palette.blackColor = new Color((Mathf.Exp(4.1f*(baseBlue-0.025f)))/(Mathf.Exp(4f))+0.005f, (Mathf.Exp(4f*(baseBlue-0.025f)))/(Mathf.Exp(4f))+0.1f, baseBlue);
                }
                else if (!something.albino)
                {
                    float baseBlue = 0f;
                    baseBlue = Random.Range(0.045f, 0.16f);
                    palette.blackColor = new Color(0.01f, baseBlue - 0.01f, baseBlue);
                }
            }
        orig(self, sLeaser, rCam, palette);
        };

        On.DaddyLongLegs.Update += (orig, self, eu) =>
        {
            orig(self, eu);
            bool flag = false;
            if (self.Template.type == CreatureTemplateType.ZapDaddyLongLegs)
            {
                customStuff.TryGetValue(self, out var something);
                if (something.explosionCooldown > 0f)
                {
                    something.explosionCooldown -= 0.1f;
                }
                if (something.grabDecisionCooldown > 0)
                {
                    something.grabDecisionCooldown -= 1;
                }
                if (something.coreZapCooldown > 0)
                {
                    something.coreZapCooldown -= 1;
                }
                foreach (var creature in self.room.abstractRoom.creatures)
                {
                    if (Custom.Dist(creature.realizedCreature.mainBodyChunk.pos, self.mainBodyChunk.pos) <= 95 && creature.realizedCreature != self && something.coreZapCooldown <= 0)
                    {
                        creature.realizedCreature.stun = 90;
                        Vector2 a = Custom.DegToVec(360f * Random.value);
                        for (int i = 0; i < 15; i++)
                        {
                            self.room.AddObject(new Spark(creature.realizedCreature.mainBodyChunk.pos, a * Mathf.Lerp(6f, 18f, Random.value), Color.white, null, 20, 30));
                        }
                        self.room.AddObject(new Explosion.ExplosionLight(creature.realizedCreature.mainBodyChunk.pos, 120f, 0.9f, 4, new Color(0.7f, 1f, 1f)));
                        self.room.PlaySound(StaticElectricityEnums.StaticElectricity, self.mainBodyChunk.pos, 0.4f, 0.75f);
                        flag = true;
                    }
                }
                if (flag)
                {
                    something.coreZapCooldown = 80;
                    flag = false;
                }
                self.eyeColor = new Color(0.098f, (1/15*something.initialGColor)*Mathf.Sin(0.5f*something.t)+Random.Range(0.25f,0.89f), (1/15*something.initialGColor)*Mathf.Sin(0.5f*something.t)+Random.Range(0.26f,0.9f));
                something.t += 0.1f;
            }
        };
    }
}