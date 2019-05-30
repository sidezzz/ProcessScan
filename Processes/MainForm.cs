using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Processes.Scanning;

namespace Processes
{
    public partial class MainForm : Form
    {

        private BindingList<ModuleInfoRow> ModuleStore = new BindingList<ModuleInfoRow>();
        private BindingList<DriverObjectInfoRow> DriverObjectStore = new BindingList<DriverObjectInfoRow>();

        public MainForm()
        {
            InitializeComponent();

            modulesDataGridView.DataSource = ModuleStore;
            modulesDataGridView.RowsAdded += modulesDataGridView_RowsAdded;

            driverObjectDataGridView.DataSource = DriverObjectStore;
            driverObjectDataGridView.RowsAdded += driverObjectDataGridView_RowsAdded;
        }

        private void modulesDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            var status = modulesDataGridView.Rows[e.RowIndex].Cells["moduleResult"].Value as string;

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

        private void driverObjectDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            var status = driverObjectDataGridView.Rows[e.RowIndex].Cells["driverObjectResult"].Value as string;

            if (status == "Success")
            {
                //modulesDataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
            }
            else
            {
                driverObjectDataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
            }

        }

        class ProcessInfoRow
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
        
        class ModuleInfoRow
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

        class DriverObjectInfoRow
        {
            public readonly DriverObjectInfo info;
            public string Name => info.Name;
            public string Result => info.Result;
            public DriverObjectInfoRow(DriverObjectInfo info)
            {
                this.info = info;
            }
        }


        List<ProcessInfoRow> RefreshProcessGrid()
        {
            return Utils.GetProcessList().Select(p => new ProcessInfoRow(p)).ToList();
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

        private async void scanButton_Click(object sender, EventArgs eargs)
        {
            scanButton.Enabled = false;
            var oldText = scanButton.Text;
            scanButton.Text = "Scanning...";
            modulesDataGridView.Rows.Clear();
     
            await Task.Run(() =>
            {
                var processList = new List<ProcessInfo>();
                foreach(var row in processesDataGridView.SelectedRows.Cast<DataGridViewRow>())
                {
                    if (row?.DataBoundItem is ProcessInfoRow proc)
                    {
                        processList.Add(proc.info);
                    }
                }
                Program.Scanner.BeginScan(processList, AddModuleHandler);
            });
            
            scanButton.Text = oldText;
            scanButton.Enabled = true;
        }

        private async void scanKernelButton_Click(object sender, EventArgs e)
        {
            scanKernelButton.Enabled = false;
            var oldText = scanKernelButton.Text;
            scanKernelButton.Text = "Scanning...";
            driverObjectDataGridView.Rows.Clear();

            
            foreach(var info in Program.Scanner.ScanKernel())
            {
                DriverObjectStore.Add(new DriverObjectInfoRow(info));
            }

            scanKernelButton.Text = oldText;
            scanKernelButton.Enabled = true;
        }
    }
}
