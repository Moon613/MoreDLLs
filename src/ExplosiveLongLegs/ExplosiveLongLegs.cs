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

public class ExplosiveLongLegs : DaddyLongLegs, IProvideWarmth
{
    public float initialRColor;
    public float initialGColor;
    public bool albino;
    public float explosionCooldown;
    public int grabDecisionCooldown;
    public float t;
    public ExplosiveLongLegs(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
    {
        var state = Random.state;
        Random.InitState(this.abstractCreature.ID.RandomSeed);
        Random.state = state;
        float randNumColor = Random.Range(0,101);
        //Use this line for testing the albino color
        //randNumColor = 99f;
        if (randNumColor >= 96)
        {
            this.albino = true;
        }
        Debug.Log("The sizeclass is: " + this.SizeClass);
        if (this.SizeClass)
        {
            this.initialRColor = Random.Range(0.7f, 0.95f);
            this.initialGColor = Random.Range(0f, 0.1f);
            this.effectColor = new Color(this.initialRColor, this.initialGColor, 0f);
            this.eyeColor = this.effectColor;
            /*if (randNum > 5f)
            {
                something.initialRColor = Random.Range(0.75f, 0.825f);
                something.initialGColor = Random.Range(0.8f, 0.95f);
                self.effectColor = new Color(something.initialRColor, something.initialGColor, 0f);
                self.eyeColor = self.effectColor;
                something.eyecolor = 2;
            }*/
        }
    }
    public override void Update(bool eu)
    {
        base.Update(eu);
        if (this.explosionCooldown > 0f)
        {
            this.explosionCooldown -= 0.1f;
        }
        if (this.grabDecisionCooldown > 0)
        {
            this.grabDecisionCooldown -= 1;
        }
        this.eyeColor = new Color(((Mathf.Sin((this.t/2) - ((1.25f*Mathf.PI)/2))+6.75f)/(Random.Range(7,10)+3*this.initialRColor)), ((Mathf.Sin((this.t/2) - (Mathf.PI/2))+1)/(Random.Range(8,11)+3*this.initialGColor)), 0f);
        /*if (something.eyecolor == 2)
        {
            self.eyeColor = new Color(((Mathf.Sin((1.5f*something.x) - ((3*Mathf.PI)/2))+6.75f)/(Random.Range(7,10)+3*something.initialRColor)), ((Mathf.Sin((3*something.x) - (Mathf.PI/2))+5)/(Random.Range(8,11)+3*something.initialGColor)), 0f);
        }*/
        this.t += 0.1f;
    }
	public override void InitiateGraphicsModule()
	{
		if (base.graphicsModule == null)
		{
			base.graphicsModule = new ExplosiveGraphics(this);
		}
	}
    public Vector2 Position() {
        return this.mainBodyChunk.pos;
    }
    public float range => this.mainBodyChunk.rad*2.5f;

    public Room loadedRoom => this.room;

    public float warmth => 0.0005f;
}