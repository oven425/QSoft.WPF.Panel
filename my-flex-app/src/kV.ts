export type Kv<T1, T2> = {
    k: T1
    v: T2
}

export const justifyContents:Kv<string, string>[] = [
  {k:'Start', v:'justify-start'},
  {k:'Center', v:'justify-center'},
  {k:'End', v:'justify-end'},
  {k:'Between', v:'justify-between'},
  {k:'Around', v:'justify-around'},
  {k:'Evenly', v:'justify-evenly'},
]

export const directions:Kv<string, string>[] = [
  {k:'Row', v:'flex-row'},
  {k:'Column', v:'flex-col'},
]

export const alignItems_:Kv<string, string>[] = [
  {k:'Start', v:'items-start'},
  {k:'Center', v:'items-center'},
  {k:'End', v:'items-end'},
  {k:'Stretch', v:'items-stretch'},
]

export const alignSelfs: Kv<string, string>[] = [
    { k: 'Auto', v: 'self-auto' },
    { k: 'Start', v: 'self-start' },
    { k: 'Center', v: 'self-center' },
    { k: 'End', v: 'self-end' },
    { k: 'Stretch', v: 'self-stretch' },
]

