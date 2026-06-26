import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InventoryLogDetailsComponent } from './inventory-log-details.component';

describe('InventoryLogDetailsComponent', () => {
  let component: InventoryLogDetailsComponent;
  let fixture: ComponentFixture<InventoryLogDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InventoryLogDetailsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(InventoryLogDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
