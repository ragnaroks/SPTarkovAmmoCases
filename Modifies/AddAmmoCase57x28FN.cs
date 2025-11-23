using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
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
public class AddAmmoCase57x28FN : IOnLoad {
    private ISptLogger<AddAmmoCase57x28FN> Logger { get; }
    private DatabaseService DatabaseService { get; }
    private CustomItemService CustomItemService { get; }
    private Double HandbookPrice { get; } = 50_0000D;
    private MongoId BaseId { get; } = new("6809180bd9e2604b7714e000");
    private MongoId NewId { get; } = new("6809180bd9e2604b7714e001");
    private MongoId RotateId { get; set; } = new("6809180bd9e2604b7714e020");
    private IEnumerable<MongoId> ItemTpls { get; } = [
        ItemTpl.AMMO_57X28_SS190,
        ItemTpl.AMMO_57X28_L191,
        ItemTpl.AMMO_57X28_SB193,
        ItemTpl.AMMO_57X28_SS197SR,
        ItemTpl.AMMO_57X28_SS198LF,
        ItemTpl.AMMO_57X28_R37X,
        ItemTpl.AMMO_57X28_R37F,
        ItemTpl.MONEY_ROUBLES,
        ItemTpl.MONEY_ROUBLES,
        ItemTpl.MONEY_ROUBLES,
        ItemTpl.MONEY_ROUBLES,
        ItemTpl.MONEY_ROUBLES,
        ItemTpl.MONEY_ROUBLES,
        ItemTpl.MONEY_ROUBLES
    ];

#pragma warning disable IDE0290 // 使用主构造函数
    public AddAmmoCase57x28FN (ISptLogger<AddAmmoCase57x28FN> logger, DatabaseService databaseService, CustomItemService customItemService) {
        this.Logger = logger;
        this.DatabaseService = databaseService;
        this.CustomItemService = customItemService;
    }
#pragma warning restore IDE0290 // 使用主构造函数

    public Task OnLoad () {
        IList<Grid> overrideGrids = [];
        Int32 columeIndex = 0;
        foreach (MongoId id in this.ItemTpls) {
            this.RotateId = Helper.Miscellaneous.MongoIdCalc(this.RotateId, 1);
            columeIndex++;
            overrideGrids.Add(new() {
                Id = this.RotateId,
                Name = String.Concat("colume", columeIndex),
                Parent = this.NewId,
                Prototype = "55d329c24bdc2d892f8b4567",
                Properties = new() {
                    CellsH = 1,
                    CellsV = 14,
                    Filters = [
                        new(){
                            Filter = [id],
                            ExcludedFilter = null
                        }
                    ],
                    IsSortingTable = false,
                    MaxCount = 0,
                    MaxWeight = 0,
                    MinCount = 0
                }
            });
        }

        NewItemFromCloneDetails newItem = new() {
            ItemTplToClone = ItemTpl.CONTAINER_AMMUNITION_CASE,
            NewId = this.NewId,
            ParentId = BaseClasses.SIMPLE_CONTAINER,
            FleaPriceRoubles = Math.Ceiling(this.HandbookPrice * 1.25),
            HandbookPriceRoubles = this.HandbookPrice,
            HandbookParentId = Constants.HandbookIdForContainer,
            Locales = new(){
                {"en",new(){Name = "5.7x28FN ammo case",ShortName = "5.7x28FN",Description = "skydust™ 5.7x28FN ammo case"}},
                {"ch",new(){Name = "5.7x28FN 弹药箱",ShortName = "5.7x28FN",Description = "skydust™ 5.7x28FN 弹药箱"}}
            },
            OverrideProperties = new() {
                BackgroundColor = "blue",
                CanSellOnRagfair = false,
                Rarity = LootRarity.Not_exist,
                RarityPvE = "not_exist",
                Weight = 0,
                ExamineExperience = (Int32)Math.Ceiling(this.HandbookPrice / 10000),
                LootExperience = (Int32)Math.Ceiling(this.HandbookPrice / 10000),
                Grids = overrideGrids
            }
        };
        CreateItemResult createItemResult = this.CustomItemService.CreateItemFromClone(newItem);
        if (createItemResult.Success is false) {
            this.Logger.Log(
                LogLevel.Info,
                String.Concat(Constants.LoggerPrefix, "AddAmmoCase57x28FN.OnLoad() / failed / ", String.Join("；", createItemResult.Errors ?? Enumerable.Empty<String>())),
                LogTextColor.Yellow
            );
            return Task.CompletedTask;
        }

        Trader? trader = this.DatabaseService.GetTrader(Traders.REF);
        if (trader is null) {
            this.Logger.Log(
                LogLevel.Info,
                String.Concat(Constants.LoggerPrefix, "AddAmmoCase57x28FN.OnLoad() / failed / trader not found"),
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

        Dictionary<MongoId, TemplateItem> templates = this.DatabaseService.GetItems();
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
            String.Concat(Constants.LoggerPrefix, "AddAmmoCase57x28FN.OnLoad() / success / ", this.BaseId, " / ", this.RotateId),
            LogTextColor.Green
        );
        return Task.CompletedTask;
    }
}
