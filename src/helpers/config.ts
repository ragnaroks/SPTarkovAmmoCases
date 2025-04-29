import configJson from '../../assets/config.json';
import {ItemTpl} from '@spt/models/enums/ItemTpl';
import includes from './includes';

const _tplArraya = [`${ItemTpl.MONEY_ROUBLES}`,`${ItemTpl.MONEY_DOLLARS}`,`${ItemTpl.MONEY_EUROS}`,`${ItemTpl.MONEY_GP_COIN}`] as const;


// from assembly-csharp => JsonType.TaxonomyColor
const backgroundArray = ['blue','yellow','green','red','black','grey','violet','orange','tracerYellow','tracerGreen','tracerRed','@default'] as const;

export type Config = {
  _tpl: typeof _tplArraya[number],
  count: number,
  background: typeof backgroundArray[number],
  size: number
};

export function getConfig(): Config {
  const config: Config = {_tpl: ItemTpl.MONEY_ROUBLES,count: 50_0000,background: 'red',size: 1};
  if(includes(_tplArraya,configJson._tpl)) {
    config._tpl = configJson._tpl;
  } else {
    this.logger.warn('[SPTarkovAmmoCases]：config.json field "_tpl" invalid');
  }
  if(configJson.count > 0) {
    config.count = configJson.count;
  } else {
    this.logger.warn('[SPTarkovAmmoCases]：config.json field "count" invalid');
  }
  if(includes(backgroundArray,configJson.background)) {
    config.background = configJson.background;
  } else {
    this.logger.warn('[SPTarkovAmmoCases]：config.json field "background" invalid');
  }
  if(configJson.size > 0 && config.size < 4) {
    config.size = configJson.size;
  } else {
    this.logger.warn('[SPTarkovAmmoCases]：config.json field "size" invalid');
  }
  return config;
};
