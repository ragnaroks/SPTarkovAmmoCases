export default function colorByPenetration(penetration:number) : string {
  if(isNaN(penetration)){return 'black';}
  // 61 ~ ∞
  if(penetration>60){return 'red';}
  // 51 ~ 60
  if(penetration>50){return 'orange';}
  // 41 ~ 50
  if(penetration>40){return 'yellow';}
  // 31 ~ 40
  if(penetration>30){return 'violet';}
  // 21 ~ 30
  if(penetration>20){return 'blue';}
  // 11 ~ 20
  if(penetration>10){return 'green';}
  // 01 ~ 10
  if(penetration>0){return 'grey';}
  // 0、others
  return 'black';
};
