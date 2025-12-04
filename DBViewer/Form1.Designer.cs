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
        listBoxTables.Location = new Point(12, 42);
        listBoxTables.Name = "listBoxTables";
        listBoxTables.Size = new Size(250, 394);
        listBoxTables.TabIndex = 0;
        listBoxTables.SelectedIndexChanged += ListBoxTables_SelectedIndexChanged;
        // 
        // dataGridViewData
        // 
        dataGridViewData.AllowUserToAddRows = false;
        dataGridViewData.AllowUserToDeleteRows = false;
        dataGridViewData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewData.Location = new Point(268, 42);
        dataGridViewData.Name = "dataGridViewData";
        dataGridViewData.ReadOnly = true;
        dataGridViewData.Size = new Size(520, 394);
        dataGridViewData.TabIndex = 1;
        // 
        // labelStatus
        // 
        labelStatus.AutoSize = true;
        labelStatus.Location = new Point(12, 9);
        labelStatus.Name = "labelStatus";
        labelStatus.Size = new Size(91, 15);
        labelStatus.TabIndex = 2;
        labelStatus.Text = "Connecting...";
        // 
        // 
        // labelMode
        // 
        labelMode = new Label();
        labelMode.AutoSize = true;
        labelMode.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        labelMode.ForeColor = Color.DarkGreen;
        labelMode.Location = new Point(650, 9);
        labelMode.Name = "labelMode";
        labelMode.Size = new Size(80, 15);
        labelMode.TabIndex = 3;
        labelMode.Text = "👤 User Mode";
        // 
        // btnAdminLogin
        // 
        btnAdminLogin = new Button();
        btnAdminLogin.Location = new Point(650, 442);
        btnAdminLogin.Name = "btnAdminLogin";
        btnAdminLogin.Size = new Size(138, 30);
        btnAdminLogin.TabIndex = 4;
        btnAdminLogin.Text = "🔐 Admin Login";
        btnAdminLogin.UseVisualStyleBackColor = true;
        btnAdminLogin.Click += BtnAdminLogin_Click;
        // 
        // btnSwitchToUser
        // 
        btnSwitchToUser = new Button();
        btnSwitchToUser.Location = new Point(650, 442);
        btnSwitchToUser.Name = "btnSwitchToUser";
        btnSwitchToUser.Size = new Size(138, 30);
        btnSwitchToUser.TabIndex = 5;
        btnSwitchToUser.Text = "↩️ Switch to User";
        btnSwitchToUser.UseVisualStyleBackColor = true;
        btnSwitchToUser.Visible = false;
        btnSwitchToUser.Click += BtnSwitchToUser_Click;
        // 
        // btnAddTable
        // 
        btnAddTable = new Button();
        btnAddTable.Location = new Point(268, 442);
        btnAddTable.Name = "btnAddTable";
        btnAddTable.Size = new Size(120, 30);
        btnAddTable.TabIndex = 6;
        btnAddTable.Text = "➕ Add for Users";
        btnAddTable.UseVisualStyleBackColor = true;
        btnAddTable.Visible = false;
        btnAddTable.Click += BtnAddTable_Click;
        // 
        // btnRemoveTable
        // 
        btnRemoveTable = new Button();
        btnRemoveTable.Location = new Point(394, 442);
        btnRemoveTable.Name = "btnRemoveTable";
        btnRemoveTable.Size = new Size(120, 30);
        btnRemoveTable.TabIndex = 7;
        btnRemoveTable.Text = "➖ Remove";
        btnRemoveTable.UseVisualStyleBackColor = true;
        btnRemoveTable.Visible = false;
        btnRemoveTable.Click += BtnRemoveTable_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 484);
        Controls.Add(btnRemoveTable);
        Controls.Add(btnAddTable);
        Controls.Add(btnSwitchToUser);
        Controls.Add(btnAdminLogin);
        Controls.Add(labelMode);
        Controls.Add(labelStatus);
        Controls.Add(dataGridViewData);
        Controls.Add(listBoxTables);
        Name = "Form1";
        Text = "Database Table Viewer - REMOTE2.NSDB";
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
