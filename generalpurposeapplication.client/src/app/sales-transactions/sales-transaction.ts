export interface SalesTransaction{
  id: number;
  totalAmount: number;
  paymentMethod: string;
  processedByUserId: number;
  procsssedByUserName: string;
  date: Date;
}
