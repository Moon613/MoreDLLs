using System.Diagnostics.CodeAnalysis;

namespace MoreDlls;

public static class CreatureTemplateType
{
    [AllowNull] public static CreatureTemplate.Type ExplosiveDaddyLongLegs = new(nameof(ExplosiveDaddyLongLegs), true);
    [AllowNull] public static CreatureTemplate.Type ZapDaddyLongLegs = new(nameof(ZapDaddyLongLegs), true);
    [AllowNull] public static CreatureTemplate.Type RadioDaddyLongLegs = new(nameof(RadioDaddyLongLegs), true);
}

public static class SandboxUnlockID
{
    [AllowNull] public static MultiplayerUnlocks.SandboxUnlockID ExplosiveDaddyLongLegs = new(nameof(ExplosiveDaddyLongLegs), true);
    [AllowNull] public static MultiplayerUnlocks.SandboxUnlockID ZapDaddyLongLegs = new(nameof(ZapDaddyLongLegs), true);
    [AllowNull] public static MultiplayerUnlocks.SandboxUnlockID RadioDaddyLongLegs = new(nameof(RadioDaddyLongLegs), true);
}

public static class StaticElectricityEnums
{
    public static void RegisterValues()
    {
        StaticElectricityEnums.StaticElectricity = new SoundID("staticelectricityshort", true);
    }
    public static SoundID? StaticElectricity;
}

public class DLLValues
{
    public bool albino = false;
    
    //Defunct int, used to be used to determine the color of the DLL & type when it was just a hook and not a Fisobs creature.
    //public int eyecolor = 0;
    public float t = 0;
    public float initialRColor = 0f;
    public float initialGColor = 0f;
    public float initialBColor = 0f;
    public float explosionCooldown = 0f;
    public int grabDecisionCooldown = 0;
    public int coreZapCooldown = 0;
    public bool maxZapxReached = false;
    public bool minZapxReached = false;
    public bool maxZapyReached = false;
    public bool minZapyReached = false;
    public float maxZapx = 0;
    public float minZapx = 0;
    public float maxZapy = 0;
    public float minZapy = 0;
}