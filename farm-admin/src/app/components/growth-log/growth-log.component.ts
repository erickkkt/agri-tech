import { Component, OnInit } from '@angular/core';
import { GrowthLog, CreateGrowthLog } from '../../models/growth-log.model';
import { GrowthLogService } from '../../services/growth-log.service';
import { AnimalService } from '../../services/animal.service';
import { Animal } from '../../models/animal.model';

/**
 * Phase 1 - Growth log management.
 * Pick an animal, view its weight/height log history, add a new entry.
 */
@Component({
  selector: 'app-growth-log',
  standalone: false,
  templateUrl: './growth-log.component.html'
})
export class GrowthLogComponent implements OnInit {
  animals: Animal[] = [];
  selectedAnimalId: string | null = null;
  logs: GrowthLog[] = [];

  // Inline new-entry form
  newWeight: number | null = null;
  newHeight: number | null = null;
  newBcs: number | null = null;
  newNotes = '';

  constructor(private animalService: AnimalService, private growthService: GrowthLogService) { }

  async ngOnInit() {
    const result = await this.animalService.getAnimals('Name', 'asc', 0, 100);
    this.animals = result?.items ?? [];
    if (this.animals.length) {
      this.selectedAnimalId = this.animals[0].id;
      await this.loadLogs();
    }
  }

  async loadLogs() {
    if (!this.selectedAnimalId) return;
    this.logs = (await this.growthService.getByAnimal(this.selectedAnimalId, 50)) ?? [];
  }

  async addLog() {
    if (!this.selectedAnimalId || this.newWeight == null) return;
    const payload: CreateGrowthLog = {
      animalId: this.selectedAnimalId,
      recordedAt: new Date(),
      weight: this.newWeight,
      height: this.newHeight ?? undefined,
      bodyConditionScore: this.newBcs ?? undefined,
      notes: this.newNotes
    };
    await this.growthService.create(payload);
    this.newWeight = null;
    this.newHeight = null;
    this.newBcs = null;
    this.newNotes = '';
    await this.loadLogs();
  }
}
