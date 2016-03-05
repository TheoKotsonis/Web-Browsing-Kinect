using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;

namespace KinectSetupDev
{
    public partial class SearchVoiceCommands : Form
    {

        public List<string> VSCmd;

        public SearchVoiceCommands()
        {
            InitializeComponent();
        }

        private void SearchVoiceCommands_Load(object sender, EventArgs e)
        {
            CreateDataGrid();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            try
            {
                VSCmd.Add("");
            }
            catch (ArgumentException)
            {
                //Do nothing
            }
            CreateDataGrid();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = this.dataGridView1;
            int currentrow = e.RowIndex;

            if (currentrow >= 0)
            {
                if (dgv.Rows[currentrow].Cells["delete"].ColumnIndex == e.ColumnIndex)
                {

                    VSCmd.Remove((string)dgv.Rows[currentrow].Cells["Command"].Value);
                    dgv.Rows.Remove(dgv.Rows[currentrow]);

                }
            }
        }

        private void CreateDataGrid()
        {
            DataTable table = new DataTable("table");
            DataColumn colItem1 = new DataColumn("Command", Type.GetType("System.String"));

            table.Columns.Add(colItem1);

            foreach (string key in VSCmd)
            {
                DataRow NewRow;
                NewRow = table.NewRow();
                NewRow["Command"] = key;

                table.Rows.Add(NewRow);
            }

            this.dataGridView1.DataSource = table;
            this.dataGridView1.Sort(dataGridView1.Columns["Command"], ListSortDirection.Ascending);
            this.dataGridView1.AllowUserToAddRows = false;
            DataGridView dgv = this.dataGridView1;

            dgv.Columns["Command"].Width = 350;

            int i = 0;
            foreach (var r in dgv.Rows)
            {
                dgv.AutoResizeRow(i++);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            bool dupl = false;
            bool isEmpty = false;
            DataGridView grv = this.dataGridView1;

            for (int currentRow = 0; currentRow < grv.Rows.Count; currentRow++)
            {
                DataGridViewRow rowToCompare = grv.Rows[currentRow];
                if (rowToCompare.Cells["Command"].Value.ToString() == "")
                {
                    isEmpty = true;
                    break;
                }
                for (int otherRow = currentRow + 1; otherRow < grv.Rows.Count; otherRow++)
                {

                    DataGridViewRow row = grv.Rows[otherRow];

                    bool duplicateRow = true;

                    if (!rowToCompare.Cells["Command"].Value.Equals(row.Cells["Command"].Value))
                    {
                        duplicateRow = false;
                    }

                    if (duplicateRow)
                    {
                        System.Windows.Forms.MessageBox.Show("Η ληξη " + row.Cells["Command"].Value.ToString().ToUpperInvariant() + " υπάρχει παραπάνω φορες στον πινακα");
                        dupl = true;
                    }
                }
            }

            if (isEmpty)
            {
                System.Windows.Forms.MessageBox.Show("Υπαρχει κενη γραμμη στο πινακα. Διαγραψτε την για να αποθηκευσετε");
                return;
            }

            if (dupl) return;

            try
            {
                StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\searchCommands.txt", false);
                VSCmd.Clear();
                foreach (DataGridViewRow r in dataGridView1.Rows)
                {
                    if (r.Index == 0)
                        sw.Write("{0}", r.Cells["Command"].Value);
                    else if (r.Index != dataGridView1.Rows.Count)
                        sw.Write("\n{0}", r.Cells["Command"].Value);
                    VSCmd.Add(r.Cells["Command"].Value.ToString());
                }

                sw.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SearchVoiceCommands_FormClosing(object sender, FormClosingEventArgs e)
        {
            KinectSetupDev.MainWindow.searchbo = false;
            MessageBox.Show(" Προσοχή! Οι αλλαγές που έχετε κάνει θα ισχύσουν μετά την επανεκκίνηση της εφαρμογής.");
        }

    }
}
