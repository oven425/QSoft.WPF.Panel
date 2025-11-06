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
          <div className='bg-green-500 w-36 flex justify-start flex-col border-r-2'>
            <form>
              <FieldSet>
        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="username">Username</FieldLabel>
            <Input id="username" type="text" placeholder="Max Leiter" />
            <FieldDescription>
              Choose a unique username for your account.
            </FieldDescription>
          </Field>
          <Field>
            <FieldLabel htmlFor="password">Password</FieldLabel>
            <FieldDescription>
              Must be at least 8 characters long.
            </FieldDescription>
            <Input id="password" type="password" placeholder="••••••••" />
          </Field>
        </FieldGroup>
      </FieldSet>
            </form>
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
