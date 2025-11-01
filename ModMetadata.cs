using SPTarkov.Server.Core.Models.Spt.Mod;
using System;
using System.Collections.Generic;

namespace SPTarkovAmmoCases;
public record ModMetadata : AbstractModMetadata {
    public override String ModGuid {get;init;} = "";
    public override String Name {get;init;} = "";
    public override String Author {get;init;} = "";
    public override List<String>? Contributors {get;init;} = "";
    public override Version Version {get;init;} = "";
    public override Range SptVersion {get;init;} = "";
    public override List<System.String>? Incompatibilities {get;init;} = "";
    public override Dictionary<System.String, Range>? ModDependencies {get;init;} = "";
    public override System.String? Url {get;init;} = "";
    public override System.Boolean? IsBundleMod {get;init;} = "";
    public override System.String License {get;init;} = "";
}
