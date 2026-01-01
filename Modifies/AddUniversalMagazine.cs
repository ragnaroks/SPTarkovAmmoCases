using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Logging;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Services.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPTarkovAmmoCases.Modifies;

[Injectable(InjectionType.Scoped, null, OnLoadOrder.PostDBModLoader + 1)]
public class AddUniversalMagazine : IOnLoad {
    private ISptLogger<AddUniversalMagazine> Logger { get; }
    private DatabaseService DatabaseService { get; }
    private CustomItemService CustomItemService { get; }
    private ConfigServer ConfigServer { get; }
    private Double HandbookPrice { get; } = 10_0000D;
    private MongoId BaseId { get; } = new("692991ba6e9e97027c9b7400");
    private MongoId NewId { get; } = new("692991ba6e9e97027c9b7401");
    private MongoId RotateId { get; set; } = new("692991ba6e9e97027c9b7420");

#pragma warning disable IDE0290 // 使用主构造函数
    public AddUniversalMagazine (ISptLogger<AddUniversalMagazine> logger, DatabaseService databaseService, CustomItemService customItemService, ConfigServer configServer) {
        this.Logger = logger;
        this.DatabaseService = databaseService;
        this.CustomItemService = customItemService;
        this.ConfigServer = configServer;
    }
#pragma warning restore IDE0290 // 使用主构造函数

    public Task OnLoad () {
        this.RotateId = Helper.Miscellaneous.MongoIdCalc(this.RotateId, 1);
        NewItemFromCloneDetails newItem = new() {
            // IDK why the magazine of toygun will cause a strange bug in detail window 
            ItemTplToClone = ItemTpl.MAGAZINE_9X18PM_PM_8RND,
            NewId = this.NewId,
            ParentId = BaseClasses.MAGAZINE,
            FleaPriceRoubles = Math.Ceiling(this.HandbookPrice * 1.25),
            HandbookPriceRoubles = this.HandbookPrice,
            HandbookParentId = Constants.HandbookIdForMagazine,
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
                Width = 1,
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
        foreach (TemplateItem template in templates.Values) {
            if (template.Properties is null) { continue; }
            if (template.Properties.IsFlareGun is true || template.Properties.IsGrenadeLauncher is true) { continue; }
            if (template.Properties.BFirerate is null or < 29D) { continue; }
            if (template.Properties.Slots is null || template.Properties.Slots.Any() is false) { continue; }
            //if (template.Properties.Chambers is null || template.Properties.Slots.Any() is false) { continue; }
            if (template.Properties.WeapFireType is null || template.Properties.WeapFireType.Count < 1) { continue; }
            foreach (Slot slot in template.Properties.Slots) {
                if (slot.Name is not "mod_magazine") { continue; }
                if (slot.Properties is null || slot.Properties.Filters is null || slot.Properties.Filters.Any() is false) { continue; }
                SlotFilter slotFilter = slot.Properties.Filters.First();
                if (slotFilter.Filter is null) { continue; }
                _ = slotFilter.Filter.Add(this.NewId);
                break;
            }
        }

        BotConfig botConfig = this.ConfigServer.GetConfig<BotConfig>();
        foreach (KeyValuePair<String, EquipmentFilters?> equipments in botConfig.Equipment) {
            if (equipments.Value is null || equipments.Value.Blacklist is null) { continue; }
            foreach (EquipmentFilterDetails details in equipments.Value.Blacklist) {
                if (details.Equipment is null) { continue; }
                foreach (KeyValuePair<String, HashSet<MongoId>> equipment in details.Equipment) {
                    if (equipment.Key is not "mod_magazine") { continue; }
                    _ = equipment.Value.Add(this.NewId);
                    break;
                }
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
