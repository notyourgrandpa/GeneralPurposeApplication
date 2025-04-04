export interface Product {
  id: number;
  name: string;
  categoryName: string;
  categoryId: number;
  costPrice: number;
  sellingPrice: number;
  isActive: boolean;
  dateAdded: Date;
  lastUpdated: Date
}
