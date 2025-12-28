# Security Policy

## Reporting Security Issues

If you discover a security vulnerability in SmartAirport, please email the maintainers directly rather than opening a public issue.

## Security Best Practices Implemented

### 1. Credentials Management
? AWS Secrets Manager for database credentials  
? No hardcoded passwords in code  
? JWT secrets stored securely  
? Credentials excluded from version control  

### 2. Logging Security
? No sensitive data in logs (passwords, tokens, connection strings)  
? Generic error messages in production  
? Environment-specific logging levels  
? Minimal AWS infrastructure details exposed  

### 3. API Security
? JWT Bearer token authentication  
? CORS configured  
? HTTPS enforcement  
? API versioning  
? Health check endpoints  

### 4. Database Security
? SSL/TLS connections required  
? Parameterized queries (EF Core)  
? Connection pooling  
? Migrations for schema management  

### 5. Configuration Security
? Template files instead of real credentials in Git  
? Environment-specific configuration files  
? .gitignore rules for sensitive files  

## Configuration Files

- `appsettings.Template.json` - Safe template (in Git)
- `appsettings.json` - Real credentials (NOT in Git)
- `appsettings.Development.json` - Dev settings (safe to commit)
- `appsettings.Production.json` - Prod settings (safe to commit)

## Secure Setup

See [FlightService/CONFIGURATION.md](FlightService/CONFIGURATION.md) for detailed setup instructions.

## Security Checklist

Before deploying to production:

- [ ] All secrets in AWS Secrets Manager
- [ ] JWT secret key is strong (32+ characters)
- [ ] AWS IAM policies follow least privilege
- [ ] Database security groups properly configured
- [ ] SSL/TLS certificates are valid
- [ ] Logging level set to Warning or Error
- [ ] CORS policy restricted to known origins
- [ ] API rate limiting configured (if applicable)
- [ ] Regular security updates applied

## Vulnerability Disclosure

We take security seriously. If you believe you've found a security vulnerability:

1. **DO NOT** open a public GitHub issue
2. Email the project maintainers
3. Provide details of the vulnerability
4. Allow reasonable time for a fix before public disclosure

## Security Updates

- Secrets should be rotated every 90 days
- Dependencies should be updated regularly
- Security patches should be applied promptly

---

**Last Updated:** 2024-12-27  
**Version:** 1.0
