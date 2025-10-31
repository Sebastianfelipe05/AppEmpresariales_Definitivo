using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Services;
using EmpresarialesClienteCSharp.Utils;

namespace EmpresarialesClienteCSharp.Forms
{
    public partial class BuscarMantenimientoForm : Form
    {
        private readonly MantenimientoService _mantenimientoService;
        private TextBox txtId = null!;
        private DataGridView dgvResultados = null!;
        private Panel searchPanel;
        private Label lblResultadoInfo;

        public BuscarMantenimientoForm()
        {
            _mantenimientoService = new MantenimientoService();
            InitializeComponent();
            ModernUI.MakeResponsive(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Buscar Mantenimiento por ID";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ModernUI.Colors.Background;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.MinimumSize = new Size(900, 600);

            // Panel principal con scroll
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = ModernUI.Colors.Background,
                Padding = new Padding(30)
            };

            // Header
            var headerPanel = CreateHeader();
            mainPanel.Controls.Add(headerPanel);

            // Search Panel
            searchPanel = CreateSearchPanel();
            searchPanel.Location = new Point(30, 120);
            mainPanel.Controls.Add(searchPanel);

            // Info Label
            lblResultadoInfo = new Label
            {
                Text = "Ingrese un ID (UUID) para buscar",
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Gray600,
                AutoSize = true,
                Location = new Point(30, 260),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(lblResultadoInfo);

            // DataGridView Results
            dgvResultados = ModernUI.CreateModernDataGrid();
            dgvResultados.Location = new Point(30, 290);
            dgvResultados.Size = new Size(1010, 370);
            dgvResultados.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            mainPanel.Controls.Add(dgvResultados);

            this.Controls.Add(mainPanel);
        }

        private Panel CreateHeader()
        {
            var header = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1040, 100),
                BackColor = Color.Transparent
            };

            // Botón volver
            var btnVolver = CreateBackButton();
            btnVolver.Location = new Point(0, 10);
            header.Controls.Add(btnVolver);

            // Título
            var lblTitulo = new Label
            {
                Text = "🔍 Buscar Mantenimiento",
                Font = ModernUI.Fonts.Heading1,
                ForeColor = ModernUI.Colors.Gray900,
                AutoSize = true,
                Location = new Point(0, 50),
                BackColor = Color.Transparent
            };
            header.Controls.Add(lblTitulo);

            // Subtítulo
            var lblSubtitulo = new Label
            {
                Text = "Busque un mantenimiento específico por su ID único (UUID)",
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Gray600,
                AutoSize = true,
                Location = new Point(0, 80),
                BackColor = Color.Transparent
            };
            header.Controls.Add(lblSubtitulo);

            return header;
        }

        private Button CreateBackButton()
        {
            var btn = new Button
            {
                Text = "← Volver",
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Info,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 35),
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ModernUI.Colors.Gray100;

            btn.Click += (s, e) => this.Close();

            return btn;
        }

