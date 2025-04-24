import {BaseClasses} from '@spt/models/enums/BaseClasses';
import {ItemTpl} from '@spt/models/enums/ItemTpl';
import {NewItemFromCloneDetails} from '@spt/models/spt/mod/NewItemDetails';
import {IDatabaseTables} from '@spt/models/spt/server/IDatabaseTables';
import {ILogger} from '@spt/models/spt/utils/ILogger';
import {CustomItemService} from '@spt/services/mod/CustomItemService';
import {Traders} from '@spt/models/enums/Traders';
import idcalc from '../helpers/idcalc';

const newId: string = '6808f75205109f6aadebd500';
const assortId1: string = idcalc(newId,0xff);
const assortId2: string = idcalc(newId,0xfe);

export default function addCustomAmmoCase_9x18(logger: ILogger,customItemService: CustomItemService,tables: IDatabaseTables) {
  const newItem: NewItemFromCloneDetails = {
    itemTplToClone: ItemTpl.CONTAINER_AMMUNITION_CASE,
    newId: newId,
    parentId: BaseClasses.SIMPLE_CONTAINER,
    fleaPriceRoubles: 62_5000,
    handbookPriceRoubles: 50_0000,
    handbookParentId: '5b5f6fa186f77409407a7eb7',
    locales: {
      en: {
        name: 'custom 9x18 ammo case',
        shortName: '9x18',
        description: 'custom 9x18 ammo case'
      },
      ch: {
        name: '客制 9x18 弹药箱',
        shortName: '9x18',
        description: '客制 9x18 弹药箱'
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
          _name: 'colume1',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 1,
            cellsV: 14,
            filters: [
              {
                Filter: [ItemTpl.AMMO_9X18PM_PBM],
                ExcludedFilter: []
              }
            ],
            isSortingTable: false,
            maxCount: 0,
            maxWeight: 0,
            minCount: 0
          }
        },{
          _id: idcalc(newId,0x02),
          _name: 'colume2',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 1,
            cellsV: 14,
            filters: [
              {
                Filter: [ItemTpl.AMMO_9X18PM_PSTM],
                ExcludedFilter: []
              }
            ],
            isSortingTable: false,
            maxCount: 0,
            maxWeight: 0,
            minCount: 0
          }
        },{
          _id: idcalc(newId,0x03),
          _name: 'colume3',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 1,
            cellsV: 14,
            filters: [
              {
                Filter: [ItemTpl.AMMO_9X18PM_BZHT],
                ExcludedFilter: []
              }
            ],
            isSortingTable: false,
            maxCount: 0,
            maxWeight: 0,
            minCount: 0
          }
        },{
          _id: idcalc(newId,0x04),
          _name: 'colume4',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 1,
            cellsV: 14,
            filters: [
              {
                Filter: [ItemTpl.AMMO_9X18PM_RG028],
                ExcludedFilter: []
              }
            ],
            isSortingTable: false,
            maxCount: 0,
            maxWeight: 0,
            minCount: 0
          }
        },{
          _id: idcalc(newId,0x05),
          _name: 'colume5',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 1,
            cellsV: 14,
            filters: [
              {
                Filter: [ItemTpl.AMMO_9X18PM_PST],
                ExcludedFilter: []
              }
            ],
            isSortingTable: false,
            maxCount: 0,
            maxWeight: 0,
            minCount: 0
          }
        },{
          _id: idcalc(newId,0x06),
          _name: 'colume6',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 1,
            cellsV: 14,
            filters: [
              {
                Filter: [ItemTpl.AMMO_9X18PM_PPT],
                ExcludedFilter: []
              }
            ],
            isSortingTable: false,
            maxCount: 0,
            maxWeight: 0,
            minCount: 0
          }
        },{
          _id: idcalc(newId,0x07),
          _name: 'colume7',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 1,
            cellsV: 14,
            filters: [
              {
                Filter: [ItemTpl.AMMO_9X18PM_PPE],
                ExcludedFilter: []
              }
            ],
            isSortingTable: false,
            maxCount: 0,
            maxWeight: 0,
            minCount: 0
          }
        },{
          _id: idcalc(newId,0x08),
          _name: 'colume8',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 1,
            cellsV: 14,
            filters: [
              {
                Filter: [ItemTpl.AMMO_9X18PM_PRS],
                ExcludedFilter: []
              }
            ],
            isSortingTable: false,
            maxCount: 0,
            maxWeight: 0,
            minCount: 0
          }
        },{
          _id: idcalc(newId,0x09),
          _name: 'colume9',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 1,
            cellsV: 14,
            filters: [
              {
                Filter: [ItemTpl.AMMO_9X18PM_PS_PPO],
                ExcludedFilter: []
              }
            ],
            isSortingTable: false,
            maxCount: 0,
            maxWeight: 0,
            minCount: 0
          }
        },{
          _id: idcalc(newId,0x0a),
          _name: 'colume10',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 1,
            cellsV: 14,
            filters: [
              {
                Filter: [ItemTpl.AMMO_9X18PM_P],
                ExcludedFilter: []
              }
            ],
            isSortingTable: false,
            maxCount: 0,
            maxWeight: 0,
            minCount: 0
          }
        },{
          _id: idcalc(newId,0x0b),
          _name: 'colume11',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 1,
            cellsV: 14,
            filters: [
              {
                Filter: [ItemTpl.AMMO_9X18PM_PSO],
                ExcludedFilter: []
              }
            ],
            isSortingTable: false,
            maxCount: 0,
            maxWeight: 0,
            minCount: 0
          }
        },{
          _id: idcalc(newId,0x0c),
          _name: 'colume12',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 1,
            cellsV: 14,
            filters: [
              {
                Filter: [ItemTpl.AMMO_9X18PM_PSV],
                ExcludedFilter: []
              }
            ],
            isSortingTable: false,
            maxCount: 0,
            maxWeight: 0,
            minCount: 0
          }
        },{
          _id: idcalc(newId,0x0d),
          _name: 'colume13',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 1,
            cellsV: 14,
            filters: [
              {
                Filter: [ItemTpl.AMMO_9X18PM_SP7],
                ExcludedFilter: []
              }
            ],
            isSortingTable: false,
            maxCount: 0,
            maxWeight: 0,
            minCount: 0
          }
        },{
          _id: idcalc(newId,0x0e),
          _name: 'colume14',
          _parent: newId,
          _proto: '55d329c24bdc2d892f8b4567',
          _props: {
            cellsH: 1,
            cellsV: 14,
            filters: [
              {
                Filter: [ItemTpl.AMMO_9X18PM_SP8],
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
    logger.error('[SPTarkovAmmoCases]：addCustomAmmoCase_9x18，Error：' + createResult.errors.join('、'));
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

  logger.success('[SPTarkovAmmoCases]：addCustomAmmoCase_9x18，ID：' + createResult.itemId);
}
