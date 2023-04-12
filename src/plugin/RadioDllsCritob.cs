using Fisobs.Creatures;
using Fisobs.Core;
using System.Collections.Generic;
using Fisobs.Sandbox;
using static PathCost.Legality;
using UnityEngine;
using DevInterface;
using RWCustom;
using Fisobs.Properties;

namespace MoreDlls;

sealed class RadioDllCritob : Critob
{
    public RadioDllCritob() : base(CreatureTemplateType.RadioDaddyLongLegs)
    {
        Icon = new SimpleIcon("Kill_Daddy", new Color(1f, 168f/255f, 12f/255f));
        RegisterUnlock(KillScore.Configurable(25), SandboxUnlockID.RadioDaddyLongLegs);
        SandboxPerformanceCost = new(3f, 1.5f);
        LoadedPerformanceCost = 200f;
        ShelterDanger = ShelterDanger.Hostile;
        //RadioHooks.Apply();
    }

    public override void ConnectionIsAllowed(AImap map, MovementConnection connection, ref bool? allow)
    {
        if (connection.type == MovementConnection.MovementType.ShortCut)
        {
            if (connection.startCoord.TileDefined && map.room.shortcutData(connection.StartTile).shortCutType == ShortcutData.Type.Normal)
                allow = true;
            if (connection.destinationCoord.TileDefined && map.room.shortcutData(connection.DestTile).shortCutType == ShortcutData.Type.Normal)
                allow = true;
        }
        else if (connection.type == MovementConnection.MovementType.BigCreatureShortCutSqueeze)
        {
            if (map.room.GetTile(connection.startCoord).Terrain == Room.Tile.TerrainType.ShortcutEntrance && map.room.shortcutData(connection.StartTile).shortCutType == ShortcutData.Type.Normal)
                allow = true;
            if (map.room.GetTile(connection.destinationCoord).Terrain == Room.Tile.TerrainType.ShortcutEntrance && map.room.shortcutData(connection.DestTile).shortCutType == ShortcutData.Type.Normal)
                allow = true;
        }
    }

    public override void TileIsAllowed(AImap map, IntVector2 tilePos, ref bool? allow) => allow = map.getAItile(tilePos).terrainProximity > 1;

    public override int ExpeditionScore() => 20;

    public override Color DevtoolsMapColor(AbstractCreature acrit) => new Color(1f, 168f/255f, 12f/255f);

    public override string DevtoolsMapName(AbstractCreature acrit) => "rll";

    public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction() => new[] { RoomAttractivenessPanel.Category.LikesInside };

    public override IEnumerable<string> WorldFileAliases() => new[] { "radiodll" };

    public override CreatureTemplate CreateTemplate()
    {
        var t = new CreatureFormula(CreatureTemplate.Type.DaddyLongLegs, Type, "RadioDll")
        {
            TileResistances = new()
            {
                Air = new(1f, Allowed)
            },
            ConnectionResistances = new() 
            {
                Standard = new(1f, Allowed),
                ShortCut = new(1f, Allowed),
                BigCreatureShortCutSqueeze = new(0f, Allowed),
                OffScreenMovement = new(1f, Allowed),
                BetweenRooms = new(0f, Allowed)
            },
            DefaultRelationship = new(CreatureTemplate.Relationship.Type.Eats, 1f),
            DamageResistances = new() { Base = 50f},
            StunResistances = new() { Base = 150f},
            HasAI = true,
            Pathing = PreBakedPathing.Ancestral(CreatureTemplate.Type.DaddyLongLegs)
        }.IntoTemplate();
        t.shortcutColor = new Color(1f, 168f/255f, 12f/255f);
        return t;
    }

    public override void EstablishRelationships()
    {
        Relationships daddy = new Relationships(this.Type);
        daddy.IsInPack(this.Type, 1f);
        daddy.EatenBy(CreatureTemplateType.ZapDaddyLongLegs, 0.95f);
        //daddy.HasDynamicRelationship(CreatureTemplate.Type.Slugcat, 200f);
    }

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new DaddyAI(acrit, acrit.world);

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new RadioLongLegs(acrit, acrit.world);

    public override CreatureState CreateState(AbstractCreature acrit) => new RadioLongLegs.DaddyState(acrit);
    public override ItemProperties? Properties(Creature crit)
    {
        if (crit is RadioLongLegs radiodll) {
            return new RadioProperties(radiodll);
        }
        return null;
    }

    public override void LoadResources(RainWorld rainWorld) { }

    public override CreatureTemplate.Type ArenaFallback() => CreatureTemplate.Type.DaddyLongLegs;
}