        private Panel CreateSearchPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1010, 120),
                BackColor = Color.White
            };

            panel.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Sombra
                using (var shadowPath = GetRoundedRectangle(new Rectangle(2, 2, 1006, 116), 12))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }

                // Fondo
                using (var path = GetRoundedRectangle(new Rectangle(0, 0, 1009, 119), 12))
                using (var brush = new SolidBrush(Color.White))
                using (var pen = new Pen(ModernUI.Colors.Border, 1))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            // Label
            var lblId = new Label
            {
                Text = "ID del Mantenimiento (UUID)",
                Font = ModernUI.Fonts.BodyBold,
                ForeColor = ModernUI.Colors.Gray700,
                AutoSize = true,
                Location = new Point(25, 25),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(lblId);

            // TextBox Container
            var txtContainer = new Panel
            {
                Location = new Point(25, 50),
                Size = new Size(450, 45),
                BackColor = Color.White
            };

            txtContainer.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectangle(txtContainer.ClientRectangle, 8))
                using (var pen = new Pen(ModernUI.Colors.Border, 2))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };

            txtId = new TextBox
            {
                Location = new Point(15, 10),
                Width = 420,
                Height = 25,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = ModernUI.Colors.Gray900
            };

            txtId.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    BtnBuscar_Click(null, null);
                }
            };

            txtContainer.Controls.Add(txtId);
            panel.Controls.Add(txtContainer);

            // Botón Buscar
            var btnBuscar = ModernUI.CreateButton("🔍 Buscar Mantenimiento", ModernUI.Colors.Info, Color.White, BtnBuscar_Click);
            btnBuscar.Location = new Point(500, 50);
            btnBuscar.Size = new Size(200, 45);
            btnBuscar.Font = ModernUI.Fonts.Button;
            panel.Controls.Add(btnBuscar);

            // Botón Limpiar
            var btnLimpiar = ModernUI.CreateSecondaryButton("🔄 Limpiar", (s, e) => {
                txtId.Clear();
                dgvResultados.DataSource = null;
                lblResultadoInfo.Text = "Ingrese un ID (UUID) para buscar";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Gray600;
                txtId.Focus();
            });
            btnLimpiar.Location = new Point(720, 50);
            btnLimpiar.Size = new Size(150, 45);
            btnLimpiar.Font = ModernUI.Fonts.Button;
            panel.Controls.Add(btnLimpiar);

            return panel;
        }

        private async void BtnBuscar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    MessageBox.Show("Por favor, ingrese un ID de mantenimiento.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtId.Focus();
                    return;
                }

                // Mostrar indicador de carga
                this.Cursor = Cursors.WaitCursor;
                lblResultadoInfo.Text = "🔍 Buscando...";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Info;
                dgvResultados.DataSource = null;

                var mantenimiento = await _mantenimientoService.BuscarPorIdAsync(txtId.Text.Trim());

                this.Cursor = Cursors.Default;

                if (mantenimiento != null)
                {
                    dgvResultados.DataSource = new[] { mantenimiento };

                    // Personalizar columnas
                    if (dgvResultados.Columns.Count > 0)
                    {
                        dgvResultados.Columns["Id"].HeaderText = "ID";
                        dgvResultados.Columns["PlacaCarro"].HeaderText = "Placa del Carro";
                        dgvResultados.Columns["TipoMantenimiento"].HeaderText = "Tipo";
                        dgvResultados.Columns["FechaMantenimiento"].HeaderText = "Fecha";
                        dgvResultados.Columns["FechaMantenimiento"].DefaultCellStyle.Format = "dd/MM/yyyy";
                        dgvResultados.Columns["Kilometraje"].HeaderText = "Kilometraje (km)";
                        dgvResultados.Columns["Kilometraje"].DefaultCellStyle.Format = "N0";
                        dgvResultados.Columns["Costo"].HeaderText = "Costo";
                        dgvResultados.Columns["Costo"].DefaultCellStyle.Format = "C0";
                        dgvResultados.Columns["Completado"].HeaderText = "Estado";
                        dgvResultados.Columns["Descripcion"].HeaderText = "Descripción";

                        if (dgvResultados.Columns.Contains("FechaRegistro"))
                        {
                            dgvResultados.Columns["FechaRegistro"].HeaderText = "Fecha Registro";
                            dgvResultados.Columns["FechaRegistro"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                        }
                    }

                    var estadoTexto = mantenimiento.Completado ? "COMPLETADO" : "PENDIENTE";
                    lblResultadoInfo.Text = $"✅ Mantenimiento encontrado - {mantenimiento.TipoMantenimiento} ({estadoTexto})";
                    lblResultadoInfo.ForeColor = ModernUI.Colors.Success;

                    // Mostrar detalles en MessageBox
                    var detalles = $"✅ Mantenimiento Encontrado\n\n" +
                                 $"🔖 IDENTIFICACIÓN\n" +
                                 $"├─ ID: {mantenimiento.Id}\n" +
                                 $"└─ Placa del Carro: {mantenimiento.PlacaCarro}\n\n" +
                                 $"🔧 SERVICIO\n" +
                                 $"├─ Tipo: {mantenimiento.TipoMantenimiento}\n" +
                                 $"├─ Fecha: {mantenimiento.FechaMantenimiento:dd/MM/yyyy}\n" +
                                 $"├─ Kilometraje: {mantenimiento.Kilometraje:N0} km\n" +
                                 $"└─ Estado: {estadoTexto}\n\n" +
                                 $"💰 COSTO\n" +
                                 $"└─ {mantenimiento.Costo:C0}\n\n" +
                                 $"📝 DESCRIPCIÓN\n" +
                                 $"└─ {mantenimiento.Descripcion}";

                    MessageBox.Show(detalles, "Mantenimiento Encontrado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblResultadoInfo.Text = $"❌ No se encontró ningún mantenimiento con el ID: {txtId.Text}";
                    lblResultadoInfo.ForeColor = ModernUI.Colors.Danger;
                    dgvResultados.DataSource = null;

                    MessageBox.Show($"No se encontró ningún mantenimiento con el ID:\n\n{txtId.Text}",
                        "No Encontrado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                lblResultadoInfo.Text = $"❌ Error al buscar: {ex.Message}";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Danger;

                MessageBox.Show($"Error al buscar mantenimiento:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
