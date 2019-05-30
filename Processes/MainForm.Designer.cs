namespace Processes
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.refreshButton = new System.Windows.Forms.Button();
            this.processesDataGridView = new System.Windows.Forms.DataGridView();
            this.processIconDataGridViewImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.processPID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.processName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.scanButton = new System.Windows.Forms.Button();
            this.processPanel = new System.Windows.Forms.Panel();
            this.scanPanel = new System.Windows.Forms.Panel();
            this.modulesTabPage = new System.Windows.Forms.TabPage();
            this.modulesDataGridView = new System.Windows.Forms.DataGridView();
            this.moduleResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.moduleName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.scanTabControl = new System.Windows.Forms.TabControl();
            ((System.ComponentModel.ISupportInitialize)(this.processesDataGridView)).BeginInit();
            this.processPanel.SuspendLayout();
            this.scanPanel.SuspendLayout();
            this.modulesTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.modulesDataGridView)).BeginInit();
            this.scanTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // refreshButton
            // 
            this.refreshButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.refreshButton.Location = new System.Drawing.Point(10, 649);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(352, 23);
            this.refreshButton.TabIndex = 0;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // processesDataGridView
            // 
            this.processesDataGridView.AllowUserToAddRows = false;
            this.processesDataGridView.AllowUserToDeleteRows = false;
            this.processesDataGridView.AllowUserToOrderColumns = true;
            this.processesDataGridView.AllowUserToResizeColumns = false;
            this.processesDataGridView.AllowUserToResizeRows = false;
            this.processesDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.processesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.processesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.processIconDataGridViewImageColumn,
            this.processPID,
            this.processName});
            this.processesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processesDataGridView.Location = new System.Drawing.Point(10, 10);
            this.processesDataGridView.Name = "processesDataGridView";
            this.processesDataGridView.ReadOnly = true;
            this.processesDataGridView.RowHeadersVisible = false;
            this.processesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.processesDataGridView.Size = new System.Drawing.Size(352, 639);
            this.processesDataGridView.TabIndex = 1;
            // 
            // processIconDataGridViewImageColumn
            // 
            this.processIconDataGridViewImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.processIconDataGridViewImageColumn.DataPropertyName = "icon";
            this.processIconDataGridViewImageColumn.HeaderText = "";
            this.processIconDataGridViewImageColumn.MinimumWidth = 18;
            this.processIconDataGridViewImageColumn.Name = "processIconDataGridViewImageColumn";
            this.processIconDataGridViewImageColumn.ReadOnly = true;
            this.processIconDataGridViewImageColumn.Width = 18;
            // 
            // processPID
            // 
            this.processPID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.processPID.DataPropertyName = "pid";
            this.processPID.HeaderText = "PID";
            this.processPID.Name = "processPID";
            this.processPID.ReadOnly = true;
            this.processPID.Width = 50;
            // 
            // processName
            // 
            this.processName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.processName.DataPropertyName = "name";
            this.processName.HeaderText = "Name";
            this.processName.Name = "processName";
            this.processName.ReadOnly = true;
            // 
            // scanButton
            // 
            this.scanButton.Location = new System.Drawing.Point(393, 455);
            this.scanButton.Name = "scanButton";
            this.scanButton.Size = new System.Drawing.Size(75, 23);
            this.scanButton.TabIndex = 3;
            this.scanButton.Text = "Scan";
            this.scanButton.UseVisualStyleBackColor = true;
            this.scanButton.Click += new System.EventHandler(this.scanButton_Click);
            // 
            // processPanel
            // 
            this.processPanel.Controls.Add(this.processesDataGridView);
            this.processPanel.Controls.Add(this.refreshButton);
            this.processPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.processPanel.Location = new System.Drawing.Point(0, 0);
            this.processPanel.MinimumSize = new System.Drawing.Size(100, 0);
            this.processPanel.Name = "processPanel";
            this.processPanel.Padding = new System.Windows.Forms.Padding(10);
            this.processPanel.Size = new System.Drawing.Size(372, 682);
            this.processPanel.TabIndex = 4;
            // 
            // scanPanel
            // 
            this.scanPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scanPanel.AutoSize = true;
            this.scanPanel.Controls.Add(this.scanTabControl);
            this.scanPanel.Location = new System.Drawing.Point(490, 0);
            this.scanPanel.MaximumSize = new System.Drawing.Size(10000, 10000);
            this.scanPanel.Name = "scanPanel";
            this.scanPanel.Padding = new System.Windows.Forms.Padding(10);
            this.scanPanel.Size = new System.Drawing.Size(370, 682);
            this.scanPanel.TabIndex = 7;
            // 
            // modulesTabPage
            // 
            this.modulesTabPage.Controls.Add(this.modulesDataGridView);
            this.modulesTabPage.Location = new System.Drawing.Point(4, 22);
            this.modulesTabPage.Name = "modulesTabPage";
            this.modulesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.modulesTabPage.Size = new System.Drawing.Size(342, 636);
            this.modulesTabPage.TabIndex = 0;
            this.modulesTabPage.Text = "Modules";
            this.modulesTabPage.UseVisualStyleBackColor = true;
            // 
            // modulesDataGridView
            // 
            this.modulesDataGridView.AllowUserToAddRows = false;
            this.modulesDataGridView.AllowUserToDeleteRows = false;
            this.modulesDataGridView.AllowUserToOrderColumns = true;
            this.modulesDataGridView.AllowUserToResizeColumns = false;
            this.modulesDataGridView.AllowUserToResizeRows = false;
            this.modulesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.modulesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.moduleName,
            this.moduleResult});
            this.modulesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modulesDataGridView.Location = new System.Drawing.Point(3, 3);
            this.modulesDataGridView.MultiSelect = false;
            this.modulesDataGridView.Name = "modulesDataGridView";
            this.modulesDataGridView.RowHeadersVisible = false;
            this.modulesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.modulesDataGridView.Size = new System.Drawing.Size(336, 630);
            this.modulesDataGridView.TabIndex = 2;
            this.modulesDataGridView.Visible = false;
            // 
            // moduleResult
            // 
            this.moduleResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.moduleResult.DataPropertyName = "result";
            this.moduleResult.HeaderText = "Result";
            this.moduleResult.Name = "moduleResult";
            this.moduleResult.Width = 62;
            // 
            // moduleName
            // 
            this.moduleName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.moduleName.DataPropertyName = "name";
            this.moduleName.HeaderText = "Name";
            this.moduleName.Name = "moduleName";
            // 
            // scanTabControl
            // 
            this.scanTabControl.Controls.Add(this.modulesTabPage);
            this.scanTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scanTabControl.Location = new System.Drawing.Point(10, 10);
            this.scanTabControl.Multiline = true;
            this.scanTabControl.Name = "scanTabControl";
            this.scanTabControl.SelectedIndex = 0;
            this.scanTabControl.Size = new System.Drawing.Size(350, 662);
            this.scanTabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.scanTabControl.TabIndex = 6;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 682);
            this.Controls.Add(this.scanPanel);
            this.Controls.Add(this.processPanel);
            this.Controls.Add(this.scanButton);
            this.Name = "MainForm";
            this.Text = "Scanner";
            this.Load += new System.EventHandler(this.refreshButton_Click);
            ((System.ComponentModel.ISupportInitialize)(this.processesDataGridView)).EndInit();
            this.processPanel.ResumeLayout(false);
            this.scanPanel.ResumeLayout(false);
            this.modulesTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.modulesDataGridView)).EndInit();
            this.scanTabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.DataGridView processesDataGridView;
        private System.Windows.Forms.Button scanButton;
        private System.Windows.Forms.DataGridViewImageColumn processIconDataGridViewImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn processPID;
        private System.Windows.Forms.DataGridViewTextBoxColumn processName;
        private System.Windows.Forms.Panel processPanel;
        private System.Windows.Forms.Panel scanPanel;
        private System.Windows.Forms.TabControl scanTabControl;
        private System.Windows.Forms.TabPage modulesTabPage;
        private System.Windows.Forms.DataGridView modulesDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn moduleName;
        private System.Windows.Forms.DataGridViewTextBoxColumn moduleResult;
    }
}

