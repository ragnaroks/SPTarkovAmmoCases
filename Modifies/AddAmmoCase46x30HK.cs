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
public class AddAmmoCase46x30HK : IOnLoad {
    private ISptLogger<AddAmmoCase46x30HK> Logger { get; }
    private DatabaseService DatabaseService { get; }
    private CustomItemService CustomItemService { get; }
    private Double HandbookPrice { get; } = 50_0000D;
    private MongoId BaseId { get; } = new("680916d2bb472dd9599db000");
    private MongoId NewId { get; } = new("680916d2bb472dd9599db001");
    private MongoId RotateId { get; set; } = new("680916d2bb472dd9599db020");
    private IEnumerable<MongoId> ItemTpls { get; } = [
        ItemTpl.AMMO_46X30_AP_SX,
        ItemTpl.AMMO_46X30_FMJ_SX,
        ItemTpl.AMMO_46X30_JSP_SX,
        ItemTpl.AMMO_46X30_SUBSONIC_SX,
        ItemTpl.AMMO_46X30_ACTION_SX,
        ItemTpl.MONEY_ROUBLES,
        ItemTpl.MONEY_ROUBLES,
        ItemTpl.MONEY_ROUBLES,
        ItemTpl.MONEY_ROUBLES,
        ItemTpl.MONEY_ROUBLES,
        ItemTpl.MONEY_ROUBLES,
        ItemTpl.MONEY_ROUBLES,
        ItemTpl.MONEY_ROUBLES,
        ItemTpl.MONEY_ROUBLES
    ];

#pragma warning disable IDE0290 // 使用主构造函数
    public AddAmmoCase46x30HK (ISptLogger<AddAmmoCase46x30HK> logger, DatabaseService databaseService, CustomItemService customItemService) {
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
                {"en",new(){Name = "4.6x30HK ammo case",ShortName = "4.6x30HK",Description = "skydust™ 4.6x30HK ammo case"}},
                {"ch",new(){Name = "4.6x30HK 弹药箱",ShortName = "4.6x30HK",Description = "skydust™ 4.6x30HK 弹药箱"}}
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
                String.Concat(Constants.LoggerPrefix, "AddAmmoCase46x30HK.OnLoad() / failed / ", String.Join("；", createItemResult.Errors ?? Enumerable.Empty<String>())),
                LogTextColor.Yellow
            );
            return Task.CompletedTask;
        }

        Trader? trader = this.DatabaseService.GetTrader(Traders.REF);
        if (trader is null) {
            this.Logger.Log(
                LogLevel.Info,
                String.Concat(Constants.LoggerPrefix, "AddAmmoCase46x30HK.OnLoad() / failed / trader not found"),
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
            String.Concat(Constants.LoggerPrefix, "AddAmmoCase46x30HK.OnLoad() / success / ", this.BaseId, " / ", this.RotateId),
            LogTextColor.Green
        );
        return Task.CompletedTask;
    }
}
