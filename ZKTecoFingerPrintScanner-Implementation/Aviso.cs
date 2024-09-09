using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZKTecoFingerPrintScanner_Implementation;
using ZKTecoFingerPrintScanner_Implementation.Helpers;

namespace BIOCHECK
{
    public partial class Aviso : Form
    {
        private ManagementZk managementZk;
        public Aviso()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void ScreenHome_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            DataManager ma = new DataManager();
            string key = ma.ReadData();
            if (key == "")
            {
                this.Hide();
            }
            else
            {
                //Finalizar procesos activos
                Aviso aviso = new Aviso();
                Login login = new Login();
                LoadingForm loadingForm = new LoadingForm();
                aviso.FormClosed += ScreenHome_FormClosed;
                login.FormClosed += ScreenHome_FormClosed;
                loadingForm.FormClosed += ScreenHome_FormClosed;
                closeWindow();
            }
        }

        private void closeWindow()
        {
            try
            {
                List<Form> formsToClose = new List<Form>();
                foreach (Form form in Application.OpenForms)
                {
                    formsToClose.Add(form);
                }
                foreach (Form form in formsToClose)
                {
                    form.Close();
                    form.Dispose();
                }
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }

        }
    }
}
