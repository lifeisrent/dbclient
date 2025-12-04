# Quick Start Guide

## Running the WinForms Database Viewer

### Option 1: Using dotnet CLI
```bash
cd c:\001.workspace\code\DB\dbclient\DBViewer
dotnet run
```

### Option 2: Build and Run Executable
```bash
cd c:\001.workspace\code\DB\dbclient\DBViewer
dotnet build
.\bin\Debug\net10.0-windows\DBViewer.exe
```

### Option 3: From Visual Studio
1. Open `dbclient.slnx` in Visual Studio
2. Set `DBViewer` as the startup project
3. Press F5 to run

## First Time Setup

No additional setup is required! The application will:
1. Automatically connect to `ASUS\MONITORTF` server
2. Query the `REMOTE2.NSDB` database
3. Load all available tables
4. Display them in the left panel

## Using the Application

### Step 1: Launch
Run the application using one of the methods above.

### Step 2: Wait for Connection
- The status label will show "Connecting to database..."
- Once connected: "✅ Connected - X tables found"

### Step 3: Browse Tables
- All tables from `REMOTE2.NSDB` appear in the left list
- Click any table name to view its data

### Step 4: View Data
- The right panel shows all columns and rows
- Scroll horizontally/vertically to see more data
- Columns are auto-sized for readability

## Troubleshooting

### "Connection Failed" Error
- Check that SQL Server is running
- Verify the server name is correct: `ASUS\MONITORTF`
- Ensure the `sa` account password is correct
- Confirm `REMOTE2` linked server is configured

### "No Tables Found"
- Verify `REMOTE2.NSDB` database exists
- Check that linked server permissions are correct

### Application Doesn't Start
- Ensure .NET 10.0 SDK is installed
- Run `dotnet --version` to verify
- Rebuild the project: `dotnet build`

## Code Modifications

### Change Database
Edit `Form1.cs`, line ~38:
```csharp
string query = @"
    SELECT TABLE_NAME 
    FROM YOUR_DATABASE.INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_TYPE = 'BASE TABLE'
    ORDER BY TABLE_NAME;
";
```

### Change Connection String
Edit `Form1.cs`, line ~15-21:
```csharp
var builder = new SqlConnectionStringBuilder
{
    DataSource = "YOUR_SERVER",
    UserID = "YOUR_USER",
    Password = "YOUR_PASSWORD",
    Encrypt = true,
    TrustServerCertificate = true
};
```

### Add Features
The code is well-structured for extension:
- `LoadTableNamesAsync()` - Modify to change table listing
- `LoadTableDataAsync()` - Modify to change data display
- `Form1.Designer.cs` - Modify to change UI layout

## Performance Tips

- Large tables may take time to load
- Consider adding pagination for tables with many rows
- Use filtering/search for better performance with large datasets
