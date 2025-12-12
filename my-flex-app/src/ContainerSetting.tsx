import React from 'react';
import { justifyContents, directions, alignItems_ } from './kV'
// type Kv<T1, T2>={
//     k: T1
//     v: T2
//   }

// const justifyContents:Kv<string, string>[] = [
//   {k:'Start', v:'justify-start'},
//   {k:'Center', v:'justify-center'},
//   {k:'End', v:'justify-end'},
//   {k:'Between', v:'justify-between'},
//   {k:'Around', v:'justify-around'},
//   {k:'Evenly', v:'justify-evenly'},
// ]

// const directions:Kv<string, string>[] = [
//   {k:'Row', v:'flex-row'},
//   {k:'Column', v:'flex-col'},
// ]

// const alignItems_:Kv<string, string>[] = [
//   {k:'Start', v:'items-start'},
//   {k:'Center', v:'items-center'},
//   {k:'End', v:'items-end'},
//   {k:'Stretch', v:'items-stretch'},
// ]

export type ContainerSettingContext = {
  direction: string;
  justifyContent: string;
  alignItems: string;
  gap: string;
  isShowScrollbar: boolean;
}
type ContainerSettingProps = {
  containerSetting: ContainerSettingContext;
  setContainerSetting: React.Dispatch<React.SetStateAction<ContainerSettingContext>>;
  className: string | null;
}
function ContainerSetting({ containerSetting, setContainerSetting, className }: ContainerSettingProps) {
  const handleContainerChange = (name: string, value: string | boolean) => {
    setContainerSetting(x => ({
      ...x,
      [name]: value
    }));
  }
  return (
    <div className={`${className} flex flex-col`}>
      <h5>Direction</h5>
      <div className='grid grid-cols-2 gap-0.5 rounded-sm border border-neutral-500  dark:bg-gray-950 dark:border-gray-700 p-0.5 '>
        {
          directions.map((x, i) => (
            <div key={i} onClick={() => handleContainerChange('direction', x.v)} className={`flex justify-center ${x.v === containerSetting.direction ? 'dark:bg-gray-800 bg-neutral-300' : ''} rounded-sm p-1`}>{x.k}</div>
          ))
        }
      </div>
      <h5>Justify-content</h5>
      <div className='grid grid-cols-2 gap-0.5 rounded-sm border border-neutral-500  dark:bg-gray-950 dark:border-gray-700 p-0.5 '>
        {
          justifyContents.map((x, i) => (
            <div key={i} onClick={() => handleContainerChange('justifyContent', x.v)} className={`flex justify-center ${x.v === containerSetting.justifyContent ? 'dark:bg-gray-800 bg-neutral-300' : ''} rounded-sm p-1`}>{x.k}</div>
          ))
        }
      </div>
      <h5>Align-items</h5>
      <div className='grid grid-cols-2 gap-0.5 rounded-sm border border-neutral-500  dark:bg-gray-950 dark:border-gray-700 p-0.5 '>
        {
          alignItems_.map((x, i) => (
            <div key={i} onClick={() => handleContainerChange('alignItems', x.v)} className={`flex justify-center ${x.v === containerSetting.alignItems ? 'dark:bg-gray-800 bg-neutral-300' : ''} rounded-sm p-1`}>{x.k}</div>
          ))
        }
      </div>
      <h5>Gap(px)</h5>
      <input value={containerSetting.gap} onChange={x => handleContainerChange('gap', x.target.value)} type='number' min="0" />
      <label className="flex items-center gap-1 ">
        <input type="checkbox" onChange={e => handleContainerChange('isShowScrollbar', e.target.checked)} checked={containerSetting.isShowScrollbar}></input>
        Scroll bar
      </label>
    </div>
  )
}

//export default ContainerSetting;
export default React.memo(ContainerSetting);