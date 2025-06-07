import { Router } from 'express';
import { body, query, validationResult } from 'express-validator';
import { authenticateToken, requireDriverOrAdmin } from '../middleware/auth';
import { ApiResponse, PaginatedResponse, Job } from '../types';
import { NotFoundError } from '../middleware/errorHandler';

const router = Router();

// Mock database - replace with actual database
const jobs: Job[] = [
  {
    id: 'job_1',
    title: 'Electronics Delivery - Austin to Dallas',
    description: 'Urgent delivery of electronic components',
    pickupAddress: '1234 Tech Blvd, Austin, TX 78701',
    deliveryAddress: '5678 Business Dr, Dallas, TX 75201',
    pickupCity: 'Austin',
    deliveryCity: 'Dallas',
    pickupCoordinates: { latitude: 30.2672, longitude: -97.7431 },
    deliveryCoordinates: { latitude: 32.7767, longitude: -96.7970 },
    distance: 195,
    estimatedDuration: 180, // minutes
    rate: 450,
    priority: 'high',
    cargoType: 'Electronics',
    weight: 150,
    pickupTime: new Date('2024-01-15T09:00:00Z'),
    deliveryTime: new Date('2024-01-15T14:00:00Z'),
    status: 'available',
    customerId: 'customer_1',
    customerName: 'TechCorp Solutions',
    customerPhone: '(512) 555-0123',
    customerEmail: 'orders@techcorp.com',
    specialInstructions: 'Handle with care - fragile electronics',
    createdAt: new Date('2024-01-14T10:30:00Z'),
    updatedAt: new Date('2024-01-14T10:30:00Z')
  },
  {
    id: 'job_2',
    title: 'Medical Supplies - Houston to San Antonio',
    description: 'Temperature-controlled medical supply delivery',
    pickupAddress: '789 Medical Center Blvd, Houston, TX 77030',
    deliveryAddress: '321 Hospital Way, San Antonio, TX 78201',
    pickupCity: 'Houston',
    deliveryCity: 'San Antonio',
    pickupCoordinates: { latitude: 29.7604, longitude: -95.3698 },
    deliveryCoordinates: { latitude: 29.4241, longitude: -98.4936 },
    distance: 197,
    estimatedDuration: 200,
    rate: 520,
    priority: 'urgent',
    cargoType: 'Medical Supplies',
    weight: 85,
    pickupTime: new Date('2024-01-15T06:00:00Z'),
    deliveryTime: new Date('2024-01-15T10:30:00Z'),
    status: 'available',
    customerId: 'customer_2',
    customerName: 'MedSupply Corp',
    customerPhone: '(713) 555-0456',
    customerEmail: 'logistics@medsupply.com',
    specialInstructions: 'Temperature controlled - keep refrigerated',
    createdAt: new Date('2024-01-14T08:15:00Z'),
    updatedAt: new Date('2024-01-14T08:15:00Z')
  }
];

// Get all jobs with filtering and pagination
router.get('/', [
  query('page').optional().isInt({ min: 1 }),
  query('limit').optional().isInt({ min: 1, max: 100 }),
  query('status').optional().isIn(['available', 'assigned', 'in_progress', 'completed', 'cancelled']),
  query('priority').optional().isIn(['low', 'medium', 'high', 'urgent']),
  query('city').optional().isString()
], authenticateToken, (req: any, res) => {
  const errors = validationResult(req);
  if (!errors.isEmpty()) {
    return res.status(400).json({
      success: false,
      error: 'Validation failed',
      details: errors.array()
    } as ApiResponse);
  }

  const page = parseInt(req.query.page) || 1;
  const limit = parseInt(req.query.limit) || 10;
  const status = req.query.status;
  const priority = req.query.priority;
  const city = req.query.city;

  let filteredJobs = [...jobs];

  // Apply filters
  if (status) {
    filteredJobs = filteredJobs.filter(job => job.status === status);
  }
  if (priority) {
    filteredJobs = filteredJobs.filter(job => job.priority === priority);
  }
  if (city) {
    filteredJobs = filteredJobs.filter(job => 
      job.pickupCity.toLowerCase().includes(city.toLowerCase()) ||
      job.deliveryCity.toLowerCase().includes(city.toLowerCase())
    );
  }

  // For drivers, only show available jobs or their assigned jobs
  if (req.user.role === 'driver') {
    filteredJobs = filteredJobs.filter(job => 
      job.status === 'available' || job.driverId === req.user.id
    );
  }

  // Pagination
  const total = filteredJobs.length;
  const totalPages = Math.ceil(total / limit);
  const startIndex = (page - 1) * limit;
  const endIndex = startIndex + limit;
  const paginatedJobs = filteredJobs.slice(startIndex, endIndex);

  res.json({
    success: true,
    data: paginatedJobs,
    pagination: {
      page,
      limit,
      total,
      totalPages
    }
  } as PaginatedResponse<Job>);
});

