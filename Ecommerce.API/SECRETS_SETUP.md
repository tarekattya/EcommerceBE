# User Secrets Setup Guide

Sensitive configuration values (like email passwords) are stored in **User Secrets** instead of `appsettings.json` for security.

## Current Secrets (Already Configured)

Your secrets are already set up. To view them:

```bash
cd Ecommerce.API
dotnet user-secrets list
```

## Setting/Updating Secrets

### Set Email Settings:

```bash
cd Ecommerce.API

# Set email address
dotnet user-secrets set "MailSettings:Email" "your-email@gmail.com"

# Set email password (use App Password for Gmail)
dotnet user-secrets set "MailSettings:Password" "your-app-password"

# Set SMTP host (optional, defaults to smtp.gmail.com)
dotnet user-secrets set "MailSettings:Host" "smtp.gmail.com"

# Set SMTP port (optional, defaults to 587)
dotnet user-secrets set "MailSettings:Port" "587"

# Set display name (optional)
dotnet user-secrets set "MailSettings:DisplayName" "Ecommerce Store"
```

### View All Secrets:

```bash
dotnet user-secrets list
```

### Remove a Secret:

```bash
dotnet user-secrets remove "MailSettings:Email"
```

### Clear All Secrets:

```bash
dotnet user-secrets clear
```

## Gmail Setup

1. Enable **2-Step Verification** on your Google account
2. Generate an **App Password**: https://myaccount.google.com/apppasswords
3. Use the 16-character app password (not your regular password)

## Other Email Providers

- **Outlook/Hotmail**: `smtp-mail.outlook.com`, Port `587`
- **Yahoo**: `smtp.mail.yahoo.com`, Port `587`
- **Custom SMTP**: Use your provider's settings

## Notes

- User secrets are **only available in Development** environment
- For **Production**, use environment variables or Azure Key Vault
- Secrets are stored locally and never committed to git
