import { useState } from 'react';
import './App.css'
// import { Button } from "@/components/ui/button"
import { Button } from './components/ui/button'
import {
  Field,
  FieldDescription,
  FieldGroup,
  FieldLabel,
  FieldLegend,
  FieldSeparator,
  FieldSet,
} from "./components/ui/field"

import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./components/ui/select"
import { Input } from './components/ui/input';

function App() {
  const [gap, setGap] = useState("0");
  const [direction, setDirection] = useState('flex-row');
  const [justifyContent, setJustifyContent] = useState('justify-start');
  const [alignItems, setAlignItems] = useState('items-start');
  return (
    <>
      <div className='w-screen h-screen bg-red-300 flex flex-col'>
        <div className='border-b-2'>
          <h1>Flex panel</h1>
        </div>   
        <div className='flex flex-row grow bg-yellow-200 '>
          <form>
            {/* https://ui.shadcn.com/docs/components/field */}
            <FieldGroup>
              <Field>
                <FieldLabel htmlFor="checkout-7j9-card-name-43j">
                  Justify-content
                </FieldLabel>
                <Select defaultValue="justify-start" onValueChange={e=>setDirection(e)}>
                    <SelectTrigger id="checkout-exp-month-ts6">
                      <SelectValue placeholder="MM" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="justify-start">Start</SelectItem>
                      <SelectItem value="justify-center">Center</SelectItem>
                      <SelectItem value="justify-end">End</SelectItem>
                      <SelectItem value="justify-between">Between</SelectItem>
                      <SelectItem value="justify-around">Around</SelectItem>
                      <SelectItem value="justify-evenly">Evenly</SelectItem>
                    </SelectContent>
                  </Select>
              </Field>
            </FieldGroup>
            <div className='bg-green-500 w-36 flex justify-start flex-col border-r-2'>

            <Button variant="secondary">Add item</Button>
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
          </div>
          </form>
          
          <div style={{gap:`${gap}px`}} className={`bg-blue-500 flex ${direction} ${alignItems} ${justifyContent}  grow`}>
            <div className='bg-pink-500'>
              AAA
            </div>
            <div className='bg-purple-500'>
              BBB
            </div>
            <div className='bg-yellow-400 border-amber-900 border-2 basis-10 flex hover:bg-red-500 hover:border-gray-700'>
              {/* <img src={deleteLogo} className="logo react" alt="React logo" /> */}
            </div>
          </div>
        </div>
      </div>
      {/* <div className="flex min-h-svh flex-col items-center justify-center">
        <Button variant="secondary">Click me</Button>
      </div> */}
    </>
  )
}

export default App
