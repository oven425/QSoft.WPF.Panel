
import { useRef, useState, useEffect } from 'react'
import './App.css'
import deleteLogo from './delete-alert-outline.svg'
import editLogo from './pencil.svg'
function App() {
  const [gap, setGap] = useState("0");
  const [direction, setDirection] = useState('flex-row');
  const [justifyContent, setJustifyContent] = useState('justify-start');
  const [alignItems, setAlignItems] = useState('items-start');
  const [items, setItems] = useState<Itemd[]>([]); 
  const [item, setItem] = useState<Itemd|null>(null);
  const basisunit = useRef('px');
  const id = useRef(0);
  type FlexBasis = "auto" | `${number}px` | `${number}%`;
  //type FlexBasis = "auto" | `${number}`
  type Itemd = {
      id:number
      alignSelf:string
      grow:number
      shrink:number
      basis:string
  };
  const addItem = ()=>{
    const admin: Itemd = {
      id : id.current++,
      alignSelf: 'self-auto',
      grow:0,
      shrink:0,
      basis:"auto"
    };
    setItems(prevItems => [...prevItems, admin]); 
    
  }
  const removeItem = (x:Itemd)=>{
    if(item!==null && x === item){
      setItem(null);
    }
    setItems(prevItems => prevItems.filter(item => item !== x));
  }

  const alignSelfChange = (newvalue:string)=>{
    setItem(x => x ? {
       ...x
       , alignSelf: newvalue 
      } : x);
  }

  const growChange = (newvalue:number)=>{
    setItem(x => x ? {
       ...x
       , grow: newvalue 
      } : x);
  }

  const shrinkChange = (newvalue:number)=>{
    setItem(x => x ? {
       ...x
       , shrink: newvalue 
      } : x);
  }

  const isFlexBasis = (value:string): value is FlexBasis=>{
    return value === "auto" || /^\d+$/.test(value);
  }
  const basisChange = (newvalue:string)=>{
    if(isFlexBasis(newvalue)){
      
    }  
    console.log(newvalue);
      setItem(x => x ? {
       ...x
       , basis: newvalue 
      } : x);
  }

  useEffect(()=>{
    if(!item) return;
    setItems(prevItems => {
          return prevItems.map(x => {
            if (x.id === item.id) {
              return item;
            }
            return x;
          });
        });
      },[item]);

  return (
    <>
      <div className='w-screen h-screen dark:bg-gray-900 dark:text-white flex flex-col'>
        <div className='border-b dark:border-gray-700 py-2'>
          <h1 className='text-3xl font-semibold dark:text-white ml-4'>Flex test tool</h1>
        </div>
        <div className='flex flex-row grow '>
          <aside className='w-48 flex pt-1 pl-1 pr-2 justify-start flex-col border-r dark:border-gray-700'>
            <button className=' text-gray-900 bg-white border border-gray-300 focus:outline-none hover:bg-gray-100 font-medium rounded-sm text-sm px-5 py-2.5  dark:bg-gray-800 dark:text-white dark:border-gray-600 dark:hover:bg-gray-700 dark:hover:border-gray-600 dark:focus:ring-gray-700' onClick={()=>addItem()}>add item</button>
            <h4>Container</h4>
            <label htmlFor='direction' className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Direction</label>
            <select id='direction' className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white focus:ring-0' value={direction} onChange={e=>setDirection(e.target.value)}>
              <option value="flex-row">Row</option>
              <option value="flex-col">Column</option>
            </select>
            <label htmlFor='justifycontent' className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">Justify-content</label>
            <select className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-sm focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500' value={justifyContent} onChange={e=>setJustifyContent(e.target.value)}>
              <option value="justify-start">Start</option>
              <option value="justify-center">Center</option>
              <option value="justify-end">End</option>
              <option value="justify-between">Between</option>
              <option value="justify-around">Around</option>
              <option value="justify-evenly">Evenly</option>
            </select>
            
            <h5>Align-items</h5>
            <select value={alignItems} onChange={e=>setAlignItems(e.target.value)}>
              <option value="items-start">Start</option>
              <option value="items-center">Center</option>
              <option value="items-end">End</option>
              <option value="items-stretch">Stretch</option>
            </select>
            <h5>Gap(px)</h5>
            <input value={gap} onChange={x=>setGap(x.target.value)} type='number' min="0"/>
            <div className={`${item === null ? "hidden" : "flex"} flex-col grow `}>
              <h4>Item</h4>
              <h5>Align-self</h5>
              <select value={item?.alignSelf} onChange={e=>alignSelfChange(e.target.value)}>
                <option value="self-auto">Auto</option>
                <option value="self-start">Start</option>
                <option value="self-center">Center</option>
                <option value="self-end">End</option>
                <option value="self-stretch">Stretch</option>
              </select>
              <h5>Flex-grow</h5>
              <input value={item?.grow} onChange={x=>growChange(Number(x.target.value))} type='number'/>
              <h5>Flex-shrink</h5>
              <input value={item?.shrink} onChange={x=>shrinkChange(Number(x.target.value))} type='number'/>
              <h5>Flex-basis({basisunit.current})</h5>
              <input placeholder="auto" value={item?.basis} onChange={x=>basisChange(x.target.value)}/>
            </div>
            
          </aside>
          <div className='dark:bg-red-900 p-1 overflow-auto flex grow'>
            <div style={{gap:`${gap}px`}} className={`flex ${direction} ${alignItems} ${justifyContent} grow  dark:bg-gray-950 rounded-sm`}>
            {
              items.map((x,i)=>(
                <div key={i} style={{flexGrow:`${x.grow}`, flexShrink:`${x.shrink}`, flexBasis:`${x.basis}px`}} className={`bg-gray-800 flex ${x.alignSelf} p-0.5 border rounded-sm dark:border-gray-600`} >
                  <div className='self-center px-3 py-1 grow'>index:{i}</div>
                  <div className="border-r dark:border-gray-600"></div>
                  <div onClick={()=>setItem(x)} className={`flex items-center w-10 ${item===x?"bg-gray-600":""}  dark:hover:bg-gray-600`}>
                    <img src={editLogo}/>
                  </div>
                  <div className="border-r dark:border-gray-600"></div>
                  <div onClick={()=>removeItem(x)} className='flex items-center w-10 dark:hover:bg-gray-600'>
                    <img src={deleteLogo} className="logo react" alt="React logo" />
                  </div>
                </div>
              ))
            }
          </div>
        </div>
          
        </div>
      </div>
    </>
  )
}

export default App
