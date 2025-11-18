
import { useRef, useState, useEffect } from 'react'
import './App.css'
import deleteLogo from './assets/delete-alert-outline.svg'
import editLogo from './assets/pencil.svg'
import gitubWhiteLogo from './assets/github-mark-white.svg'
import githubLogo from './assets/github-mark.svg'
import Modal from './Modal'

type Kv<T1, T2>={
    k: T1
    v: T2
  }

const justifyContents:Kv<string, string>[] = [
  {k:'Start', v:'justify-start'},
  {k:'Center', v:'justify-center'},
  {k:'End', v:'justify-end'},
  {k:'Between', v:'justify-between'},
  {k:'Around', v:'justify-around'},
  {k:'Evenly', v:'justify-evenly'},
]

const directions:Kv<string, string>[] = [
  {k:'Row', v:'flex-row'},
  {k:'Column', v:'flex-col'},
]

const alignItems_:Kv<string, string>[] = [
  {k:'Start', v:'items-start'},
  {k:'Center', v:'items-center'},
  {k:'End', v:'items-end'},
  {k:'Stretch', v:'items-stretch'},
]


function App() {
  const [gap, setGap] = useState("0");
  const [direction, setDirection] = useState('flex-row');
  const [justifyContent, setJustifyContent] = useState('justify-start');
  const [alignItems, setAlignItems] = useState('items-start');
  const [items, setItems] = useState<Itemd[]>([]);
  const [item, setItem] = useState<Itemd | null>(null);
  const basisunit = useRef('px');
  const id = useRef(0);
  
  const [themeMode, setThemeMode] = useState<Theme>('system');
  const [isDarkMode, setIsDarkMode] = useState<boolean>(false);
  const [containerOpen, setContainerOpen] = useState(false);
  type Theme = "dark" | "light" | "system";
  useEffect(() => {
    if(themeMode === 'dark'){
      setIsDarkMode(true);
    }
    else if(themeMode === 'system'){
      let systemdark = window.matchMedia("(prefers-color-scheme: dark)").matches;
      setIsDarkMode(systemdark);
    }
    else{
      setIsDarkMode(false)
    }
  }, [themeMode]);

  useEffect(()=>{
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    const handleChange = (e: MediaQueryListEvent) => {
      if(themeMode === 'system'){
        const isDark = e.matches;
        setIsDarkMode(isDark);
      }
    };
    mediaQuery.addEventListener('change', handleChange);
    return () => {
      mediaQuery.removeEventListener('change', handleChange);
    };
  },[]);

  useEffect(()=>{
    document.documentElement.classList.toggle('dark', isDarkMode);
  },[isDarkMode])

  type FlexBasis = "auto" | `${number}px` | `${number}%`;
  //type FlexBasis = "auto" | `${number}`
  type Itemd = {
    id: number
    alignSelf: string
    grow: number
    shrink: number
    basis: string
  };

  


  const addItem = () => {
    const admin: Itemd = {
      id: id.current++,
      alignSelf: 'self-auto',
      grow: 0,
      shrink: 0,
      basis: "auto"
    };
    setItems(prevItems => [...prevItems, admin]);

  }
  const removeItem = (x: Itemd) => {
    if (item !== null && x === item) {
      setItem(null);
    }
    setItems(prevItems => prevItems.filter(item => item !== x));
  }

  const alignSelfChange = (newvalue: string) => {
    setItem(x => x ? {
      ...x
      , alignSelf: newvalue
    } : x);
  }

  const growChange = (newvalue: number) => {
    setItem(x => x ? {
      ...x
      , grow: newvalue
    } : x);
  }

  const shrinkChange = (newvalue: number) => {
    setItem(x => x ? {
      ...x
      , shrink: newvalue
    } : x);
  }

  const isFlexBasis = (value: string): value is FlexBasis => {
    return value === "auto" || /^\d+$/.test(value);
  }
  const basisChange = (newvalue: string) => {
    if (isFlexBasis(newvalue)) {

    }
    console.log(newvalue);
    setItem(x => x ? {
      ...x
      , basis: newvalue
    } : x);
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



  return (
    <>
      <div className='w-screen h-screen bg-white text-neutral-700 dark:bg-gray-900 dark:text-white flex flex-col'>
        <header className='flex border-b border-neutral-400 dark:border-gray-700 py-2 justify-between'>
          <h1 className='text-3xl font-semibold dark:text-white ml-4'>Flex test tool</h1>
          <div className='flex pr-4 gap-2'>
            <div className='hidden md:flex items-end border rounded-sm  border-neutral-500  dark:bg-gray-950 dark:border-gray-700 p-1'>
              <div className={`w-8 rounded-sm ${themeMode==='system'?'dark:bg-gray-800 bg-neutral-300':''}`} onClick={() => setThemeMode('system')}>
                <svg viewBox="0 0 28 28" fill="none"><path d="M7.5 8.5C7.5 7.94772 7.94772 7.5 8.5 7.5H19.5C20.0523 7.5 20.5 7.94772 20.5 8.5V16.5C20.5 17.0523 20.0523 17.5 19.5 17.5H8.5C7.94772 17.5 7.5 17.0523 7.5 16.5V8.5Z" stroke="currentColor"></path><path d="M7.5 8.5C7.5 7.94772 7.94772 7.5 8.5 7.5H19.5C20.0523 7.5 20.5 7.94772 20.5 8.5V14.5C20.5 15.0523 20.0523 15.5 19.5 15.5H8.5C7.94772 15.5 7.5 15.0523 7.5 14.5V8.5Z" stroke="currentColor"></path><path d="M16.5 20.5V17.5H11.5V20.5M16.5 20.5H11.5M16.5 20.5H17.5M11.5 20.5H10.5" stroke="currentColor" stroke-linecap="round"></path></svg>
              </div>
              <div className={`w-8 rounded-sm ${themeMode==='light'?'dark:bg-gray-800 bg-neutral-300':''}`} onClick={() => setThemeMode('light')}>
                <svg viewBox="0 0 28 28" fill="none"><circle cx="14" cy="14" r="3.5" stroke="currentColor"></circle><path d="M14 8.5V6.5" stroke="currentColor" stroke-linecap="round"></path><path d="M17.889 10.1115L19.3032 8.69727" stroke="currentColor" stroke-linecap="round"></path><path d="M19.5 14L21.5 14" stroke="currentColor" stroke-linecap="round"></path><path d="M17.889 17.8885L19.3032 19.3027" stroke="currentColor" stroke-linecap="round"></path><path d="M14 21.5V19.5" stroke="currentColor" stroke-linecap="round"></path><path d="M8.69663 19.3029L10.1108 17.8887" stroke="currentColor" stroke-linecap="round"></path><path d="M6.5 14L8.5 14" stroke="currentColor" stroke-linecap="round"></path><path d="M8.69663 8.69711L10.1108 10.1113" stroke="currentColor" stroke-linecap="round"></path></svg>
              </div>
              <div className={`w-8 rounded-sm ${themeMode==='dark'?'bg-gray-800':''}`} onClick={() => setThemeMode('dark')}>   
                <svg viewBox="0 0 28 28" fill="none"><path d="M10.5 9.99914C10.5 14.1413 13.8579 17.4991 18 17.4991C19.0332 17.4991 20.0176 17.2902 20.9132 16.9123C19.7761 19.6075 17.109 21.4991 14 21.4991C9.85786 21.4991 6.5 18.1413 6.5 13.9991C6.5 10.8902 8.39167 8.22304 11.0868 7.08594C10.7089 7.98159 10.5 8.96597 10.5 9.99914Z" stroke="currentColor" stroke-linejoin="round"></path><path d="M16.3561 6.50754L16.5 5.5L16.6439 6.50754C16.7068 6.94752 17.0525 7.29321 17.4925 7.35607L18.5 7.5L17.4925 7.64393C17.0525 7.70679 16.7068 8.05248 16.6439 8.49246L16.5 9.5L16.3561 8.49246C16.2932 8.05248 15.9475 7.70679 15.5075 7.64393L14.5 7.5L15.5075 7.35607C15.9475 7.29321 16.2932 6.94752 16.3561 6.50754Z" fill="currentColor" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round"></path><path d="M20.3561 11.5075L20.5 10.5L20.6439 11.5075C20.7068 11.9475 21.0525 12.2932 21.4925 12.3561L22.5 12.5L21.4925 12.6439C21.0525 12.7068 20.7068 13.0525 20.6439 13.4925L20.5 14.5L20.3561 13.4925C20.2932 13.0525 19.9475 12.7068 19.5075 12.6439L18.5 12.5L19.5075 12.3561C19.9475 12.2932 20.2932 11.9475 20.3561 11.5075Z" fill="currentColor" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round"></path></svg>
              </div>
            </div>
            <div className='hidden md:flex items-center w-8'>
              <img src={gitubWhiteLogo} className='hidden dark:block' alt="GitHub logo" />
              <img src={githubLogo} className='block dark:hidden' alt="GitHub logo" />
            </div>
            <div onClick={()=>setContainerOpen(true)} className='stroke-gray-300 flex md:hidden items-center w-8'>
              <svg fill="none" viewBox="0 0 24 24" stroke-width="1" stroke="currentColor" className="size-6">
                <path stroke-linecap="round" stroke-linejoin="round" d="M9.594 3.94c.09-.542.56-.94 1.11-.94h2.593c.55 0 1.02.398 1.11.94l.213 1.281c.063.374.313.686.645.87.074.04.147.083.22.127.325.196.72.257 1.075.124l1.217-.456a1.125 1.125 0 0 1 1.37.49l1.296 2.247a1.125 1.125 0 0 1-.26 1.431l-1.003.827c-.293.241-.438.613-.43.992a7.723 7.723 0 0 1 0 .255c-.008.378.137.75.43.991l1.004.827c.424.35.534.955.26 1.43l-1.298 2.247a1.125 1.125 0 0 1-1.369.491l-1.217-.456c-.355-.133-.75-.072-1.076.124a6.47 6.47 0 0 1-.22.128c-.331.183-.581.495-.644.869l-.213 1.281c-.09.543-.56.94-1.11.94h-2.594c-.55 0-1.019-.398-1.11-.94l-.213-1.281c-.062-.374-.312-.686-.644-.87a6.52 6.52 0 0 1-.22-.127c-.325-.196-.72-.257-1.076-.124l-1.217.456a1.125 1.125 0 0 1-1.369-.49l-1.297-2.247a1.125 1.125 0 0 1 .26-1.431l1.004-.827c.292-.24.437-.613.43-.991a6.932 6.932 0 0 1 0-.255c.007-.38-.138-.751-.43-.992l-1.004-.827a1.125 1.125 0 0 1-.26-1.43l1.297-2.247a1.125 1.125 0 0 1 1.37-.491l1.216.456c.356.133.751.072 1.076-.124.072-.044.146-.086.22-.128.332-.183.582-.495.644-.869l.214-1.28Z" />
                <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z" />
              </svg>
            </div>
          </div>
        </header>
        <div className='flex flex-row grow '>
          <aside className='hidden md:flex w-48 pt-1 pl-1 pr-2 justify-start flex-col border-r border-neutral-400 dark:border-gray-700'>
            <button className=' text-gray-900 bg-white border border-gray-300 focus:outline-none hover:bg-gray-100 font-medium rounded-sm text-sm px-5 py-2.5  dark:bg-gray-800 dark:text-white dark:border-gray-600 dark:hover:bg-gray-700 dark:hover:border-gray-600 dark:focus:ring-gray-700' onClick={() => addItem()}>add item</button>
            <h4>Container</h4>
            <label htmlFor='direction' className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Direction</label>
            <select id='direction' className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white focus:ring-0' value={direction} onChange={e => setDirection(e.target.value)}>
              <option value="flex-row">Row</option>
              <option value="flex-col">Column</option>
            </select>
            {/* <label htmlFor='justifycontent' className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Justify-content</label> */}
            {/* <select className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-sm focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500' value={justifyContent} onChange={e => setJustifyContent(e.target.value)}>
              <option value="justify-start">Start</option>
              <option value="justify-center">Center</option>
              <option value="justify-end">End</option>
              <option value="justify-between">Between</option>
              <option value="justify-around">Around</option>
              <option value="justify-evenly">Evenly</option>
            </select> */}
            <h5>Justify-content</h5>
            <div className='grid grid-cols-2 gap-0.5 rounded-sm border border-neutral-500  dark:bg-gray-950 dark:border-gray-700 p-0.5 '>
              {
                justifyContents.map((x,i)=>(
                  <div key={i} onClick={()=>setJustifyContent(x.v)} className={`flex justify-center ${x.v===justifyContent?'bg-gray-800':''} rounded-sm p-1`}>{x.k}</div>
              ))
              }
            </div>
            
            {/* <h5>Align-items</h5>
            <select value={alignItems} onChange={e => setAlignItems(e.target.value)}>
              <option value="items-start">Start</option>
              <option value="items-center">Center</option>
              <option value="items-end">End</option>
              <option value="items-stretch">Stretch</option>
            </select> */}

            <h5>Align-items</h5>
            <div className='grid grid-cols-2 gap-0.5 rounded-sm border border-neutral-500  dark:bg-gray-950 dark:border-gray-700 p-0.5 '>
              {
                alignItems_.map((x,i)=>(
                  <div key={i} onClick={()=>setAlignItems(x.v)} className={`flex justify-center ${x.v===alignItems?'bg-gray-800 ':''} rounded-sm p-1`}>{x.k}</div>
              ))
              }
            </div>

            <h5>Gap(px)</h5>
            <input value={gap} onChange={x => setGap(x.target.value)} type='number' min="0" />
            <div className={`${item === null ? "hidden" : "flex"} flex-col grow `}>
              <h4>Item</h4>
              <h5>Align-self</h5>
              <select value={item?.alignSelf} onChange={e => alignSelfChange(e.target.value)}>
                <option value="self-auto">Auto</option>
                <option value="self-start">Start</option>
                <option value="self-center">Center</option>
                <option value="self-end">End</option>
                <option value="self-stretch">Stretch</option>
              </select>
              <h5>Flex-grow</h5>
              <input value={item?.grow} onChange={x => growChange(Number(x.target.value))} type='number' />
              <h5>Flex-shrink</h5>
              <input value={item?.shrink} onChange={x => shrinkChange(Number(x.target.value))} type='number' />
              <h5>Flex-basis({basisunit.current})</h5>
              <input placeholder="auto" value={item?.basis} onChange={x => basisChange(x.target.value)} />
            </div>
          </aside>
          <div className='p-1 overflow-auto flex grow'>
            <div style={{ gap: `${gap}px` }} className={`flex ${direction} ${alignItems} ${justifyContent} grow  dark:bg-gray-950 bg-neutral-200 rounded-sm`}>
              {
                items.map((x, i) => (
                  <div key={i} style={{ flexGrow: `${x.grow}`, flexShrink: `${x.shrink}`, flexBasis: `${x.basis}px` }} className={`dark:bg-gray-800 bg-white border-neutral-400 flex ${x.alignSelf} p-0.5 border rounded-sm dark:border-gray-600`} >
                    <div className='self-center px-3 py-1 grow'>index:{i}</div>
                    <div className="border-r dark:border-gray-600"></div>
                    <div onClick={() => setItem(x)} className={`flex items-center w-10 ${item === x ? "bg-gray-600" : ""}  dark:hover:bg-gray-600`}>
                      <img src={editLogo} />
                    </div>
                    <div className="border-r dark:border-gray-600"></div>
                    <div onClick={() => removeItem(x)} className='flex items-center w-10 dark:hover:bg-gray-600'>
                      <img src={deleteLogo} className="logo react" alt="React logo" />
                    </div>
                  </div>
                ))
              }
            </div>
          </div>
        </div>
        <Modal open={containerOpen} onClose={() => setContainerOpen(false)}>
          <button>123</button>
        </Modal>
      </div>
    </>
  )
}

export default App
