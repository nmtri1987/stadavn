using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RBVH.Stada.Intranet.ConsoleTest
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                //POCImportShift pOCImportShift = new POCImportShift();
                //pOCImportShift.ImportExcel();

                //TestWorkflow();
                //TestFixIssue();
                // FixContentType();
                //TestDateTime(16, 8, 2015, 17, 8, 2015);
                // TestDateTime(16, 8, 2015, 16, 8, 2015);
                //TestDateTime(16, 8, 2015, 17, 8, 2015);
                //UpdateListItem();
                //TestStringFormat();
                //  UpdateEmployeeShiftTime();
                ///   TestCAML();
                //ParseXMLtoDynamic();

                // TestResource();
                // InsatllFeature();
                //SendMail();

                //SendMail();
                //DeleteGroups();

                MessageBox.Show("Done!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
