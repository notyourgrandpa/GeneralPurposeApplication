import { SortDirection } from '@angular/material/sort';

export interface ProductQueryParams {
  pageIndex: number;
  pageSize: number;
  search?: string;
  categoryId?: number;
  isActive?: boolean;
  sort?: string;
  direction?: SortDirection;
}
