# FlightService Configuration Setup

## ⚠️ Important: Configuration Files

The `appsettings.json` file contains **sensitive credentials** and is **NOT included** in the repository for security reasons.

## 🛠️ Setup Instructions

### 1. Copy the Template File

```bash
cd FlightService
cp appsettings.Template.json appsettings.json
```

### 2. Update `appsettings.json` with Your Credentials

Open `FlightService/appsettings.json` and replace the placeholders:

```json
{
  "AWS": {
    "Region": "us-east-1",  // Your AWS region
    "UseSecretsManager": true,
    "SecretName": "YOUR-AWS-SECRET-NAME",  // Your AWS Secrets Manager secret name
    "DatabaseHost": "YOUR-AURORA-ENDPOINT.rds.amazonaws.com",  // Your Aurora endpoint
    "DatabasePort": 5432,
    "DatabaseName": "YOUR-DATABSE-NAME"  // Your database name
  },
  "JwtSettings": {
    "SecretKey": "YOUR-JWT-SECRET-KEY-HERE-MINIMUM-32-CHARACTERS",  // Generate a strong key
    "Issuer": "FlightService",
    "Audience": "FlightServiceClients",
    "ExpiryMinutes": 60
  }
}
```

### 3. Find Your AWS Information

#### AWS Region
Look at your AWS Console URL: `https://console.aws.amazon.com/rds/home?region=us-east-1`

#### AWS Secret Name
1. Go to [AWS Secrets Manager](https://console.aws.amazon.com/secretsmanager/)
2. Find your RDS secret (usually starts with `rds!db-`)
3. Copy the secret name

#### Aurora Endpoint
1. Go to [AWS RDS Console](https://console.aws.amazon.com/rds/)
2. Click on **Databases**
3. Select your Aurora cluster
4. Copy the **Writer endpoint**

### 4. Generate JWT Secret Key

Use PowerShell to generate a secure key:

```powershell
-join ((65..90) + (97..122) + (48..57) | Get-Random -Count 32 | ForEach-Object {[char]$_})
```

Or use any password generator to create a 32+ character random string.

### 5. Configure AWS Credentials

```bash
aws configure
```

Enter:
- AWS Access Key ID
- AWS Secret Access Key
- Default region name
- Default output format (press Enter)

### 6. Run the Application

```bash
dotnet run
```

## 📁 Configuration Files Explained

- `appsettings.Template.json` - Template with placeholders (committed to Git)
- `appsettings.json` - Your actual config with secrets (**NOT committed to Git**)
- `appsettings.Development.json` - Development-specific settings (safe to commit)
- `appsettings.Production.json` - Production-specific settings (safe to commit)

## 🔒 Security Notes

- **Never commit** `appsettings.json` with real credentials
- **Never share** your JWT secret key
- **Never expose** AWS secret names publicly
- **Rotate secrets** regularly (every 90 days)
- Use AWS Secrets Manager for production deployments

## ✅ Verification

Test your setup:

```bash
# Test database connection
curl http://localhost:5126/api/DatabaseTest/connection

# Expected response
{
  "status": "success",
  "message": "Database connection is working!"
}
```

## 🆘 Troubleshooting

### "Cannot connect to database"
- Check your Aurora endpoint is correct
- Verify Security Group allows port 5432
- Ensure Aurora is publicly accessible (for local development)

### "Cannot find AWS secret"
- Verify secret name is correct
- Check AWS region matches
- Ensure AWS credentials are configured

### "JWT token invalid"
- Verify JWT SecretKey is at least 32 characters
- Check Issuer and Audience match in your client
