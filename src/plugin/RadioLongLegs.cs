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

public class RadioLongLegs : DaddyLongLegs, IPlayerEdible
{
    public int bites;
    public int foodPoints;
    public int eatenFoodPoints;
    public bool albino;
    public float initialRColor;
    public float initialGColor;
    public float initialBColor;
    public float colorTimer;
    public float[] initialBulbSizes = new float[0];
    public RadioLongLegs(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
    {
        this.bites = 3;
        this.eatenFoodPoints = 0;
        Random.State state = Random.state;
        Random.InitState(this.abstractCreature.ID.RandomSeed);
        Random.state = state;
        float randNumColor = Random.Range(0,101);
        //Use this line for testing the albino color
        //randNumColor = 99f;
        if (randNumColor >= 96)
        {
            this.albino = true;
        }
        else {
            this.albino = false;
        }
        if (this.SizeClass)
        {
            this.initialRColor = Random.Range(0.9f, 0.96f);
            this.initialGColor = Random.Range(0.7f, 0.816f);
            this.initialBColor = Random.Range(0.37f, 0.419f);
            this.effectColor = new Color(this.initialRColor, this.initialGColor, this.initialBColor);
            this.eyeColor = this.effectColor;
        }
        
        float size = 0.3f; //0.4f;
        int amountOfChunks = /*7;*/Random.Range(4, 9);
        //Resizing
        for (int i = 0; i < this.bodyChunks.Length; i++) {
            this.bodyChunks[i].rad *= size;
            this.bodyChunks[i].mass *= 0.15f * size;
            if (i >= amountOfChunks && this.bodyChunks[i] != this.mainBodyChunk) {
                this.bodyChunks[i] = null;
            }
        }
        for (int i = 0; i < this.bodyChunkConnections.Length; i++) {
            this.bodyChunkConnections[i].distance *= size;
            if (Array.IndexOf(this.bodyChunks, this.bodyChunkConnections[i].chunk1) >= amountOfChunks || Array.IndexOf(this.bodyChunks, this.bodyChunkConnections[i].chunk2) >= amountOfChunks) {
                this.bodyChunkConnections[i] = null;
            }
        }
        for (int i = 0; i < this.tentacles.Length; i++) {
            this.tentacles[i].idealLength *= size;
        }
        this.bodyChunks = this.bodyChunks.Where(c => c != null).ToArray();
        this.bodyChunkConnections = this.bodyChunkConnections.Where(c => c != null).ToArray();
        this.foodPoints = this.bodyChunks.Length;
    }
    public override void Update(bool eu)
    {
        base.Update(eu);
        this.eyeColor = new Color(0.1f*Mathf.Cos(this.colorTimer/2f)+0.86f - this.initialRColor/15f, 0.06f*Mathf.Cos(this.colorTimer/2.3f)+0.74f - this.initialGColor/15f, 0.03f*Mathf.Cos(this.colorTimer/3.5f)+0.39f - this.initialBColor/15f);
        this.colorTimer += 0.1f;
        if (this.AI.noiseTracker != null && this.AI.noiseTracker.soundToExamine != null && this.AI.noiseTracker.soundToExamine.creatureRep != null && this.AI.noiseTracker.soundToExamine.creatureRep.representedCreature != null) {
            Debug.Log(this.AI.noiseTracker.soundToExamine.creatureRep.representedCreature.type.ToString() + (this.AI as RadioAI).radioAI.pack.packName);
        }
    }
	public override void InitiateGraphicsModule()
	{
		if (base.graphicsModule == null)
		{
			base.graphicsModule = new RadioGraphics(this);
		}
	}
    public void BitByPlayer(Creature.Grasp grasp, bool eu)
    {
        this.bites--;
        //this.Die();
        this.room.PlaySound((this.bites == 0) ? SoundID.Slugcat_Eat_Centipede : SoundID.Slugcat_Bite_Centipede, base.mainBodyChunk.pos);
        base.firstChunk.MoveFromOutsideMyUpdate(eu, grasp.grabber.mainBodyChunk.pos);
        if (this.bites < 1)
        {
            (grasp.grabber as Player).ObjectEaten(this);
            grasp.Release();
            this.Destroy();
        }
    }
    public void ThrowByPlayer()
    {

    }
    public bool Edible
    {
        get
        {
            return true;
        }
    }
    public bool AutomaticPickUp
    {
        get
        {
            if (this.bodyChunks.Length <= 2) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    public int FoodPoints
    {
        get
        {
            if (this.grabbedBy[0].grabber.GetType().Name == MoreSlugcatsEnums.SlugcatStatsName.Saint.ToString()) {
                return -1;
            } else {
                return this.foodPoints;
            }
        }
    }
    public int BitesLeft
    {
        get
        {
            return this.bites;
        }
    }
}