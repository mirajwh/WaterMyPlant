import { TestBed } from '@angular/core/testing';

import { PlantserviceService } from './plantservice.service';

describe('PlantserviceService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PlantserviceService = TestBed.get(PlantserviceService);
    expect(service).toBeTruthy();
  });
});
