namespace DBViewer;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            connection?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        listBoxTables = new ListBox();
        dataGridViewData = new DataGridView();
        labelStatus = new Label();
        ((System.ComponentModel.ISupportInitialize)dataGridViewData).BeginInit();
        SuspendLayout();
        // 
        // listBoxTables
        // 
        listBoxTables.FormattingEnabled = true;
        listBoxTables.ItemHeight = 15;
        listBoxTables.Location = new Point(15, 50);
        listBoxTables.Name = "listBoxTables";
        listBoxTables.Size = new Size(280, 500);
        listBoxTables.TabIndex = 0;
        listBoxTables.Font = new Font("Segoe UI", 10F);
        listBoxTables.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        listBoxTables.SelectedIndexChanged += ListBoxTables_SelectedIndexChanged;
        // 
        // dataGridViewData
        // 
        dataGridViewData.AllowUserToAddRows = false;
        dataGridViewData.AllowUserToDeleteRows = false;
        dataGridViewData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewData.Location = new Point(305, 50);
        dataGridViewData.Name = "dataGridViewData";
        dataGridViewData.ReadOnly = true;
        dataGridViewData.Size = new Size(700, 500);
        dataGridViewData.TabIndex = 1;
        dataGridViewData.Font = new Font("Segoe UI", 9F);
        dataGridViewData.BackgroundColor = Color.White;
        dataGridViewData.BorderStyle = BorderStyle.Fixed3D;
        dataGridViewData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        // 
        // labelStatus
        // 
        labelStatus.AutoSize = true;
        labelStatus.Location = new Point(15, 15);
        labelStatus.Name = "labelStatus";
        labelStatus.Size = new Size(150, 20);
        labelStatus.TabIndex = 2;
        labelStatus.Text = "Connecting...";
        labelStatus.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
        labelStatus.ForeColor = Color.FromArgb(60, 60, 60);
        // 
        // 
        // labelMode
        // 
        labelMode = new Label();
        labelMode.AutoSize = true;
        labelMode.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        labelMode.ForeColor = Color.DarkGreen;
        labelMode.Location = new Point(700, 15);
        labelMode.Name = "labelMode";
        labelMode.Size = new Size(120, 20);
        labelMode.TabIndex = 3;
        labelMode.Text = "👤 User Mode";
        labelMode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        // 
        // btnAdminLogin
        // 
        btnAdminLogin = new Button();
        btnAdminLogin.Location = new Point(850, 10);
        btnAdminLogin.Name = "btnAdminLogin";
        btnAdminLogin.Size = new Size(150, 35);
        btnAdminLogin.TabIndex = 4;
        btnAdminLogin.Text = "🔐 Admin Login";
        btnAdminLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnAdminLogin.BackColor = Color.FromArgb(52, 152, 219);
        btnAdminLogin.ForeColor = Color.White;
        btnAdminLogin.FlatStyle = FlatStyle.Flat;
        btnAdminLogin.FlatAppearance.BorderSize = 0;
        btnAdminLogin.Cursor = Cursors.Hand;
        btnAdminLogin.UseVisualStyleBackColor = false;
        btnAdminLogin.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAdminLogin.Click += BtnAdminLogin_Click;
        // 
        // btnSwitchToUser
        // 
        btnSwitchToUser = new Button();
        btnSwitchToUser.Location = new Point(850, 10);
        btnSwitchToUser.Name = "btnSwitchToUser";
        btnSwitchToUser.Size = new Size(150, 35);
        btnSwitchToUser.TabIndex = 5;
        btnSwitchToUser.Text = "↩️ Switch to User";
        btnSwitchToUser.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnSwitchToUser.BackColor = Color.FromArgb(231, 76, 60);
        btnSwitchToUser.ForeColor = Color.White;
        btnSwitchToUser.FlatStyle = FlatStyle.Flat;
        btnSwitchToUser.FlatAppearance.BorderSize = 0;
        btnSwitchToUser.Cursor = Cursors.Hand;
        btnSwitchToUser.UseVisualStyleBackColor = false;
        btnSwitchToUser.Visible = false;
        btnSwitchToUser.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSwitchToUser.Click += BtnSwitchToUser_Click;
        // 
        // btnAddTable
        // 
        btnAddTable = new Button();
        btnAddTable.Location = new Point(305, 560);
        btnAddTable.Name = "btnAddTable";
        btnAddTable.Size = new Size(140, 35);
        btnAddTable.TabIndex = 6;
        btnAddTable.Text = "➕ Add for Users";
        btnAddTable.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnAddTable.BackColor = Color.FromArgb(46, 204, 113);
        btnAddTable.ForeColor = Color.White;
        btnAddTable.FlatStyle = FlatStyle.Flat;
        btnAddTable.FlatAppearance.BorderSize = 0;
        btnAddTable.Cursor = Cursors.Hand;
        btnAddTable.UseVisualStyleBackColor = false;
        btnAddTable.Visible = false;
        btnAddTable.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnAddTable.Click += BtnAddTable_Click;
        // 
        // btnRemoveTable
        // 
        btnRemoveTable = new Button();
        btnRemoveTable.Location = new Point(455, 560);
        btnRemoveTable.Name = "btnRemoveTable";
        btnRemoveTable.Size = new Size(140, 35);
        btnRemoveTable.TabIndex = 7;
        btnRemoveTable.Text = "➖ Remove";
        btnRemoveTable.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnRemoveTable.BackColor = Color.FromArgb(231, 76, 60);
        btnRemoveTable.ForeColor = Color.White;
        btnRemoveTable.FlatStyle = FlatStyle.Flat;
        btnRemoveTable.FlatAppearance.BorderSize = 0;
        btnRemoveTable.Cursor = Cursors.Hand;
        btnRemoveTable.UseVisualStyleBackColor = false;
        btnRemoveTable.Visible = false;
        btnRemoveTable.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnRemoveTable.Click += BtnRemoveTable_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1024, 615);
        BackColor = Color.FromArgb(240, 240, 240);
        Controls.Add(btnRemoveTable);
        Controls.Add(btnAddTable);
        Controls.Add(btnSwitchToUser);
        Controls.Add(btnAdminLogin);
        Controls.Add(labelMode);
        Controls.Add(labelStatus);
        Controls.Add(dataGridViewData);
        Controls.Add(listBoxTables);
        MinimumSize = new Size(800, 600);
        Name = "Form1";
        Text = "Database Table Viewer - REMOTE2.NSDB";
        WindowState = FormWindowState.Maximized;
        Load += Form1_Load;
        FormClosing += Form1_FormClosing;
        ((System.ComponentModel.ISupportInitialize)dataGridViewData).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private ListBox listBoxTables;
    private DataGridView dataGridViewData;
    private Label labelStatus;
    private Label labelMode;
    private Button btnAdminLogin;
    private Button btnSwitchToUser;
    private Button btnAddTable;
    private Button btnRemoveTable;
}
