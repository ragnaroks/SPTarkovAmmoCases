import {BaseClasses} from '@spt/models/enums/BaseClasses';
import {ItemTpl} from '@spt/models/enums/ItemTpl';
import {NewItemFromCloneDetails} from '@spt/models/spt/mod/NewItemDetails';
import {IDatabaseTables} from '@spt/models/spt/server/IDatabaseTables';
import {ILogger} from '@spt/models/spt/utils/ILogger';
import {CustomItemService} from '@spt/services/mod/CustomItemService';
import {Traders} from '@spt/models/enums/Traders';
import idcalc from '../helpers/idcalc';
import {ItemHelper} from '@spt/helpers/ItemHelper';

const newId: string = '68108b05b9a977d3588d1d00';
const assortId1: string = idcalc(newId,0xff);
const assortId2: string = idcalc(newId,0xfe);

export default function addCustomAmmoCase_PL2(logger: ILogger,customItemService: CustomItemService,itemHelper: ItemHelper,tables: IDatabaseTables) {
  const idArray = itemHelper
    .getItemTplsOfBaseType(BaseClasses.AMMO)
    .filter(function(value) {
      const template = tables.templates.items[value] || null;
      if(!template) {return false;}
      if(Array.isArray(template._props.FlareTypes) && template._props.FlareTypes.length > 0) {return false;}
      if(!template._props.InitialSpeed || template._props.InitialSpeed < 100) {return false;}
      if(!template._props.PenetrationPower || template._props.PenetrationPower < 21) {return false;}
      if(!template._props.PenetrationPower || template._props.PenetrationPower > 30) {return false;}
      return true;
    });

  const newItem: NewItemFromCloneDetails = {
    itemTplToClone: ItemTpl.CONTAINER_AMMUNITION_CASE,
    newId: newId,
    parentId: BaseClasses.SIMPLE_CONTAINER,
    fleaPriceRoubles: 62_5000,
    handbookPriceRoubles: 50_0000,
    handbookParentId: '5b5f6fa186f77409407a7eb7',
    locales: {
      en: {
        name: 'custom PL2 ammo case',
        shortName: 'PL2',
        description: 'custom PL2 ammo case, can store ammos those penetration power between 21 and 30'
      },
      ch: {
        name: '客制 PL2 弹药箱',
        shortName: 'PL2',
        description: '客制 PL2 弹药箱，可存放穿透力 21 至 30 的弹药'
      }
    },
    overrideProperties: {
      CanSellOnRagfair: false,
      BackgroundColor: 'red',
      Weight: 0,
      Width: 1,
      Height: 1,
      mousePenalty: 0,
      speedPenaltyPercent: 0,
      weaponErgonomicPenalty: 0,
      ExamineExperience: 50,
      LootExperience: 50,
      Grids: [
        {
          _id: idcalc(newId,0x01),
          _name: 'main',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 14,
            cellsV: 14,
            filters: [
              {
                Filter: idArray,
                ExcludedFilter: []
              }
            ],
            isSortingTable: false,
            maxCount: 0,
            maxWeight: 0,
            minCount: 0
          }
        }
      ]
    }
  };

  const createResult = customItemService.createItemFromClone(newItem);
  if(!createResult.success) {
    logger.error('[SPTarkovAmmoCases]：addCustomAmmoCase_PL2，Error：' + createResult.errors.join('、'));
    return;
  }

  const assort1 = tables.traders[Traders.REF].assort;
  assort1.items.push({
    _id: assortId1,
    _tpl: createResult.itemId,
    parentId: 'hideout',
    slotId: 'hideout',
    upd: {
      UnlimitedCount: true,
      StackObjectsCount: 9999999,
      BuyRestrictionMax: 1,
      BuyRestrictionCurrent: 0
    }
  });
  assort1.loyal_level_items[assortId1] = 1;
  assort1.barter_scheme[assortId1] = [
    [
      {_tpl: ItemTpl.MONEY_GP_COIN,count: Math.floor(newItem.handbookPriceRoubles / 7500)}
    ]
  ];

  const assort2 = tables.traders[Traders.MECHANIC].assort;
  assort2.items.push({
    _id: assortId2,
    _tpl: createResult.itemId,
    parentId: 'hideout',
    slotId: 'hideout',
    upd: {
      UnlimitedCount: true,
      StackObjectsCount: 9999999,
      BuyRestrictionMax: 3,
      BuyRestrictionCurrent: 0
    }
  });
  assort2.loyal_level_items[assortId2] = 2;
  assort2.barter_scheme[assortId2] = [
    [
      {_tpl: ItemTpl.MONEY_ROUBLES,count: newItem.handbookPriceRoubles}
    ]
  ];

  logger.success('[SPTarkovAmmoCases]：addCustomAmmoCase_PL2，ID：' + createResult.itemId);
}
