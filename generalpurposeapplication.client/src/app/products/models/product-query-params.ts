export interface ProductQueryParams {
  pageIndex: number;
  pageSize: number;
  search?: string;
  categoryId?: number;
  isActive?: boolean;
  sort?: 'name' | 'price' | 'dateAdded';
  direction?: 'asc' | 'desc';
}
