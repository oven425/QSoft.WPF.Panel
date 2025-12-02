export type Kv<T1, T2> = {
    k: T1
    v: T2
}

export const alignSelfs: Kv<string, string>[] = [
    { k: 'Auto', v: 'self-auto' },
    { k: 'Start', v: 'self-start' },
    { k: 'Center', v: 'self-center' },
    { k: 'End', v: 'self-end' },
    { k: 'Stretch', v: 'self-stretch' },
]

