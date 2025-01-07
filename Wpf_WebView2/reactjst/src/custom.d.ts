// custom.d.ts
interface Chrome {
    webview?: {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      postMessage: (message: any) => void;
    };
  }
  
  interface Window {
    chrome?: Chrome;
  }
  