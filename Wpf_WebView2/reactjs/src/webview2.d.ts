/* eslint-disable @typescript-eslint/no-explicit-any */
// interface Window {
//     chrome: {
//         webview: WebView;
//     };
// }

declare global {
    interface String {
        prependHello(): string;
    }
}
interface Window {
    chrome: {
        webview: WebView;
    };
}

interface WebViewEventListenerObject {
    handleEvent(object: Event & { data?: any }): void;
}

interface WebViewEventListener
{
    (evt: Event & { data?: any }): void;
}
type WebViewEventListenerOrEventListenerObject = WebViewEventListener | WebViewEventListenerObject;

class WebView
{
    addEventListener(
        type: string,
        listener: WebViewEventListenerOrEventListenerObject,
        options?: boolean | AddEventListenerOptions): void;
    postMessage(message: any) : void;
    releaseBuffer(buffer: ArrayBuffer): void;
}

