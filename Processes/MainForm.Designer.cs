﻿namespace Processes
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
            this.scanTabControl = new System.Windows.Forms.TabControl();
            this.modulesTabPage = new System.Windows.Forms.TabPage();
            this.modulesDataGridView = new System.Windows.Forms.DataGridView();
            this.moduleName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.moduleResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.driverTabPage = new System.Windows.Forms.TabPage();
            this.driverObjectDataGridView = new System.Windows.Forms.DataGridView();
            this.driverObjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.driverObjectResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.scanKernelButton = new System.Windows.Forms.Button();
            this.scanProgressBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.processesDataGridView)).BeginInit();
            this.processPanel.SuspendLayout();
            this.scanPanel.SuspendLayout();
            this.scanTabControl.SuspendLayout();
            this.modulesTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.modulesDataGridView)).BeginInit();
            this.driverTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.driverObjectDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // refreshButton
            // 
            this.refreshButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.refreshButton.Location = new System.Drawing.Point(10, 653);
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
            this.processesDataGridView.Size = new System.Drawing.Size(352, 643);
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
            this.scanButton.Location = new System.Drawing.Point(401, 456);
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
            this.processPanel.Size = new System.Drawing.Size(372, 686);
            this.processPanel.TabIndex = 4;
            // 
            // scanPanel
            // 
            this.scanPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scanPanel.AutoSize = true;
            this.scanPanel.Controls.Add(this.scanTabControl);
            this.scanPanel.Location = new System.Drawing.Point(495, 0);
            this.scanPanel.MaximumSize = new System.Drawing.Size(10000, 10000);
            this.scanPanel.Name = "scanPanel";
            this.scanPanel.Padding = new System.Windows.Forms.Padding(10);
            this.scanPanel.Size = new System.Drawing.Size(382, 686);
            this.scanPanel.TabIndex = 7;
            // 
            // scanTabControl
            // 
            this.scanTabControl.Controls.Add(this.modulesTabPage);
            this.scanTabControl.Controls.Add(this.driverTabPage);
            this.scanTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scanTabControl.Location = new System.Drawing.Point(10, 10);
            this.scanTabControl.Multiline = true;
            this.scanTabControl.Name = "scanTabControl";
            this.scanTabControl.SelectedIndex = 0;
            this.scanTabControl.Size = new System.Drawing.Size(362, 666);
            this.scanTabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.scanTabControl.TabIndex = 6;
            // 
            // modulesTabPage
            // 
            this.modulesTabPage.Controls.Add(this.modulesDataGridView);
            this.modulesTabPage.Location = new System.Drawing.Point(4, 22);
            this.modulesTabPage.Name = "modulesTabPage";
            this.modulesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.modulesTabPage.Size = new System.Drawing.Size(354, 640);
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
            this.modulesDataGridView.Size = new System.Drawing.Size(348, 634);
            this.modulesDataGridView.TabIndex = 2;
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
            // driverTabPage
            // 
            this.driverTabPage.Controls.Add(this.driverObjectDataGridView);
            this.driverTabPage.Location = new System.Drawing.Point(4, 22);
            this.driverTabPage.Name = "driverTabPage";
            this.driverTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.driverTabPage.Size = new System.Drawing.Size(342, 636);
            this.driverTabPage.TabIndex = 1;
            this.driverTabPage.Text = "Driver Objects";
            this.driverTabPage.UseVisualStyleBackColor = true;
            // 
            // driverObjectDataGridView
            // 
            this.driverObjectDataGridView.AllowUserToAddRows = false;
            this.driverObjectDataGridView.AllowUserToDeleteRows = false;
            this.driverObjectDataGridView.AllowUserToOrderColumns = true;
            this.driverObjectDataGridView.AllowUserToResizeColumns = false;
            this.driverObjectDataGridView.AllowUserToResizeRows = false;
            this.driverObjectDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.driverObjectDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.driverObjectName,
            this.driverObjectResult});
            this.driverObjectDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.driverObjectDataGridView.Location = new System.Drawing.Point(3, 3);
            this.driverObjectDataGridView.MultiSelect = false;
            this.driverObjectDataGridView.Name = "driverObjectDataGridView";
            this.driverObjectDataGridView.RowHeadersVisible = false;
            this.driverObjectDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.driverObjectDataGridView.Size = new System.Drawing.Size(336, 630);
            this.driverObjectDataGridView.TabIndex = 0;
            // 
            // driverObjectName
            // 
            this.driverObjectName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.driverObjectName.DataPropertyName = "name";
            this.driverObjectName.HeaderText = "Name";
            this.driverObjectName.Name = "driverObjectName";
            // 
            // driverObjectResult
            // 
            this.driverObjectResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.driverObjectResult.DataPropertyName = "result";
            this.driverObjectResult.HeaderText = "Result";
            this.driverObjectResult.Name = "driverObjectResult";
            this.driverObjectResult.Width = 62;
            // 
            // scanKernelButton
            // 
            this.scanKernelButton.Location = new System.Drawing.Point(401, 427);
            this.scanKernelButton.Name = "scanKernelButton";
            this.scanKernelButton.Size = new System.Drawing.Size(75, 23);
            this.scanKernelButton.TabIndex = 8;
            this.scanKernelButton.Text = "Scan Kernel";
            this.scanKernelButton.UseVisualStyleBackColor = true;
            this.scanKernelButton.Click += new System.EventHandler(this.scanKernelButton_Click);
            // 
            // scanProgressBar
            // 
            this.scanProgressBar.Location = new System.Drawing.Point(368, 484);
            this.scanProgressBar.Name = "scanProgressBar";
            this.scanProgressBar.Size = new System.Drawing.Size(133, 23);
            this.scanProgressBar.TabIndex = 9;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(869, 686);
            this.Controls.Add(this.scanProgressBar);
            this.Controls.Add(this.scanKernelButton);
            this.Controls.Add(this.scanPanel);
            this.Controls.Add(this.processPanel);
            this.Controls.Add(this.scanButton);
            this.Name = "MainForm";
            this.Text = "Scanner";
            this.Load += new System.EventHandler(this.refreshButton_Click);
            ((System.ComponentModel.ISupportInitialize)(this.processesDataGridView)).EndInit();
            this.processPanel.ResumeLayout(false);
            this.scanPanel.ResumeLayout(false);
            this.scanTabControl.ResumeLayout(false);
            this.modulesTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.modulesDataGridView)).EndInit();
            this.driverTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.driverObjectDataGridView)).EndInit();
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
        private System.Windows.Forms.TabPage driverTabPage;
        private System.Windows.Forms.DataGridView driverObjectDataGridView;
        private System.Windows.Forms.Button scanKernelButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn driverObjectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn driverObjectResult;
        private System.Windows.Forms.ProgressBar scanProgressBar;
    }
}

