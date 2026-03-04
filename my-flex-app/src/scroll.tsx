import { useRef, useState } from "react";
import gitubWhiteLogo from './assets/github-mark-white.svg'
import githubLogo from './assets/github-mark.svg'
import ContainerSetting from "./ContainerSetting"
import { type ContainerSettingContext } from './ContainerSetting'
import type { ItemSettingContext } from "./ItemSetting";
import useThemeMode from './useThemeMode'

export const ScrollT = () => {
    const [containerSetting, setContainerSetting] = useState<ContainerSettingContext>({ gap: '0', alignItems: 'items-start', justifyContent: 'justify-start', direction: 'flex-row', wrap:'flex-nowrap', isShowScrollbar: false });
    const [items, setItems] = useState<ItemSettingContext[]>([]);
    const [item, setItem] = useState<ItemSettingContext | null>(null);
    const id = useRef(0);
    const [themeMode, setThemeMode] = useThemeMode();
    const addItem = () => {
        const admin: ItemSettingContext = {
            id: id.current++,
            alignSelf: 'self-auto',
            grow: 0,
            shrink: 0,
            basis: "auto",
            minWidth: '0',
            maxWidth: 'none',
            width: 'auto',
            minHeight: '0',
            maxHeight: 'none',
            height: 'auto'
        };
        setItems(prevItems => [...prevItems, admin]);

    }

    const removeItem = (x: ItemSettingContext) => {
        if (item !== null && x === item) {
            setItem(null);
            //setSS("selectContainer")
        }
        setItems(prevItems => prevItems.filter(item => item !== x));
    }

    const editItem = (x: ItemSettingContext) => {
        setItem(x);
        //setSS('selectItem');
        //setItemOpen(true);
    }



    const pixelConvert = (data: string, dfv: string): string => {
        if (data === "") return dfv
        let num = Number(data);
        if (Number.isNaN(num)) return dfv
        return data + 'px';
    }
    return (
        <div className="grid grid-cols-[auto_1fr] grid-rows-[auto_1fr] w-screen h-screen bg-white text-neutral-700 dark:bg-gray-900 dark:text-white">
            <header className="col-span-2 text-white p-4 shadow-md z-10">
                <h1 className="text-lg font-bold">Desktop App Shell</h1>
            </header>
            <aside className="flex flex-col min-h-0 border-r border-neutral-400 dark:border-gray-700">
                <div className=" w-fit flex flex-col h-full">
                    <button className=' text-gray-900 bg-white border border-gray-300 focus:outline-none hover:bg-gray-100 font-medium rounded-sm text-sm px-5 py-2.5  dark:bg-gray-800 dark:text-white dark:border-gray-600 dark:hover:bg-gray-700 dark:hover:border-gray-600 dark:focus:ring-gray-700' onClick={() => addItem()}>add item</button>
                    <div id="setting" className="flex-1 min-h-0 overflow-y-auto">
                        <div className="">
                            <ContainerSetting className={''} containerSetting={containerSetting} setContainerSetting={setContainerSetting} />
                        </div>
                    </div>
                </div>
            </aside>
            <main className="overflow-auto min-h-0 min-w-0 bg-neutral-200 dark:bg-gray-950">
    
    {/* 2. Wrapper 負責撐開內容與 Padding */}
    <div className="min-w-full min-h-full p-8 flex flex-col">
        
        {/* 3. Box 負責渲染背景與排列內部 */}
        <div id="box"
            /* 關鍵修正：
               - 加入 flex-1：告訴瀏覽器「請把 Wrapper 剩下的垂直空間全部佔滿」
               - 拿掉 min-w-full 與 min-h-full：交給 flex-1 與外層的 flex-col 處理
            */
            className={`flex-1 flex ${containerSetting.direction} ${containerSetting.alignItems} ${containerSetting.justifyContent} dark:bg-gray-900 bg-neutral-200 rounded-sm`}
        >
                        {
                            items.map((x, i) => (
                                <div key={i} style={{ width: pixelConvert(x.width, 'auto'), minWidth: pixelConvert(x.minWidth, '0'), maxWidth: pixelConvert(x.maxWidth, 'none'), height: pixelConvert(x.height, 'auto'), flexGrow: `${x.grow}`, flexShrink: `${x.shrink}`, flexBasis: pixelConvert(x.basis, 'auto') }} className={`dark:bg-gray-800 overflow-hidden bg-white border-neutral-400 flex ${x.alignSelf} p-0.5 border rounded-sm dark:border-gray-600`} >
                                    <div className='self-center px-3 py-1 grow shrink-0'>index: {i}</div>
                                    <div className="border-r dark:border-gray-600 border-neutral-400"></div>
                                    <div onClick={() => editItem(x)} className={`flex items-center shrink-0 fill-gray-950 w-10 ${item === x ? "dark:bg-gray-600 bg-neutral-200" : ""}  dark:hover:bg-gray-600`}>
                                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path d="M20.71,7.04C21.1,6.65 21.1,6 20.71,5.63L18.37,3.29C18,2.9 17.35,2.9 16.96,3.29L15.12,5.12L18.87,8.87M3,17.25V21H6.75L17.81,9.93L14.06,6.18L3,17.25Z" /></svg>
                                    </div>
                                    <div className="border-r dark:border-gray-600 border-neutral-400"></div>
                                    <div onClick={() => removeItem(x)} className='flex items-center w-10 shrink-0 fill-gray-950 dark:hover:bg-gray-600'>
                                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path d="M4 19V7H16V19C16 20.1 15.1 21 14 21H6C4.9 21 4 20.1 4 19M6 9V19H14V9H6M13.5 4H17V6H3V4H6.5L7.5 3H12.5L13.5 4M19 17V15H21V17H19M19 13V7H21V13H19Z" /></svg>
                                    </div>
                                </div>
                            ))
                        }
                    </div>
                </div>
            </main>
        </div>
    )
}