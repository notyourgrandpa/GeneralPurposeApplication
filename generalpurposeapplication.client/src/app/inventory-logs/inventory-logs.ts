export interface InventoryLog {
  id: number;
  productId: string;
  productName: string;
  quantity: number;
  changeType: number;
  remarks: string;
  date: Date;
}
