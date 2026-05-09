export enum Status {
    Active = 1,
    Inactive = 0
}

export namespace Status {
    export function values() {
        return Object.keys(Status).filter((type) => isNaN(<any>type) && type !== 'values');
    }
}