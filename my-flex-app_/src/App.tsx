import './App.css'
// import { Button } from "@/components/ui/button"
function App() {

  return (
    <>
      <div className='w-screen h-screen bg-red-300 flex flex-col'>
        <div className='border-b-2'>
          <h1>Flex panel</h1>
        </div>   
        <div className='flex flex-row flex-grow bg-yellow-200 '>
          <div className='bg-green-500 w-36 flex justify-start flex-col border-r-2'>
            {/* <Button>Add item</Button> */}
            <h5>Direction</h5>
            <select>
              <option>Row</option>
              <option>Column</option>
            </select>
            <h5>Justify-content</h5>
            <select>
              <option>Start</option>
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
            <input/>
            <h5>Padding</h5>
            <input/>
          </div>
          <div className='bg-blue-500 flex-grow'>
            
          </div>
        </div>
      </div>
    </>
  )
}

export default App
