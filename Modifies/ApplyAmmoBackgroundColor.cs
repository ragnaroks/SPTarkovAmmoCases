using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Logging;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPTarkovAmmoCases.Modifies;

[Injectable(InjectionType.Scoped, null, Int32.MaxValue)]
public class ApplyAmmoBackgroundColor : IOnLoad {
    private ISptLogger<ApplyAmmoBackgroundColor> Logger { get; }
    private DatabaseService DatabaseService { get; }
    private ItemHelper ItemHelper { get; }

    private IList<MongoId> ForceRedList { get; } = [
        ItemTpl.AMMO_40MMRU_VOG25,
        ItemTpl.AMMO_40X46_M381,
        ItemTpl.AMMO_40X46_M386,
        ItemTpl.AMMO_40X46_M406,
        ItemTpl.AMMO_40X46_M433,
        ItemTpl.AMMO_40X46_M441,
        ItemTpl.AMMO_40X46_M576,
        ItemTpl.AMMO_40X46_M716
    ];

    private IList<MongoId> ExcludeList { get; } = [
        ItemTpl.AMMO_40MMRU_VOG25,
        ItemTpl.AMMO_40X46_M381,
        ItemTpl.AMMO_40X46_M386,
        ItemTpl.AMMO_40X46_M406,
        ItemTpl.AMMO_40X46_M433,
        ItemTpl.AMMO_40X46_M441,
        ItemTpl.AMMO_40X46_M576,
        ItemTpl.AMMO_40X46_M716,
        ItemTpl.AMMO_26X75_AG,
        ItemTpl.AMMO_26X75_FLARE,
        ItemTpl.AMMO_26X75_GREEN,
        ItemTpl.AMMO_26X75_RED,
        ItemTpl.AMMO_26X75_YELLOW,
        ItemTpl.AMMO_26X75_SIGNAL_FLARE_BLUE,
        ItemTpl.AMMO_26X75_SIGNAL_FLARE_GREEN,
        ItemTpl.AMMO_26X75_SIGNAL_FLARE_NEW_YEAR,
        ItemTpl.AMMO_26X75_SIGNAL_FLARE_RED,
        ItemTpl.AMMO_26X75_SIGNAL_FLARE_SPECIAL_YELLOW,
        ItemTpl.AMMO_26X75_SIGNAL_FLARE_WHITE,
        ItemTpl.AMMO_26X75_SIGNAL_FLARE_YELLOW,
        ItemTpl.AMMO_30X29_VOG30,
        ItemTpl.AMMO_20X1MM_DISK
    ];

#pragma warning disable IDE0290 // 使用主构造函数    
    public ApplyAmmoBackgroundColor (ISptLogger<ApplyAmmoBackgroundColor> logger, DatabaseService databaseService, ItemHelper itemHelper) {
        this.Logger = logger;
        this.DatabaseService = databaseService;
        this.ItemHelper = itemHelper;
    }
#pragma warning restore IDE0290 // 使用主构造函数

    public Task OnLoad () {
        Dictionary<MongoId, TemplateItem> templates = this.DatabaseService.GetItems();
        IEnumerable<MongoId> templates2 = this.ItemHelper.GetItemTplsOfBaseType(BaseClasses.AMMO).Where(x => !this.ExcludeList.Contains(x));

        foreach (MongoId id in templates2) {
            if (templates.TryGetValue(id, out TemplateItem? template) is false) { continue; }
            if (template is null) { continue; }
            if (template.Properties is null) { continue; }
            template.Properties.BackgroundColor = Helper.Miscellaneous.BackgroundColorByPenetration(template.Properties.PenetrationPower, template.Properties.BackgroundColor);
        }

        foreach (MongoId id in this.ForceRedList) {
            if (templates.TryGetValue(id, out TemplateItem? template) is false) { continue; }
            if (template is null) { continue; }
            if (template.Properties is null) { continue; }
            template.Properties.BackgroundColor = "red";
        }

        this.Logger.Log(
            LogLevel.Info,
            String.Concat(Constants.LoggerPrefix, "ApplyAmmoBackgroundColor.OnLoad() / success"),
            LogTextColor.Green
        );
        return Task.CompletedTask;
    }
}
