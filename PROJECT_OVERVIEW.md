# Database Client Projects Overview

## Project Structure

```
dbclient/
├── dbclient/              # Console Application (Original)
│   ├── Program.cs         # Console app that queries DCP table
│   └── dbclient.csproj
│
└── DBViewer/              # WinForms Application (NEW)
    ├── Form1.cs           # Main form logic
    ├── Form1.Designer.cs  # UI design
    ├── Program.cs         # App entry point
    ├── DBViewer.csproj
    └── README.md
```

## Console Application (dbclient)

**Purpose**: Simple console application for testing database connectivity

**Key Features**:
- Connects to `ASUS\MONITORTF` SQL Server
- Queries `REMOTE2.NSDB.dbo.DCP` table
- Displays the `id` column from the first 10 rows
- Uses Microsoft.Data.SqlClient

**Usage**:
```bash
cd dbclient
dotnet run
```

**Output Example**:
```
Hello, World!
✅ MSSQL 연결 성공
ID: 1
ID: 2
ID: 3
...
```

## WinForms Application (DBViewer)

**Purpose**: Full-featured database table browser with GUI

**Key Features**:
- Graphical user interface
- Lists ALL tables from `REMOTE2.NSDB`
- Click any table to view all its data
- DataGridView with all columns and rows
- Status indicators and error handling

**Usage**:
```bash
cd DBViewer
dotnet run
```

**UI Layout**:
```
┌─────────────────────────────────────────────────────────────┐
│ Status: ✅ Connected - 15 tables found                      │
├────────────────┬────────────────────────────────────────────┤
│ Tables         │ Data Preview                               │
├────────────────┼────────────────────────────────────────────┤
│ ○ DCP         │ ┌──────┬──────────┬───────┬──────────────┐ │
│   Table1      │ │  ID  │   Name   │ Value │  Timestamp   │ │
│   Table2      │ ├──────┼──────────┼───────┼──────────────┤ │
│   ...         │ │  1   │  Test    │  100  │  2024-12-04  │ │
│               │ │  2   │  Sample  │  200  │  2024-12-04  │ │
│               │ └──────┴──────────┴───────┴──────────────┘ │
└────────────────┴────────────────────────────────────────────┘
```

## Connection Details

Both applications use the same connection settings:

```csharp
DataSource = "ASUS\\MONITORTF"
UserID = "sa"
Password = "Dosu123$"
Database = REMOTE2.NSDB (via linked server)
```

## When to Use Which

| Scenario | Use Console App | Use WinForms App |
|----------|----------------|------------------|
| Quick connectivity test | ✅ | ❌ |
| Automated scripts | ✅ | ❌ |
| Browse multiple tables | ❌ | ✅ |
| View full table data | ❌ | ✅ |
| User-friendly interface | ❌ | ✅ |
| Export/analyze data visually | ❌ | ✅ |
