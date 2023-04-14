using Fisobs.Properties;
using MoreSlugcats;

namespace MoreDlls;

sealed class RadioProperties : ItemProperties
{
    public readonly DaddyLongLegs radiodll;

    public RadioProperties(DaddyLongLegs radiodll)
    {
        this.radiodll = radiodll;
    }

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
    {
        if (radiodll.State.alive) {
            grabability = Player.ObjectGrabability.BigOneHand;
        } else {
            grabability = Player.ObjectGrabability.Drag;
        }
    }

    public override void Throwable(Player player, ref bool throwable)
    {
        throwable = true;
    }
}