
import { useRef, useState, useEffect } from 'react'
import './App.css'
import deleteLogo from './delete-alert-outline.svg'
function App() {
  const [gap, setGap] = useState("0");
  const [direction, setDirection] = useState('flex-row');
  const [justifyContent, setJustifyContent] = useState('justify-start');
  const [alignItems, setAlignItems] = useState('items-start');
  const [items, setItems] = useState<Itemd[]>([]); 
  const [item, setItem] = useState<Itemd>({id: 0, alignSelf: 'self-start',grow:0 , basis:"auto"});
  const id = useRef(0);
  type flexBasis = number | "auto";
  type Itemd = {
      id:number
      alignSelf:string
      grow:number
      basis:flexBasis
  };
  const addItem = ()=>{
    const admin: Itemd = {
      id : id.current++,
      alignSelf: 'self-center',
      grow:0,
      basis:"auto"
    };
    setItems(prevItems => [...prevItems, admin]); 
    
  }
  const removeItem = (x:Itemd)=>{
    setItems(prevItems => prevItems.filter(item => item !== x));
  }

  const alignSelfChange = (newvalue:string)=>{
    setItem(x=>({
      ... x,
      alignSelf: newvalue
    }));
  }

  const growChange = (newvalue:number)=>{
    setItem(x=>({
      ... x,
      grow: newvalue
    }));
  }

  const basisChange = (newvalue:flexBasis)=>{
    setItem(x=>({
      ... x,
      basis: newvalue
    }));
  }

  useEffect(()=>{
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
      <div className='w-screen h-screen bg-red-300 flex flex-col'>
        <div className='border-b-2'>
          <h1>Flex panel</h1>
        </div>   
        <div className='flex flex-row grow bg-yellow-200 '>
          <div className='bg-green-500 w-36 flex justify-start flex-col border-r-2'>
            <button className='bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 border border-blue-700 rounded' onClick={()=>addItem()}>add item</button>
            <h5>Direction</h5>
            <select value={direction} onChange={e=>setDirection(e.target.value)}>
              <option value="flex-row">Row</option>
              <option value="flex-col">Column</option>
            </select>
            <h5>Justify-content</h5>
            <select value={justifyContent} onChange={e=>setJustifyContent(e.target.value)}>
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
            <h5>Gap</h5>
            <input value={gap} onChange={x=>setGap(x.target.value)} type='number'/>
            <h5>Padding</h5>
            <input/>
            <h4>Item</h4>
            <h5>Align-self</h5>
            <select value={item.alignSelf} onChange={e=>alignSelfChange(e.target.value)}>
              <option value="self-start">Start</option>
              <option value="self-center">Center</option>
              <option value="self-end">End</option>
              <option value="self-stretch">Stretch</option>
            </select>
            <h5>Flex-grow</h5>
            <input value={item.grow} onChange={x=>growChange(Number(x.target.value))} type='number'/>
            <h5>Flex-basis</h5>
            <input placeholder="auto" value={typeof item.basis === "number" ? item.basis : ""} onChange={x=>basisChange(x.target.value === "" ? "auto" : Number(x.target.value))} type='number'/>
          </div>
          <div style={{gap:`${gap}px`}} className={`bg-blue-500 flex ${direction} ${alignItems} ${justifyContent}  grow`}>
            {
              items.map((x,i)=>(
                <div key={i} style={{flexGrow:`${x.grow}`, flexBasis:`${x.basis}`}} className={`bg-amber-400 flex ${x.alignSelf}`} >
                  <div onClick={()=>setItem(x)} className='self-center'>Index:{i} id:{x.id}</div>
                  <div onClick={()=>removeItem(x)} className='bg-yellow-400 border-amber-900 w-10 hover:bg-red-500 hover:border-gray-700'>
                    <img src={deleteLogo} className="logo react" alt="React logo" />
                  </div>
                </div>
              ))
            }
          </div>
        </div>
      </div>
    </>
  )
}

export default App
