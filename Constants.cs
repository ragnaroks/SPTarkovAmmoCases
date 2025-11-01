using System;

namespace SPTarkovAmmoCases;
public static class Constants {
    public static ModMetadata ModMetadata { get; } = new ModMetadata();

    public static String LoggerPrefix { get; } = String.Concat('[', ModMetadata.Name, '@', ModMetadata.Version, ']',' ');

    public static String HandbookIdForContainer { get; } = "5b5f6fa186f77409407a7eb7";

    public static Int32 GPCoinValue{get;} = 15000;
}
