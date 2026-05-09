export enum Gender {
    Male = 1,
    Female = 0
}

export namespace Gender {
    export function values() {
        return Object.keys(Gender).filter((type) => isNaN(<any>type) && type !== 'values');
    }
}