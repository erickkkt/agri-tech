export enum HealthStatus {
    Unknow = 0,
    Healthy = 1,
    Weak = 2,
    Sick = 3,
    Dead = 4
}

export namespace HealthStatus {
    export function values() {
        return Object.keys(HealthStatus).filter((type) => isNaN(<any>type) && type !== 'values');
    }
}