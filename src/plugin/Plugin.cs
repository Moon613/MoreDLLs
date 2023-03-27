using BepInEx;
using UnityEngine;
using Noise;
using MoreSlugcats;
using RWCustom;
using Fisobs.Core;
using System.Security;
using System.Security.Permissions;
using System;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace MoreDlls;

[BepInPlugin("moredll", "MoreDlls", "0.1.0")]
public class MoreDlls : BaseUnityPlugin
{
    bool init;
    public MoreDllsOptions Options;
    public static MoreDllsOptions staticOptions;
    bool configWorking = false;
    public void OnEnable()
    {
        Content.Register(new ExplosiveDllCritob());
        Content.Register(new ZapDllCritob());
        StaticElectricityEnums.RegisterValues();
        //On.Player.AddFood += PlayerAddFoodHook;
        On.WorldLoader.CreatureTypeFromString += WorldLoader_CreatureTypeFromString;
        On.RainWorld.OnModsInit += Init;
        /*On.WorldLoader.AddSpawnersFromString += (orig, self, line) =>
        {
            //base.Logger.LogDebug("" + string.Join(",",line));
            string[] array = Regex.Split(Custom.ValidateSpacedDelimiter(line[1], ","), ", ");
            //base.Logger.LogDebug("" + string.Join(", ",array));
            //if (self.worldName == "LM")
            //{
            //    array = Regex.Split(line[1], ",");
            //}
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = Regex.Split(array[i], "-");
                array2[0] = array2[0].Trim();
                array2[1] = "ZapDaddyLongLegs";
                array[i] = string.Join("-", array2);
            }
            line[1] = string.Join(", ", array);
            //base.Logger.LogDebug("Randomized Spawner " + string.Join(", ", line));
            orig(self, line);
        };*/
        
        /*On.WorldLoader.GeneratePopulation += (orig, self, fresh) =>
        {
            try{
                foreach (AbstractRoom abstractRoom in self.abstractRooms)
                {
                    abstractRoom.creatures.Clear();
                    abstractRoom.entitiesInDens.Clear();
                }
                fresh = true;
                orig(self, fresh);
            }
            catch (Exception ex) {
                //base.Logger.LogError(ex.Message);
            }
        };*/
    }
    private void Init(On.RainWorld.orig_OnModsInit orig, RainWorld self) {
        orig(self);

        if (!init) {
            init = true;

            try {
                this.Options = new MoreDllsOptions(this, Logger);
                staticOptions = this.Options;
                MachineConnector.SetRegisteredOI("moredlls", Options);
                configWorking = true;
            } catch (Exception err) {
                Logger.LogError(err);
                configWorking = false;
            }
        }
    }
    private static CreatureTemplate.Type WorldLoader_CreatureTypeFromString(On.WorldLoader.orig_CreatureTypeFromString orig, string s)
    {
        if (s.ToLower() == "explosivedll" || s.ToLower() == "ell" || s.ToLower() == "explosivedaddylonglegs")
        {
            return CreatureTemplateType.ExplosiveDaddyLongLegs;
        }
        else if (s.ToLower() == "zapdll" || s.ToLower() == "zll" || s.ToLower() == "zappylonglegs" || s.ToLower() == "zaplonglegs")
        {
            return CreatureTemplateType.ZapDaddyLongLegs;
        }
        else
        {
            return orig(s);
        }
    }    
}