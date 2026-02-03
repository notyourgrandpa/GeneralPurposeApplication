import { SortDirection } from '@angular/material/sort';

export interface ProductQueryParams {
  pageIndex: number;
  pageSize: number;
  filterQuery?: string;
  filter?: ProductFilter;
  sort?: string;
  direction?: SortDirection;
}

export interface ProductFilter{
  categoryId?: number;
  isActive?: boolean;
}
