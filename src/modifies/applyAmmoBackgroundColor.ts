import {BaseClasses} from '@spt/models/enums/BaseClasses';
import {ItemTpl} from '@spt/models/enums/ItemTpl';
import {IDatabaseTables} from '@spt/models/spt/server/IDatabaseTables';
import {ILogger} from '@spt/models/spt/utils/ILogger';
import {ItemHelper} from '@spt/helpers/ItemHelper';
import colorByPenetration from '../helpers/colorByPenetration';
import path from 'node:path';
import fs from 'node:fs';

const redArray: Array<string> = [
  ItemTpl.AMMO_40MMRU_VOG25,
  ItemTpl.AMMO_40X46_M381,
  ItemTpl.AMMO_40X46_M386,
  ItemTpl.AMMO_40X46_M406,
  ItemTpl.AMMO_40X46_M433,
  ItemTpl.AMMO_40X46_M441,
  ItemTpl.AMMO_40X46_M576,
  ItemTpl.AMMO_40X46_M716
];

const excludeArray: Array<string> = [
  ...redArray,
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

export default function applyAmmoBackgroundColor(logger: ILogger,itemHelper: ItemHelper,tables: IDatabaseTables) {
  const dirs = fs
    .readdirSync(path.resolve(__dirname,'../../../'),{withFileTypes: false,recursive: false,encoding: 'utf-8'})
    .map(x => x.toLocaleLowerCase());
  if(dirs.includes('rairaitheraichu-ammostats')) {return;}
  if(dirs.includes('odt-iteminfo')) {return;}
  if(dirs.includes('zzacidphantasm-itemvaluation')) {return;}
  if(dirs.includes('refringe-easyammunition')) {return;}

  const idArray = itemHelper
    .getItemTplsOfBaseType(BaseClasses.AMMO)
    .filter(x => !excludeArray.includes(x))

  for(const id of idArray) {
    const template = tables.templates.items[id] || null;
    if(!template) {continue;}
    template._props.BackgroundColor = colorByPenetration(template._props.PenetrationPower)
  }

  for(const id of redArray) {
    const template = tables.templates.items[id] || null;
    if(!template) {continue;}
    template._props.BackgroundColor = 'red';
  }

  logger.success('[SPTarkovAmmoCases]：applyAmmoBackgroundColor');
}
