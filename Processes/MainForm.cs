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
using Processes.ThirdParty;

namespace Processes
{
    public partial class MainForm : Form
    {
        private SortableBindingList<ProcessInfoRow> ProcessStore = new SortableBindingList<ProcessInfoRow>();
        private SortableBindingList<ModuleInfoRow> ModuleStore = new SortableBindingList<ModuleInfoRow>();
        private BindingList<DriverObjectInfoRow> DriverObjectStore = new BindingList<DriverObjectInfoRow>();

        public MainForm()
        {
            InitializeComponent();

            processesDataGridView.DataSource = ProcessStore;

            modulesDataGridView.DataSource = ModuleStore;
            modulesDataGridView.RowsAdded += modulesDataGridView_RowsAdded;
            modulesDataGridView.Sorted += modulesDataGridView_Sorted;

            driverObjectDataGridView.DataSource = DriverObjectStore;
            driverObjectDataGridView.RowsAdded += driverObjectDataGridView_RowsAdded;

            scanKernelButton.Enabled = Program.Scanner.IsDriverLoaded;
            Program.Scanner.ModuleScanned += OnModuleAdd;
            Program.Scanner.ModuleScanned += OnModuleScanned;
        }


        void SetRowColor(DataGridViewRow row)
        {
            var status = row.Cells["moduleResult"].Value as string;

            if (status == "Success")
            {
                row.DefaultCellStyle.BackColor = Color.White;
            }
            else if (status == "Unsafe")
            {
                row.DefaultCellStyle.BackColor = Color.Yellow;
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.Red;
            }
        }
        private void modulesDataGridView_Sorted(object sender, EventArgs e)
        {
            for (int a = 0; a < modulesDataGridView.Rows.Count; a++)
            {
                SetRowColor(modulesDataGridView.Rows[a]);
            }
        }

        private void modulesDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            SetRowColor(modulesDataGridView.Rows[e.RowIndex]);
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
            var processList = await Task.Run(() => RefreshProcessGrid());
            ProcessStore.Clear();
            ProcessStore.AddRange(processList);
            refreshButton.Text = "Refresh";
            refreshButton.Enabled = true;
        }



        HashSet<string> ExistedModules = new HashSet<string>();
        private void OnModuleAdd(ModuleInfo module)
        {
            if (module.Result != null)
            {
                if (InvokeRequired)
                {
                    modulesDataGridView.BeginInvoke(new MethodInvoker(() => OnModuleAdd(module)));
                }
                else
                {
                    if (!ExistedModules.Contains(module.Path))
                    {
                        ExistedModules.Add(module.Path);
                        ModuleStore.Add(new ModuleInfoRow(module));
                    }
                }
            }
        }

        private void OnModuleScanned(ModuleInfo module)
        {
            if (InvokeRequired)
            {
                scanProgressBar.BeginInvoke(new MethodInvoker(() => OnModuleScanned(module)));
            }
            else
            {
                scanProgressBar.Value++;
            }
        }

        private async void scanButton_Click(object sender, EventArgs eargs)
        {
            scanButton.Enabled = false;
            var oldText = scanButton.Text;
            scanButton.Text = "Scanning...";
            modulesDataGridView.Rows.Clear();
            ExistedModules.Clear();

            scanProgressBar.Value = 0;
            await Task.Run(() =>
            {
                int moduleCount = 0;
                var processList = new List<ProcessInfo>();
                foreach(var row in processesDataGridView.SelectedRows.Cast<DataGridViewRow>())
                {
                    if (row?.DataBoundItem is ProcessInfoRow proc)
                    {
                        moduleCount+= proc.info.Modules.Count; ;
                        processList.Add(proc.info);
                    }
                }
                scanProgressBar.BeginInvoke((MethodInvoker)delegate ()
                {
                    scanProgressBar.Maximum = moduleCount;
                });

                Program.Scanner.BeginScan(processList);
            });

            scanProgressBar.Value = scanProgressBar.Maximum;
            scanButton.Text = oldText;
            scanButton.Enabled = true;
            scanTabControl.SelectedIndex = 0;
        }

        private void scanKernelButton_Click(object sender, EventArgs e)
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
            scanTabControl.SelectedIndex = 1;
        }
    }
}
