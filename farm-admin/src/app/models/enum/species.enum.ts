export enum Species {
    Deer = 0,
    Sheep = 1,
    Cow = 2,
}

export namespace Species {
    export function values() {
        return Object.keys(Species).filter((type) => isNaN(<any>type) && type !== 'values');
    }
}