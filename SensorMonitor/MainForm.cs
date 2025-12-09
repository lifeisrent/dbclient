using Microsoft.Data.SqlClient;
using System.Data;

namespace SensorMonitor;

public partial class MainForm : Form
{
    private readonly string connectionString;
    private SqlConnection? connection;
    private System.Windows.Forms.Timer refreshTimer;
    private FlowLayoutPanel sensorPanel;
    private Label labelStatus;
    private Label labelTitle;
    private Label labelLastUpdate;
    private Panel headerPanel;
    private Panel statusPanel;
    private Panel sidebarPanel;
    private FlowLayoutPanel checkboxPanel;
    private Label sidebarTitle;
    private Dictionary<int, SensorCard> sensorCards = new();
    private Dictionary<int, CheckBox> sensorCheckboxes = new();
    private HashSet<int> activeRawIds = new();

    public MainForm()
    {
        // Connection string - connect directly to NSDB where MONITOR_DCP exists
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = "ASUS\\MONITORTF",
            UserID = "sa",
            Password = "Dosu123$",
            Encrypt = true,
            TrustServerCertificate = true
        };
        connectionString = builder.ConnectionString;

        InitializeUI();
        SetupTimer();
    }

    private void InitializeUI()
    {
        // Form settings
        this.Text = "센서 모니터링 시스템";
        this.Size = new Size(1400, 900);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(18, 18, 24);
        this.Font = new Font("Segoe UI", 10F);
        this.DoubleBuffered = true;

        // Header Panel
        headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = Color.FromArgb(25, 25, 35),
            Padding = new Padding(20)
        };

        labelTitle = new Label
        {
            Text = "🔬 센서 모니터링 대시보드",
            Font = new Font("Segoe UI", 22F, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 180, 255),
            AutoSize = true,
            Location = new Point(20, 20)
        };
        headerPanel.Controls.Add(labelTitle);

        labelLastUpdate = new Label
        {
            Text = "마지막 업데이트: -",
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.FromArgb(150, 150, 160),
            AutoSize = true,
            Location = new Point(20, 52)
        };
        headerPanel.Controls.Add(labelLastUpdate);

        this.Controls.Add(headerPanel);

        // Status Panel (Bottom)
        statusPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 40,
            BackColor = Color.FromArgb(25, 25, 35),
            Padding = new Padding(10)
        };

        labelStatus = new Label
        {
            Text = "준비 중...",
            ForeColor = Color.FromArgb(150, 150, 160),
            AutoSize = true,
            Location = new Point(10, 10)
        };
        statusPanel.Controls.Add(labelStatus);

        this.Controls.Add(statusPanel);

        // Sidebar Panel (Left)
        sidebarPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 250,
            BackColor = Color.FromArgb(22, 22, 32),
            Padding = new Padding(10)
        };

        sidebarTitle = new Label
        {
            Text = "📋 센서 목록",
            Font = new Font("Segoe UI Semibold", 14F),
            ForeColor = Color.FromArgb(100, 180, 255),
            Dock = DockStyle.Top,
            Height = 40,
            TextAlign = ContentAlignment.MiddleLeft
        };
        sidebarPanel.Controls.Add(sidebarTitle);

        checkboxPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.FromArgb(22, 22, 32),
            Padding = new Padding(5)
        };
        sidebarPanel.Controls.Add(checkboxPanel);
        checkboxPanel.BringToFront();

        this.Controls.Add(sidebarPanel);

        // FlowLayoutPanel for sensor cards
        sensorPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(18, 18, 24),
            AutoScroll = true,
            Padding = new Padding(15),
            WrapContents = true
        };

        this.Controls.Add(sensorPanel);

        // Bring panels to front (order matters for docking)
        headerPanel.BringToFront();
        statusPanel.BringToFront();
        sidebarPanel.BringToFront();
    }

    private void SetupTimer()
    {
        refreshTimer = new System.Windows.Forms.Timer
        {
            Interval = 5000  // Refresh every 5 seconds
        };
        refreshTimer.Tick += async (s, e) => await RefreshDataAsync();
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await ConnectAndLoadAsync();
        refreshTimer.Start();
    }

    private async Task ConnectAndLoadAsync()
    {
        try
        {
            labelStatus.Text = "데이터베이스 연결 중...";
            labelStatus.ForeColor = Color.FromArgb(255, 200, 100);

            connection?.Dispose();
            connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // ★ DB 전환 없음 → 기본 DB (지금 네 상황에선 master)
            labelStatus.Text = $"✅ 연결됨 (DB: {connection.Database})";
            labelStatus.ForeColor = Color.FromArgb(100, 200, 100);

            // Load sidebar checkboxes
            await LoadSidebarAsync();

            await RefreshDataAsync();
        }
        catch (Exception ex)
        {
            labelStatus.Text = $"❌ 연결 실패: {ex.Message}";
            labelStatus.ForeColor = Color.FromArgb(255, 100, 100);
        }
    }

    private async Task LoadSidebarAsync()
    {
        if (connection == null || connection.State != ConnectionState.Open)
            return;

        try
        {
            // 1) Get current month's table name
            string currentMonthTable = $"H01RAW_{DateTime.Now:yyyyMM}";

            // 2) Get all active DCP_IDs from current month's RAW table
            activeRawIds.Clear();
            string rawQuery = $@"
                SELECT DISTINCT [DCP_ID] 
                FROM REMOTE2.NSDB.dbo.[{currentMonthTable}] WITH (NOLOCK)
            ";

            try
            {
                using var cmdRaw = new SqlCommand(rawQuery, connection) { CommandTimeout = 30 };
                using var readerRaw = await cmdRaw.ExecuteReaderAsync();
                while (await readerRaw.ReadAsync())
                {
                    if (!readerRaw.IsDBNull(0))
                        activeRawIds.Add(readerRaw.GetInt32(0));
                }
            }
            catch
            {
                // Table might not exist yet, try previous month
                string prevMonthTable = $"H01RAW_{DateTime.Now.AddMonths(-1):yyyyMM}";
                rawQuery = $@"
                    SELECT DISTINCT [DCP_ID] 
                    FROM REMOTE2.NSDB.dbo.[{prevMonthTable}] WITH (NOLOCK)
                ";
                try
                {
                    using var cmdRaw = new SqlCommand(rawQuery, connection) { CommandTimeout = 30 };
                    using var readerRaw = await cmdRaw.ExecuteReaderAsync();
                    while (await readerRaw.ReadAsync())
                    {
                        if (!readerRaw.IsDBNull(0))
                            activeRawIds.Add(readerRaw.GetInt32(0));
                    }
                }
                catch { /* Ignore if both fail */ }
            }

            // 3) Get all sensors from DCP table
            const string dcpQuery = @"
                SELECT [ID], [Name] 
                FROM REMOTE2.NSDB.dbo.DCP 
                ORDER BY [ID]
            ";

            using var cmd = new SqlCommand(dcpQuery, connection) { CommandTimeout = 30 };
            using var reader = await cmd.ExecuteReaderAsync();

            // Clear existing checkboxes
            checkboxPanel.Controls.Clear();
            sensorCheckboxes.Clear();

            // Collect checkboxes into two lists for sorting
            var activeCheckboxes = new List<CheckBox>();
            var inactiveCheckboxes = new List<CheckBox>();

            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32(0);
                string name = reader.IsDBNull(1) ? $"센서 {id}" : reader.GetString(1);

                bool isActive = activeRawIds.Contains(id);

                var checkbox = new CheckBox
                {
                    Text = name,
                    Tag = id,
                    Checked = isActive,
                    Enabled = true,
                    AutoSize = false,
                    Width = 220,
                    Height = 32,
                    Font = new Font("Segoe UI", 12F),
                    ForeColor = isActive 
                        ? Color.FromArgb(200, 200, 210) 
                        : Color.FromArgb(255, 255, 255),
                    BackColor = isActive 
                        ? Color.Transparent 
                        : Color.FromArgb(28, 28, 38),
                    Margin = new Padding(2)
                };

                // Hover effect for active
                if (isActive)
                {
                    checkbox.MouseEnter += (s, e) => checkbox.ForeColor = Color.FromArgb(100, 180, 255);
                    checkbox.MouseLeave += (s, e) => checkbox.ForeColor = Color.FromArgb(200, 200, 210);
                    
                    // Add event to show/hide sensor card when checkbox state changes
                    int capturedId = id;  // Capture for closure
                    checkbox.CheckedChanged += (s, e) =>
                    {
                        if (sensorCards.TryGetValue(capturedId, out var card))
                        {
                            card.Visible = checkbox.Checked;
                        }
                    };
                    
                    activeCheckboxes.Add(checkbox);
                }
                else
                {
                    // Prevent state changes for inactive sensors
                    checkbox.Click += (s, e) => checkbox.Checked = false;
                    inactiveCheckboxes.Add(checkbox);
                }

                sensorCheckboxes[id] = checkbox;
            }

            // Add active checkboxes first (top), then inactive (bottom)
            foreach (var cb in activeCheckboxes)
                checkboxPanel.Controls.Add(cb);

            // Add separator if there are both active and inactive
            if (activeCheckboxes.Count > 0 && inactiveCheckboxes.Count > 0)
            {
                var separator = new Label
                {
                    Text = "── 비활성 센서 ──",
                    ForeColor = Color.FromArgb(100, 100, 120),
                    Font = new Font("Segoe UI", 8F),
                    Width = 220,
                    Height = 25,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = new Padding(2, 10, 2, 5)
                };
                checkboxPanel.Controls.Add(separator);
            }

            foreach (var cb in inactiveCheckboxes)
                checkboxPanel.Controls.Add(cb);

            // Update sidebar title with count
            sidebarTitle.Text = $"📋 센서 목록 ({activeCheckboxes.Count}/{sensorCheckboxes.Count})";
        }
        catch (Exception ex)
        {
            sidebarTitle.Text = $"📋 센서 목록 (오류)";
            System.Diagnostics.Debug.WriteLine($"Sidebar load error: {ex.Message}");
        }
    }

    private static int SafeCountStatus(DataTable dt, string columnName, int code)
    {
        if (!dt.Columns.Contains(columnName))
            return 0;

        return dt.AsEnumerable()
                 .Count(r =>
                 {
                     var val = r[columnName];
                     if (val == null || val == DBNull.Value) return false;
                     if (!int.TryParse(val.ToString(), out int v)) return false;
                     return v == code;
                 });
    }

    private async Task RefreshDataAsync()
    {
        if (connection == null || connection.State != ConnectionState.Open)
        {
            await ConnectAndLoadAsync();
            if (connection == null || connection.State != ConnectionState.Open)
                return;
        }

        try
        {
            // 1) master.dbo.MONITOR_DCP 존재 여부 먼저 확인
            if (!await MonitorTableExistsAsync())
            {
                labelStatus.Text = "⚠ [master].[dbo].[MONITOR_DCP] 테이블이 없습니다. DB_Updater가 제대로 도는지 확인하세요.";
                labelStatus.ForeColor = Color.FromArgb(255, 180, 100);

                // 카드/뷰 비우고 종료
                UpdateSensorCards(new DataTable());
                labelLastUpdate.Text = "마지막 업데이트: -";
                return;
            }

            // 2) 실제 데이터 조회 (SSMS에서 확인된 형태 그대로)
            const string query = @"
            SELECT 
                DCP_ID,
                Name,
                DT,
                VALUE,
                STATUS
            FROM [master].[dbo].[MONITOR_DCP] WITH (NOLOCK)
            ORDER BY DCP_ID;
        ";

            using var cmd = new SqlCommand(query, connection)
            {
                CommandTimeout = 30
            };

            using var adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            await Task.Run(() => adapter.Fill(dt));

            // 3) 센서 카드 갱신
            UpdateSensorCards(dt);

            // 4) 마지막 업데이트 표기
            labelLastUpdate.Text = $"마지막 업데이트: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            // 5) 상태 카운트 (STATUS 없음/쓰레기값도 방어)
            int total = dt.Rows.Count;
            int normal = SafeCountStatus(dt, "STATUS", 0);
            int failed = SafeCountStatus(dt, "STATUS", 103);

            labelStatus.Text = $"✅ 센서 {total}개 | 정상: {normal}개 | 실패: {failed}개";
            labelStatus.ForeColor = failed > 0
                ? Color.FromArgb(255, 180, 100)
                : Color.FromArgb(100, 200, 100);
        }
        catch (SqlException ex)
        {
            labelStatus.Text = $"❌ 데이터 로드 실패(SQL): {ex.Message}";
            labelStatus.ForeColor = Color.FromArgb(255, 100, 100);
        }
        catch (Exception ex)
        {
            labelStatus.Text = $"❌ 데이터 로드 실패: {ex.Message}";
            labelStatus.ForeColor = Color.FromArgb(255, 100, 100);
        }
    }

    private async Task<bool> MonitorTableExistsAsync()
    {
        if (connection == null || connection.State != ConnectionState.Open)
            return false;

        const string sql = @"
        SELECT TOP (1) 1
        FROM master.sys.tables t
        JOIN master.sys.schemas s ON t.schema_id = s.schema_id
        WHERE t.name = 'MONITOR_DCP'
          AND s.name = 'dbo';
    ";

        using var cmd = new SqlCommand(sql, connection);
        var result = await cmd.ExecuteScalarAsync();
        return result != null && result != DBNull.Value;
    }



    private void UpdateSensorCards(DataTable dt)
    {
        HashSet<int> currentIds = new();

        foreach (DataRow row in dt.Rows)
        {
            int dcpId = Convert.ToInt32(row["DCP_ID"]);
            string name = row["Name"]?.ToString() ?? $"센서 {dcpId}";
            DateTime updateTime = row["DT"] != DBNull.Value ? Convert.ToDateTime(row["DT"]) : DateTime.MinValue;
            double value = row["VALUE"] != DBNull.Value ? Convert.ToDouble(row["VALUE"]) : 0;
            int status = row["STATUS"] != DBNull.Value ? Convert.ToInt32(row["STATUS"]) : -1;

            currentIds.Add(dcpId);

            if (sensorCards.TryGetValue(dcpId, out var card))
            {
                // Update existing card
                card.UpdateData(name, updateTime, value, status);
            }
            else
            {
                // Create new card
                var newCard = new SensorCard(dcpId, name, updateTime, value, status);
                sensorCards[dcpId] = newCard;
                sensorPanel.Controls.Add(newCard);
            }
        }

        // Remove cards that no longer exist
        var toRemove = sensorCards.Keys.Where(id => !currentIds.Contains(id)).ToList();
        foreach (var id in toRemove)
        {
            if (sensorCards.TryGetValue(id, out var card))
            {
                sensorPanel.Controls.Remove(card);
                card.Dispose();
                sensorCards.Remove(id);
            }
        }
    }

    private static void TrySwitchToNSDB(SqlConnection connection)
    {
        try
        {
            if (connection.Database == "master")
            {
                using var cmdCheck = new SqlCommand(
                    "SELECT database_id FROM sys.databases WHERE name = 'NSDB'",
                    connection
                );
                var result = cmdCheck.ExecuteScalar();
                if (result != null)
                {
                    connection.ChangeDatabase("NSDB");
                }
            }
        }
        catch
        {
            // Silently ignore
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        refreshTimer?.Stop();
        refreshTimer?.Dispose();
        connection?.Close();
        connection?.Dispose();
        base.OnFormClosing(e);
    }
}

