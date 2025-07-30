export interface InventoryLog {
  id: number;
  productId: string;
  productName: string;
  quantityChange: number;
  remarks: string;
  date: Date;
}
