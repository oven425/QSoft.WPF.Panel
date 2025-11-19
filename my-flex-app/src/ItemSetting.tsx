import { useRef } from "react"

export type ItemSettingContext = {
    id: number
    alignSelf: string
    grow: number
    shrink: number
    basis: string
}

type ItemSettingProps = {
    itemSetting: ItemSettingContext|null;
    setItemSetting: React.Dispatch<React.SetStateAction<ItemSettingContext|null>>;
}

type Kv<T1, T2>={
    k: T1
    v: T2
  }

const alignSelfs:Kv<string, string>[] = [
  {k:'Auto', v:'self-auto'},
  {k:'Start', v:'self-start'},
  {k:'Center', v:'self-center'},
  {k:'End', v:'self-end'},
  {k:'Stretch', v:'self-stretch'},
]

function ItemSetting(props: ItemSettingProps) {
    const basisunit = useRef('px');
    const handleItemChange = (name: string, value: string) => {
        props.setItemSetting(x=>{
            if(x){
                return{
                    ...x,
                    [name]:value
                }
            }
            return null;
        });
    };
    return (
        <div className="flex flex-col">
            <h5>Align-self</h5>
            {/* <select value={props.itemSetting?.alignSelf} onChange={e => handleItemChange('alignSelf', e.target.value)}>
                <option value="self-auto">Auto</option>
                <option value="self-start">Start</option>
                <option value="self-center">Center</option>
                <option value="self-end">End</option>
                <option value="self-stretch">Stretch</option>
            </select> */}
            <div className='grid grid-cols-2 gap-0.5 rounded-sm border border-neutral-500  dark:bg-gray-950 dark:border-gray-700 p-0.5 '>
              {
                alignSelfs.map((x,i)=>(
                  <div key={i} onClick={()=>handleItemChange('alignSelf', x.v)} className={`flex justify-center ${x.v===props.itemSetting?.alignSelf?'dark:bg-gray-800 bg-neutral-300':''} rounded-sm p-1`}>{x.k}</div>
              ))
              }
            </div>
            <h5>Flex-grow</h5>
            <input value={props.itemSetting?.grow} onChange={x => handleItemChange('grow', x.target.value)} type='number' />
            <h5>Flex-shrink</h5>
            <input value={props.itemSetting?.shrink} onChange={x => handleItemChange('shrink', x.target.value)} type='number' />
            <h5>Flex-basis({basisunit.current})</h5>
            <input placeholder="auto" value={props.itemSetting?.basis} onChange={x => handleItemChange('basis', x.target.value)} />
        </div>
    )
}
export default ItemSetting;