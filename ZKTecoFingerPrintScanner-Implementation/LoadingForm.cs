﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZKTecoFingerPrintScanner_Implementation.Helpers;
using ZKTecoFingerPrintScanner_Implementation.Services;

namespace ZKTecoFingerPrintScanner_Implementation
{
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "";
            this.BackColor = Color.White;
           // this.Opacity = 0;
        }

        private async void LoadingForm_Load(object sender, EventArgs e)
        {
            DataManager ma = new DataManager();

            string key = ma.ReadData();
            if (!string.IsNullOrEmpty(key))
            {
                AppsFitService serv = new AppsFitService();
                var isValid = await serv.VerifyDkey(new { DefaultKeyEmpresa = key });
                if (isValid.Success)
                {
                    ScreenHome home = new ScreenHome();
                    DataSession.DKey = key;
                    DataSession.Unidad = isValid.Data.unidad;
                    DataSession.Sede = isValid.Data.sede;
                    DataSession.Rubro = isValid.Data.rubro;
                    DataSession.Logo = isValid.Data.image;
                    DataSession.Name = isValid.Data.name;

                    this.Hide();
                    home.FormClosed += LoadingForm_FormClosed;
                    home.ShowDialog();

                }
                else
                {
                    Login login = new Login();
                    this.Hide();
                    login.FormClosed += LoadingForm_FormClosed;
                    login.ShowDialog();

                }
            }
            else
            {
                this.Hide();
                Login login = new Login();
                login.FormClosed += LoadingForm_FormClosed;
                login.ShowDialog();

                //this.FormClosed += LoadingForm_FormClosed;
                //this.Hide();

                //Login login = new Login();
                //login.ShowDialog();

            }

        }
       

        private void LoadingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
