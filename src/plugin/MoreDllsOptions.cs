using BepInEx.Logging;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace MoreDlls;
public class MoreDllsOptions : OptionInterface {
    private readonly ManualLogSource Logger;
    public readonly Configurable<int> poleShockRangeX = new Configurable<int>(700);
    public readonly Configurable<int> poleShockRangeY = new Configurable<int>(400);
    public readonly Configurable<int> poleZapFrequency = new Configurable<int>(10);
    public readonly Configurable<bool> allowPoleZaps = new Configurable<bool>(false);
    private UIelement[] UIArrPlayerOptions;

    public MoreDllsOptions(MoreDlls pluginInstance, ManualLogSource logSource) {
        Logger = logSource;
        poleShockRangeX = config.Bind("poleShockRangeX", 700, new ConfigAcceptableRange<int>(1, 1400));
        poleShockRangeY = config.Bind("poleShockRangeY", 400, new ConfigAcceptableRange<int>(1, 800));
        poleZapFrequency = config.Bind("poleZapFrequency", 10, new ConfigAcceptableRange<int>(1, 50));
        allowPoleZaps = config.Bind("allowPoleZaps", true, (ConfigurableInfo)null);
    }

    public override void Initialize() {
        OpTab opTab = new OpTab(this, "OptionsLongLegs");
        this.Tabs = new[]
        {
            opTab
        };

        UIArrPlayerOptions = new UIelement[]
        {
            new OpLabel(260f, 570f, "Options", true),
            
            new OpLabel(150f, 520f, "Pole Shock Range Horizontal"),
            new OpUpdown(poleShockRangeX, new Vector2(330f,515f), 80f) {description = "Determines the length of pole ZLLs can shock on the horizontal. 700 for 1 screen wide (assuming start is in the middle of the room)."},
            
            new OpLabel(150f, 480f, "Pole Shock Range Vertical"),
            new OpUpdown(poleShockRangeY, new Vector2(330f,475f), 80f) {description = "Determines the length of pole ZLLs can shock on the vertical. 400 for 1 screen tall (assuming start is in the middle of the room)."},
             
            new OpLabel(150f, 440f, "Frequency of Pole Zaps"),
            new OpUpdown(poleZapFrequency, new Vector2(330f,435f), 80f) {description = "The frequency at which ZLLs will zap poles"},

            new OpLabel(180f, 400f, "Allow Pole Zaps"),
            new OpCheckBox(allowPoleZaps, new Vector2(360f,400f)) { description = "Allows ZLLs to electrocute poles on touch" },
           
        };
        opTab.AddItems(UIArrPlayerOptions);
    }
}
