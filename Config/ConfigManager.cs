using System.Collections.Generic;
using UnityEngine;

namespace HaxxToyBox.Config;

public static class ConfigManager
{
    internal static readonly Dictionary<string, ConfigElement> ConfigElements = new();

    public static ConfigHandler Handler { get; private set; }

    public static ConfigElement Canvas_Toggle;
    public static ConfigElement SpeedUp_Toggle;
    public static ConfigElement SpeedDown_Toggle;
    public static ConfigElement Recover_Toggle;
    public static ConfigElement RefreshBounty_Toggle;
    public static ConfigElement Map0;
    public static ConfigElement Map1;
    public static ConfigElement Map2;
    public static ConfigElement Map3;
    public static ConfigElement Map4;
    public static ConfigElement Map5;
    public static bool isHook = true;
    public static void Init(ConfigHandler configHandler)
    {
        Handler = configHandler;

        CreateConfigElements();

        Handler.LoadConfig();
    }

    internal static void RegisterConfigElement(ConfigElement configElement)
    {
        Handler.RegisterConfigElement(configElement);
        ConfigElements.Add(configElement.Name, configElement);
    }

    private static void CreateConfigElements()
    {
        Canvas_Toggle = new("HaxxToyBox Toggle",
            "The key to show and hide ToyBox panel.",
            KeyCode.Tab);

        SpeedUp_Toggle = new("SpeedUp Toggle",
            "The key to increase game speed.",
            KeyCode.Equals);
        SpeedDown_Toggle = new("SpeedDown Toggle",
            "The key to decrease game speed.",
            KeyCode.Minus);

        Recover_Toggle = new("Recover Toggle",
            "The key to recover all statuses of the entire team.",
            KeyCode.F1);
        RefreshBounty_Toggle = new("RefreshBounty Toggle",
            "This key is used to refresh tasks.",
            KeyCode.F2);
        Map0 = new("Enable Map WuMingXiaoCun",
            "This key is used to Enable Map WuMingXiaoCun.",
            KeyCode.F3);
        Map1 = new("Enable Map XiangFanDiQu",
            "This key is used Enable Map XiangFanDiQu.",
            KeyCode.F4);
        Map2 = new("Enable Map ZhongZhouDiQu",
            "This key is used to Enable Map ZhongZhouDiQu.",
            KeyCode.F5);
        Map3 = new("Enable Map WuYueDiQu",
            "This key is used to Enable Map WuYueDiQu.",
            KeyCode.F6);
        Map4 = new("Enable Map XiNanDiQu",
            "This key is used to Enable Map XiNanDiQu.",
            KeyCode.F7);
        Map5 = new("Enable Map XiYuDiQu",
            "This key is used to Enable Map XiYuDiQu.",
            KeyCode.F8);
    }
}
