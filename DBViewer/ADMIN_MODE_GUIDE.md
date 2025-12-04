# Admin/User Mode Feature Documentation

## Overview

The Database Table Viewer now includes **Admin** and **User** mode functionality with password protection and table permission management.

## Default Behavior

- **Application starts in User Mode** by default
- **Users see NO tables** unless an admin has granted access
- User permissions persist across application restarts

## Modes

### 👤 User Mode (Default)

**Characteristics:**
- Limited table visibility
- Can only see tables that admin has allowed
- No table management capabilities
- Cannot switch to admin mode without password

**UI Elements:**
- Status shows: `👤 User Mode - X allowed tables`
- Only "🔐 Admin Login" button visible
- Table list shows only allowed tables

### 🔑 Admin Mode

**Characteristics:**
- Full database access
- Can see ALL tables from REMOTE2.NSDB
- Can add/remove tables for user mode
- Can switch back to user mode

**UI Elements:**
- Status shows: `🔑 Admin Mode - X tables available`
- Buttons visible:
  - `➕ Add for Users` - Grant table access to users
  - `➖ Remove` - Revoke table access from users
  - `↩️ Switch to User` - Return to user mode

## Password

**Default Admin Password:** `admin123`

To change the password, edit `Form1.cs` line 14:
```csharp
private const string ADMIN_PASSWORD = "your_new_password";
```

## How to Use

### As a User

1. **Launch Application**
   - App opens in User Mode
   - May see no tables (if admin hasn't granted access yet)

2. **View Allowed Tables**
   - Click any table name to view its data
   - Only tables added by admin are visible

### As an Admin

1. **Login to Admin Mode**
   - Click `🔐 Admin Login` button
   - Enter password: `admin123`
   - Click OK

2. **Add Tables for Users**
   - Select a table from the list
   - Click `➕ Add for Users`
   - Confirmation message appears
   - Users will now see this table

3. **Remove Table Access**
   - Select a table that users currently have access to
   - Click `➖ Remove`
   - Confirmation message appears
   - Users will no longer see this table

4. **Return to User Mode**
   - Click `↩️ Switch to User`
   - View changes as users see them

## Persistence

### Configuration File

User table permissions are saved in: `user_config.json`

**Location:** Same directory as the executable

**Format:**
```json
{
  "AllowedTables": [
    "DCP",
    "Table1",
    "Table2"
  ]
}
```

### When Data is Saved

- Automatically on application close
- Data persists in `user_config.json`

### When Data is Loaded

- Automatically on application start
- Empty list if no config file exists

## Security Notes

⚠️ **Important Security Information:**

1. **Password is hardcoded** in source code
   - Good for development/internal tools
   - NOT suitable for production security
   - Anyone with source access can see the password

2. **No encryption** on config file
   - `user_config.json` is plain text
   - Can be manually edited

3. **Recommended for:**
   - Internal tools
   - Trusted environments
   - Development/testing scenarios

4. **NOT recommended for:**
   - Public-facing applications
   - Systems with sensitive data access control
   - Multi-user enterprise environments

### Production Security Improvements

If deploying to production, consider:
- Move password to encrypted configuration
- Implement proper user authentication system
- Add user roles and permissions database
- Enable audit logging for admin actions
- Encrypt the config file

## Visual Differences

### User Mode Display
```
┌─────────────────────────────────────────────────┐
│ Status: 👤 User Mode - 2 allowed tables         │
│ Mode: 👤 User Mode            [🔐 Admin Login]  │
├──────────────┬──────────────────────────────────┤
│ Tables       │ Data Preview                     │
├──────────────┼──────────────────────────────────┤
│ ○ DCP       │                                   │
│   Table1    │                                   │
│             │                                   │
└──────────────┴──────────────────────────────────┘
```

### Admin Mode Display
```
┌──────────────────────────────────────────────────────────┐
│ Status: 🔑 Admin Mode - 15 tables available              │
│ Mode: 🔑 Admin Mode                  [↩️ Switch to User] │
├──────────────┬───────────────────────────────────────────┤
│ Tables       │ Data Preview                              │
├──────────────┼───────────────────────────────────────────┤
│ ○ DCP       │                 [➕ Add]  [➖ Remove]      │
│   Table1    │                                            │
│   Table2    │                                            │
│   ...       │                                            │
└──────────────┴────────────────────────────────────────────┘
```

## Workflow Example

### Initial Setup (First Run)

1. Admin launches app → sees all tables in admin mode (NO - starts in user mode)
2. Admin clicks "🔐 Admin Login" → enters password
3. Admin selects tables to allow for users
4. Admin clicks "➕ Add for Users" for each table
5. Admin clicks "↩️ Switch to User" to verify user view
6. Admin closes app → settings saved

### User Experience (After Setup)

1. User launches app → sees only allowed tables
2. User clicks table → views data
3. No access to admin features

### Admin Updates Permissions

1. Admin launches app in user mode
2. Admin clicks "🔐 Admin Login"
3. Admin adds/removes tables as needed
4. Changes immediately reflected in user mode
5. Settings saved on close

## Troubleshooting

### "I forgot the admin password"

Edit `Form1.cs` line 14 and recompile:
```csharp
private const string ADMIN_PASSWORD = "new_password";
```

### "Config file is corrupted"

Delete `user_config.json` and restart the app. A new empty config will be created.

### "Users can't see any tables"

Admin must:
1. Login to admin mode
2. Select tables
3. Click "➕ Add for Users"
4. Close app to save settings

### "Tables disappear after restart"

Check if `user_config.json` exists and has correct permissions. The app must be able to write to the current directory.

## File Summary

- **Form1.cs** - Main logic with mode switching
- **Form1.Designer.cs** - UI layout with admin controls
- **UserConfig.cs** - Configuration persistence class
- **user_config.json** - Runtime config file (auto-generated)