/// <summary>
/// Individual sensor card UI component
/// </summary>
public class SensorCard : Panel
{
    private Label lblName;
    private Label lblValue;
    private Label lblTime;
    private Label lblStatus;
    private Panel statusIndicator;
    private int dcpId;

    public SensorCard(int id, string name, DateTime updateTime, double value, int status)
    {
        dcpId = id;
        InitializeCard();
        UpdateData(name, updateTime, value, status);
    }

    private void InitializeCard()
    {
        // Card styling
        this.Size = new Size(280, 160);
        this.Margin = new Padding(10);
        this.BackColor = Color.FromArgb(30, 30, 42);
        this.Padding = new Padding(15);

        // Add rounded corners effect via paint
        this.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(50, 50, 70), 1);
            e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
        };

        // Status indicator bar (left side)
        statusIndicator = new Panel
        {
            Size = new Size(5, this.Height - 20),
            Location = new Point(0, 10),
            BackColor = Color.FromArgb(100, 100, 100)
        };
        this.Controls.Add(statusIndicator);

        // Sensor Name
        lblName = new Label
        {
            Location = new Point(20, 12),
            Size = new Size(240, 28),
            Font = new Font("Segoe UI Semibold", 13F),
            ForeColor = Color.FromArgb(220, 220, 240),
            AutoEllipsis = true
        };
        this.Controls.Add(lblName);

        // Value
        lblValue = new Label
        {
            Location = new Point(20, 48),
            Size = new Size(240, 40),
            Font = new Font("Segoe UI", 24F, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 200, 255)
        };
        this.Controls.Add(lblValue);

        // Last update time
        lblTime = new Label
        {
            Location = new Point(20, 95),
            Size = new Size(240, 20),
            Font = new Font("Segoe UI", 9F),
            ForeColor = Color.FromArgb(120, 120, 140)
        };
        this.Controls.Add(lblTime);

        // Status text
        lblStatus = new Label
        {
            Location = new Point(20, 118),
            Size = new Size(240, 25),
            Font = new Font("Segoe UI Semibold", 10F)
        };
        this.Controls.Add(lblStatus);

        // Hover effect
        this.MouseEnter += (s, e) => this.BackColor = Color.FromArgb(38, 38, 52);
        this.MouseLeave += (s, e) => this.BackColor = Color.FromArgb(30, 30, 42);
    }

    public void UpdateData(string name, DateTime updateTime, double value, int status)
    {
        lblName.Text = name;
        lblValue.Text = value.ToString("F2");
        lblTime.Text = updateTime != DateTime.MinValue 
            ? $"🕐 {updateTime:yyyy-MM-dd HH:mm:ss}" 
            : "🕐 업데이트 없음";

        // Status display
        switch (status)
        {
            case 0:
                lblStatus.Text = "● 정상";
                lblStatus.ForeColor = Color.FromArgb(80, 200, 120);
                statusIndicator.BackColor = Color.FromArgb(80, 200, 120);
                lblValue.ForeColor = Color.FromArgb(100, 200, 255);
                break;
            case 103:
                lblStatus.Text = "● 연결없음";
                lblStatus.ForeColor = Color.FromArgb(255, 100, 100);
                statusIndicator.BackColor = Color.FromArgb(255, 100, 100);
                lblValue.ForeColor = Color.FromArgb(255, 100, 100);
                break;
            default:
                lblStatus.Text = $"● 통신없음 ({status})";
                lblStatus.ForeColor = Color.FromArgb(255, 200, 100);
                statusIndicator.BackColor = Color.FromArgb(255, 200, 100);
                lblValue.ForeColor = Color.FromArgb(255, 200, 100);
                break;
        }
    }
}
