# Admin/User Mode - Visual Guide

## 🎨 UI Layout Overview

### User Mode (Default State)
```
╔═══════════════════════════════════════════════════════════════════════╗
║ Database Table Viewer - REMOTE2.NSDB                                 ║
╠═══════════════════════════════════════════════════════════════════════╣
║ Status: 👤 User Mode - 0 allowed tables                   Mode: 👤 User Mode ║
╠═══════════════════════════════════════════════════════════════════════╣
║                                                                       ║
║  ┌─────────────────┐  ┌────────────────────────────────────────┐    ║
║  │ Tables          │  │ Data Preview                           │    ║
║  │                 │  │                                        │    ║
║  │ (no tables)     │  │ (select a table to view data)          │    ║
║  │                 │  │                                        │    ║
║  │                 │  │                                        │    ║
║  │                 │  │                                        │    ║
║  │                 │  │                                        │    ║
║  └─────────────────┘  └────────────────────────────────────────┘    ║
║                                                                       ║
║                                              [🔐 Admin Login]         ║
╚═══════════════════════════════════════════════════════════════════════╝
```

### Admin Mode (After Login)
```
╔═══════════════════════════════════════════════════════════════════════╗
║ Database Table Viewer - REMOTE2.NSDB                                 ║
╠═══════════════════════════════════════════════════════════════════════╣
║ Status: 🔑 Admin Mode - 15 tables available           Mode: 🔑 Admin Mode ║
╠═══════════════════════════════════════════════════════════════════════╣
║                                                                       ║
║  ┌─────────────────┐  ┌────────────────────────────────────────┐    ║
║  │ Tables          │  │ Data Preview                           │    ║
║  │ ○ DCP          │  │ ┌───┬─────┬──────┬──────────────┐      │    ║
║  │   Table1       │  │ │ID │Name │Value │  Timestamp   │      │    ║
║  │   Table2       │  │ ├───┼─────┼──────┼──────────────┤      │    ║
║  │   Table3       │  │ │1  │Test │ 100  │ 2024-12-04   │      │    ║
║  │   ...          │  │ │2  │Demo │ 200  │ 2024-12-04   │      │    ║
║  │                 │  │ └───┴─────┴──────┴──────────────┘      │    ║
║  └─────────────────┘  └────────────────────────────────────────┘    ║
║                                                                       ║
║                     [➕ Add for Users] [➖ Remove] [↩️ Switch to User] ║
╚═══════════════════════════════════════════════════════════════════════╝
```

## 🔄 Mode Switching Flow

```
┌─────────────┐
│  App Start  │
└──────┬──────┘
       │
       ▼
┌─────────────────────┐
│   👤 User Mode      │
│  (No tables shown)  │
└──────┬──────────────┘
       │
       │ Click "🔐 Admin Login"
       ▼
┌─────────────────────┐
│ Password Prompt     │
│ ┌─────────────────┐ │
│ │ admin123        │ │
│ └─────────────────┘ │
│   [OK]  [Cancel]    │
└──────┬──────────────┘
       │
       │ Correct Password
       ▼
┌─────────────────────┐
│   🔑 Admin Mode     │
│  (All tables shown) │
│                     │
│ [➕] Add for Users  │
│ [➖] Remove         │
│ [↩️] Switch to User │
└──────┬──────────────┘
       │
       │ Click "↩️ Switch to User"
       ▼
┌─────────────────────┐
│   👤 User Mode      │
│ (Allowed tables)    │
└─────────────────────┘
```

## 🔐 Admin Actions Flow

### Adding a Table for Users

```
Admin Mode
    │
    ├─→ Select "DCP" from list
    │
    ├─→ Click "➕ Add for Users"
    │
    ├─→ 📝 Confirmation Dialog:
    │   "Table 'DCP' added to user mode.
    │    Users will now see this table."
    │   [OK]
    │
    └─→ DCP added to user_config.json
```

### Removing a Table

