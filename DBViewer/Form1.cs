using Microsoft.Data.SqlClient;
using System.Data;

namespace DBViewer;

public partial class Form1 : Form
{
    private SqlConnection? connection;
    private readonly string connectionString;
    private bool isAdminMode = false;
    private List<string> allTables = new();
    private UserConfig userConfig = new();
    
    private const string ADMIN_PASSWORD = "admin123";

    public Form1()
    {
        InitializeComponent();

        // Using the same connection string from the console app
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = "ASUS\\MONITORTF",
            UserID = "sa",
            Password = "Dosu123$",
            Encrypt = true,
            TrustServerCertificate = true
        };
        connectionString = builder.ConnectionString;
        
        // Add hover effects to buttons
        SetupButtonHoverEffects();
    }
    
    private void SetupButtonHoverEffects()
    {
        // Admin Login button hover
        btnAdminLogin.MouseEnter += (s, e) => { btnAdminLogin.BackColor = Color.FromArgb(41, 128, 185); };
        btnAdminLogin.MouseLeave += (s, e) => { btnAdminLogin.BackColor = Color.FromArgb(52, 152, 219); };
        
        // Switch to User button hover
        btnSwitchToUser.MouseEnter += (s, e) => { btnSwitchToUser.BackColor = Color.FromArgb(192, 57, 43); };
        btnSwitchToUser.MouseLeave += (s, e) => { btnSwitchToUser.BackColor = Color.FromArgb(231, 76, 60); };
        
        // Add Table button hover
        btnAddTable.MouseEnter += (s, e) => { btnAddTable.BackColor = Color.FromArgb(39, 174, 96); };
        btnAddTable.MouseLeave += (s, e) => { btnAddTable.BackColor = Color.FromArgb(46, 204, 113); };
        
        // Remove Table button hover
        btnRemoveTable.MouseEnter += (s, e) => { btnRemoveTable.BackColor = Color.FromArgb(192, 57, 43); };
        btnRemoveTable.MouseLeave += (s, e) => { btnRemoveTable.BackColor = Color.FromArgb(231, 76, 60); };
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
        // Load user configuration
        userConfig = UserConfig.Load();
        
        // Set to user mode by default
        SwitchToUserMode();
        
        // Load all tables from database
        await LoadAllTablesAsync();

        // Setup dynamic UI elements
        SetupDynamicUI();
    }

    private void SetupDynamicUI()
    {
        // Create a new button for updating the Monitor table
        var btnUpdateMonitor = new Button
        {
            Text = "Update Monitor Table",
            Size = new Size(150, 30),
            Location = new Point(btnAddTable.Left - 160, btnAddTable.Top),
            BackColor = Color.FromArgb(142, 68, 173), // Purple
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        
        btnUpdateMonitor.FlatAppearance.BorderSize = 0;
        btnUpdateMonitor.MouseEnter += (s, e) => { btnUpdateMonitor.BackColor = Color.FromArgb(155, 89, 182); };
        btnUpdateMonitor.MouseLeave += (s, e) => { btnUpdateMonitor.BackColor = Color.FromArgb(142, 68, 173); };
        
        btnUpdateMonitor.Click += async (s, e) => await CreateMonitorTableAsync();
        
        this.Controls.Add(btnUpdateMonitor);
        
        // Ensure it appears on top in Z-order if needed, though adding last usually puts it at top
        btnUpdateMonitor.BringToFront();
    }

    private async Task CreateMonitorTableAsync()
    {
        if (connection == null || connection.State != ConnectionState.Open)
        {
            MessageBox.Show("Database not connected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            // Try to switch to NSDB locally if it exists, to avoid creating tables in master
            // This is a best-effort attempt.
            try 
            {
                if (connection.Database == "master")
                {
                    await using var cmdCheck = new SqlCommand("SELECT database_id FROM sys.databases WHERE name = 'NSDB'", connection);
                    if (await cmdCheck.ExecuteScalarAsync() != null)
                    {
                        connection.ChangeDatabase("NSDB");
                    }
                }
            } 
            catch { /* Ignore if switching fails, proceed in current DB */ }

            labelStatus.Text = "Updating MONITOR_DCP table...";
            
            var currentMonth = DateTime.Now;
            string tableName = $"H01RAW_{currentMonth:yyyyMM}";
            
            // Check availability - using remote check
            if (!await TableExistsAsync(tableName))
            {
               var prevMonth = currentMonth.AddMonths(-1);
               var prevTableName = $"H01RAW_{prevMonth:yyyyMM}";
               if (await TableExistsAsync(prevTableName))
               {
                   tableName = prevTableName;
               }
               else
               {
                   tableName = "H01RAW_202511"; 
               }
            }

            var confirm = MessageBox.Show($"Create/Update MONITOR_DCP from {tableName}?\n\nTarget Database: {connection.Database}\nSource: REMOTE2.NSDB", 
                                          "Confirm Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            // SQL Logic:
            // 1. Drop existing MONITOR_DCP in LOCAL DB
            // 2. Insert distinct Data from REMOTE DB
            
            string query = $@"
                IF OBJECT_ID('dbo.MONITOR_DCP', 'U') IS NOT NULL
                    DROP TABLE dbo.MONITOR_DCP;

                WITH RankedData AS (
                    SELECT 
                        R.*,
                        ROW_NUMBER() OVER (PARTITION BY R.DCP_ID ORDER BY R.DT DESC) as rn
                    FROM REMOTE2.NSDB.dbo.[{tableName}] R
                )
                SELECT 
                    D.Name,
                    D.ID, 
                    R.*
                INTO dbo.MONITOR_DCP
                FROM RankedData R
                LEFT JOIN REMOTE2.NSDB.dbo.DCP D ON R.DCP_ID = D.ID
                WHERE R.rn = 1
                ORDER BY R.DT DESC, R.DCP_ID ASC;
            ";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.CommandTimeout = 300; 
                await cmd.ExecuteNonQueryAsync();
            }

            labelStatus.Text = $"✅ Created MONITOR_DCP from {tableName} in {connection.Database}!";
            MessageBox.Show($"Successfully created MONITOR_DCP in database '{connection.Database}'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Refresh list might not show the new table if we are only listing REMOTE tables
            // But the user might want to see it.
            // existing LoadAllTablesAsync queries REMOTE2.NSDB.INFORMATION_SCHEMA...
            // So if we created it LOCALLY, it won't show up in the list unless we change LoadAllTablesAsync.
            // For now, I will not change LoadAllTablesAsync logic to avoid breaking existing view, 
            // but I will notify the user.
        }
        catch (Exception ex)
        {
            labelStatus.Text = $"❌ Error: {ex.Message}";
            MessageBox.Show($"Failed to update MONITOR_DCP:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task<bool> TableExistsAsync(string tableName)
    {
        try 
        {
            // Check existence in REMOTE DB
            string query = $"SELECT COUNT(*) FROM REMOTE2.NSDB.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);
                int count = (int)(await cmd.ExecuteScalarAsync() ?? 0);
                return count > 0;
            }
        } 
        catch 
        { 
            return false; 
        }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Save user configuration on exit
        userConfig.Save();
    }

    private async Task LoadAllTablesAsync()
    {
        try
        {
            labelStatus.Text = "Connecting to database...";
            connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            labelStatus.Text = "Loading table names...";

            // Query to get all user tables from REMOTE2.NSDB
            string query = @"
                SELECT TABLE_NAME 
                FROM REMOTE2.NSDB.INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_TYPE = 'BASE TABLE'
                ORDER BY TABLE_NAME;
            ";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                allTables.Clear();
                while (await reader.ReadAsync())
                {
                    var tableName = reader["TABLE_NAME"].ToString();
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        allTables.Add(tableName);
                    }
                }
            }

            // Refresh the table list based on current mode
            RefreshTableList();
            
            labelStatus.Text = $"✅ Connected - {allTables.Count} total tables";
        }
        catch (Exception ex)
        {
            labelStatus.Text = $"❌ Error: {ex.Message}";
            MessageBox.Show($"Failed to connect to database:\n{ex.Message}", "Connection Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void RefreshTableList()
    {
        listBoxTables.Items.Clear();
        
        if (isAdminMode)
        {
            // Admin sees all tables
            foreach (var table in allTables)
            {
                listBoxTables.Items.Add(table);
            }
            labelStatus.Text = $"🔑 Admin Mode - {allTables.Count} tables available";
        }
        else
        {
            // User sees only allowed tables
            foreach (var table in allTables)
            {
                if (userConfig.AllowedTables.Contains(table))
                {
                    listBoxTables.Items.Add(table);
                }
            }
            labelStatus.Text = $"👤 User Mode - {userConfig.AllowedTables.Count} allowed tables";
        }
    }

    private async void ListBoxTables_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (listBoxTables.SelectedItem == null || connection == null)
            return;

        string tableName = listBoxTables.SelectedItem.ToString()!;
        await LoadTableDataAsync(tableName);
    }

    private async Task LoadTableDataAsync(string tableName)
    {
        try
        {
            labelStatus.Text = $"Loading data from {tableName}...";

            // Query to get all data from selected table
            string query = $"SELECT * FROM REMOTE2.NSDB.dbo.[{tableName}];";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dataTable = new DataTable();
                    await Task.Run(() => adapter.Fill(dataTable));
                    
                    dataGridViewData.DataSource = dataTable;
                    
                    // Auto-resize columns for better visibility
                    dataGridViewData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                }
            }

            string modePrefix = isAdminMode ? "🔑" : "👤";
            labelStatus.Text = $"{modePrefix} Showing {dataGridViewData.Rows.Count} rows from {tableName}";
        }
        catch (Exception ex)
        {
            labelStatus.Text = $"❌ Error loading table: {ex.Message}";
            MessageBox.Show($"Failed to load table data:\n{ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnAdminLogin_Click(object sender, EventArgs e)
    {
        // Prompt for password
        string? password = PromptForPassword();
        
        if (password == ADMIN_PASSWORD)
        {
            SwitchToAdminMode();
        }
        else if (password != null) // null means user cancelled
        {
            MessageBox.Show("Incorrect password!", "Access Denied", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnSwitchToUser_Click(object sender, EventArgs e)
    {
        SwitchToUserMode();
    }

    private void BtnAddTable_Click(object sender, EventArgs e)
    {
        if (listBoxTables.SelectedItem == null)
        {
            MessageBox.Show("Please select a table to add for user mode.", "No Selection", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string tableName = listBoxTables.SelectedItem.ToString()!;
        
        if (!userConfig.AllowedTables.Contains(tableName))
        {
            userConfig.AllowedTables.Add(tableName);
            MessageBox.Show($"Table '{tableName}' added to user mode.\n\nUsers will now see this table.", 
                "Table Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            MessageBox.Show($"Table '{tableName}' is already available to users.", 
                "Already Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void BtnRemoveTable_Click(object sender, EventArgs e)
    {
        if (listBoxTables.SelectedItem == null)
        {
            MessageBox.Show("Please select a table to remove from user mode.", "No Selection", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string tableName = listBoxTables.SelectedItem.ToString()!;
        
        if (userConfig.AllowedTables.Contains(tableName))
        {
            userConfig.AllowedTables.Remove(tableName);
            MessageBox.Show($"Table '{tableName}' removed from user mode.\n\nUsers will no longer see this table.", 
                "Table Removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            MessageBox.Show($"Table '{tableName}' is not in user mode.", 
                "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void SwitchToAdminMode()
    {
        isAdminMode = true;
        
        // Update UI
        labelMode.Text = "🔑 Admin Mode";
        labelMode.ForeColor = Color.DarkRed;
        
        // Show/hide buttons
        btnAdminLogin.Visible = false;
        btnSwitchToUser.Visible = true;
        btnAddTable.Visible = true;
        btnRemoveTable.Visible = true;
        
        // Refresh table list to show all tables
        RefreshTableList();
    }

    private void SwitchToUserMode()
    {
        isAdminMode = false;
        
        // Update UI
        labelMode.Text = "👤 User Mode";
        labelMode.ForeColor = Color.DarkGreen;
        
        // Show/hide buttons
        btnAdminLogin.Visible = true;
        btnSwitchToUser.Visible = false;
        btnAddTable.Visible = false;
        btnRemoveTable.Visible = false;
        
        // Refresh table list to show only allowed tables
        RefreshTableList();
    }

    private string? PromptForPassword()
    {
        using (var form = new Form())
        {
            form.Text = "Admin Login";
            form.Size = new Size(350, 150);
            form.StartPosition = FormStartPosition.CenterParent;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;
            form.MinimizeBox = false;

            var label = new Label
            {
                Text = "Enter admin password:",
                Location = new Point(20, 20),
                Size = new Size(300, 20)
            };

            var textBox = new TextBox
            {
                Location = new Point(20, 45),
                Size = new Size(290, 20),
                UseSystemPasswordChar = true
            };

            var btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(150, 75),
                Size = new Size(75, 25)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(235, 75),
                Size = new Size(75, 25)
            };

            form.Controls.AddRange(new Control[] { label, textBox, btnOk, btnCancel });
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            return form.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }
    }
}
