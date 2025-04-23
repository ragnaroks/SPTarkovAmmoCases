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
    
    addCustomAmmoCase_9x18(this.logger,this.customItemService,tables);
    
    addCustomAmmoCase_9x19(this.logger,this.customItemService,tables);

    addCustomAmmoCase_9x21(this.logger,this.customItemService,tables);

    //
    this.logger.success('[SPTarkovAmmoCases]: done');
  }

  public postSptLoad(container: DependencyContainer): void {
    //
  }
}

export const mod = new Mod();
