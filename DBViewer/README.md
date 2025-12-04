# Database Table Viewer - WinForms Application

## Overview
This is a Windows Forms application that connects to the `REMOTE2.NSDB` database and provides a graphical interface to browse and view table data.

## Features

### Core Functionality

1. **Automatic Table Discovery**: Automatically loads all tables from the `REMOTE2.NSDB` database on startup
2. **Table List**: Displays available tables in a list on the left side
3. **Data Viewer**: Shows all column data from the selected table in a DataGridView on the right side
4. **Status Indicator**: Shows connection status, current mode, and loading progress
5. **Error Handling**: Displays user-friendly error messages if connection or data loading fails

### 🔐 Admin/User Mode (NEW!)

6. **User Mode (Default)**: 
   - Restricted table visibility
   - Users only see tables granted by admin
   - No tables visible by default
   
7. **Admin Mode**: 
   - Password-protected access (default: `admin123`)
   - View all database tables
   - Grant/revoke table access for users
   - Switch back to user mode
   
8. **Permission Persistence**: 
   - User table permissions saved to `user_config.json`
   - Settings persist across application restarts

📖 **See [ADMIN_MODE_GUIDE.md](ADMIN_MODE_GUIDE.md) for detailed admin/user mode documentation**

## Project Structure

```
DBViewer/
├── Form1.cs              # Main form logic and database operations
├── Form1.Designer.cs     # UI layout and controls
├── UserConfig.cs         # Configuration management and persistence
├── Program.cs            # Application entry point
├── DBViewer.csproj       # Project configuration
├── user_config.json      # Runtime config (auto-generated)
├── README.md             # This file
├── ADMIN_MODE_GUIDE.md   # Admin/User mode documentation
└── QUICK_START.md        # Quick start guide
```

## How to Use

1. **Run the Application**:
   ```bash
   cd DBViewer
   dotnet run
   ```

2. **View Tables**:
   - When the application starts, it automatically connects to the database
   - All tables from `REMOTE2.NSDB` are loaded into the left panel
   - The status label shows connection status and table count

3. **View Table Data**:
   - Click on any table name in the left list
   - All columns and rows from that table will be displayed in the DataGridView
   - Columns are automatically resized for better visibility
   - The status label shows the number of rows loaded

## Connection Details

The application connects to:
- **Server**: `ASUS\MONITORTF`
- **Database**: `REMOTE2.NSDB` (via linked server)
- **Authentication**: SQL Authentication
- **User**: `sa`

> **Note**: The connection string is the same as used in the console application (`dbclient/Program.cs`)

## Dependencies

- **.NET 10.0** with Windows Forms
- **Microsoft.Data.SqlClient** (6.1.3) - For SQL Server connectivity

## Comparison with Console App

| Feature | Console App | WinForms App |
|---------|------------|--------------|
| Purpose | Simple ID query from DCP table | Full table browser |
| Interface | Command line | Graphical UI |
| Tables | Single table (DCP) | All tables from NSDB |
| Data Display | Console output (ID only) | DataGridView (all columns) |
| User Interaction | None | Click to select tables |

## Future Enhancements

Potential improvements could include:
- Search/filter functionality
- Export data to CSV or Excel
- Query builder interface
- Connection string configuration UI
- Multiple database support
