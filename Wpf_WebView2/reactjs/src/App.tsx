import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'

function App() {
  const [text, setText] = useState('text');
  const [count, setCount] = useState(0)
  const ttt = ()=>
  {
    setCount((count) => count + 1)
    window.chrome.webview.postMessage('aaa');
    // window.chrome.webview.addEventListener('aa', ()=>
    //   {
    //     console.log('123')
    //   }, false);

      window.chrome.webview.addEventListener('message', arg => {
        setText(arg.data);         
        document.body.style.background = 'red';    
        //alert(arg.data);
                 });
  }
  return (
    <>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Vite + React</h1>
      <div className="card">
        <p>{text}</p>
        <button onClick={ttt}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>
    </>
  )
}

export default App
