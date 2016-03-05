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
    public partial class UrlVoiceCommands : Form
    {

        public Dictionary<string, string> VoiceCommands;

        public UrlVoiceCommands()
        {
            InitializeComponent();
        }

        private void UrlVoiceCommands_Load(object sender, EventArgs e)
		{
			CreateDataGrid();			
		}

		private void AddButton_Click(object sender, EventArgs e)
		{
            try
            {
                VoiceCommands.Add("", "");
            }
            catch (ArgumentException)
            {
                //Do nothing
            }
			CreateDataGrid();			
		}

		private void dataGridView1_CellContentClick ( object sender, DataGridViewCellEventArgs e )
		{

			DataGridView dgv = this.dataGridView1;
			int currentrow = e.RowIndex;

			if (currentrow >= 0)
			{
                if (dgv.Rows[currentrow].Cells["delete"].ColumnIndex == e.ColumnIndex)
                {

                    VoiceCommands.Remove((string)dgv.Rows[currentrow].Cells["Command"].Value);
                    dgv.Rows.Remove(dgv.Rows[currentrow]);

                }
			}			
		}


		private void CreateDataGrid()
		{
			DataTable table = new DataTable( "table" );
			DataColumn colItem1 = new DataColumn( "Command", Type.GetType( "System.String" ) );
            DataColumn colItem2 = new DataColumn("Url Voice Commands", Type.GetType("System.String"));
            
			table.Columns.Add( colItem1 );
			table.Columns.Add( colItem2 );

			foreach ( KeyValuePair<string, string> pair in VoiceCommands )
			{
				DataRow NewRow;
				NewRow = table.NewRow();
				NewRow ["Command"] = pair.Key;
				NewRow ["Url Voice Commands"] = pair.Value;

				table.Rows.Add( NewRow );
			}

			this.dataGridView1.DataSource = table;
            this.dataGridView1.Sort(dataGridView1.Columns["Command"], ListSortDirection.Ascending);
            this.dataGridView1.AllowUserToAddRows = false;
			DataGridView dgv = this.dataGridView1;

			dgv.Columns ["Command"].Width = 350;

            dgv.Columns["Url Voice Commands"].Width = 500;
            
			int i = 0;
			foreach ( var r in dgv.Rows )
			{
				dgv.AutoResizeRow( i++ );
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

            if(dupl) return;
			
            try
			{
                StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\urlCommands.txt", false);
                VoiceCommands.Clear();
				foreach ( DataGridViewRow r in dataGridView1.Rows )
				{
					if ( r.Index == 0 )
                        sw.Write("{0}={1}", r.Cells["Command"].Value, r.Cells["Url Voice Commands"].Value);
					else if ( r.Index != dataGridView1.Rows.Count)
                        sw.Write("\n{0}={1}", r.Cells["Command"].Value, r.Cells["Url Voice Commands"].Value);
                    VoiceCommands.Add(r.Cells["Command"].Value.ToString(), r.Cells["Url Voice Commands"].Value.ToString());
				}

				sw.Close();
			}
			catch ( IOException ex )
			{
				Console.WriteLine( ex );
			}
		}

        private void UrlVoiceCommands_FormClosing(object sender, FormClosingEventArgs e)
        {
            KinectSetupDev.MainWindow.urlbo = false;
            MessageBox.Show(" Προσοχή! Οι αλλαγές που έχετε κάνει θα ισχύσουν μετά την επανεκκίνηση της εφαρμογής.");
        }
	}
}

