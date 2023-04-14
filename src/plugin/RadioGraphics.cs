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

public class RadioGraphics : DaddyGraphics
{
    public RadioGraphics(PhysicalObject ow) : base(ow)
    {

    }
    public new RadioLongLegs daddy
    {
        get
        {
            return this.owner as RadioLongLegs;
        }
    }
    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        if (this.daddy.albino)
        {
            float baseRed = Random.Range(0.84f, 1f);
            palette.blackColor = new Color(baseRed, (Mathf.Exp(4f*(baseRed-0.025f)))/(Mathf.Exp(4f))+0.1f, (Mathf.Exp(4.1f*(baseRed-0.025f)))/(Mathf.Exp(4f))+0.005f);
        }
        else if (!this.daddy.albino)
        {
            float baseRed = 0f, baseGreen = 0f, baseBlue = 0f;
            baseRed = Random.Range(0.3f, 0.31f);
            baseGreen = Random.Range(0.15f, 0.1616f);
            baseBlue = Random.Range(0.037f, 0.0419f);
            palette.blackColor = new Color(baseRed, baseGreen, baseBlue);
        }
        base.ApplyPalette(sLeaser, rCam, palette);
    }
    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        if (this.daddy.bites == 2) {
            for (int i = this.EyeSprite(0,0); i < this.EyeSprite(Mathf.RoundToInt(this.daddy.bodyChunks.Length/3),0); i++) {
                Debug.Log(i);
                sLeaser.sprites[i].isVisible = false;
            }
        }
        else if (this.daddy.bites == 1) {
            for (int i = this.EyeSprite(0,0); i < this.EyeSprite(Mathf.RoundToInt(this.daddy.bodyChunks.Length*2/3),0); i++) {
                Debug.Log(i);
                sLeaser.sprites[i].isVisible = false;
            }
        }
        /*for (int i = this.BodySprite(0); i < this.BodySprite(this.daddy.bodyChunks.Length); i++) {
            sLeaser.sprites[i].scale = this.daddy.initialBulbSizes[i-this.BodySprite(0)] + this.daddy.eatenFoodPoints;
        }*/
    }
    /*public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        base.InitiateSprites(sLeaser, rCam);
        for (int i = this.BodySprite(0); i < this.BodySprite(this.daddy.bodyChunks.Length); i++) {
            Array.Resize(ref this.daddy.initialBulbSizes, this.daddy.initialBulbSizes.Length + 1);
            this.daddy.initialBulbSizes[i] = sLeaser.sprites[i].scale;
        }
    }*/
}