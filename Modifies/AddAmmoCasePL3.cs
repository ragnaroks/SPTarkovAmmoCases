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
using System.Threading.Tasks;

namespace SPTarkovAmmoCases.Modifies;

[Injectable(InjectionType.Scoped, null, OnLoadOrder.PostDBModLoader + 1)]
public class AddAmmoCasePL3 : IOnLoad {
    private ISptLogger<AddAmmoCasePL3> Logger { get; }
    private DatabaseService DatabaseService { get; }
    private CustomItemService CustomItemService { get; }
    private ItemHelper ItemHelper { get; }
    private Double HandbookPrice { get; } = 100_0000D;
    private MongoId BaseId { get; } = new("68108b4af1b8c46964449300");
    private MongoId NewId { get; } = new("68108b4af1b8c46964449301");
    private MongoId RotateId { get; set; } = new("68108b4af1b8c46964449320");

#pragma warning disable IDE0290 // 使用主构造函数
    public AddAmmoCasePL3 (ISptLogger<AddAmmoCasePL3> logger, DatabaseService databaseService, CustomItemService customItemService, ItemHelper itemHelper) {
        this.Logger = logger;
        this.DatabaseService = databaseService;
        this.CustomItemService = customItemService;
        this.ItemHelper = itemHelper;
    }
#pragma warning restore IDE0290 // 使用主构造函数

    public Task OnLoad () {
        IEnumerable<MongoId> ammoTpls = this.ItemHelper.GetItemTplsOfBaseType(BaseClasses.AMMO);
        Dictionary<MongoId, TemplateItem> templates = this.DatabaseService.GetItems();

        HashSet<MongoId> itemTpls = [];
        foreach (MongoId id in ammoTpls) {
            if (templates.TryGetValue(id, out TemplateItem? template) is false) { continue; }
            if (template.Properties is null) { continue; }
            if (template.Properties.FlareTypes is not null && template.Properties.FlareTypes.Any()) { continue; }
            if (template.Properties.InitialSpeed is null or < 100) { continue; }
            if (template.Properties.PenetrationPower is null or < 30 or > 39) { continue; }
            _ = itemTpls.Add(id);
        }

        this.RotateId = Helper.Miscellaneous.MongoIdCalc(this.RotateId, 1);
        NewItemFromCloneDetails newItem = new() {
            ItemTplToClone = ItemTpl.CONTAINER_AMMUNITION_CASE,
            NewId = this.NewId,
            ParentId = BaseClasses.SIMPLE_CONTAINER,
            FleaPriceRoubles = Math.Ceiling(this.HandbookPrice * 1.25),
            HandbookPriceRoubles = this.HandbookPrice,
            HandbookParentId = Constants.HandbookIdForContainer,
            Locales = new(){
                {"en",new(){Name = "PL3 ammo case",ShortName = "PL3",Description = "skydust™ PL3 ammo case, can store ammos those penetration power between 30 and 39"}},
                {"ch",new(){Name = "PL3 弹药箱",ShortName = "PL3",Description = "skydust™ PL3 弹药箱，可存放穿透力 30 至 39 的弹药"}}
            },
            OverrideProperties = new() {
                BackgroundColor = "blue",
                CanSellOnRagfair = false,
                Rarity = LootRarity.Not_exist,
                RarityPvE = "not_exist",
                Weight = 0,
                ExamineExperience = (Int32)Math.Ceiling(this.HandbookPrice / 10000),
                LootExperience = (Int32)Math.Ceiling(this.HandbookPrice / 10000),
                Grids = [
                    new() {
                        Id = this.RotateId,
                        Name = "main",
                        Parent = this.NewId,
                        Prototype = "55d329c24bdc2d892f8b4567",
                        Properties = new() {
                            CellsH = 14,
                            CellsV = 14,
                            Filters = [
                                new(){
                                    Filter = itemTpls,
                                    ExcludedFilter = null
                                }
                            ],
                            IsSortingTable = false,
                            MaxCount = 0,
                            MaxWeight = 0,
                            MinCount = 0
                        }
                    }
                ]
            }
        };
        CreateItemResult createItemResult = this.CustomItemService.CreateItemFromClone(newItem);
        if (createItemResult.Success is false) {
            this.Logger.Log(
                LogLevel.Info,
                String.Concat(Constants.LoggerPrefix, "AddAmmoCasePL3.OnLoad() / failed / ", String.Join("；", createItemResult.Errors ?? Enumerable.Empty<String>())),
                LogTextColor.Yellow
            );
            return Task.CompletedTask;
        }

        Trader? trader = this.DatabaseService.GetTrader(Traders.REF);
        if (trader is null) {
            this.Logger.Log(
                LogLevel.Info,
                String.Concat(Constants.LoggerPrefix, "AddAmmoCasePL3.OnLoad() / failed / trader not found"),
                LogTextColor.Yellow
            );
            return Task.CompletedTask;
        }
        this.RotateId = Helper.Miscellaneous.MongoIdCalc(this.RotateId, 1);
        trader.Assort.LoyalLevelItems.Add(this.RotateId, 1);
        trader.Assort.BarterScheme.Add(
            this.RotateId,
            [
                [
                    new(){
                        Template = ItemTpl.MONEY_GP_COIN,
                        Count = Math.Ceiling(this.HandbookPrice / Constants.GPCoinValue),
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

        this.Logger.Log(
            LogLevel.Info,
            String.Concat(Constants.LoggerPrefix, "AddAmmoCasePL3.OnLoad() / success / ", this.BaseId, " / ", this.RotateId),
            LogTextColor.Green
        );
        return Task.CompletedTask;
    }
}
