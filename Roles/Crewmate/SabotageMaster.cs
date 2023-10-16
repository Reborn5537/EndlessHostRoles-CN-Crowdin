using System.Collections.Generic;
using System.Linq;

namespace TOHE.Roles.Crewmate;

public static class SabotageMaster
{
    private static readonly int Id = 7000;
    public static List<byte> playerIdList = new();

    public static OptionItem SkillLimit;
    public static OptionItem FixesDoors;
    public static OptionItem FixesReactors;
    public static OptionItem FixesOxygens;
    public static OptionItem FixesComms;
    public static OptionItem FixesElectrical;
    public static OptionItem SMAbilityUseGainWithEachTaskCompleted;
    public static OptionItem UsesUsedWhenFixingReactorOrO2;
    public static OptionItem UsesUsedWhenFixingLightsOrComms;
    public static float UsedSkillCount;

    private static bool DoorsProgressing;

    public static void SetupCustomOption()
    {
        Options.SetupRoleOptions(Id, TabGroup.CrewmateRoles, CustomRoles.SabotageMaster);
        SkillLimit = IntegerOptionItem.Create(Id + 10, "SabotageMasterSkillLimit", new(0, 80, 1), 2, TabGroup.CrewmateRoles, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster])
            .SetValueFormat(OptionFormat.Times);
        FixesDoors = BooleanOptionItem.Create(Id + 11, "SabotageMasterFixesDoors", true, TabGroup.CrewmateRoles, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster]);
        FixesReactors = BooleanOptionItem.Create(Id + 12, "SabotageMasterFixesReactors", true, TabGroup.CrewmateRoles, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster]);
        FixesOxygens = BooleanOptionItem.Create(Id + 13, "SabotageMasterFixesOxygens", true, TabGroup.CrewmateRoles, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster]);
        FixesComms = BooleanOptionItem.Create(Id + 14, "SabotageMasterFixesCommunications", true, TabGroup.CrewmateRoles, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster]);
        FixesElectrical = BooleanOptionItem.Create(Id + 15, "SabotageMasterFixesElectrical", true, TabGroup.CrewmateRoles, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster]);
        SMAbilityUseGainWithEachTaskCompleted = FloatOptionItem.Create(Id + 16, "AbilityUseGainWithEachTaskCompleted", new(0f, 5f, 0.1f), 3f, TabGroup.CrewmateRoles, false)
            .SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster])
            .SetValueFormat(OptionFormat.Times);
        UsesUsedWhenFixingReactorOrO2 = FloatOptionItem.Create(Id + 17, "SMUsesUsedWhenFixingReactorOrO2", new(0f, 5f, 0.1f), 4f, TabGroup.CrewmateRoles, false)
            .SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster])
            .SetValueFormat(OptionFormat.Times);
        UsesUsedWhenFixingLightsOrComms = FloatOptionItem.Create(Id + 18, "SMUsesUsedWhenFixingLightsOrComms", new(0f, 5f, 0.1f), 1f, TabGroup.CrewmateRoles, false)
            .SetParent(Options.CustomRoleSpawnChances[CustomRoles.SabotageMaster])
            .SetValueFormat(OptionFormat.Times);
    }
    public static void Init()
    {
        playerIdList = new();
        UsedSkillCount = 0;
    }
    public static void Add(byte playerId)
    {
        playerIdList.Add(playerId);
    }
    public static bool IsEnable() => playerIdList.Any();
    public static void RepairSystem(ShipStatus __instance, SystemTypes systemType, byte amount)
    {
        switch (systemType)
        {
            case SystemTypes.Reactor:
                if (!FixesReactors.GetBool()) break;
                if (SkillLimit.GetFloat() > 0 && UsedSkillCount + UsesUsedWhenFixingReactorOrO2.GetFloat() - 1 >= SkillLimit.GetFloat()) break;
                if (amount is 64 or 65)
                {
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16);
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 17);
                    UsedSkillCount += UsesUsedWhenFixingReactorOrO2.GetFloat();
                }
                break;
            case SystemTypes.Laboratory:
                if (!FixesReactors.GetBool()) break;
                if (SkillLimit.GetFloat() > 0 && UsedSkillCount + UsesUsedWhenFixingReactorOrO2.GetFloat() - 1 >= SkillLimit.GetFloat()) break;
                if (amount is 64 or 65)
                {
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.Laboratory, 67);
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.Laboratory, 66);
                    UsedSkillCount += UsesUsedWhenFixingReactorOrO2.GetFloat();
                }
                break;
            case SystemTypes.LifeSupp:
                if (!FixesOxygens.GetBool()) break;
                if (SkillLimit.GetFloat() > 0 && UsedSkillCount + UsesUsedWhenFixingReactorOrO2.GetFloat() - 1 >= SkillLimit.GetFloat()) break;
                if (amount is 64 or 65)
                {
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 67);
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 66);
                    UsedSkillCount += UsesUsedWhenFixingReactorOrO2.GetFloat();
                }
                break;
            case SystemTypes.Comms:
                if (!FixesComms.GetBool()) break;
                if (SkillLimit.GetFloat() > 0 && UsedSkillCount + UsesUsedWhenFixingLightsOrComms.GetFloat() - 1 >= SkillLimit.GetFloat()) break;
                if (amount is 64 or 65)
                {
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16);
                    ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 17);
                    UsedSkillCount += UsesUsedWhenFixingLightsOrComms.GetFloat();
                }
                break;
            case SystemTypes.Doors:
                if (!FixesDoors.GetBool()) break;
                if (DoorsProgressing == true) break;

                int mapId = Main.NormalOptions.MapId;
                if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay) mapId = AmongUsClient.Instance.TutorialMapId;

                DoorsProgressing = true;
                if (mapId == 2)
                {
                    //Polus
                    RepairSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 71, 72);
                    RepairSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 67, 68);
                    RepairSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 64, 66);
                    RepairSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 73, 74);
                }
                else if (mapId == 4)
                {
                    //Airship
                    RepairSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 64, 67);
                    RepairSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 71, 73);
                    RepairSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 74, 75);
                    RepairSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 76, 78);
                    RepairSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 68, 70);
                    RepairSystemPatch.CheckAndOpenDoorsRange(__instance, amount, 83, 84);
                }
                DoorsProgressing = false;
                break;
        }
    }
    public static void SwitchSystemRepair(SwitchSystem __instance, byte amount)
    {
        if (!FixesElectrical.GetBool()) return;
        if (SkillLimit.GetFloat() > 0 &&
            UsedSkillCount + UsesUsedWhenFixingLightsOrComms.GetFloat() - 1 >= SkillLimit.GetFloat())
            return;

        if (amount is >= 0 and <= 4)
        {
            __instance.ActualSwitches = 0;
            __instance.ExpectedSwitches = 0;
            UsedSkillCount += UsesUsedWhenFixingLightsOrComms.GetFloat();
        }
    }
}