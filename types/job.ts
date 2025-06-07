export interface Job {
  id: string;
  title: string;
  pickupAddress: string;
  deliveryAddress: string;
  pickupCity: string;
  deliveryCity: string;
  distance: number; // in miles
  rate: number; // payment amount
  priority: 'low' | 'medium' | 'high' | 'urgent';
  cargoType: string;
  weight: number; // in lbs
  pickupTime: string;
  deliveryTime: string;
  status: 'available' | 'assigned' | 'in_progress' | 'completed' | 'cancelled';
  customerName: string;
  customerPhone: string;
  specialInstructions?: string;
  createdAt: string;
}

export interface Driver {
  id: string;
  name: string;
  email: string;
  phone: string;
  rating: number;
  totalJobs: number;
  totalEarnings: number;
  vehicleType: string;
  licenseNumber: string;
  profileImage?: string;
}