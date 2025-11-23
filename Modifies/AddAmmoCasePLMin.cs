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
public class AddAmmoCasePLMin : IOnLoad {
    private ISptLogger<AddAmmoCasePLMin> Logger { get; }
    private DatabaseService DatabaseService { get; }
    private CustomItemService CustomItemService { get; }
    private ItemHelper ItemHelper { get; }
    private Double HandbookPrice { get; } = 100_0000D;
    private MongoId BaseId { get; } = new("681058a713ee981aa38d8700");
    private MongoId NewId { get; } = new("681058a713ee981aa38d8701");
    private MongoId RotateId { get; set; } = new("681058a713ee981aa38d8720");

#pragma warning disable IDE0290 // 使用主构造函数
    public AddAmmoCasePLMin (ISptLogger<AddAmmoCasePLMin> logger, DatabaseService databaseService, CustomItemService customItemService, ItemHelper itemHelper) {
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
            if (template.Properties.PenetrationPower is null or < 0 or > 9) { continue; }
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
                {"en",new(){Name = "PLMin ammo case",ShortName = "PLMin",Description = "skydust™ PLMin ammo case, can store ammos those penetration power less than 10"}},
                {"ch",new(){Name = "PLMin 弹药箱",ShortName = "PLMin",Description = "skydust™ PLMin 弹药箱，可存放穿透力 10 以下的弹药"}}
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
                String.Concat(Constants.LoggerPrefix, "AddAmmoCasePLMin.OnLoad() / failed / ", String.Join("；", createItemResult.Errors ?? Enumerable.Empty<String>())),
                LogTextColor.Yellow
            );
            return Task.CompletedTask;
        }

        Trader? trader = this.DatabaseService.GetTrader(Traders.REF);
        if (trader is null) {
            this.Logger.Log(
                LogLevel.Info,
                String.Concat(Constants.LoggerPrefix, "AddAmmoCasePLMin.OnLoad() / failed / trader not found"),
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

        IEnumerable<MongoId> caseTpls = [ItemTpl.CONTAINER_THICC_ITEM_CASE,ItemTpl.CONTAINER_ITEM_CASE];
        foreach (MongoId id in caseTpls) {
            if(templates.TryGetValue(id, out TemplateItem? template) is false || template is null){continue;}
            if(template.Properties is null || template.Properties.Grids is null || template.Properties.Grids.Any() is false){continue;}
            foreach (Grid grid in template.Properties.Grids) {
                if(grid.Properties is null || grid.Properties.Filters is null || grid.Properties.Filters.Any() is false){continue;}
                GridFilter gridFilter = grid.Properties.Filters.First();
                if(gridFilter.Filter is null){continue;}
                _ = gridFilter.Filter.Add(this.NewId);
                break;
            }
        }

        this.Logger.Log(
            LogLevel.Info,
            String.Concat(Constants.LoggerPrefix, "AddAmmoCasePLMin.OnLoad() / success / ", this.BaseId, " / ", this.RotateId),
            LogTextColor.Green
        );
        return Task.CompletedTask;
    }
}
