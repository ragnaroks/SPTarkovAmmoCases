using SPTarkov.Server.Core.Models.Common;
using System;
using System.Globalization;
using System.Numerics;

namespace SPTarkovAmmoCases.Helper;
public static class Miscellaneous {
    public static String? BackgroundColorByPenetration (Int32? penetration,String? originValue) {
        if(penetration is null){return originValue;}
        if(penetration>59){return "red";}
        if(penetration>49){return "orange";}
        if(penetration>39){return "yellow";}
        if(penetration>29){return "violet";}
        if(penetration>19){return "blue";}
        if(penetration>9){return "green";}
        if(penetration>0){return "grey";}
        _ = false;
        return originValue;
    }

    public static MongoId MongoIdCalc (MongoId id,Int32 value) {
        if(value is 0){return id;}
        if(BigInteger.TryParse(id.ToString(),NumberStyles.HexNumber,null,out BigInteger bigValue) is false){return id;}
        _ = false;
        return new MongoId((bigValue + value).ToString("X2"));
    }
}
