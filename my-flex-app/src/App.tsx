import { useRef, useState, useEffect } from 'react'
import './App.css'
import gitubWhiteLogo from './assets/github-mark-white.svg'
import githubLogo from './assets/github-mark.svg'
import Modal from './Modal'
import { type ContainerSettingContext } from './ContainerSetting'
import ContainerSetting from './ContainerSetting'
import ItemSetting from './ItemSetting'
import type { ItemSettingContext } from './ItemSetting'
import useThemeMode from './useThemeMode'

function App() {
  const [containerSetting, setContainerSetting] = useState<ContainerSettingContext>({ gap: '0', alignItems: 'items-start', justifyContent: 'justify-start', direction: 'flex-row', isShowScrollbar: false });
  const [items, setItems] = useState<ItemSettingContext[]>([]);
  const [item, setItem] = useState<ItemSettingContext | null>(null);
  const id = useRef(0);
  const [containerOpen, setContainerOpen] = useState(false);
  const [itemOpen, setItemOpen] = useState(false);
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
      width: 'auto'
    };
    setItems(prevItems => [...prevItems, admin]);

  }
  const removeItem = (x: ItemSettingContext) => {
    if (item !== null && x === item) {
      setItem(null);
      setSS("selectContainer")
    }
    setItems(prevItems => prevItems.filter(item => item !== x));
  }

  useEffect(() => {
    if (!item) return;
    setItems(prevItems => {
      return prevItems.map(x => {
        if (x.id === item.id) {
          return item;
        }
        return x;
      });
    });
  }, [item]);

  type settingSelect = "selectContainer" | "selectItem";
  const [ss, setSS] = useState<settingSelect>("selectContainer");

  const editItem = (x: ItemSettingContext) => {
    setItem(x);
    setSS('selectItem');
    setItemOpen(true);
  }

  const closeItemSettingModal = () => {
    setItem(null);
    setSS('selectContainer');
    setItemOpen(false);
  }

  const pixelConvert = (data: string, dfv: string): string => {
    if (data === "") return dfv
    let num = Number(data);
    if (Number.isNaN(num)) return dfv
    return data + 'px';
  }

  return (
    <>
      <div className='select-none w-screen h-screen bg-white text-neutral-700 dark:bg-gray-900 dark:text-white flex flex-col'>
        <header className='flex border-b border-neutral-400 h-15 dark:border-gray-700 py-2 justify-between'>
          <h1 className='text-3xl font-semibold dark:text-white ml-4'>Flex test tool</h1>
          <div className='flex pr-4 gap-2'>
            <div className='hidden md:flex items-end border rounded-sm  border-neutral-500  dark:bg-gray-950 dark:border-gray-700 p-1'>
              <div className={`w-8 rounded-sm ${themeMode === 'system' ? 'dark:bg-gray-800 bg-neutral-300' : ''}`} onClick={() => setThemeMode('system')}>
                <svg viewBox="0 0 28 28" fill="none"><path d="M7.5 8.5C7.5 7.94772 7.94772 7.5 8.5 7.5H19.5C20.0523 7.5 20.5 7.94772 20.5 8.5V16.5C20.5 17.0523 20.0523 17.5 19.5 17.5H8.5C7.94772 17.5 7.5 17.0523 7.5 16.5V8.5Z" stroke="currentColor"></path><path d="M7.5 8.5C7.5 7.94772 7.94772 7.5 8.5 7.5H19.5C20.0523 7.5 20.5 7.94772 20.5 8.5V14.5C20.5 15.0523 20.0523 15.5 19.5 15.5H8.5C7.94772 15.5 7.5 15.0523 7.5 14.5V8.5Z" stroke="currentColor"></path><path d="M16.5 20.5V17.5H11.5V20.5M16.5 20.5H11.5M16.5 20.5H17.5M11.5 20.5H10.5" stroke="currentColor" stroke-linecap="round"></path></svg>
              </div>
              <div className={`w-8 rounded-sm ${themeMode === 'light' ? 'dark:bg-gray-800 bg-neutral-300' : ''}`} onClick={() => setThemeMode('light')}>
                <svg viewBox="0 0 28 28" fill="none"><circle cx="14" cy="14" r="3.5" stroke="currentColor"></circle><path d="M14 8.5V6.5" stroke="currentColor" stroke-linecap="round"></path><path d="M17.889 10.1115L19.3032 8.69727" stroke="currentColor" stroke-linecap="round"></path><path d="M19.5 14L21.5 14" stroke="currentColor" stroke-linecap="round"></path><path d="M17.889 17.8885L19.3032 19.3027" stroke="currentColor" stroke-linecap="round"></path><path d="M14 21.5V19.5" stroke="currentColor" stroke-linecap="round"></path><path d="M8.69663 19.3029L10.1108 17.8887" stroke="currentColor" stroke-linecap="round"></path><path d="M6.5 14L8.5 14" stroke="currentColor" stroke-linecap="round"></path><path d="M8.69663 8.69711L10.1108 10.1113" stroke="currentColor" stroke-linecap="round"></path></svg>
              </div>
              <div className={`w-8 rounded-sm ${themeMode === 'dark' ? 'bg-gray-800' : ''}`} onClick={() => setThemeMode('dark')}>
                <svg viewBox="0 0 28 28" fill="none"><path d="M10.5 9.99914C10.5 14.1413 13.8579 17.4991 18 17.4991C19.0332 17.4991 20.0176 17.2902 20.9132 16.9123C19.7761 19.6075 17.109 21.4991 14 21.4991C9.85786 21.4991 6.5 18.1413 6.5 13.9991C6.5 10.8902 8.39167 8.22304 11.0868 7.08594C10.7089 7.98159 10.5 8.96597 10.5 9.99914Z" stroke="currentColor" stroke-linejoin="round"></path><path d="M16.3561 6.50754L16.5 5.5L16.6439 6.50754C16.7068 6.94752 17.0525 7.29321 17.4925 7.35607L18.5 7.5L17.4925 7.64393C17.0525 7.70679 16.7068 8.05248 16.6439 8.49246L16.5 9.5L16.3561 8.49246C16.2932 8.05248 15.9475 7.70679 15.5075 7.64393L14.5 7.5L15.5075 7.35607C15.9475 7.29321 16.2932 6.94752 16.3561 6.50754Z" fill="currentColor" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round"></path><path d="M20.3561 11.5075L20.5 10.5L20.6439 11.5075C20.7068 11.9475 21.0525 12.2932 21.4925 12.3561L22.5 12.5L21.4925 12.6439C21.0525 12.7068 20.7068 13.0525 20.6439 13.4925L20.5 14.5L20.3561 13.4925C20.2932 13.0525 19.9475 12.7068 19.5075 12.6439L18.5 12.5L19.5075 12.3561C19.9475 12.2932 20.2932 11.9475 20.3561 11.5075Z" fill="currentColor" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round"></path></svg>
              </div>
            </div>
            <a className='hidden md:flex items-center w-8' href='https://github.com/oven425/QSoft.WPF.Panel/tree/master/my-flex-app' target="_blank">
              <img src={gitubWhiteLogo} className='hidden dark:block' alt="GitHub logo" />
              <img src={githubLogo} className='block dark:hidden' alt="GitHub logo" />
            </a>
            <button className='md:hidden text-gray-900 bg-white border border-gray-300 focus:outline-none hover:bg-gray-100 font-medium rounded-sm text-sm px-5 py-2.5  dark:bg-gray-800 dark:text-white dark:border-gray-600 dark:hover:bg-gray-700 dark:hover:border-gray-600 dark:focus:ring-gray-700' onClick={() => addItem()}>add item</button>
            <div onClick={() => setContainerOpen(true)} className='stroke-gray-300 flex md:hidden items-center w-8'>
              <svg fill="none" viewBox="0 0 24 24" stroke-width="1" stroke="currentColor" className="size-6">
                <path stroke-linecap="round" stroke-linejoin="round" d="M9.594 3.94c.09-.542.56-.94 1.11-.94h2.593c.55 0 1.02.398 1.11.94l.213 1.281c.063.374.313.686.645.87.074.04.147.083.22.127.325.196.72.257 1.075.124l1.217-.456a1.125 1.125 0 0 1 1.37.49l1.296 2.247a1.125 1.125 0 0 1-.26 1.431l-1.003.827c-.293.241-.438.613-.43.992a7.723 7.723 0 0 1 0 .255c-.008.378.137.75.43.991l1.004.827c.424.35.534.955.26 1.43l-1.298 2.247a1.125 1.125 0 0 1-1.369.491l-1.217-.456c-.355-.133-.75-.072-1.076.124a6.47 6.47 0 0 1-.22.128c-.331.183-.581.495-.644.869l-.213 1.281c-.09.543-.56.94-1.11.94h-2.594c-.55 0-1.019-.398-1.11-.94l-.213-1.281c-.062-.374-.312-.686-.644-.87a6.52 6.52 0 0 1-.22-.127c-.325-.196-.72-.257-1.076-.124l-1.217.456a1.125 1.125 0 0 1-1.369-.49l-1.297-2.247a1.125 1.125 0 0 1 .26-1.431l1.004-.827c.292-.24.437-.613.43-.991a6.932 6.932 0 0 1 0-.255c.007-.38-.138-.751-.43-.992l-1.004-.827a1.125 1.125 0 0 1-.26-1.43l1.297-2.247a1.125 1.125 0 0 1 1.37-.491l1.216.456c.356.133.751.072 1.076-.124.072-.044.146-.086.22-.128.332-.183.582-.495.644-.869l.214-1.28Z" />
                <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z" />
              </svg>
            </div>
          </div>
        </header>
        <div className='flex flex-row grow'>
          <aside className='hidden md:flex shrink-0 gap-1 w-66 pt-1 pl-1 pr-2 justify-start flex-col border-r border-neutral-400 dark:border-gray-700'>
            <button className=' text-gray-900 bg-white border border-gray-300 focus:outline-none hover:bg-gray-100 font-medium rounded-sm text-sm px-5 py-2.5  dark:bg-gray-800 dark:text-white dark:border-gray-600 dark:hover:bg-gray-700 dark:hover:border-gray-600 dark:focus:ring-gray-700' onClick={() => addItem()}>add item</button>
            <div className='grid grid-cols-2 '>
              <div className={`${ss === 'selectContainer' ? 'dark:bg-gray-800 bg-neutral-300' : ''} rounded-sm p-1`} onClick={() => setSS("selectContainer")}>Container</div>
              <div className={`${item === null ? "invisible" : "visible"} ${ss === 'selectItem' ? 'dark:bg-gray-800 bg-neutral-300' : ''} rounded-sm p-1`} onClick={() => setSS("selectItem")}>Item</div>
              <div className='col-span-2'>
                <ContainerSetting className={`${ss === 'selectContainer' ? 'block' : 'hidden'}`} setContainerSetting={setContainerSetting} containerSetting={containerSetting} />
                <div>
                  <ItemSetting className={`${item === null || ss === 'selectContainer' ? "hidden" : "block"}`} itemSetting={item!} setItemSetting={setItem} />
                </div>
              </div>
            </div>
          </aside>
          <div className={`p-1 flex ${containerSetting.direction == 'flex-row' ? 'flex-row' : 'flex-col'} ${containerSetting.isShowScrollbar === true ? 'overflow-auto' : ''}  h-[calc(100vh-3.75rem)] grow`}>
            <div style={{ gap: `${containerSetting.gap}px` }} className={`flex ${containerSetting.direction} ${containerSetting.alignItems} ${containerSetting.justifyContent} grow dark:bg-gray-950 bg-neutral-200 rounded-sm`}>
              {
                items.map((x, i) => (
                  <div key={i} style={{ width: `${pixelConvert(x.width, 'auto')}`, minWidth: `${pixelConvert(x.minWidth, '0')}`, maxWidth: `${pixelConvert(x.maxWidth, 'none')}`, flexGrow: `${x.grow}`, flexShrink: `${x.shrink}`, flexBasis: `${pixelConvert(x.basis, 'auto')}` }} className={`dark:bg-gray-800 overflow-hidden bg-white border-neutral-400 flex ${x.alignSelf} p-0.5 border rounded-sm dark:border-gray-600`} >
                    <div className='self-center px-3 py-1 grow shrink-0'>index:{i}</div>
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

              {/* <div className={`dark:bg-gray-800 basis-30 min-w-0 overflow-hidden bg-white border-neutral-400 flex  p-0.5 border rounded-sm dark:border-gray-600`} >
                <div className='self-center px-3 py-1 grow shrink-0'>index:0</div>
                <div className="border-r dark:border-gray-600 border-neutral-400 shrink-0"></div>
                <div className={`flex items-center fill-gray-950 w-10 shrink-0   dark:hover:bg-gray-600`}>
                  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path d="M20.71,7.04C21.1,6.65 21.1,6 20.71,5.63L18.37,3.29C18,2.9 17.35,2.9 16.96,3.29L15.12,5.12L18.87,8.87M3,17.25V21H6.75L17.81,9.93L14.06,6.18L3,17.25Z" /></svg>
                </div>
                <div className="border-r dark:border-gray-600 border-neutral-400" ></div>
                <div className='flex items-center w-10 fill-gray-950 shrink-0 dark:hover:bg-gray-600'>
                  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path d="M4 19V7H16V19C16 20.1 15.1 21 14 21H6C4.9 21 4 20.1 4 19M6 9V19H14V9H6M13.5 4H17V6H3V4H6.5L7.5 3H12.5L13.5 4M19 17V15H21V17H19M19 13V7H21V13H19Z" /></svg>
                </div>
              </div> */}

            </div>
          </div>
        </div>

        <Modal className={`md:hidden`} open={containerOpen} onClose={() => setContainerOpen(false)}>
          <ContainerSetting className={``} setContainerSetting={setContainerSetting} containerSetting={containerSetting} />
        </Modal>
        <Modal className={`md:hidden`} open={itemOpen} onClose={() => closeItemSettingModal()}>
          <ItemSetting className={``} itemSetting={item!} setItemSetting={setItem} />
        </Modal>
      </div>
    </>
  )
}

export default App
