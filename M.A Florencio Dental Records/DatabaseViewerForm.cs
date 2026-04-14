using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    public class DatabaseViewerForm : Form
    {
        private TreeView treeStructure;
        private DataGridView gridData;
        private Label lblRowCount;
        private Button btnRefresh;
        private Button btnClose;
        private SplitContainer splitContainer;
        private Label lblTableName;
        private RichTextBox txtSql;
        private Button btnRunSql;
        private TabControl tabControl;

        public DatabaseViewerForm()
        {
            BuildUI();
            LoadDatabaseStructure();
        }

        private void BuildUI()
        {
            this.Text = "Database Viewer — M.A Florencio Dental";
            this.Size = new Size(1100, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // ── HEADER ──────────────────────────────────────────────────
            Panel header = new Panel();
            header.Dock = DockStyle.Top;
            header.Height = 55;
            header.BackColor = Color.FromArgb(0, 102, 102);

            Label title = new Label();
            title.Text = "⚙  Database Structure Viewer";
            title.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            title.ForeColor = Color.White;
            title.Location = new Point(15, 12);
            title.AutoSize = true;
            header.Controls.Add(title);

            btnRefresh = new Button();
            btnRefresh.Text = "⟳  Refresh";
            btnRefresh.Size = new Size(110, 34);
            btnRefresh.Location = new Point(860, 10);
            btnRefresh.BackColor = Color.FromArgb(0, 140, 140);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => LoadDatabaseStructure();
            header.Controls.Add(btnRefresh);

            btnClose = new Button();
            btnClose.Text = "✕  Close";
            btnClose.Size = new Size(95, 34);
            btnClose.Location = new Point(980, 10);
            btnClose.BackColor = Color.FromArgb(180, 60, 60);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();
            header.Controls.Add(btnClose);

            // ── SPLIT: LEFT TREE | RIGHT TABS ────────────────────────────
            splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.SplitterDistance = 230;
            splitContainer.BackColor = Color.FromArgb(220, 220, 220);

            // LEFT — Tree
            Panel leftPanel = new Panel();
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.BackColor = Color.White;

            Label lblTables = new Label();
            lblTables.Text = "  Tables";
            lblTables.Dock = DockStyle.Top;
            lblTables.Height = 30;
            lblTables.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTables.ForeColor = Color.White;
            lblTables.BackColor = Color.FromArgb(60, 60, 60);
            lblTables.TextAlign = ContentAlignment.MiddleLeft;

            treeStructure = new TreeView();
            treeStructure.Dock = DockStyle.Fill;
            treeStructure.Font = new Font("Segoe UI", 9);
            treeStructure.BackColor = Color.White;
            treeStructure.BorderStyle = BorderStyle.None;
            treeStructure.AfterSelect += Tree_AfterSelect;

            leftPanel.Controls.Add(treeStructure);
            leftPanel.Controls.Add(lblTables);

            // RIGHT — Tabs
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 9);

            // Tab 1: Data
            TabPage tabData = new TabPage("  📋 Table Data  ");
            tabData.BackColor = Color.White;

            lblTableName = new Label();
            lblTableName.Text = "← Select a table";
            lblTableName.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblTableName.ForeColor = Color.FromArgb(0, 102, 102);
            lblTableName.Dock = DockStyle.Top;
            lblTableName.Height = 32;
            lblTableName.TextAlign = ContentAlignment.MiddleLeft;
            lblTableName.Padding = new Padding(5, 0, 0, 0);

            lblRowCount = new Label();
            lblRowCount.Text = "";
            lblRowCount.Font = new Font("Segoe UI", 9);
            lblRowCount.ForeColor = Color.Gray;
            lblRowCount.Dock = DockStyle.Top;
            lblRowCount.Height = 22;
            lblRowCount.TextAlign = ContentAlignment.MiddleLeft;
            lblRowCount.Padding = new Padding(5, 0, 0, 0);

            gridData = new DataGridView();
            gridData.Dock = DockStyle.Fill;
            gridData.ReadOnly = true;
            gridData.AllowUserToAddRows = false;
            gridData.AllowUserToDeleteRows = false;
            gridData.BackgroundColor = Color.White;
            gridData.BorderStyle = BorderStyle.None;
            gridData.RowHeadersVisible = false;
            gridData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            gridData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 102);
            gridData.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridData.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            gridData.EnableHeadersVisualStyles = false;
            gridData.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 248);

            tabData.Controls.Add(gridData);
            tabData.Controls.Add(lblRowCount);
            tabData.Controls.Add(lblTableName);

            // Tab 2: Structure
            TabPage tabStructure = new TabPage("  🔧 Column Structure  ");
            tabStructure.BackColor = Color.White;

            DataGridView gridColumns = new DataGridView();
            gridColumns.Name = "gridColumns";
            gridColumns.Dock = DockStyle.Fill;
            gridColumns.ReadOnly = true;
            gridColumns.AllowUserToAddRows = false;
            gridColumns.BackgroundColor = Color.White;
            gridColumns.BorderStyle = BorderStyle.None;
            gridColumns.RowHeadersVisible = false;
            gridColumns.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            gridColumns.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60);
            gridColumns.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridColumns.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            gridColumns.EnableHeadersVisualStyles = false;
            gridColumns.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            tabStructure.Controls.Add(gridColumns);

            // Tab 3: SQL Runner
            TabPage tabSql = new TabPage("  ▶ SQL Runner  ");
            tabSql.BackColor = Color.White;

            Label lblSqlHint = new Label();
            lblSqlHint.Text = "  Run a SELECT query (read-only):";
            lblSqlHint.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblSqlHint.ForeColor = Color.FromArgb(60, 60, 60);
            lblSqlHint.Dock = DockStyle.Top;
            lblSqlHint.Height = 28;
            lblSqlHint.TextAlign = ContentAlignment.MiddleLeft;

            txtSql = new RichTextBox();
            txtSql.Dock = DockStyle.Top;
            txtSql.Height = 100;
            txtSql.Font = new Font("Consolas", 10);
            txtSql.Text = "SELECT TOP 100 * FROM Users";
            txtSql.BackColor = Color.FromArgb(30, 30, 30);
            txtSql.ForeColor = Color.LightGreen;

            btnRunSql = new Button();
            btnRunSql.Text = "▶  Run Query";
            btnRunSql.Dock = DockStyle.Top;
            btnRunSql.Height = 36;
            btnRunSql.BackColor = Color.FromArgb(0, 102, 102);
            btnRunSql.ForeColor = Color.White;
            btnRunSql.FlatStyle = FlatStyle.Flat;
            btnRunSql.FlatAppearance.BorderSize = 0;
            btnRunSql.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRunSql.Cursor = Cursors.Hand;
            btnRunSql.Click += BtnRunSql_Click;

            DataGridView gridSqlResult = new DataGridView();
            gridSqlResult.Name = "gridSqlResult";
            gridSqlResult.Dock = DockStyle.Fill;
            gridSqlResult.ReadOnly = true;
            gridSqlResult.AllowUserToAddRows = false;
            gridSqlResult.BackgroundColor = Color.White;
            gridSqlResult.BorderStyle = BorderStyle.None;
            gridSqlResult.RowHeadersVisible = false;
            gridSqlResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            gridSqlResult.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 102);
            gridSqlResult.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridSqlResult.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            gridSqlResult.EnableHeadersVisualStyles = false;

            tabSql.Controls.Add(gridSqlResult);
            tabSql.Controls.Add(btnRunSql);
            tabSql.Controls.Add(txtSql);
            tabSql.Controls.Add(lblSqlHint);

            tabControl.TabPages.Add(tabData);
            tabControl.TabPages.Add(tabStructure);
            tabControl.TabPages.Add(tabSql);
            tabControl.SelectedIndexChanged += (s, e) =>
            {
                if (treeStructure.SelectedNode?.Tag?.ToString() is string table)
                {
                    if (tabControl.SelectedIndex == 1)
                        LoadColumnStructure(table, gridColumns);
                }
            };

            splitContainer.Panel1.Controls.Add(leftPanel);
            splitContainer.Panel2.Controls.Add(tabControl);

            this.Controls.Add(splitContainer);
            this.Controls.Add(header);
        }

        // ── LOAD TREE ──────────────────────────────────────────────────────
        private void LoadDatabaseStructure()
        {
            treeStructure.Nodes.Clear();
            string connStr = ConnectionHelper.GetConnectionString();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Get database name
                    string dbName = conn.Database;
                    TreeNode dbNode = new TreeNode($"🗄  {dbName}");
                    dbNode.ForeColor = Color.FromArgb(0, 102, 102);
                    dbNode.NodeFont = new Font("Segoe UI", 9, FontStyle.Bold);

                    // Get all tables
                    string sql = @"
                        SELECT TABLE_NAME 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_TYPE = 'BASE TABLE'
                        ORDER BY TABLE_NAME";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tableName = reader.GetString(0);
                            TreeNode tableNode = new TreeNode($"📋  {tableName}");
                            tableNode.Tag = tableName;
                            dbNode.Nodes.Add(tableNode);
                        }
                    }

                    treeStructure.Nodes.Add(dbNode);
                    dbNode.Expand();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading structure:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── TREE SELECT → LOAD TABLE DATA ─────────────────────────────────
        private void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag?.ToString() is string tableName)
            {
                lblTableName.Text = $"  📋  {tableName}";
                LoadTableData(tableName);

                // If structure tab is visible, refresh it too
                if (tabControl.SelectedIndex == 1)
                {
                    var gridColumns = tabControl.TabPages[1].Controls["gridColumns"] as DataGridView;
                    if (gridColumns != null)
                        LoadColumnStructure(tableName, gridColumns);
                }
            }
        }

        // ── LOAD TABLE DATA ────────────────────────────────────────────────
        private void LoadTableData(string tableName)
        {
            string connStr = ConnectionHelper.GetConnectionString();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Row count
                    int rowCount = 0;
                    using (SqlCommand countCmd = new SqlCommand(
                        $"SELECT COUNT(*) FROM [{tableName}]", conn))
                    {
                        rowCount = (int)countCmd.ExecuteScalar();
                    }

                    lblRowCount.Text = $"   {rowCount:N0} rows";

                    // Data (top 500 rows for performance)
                    string sql = $"SELECT TOP 500 * FROM [{tableName}]";
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    gridData.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                lblRowCount.Text = "Error";
                MessageBox.Show($"Error loading table:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── LOAD COLUMN STRUCTURE ──────────────────────────────────────────
        private void LoadColumnStructure(string tableName, DataGridView grid)
        {
            string connStr = ConnectionHelper.GetConnectionString();

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string sql = @"
                        SELECT 
                            c.COLUMN_NAME        AS [Column],
                            c.DATA_TYPE          AS [Type],
                            ISNULL(CAST(c.CHARACTER_MAXIMUM_LENGTH AS VARCHAR), 
                                   CAST(c.NUMERIC_PRECISION AS VARCHAR)) AS [Length],
                            c.IS_NULLABLE        AS [Nullable],
                            c.COLUMN_DEFAULT     AS [Default],
                            CASE WHEN pk.COLUMN_NAME IS NOT NULL 
                                 THEN '🔑 PK' ELSE '' END AS [Key],
                            CASE WHEN fk.COLUMN_NAME IS NOT NULL 
                                 THEN '🔗 FK' ELSE '' END AS [FK]
                        FROM INFORMATION_SCHEMA.COLUMNS c
                        LEFT JOIN (
                            SELECT ku.COLUMN_NAME
                            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                            JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
                                ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                            WHERE tc.TABLE_NAME = @table 
                              AND tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
                        ) pk ON c.COLUMN_NAME = pk.COLUMN_NAME
                        LEFT JOIN (
                            SELECT ku.COLUMN_NAME
                            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                            JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
                                ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                            WHERE tc.TABLE_NAME = @table 
                              AND tc.CONSTRAINT_TYPE = 'FOREIGN KEY'
                        ) fk ON c.COLUMN_NAME = fk.COLUMN_NAME
                        WHERE c.TABLE_NAME = @table
                        ORDER BY c.ORDINAL_POSITION";

                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@table", tableName);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    grid.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading columns:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── SQL RUNNER ─────────────────────────────────────────────────────
        private void BtnRunSql_Click(object sender, EventArgs e)
        {
            string query = txtSql.Text.Trim();

            // Safety: only allow SELECT
            if (!query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "Only SELECT queries are allowed in this viewer.",
                    "Read-Only Mode",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string connStr = ConnectionHelper.GetConnectionString();
            var gridResult = tabControl.TabPages[2].Controls["gridSqlResult"] as DataGridView;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    gridResult.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Query error:\n{ex.Message}", "SQL Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}