using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics.Contracts;
using Processes.Scan;

namespace Processes
{
    public partial class MainForm : Form
    {

        private BindingList<ModuleInfoRow> ModuleStore = new BindingList<ModuleInfoRow>();
        private BindingList<ScanProcessInfoRow> ProcessScanStore = new BindingList<ScanProcessInfoRow>();

        public MainForm()
        {
            InitializeComponent();
            modulesDataGridView.DataSource = ModuleStore;
            processScanDataGridView.DataSource = ProcessScanStore;

            modulesDataGridView.RowsAdded += modulesDataGridView_RowsAdded;
        }

        private void modulesDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            var status = modulesDataGridView.Rows[e.RowIndex].Cells["moduleResult"].Value as string;
            Contract.Invariant(status != null, "moduleResult not string");

            if (status == "Success")
            {
                //modulesDataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
            }
            else if (status == "Unsafe")
            {
                modulesDataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Yellow;
            }
            else
            {
                modulesDataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
            }

        }

        internal class ProcessInfoRow
        {
            public readonly ProcessInfo info;

            public Icon Icon => info.Icon;
            public string PID => info.PID.ToString();
            public string Name => info.Name;
            public ProcessInfoRow(ProcessInfo info)
            {
                this.info = info;
            }
        }

        internal class ScanProcessInfoRow : ProcessInfoRow
        {
            public string Result => info.Result.ToString();
            public ScanProcessInfoRow(ProcessInfo info)
                :base(info)
            {

            }
        }
        
        internal class ModuleInfoRow
        {
            public readonly ModuleInfo info;

            //public Icon Icon => info.Icon;
            public string Name => info.Name;
            public string Result => info.Result;
            public ModuleInfoRow(ModuleInfo info)
            {
                this.info = info;
            }
        }


        internal List<ProcessInfoRow> RefreshProcessGrid()
        {
            return Program.scanner.Refresh().Select(p => new ProcessInfoRow(p)).ToList();
        }

        private async void refreshButton_Click(object sender, EventArgs e)
        {
            refreshButton.Enabled = false;
            refreshButton.Text = "Refreshing...";
            processesDataGridView.DataSource = await Task.Run(() => RefreshProcessGrid());
            refreshButton.Text = "Refresh";
            refreshButton.Enabled = true;
        }


        private void AddModuleHandler(ModuleInfo info)
        {
            if (InvokeRequired)
            {
                modulesDataGridView.BeginInvoke((MethodInvoker)delegate ()
                {
                    ModuleStore.Add(new ModuleInfoRow(info));
                });
            }
        }

        private void AddProcessHandler(ProcessInfo info)
        {
            if (InvokeRequired)
            {
                processScanDataGridView.BeginInvoke((MethodInvoker)delegate ()
                {
                    ProcessScanStore.Add(new ScanProcessInfoRow(info));
                });
            }
        }

        private async void scanButton_Click(object sender, EventArgs eargs)
        {
            modulesDataGridView.Visible = true;
            scanButton.Enabled = false;
            scanButton.Text = "Scanning...";
            modulesDataGridView.Rows.Clear();
            processScanDataGridView.Rows.Clear();

            
            await Task.Run(() =>
            {
                var processList = new List<ProcessInfo>();
                foreach(var row in processesDataGridView.SelectedRows.Cast<DataGridViewRow>())
                {
                    if (row != null && row.DataBoundItem is ProcessInfoRow proc)
                    {
                        processList.Add(proc.info);
                    }
                }
                Program.scanner.BeginScan(processList, AddModuleHandler, AddProcessHandler);
            });
            
            scanButton.Text = "Scan";
            scanButton.Enabled = true;
        }
    }
}
