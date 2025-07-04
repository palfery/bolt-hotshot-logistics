# Security Configuration Guide

## Production Security Checklist

### 1. Environment Variables (Required)
```bash
# Authentication
JWT_SECRET_KEY="your-256-bit-secret-key-here-use-crypto-random-generator"
JWT_ISSUER="https://your-production-domain.com"
JWT_AUDIENCE="hotshot-logistics-api"

# Database
HSL_DBServer="your-sql-server.database.windows.net"
HSL_DBName="hotshot_logistics_prod"
HSL_DBUser="your-sql-user"
HSL_DBPassword="your-secure-password"

# Azure
AZURE_CLIENT_ID="your-azure-client-id"
AZURE_CLIENT_SECRET="your-azure-client-secret"
AZURE_TENANT_ID="your-azure-tenant-id"
AZURE_SUBSCRIPTION_ID="your-azure-subscription-id"

# Application Insights
APPLICATIONINSIGHTS_CONNECTION_STRING="your-app-insights-connection-string"
```

### 2. Azure Key Vault Setup
1. Create Azure Key Vault
2. Store all secrets in Key Vault
3. Configure Managed Identity for Function App
4. Update application to read from Key Vault

### 3. SQL Server Security
- [ ] Disable public network access
- [ ] Configure private endpoints
- [ ] Enable auditing and threat detection
- [ ] Use strong passwords (20+ characters)
- [ ] Enable transparent data encryption

### 4. Function App Security
- [ ] Enable HTTPS only
- [ ] Configure custom domain with SSL
- [ ] Set minimum TLS version to 1.2
- [ ] Enable authentication/authorization
- [ ] Configure CORS properly
- [ ] Set appropriate function authorization levels

### 5. Network Security
- [ ] Configure Virtual Network integration
- [ ] Set up Network Security Groups
- [ ] Enable DDoS protection
- [ ] Configure Web Application Firewall

### 6. Monitoring and Logging
- [ ] Enable Azure Security Center
- [ ] Configure Log Analytics workspace
- [ ] Set up security alerts
- [ ] Enable audit logging
- [ ] Configure automated backup

### 7. CI/CD Security
- [ ] Scan code for vulnerabilities
- [ ] Scan dependencies for vulnerabilities
- [ ] Run security tests in pipeline
- [ ] Use secure deployment practices
- [ ] Rotate secrets regularly

## Security Headers Configuration

### HTTPS Enforcement
```csharp
// In Startup.cs or Program.cs
app.UseHttpsRedirection();
app.UseHsts(); // For production only
```

### Security Headers
The `SecurityHeadersMiddleware` class automatically adds:
- X-Content-Type-Options: nosniff
- X-Frame-Options: DENY
- X-XSS-Protection: 1; mode=block
- Content-Security-Policy: (restrictive policy)
- Referrer-Policy: strict-origin-when-cross-origin

## Rate Limiting Configuration

### Basic Rate Limiting
```csharp
// Configure rate limiting service
services.AddSingleton<RateLimitingService>(provider => 
    new RateLimitingService(
        provider.GetRequiredService<IMemoryCache>(),
        TimeSpan.FromMinutes(1), // Window size
        100 // Max requests per window
    ));
```

### Per-Endpoint Rate Limiting
Different endpoints can have different rate limits:
- Authentication endpoints: 5 requests/minute
- Read operations: 100 requests/minute
- Write operations: 20 requests/minute

## Input Validation

### Validation Rules
- All user inputs must be validated
- Use whitelist approach (allow known good)
- Sanitize data before processing
- Validate data types, lengths, and formats

### FluentValidation Usage
```csharp
// Register validators
services.AddValidatorsFromAssemblyContaining<DriverRequestValidator>();
services.AddValidatorsFromAssemblyContaining<JobRequestValidator>();
```

## Error Handling

### Production Error Responses
- Never expose stack traces
- Log detailed errors server-side
- Return generic error messages to clients
- Use correlation IDs for tracking

### Logging Best Practices
- Log security events (failed auth, suspicious activity)
- Don't log sensitive data (passwords, tokens)
- Use structured logging
- Set appropriate log levels

## Authentication and Authorization

### JWT Token Configuration
- Use strong secret keys (256-bit minimum)
- Set appropriate token expiration (2 hours max)
- Include necessary claims only
- Validate all token parameters

### Function Authorization Levels
- Use `AuthorizationLevel.Anonymous` with custom auth
- Implement role-based access control
- Validate permissions for each operation

## Database Security

### Connection Security
- Use encrypted connections (SSL/TLS)
- Avoid `TrustServerCertificate=true` in production
- Use connection pooling limits
- Implement database firewalls

### Data Protection
- Encrypt sensitive data at rest
- Use parameterized queries (EF Core handles this)
- Implement proper backup encryption
- Follow principle of least privilege

## Infrastructure Security (Terraform)

### Network Configuration
```hcl
# Disable public access
public_network_access_enabled = false

# Configure private endpoints
private_endpoint_enabled = true

# Enable auditing
auditing_enabled = true
```

### Security Monitoring
- Enable Azure Security Center
- Configure security policies
- Set up automated responses
- Regular vulnerability assessments

## Compliance Considerations

### GDPR Compliance
- Data encryption at rest and in transit
- Right to erasure implementation
- Data processing logging
- Privacy by design

### Industry Standards
- Follow OWASP guidelines
- Implement security controls framework
- Regular security audits
- Penetration testing

## Emergency Procedures

### Security Incident Response
1. Isolate affected systems
2. Preserve evidence
3. Notify stakeholders
4. Implement fixes
5. Monitor for recurrence
6. Post-incident review

### Contact Information
- Security Team: security@hotshotlogistics.com
- Emergency: +1-555-SECURITY
- On-call rotation: Check internal documentation