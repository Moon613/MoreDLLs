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

public class ExplosiveGraphics : DaddyGraphics
{
    public ExplosiveGraphics(PhysicalObject ow) : base(ow)
    {

    }
    public new ExplosiveLongLegs daddy
    {
        get
        {
            return this.owner as ExplosiveLongLegs;
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
            float baseRed = 0f;
            baseRed = Random.Range(0.09f, 0.2f);
            palette.blackColor = new Color(baseRed, 0.08f, 0.04f);
        }
        base.ApplyPalette(sLeaser, rCam, palette);
    }
}