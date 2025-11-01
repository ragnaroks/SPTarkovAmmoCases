using SPTarkov.Server.Core.Models.Spt.Mod;
using System;
using System.Collections.Generic;

namespace SPTarkovAmmoCases;

public record ModMetadata : AbstractModMetadata {
    public override String ModGuid { get; init; } = "net.skydust.sptarkovammocases";
    public override String Name { get; init; } = "SPTarkovAmmoCases";
    public override String Author { get; init; } = "ragnaroks";
    public override List<String>? Contributors { get; init; } = null;
    public override SemanticVersioning.Version Version { get; init; } = new("4.0.20251101");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<String>? Incompatibilities { get; init; } = null;
    public override Dictionary<String, SemanticVersioning.Range>? ModDependencies { get; init; } = null;
    public override String? Url { get; init; } = "https://github.com/ragnaroks/SPTarkovAmmoCases";
    public override Boolean? IsBundleMod { get; init; } = false;
    public override String License { get; init; } = "AGPLv3";
}
