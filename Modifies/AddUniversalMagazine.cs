using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Logging;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Services.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SPTarkovAmmoCases.Modifies;

[Injectable(InjectionType.Scoped, null, OnLoadOrder.PostDBModLoader + 1)]
public class AddUniversalMagazine : IOnLoad {
    private ISptLogger<AddUniversalMagazine> Logger { get; }
    private DatabaseService DatabaseService { get; }
    private CustomItemService CustomItemService { get; }
    private ItemHelper ItemHelper { get; }
    private Double HandbookPrice { get; } = 10_0000D;
    private MongoId BaseId { get; } = new("692991ba6e9e97027c9b7300");
    private MongoId NewId { get; } = new("692991ba6e9e97027c9b7301");
    private MongoId RotateId { get; set; } = new("692991ba6e9e97027c9b7320");

#pragma warning disable IDE0290 // 使用主构造函数
    public AddUniversalMagazine (ISptLogger<AddUniversalMagazine> logger, DatabaseService databaseService, CustomItemService customItemService, ItemHelper itemHelper) {
        this.Logger = logger;
        this.DatabaseService = databaseService;
        this.CustomItemService = customItemService;
        this.ItemHelper = itemHelper;
    }
#pragma warning restore IDE0290 // 使用主构造函数

    public Task OnLoad () {
        this.RotateId = Helper.Miscellaneous.MongoIdCalc(this.RotateId, 1);
        NewItemFromCloneDetails newItem = new() {
            ItemTplToClone = ItemTpl.MAGAZINE_20X1MM_DRUM_20RND,
            NewId = this.NewId,
            ParentId = BaseClasses.MAGAZINE,
            FleaPriceRoubles = Math.Ceiling(this.HandbookPrice * 1.25),
            HandbookPriceRoubles = this.HandbookPrice,
            HandbookParentId = Constants.HandbookIdForContainer,
            Locales = new(){
                {"en",new(){Name = "universal magazine",ShortName = "universal",Description = "skydust™ universal magazine"}},
                {"ch",new(){Name = "万能弹匣",ShortName = "万能",Description = "skydust™ 万能弹匣"}}
            },
            OverrideProperties = new() {
                BackgroundColor = "blue",
                CanSellOnRagfair = false,
                Rarity = LootRarity.Not_exist,
                RarityPvE = "not_exist",
                Weight = 0.25,
                Width = 2,
                Height = 2,
                ExamineExperience = (Int32)Math.Ceiling(this.HandbookPrice / 10000),
                LootExperience = (Int32)Math.Ceiling(this.HandbookPrice / 10000),
                Cartridges = [
                    new(){
                        Name = "cartridges",
                        Id = this.RotateId,
                        Parent = this.NewId,
                        MaxCount = 120,
                        Properties = new(){
                            Filters = [
                                new(){
                                    Filter = [BaseClasses.AMMO]
                                }
                            ]
                        }

                    }
                ],
                CheckTimeModifier = 25D,
                LoadUnloadModifier = 25D,
                CanFast = false,
            }
        };
        CreateItemResult createItemResult = this.CustomItemService.CreateItemFromClone(newItem);
        if (createItemResult.Success is false) {
            this.Logger.Log(
                LogLevel.Info,
                String.Concat(Constants.LoggerPrefix, "AddUniversalMagazine.OnLoad() / failed / ", String.Join("；", createItemResult.Errors ?? Enumerable.Empty<String>())),
                LogTextColor.Yellow
            );
            return Task.CompletedTask;
        }

        Trader? trader = this.DatabaseService.GetTrader(Traders.MECHANIC);
        if (trader is null) {
            this.Logger.Log(
                LogLevel.Info,
                String.Concat(Constants.LoggerPrefix, "AddUniversalMagazine.OnLoad() / failed / trader not found"),
                LogTextColor.Yellow
            );
            return Task.CompletedTask;
        }
        this.RotateId = Helper.Miscellaneous.MongoIdCalc(this.RotateId, 1);
        trader.Assort.LoyalLevelItems.Add(this.RotateId, 4);
        trader.Assort.BarterScheme.Add(
            this.RotateId,
            [
                [
                    new(){
                        Template = ItemTpl.MONEY_ROUBLES,
                        Count = this.HandbookPrice,
                        Level = 15
                    }
                ]
            ]
        );
        trader.Assort.Items.Add(new() {
            Id = this.RotateId,
            Template = this.NewId,
            ParentId = "hideout",
            SlotId = "hideout",
            Upd = new() {
                UnlimitedCount = true,
                StackObjectsCount = 999,
                BuyRestrictionMax = 9,
                BuyRestrictionCurrent = 0
            }
        });

        Dictionary<MongoId, TemplateItem> templates = this.DatabaseService.GetItems();
        FieldInfo[] weaponFields = typeof(Weapons).GetFields(BindingFlags.Public|BindingFlags.Static);
        //IEnumerable<MongoId> weaponTpls = this.ItemHelper.GetItemTplsOfBaseType(BaseClass.WEAPON);
        IEnumerable<MongoId> weaponTpls = weaponFields.Select(x=>x.GetValue(null)).Where(x=>x is not null).Cast<MongoId>();
        IEnumerable<MongoId> excludeTpls = [
            ItemTpl.REVOLVER_CHIAPPA_RHINO_200DS_9X19,
            ItemTpl.REVOLVER_CHIAPPA_RHINO_50DS_357,
            ItemTpl.REVOLVER_RSH12_127X55,
            ItemTpl.REVOLVER_MTS25512_12GA_SHOTGUN,
            ItemTpl.REVOLVER_MILKOR_M32A1_MSGL_40MM_GRENADE_LAUNCHER,
            ItemTpl.FLARE_ROP30_REACTIVE_FLARE_CARTRIDGE_WHITE,
            ItemTpl.FLARE_RSP30_REACTIVE_SIGNAL_CARTRIDGE_BLUE,
            ItemTpl.FLARE_RSP30_REACTIVE_SIGNAL_CARTRIDGE_FIREWORK,
            ItemTpl.FLARE_RSP30_REACTIVE_SIGNAL_CARTRIDGE_GREEN,
            ItemTpl.FLARE_RSP30_REACTIVE_SIGNAL_CARTRIDGE_RED,
            ItemTpl.FLARE_RSP30_REACTIVE_SIGNAL_CARTRIDGE_SPECIAL_YELLOW,
            ItemTpl.FLARE_RSP30_REACTIVE_SIGNAL_CARTRIDGE_YELLOW,
            ItemTpl.SIGNALPISTOL_ZID_SP81_26X75_SIGNAL_PISTOL,
            ItemTpl.ROCKETLAUNCHER_RSHG2_725MM_ROCKET_LAUNCHER,
            ItemTpl.GRENADE_F1_HAND,
            ItemTpl.GRENADE_F1_HAND_GRENADE_REDUCED_DELAY,
            ItemTpl.GRENADE_M18_SMOKE_GRENADE_GREEN,
            ItemTpl.GRENADE_M67_HAND,
            ItemTpl.GRENADE_MODEL_7290_FLASH_BANG,
            ItemTpl.GRENADE_RDG2B_SMOKE,
            ItemTpl.GRENADE_RGD5_HAND,
            ItemTpl.GRENADE_RGN_HAND,
            ItemTpl.GRENADE_RGO_HAND,
            ItemTpl.GRENADE_V40_MINI,
            ItemTpl.GRENADE_VOG17_KHATTABKA_IMPROVISED_HAND,
            ItemTpl.GRENADE_VOG25_KHATTABKA_IMPROVISED_HAND,
            ItemTpl.GRENADE_ZARYA_STUN,
            ItemTpl.SHOTGUN_BENELLI_M3_SUPER_90_12GA_DUALMODE,
            ItemTpl.SHOTGUN_MOSSBERG_590A1_12GA_PUMPACTION,
            ItemTpl.SHOTGUN_MP133_12GA_PUMPACTION,
            ItemTpl.SHOTGUN_MP153_12GA_SEMIAUTOMATIC,
            ItemTpl.SHOTGUN_MP155_12GA_SEMIAUTOMATIC,
            ItemTpl.SHOTGUN_MP18_762X54R_SINGLESHOT_RIFLE,
            ItemTpl.SHOTGUN_MP431C_12GA_DOUBLEBARREL,
            ItemTpl.SHOTGUN_MP43_12GA_SAWEDOFF_DOUBLEBARREL,
            ItemTpl.SHOTGUN_REMINGTON_MODEL_870_12GA_PUMPACTION,
            ItemTpl.SHOTGUN_TOZ_KS23M_23X75MM_PUMPACTION,
            ItemTpl.GRENADELAUNCHER_FN40GL_01,
            ItemTpl.GRENADELAUNCHER_FN40GL_02,
            ItemTpl.GRENADELAUNCHER_FN40GL_03
        ];
        IEnumerable<MongoId> filteredTpls = weaponTpls.Except(excludeTpls);
        foreach (MongoId id in filteredTpls) {
            Console.WriteLine("check for {0}",id);
            if (templates.TryGetValue(id, out TemplateItem? template) is false || template is null) { continue; }
            if (template.Properties is null || template.Properties.Slots is null || template.Properties.Slots.Any() is false) { continue; }
            Console.WriteLine("adding for {0}",id);
            foreach (Slot slot in template.Properties.Slots) {
                if (slot.Name is not "mod_magazine") { continue; }
                if (slot.Properties is null || slot.Properties.Filters is null || slot.Properties.Filters.Any() is false) { continue; }
                SlotFilter slotFilter = slot.Properties.Filters.First();
                if (slotFilter.Filter is null) { continue; }
                _ = slotFilter.Filter.Add(this.NewId);
                Console.WriteLine("added for {0}",id);
                break;
            }
        }

        this.Logger.Log(
            LogLevel.Info,
            String.Concat(Constants.LoggerPrefix, "AddUniversalMagazine.OnLoad() / success / ", this.BaseId, " / ", this.RotateId),
            LogTextColor.Green
        );
        return Task.CompletedTask;
    }
}
