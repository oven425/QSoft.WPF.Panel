
import { useState } from 'react'
import './App.css'

function App() {
  const [gap, setGap] = useState(2.2);
  const [direction, setDirection] = useState('flex-row');
  return (
    <>
      <div className='w-screen h-screen bg-red-300 flex flex-col'>
        <div className='border-b-2'>
          <h1>Flex panel</h1>
        </div>   
        <div className='flex flex-row grow bg-yellow-200 '>
          <div className='bg-green-500 w-36 flex justify-start flex-col border-r-2'>
            {/* <Button>Add item</Button> */}
            <h5>Direction</h5>
            <select value={direction} onChange={e=>setDirection(e.target.value)}>
              <option value="flex-row">Row</option>
              <option value="flex-col">Column</option>
            </select>
            <h5>Justify-content</h5>
            <select>
              <option value="justify-start">Start</option>
              <option>Center</option>
              <option>End</option>
            </select>
            <h5>Align-items</h5>
            <select>
              <option>Start</option>
              <option>Center</option>
              <option>End</option>
            </select>
            <h5>Gap</h5>
            {/* <input value={gap} onChange={x=>setGap(x.target.value)}/> */}
            <button onClick={()=>setGap(x=>x+2)}>AAA</button>
            <h5>Padding</h5>
            <input/>
          </div>
          <div className={`bg-blue-500 flex ${direction} align-top justify-start gap-[${gap}px] grow`}>
          {/* <div className='bg-blue-500 flex align-top justify-start gap-[2px] grow'> */}
            <div className='bg-pink-500 w-5 h-5'>

            </div>
            <div className='bg-purple-500 w-5 h-5'>

            </div>
          </div>
        </div>
      </div>
    </>
  )
}

export default App
