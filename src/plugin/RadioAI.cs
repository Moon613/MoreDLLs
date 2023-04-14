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
using System.Collections.Generic;

namespace MoreDlls;

public class RadioAI : DaddyAI
{
    public RadioAI(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
    {
        this.radioAI = new RadioAIModule(this);
        base.AddModule(radioAI);
        base.AddModule(new NoiseTracker(this, base.tracker));
        base.noiseTracker.forgetTime = 320;
        base.noiseTracker.ignoreSeenNoises = false;
        base.utilityComparer.AddComparedModule(radioAI, null, 1f, 1f);
        //base.AddModule(new SuperHearing(this, this.tracker, 100f));
    }
    public override PathCost TravelPreference(MovementConnection connection, PathCost cost)
    {
        if (this.creature.creatureTemplate.type == CreatureTemplateType.RadioDaddyLongLegs) {
            cost = this.radioAI.TravelPreference(connection, cost);
        }
        return base.TravelPreference(connection, cost);
    }
    public override void Update()
    {
        //this.noiseTracker.Update();
        base.Update();
        if (this.radioAI.communicating > 0) {
            this.daddy.room.AddObject(new Spark(this.daddy.mainBodyChunk.pos, new Vector2(5,5), Color.blue, null, 10, 20));
            Debug.Log("The pack count is: " + this.radioAI.pack.members.Count);
            Debug.Log("Tried to communicate!");
        }
        /*AIModule aimodule = this.utilityComparer.HighestUtilityModule();
        if (aimodule != null && aimodule is NoiseTracker) {
            this.behavior = RadioAI.Behavior.ExamineSound;
        }
        if (this.behavior == RadioAI.Behavior.ExamineSound) {
            if (this.noiseTracker.mysteriousNoises != null && !this.daddy.safariControlled) {
                this.creature.abstractAI.SetDestination(this.daddy.room.GetWorldCoordinate(this.noiseTracker.soundToExamine.pos));
            }
        }*/
    }
    public RadioAIModule radioAI;
}

public class RadioAIModule : AIModule
{
    public RadioAIModule(ArtificialIntelligence AI) : base(AI)
    {
        this.radiodll = (AI.creature.realizedCreature as RadioLongLegs);
        this.pack = new RadioAIModule.RadioPack(this.radiodll.abstractCreature);
    }
    public override void Update()
    {
        base.Update();

        #region CommunicatingAndFlicker
        if (this.communicating > 0)
		{
			this.communicating--;
		}
		bool flag = false;
		if (this.radiodll.safariControlled && this.radiodll.inputWithDiagonals != null && !this.radiodll.inputWithDiagonals.Value.AnyDirectionalInput && this.radiodll.inputWithDiagonals.Value.jmp)
		{
			this.communicating = 14;
			flag = true;
		}
		float num = Mathf.InverseLerp(0f, 14f, (float)this.communicating);
		if (this.commFlicker < num)
		{
			this.commFlicker = Mathf.Min(num, this.commFlicker + 0.25f);
		}
		else
		{
			this.commFlicker = Mathf.Max(num, this.commFlicker - 0.025f);
		}
        #endregion
        
        for (int i = 0; i < this.radiodll.room.abstractRoom.creatures.Count; i++) {
            if (this.radiodll.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplateType.RadioDaddyLongLegs && this.radiodll.room.abstractRoom.creatures[i].realizedCreature != null && this.radiodll.room.abstractRoom.creatures[i].realizedCreature.Consious && this.radiodll.room.abstractRoom.creatures[i] != this.AI.creature) {
                this.ConsiderOtherLongLegs(this.radiodll.room.abstractRoom.creatures[i]);
            }
        }
        //Debug.Log("Is getting updated");
		for (int j = 0; j < this.pack.members.Count; j++)   //Loops through each member of the surrent pack
		{
			if (this.pack.members[j].radiodll.abstractAI.RealAI != null)
			{
				for (int k = 0; k < this.pack.members[j].radiodll.abstractAI.RealAI.tracker.CreaturesCount; k++)    //Loops through each creature it is keeping track of
				{
					if (this.pack.members[j].radiodll.realizedCreature != null && this.pack.members[j].radiodll.realizedCreature.Consious)
					{
                        Debug.Log("Made it to inner checks, calling PackMemberIsSeeingCreature");
						this.PackMemberIsSeeingCreature(this.pack.members[j].radiodll.realizedCreature as RadioLongLegs, this.pack.members[j].radiodll.abstractAI.RealAI.tracker.GetRep(k));
					}
				}
                if (flag && this.pack.members[j].radiodll != this.radiodll.abstractCreature)
                {
                    //(this.pack.members[j].radiodll.realizedCreature as RadioLongLegs).AI.excitement = 1f;
                    //(this.pack.members[j].radiodll.realizedCreature as RadioLongLegs).AI.runSpeed = 1f;
                    //this.pack.members[j].radiodll.abstractAI.SetDestination(this.radiodll.abstractCreature.pos);
                    this.pack.members[j].radiodll.abstractAI.RealAI.SetDestination(this.radiodll.abstractCreature.pos);
                    //this.pack.members[j].radiodll.abstractAI.InternalSetDestination(this.radiodll.abstractCreature.pos);
                    Debug.Log("Set the target position of " + this.pack.members[j].radiodll.abstractAI.GetType().ToString() + ", pack number: " + j + ", to: " + this.radiodll.abstractCreature.pos.ToString());
                }
			}
		}
    }
    public PathCost TravelPreference(MovementConnection connection, PathCost cost) {
        if (this.radiodll == null || this.radiodll.slatedForDeletetion || this.radiodll.room == null || this.radiodll.AI == null || this.pack == null) {return cost;}
        if (this.radiodll.AI.behavior == DaddyAI.Behavior.Hunt) {
            Vector2 a = this.radiodll.room.MiddleOfTile(connection.destinationCoord);
            for (int i = 0; i < this.pack.members.Count; i++) {
                if (this.pack.members[i].radiodll == null || this.pack.members[i].radiodll.slatedForDeletion) {return cost;}
                if (this.pack.members[i].radiodll != this.radiodll.abstractCreature && this.pack.members[i].radiodll.Room == this.radiodll.room.abstractRoom && this.pack.members[i].radiodll.realizedCreature != null) {
                    cost.resistance += Mathf.InverseLerp(300f, 0f, Vector2.Distance(a, this.pack.members[i].radiodll.realizedCreature.mainBodyChunk.pos)) * 200f / (float)(this.pack.members.Count - 1);
                    if (connection.destinationCoord.Tile == this.pack.members[i].radiodll.pos.Tile) {
                        cost.resistance += 300f/ (float)(this.pack.members.Count - 1);
                    }
                }
            }
        }
        return cost;
    }
    public void ConsiderOtherLongLegs(AbstractCreature otherLongLegs) { //Just adds the LongLegs to a pack if they aren't already
        this.AI.tracker.SeeCreature(otherLongLegs);
        if (this.Pack(otherLongLegs.realizedCreature) != this.pack) {
            this.pack.RemoveLongLegs(this.radiodll.abstractCreature);
            this.Pack(otherLongLegs.realizedCreature).AddLongLegs(this.radiodll.abstractCreature);
            this.pack = this.Pack(otherLongLegs.realizedCreature);
        }
    }
    public void PackMemberIsSeeingCreature(RadioLongLegs packMember, Tracker.CreatureRepresentation rep) {
        if (packMember == this.radiodll || rep.representedCreature.realizedCreature == null || !rep.VisualContact) {return;}
        Tracker.CreatureRepresentation creatureRepresentation = this.radiodll.AI.tracker.RepresentationForObject(rep.representedCreature.realizedCreature, false);
        if (creatureRepresentation == null) {
            Debug.Log("Pack member " + packMember.AI.daddy.Template.type.ToString() + " check IsSeeingCreature");
            this.RecieveInfoOnCritter(packMember, rep);
            return;
        }
        if (creatureRepresentation.VisualContact) {return;}
        for (int i = 0; i < rep.representedCreature.realizedCreature.bodyChunks.Length; i++) {
            if (packMember.AI.VisualContact(rep.representedCreature.realizedCreature.bodyChunks[i])) {
                this.RecieveInfoOnCritter(packMember, rep);
                return;
            }
        }
    }
    public void RecieveInfoOnCritter(RadioLongLegs packMember, Tracker.CreatureRepresentation rep) {
        this.radiodll.AI.tracker.SeeCreature(rep.representedCreature);
        if (packMember.AI.noiseTracker == null) {Debug.Log("packMember AI noiseTracker is null!");}
        if (packMember.room == this.radiodll.room && packMember.AI.noiseTracker != null)
        {
            Debug.Log("Pack member " + packMember.AI.daddy.Template.type.ToString() + " is recieving info on target creature. It's sources are: " + packMember.AI.noiseTracker.sources.ToString() + ". And position is: " + packMember.AI.noiseTracker.sources[0].pos.ToString());
            packMember.AI.noiseTracker.sources.AddRange(this.radiodll.AI.noiseTracker.sources);
            packMember.AI.noiseTracker.ignoreSeenNoises = false;
            packMember.AI.behavior = DaddyAI.Behavior.ExamineSound;
            Debug.Log("Packmember sources are: " + packMember.AI.noiseTracker.sources.ToString());
        }
        else if (packMember.room != this.radiodll.room){
            packMember.abstractCreature.abstractAI.RealAI.SetDestination(this.radiodll.abstractCreature.pos);
        }
        this.communicating = Math.Max(this.communicating, Random.Range(3, (int)Mathf.Lerp(3f, 50f, rep.dynamicRelationship.currentRelationship.intensity)));
        (packMember.AI as RadioAI).radioAI.communicating = Math.Max((packMember.AI as RadioAI).radioAI.communicating, Random.Range(3, (int)Mathf.Lerp(3f, 50f, rep.dynamicRelationship.currentRelationship.intensity)));
    }
    public RadioAIModule.RadioPack Pack(Creature radll) {
        return (radll.abstractCreature.abstractAI.RealAI as RadioAI).radioAI.pack;
    }
    public RadioLongLegs radiodll;
    public RadioAIModule.RadioPack pack;
    public int communicating;
    public float commFlicker;
    
    public class RadioPack
    {
        public RadioPack(AbstractCreature firstLongLeg)
        {
            this.members = new List<RadioAIModule.RadioPack.PackMember>{new RadioAIModule.RadioPack.PackMember(firstLongLeg)};
            this.packName = "Pack " + Random.Range(0, 1000).ToString();
        }
        public void Update()
        {
        }
        public void AddLongLegs(AbstractCreature newLongLegs) {
            this.members.Add(new RadioAIModule.RadioPack.PackMember(newLongLegs));
        }
        public void RemoveLongLegs(AbstractCreature removeLongLegs) {
            for (int i = this.members.Count - 1; i >= 0; i--) {
                if (this.members[i].radiodll == removeLongLegs) {
                    this.members.RemoveAt(i);
                }
            }
        }
        public void RemoveLongLegs(int index) {
            this.members.RemoveAt(index);
        }
        public string packName;
        public List<RadioAIModule.RadioPack.PackMember> members;
        public class PackMember
        {
            public PackMember(AbstractCreature radiodll) {
                this.radiodll = radiodll;
            }
            public AbstractCreature radiodll;
        }
    }
}