```
Admin Mode
    │
    ├─→ Select "Table1" from list
    │
    ├─→ Click "➖ Remove"
    │
    ├─→ 📝 Confirmation Dialog:
    │   "Table 'Table1' removed from user mode.
    │    Users will no longer see this table."
    │   [OK]
    │
    └─→ Table1 removed from user_config.json
```

## 💾 Data Persistence Flow

```
┌─────────────────┐
│   App Startup   │
└────────┬────────┘
         │
         ├─→ Load user_config.json
         │   ├─ If exists → Load allowed tables
         │   └─ If not → Create empty list
         │
         ▼
┌─────────────────┐
│ Display Tables  │
│ based on mode   │
└────────┬────────┘
         │
         │ User interacts...
         │ Admin adds/removes tables...
         │
         ▼
┌─────────────────┐
│  App Closing    │
└────────┬────────┘
         │
         └─→ Save user_config.json
             {
               "AllowedTables": [
                 "DCP",
                 "Table1"
               ]
             }
```

## 🎯 Button State Matrix

| Mode | Admin Login | Switch to User | Add for Users | Remove |
|------|-------------|----------------|---------------|--------|
| User | ✅ Visible  | ❌ Hidden      | ❌ Hidden     | ❌ Hidden |
| Admin| ❌ Hidden   | ✅ Visible     | ✅ Visible    | ✅ Visible |

## 🎨 Color Coding

| Element | User Mode | Admin Mode |
|---------|-----------|------------|
| Mode Label | 👤 **Green** (DarkGreen) | 🔑 **Red** (DarkRed) |
| Status Text | 👤 prefix | 🔑 prefix |

## 📊 Status Messages

### User Mode
```
👤 User Mode - 0 allowed tables          (no tables configured)
👤 User Mode - 3 allowed tables          (3 tables configured)
👤 Showing 150 rows from DCP             (viewing table data)
```

### Admin Mode
```
🔑 Admin Mode - 15 tables available      (connected to database)
🔑 Showing 150 rows from DCP             (viewing table data)
```

## 🔔 Notification Messages

### Success Messages
```
✓ Table 'DCP' added to user mode.
  Users will now see this table.

✓ Table 'Table1' removed from user mode.
  Users will no longer see this table.
```

### Error Messages
```
✗ Incorrect password!

✗ Please select a table to add for user mode.

✗ Please select a table to remove from user mode.
```

### Info Messages
```
ℹ Table 'DCP' is already available to users.

ℹ Table 'Table1' is not in user mode.
```

## 📁 File Structure After Running

```
DBViewer/
├── bin/
│   └── Debug/
│       └── net10.0-windows/
│           ├── DBViewer.dll
│           ├── DBViewer.exe
│           └── user_config.json  ← Created at runtime
├── Form1.cs
├── Form1.Designer.cs
├── UserConfig.cs
├── Program.cs
└── DBViewer.csproj
```

## 🎬 Complete Usage Scenario

```
FIRST TIME ADMIN SETUP
─────────────────────────────────────────
1. 🚀 Launch app
   → Shows User Mode with NO tables

2. 🔐 Click "Admin Login"
   → Password dialog appears

3. ⌨️  Enter "admin123"
   → Switches to Admin Mode
   → All 15 database tables visible

4. 📋 Select "DCP"
   → Click "➕ Add for Users"
   → Confirmation: "Table 'DCP' added"

5. 📋 Select "Table1"
   → Click "➕ Add for Users"
   → Confirmation: "Table 'Table1' added"

6. 👁️  Click "↩️ Switch to User"
   → Returns to User Mode
   → Now shows DCP and Table1

7. 💾 Close app
   → Saves to user_config.json

SUBSEQUENT USER SESSION
─────────────────────────────────────────
1. 🚀 Launch app
   → Shows User Mode
   → DCP and Table1 visible

2. 🖱️  Click "DCP"
   → Loads data in grid
   → Shows: "👤 Showing 150 rows from DCP"

3. 🖱️  Click "Table1"
   → Loads different data
   → Shows: "👤 Showing 75 rows from Table1"
```
