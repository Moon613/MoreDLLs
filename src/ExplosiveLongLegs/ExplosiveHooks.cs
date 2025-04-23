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

namespace MoreDlls;

public class ExplosiveHooks
{
    internal static void Apply()
    {
        
        //When adding new creatures, make sure to continue the or statement change in these hooks
        On.ArenaCreatureSpawner.IsMajorCreature += (orig, type) => type == CreatureTemplateType.ExplosiveDaddyLongLegs || type == CreatureTemplateType.ZapDaddyLongLegs || orig(type);

        new Hook(typeof(DaddyLongLegs).GetMethod("get_SizeClass", Public | NonPublic | Instance), (Func<DaddyLongLegs, bool> orig, DaddyLongLegs self) => self.Template.type == CreatureTemplateType.ExplosiveDaddyLongLegs || orig(self));

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
        };
                
        On.DaddyTentacle.CollideWithCreature += (orig, self, tChunk, creatureChunk) =>
        {
            orig(self, tChunk, creatureChunk);
            if (self.daddy.Template.type == CreatureTemplateType.ExplosiveDaddyLongLegs)
            {
                var daddy = (self.daddy as ExplosiveLongLegs);
                Creature? creatureTouched = creatureChunk.owner as Creature;
                if(!(creatureTouched.dead) && (creatureTouched.stun == 0) && (daddy.grabDecisionCooldown <= 0) && (Random.Range(0f, 80f) >= 79f))
                {
                    if (daddy.explosionCooldown <= 0f)
                    {
                        Debug.Log("ATTENTION!");
                        Debug.Log(daddy.albino);
                        Vector2 vector = Vector2.Lerp(creatureChunk.pos, creatureChunk.lastPos, 0.35f);
                        if (!daddy.albino)
                        {
                            self.room.AddObject(new Explosion(self.room, self.owner, vector, 5, 80f, 0.25f, 0.5f, 60f, 0.07f, null, 0.8f, 0.5f, 0.3f));
                            for (int i = 0; i < 14; i++)
                            {
                                self.room.AddObject(new Explosion.ExplosionSmoke(vector, Custom.RNV() * 5f * Random.value, 1f));
                            }
                            self.room.AddObject(new Explosion.ExplosionLight(vector, 160f, 1f, 3, new Color(1f, 0.4f, 0.3f)));
                            self.room.AddObject(new ExplosionSpikes(self.room, vector, 9, 4f, 5f, 5f, 90f, new Color(1f, 0.4f, 0.3f)));
                            self.room.AddObject(new ShockWave(vector, 60f, 0.045f, 4, false));
                            for (int j = 0; j < 20; j++)
                            {
                                Vector2 a = Custom.RNV();
                                self.room.AddObject(new Spark(vector + a * Random.value * 40f, a * Mathf.Lerp(4f, 30f, Random.value), new Color(1f, 0.4f, 0.3f), null, 4, 18));
                            }
                            self.room.PlaySound(SoundID.Fire_Spear_Explode, vector);
                            self.room.InGameNoise(new InGameNoise(vector, 8000f, creatureTouched, 1f));
                            daddy.explosionCooldown = Random.Range(2f, 8f);
                        }
                        else if (daddy.albino)
                        {
                            self.room.AddObject(new Explosion(self.room, creatureTouched, vector, 2, 112.5f, 3.1f, 5f, 140f, 0.125f, null, 0.3f, 80f, 0.5f));
                            self.room.AddObject(new Explosion(self.room, creatureTouched, vector, 2, 500f, 2f, 0f, 200f, 0.125f, null, 0.3f, 100f, 0.5f));
                            self.room.AddObject(new Explosion.ExplosionLight(vector, 70f, 1f, 4, new Color (1f, 1f, 1f)));
                            self.room.AddObject(new Explosion.ExplosionLight(vector, 57.5f, 1f, 2, new Color(1f, 1f, 1f)));
                            self.room.AddObject(new Explosion.ExplosionLight(vector, 500f, 2f, 30, new Color(1f, 1f, 1f)));
                            self.room.AddObject(new ShockWave(vector, 87.5f, 0.3233f, 75, true));
                            self.room.AddObject(new ShockWave(vector, 500f, 0.1233f, 45, false));
                            for (int i = 0; i < 25; i++)
                            {
                                Vector2 a = Custom.RNV();
                                if (self.room.GetTile(vector + a * 20f).Solid)
                                {
                                    if (!self.room.GetTile(vector - a * 20f).Solid)
                                    {
                                        a *= -1f;
                                    }
                                    else
                                    {
                                        a = Custom.RNV();
                                    }
                                }
                                for (int j = 0; j < 3; j++)
                                {
                                    self.room.AddObject(new Spark(vector + a * Mathf.Lerp(30f, 60f, Random.value), a * Mathf.Lerp(7f, 38f, Random.value) + Custom.RNV() * 20f * Random.value, Color.Lerp(new Color(0.5f, 0.5f, 0.5f), new Color(1f, 1f, 1f), Random.value), null, 11, 28));
                                }
                                self.room.AddObject(new Explosion.FlashingSmoke(vector + a * 40f * Random.value, a * Mathf.Lerp(4f, 20f, Mathf.Pow(Random.value, 2f)), 1f + 0.05f * Random.value, new Color(1f, 1f, 1f), new Color(0.5f, 0.5f, 0.5f), Random.Range(3, 11)));
                            }
                            for (int k = 0; k < 6; k++)
                            {
                                self.room.AddObject(new SingularityBomb.BombFragment(vector, Custom.DegToVec(((float)k + Random.value) / 6f * 360f) * Mathf.Lerp(18f, 38f, Random.value)));
                            }
                            self.room.ScreenMovement(new Vector2?(vector), default(Vector2), 0.9f);
                            for (int l = 0; l < creatureTouched.abstractPhysicalObject.stuckObjects.Count; l++)
                            {
                                creatureTouched.abstractPhysicalObject.stuckObjects[l].Deactivate();
                            }
                            self.room.PlaySound(SoundID.Bomb_Explode, vector);

                            self.room.InGameNoise(new InGameNoise(vector, 9000f, creatureTouched, 1f));
                            for (int m = 0; m < self.room.physicalObjects.Length; m++)
                            {
                                for (int n = 0; n < self.room.physicalObjects[m].Count; n++)
                                {
                                    if (self.room.physicalObjects[m][n] is Creature && Custom.Dist(self.room.physicalObjects[m][n].firstChunk.pos, creatureTouched.firstChunk.pos) < 50f && Custom.Dist(self.room.physicalObjects[m][n].firstChunk.pos, creatureTouched.firstChunk.pos) > 10f && self.room.physicalObjects[m][n] != self.daddy)
                                    {
                                        (self.room.physicalObjects[m][n] as Creature).Die();
                                    }
                                    if (self.room.physicalObjects[m][n] is ElectricSpear)
                                    {
                                        if ((self.room.physicalObjects[m][n] as ElectricSpear).abstractSpear.electricCharge == 0)
                                        {
                                            (self.room.physicalObjects[m][n] as ElectricSpear).Recharge();
                                        }
                                        else
                                        {
                                            (self.room.physicalObjects[m][n] as ElectricSpear).ExplosiveShortCircuit();
                                        }
                                    }
                                }
                            }
                            self.room.PlaySound(SoundID.Fire_Spear_Explode, vector);
                            self.room.InGameNoise(new InGameNoise(creatureTouched.firstChunk.pos, 7500f, creatureTouched, 1f));
                            creatureTouched.stun = Random.Range(50, 91);
                            daddy.explosionCooldown = 6f;
                        }
                        daddy.grabDecisionCooldown = Random.Range(45, 76) + Mathf.RoundToInt(daddy.explosionCooldown);
                    }
                }
            }
        };

    }
}