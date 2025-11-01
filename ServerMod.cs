using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services.Mod;
using System;
using System.Threading.Tasks;

namespace SPTarkovAmmoCases;

[Injectable(InjectionType.Scoped, null, OnLoadOrder.PostDBModLoader + 1)]
public class ServerMod : IOnLoad {
    private ISptLogger<ServerMod> Logger{get;}
    private CustomItemService CustomItemService{get;}

    public ServerMod (ISptLogger<ServerMod> logger,CustomItemService customItemService) {
        this.Logger = logger;
        this.CustomItemService = customItemService;
    }

    public Task OnLoad () {
        throw new NotImplementedException();
    }
}
