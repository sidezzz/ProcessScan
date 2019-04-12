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
            this.modulesDataGridView = new System.Windows.Forms.DataGridView();
            this.moduleName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.moduleResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.scanButton = new System.Windows.Forms.Button();
            this.processPanel = new System.Windows.Forms.Panel();
            this.scanTabControl = new System.Windows.Forms.TabControl();
            this.modulesTabPage = new System.Windows.Forms.TabPage();
            this.processesTabPage = new System.Windows.Forms.TabPage();
            this.processScanDataGridView = new System.Windows.Forms.DataGridView();
            this.iconDataGridViewImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.pidDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.scanPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.processesDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modulesDataGridView)).BeginInit();
            this.processPanel.SuspendLayout();
            this.scanTabControl.SuspendLayout();
            this.modulesTabPage.SuspendLayout();
            this.processesTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.processScanDataGridView)).BeginInit();
            this.scanPanel.SuspendLayout();
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
            // moduleName
            // 
            this.moduleName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.moduleName.DataPropertyName = "name";
            this.moduleName.HeaderText = "Name";
            this.moduleName.Name = "moduleName";
            // 
            // moduleResult
            // 
            this.moduleResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.moduleResult.DataPropertyName = "result";
            this.moduleResult.HeaderText = "Result";
            this.moduleResult.Name = "moduleResult";
            this.moduleResult.Width = 62;
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
            this.processPanel.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this.processPanel.Size = new System.Drawing.Size(372, 682);
            this.processPanel.TabIndex = 4;
            // 
            // scanTabControl
            // 
            this.scanTabControl.Controls.Add(this.modulesTabPage);
            this.scanTabControl.Controls.Add(this.processesTabPage);
            this.scanTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scanTabControl.Location = new System.Drawing.Point(10, 10);
            this.scanTabControl.Multiline = true;
            this.scanTabControl.Name = "scanTabControl";
            this.scanTabControl.SelectedIndex = 0;
            this.scanTabControl.Size = new System.Drawing.Size(350, 662);
            this.scanTabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.scanTabControl.TabIndex = 6;
            // 
            // modulesTabPage
            // 
            this.modulesTabPage.Controls.Add(this.modulesDataGridView);
            this.modulesTabPage.Location = new System.Drawing.Point(4, 22);
            this.modulesTabPage.Name = "modulesTabPage";
            this.modulesTabPage.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.modulesTabPage.Size = new System.Drawing.Size(342, 636);
            this.modulesTabPage.TabIndex = 0;
            this.modulesTabPage.Text = "Modules";
            this.modulesTabPage.UseVisualStyleBackColor = true;
            // 
            // processesTabPage
            // 
            this.processesTabPage.Controls.Add(this.processScanDataGridView);
            this.processesTabPage.Location = new System.Drawing.Point(4, 22);
            this.processesTabPage.Name = "processesTabPage";
            this.processesTabPage.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.processesTabPage.Size = new System.Drawing.Size(346, 656);
            this.processesTabPage.TabIndex = 1;
            this.processesTabPage.Text = "Processes";
            this.processesTabPage.UseVisualStyleBackColor = true;
            // 
            // processScanDataGridView
            // 
            this.processScanDataGridView.AllowUserToAddRows = false;
            this.processScanDataGridView.AllowUserToDeleteRows = false;
            this.processScanDataGridView.AllowUserToOrderColumns = true;
            this.processScanDataGridView.AllowUserToResizeRows = false;
            this.processScanDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.processScanDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.processScanDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iconDataGridViewImageColumn,
            this.pidDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.resultDataGridViewTextBoxColumn});
            this.processScanDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processScanDataGridView.Location = new System.Drawing.Point(3, 3);
            this.processScanDataGridView.Name = "processScanDataGridView";
            this.processScanDataGridView.ReadOnly = true;
            this.processScanDataGridView.RowHeadersVisible = false;
            this.processScanDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.processScanDataGridView.Size = new System.Drawing.Size(340, 650);
            this.processScanDataGridView.TabIndex = 2;
            // 
            // iconDataGridViewImageColumn
            // 
            this.iconDataGridViewImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.iconDataGridViewImageColumn.DataPropertyName = "icon";
            this.iconDataGridViewImageColumn.HeaderText = "";
            this.iconDataGridViewImageColumn.MinimumWidth = 18;
            this.iconDataGridViewImageColumn.Name = "iconDataGridViewImageColumn";
            this.iconDataGridViewImageColumn.ReadOnly = true;
            this.iconDataGridViewImageColumn.Width = 18;
            // 
            // pidDataGridViewTextBoxColumn
            // 
            this.pidDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.pidDataGridViewTextBoxColumn.DataPropertyName = "pid";
            this.pidDataGridViewTextBoxColumn.HeaderText = "PID";
            this.pidDataGridViewTextBoxColumn.Name = "pidDataGridViewTextBoxColumn";
            this.pidDataGridViewTextBoxColumn.ReadOnly = true;
            this.pidDataGridViewTextBoxColumn.Width = 50;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // resultDataGridViewTextBoxColumn
            // 
            this.resultDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.resultDataGridViewTextBoxColumn.DataPropertyName = "result";
            this.resultDataGridViewTextBoxColumn.HeaderText = "Result";
            this.resultDataGridViewTextBoxColumn.Name = "resultDataGridViewTextBoxColumn";
            this.resultDataGridViewTextBoxColumn.ReadOnly = true;
            this.resultDataGridViewTextBoxColumn.Width = 62;
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
            ((System.ComponentModel.ISupportInitialize)(this.modulesDataGridView)).EndInit();
            this.processPanel.ResumeLayout(false);
            this.scanTabControl.ResumeLayout(false);
            this.modulesTabPage.ResumeLayout(false);
            this.processesTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.processScanDataGridView)).EndInit();
            this.scanPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.DataGridView processesDataGridView;
        private System.Windows.Forms.DataGridView modulesDataGridView;
        private System.Windows.Forms.Button scanButton;
        private System.Windows.Forms.DataGridViewImageColumn processIconDataGridViewImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn processPID;
        private System.Windows.Forms.DataGridViewTextBoxColumn processName;
        private System.Windows.Forms.Panel processPanel;
        private System.Windows.Forms.TabControl scanTabControl;
        private System.Windows.Forms.TabPage modulesTabPage;
        private System.Windows.Forms.TabPage processesTabPage;
        private System.Windows.Forms.DataGridView processScanDataGridView;
        private System.Windows.Forms.DataGridViewImageColumn iconDataGridViewImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pidDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn moduleName;
        private System.Windows.Forms.DataGridViewTextBoxColumn moduleResult;
        private System.Windows.Forms.Panel scanPanel;
    }
}