// Get job by ID
router.get('/:id', authenticateToken, (req: any, res) => {
  const job = jobs.find(j => j.id === req.params.id);
  
  if (!job) {
    throw new NotFoundError('Job not found');
  }

  // Drivers can only see available jobs or their assigned jobs
  if (req.user.role === 'driver' && job.status !== 'available' && job.driverId !== req.user.id) {
    throw new NotFoundError('Job not found');
  }

  res.json({
    success: true,
    data: job
  } as ApiResponse<Job>);
});

// Create new job (admin only)
router.post('/', [
  body('title').notEmpty().trim(),
  body('pickupAddress').notEmpty().trim(),
  body('deliveryAddress').notEmpty().trim(),
  body('pickupCity').notEmpty().trim(),
  body('deliveryCity').notEmpty().trim(),
  body('distance').isFloat({ min: 0 }),
  body('rate').isFloat({ min: 0 }),
  body('priority').isIn(['low', 'medium', 'high', 'urgent']),
  body('cargoType').notEmpty().trim(),
  body('weight').isFloat({ min: 0 }),
  body('pickupTime').isISO8601(),
  body('deliveryTime').isISO8601(),
  body('customerName').notEmpty().trim(),
  body('customerPhone').notEmpty().trim(),
  body('customerEmail').isEmail()
], authenticateToken, requireDriverOrAdmin, (req: any, res) => {
  const errors = validationResult(req);
  if (!errors.isEmpty()) {
    return res.status(400).json({
      success: false,
      error: 'Validation failed',
      details: errors.array()
    } as ApiResponse);
  }

  const newJob: Job = {
    id: `job_${Date.now()}`,
    ...req.body,
    pickupCoordinates: { latitude: 0, longitude: 0 }, // Would be geocoded in real app
    deliveryCoordinates: { latitude: 0, longitude: 0 },
    estimatedDuration: Math.round(req.body.distance * 3), // Rough estimate
    status: 'available',
    customerId: `customer_${Date.now()}`,
    createdAt: new Date(),
    updatedAt: new Date()
  };

  jobs.push(newJob);

  res.status(201).json({
    success: true,
    data: newJob,
    message: 'Job created successfully'
  } as ApiResponse<Job>);
});

// Accept job (driver only)
router.post('/:id/accept', authenticateToken, (req: any, res) => {
  if (req.user.role !== 'driver') {
    return res.status(403).json({
      success: false,
      error: 'Only drivers can accept jobs'
    } as ApiResponse);
  }

  const job = jobs.find(j => j.id === req.params.id);
  
  if (!job) {
    throw new NotFoundError('Job not found');
  }

  if (job.status !== 'available') {
    return res.status(400).json({
      success: false,
      error: 'Job is not available'
    } as ApiResponse);
  }

  // Update job status
  job.status = 'assigned';
  job.driverId = req.user.id;
  job.updatedAt = new Date();

  res.json({
    success: true,
    data: job,
    message: 'Job accepted successfully'
  } as ApiResponse<Job>);
});

// Update job status
router.patch('/:id/status', [
  body('status').isIn(['assigned', 'in_progress', 'completed', 'cancelled'])
], authenticateToken, (req: any, res) => {
  const errors = validationResult(req);
  if (!errors.isEmpty()) {
    return res.status(400).json({
      success: false,
      error: 'Validation failed',
      details: errors.array()
    } as ApiResponse);
  }

  const job = jobs.find(j => j.id === req.params.id);
  
  if (!job) {
    throw new NotFoundError('Job not found');
  }

  // Drivers can only update their own jobs
  if (req.user.role === 'driver' && job.driverId !== req.user.id) {
    return res.status(403).json({
      success: false,
      error: 'You can only update your own jobs'
    } as ApiResponse);
  }

  const { status } = req.body;
  job.status = status;
  job.updatedAt = new Date();

  if (status === 'completed') {
    job.completedAt = new Date();
  }

  res.json({
    success: true,
    data: job,
    message: 'Job status updated successfully'
  } as ApiResponse<Job>);
});

// Add job photos/signature
router.post('/:id/documents', authenticateToken, (req: any, res) => {
  const job = jobs.find(j => j.id === req.params.id);
  
  if (!job) {
    throw new NotFoundError('Job not found');
  }

  // Only assigned driver can upload documents
  if (job.driverId !== req.user.id) {
    return res.status(403).json({
      success: false,
      error: 'You can only upload documents for your assigned jobs'
    } as ApiResponse);
  }

  const { photos, signature } = req.body;

  if (photos) {
    job.photos = [...(job.photos || []), ...photos];
  }

  if (signature) {
    job.signature = signature;
  }

  job.updatedAt = new Date();

  res.json({
    success: true,
    data: job,
    message: 'Documents uploaded successfully'
  } as ApiResponse<Job>);
});

export { router as jobRoutes };