export interface Product {
  id: number;
  name: string;
  categoryId: number;
  categoryName: string;
  costPrice: number;
  sellingPrice: number;
  stock: number;
  isActive: boolean;
  dateAdded: Date;
  lastUpdated: Date;
}
