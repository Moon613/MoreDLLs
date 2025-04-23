using Fisobs.Creatures;
using Fisobs.Core;
using System.Collections.Generic;
using Fisobs.Sandbox;
using static PathCost.Legality;
using UnityEngine;
using DevInterface;
using RWCustom;

namespace MoreDlls;

sealed class ZapDllCritob : Critob
{
    public ZapDllCritob() : base(CreatureTemplateType.ZapDaddyLongLegs)
    {
        Icon = new SimpleIcon("Kill_Daddy", Color.cyan);
        RegisterUnlock(KillScore.Configurable(25), SandboxUnlockID.ZapDaddyLongLegs);
        SandboxPerformanceCost = new(3f, 1.5f);
        LoadedPerformanceCost = 200f;
        ShelterDanger = ShelterDanger.Hostile;
        ZapHooks.Apply();
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

    public override void TileIsAllowed(AImap map, IntVector2 tilePos, ref bool? allow) => allow = map.getTerrainProximity(tilePos.x, tilePos.y) > 1;

    public override int ExpeditionScore() => 35;

    public override Color DevtoolsMapColor(AbstractCreature acrit) => Color.cyan;

    public override string DevtoolsMapName(AbstractCreature acrit) => "zll";

    public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction() => new[] { RoomAttractivenessPanel.Category.LikesInside };

    public override IEnumerable<string> WorldFileAliases() => new[] { "zapdll" };

    public override CreatureTemplate CreateTemplate()
    {
        var t = new CreatureFormula(CreatureTemplate.Type.DaddyLongLegs, Type, "ZapDll")
        {
            TileResistances = new()
            {
                Air = new(1f, Allowed),
                //Floor = new(0.0f, Allowed),
                //Solid = new(0.0f, Allowed),
                //Wall = new(0.0f, Allowed),
                //Corridor = new(0.0f, Allowed),
                //Climb = new(0.0f, Allowed),
                //Ceiling = new(0.0f, Allowed)
            },
            ConnectionResistances = new() 
            {
                Standard = new(1f, Allowed),
                ShortCut = new(1f, Allowed),
                BigCreatureShortCutSqueeze = new(10f, Allowed),
                OffScreenMovement = new(1f, Allowed),
                BetweenRooms = new(10f, Allowed)
            },
            DefaultRelationship = new(CreatureTemplate.Relationship.Type.Eats, 1f),
            DamageResistances = new() { Base = 175f, Electric = .2f },
            StunResistances = new() { Base = 200f , Electric = 1f},
            HasAI = true,
            Pathing = PreBakedPathing.Ancestral(CreatureTemplate.Type.DaddyLongLegs),
        }.IntoTemplate();
        return t;
    }

    public override void EstablishRelationships()
    {
        var daddy = new Relationships(Type);
        daddy.Ignores(Type);
        daddy.Eats(CreatureTemplateType.RadioDaddyLongLegs, 100f);
    }

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new DaddyAI(acrit, acrit.world);

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new DaddyLongLegs(acrit, acrit.world);

    public override CreatureState CreateState(AbstractCreature acrit) => new DaddyLongLegs.DaddyState(acrit);

    public override void LoadResources(RainWorld rainWorld) { }

    public override CreatureTemplate.Type ArenaFallback() => CreatureTemplate.Type.DaddyLongLegs;
}