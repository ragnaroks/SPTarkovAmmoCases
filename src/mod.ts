import {DependencyContainer} from 'tsyringe';
import {IPreSptLoadMod} from '@spt/models/external/IPreSptLoadMod';
import {IPostDBLoadMod} from '@spt/models/external/IPostDBLoadMod';
import {IPostSptLoadMod} from '@spt/models/external/IPostSptLoadMod';
import {ILogger} from "@spt/models/spt/utils/ILogger";
import {DatabaseServer} from '@spt/servers/DatabaseServer';
import {IDatabaseTables} from '@spt/models/spt/server/IDatabaseTables';
import {ItemHelper} from '@spt/helpers/ItemHelper';
import {CustomItemService} from '@spt/services/mod/CustomItemService';
import addCustomAmmoCase_9x18 from './modifies/addCustomAmmoCase_9x18';
import addCustomAmmoCase_9x19 from './modifies/addCustomAmmoCase_9x19';
import addCustomAmmoCase_9x21 from './modifies/addCustomAmmoCase_9x21';
import addCustomAmmoCase_762x25 from './modifies/addCustomAmmoCase_762x25';
import addCustomAmmoCase_9x33r from './modifies/addCustomAmmoCase_9x33r';
import addCustomAmmoCase_1143x23 from './modifies/addCustomAmmoCase_1143x23';
import addCustomAmmoCase_127x33 from './modifies/addCustomAmmoCase_127x33';
import addCustomAmmoCase_46x30 from './modifies/addCustomAmmoCase_46x30';
import addCustomAmmoCase_57x28 from './modifies/addCustomAmmoCase_57x28';
import addCustomAmmoCase_545x39 from './modifies/addCustomAmmoCase_545x39';
import addCustomAmmoCase_556x45 from './modifies/addCustomAmmoCase_556x45';
import addCustomAmmoCase_68x51 from './modifies/addCustomAmmoCase_68x51';
import addCustomAmmoCase_762x35 from './modifies/addCustomAmmoCase_762x35';
import addCustomAmmoCase_762x39 from './modifies/addCustomAmmoCase_762x39';
import addCustomAmmoCase_762x51 from './modifies/addCustomAmmoCase_762x51';
import addCustomAmmoCase_762x54 from './modifies/addCustomAmmoCase_762x54';
import addCustomAmmoCase_86x70 from './modifies/addCustomAmmoCase_86x70';
import addCustomAmmoCase_9x39 from './modifies/addCustomAmmoCase_9x39';
import addCustomAmmoCase_955x39 from './modifies/addCustomAmmoCase_955x39';
import addCustomAmmoCase_127x55 from './modifies/addCustomAmmoCase_127x55';
import addCustomAmmoCase_12gb from './modifies/addCustomAmmoCase_12gb';
import addCustomAmmoCase_12gs from './modifies/addCustomAmmoCase_12gs';
import addCustomAmmoCase_20gb from './modifies/addCustomAmmoCase_20gb';
import addCustomAmmoCase_20gs from './modifies/addCustomAmmoCase_20gs';
import addCustomAmmoCase_4g from './modifies/addCustomAmmoCase_4g';

// example：https://dev.sp-tarkov.com/chomp/ModExamples/

class Mod implements IPreSptLoadMod,IPostDBLoadMod,IPostSptLoadMod {
  private logger: ILogger;
  private databaseServer: DatabaseServer;
  private itemHelper: ItemHelper;
  private customItemService: CustomItemService;

  public preSptLoad(container: DependencyContainer): void {
    this.logger = container.resolve<ILogger>('WinstonLogger');
    this.databaseServer = container.resolve<DatabaseServer>('DatabaseServer');
    this.itemHelper = container.resolve<ItemHelper>('ItemHelper');
    this.customItemService = container.resolve<CustomItemService>('CustomItemService');
  }

  public postDBLoad(container: DependencyContainer): void {
    const tables: IDatabaseTables = this.databaseServer.getTables();
    
    // pistol

    addCustomAmmoCase_762x25(this.logger,this.customItemService,tables);

    addCustomAmmoCase_9x18(this.logger,this.customItemService,tables);
    
    addCustomAmmoCase_9x19(this.logger,this.customItemService,tables);

    addCustomAmmoCase_9x21(this.logger,this.customItemService,tables);

    addCustomAmmoCase_9x33r(this.logger,this.customItemService,tables);

    addCustomAmmoCase_1143x23(this.logger,this.customItemService,tables);

    addCustomAmmoCase_127x33(this.logger,this.customItemService,tables);

    // PDW

    addCustomAmmoCase_46x30(this.logger,this.customItemService,tables);

    addCustomAmmoCase_57x28(this.logger,this.customItemService,tables);

    // rifle

    addCustomAmmoCase_545x39(this.logger,this.customItemService,tables);

    addCustomAmmoCase_556x45(this.logger,this.customItemService,tables);

    addCustomAmmoCase_68x51(this.logger,this.customItemService,tables);

    addCustomAmmoCase_762x35(this.logger,this.customItemService,tables);

    addCustomAmmoCase_762x39(this.logger,this.customItemService,tables);

    addCustomAmmoCase_762x51(this.logger,this.customItemService,tables);

    addCustomAmmoCase_762x54(this.logger,this.customItemService,tables);

    addCustomAmmoCase_86x70(this.logger,this.customItemService,tables);

    addCustomAmmoCase_9x39(this.logger,this.customItemService,tables);

    addCustomAmmoCase_955x39(this.logger,this.customItemService,tables);

    addCustomAmmoCase_127x55(this.logger,this.customItemService,tables);

    // shotgun

    addCustomAmmoCase_12gb(this.logger,this.customItemService,tables);

    addCustomAmmoCase_12gs(this.logger,this.customItemService,tables);

    addCustomAmmoCase_20gb(this.logger,this.customItemService,tables);

    addCustomAmmoCase_20gs(this.logger,this.customItemService,tables);

    addCustomAmmoCase_4g(this.logger,this.customItemService,tables);

    //
    this.logger.success('[SPTarkovAmmoCases]: done');
  }

  public postSptLoad(container: DependencyContainer): void {
    //
  }
}

export const mod = new Mod();
