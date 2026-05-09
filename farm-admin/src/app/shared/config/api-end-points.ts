import { Injectable } from '@angular/core';
import { ConfigurationService } from '../../services/configuration.service';
import { configuration } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ApiEndPoints {
    getAuthorizedNavItems(): string {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/configuration/nav-items`;
    }

    constructor(private readonly configurationService: ConfigurationService) { }

    /*********** FARM **************/
    getActiveFarms() {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/farms/active`;
    }

    getFarmsWithPaging(sortField: string, sortDirection: string, pageIndex: number, pageSize: number) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/farms/paging/${sortField}/${sortDirection}/${pageIndex}/${pageSize}`;
    }

    createFarm() {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/farms`;
    }

    updateFarm() {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/farms`;
    }

    getFarmByFarmId(farmId: string) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/farms/${farmId}`;
    }

    deleteFarm(farmId: string) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/farms/${farmId}`;
    }

    /*********** CAGE **************/
    getActiveCages(farmId: string): string {
      return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/cages/active?farmId=${farmId}`;
    }

    getCagesWithPaging(sortField: string, sortDirection: string, pageIndex: number, pageSize: number) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/cages/paging/${sortField}/${sortDirection}/${pageIndex}/${pageSize}`;
    }

    createCage() {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/cages`;
    }

    updateCage() {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/cages`;
    }

    getCageByCageId(cageId: string) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/cages/${cageId}`;
    }

    deleteCage(cageId: string) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/cages/${cageId}`;
    }

    /*********** FARM **************/
    getAnimalsWithPaging(sortField: string, sortDirection: string, pageIndex: number, pageSize: number) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/animals/paging/${sortField}/${sortDirection}/${pageIndex}/${pageSize}`;
    }

    createAnimal() {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/animals`;
    }

    updateAnimal() {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/animals`;
    }

    getAnimalByAnimalId(animalId: string) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/animals/${animalId}`;
    }

    deleteAnimal(animalId: string) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/animals/${animalId}`;
    }


    /*********** USERS **************/
    getUserInfo() {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/users/info`;
    }

    signOut() {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/users/sign-out`;
    }

    /*********** PHASE 1 - VACCINE *********/
    getVaccinesPaging(sortField: string, sortDirection: string, pageIndex: number, pageSize: number) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/vaccines/paging/${sortField}/${sortDirection}/${pageIndex}/${pageSize}`;
    }
    createVaccine() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/vaccines`; }
    updateVaccine() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/vaccines`; }
    getVaccineSchedulesByAnimal(animalId: string) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/vaccines/schedules/animal/${animalId}`;
    }
    getVaccineUpcoming(daysAhead = 7) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/vaccines/schedules/upcoming/${daysAhead}`;
    }
    createVaccineSchedule() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/vaccines/schedules`; }
    administerVaccine() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/vaccines/schedules/administer`; }

    /*********** PHASE 1 - FEED *********/
    getFeedItemsPaging(pageIndex: number, pageSize: number) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/feed/items/paging/${pageIndex}/${pageSize}`;
    }
    createFeedItem() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/feed/items`; }
    updateFeedItem() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/feed/items`; }
    createFeedTransaction() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/feed/transactions`; }
    getFeedTransactions() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/feed/transactions`; }
    createFeedConsumption() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/feed/consumptions`; }
    getFeedConsumptions() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/feed/consumptions`; }
    getFeedSummary(farmId: string) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/feed/summary/${farmId}`;
    }
    getFeedLowStock() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/feed/low-stock`; }

    /*********** PHASE 1 - GROWTH LOG *********/
    getGrowthLogsByAnimal(animalId: string, take = 100) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/growth-logs/animal/${animalId}?take=${take}`;
    }
    createGrowthLog() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/growth-logs`; }
    updateGrowthLog() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/growth-logs`; }
    deleteGrowthLog(id: string) { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/growth-logs/${id}`; }

    /*********** PHASE 1 - DISEASE *********/
    getDiseasesByAnimal(animalId: string) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/diseases/animal/${animalId}`;
    }
    createDisease() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/diseases`; }
    updateDisease() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/diseases`; }
    createTreatment() { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/diseases/treatments`; }
    getTreatmentsByAnimal(animalId: string) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/diseases/treatments/animal/${animalId}`;
    }

    /*********** PHASE 1 - ALERT *********/
    getAlertsPaging(pageIndex: number, pageSize: number, unread?: boolean, farmId?: string) {
        let url = `${this.configurationService.apiBaseUrl}/api/${configuration.version}/alerts/paging/${pageIndex}/${pageSize}`;
        const qs: string[] = [];
        if (unread !== undefined) qs.push(`unread=${unread}`);
        if (farmId) qs.push(`farmId=${farmId}`);
        if (qs.length) url += `?${qs.join('&')}`;
        return url;
    }
    getAlertSummary(farmId?: string) {
        let url = `${this.configurationService.apiBaseUrl}/api/${configuration.version}/alerts/summary`;
        if (farmId) url += `?farmId=${farmId}`;
        return url;
    }
    markAlertRead(id: string) { return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/alerts/${id}/read`; }
    markAllAlertsRead(farmId?: string) {
        let url = `${this.configurationService.apiBaseUrl}/api/${configuration.version}/alerts/read-all`;
        if (farmId) url += `?farmId=${farmId}`;
        return url;
    }

    /*********** PHASE 1 - REPORT *********/
    getDashboard(farmId?: string) {
        let url = `${this.configurationService.apiBaseUrl}/api/${configuration.version}/reports/dashboard`;
        if (farmId) url += `?farmId=${farmId}`;
        return url;
    }

    /*********** MEDIA *********/
    uploadMedia(kind: string) {
        return `${this.configurationService.apiBaseUrl}/api/${configuration.version}/media/upload?kind=${kind}`;
    }

    /*********** REALTIME *********/
    notificationHub() {
        return `${this.configurationService.apiBaseUrl}/hubs/notifications`;
    }
}
