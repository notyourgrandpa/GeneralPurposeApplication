export interface Product {
  id: number;
  name: string;
  categoryId: number;
  costPrice: number;
  sellingPrice: number;
  isActive: boolean;
  dateAdded: Date;
  lastUpdated: Date;
}
