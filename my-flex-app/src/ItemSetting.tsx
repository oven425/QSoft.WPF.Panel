import React, { useCallback } from "react"
import { useRef } from "react"
import { alignSelfs } from './kV'

export type ItemSettingContext = {
    id: number
    alignSelf: string
    grow: number
    shrink: number
    basis: string
    minWidth:string
    maxWidth:string
    width:string
}

type ItemSettingProps = {
    itemSetting: ItemSettingContext | null;
    setItemSetting: React.Dispatch<React.SetStateAction<ItemSettingContext | null>>;
    className?: string | null
}

function ItemSetting(props: ItemSettingProps) {
    const basisunit = useRef('px');
    const handleValueChange = useCallback((name: keyof ItemSettingContext, value: string) => {
        props.setItemSetting(prev => {
            if (!prev) return null;
            return { ...prev, [name]: value };
        });
    }, [props.setItemSetting]);

    const onInputChange = useCallback((name: keyof ItemSettingContext, e: string) => {
        handleValueChange(name, e);
    }, [handleValueChange]);
    return (
        <div className={`${props.className} flex flex-col`}>
            <h5>Align-self</h5>
            <div className='grid grid-cols-2 gap-0.5 rounded-sm border border-neutral-500  dark:bg-gray-950 dark:border-gray-700 p-0.5 '>
                {
                    alignSelfs.map((x, i) => (
                        <div key={i} onClick={() => onInputChange('alignSelf', x.v)} className={`flex justify-center ${x.v === props.itemSetting?.alignSelf ? 'dark:bg-gray-800 bg-neutral-300' : ''} rounded-sm p-1`}>{x.k}</div>
                    ))
                }
            </div>
            <h5>Flex-grow</h5>
            <input value={props.itemSetting?.grow} onChange={x => onInputChange('grow', x.target.value)} type='number' min="0" />
            <h5>Flex-shrink</h5>
            <input value={props.itemSetting?.shrink} onChange={x => onInputChange('shrink', x.target.value)} type='number' />
            <h5>Flex-basis({basisunit.current})</h5>
            <input placeholder="auto" value={props.itemSetting?.basis} onChange={x => onInputChange('basis', x.target.value)} />
            <div>
                <h5>Min-width({basisunit.current})</h5>
                <input placeholder="0" value={props.itemSetting?.minWidth} onChange={x => onInputChange('minWidth', x.target.value)}/>
                <h5>Max-width</h5>
                <input placeholder="none" value={props.itemSetting?.maxWidth} onChange={x => onInputChange('maxWidth', x.target.value)}/>
                <h5>Width</h5>
                <input placeholder="auto" value={props.itemSetting?.width} onChange={x => onInputChange('width', x.target.value)}/>
            </div>
            {/* <div>
                <h5>Min-width({basisunit.current})</h5>
                <input placeholder="auto" value={props.itemSetting?.minWidth} onChange={x => onInputChange('minWidth', x.target.value)}/>
                <h5>Max-width</h5>
                <input placeholder="none" value={props.itemSetting?.maxWidth} onChange={x => onInputChange('maxWidth', x.target.value)}/>
                <h5>Width</h5>
                <input placeholder="auto" value={props.itemSetting?.width} onChange={x => onInputChange('width', x.target.value)}/>
            </div> */}

        </div>
    )
}
//export default ItemSetting;
export default React.memo(ItemSetting);