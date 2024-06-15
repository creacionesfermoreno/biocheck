﻿using MaterialSkin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Messaging;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZKTecoFingerPrintScanner_Implementation;
using ZKTecoFingerPrintScanner_Implementation.Helpers;
using ZKTecoFingerPrintScanner_Implementation.Models;
using ZKTecoFingerPrintScanner_Implementation.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BIOCHECK
{
    public partial class ScreenHomeLite : Form
    {
        private ManagementZkLite managementZk;
        private const int timeoutInSeconds = 10;
        public Label dev { get; set; }
        public Label lblSerie_ { get; set; }
        public Label lblIntents_ { get; set; }
        // public StatusBar lblMessage_ { get; set; }
        public PictureBox picHuellaMA_ { get; set; }
        public PictureBox PicRegister_ { get; set; }
        public PictureBox PicImagePersonal_ { get; set; }

        private DataSocioAll dataSocioAll;
        private STGlobal stGlobal;
        private SoundPlayer soundAccessPlayer;
        private SoundPlayer soundRestringPlayer;
        private SoundPlayer soundOtrosPlayer;
        private int SelectedIndexPersonal = 0;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn(
            int nLeft,
            int nTop,
            int nRight,
            int nButtom,
            int nWidthEllipse,
            int nHeightEllipse
        );
        public ScreenHomeLite()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            lblSerie_ = lblSerie;
            picHuellaMA_ = picHuellaMA;
            PicRegister_ = PicRegister;
            PicImagePersonal_ = pbAPHuella;
            lblIntents_ = lblIntents;

            dataSocioAll = DataSocioAll.Instance;
            stGlobal = STGlobal.Instance;
            managementZk = new ManagementZkLite(this);

            soundAccessPlayer = new SoundPlayer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\accesoSound.wav"));
            soundRestringPlayer = new SoundPlayer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\restringSound.wav"));
            soundOtrosPlayer = new SoundPlayer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\soundOtros.wav"));


            btnMarkAsistence.Region = Region.FromHrgn(CreateRoundRectRgn
                (0, 0, btnMarkAsistence.Width, btnMarkAsistence.Height, 30, 30));

            panelMA.Region = Region.FromHrgn(CreateRoundRectRgn
                (0, 0, panelMA.Width, panelMA.Height, 30, 30));

            panelDeviceConnect.Region = Region.FromHrgn(CreateRoundRectRgn
                (0, 0, panelDeviceConnect.Width, panelDeviceConnect.Height, 20, 20));

            panelIntentos.Region = Region.FromHrgn(CreateRoundRectRgn
                (0, 0, panelIntentos.Width, panelIntentos.Height, 20, 20));

            BtnConnectionNew.Region = Region.FromHrgn(CreateRoundRectRgn
                (0, 0, BtnConnectionNew.Width, BtnConnectionNew.Height, 20, 20));

            BtnDisconnectionNew.Region = Region.FromHrgn(CreateRoundRectRgn
                (0, 0, BtnDisconnectionNew.Width, BtnDisconnectionNew.Height, 20, 20));
            //Control asistencia
            panelCAUser.Region = Region.FromHrgn(CreateRoundRectRgn
                (0, 0, panelCAUser.Width, panelCAUser.Height, 20, 20));

            panelAPHuella.Region = Region.FromHrgn(CreateRoundRectRgn
                (0, 0, panelAPHuella.Width, panelAPHuella.Height, 20, 20));

            panelDeviceConnect.Region = Region.FromHrgn(CreateRoundRectRgn
                (0, 0, panelDeviceConnect.Width, panelDeviceConnect.Height, 20, 20));

        }
        //center control
        public void CenterControl(Control parent, Control child)
        {
            int x = 0;
            x = (parent.Width / 2) - (child.Width / 2);
            child.Location = new System.Drawing.Point(x, child.Location.Y);
        }

        //*************************************************** OnEventPersonal ***************************************************
        public void TOneControl(HorarioFijo hfijo, bool clear = false)
        {
            lblT1M1.Text = clear ? "0:00" : hfijo.FechaHoraIngresoTexto ?? "0:00";
            lblT1M2.Text = clear ? "0:00" : hfijo.FechaHoraRefrigerioSalidaTexto ?? "0:00";
            lblT1M3.Text = clear ? "0:00" : hfijo.FechaHoraRefrigerioRetornoTexto ?? "0:00";
            lblT1M4.Text = clear ? "0:00" : hfijo.FechaHoraSalidaTexto ?? "0:00";
        }
        public void TTwoControl(HorarioFijo hfijo, bool clear = false)
        {
            lblT2M1.Text = clear ? "0:00" : hfijo.FechaHoraIngreso_TurnoTardeTexto ?? "0:00";
            lblT2M2.Text = clear ? "0:00" : hfijo.FechaHoraRefrigerioSalida_TurnoTardeTexto ?? "0:00";
            lblT2M3.Text = clear ? "0:00" : hfijo.FechaHoraRefrigerioRetorno_TurnoTardeTexto ?? "0:00";
            lblT2M4.Text = clear ? "0:00" : hfijo.FechaHoraSalida_TurnoTardeTexto ?? "0:00";
        }
        public void InfoPersonal(SocioModel socio, bool clear = false)
        {
            lblPName.Text = clear ? "NOMBRE, APELLIDOS" : $"{socio.Nombre} {socio.Apellidos}".ToUpper();
            lblPCargo.Text = clear ? "CARGO" : $"{socio.Descripcion}".ToUpper();
            lblPPhone.Text = clear ? "CELULAR" : $"{socio.Celular}".ToUpper();

            if (clear == false)
            {
                if (string.IsNullOrEmpty(socio.ImagenUrl) == false && validateHttps(socio.ImagenUrl) == true)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        byte[] imageData = webClient.DownloadData(socio.ImagenUrl);
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            pboxPImage.Image = Image.FromStream(ms);
                        }
                    }
                }
            }
            else
            {
                pboxPImage.Image = BIOCHECK.Properties.Resources.user2;
            }
        }

        //Render Horario profesional
        private void RenderLViewProfesionales()
        {
            try
            {
                HorarioSigleton hinstance = HorarioSigleton.Instance;
                var HorariosProfesionales = hinstance.HorarioProfesional;

                if (HorariosProfesionales != null && HorariosProfesionales.Count > 0)
                {
                    lboxProfesionales.Items.Clear();


                    const int claseColumnWidth = 95;
                    const int ingresoColumnWidth = 20;
                    const int salidaColumnWidth = 20;


                    Font listBoxFont = new Font("Microsoft YaHei", 10);
                    lboxProfesionales.Font = listBoxFont;


                    lboxProfesionales.Items.Add(string.Format("{0,-" + 155 + "} {1,-" + ingresoColumnWidth + "} {2,-" + salidaColumnWidth + "}", "CLASE", "INGRESO", "SALIDA"));
                    lboxProfesionales.Items.Add(new string('-', 100 + ingresoColumnWidth + salidaColumnWidth));

                    foreach (HorarioProfesional p in HorariosProfesionales)
                    {
                        String clase = $"{p.DesSala} - {p.Disciplina} {p.HoraInicioTexto} - {p.HoraFinTexto} AFORO {p.CantidadAsistencias} DE {p.CapacidadPermitida}";

                        int w = 0;
                        if (clase.Length <= claseColumnWidth)
                        {
                            w = (claseColumnWidth - Convert.ToInt32(clase.Length));
                            if (w > 30)
                            {
                                clase = w + clase + new string('_', w + 5);

                            }
                            else
                            {

                                clase = w + clase + new string('_', w);
                            }
                        }
                        string ingresoAlineado = string.IsNullOrEmpty(p.FechaHoraIngresoTxt) ? "--:--" : p.FechaHoraIngresoTxt;
                        string salidaAlineada = string.IsNullOrEmpty(p.FechaHoraSalidaTxt) ? "--:--" : p.FechaHoraSalidaTxt;
                        lboxProfesionales.Items.Add(string.Format("{0,-" + w + "} {1,-" + ingresoColumnWidth + "} {2,-" + salidaColumnWidth + "}", clase, ingresoAlineado, salidaAlineada));

                    }

                }
                else
                {
                    lboxProfesionales.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreenHomeLite", ex);
            }
        }

        //Render Horario fijo
        public void renderHorarioFijo(HorarioFijo hfijo)
        {
            managementZk.createFile($"renderHorarioFijo: {hfijo.tipoTurno}");
            managementZk.createFile($"renderHorarioFijo: {hfijo.FechaHoraRefrigerioSalidaTexto}");
            if (hfijo.tipoTurno == 1)
            {
                // T1
                pTurno1.Visible = true;
                pTurno2.Visible = false;
                TOneControl(hfijo);
            }
            else if (hfijo.tipoTurno == 2)
            {
                // T2
                pTurno2.Visible = true;
                pTurno1.Visible = false;
                TTwoControl(hfijo);
            }
            else if (hfijo.tipoTurno == 3)
            {
                //T1 and T2
                pTurno1.Visible = true;
                pTurno2.Visible = true;
                TOneControl(hfijo);
                TTwoControl(hfijo);
            }
            else if (hfijo.tipoTurno == 0)
            {
                //T1
                pTurno1.Visible = true;
                TOneControl(hfijo);

            }
            else
            {
                Console.WriteLine("");
            }
        }


        //mark fijo
        public async Task MarckPersonalFijoAsync(int operation)
        {
            try
            {
                AppsFitService api = new AppsFitService();
                HorarioSigleton inst = HorarioSigleton.Instance;
                if (inst.SelectedPersonal)
                {
                    var body = new
                    {
                        CodigoUnidadNegocio = DataSession.Unidad,
                        CodigoSede = DataSession.Sede,
                        NumeroDocumento = inst.Personal.DNI,
                        Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        OperacionMarcacion = operation
                    };
                    var resp = await api.MarcarPersonalFijo(body);

                    if (resp.Success)
                    {
                        await managementZk.DataHorariosPersonalFijoReload();
                        renderHorarioFijo(inst.HorarioFijo);
                        StatusMessageD($"ASISTENCIA REGISTRADA", true, false);
                        inst.SelectedPersonal = false;
                        cleanMesajeAP();
                    }
                    else
                    {
                        StatusMessageD($"{resp.Message1}", false, false);
                        cleanMesajeAP();
                    }
                }
                else
                {
                    StatusMessageD($"", false, true);
                    MessageBox.Show("PARA MARCAR DEBE BUSCAR UN PERSONAL VÁLIDO");
                    cleanMesajeAP();
                }
            }
            catch (Exception ex)
            {

                managementZk.createFileLog("ScreenHomeLite", ex);
            }
        }


        public async Task MarkPersonalProfesional(int type)
        {
            try
            {
                AppsFitService api = new AppsFitService();
                HorarioSigleton hsgt = HorarioSigleton.Instance;
                var Horario = hsgt.HorarioProfesional[SelectedIndexPersonal];
                var body = new
                {
                    CodigoUnidadNegocio = DataSession.Unidad,
                    CodigoSede = DataSession.Sede,
                    CodigoHorarioClasesConfiguracion = Horario.CodigoHorarioClasesConfiguracion,
                    CodigoHorarioClasesTiempoReal = Horario.CodigoHorarioClasesTiempoReal ?? "",
                    CodigoProfesional = Horario.CodigoProfesional,
                    CodigoPersonalAsistencia = Horario.CodigoPersonalAsistencia ?? "",
                    DiaNumero = Horario.DiaNumero,
                    Fecha = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),
                    TipoAsistencia = type
                };

                var resp = await api.MarcarPersonalProfesional(body);
                if (resp.Success)
                {
                    await managementZk.DataHorariosProfesionalReload();
                    RenderLViewProfesionales();
                    StatusMessageD($"ASISTENCIA REGISTRADA", true, false);
                    hsgt.SelectedPersonal = false;
                    cleanMesajeAP();
                }
                else { 
                    StatusMessageD($"{resp.Message1}", false, false);
                    cleanMesajeAP();
                }
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreenHomeLite", ex);
            }
        }

        //*************************************************** *** ***************************************************

        public void OnEventPersonal(string type)
        {
            try
            {
                HorarioSigleton hinstance = HorarioSigleton.Instance;

                switch (type)
                {
                    case "LHF":
                        renderHorarioFijo(hinstance.HorarioFijo);
                        InfoPersonal(hinstance.Personal);
                        StatusMessage(hinstance.Message, true);
                        StatusMessageD($"", false, true);

                        break;
                    case "LHF-F":
                        TOneControl(new HorarioFijo(), true);
                        TTwoControl(new HorarioFijo(), true);
                        InfoPersonal(new SocioModel(), true);
                        StatusMessage("NO SE ENCONTRO PERSONAL", false);
                        StatusMessageD($"", false, true);
                        cleanPersonalN();
                        break;

                    //profesionales
                    case "LHPROF":
                        RenderLViewProfesionales();
                        InfoPersonal(hinstance.Personal);
                        StatusMessage(hinstance.Message, true);
                        StatusMessageD($"", false, true);
                        break;

                    case "LHPROF-F":
                        lboxProfesionales.Items.Clear();
                        StatusMessage("NO SE ENCONTRO PERSONAL", false);
                        StatusMessageD($"", false, true);
                        cleanPersonalN();
                        break;

                    default:
                        Console.WriteLine("");
                        break;
                }
            }
            catch (Exception ex)
            {

                managementZk.createFileLog("ScreenHomeLite", ex);
            }
        }

        public void OnEventGeneral(string message, dynamic message2)
        {

            try
            {
                switch (message)
                {
                    case "MA-T":
                        if (dataSocioAll.Membresias != null && dataSocioAll.Membresias.Count > 0)
                        {
                            var membresia = dataSocioAll.Membresias[0];
                            //txtMPromo.Text = membresia.NombrePaquete.ToString();
                            txtMFInicio.Text = membresia.DesFechaInicio.ToString();
                            txtMFin.Text = membresia.DesFechaFin.ToString();
                            txtMPrecio.Text = membresia.Costo.ToString();
                            txtMFrezing.Text = membresia.CantidadFreezing.ToString();
                            txtMFrezingTom.Text = membresia.CantidadFreezingTomados.ToString();
                            txtMFrezingActual.Text = membresia.CantidadAsistencia.ToString();

                            btnMarkAsistence.Visible = true;
                            lblPlan.Text = membresia.Descripcion.ToUpper();
                            MessageStatusMembresia(membresia.ObtenerTiempoVencimiento, membresia.Estado);

                            dataSocioAll.MembresiasSelected = membresia;
                            string deudaMem = dataSocioAll.MembresiasSelected.Debe > 0 ? $"DEBE {dataSocioAll.MembresiasSelected.Debe} EN MEMBRESIA" : "";
                            StlyDeudaM(deudaMem, !string.IsNullOrEmpty(deudaMem));
                            //btnVerDeudaProducto.Visible = true;
                            if (stGlobal.CheackAutomatic)
                            {
                                btnMarkAsistence.PerformClick();
                            }


                            StatusMessage(dataSocioAll.MessageGenericD, true);
                            SocioInfoMatch(true);
                        }

                        break;
                    case "MA-F":
                        soundRestringValidate();
                        SocioInfoMatch(false);
                        //StatusMessage($"No se encontro socio {DateTime.Now}", false);
                        MessageStatusMembresia("ESTADO DE MEMBRESIA", 0, true);
                        StatusMessageD("", false, true);
                        lblPlan.Text = "";
                        clearMembresiaText();
                        lblMessage.Message = $"No se encontro socio {DateTime.Now}";
                        lblMessage.StatusBarBackColor = Color.FromArgb(230, 112, 134);
                        panelMA.BackColor = Color.FromArgb(230, 112, 134);
                        StlyDeudaM("", false);
                        StlyDeuda("", false);
                        cleanSocioN();
                        break;
                    case "REG":
                        StatusMessage("Registro de huella exitosa", true);
                        PicRegister.Image = BIOCHECK.Properties.Resources.huell;
                        _ = LoadFingers(false);
                        break;
                    case "REG-FAIL":
                        StatusMessage(DataStatic.MessageGeneric, false);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }

        }

        //clear txt membresia
        public void clearMembresiaText()
        {
            txtMPromo.Text = "";
            txtMFInicio.Text = "";
            txtMFin.Text = "";
            txtMPrecio.Text = "";
            txtMFrezing.Text = "";
            txtMFrezingTom.Text = "";
            txtMFrezingActual.Text = "";
            btnVerDeudaProducto.Visible = false;
        }

        private async void cleanSocioN()
        {
            await Task.Delay(1000);
            picHuellaMA.Image = BIOCHECK.Properties.Resources.huell;
            lblMessage.Message = "";
            lblMessage.StatusBarBackColor = Color.Transparent;
            panelMA.BackColor = Color.FromArgb(233, 233, 233);
        }

        private async void cleanMesajeAP()
        {
            await Task.Delay(2000);
            StatusMessageD("", false, true);
            lblMessage.Message = "";
            lblMessage.StatusBarBackColor = Color.Transparent;
        }
        private async void cleanPersonalN()
        {
            await Task.Delay(2000);
            pbAPHuella.Image = BIOCHECK.Properties.Resources.huell;
            lblMessage.Message = "";
            lblMessage.StatusBarBackColor = Color.Transparent;
        }

        private void soundAccessValidate()
        {
            if (chkSonido.Checked)
            {
                soundAccessPlayer.Play();
            }
            else
            {
                soundAccessPlayer.Stop();
            }

        }
        private void soundRestringValidate()
        {
            if (chkSonido.Checked)
            {
                soundRestringPlayer.Play();
            }
            else
            {
                soundRestringPlayer.Stop();
            }

        }

        private void soundOtrosValidate()
        {
            if (chkSonido.Checked)
            {
                soundOtrosPlayer.Play();
            }
            else
            {
                soundOtrosPlayer.Stop();
            }

        }

        //set socio match
        private void SocioInfoMatch(bool success)
        {
            try
            {
                if (success)
                {
                    var socio = dataSocioAll.Socio;
                    lblFullName_.Text = $"{socio.Nombre}, {socio.Apellidos}".ToUpper();
                    string deudaStr = socio.DeudaSuplemento > 0 ? $"DEBE {socio.DeudaSuplemento} EN PRODUCTOS" : "";
                    //StlyDeuda(deudaStr, deudaStr.Length > 0 ? true : false);
                    if (string.IsNullOrEmpty(socio.ImagenUrl) == false && validateHttps(socio.ImagenUrl) == true)
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] imageData = webClient.DownloadData(socio.ImagenUrl);
                            using (MemoryStream ms = new MemoryStream(imageData))
                            {
                                PicMaUser.Image = Image.FromStream(ms);
                            }
                        }
                    }
                }
                else
                {
                    lblFullName_.Text = "NOMBRES, APELLIDOS COMPLETOS";
                    StlyDeuda();
                    PicMaUser.Image = BIOCHECK.Properties.Resources.user2;
                }
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }
        }

        private void MessageStatusMembresia(string message, int status, bool clear = false)
        {
            try
            {
                string finish = "😨 SU MEMBRESÍA FINALIZÓ 👎";
                lblMessageMem.Text = status == 2 ? finish : message;
                lblMessageMem.ForeColor = Color.White;
                lblMessageMem.BackColor = clear ? Color.FromArgb(37, 47, 59) : (status == 1 ? Color.Green : Color.Gray);
                tlBgEstadoMembresia.BackColor = clear ? Color.FromArgb(37, 47, 59) : (status == 1 ? Color.Green : Color.Gray);
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }
        }

        private void StlyDeuda(string message = "", bool deuda = false)
        {
            try
            {
                lblDeudaProductos.Text = string.IsNullOrEmpty(message) ? "DEUDA PRODUCTOS" : message;
                lblDeudaProductos.ForeColor = Color.White;
                lblDeudaProductos.BackColor = deuda ? Color.FromArgb(192, 0, 0) : Color.Gray;
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }
        }
        private void StlyDeudaM(string message = "", bool deuda = false)
        {
            try
            {
                lblDeudaMembresia.Text = string.IsNullOrEmpty(message) ? "DEUDA MEMBRESIA" : message;
                lblDeudaMembresia.ForeColor = Color.White;
                lblDeudaMembresia.BackColor = deuda ? Color.FromArgb(192, 0, 0) : Color.Gray;
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }
        }

        MaterialSkinManager skinManager = MaterialSkinManager.Instance;

        private bool isFirstLoad = true;
        private async void ScreenHomeLite_Load(object sender, EventArgs e)
        {

            LoadingForm loading = new LoadingForm();
            loading.Opacity = 0;
            loading.Visible = false;
            loading.Close();

            skinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            Color customColor = Color.FromArgb(0, 80, 200);

            skinManager.ColorScheme = new ColorScheme(
                Primary.Blue900,
                Primary.Blue600,
                Primary.Blue600,
                Accent.Amber700,
                TextShade.WHITE
            );

            if (isFirstLoad)
            {
                if (TabControl.SelectedTab == tabPage1)
                {
                    //handle match
                    managementZk.SetMatchSearch(true);
                    _ = LoadFingers(true);
                }
                isFirstLoad = false;
            }
            //date format 
            DateFormat();

            //loadData 
            loadInitialData();

        }

        private void loadInitialData()
        {
            try
            {
                lblGym.Text = DataSession.Name;
                lblRubro.Text = DataSession.Rubro;
                if (!string.IsNullOrEmpty(DataSession.Logo))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        byte[] imageData = webClient.DownloadData(DataSession.Logo);
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            picGymLogo.Image = Image.FromStream(ms);
                        }
                    }
                }

                DataManagerD dpre = new DataManagerD();
                DataManagerTime dtime = new DataManagerTime();
                DataManagerSonido dsonido = new DataManagerSonido();
                if (string.IsNullOrEmpty(dpre.ReadData()) || string.IsNullOrEmpty(dtime.ReadData()) || string.IsNullOrEmpty(dsonido.ReadData()))
                {
                    tbPrecicion.Value = 60;
                    lblPrecicion.Text = $"60%";
                    stGlobal.Precision = 60;

                    //tiempo
                    nudTiempoInfo.Value = 5;
                    stGlobal.TiempoInfo = 5;

                    //sonido
                    chkSonido.Checked = false;
                    stGlobal.SonidoAsistencia = false;
                }
                else
                {
                    var pre = dpre.ReadData();
                    tbPrecicion.Value = Convert.ToInt32(pre);
                    lblPrecicion.Text = $"{pre}%";
                    stGlobal.Precision = Convert.ToInt32(pre);

                    var tim = dtime.ReadData();
                    nudTiempoInfo.Value = Convert.ToInt32(tim);

                    var sonid = dsonido.ReadData();
                    chkSonido.Checked = Convert.ToBoolean(sonid);
                }
                rbTSocio.Checked = true;
                stGlobal.TypeRegister = 1;
                stGlobal.TypeMatch = 1;
                stGlobal.KeyEmpresa = DataSession.DKey;
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }
        }

        public async Task LoadFingers(bool init = true)
        {
            AppsFitService api = new AppsFitService();
            try
            {
                HuellaData huellaData = HuellaData.Instance;

                if (init)
                {
                    var response = await api.FingerPrintsList(new { DefaultKeyEmpresa = DataSession.DKey });
                    if (response.Success)
                    {
                        huellaData.SetSocios(response.Data);
                    }

                    var responseF = await api.FingerPrintsListFijo(new { DefaultKeyEmpresa = DataSession.DKey });
                    if (responseF.Success)
                    {
                        huellaData.SetFijos(responseF.Data);
                    }

                    var responseEvent = await api.FingerPrintsListEvent(new { DefaultKeyEmpresa = DataSession.DKey });
                    if (responseEvent.Success)
                    {
                        huellaData.SetProfesionales(responseEvent.Data);
                    }
                }
                else
                {
                    switch (stGlobal.TypeRegister)
                    {
                        case 2:
                            var responseF = await api.FingerPrintsListFijo(new { DefaultKeyEmpresa = DataSession.DKey });
                            if (responseF.Success)
                            {
                                huellaData.SetFijos(responseF.Data);
                            }
                            break;
                        case 3:
                            var responseEvent = await api.FingerPrintsListEvent(new { DefaultKeyEmpresa = DataSession.DKey });
                            if (responseEvent.Success)
                            {
                                huellaData.SetProfesionales(responseEvent.Data);
                            }
                            break;
                        default:
                            var response = await api.FingerPrintsList(new { DefaultKeyEmpresa = DataSession.DKey });
                            if (response.Success)
                            {
                                huellaData.SetSocios(response.Data);
                            }
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }

        }

        private void StatusMessage(string message, bool success, bool reset = false)
        {

            try
            {
                if (reset)
                {
                    lblMessage.Message = "";
                    lblMessage.StatusBarBackColor = Color.Transparent;
                }
                else
                {
                    lblMessage.Message = message;
                    lblMessage.StatusBarForeColor = Color.Transparent;
                    if (success)
                    {

                        lblMessage.StatusBarBackColor = Color.FromArgb(79, 208, 154);
                    }
                    else
                    {
                        lblMessage.StatusBarBackColor = Color.FromArgb(230, 112, 134);
                    }
                }
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }
        }

        private void StatusMessageD(string message, bool success, bool reset = false)
        {

            try
            {
                statusBar1.Message = message;
                statusBar1.StatusBarForeColor = Color.Transparent;

                if (reset)
                {
                    statusBar1.StatusBarBackColor = Color.Transparent;
                }
                else
                {
                    if (success)
                    {

                        statusBar1.StatusBarBackColor = Color.FromArgb(79, 208, 154);
                    }
                    else
                    {
                        statusBar1.StatusBarBackColor = Color.FromArgb(230, 112, 134);
                    }
                }
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }
        }

        private void StatusMessageAsistencia(string message, bool success, bool reset = false)
        {

            try
            {
                lblMensajeAsistencia.Text = message;
                lblMensajeAsistencia.BackColor = Color.Transparent;

                if (reset)
                {
                    lblMensajeAsistencia.BackColor = Color.Transparent;
                }
                else
                {
                    if (success)
                    {

                        lblMensajeAsistencia.BackColor = Color.FromArgb(10, 177, 49);
                    }
                    else
                    {
                        lblMensajeAsistencia.BackColor = Color.FromArgb(230, 112, 134);
                    }
                }
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }
        }
        public bool validateHttps(string url)
        {
            try
            {
                bool valid = false;
                string patron = @"https:\/\/\S+";
                bool contieneHTTPS = Regex.IsMatch(url, patron);

                if (contieneHTTPS)
                {
                    valid = true;
                }
                else
                {
                    valid = false;
                }
                return valid;
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
                return false;
            }
        }

        //clear register huella
        public void ClearRegister()
        {
            try
            {
                lblMessage.Message = "";
                lblIntents.Text = "3 veces más";
                PicRegister.Image = BIOCHECK.Properties.Resources.huell;
                lblMessage.StatusBarBackColor = Color.Transparent;
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }
        }

        public void clearTab2()
        {
            try
            {
                lblMessage.Message = "";
                lblIntents.Text = "";
                PicRegister.Image = BIOCHECK.Properties.Resources.huell;
                ImageUser.Image = BIOCHECK.Properties.Resources.user2;
                TxtSearch.Text = "";
                TxtCode.Text = "";
                TxtName.Text = "";
                TxtSurname.Text = "";
                TxtNro.Text = "";
                lblMessage.StatusBarBackColor = Color.Transparent;
                StatusMessageD("", false, true);

            }
            catch (Exception ex) { managementZk.createFileLog("ScreeHomeLite", ex); }
        }

        public void cleanAllTab2()
        {
            //await Task.Delay(2000);//al buscar que se elimine
            
            lblIntents.Text = "";
            lblMessage.StatusBarBackColor = Color.Transparent;
            PicRegister.Image = BIOCHECK.Properties.Resources.huell;
            StatusMessageD("", false, true);
        }

        public void clearTab3()
        {
            lblMessage.Message = "";
            statusBar1.Message = "";
            lblMessage.StatusBarBackColor = Color.Transparent;
            statusBar1.StatusBarBackColor = Color.Transparent;
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                HorarioSigleton hinstnc = HorarioSigleton.Instance;

                if (TabControl.SelectedTab == tabPage1)
                {
                    managementZk.SetMatchSearch(true);
                    managementZk.SetIsRegister(false);
                    stGlobal.TypeMatch = 1;
                    cleanAllTab1();
                }
                if (TabControl.SelectedTab == tabPage2)
                {
                    clearTab2();
                    managementZk.SetMatchSearch(false);
                }

                if (TabControl.SelectedTab == tabPage3)
                {
                    clearTab3();
                    managementZk.SetIsRegister(false);
                    managementZk.SetMatchSearch(false);
                    getConfiguration();
                }
                if (TabControl.SelectedTab == tabPage4)
                {
                    managementZk.SetMatchSearch(true);
                    managementZk.SetIsRegister(false);
                    rbTMFijo.Checked = true;
                    stGlobal.TypeMatch = 2;
                    hinstnc.SelectedPersonal = false;
                    TOneControl(new HorarioFijo(), true);
                    TTwoControl(new HorarioFijo(), true);
                    StatusMessageD("", false, true);
                    InfoPersonal(new SocioModel(), true);
                    StatusMessage("", false, true);
                    pbAPHuella.Image = BIOCHECK.Properties.Resources.huell;
                    pboxPImage.Image = BIOCHECK.Properties.Resources.user2;
                }
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }
        }

        public void getConfiguration()
        {
            try
            {
                if (!string.IsNullOrEmpty(DataSession.DKey) && !string.IsNullOrEmpty(DataSession.Name))
                {
                    txtCbusiness.Text = DataSession.Name.ToString();
                    txtCsede.Text = DataSession.Sede.ToString();
                    txtCng.Text = DataSession.Unidad.ToString();
                    txtCKey.Text = DataSession.DKey.ToString();

                    if (!string.IsNullOrEmpty(DataSession.Logo))
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] imageData = webClient.DownloadData(DataSession.Logo);
                            using (MemoryStream ms = new MemoryStream(imageData))
                            {
                                PicCLogo.Image = Image.FromStream(ms);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }

        }

        //clears--------------------
        private void clearMA()
        {
            try
            {
                lblFullName_.Text = "NOMBRES, APELLIDOS COMPLETOS";
                PicMaUser.Image = null;
                lblPlan.Text = "NOMBRE DEL PLAN";
                lblMessageMem.Text = "ESTADO MEMBRESIA";
                lblDeudaMembresia.Text = "DEUDA MEMBRESIA";
                lblDeudaProductos.Text = "DEUDA PRODUCTOS";
                lblMessage.Message = "";
                lblDeudaMembresia.BackColor = Color.Gray;
                lblDeudaProductos.BackColor = Color.Gray;
                picHuellaMA.Image = BIOCHECK.Properties.Resources.huell;
                PicMaUser.Image = BIOCHECK.Properties.Resources.user2;
                lblMessageMem.BackColor = Color.Transparent;
                tlBgEstadoMembresia.BackColor = Color.FromArgb(37, 47, 59);
                StatusMessageD("", false, true);
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }
        }

        private void cleanTab4()
        {
            HorarioSigleton hinstnc = HorarioSigleton.Instance;
            InfoPersonal(new SocioModel(), true);
            TOneControl(new HorarioFijo(), true);
            TTwoControl(new HorarioFijo(), true);
            StatusMessageD("", false, true);
            InfoPersonal(new SocioModel(), true);
            StatusMessage("", false, true);
            //lblMessage.Message = "";
            //statusBar1.Message = "";
            //lblMessage.StatusBarForeColor = Color.Transparent;
            //statusBar1.StatusBarForeColor = Color.Transparent;
            pbAPHuella.Image = BIOCHECK.Properties.Resources.huell;
            pboxPImage.Image = BIOCHECK.Properties.Resources.user2;
            managementZk.SetMatchSearch(true);
            managementZk.SetIsRegister(false);
            hinstnc.SelectedPersonal = false;

        }

        private void cleanMsjAsistencia()
        {
            lblMensajeAsistencia.Text = "...";
            lblMensajeAsistencia.BackColor = Color.Silver;
        }

        private async void cleanAll()
        {
            int seg = (int)nudTiempoInfo.Value * 1000;
            await Task.Delay(seg);
            clearMembresiaText();
            clearMA();
            StatusMessageAsistencia("...", false, true);
            lblMessage.Message = "";
            lblMessage.StatusBarBackColor = Color.Transparent;
        }
        private void cleanAllTab1()
        {
            clearMembresiaText();
            clearMA();
            StatusMessageAsistencia("...", false, true);
            StlyDeudaM("", false);
            StlyDeuda("", false);
            cleanSocioN();
            lblMessage.Message = "";
            lblMessage.StatusBarBackColor = Color.Transparent;
        }

        private void DateFormat()
        {
            DateTime dnow = DateTime.Now;
            string dnowFormat = dnow.ToString("dddd d 'de' MMMM 'de' yyyy");
            lblDate.Text = dnowFormat;
        }

        // Function to validate the membership and return a message
        private string ValidateMembresia(Membresia membresia)
        {
            try
            {
                if (membresia.ObtenerDisponibilidadHorarioPaquete <= 0)
                {
                    cleanAll();
                    soundOtrosValidate();
                    return "ESTA MEMBRESIA NO TIENE ACCESO PARA ESTA SEDE.";
                }

                if (membresia.flagPaqueteSedePermiso <= 0)
                {
                    cleanAll();
                    soundOtrosValidate();
                    return "HORARIO NO DISPONIBLE";
                }

                if (membresia.NroIngreso < membresia.NroIngresoActual)
                {
                    cleanAll();
                    soundOtrosValidate();
                    return "NRO ASISTENCIAS LLEGO A SU LIMITE, REVISA EL NRO DE SESIONES DE LA MEMBRESIA";
                }
                if (membresia.Estado == 2)
                {
                    cleanAll();
                    soundOtrosValidate();
                    return "😨 SU MEMBRESÍA FINALIZÓ 👎";
                }
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
                return string.Empty;
            }
            return string.Empty;

        }

        //******************************************************+search by register+****************************************
        private async void SearchData(int type)
        {
            try
            {
                AppsFitService api = new AppsFitService();
                var body = new
                {
                    DefaultKeyEmpresa = DataSession.DKey,
                    Filtre = TxtSearch.Text,
                };

                ResponseModel response;
                switch (type)
                {
                    case 1:
                        response = await api.SearchSocio(body);
                        break;
                    case 2:
                        response = await api.SearchPF(TxtSearch.Text);
                        break;
                    default:
                        response = await api.SearchPEvent(TxtSearch.Text);
                        break;
                }

                if (response.Success)
                {
                    DataItem item = response.Data;
                    TxtCode.Text = Convert.ToString(item.code);
                    TxtName.Text = Convert.ToString(item.name);
                    TxtSurname.Text = Convert.ToString(item.surnames);
                    TxtNro.Text = Convert.ToString(item.nro_document);

                    if (!string.IsNullOrEmpty(item.image) && validateHttps(item.image))
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] imageData = webClient.DownloadData(item.image);
                            using (MemoryStream ms = new MemoryStream(imageData))
                            {
                                ImageUser.Image = Image.FromStream(ms);
                            }
                        }
                    }

                    managementZk.ResetListenRegister();
                    ClearRegister();
                    DataSession.Filtre = TxtSearch.Text;
                    DataSession.Code = item.code;
                    stGlobal.SearchRegister = TxtSearch.Text;
                }
                else
                {
                    DataSession.Filtre = "";
                    stGlobal.SearchRegister = "";
                    StatusMessage($"{response.Message1} - {DateTime.Now}", false);
                    TxtCode.Text = "";
                    TxtName.Text = "";
                    TxtSurname.Text = "";
                    TxtNro.Text = "";
                    ImageUser.Image = BIOCHECK.Properties.Resources.user1;
                }

            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }
        }

        public async Task getDeudaProducto()
        {
            try
            {
                var socio = dataSocioAll.Socio;
                AppsFitService api = new AppsFitService();

                var body = new
                {
                    CodigoUnidadNegocio = DataSession.Unidad,
                    CodigoSede = DataSession.Sede,
                    Dni = socio.DNI
                };

                var resp = await api.SearchDeudaProductoSocio(body);

                if (resp.Success)
                {
                    StlyDeuda("DEBE " + resp.Data.deuda_product + " EN PRODUCTOS", !string.IsNullOrEmpty("" + resp.Data.deuda_product));

                }
                else
                {
                    // NO TIENE deuda
                    StlyDeuda("NO TIENE DEUDA");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Algo salio mal" + ex);
            }
        }


        //Clicks
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
                managementZk.EventGeneral -= OnEventGeneral;
                managementZk.EventPersonal -= OnEventPersonal;
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }

        }
        private void PicCloseHome_Click(object sender, EventArgs e)
        {
            closeWindow();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnConnectionNew_Click(object sender, EventArgs e)
        {
            try
            {
                managementZk.Initialize();
                managementZk.EventGeneral += OnEventGeneral;
                managementZk.EventPersonal += OnEventPersonal;
                cleanMsjAsistencia();
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }
        }

        private void BtnDisconnectionNew_Click(object sender, EventArgs e)
        {
            managementZk.Disconnect();
            managementZk.EventGeneral -= OnEventGeneral;
            managementZk.EventPersonal -= OnEventPersonal;
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            AppsFitService api = new AppsFitService();
            try
            {
                if (string.IsNullOrEmpty(TxtSearch.Text))
                {

                    StatusMessage("Ingrese Número de documento", false);
                    TxtSearch.Focus();
                }
                else
                {
                    //socio
                    if (stGlobal.TypeRegister == 1) //socio
                    {

                        cleanAllTab2();
                        SearchData(1);
                    }

                    //pf
                    if (stGlobal.TypeRegister == 2)
                    {

                        cleanAllTab2();
                        SearchData(2);
                    }
                    //event
                    if (stGlobal.TypeRegister == 3)
                    {

                        cleanAllTab2();
                        SearchData(3);
                    }
                }
            }
            catch (Exception ex)
            {
                managementZk.createFileLog("ScreeHomeLite", ex);
            }
        }

        private async void btnMarkAsistence_Click(object sender, EventArgs e)
        {
            if (managementZk.isInitialized)
            {
                try
                {
                    //StatusMessageD("", false, true);
                    StatusMessageAsistencia("", false, true);
                    if (dataSocioAll.Membresias != null && dataSocioAll.Membresias.Count == 0)
                    {
                        //StatusMessageD($"DEBE CONTAR CON UNA MEMBRESIA", false);
                        StatusMessageAsistencia($"DEBE CONTAR CON UNA MEMBRESIA", false);

                        return;
                    }
                    else
                    {

                        if (dataSocioAll.MembresiasSelected != null)
                        {
                            string message = ValidateMembresia(dataSocioAll.MembresiasSelected);

                            if (!string.IsNullOrEmpty(message))
                            {
                                //StatusMessageD($"{message}", false);
                                StatusMessageAsistencia($"{message}", false);
                                return;
                            }
                            else
                            {
                                if (Convert.ToInt32(dataSocioAll.MembresiasSelected.Debe) == 0)
                                {
                                    //? Success, validate and mark attendance
                                    AppsFitService serv = new AppsFitService();

                                    var data = new
                                    {
                                        CodigoUnidadNegocio = DataSession.Unidad,
                                        Sede = DataSession.Sede,
                                        Socio = dataSocioAll.MembresiasSelected.CodigoSocio,
                                        Membresia = dataSocioAll.MembresiasSelected.CodigoMenbresia,
                                        Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                    };

                                    var res = await serv.MarkAsistence(data);
                                    //StatusMessageD($"{res.Message1}", res.Success);
                                    StatusMessageAsistencia($"{res.Message1}", res.Success);
                                    managementZk.createFile(res.Message1 + "" + res.Success);
                                    if (res.Success)
                                    {
                                        // Update List
                                        await managementZk.ReloadDataAsistence();
                                        soundAccessValidate();
                                        cleanAll();
                                    }
                                }
                                else
                                {
                                    //Debe
                                    //StatusMessageD("", false, true);
                                    cleanAll();
                                    soundOtrosValidate();
                                    StatusMessageAsistencia($"TIENES UNA DEUDA DE {dataSocioAll.MembresiasSelected.Debe}", false, false);
                                    //MessageBox.Show($"TIENES UNA DEUDA DE {dataSocioAll.MembresiasSelected.Debe} !");

                                    return;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    managementZk.createFileLog("ScreeHomeLite", ex);
                }
            }
            else
            {
                //StatusMessageD($"DEBE CONECTAR EL DISPOSITIVO", false);
                StatusMessageAsistencia($"DEBE CONECTAR EL DISPOSITIVO", false);
                return;
            }
        }

        private void ckAuto_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            stGlobal.CheackAutomatic = checkBox.Checked;
        }

        private void btnSearchUpdate_Click(object sender, EventArgs e)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment deployment = ApplicationDeployment.CurrentDeployment;

                try
                {
                    UpdateCheckInfo updateCheckInfo = deployment.CheckForDetailedUpdate();
                    if (updateCheckInfo.UpdateAvailable)
                    {
                        deployment.Update();
                        MessageBox.Show("Se ha instalado una nueva versión de la aplicación. Reinicia la aplicación para aplicar los cambios.");
                    }
                    else
                    {
                        MessageBox.Show("No hay actualizaciones disponibles.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al verificar actualizaciones: " + ex.Message);
                }
            }
        }

        private void rbTSocio_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTSocio.Checked)
            {
                stGlobal.TypeRegister = 1;
            }
        }

        private void rbTPF_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTPF.Checked)
            {
                stGlobal.TypeRegister = 2;
            }
        }

        private void rbTPE_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTPE.Checked)
            {
                stGlobal.TypeRegister = 3;
            }
        }

        private void tbPrecicion_Scroll(object sender, EventArgs e)
        {
            lblPrecicion.Text = $"{tbPrecicion.Value}%";
        }

        private void tbPrecicion_ValueChanged(object sender, EventArgs e)
        {
            int pre = tbPrecicion.Value;

            stGlobal.Precision = pre;
            DataManagerD dm = new DataManagerD();
            dm.SaveData(pre.ToString());
        }

        private void rbTMFijo_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTMFijo.Checked)
            {
                stGlobal.TypeMatch = 2;
                panelPFijoContent.Visible = true;
                panelPProfeContent.Visible = false;
                pbAPHuella.Image = BIOCHECK.Properties.Resources.huell;
                HorarioSigleton hsigleton = HorarioSigleton.Instance;
                hsigleton.SelectedPersonal = false;
                TOneControl(new HorarioFijo(), true);
                TTwoControl(new HorarioFijo(), true);
                StatusMessageD("", false, true);
                InfoPersonal(new SocioModel(), true);
                StatusMessage("", false, true);

            }
        }

        private void rbTMEvent_CheckedChanged(object sender, EventArgs e)
        {
            if (rbTMEvent.Checked)
            {
                stGlobal.TypeMatch = 3;
                panelPFijoContent.Visible = false;
                panelPProfeContent.Visible = true;
                pbAPHuella.Image = BIOCHECK.Properties.Resources.huell;
                lboxProfesionales.Items.Clear();
                HorarioSigleton hsigleton = HorarioSigleton.Instance;
                hsigleton.SelectedPersonal = false;
                StatusMessageD("", false, true);
                InfoPersonal(new SocioModel(), true);
                StatusMessage("", false, true);
            }
        }

        private void lboxProfesionales_SelectedIndexChanged(object sender, EventArgs e)
        {
            HorarioSigleton inst = HorarioSigleton.Instance;
            if (lboxProfesionales.SelectedIndex != -1 && lboxProfesionales.SelectedIndex > 1)
            {
                int selectedIndex = lboxProfesionales.SelectedIndex;
                SelectedIndexPersonal = selectedIndex - 2;
                inst.SelectedPersonal = true;
            }
            else
            {
                inst.SelectedPersonal = false;
                SelectedIndexPersonal = 0;
            }
        }

        private async void btnMIngresoProf_Click(object sender, EventArgs e)
        {
            HorarioSigleton hsgt = HorarioSigleton.Instance;

            if (hsgt.SelectedPersonal)
            {
                await MarkPersonalProfesional(1);
            }
            else
            {
                StatusMessageD($"xxxx", false, true);
                MessageBox.Show("DEBE SELECIONAR UNA CLASE");
            }
        }

        private async void btnMSalidaProf_Click(object sender, EventArgs e)
        {
            HorarioSigleton hsgt = HorarioSigleton.Instance;

            if (hsgt.SelectedPersonal)
            {
                await MarkPersonalProfesional(2);
            }
            else
            {
                StatusMessageD($"", false, true);
                MessageBox.Show("DEBE SELECIONAR UNA CLASE");
            }
        }

        private void btnMarcarEntradaT1_Click(object sender, EventArgs e)
        {
            _ = MarckPersonalFijoAsync(1);
        }

        private void btnMarcarIBT1_Click(object sender, EventArgs e)
        {
            _ = MarckPersonalFijoAsync(2);
        }

        private void btnMarcarFBT1_Click(object sender, EventArgs e)
        {
            _ = MarckPersonalFijoAsync(3);
        }

        private void btnMarcarSalidaT1_Click(object sender, EventArgs e)
        {
            _ = MarckPersonalFijoAsync(4);
        }

        private void btnMarcarEntradaT2_Click(object sender, EventArgs e)
        {
            _ = MarckPersonalFijoAsync(5);
        }

        private void btnMarcarIBT2_Click(object sender, EventArgs e)
        {
            _ = MarckPersonalFijoAsync(6);
        }

        private void btnMarcarFBT2_Click(object sender, EventArgs e)
        {
            _ = MarckPersonalFijoAsync(7);
        }

        private void btnMarcarSalidaT2_Click(object sender, EventArgs e)
        {
            _ = MarckPersonalFijoAsync(8);
        }

        private void nudTiempoInfo_ValueChanged(object sender, EventArgs e)
        {
            int time = (int)nudTiempoInfo.Value;
            stGlobal.TiempoInfo = time;
            DataManagerTime dmt = new DataManagerTime();
            dmt.SaveData(time.ToString());
        }

        private void chkSonido_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            stGlobal.SonidoAsistencia = checkBox.Checked;
            DataManagerSonido dms = new DataManagerSonido();
            dms.SaveData(checkBox.Checked.ToString());
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            soundAccessValidate();
        }

        private void timerNow_Tick(object sender, EventArgs e)
        {
            DateTime horaActual = DateTime.Now;
            string horaFormateada = horaActual.ToString("HH:mm:ss");
            lblHour.Text = horaFormateada;
        }

        private void ScreenHomeLite_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void pbCerrarSesion_Click(object sender, EventArgs e)
        {
            DataManagerTypeAcceso datAcc = new DataManagerTypeAcceso();
            string type = datAcc.ReadData();
            if (type == "1")
            {
                Login login = new Login();
                LoadingForm loadingForm= new LoadingForm();
                login.FormClosed += ScreenHomeLite_FormClosed;
                loadingForm.FormClosed += ScreenHomeLite_FormClosed;
                closeWindow();
                DataManager dm = new DataManager();
                dm.SaveData("");
            }
            else
            {
                Login login = new Login();
                LoadingForm loadingForm = new LoadingForm();
                login.FormClosed += ScreenHomeLite_FormClosed;
                loadingForm.FormClosed += ScreenHomeLite_FormClosed;
                closeWindow();
                DataManager dm = new DataManager();
                dm.SaveData("");
            }

        }

        private void btnAccessStandar_Click(object sender, EventArgs e)
        {
            managementZk.Disconnect();
            managementZk.EventGeneral -= OnEventGeneral;
            managementZk.EventPersonal -= OnEventPersonal;
            this.Hide();
            Login login = new Login();
            login.FormClosed += ScreenHomeLite_FormClosed;
            login.ShowDialog();
        }

        private void btnVerDeudaProducto_Click(object sender, EventArgs e)
        {
            _ = getDeudaProducto();
        }

        private void lblDeudaProductos_Click(object sender, EventArgs e)
        {
            if (dataSocioAll.Membresias != null && dataSocioAll.Membresias.Count == 0)
            {
                MessageBox.Show("Antes de continuar, por favor realice la búsqueda mediante huella dactilar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else
            {
                btnVerDeudaProducto.Visible = true;
            }
        }
    }
}
