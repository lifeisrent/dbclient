# Quick Reference: Admin/User Mode

## 🔑 Admin Password
```
Default: admin123
```

## 🎯 Quick Actions

### Switch to Admin Mode
1. Click `🔐 Admin Login`
2. Enter password
3. Click OK

### Add Table for Users
1. Be in Admin Mode
2. Select table from list
3. Click `➕ Add for Users`

### Remove Table from Users
1. Be in Admin Mode
2. Select table from list
3. Click `➖ Remove`

### Switch to User Mode
- Click `↩️ Switch to User`

## 📊 Mode Indicators

| Indicator | Meaning |
|-----------|---------|
| 👤 User Mode | Limited table access |
| 🔑 Admin Mode | Full access + management |

## 🔘 Button Visibility

### User Mode
- ✅ `🔐 Admin Login` button visible
- ❌ Admin management buttons hidden

### Admin Mode
- ✅ `➕ Add for Users` button visible
- ✅ `➖ Remove` button visible
- ✅ `↩️ Switch to User` button visible
- ❌ `🔐 Admin Login` button hidden

## 💾 Persistence

**Config File:** `user_config.json`
- Created automatically
- Saved on app close
- Loaded on app start

## ⚠️ Important Notes

1. **Default User View**: Users see NO tables until admin grants access
2. **Admin Password**: Change in `Form1.cs` line 14
3. **Config Location**: Same folder as executable
4. **Security**: Password is hardcoded (dev/internal use only)

## 📝 Example Workflow

### First Time Setup
```
1. Launch app → User Mode (no tables)
2. Click "Admin Login" → Enter password
3. Now in Admin Mode → See all tables
4. Select "DCP" → Click "Add for Users"
5. Select "Table1" → Click "Add for Users"
6. Click "Switch to User" → See only DCP and Table1
7. Close app → Settings saved
```

### Subsequent User Launch
```
1. Launch app → User Mode
2. See DCP and Table1 (previously allowed)
3. Click table → View data
```

## 🚨 Troubleshooting

| Problem | Solution |
|---------|----------|
| Forgot password | Edit `Form1.cs` line 14 |
| No tables visible | Admin must add tables first |
| Changes not saved | Check write permissions on folder |
| Config corrupted | Delete `user_config.json`, restart |

## 📖 Full Documentation

For complete details, see:
- [ADMIN_MODE_GUIDE.md](ADMIN_MODE_GUIDE.md) - Full feature documentation
- [README.md](README.md) - General application info
- [QUICK_START.md](QUICK_START.md) - Installation and basic usage
