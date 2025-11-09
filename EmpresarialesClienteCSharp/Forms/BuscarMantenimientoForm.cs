using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Models;
using EmpresarialesClienteCSharp.Services;
using EmpresarialesClienteCSharp.Utils;

namespace EmpresarialesClienteCSharp.Forms
{
    public partial class BuscarMantenimientoForm : Form
    {
        private readonly MantenimientoService _mantenimientoService;
        private TextBox txtPlaca = null!;
        private DataGridView dgvResultados = null!;
        private Panel searchPanel;
        private Label lblResultadoInfo;
        private List<Mantenimiento> mantenimientosEncontrados = new List<Mantenimiento>();

        public BuscarMantenimientoForm()
        {
            _mantenimientoService = new MantenimientoService();
            InitializeComponent();
            ModernUI.MakeResponsive(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Buscar Mantenimientos por Placa";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ModernUI.Colors.Background;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.MinimumSize = new Size(1000, 700);

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
                Text = "Ingrese la placa del vehículo (ej: ABC-123)",
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
            dgvResultados.Size = new Size(1110, 420);
            dgvResultados.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvResultados.AllowUserToAddRows = false;
            dgvResultados.AllowUserToDeleteRows = false;
            dgvResultados.ReadOnly = true;
            dgvResultados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResultados.MultiSelect = false;

            // Agregar botones de acción en el grid
            dgvResultados.CellDoubleClick += DgvResultados_CellDoubleClick;

            mainPanel.Controls.Add(dgvResultados);

            this.Controls.Add(mainPanel);
        }

        private void DgvResultados_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && mantenimientosEncontrados.Count > e.RowIndex)
            {
                var mantenimiento = mantenimientosEncontrados[e.RowIndex];
                MostrarDetallesMantenimiento(mantenimiento);
            }
        }

        private void MostrarDetallesMantenimiento(dynamic mantenimiento)
        {
            var estadoTexto = mantenimiento.Completado ? "COMPLETADO" : "PENDIENTE";
            var proximoMant = mantenimiento.ProximoMantenimiento != null ?
                              $"\n├─ Próximo Mant.: {mantenimiento.ProximoMantenimiento:dd/MM/yyyy}" : "";

            var detalles = $"✅ Detalle del Mantenimiento\n\n" +
                         $"🔖 IDENTIFICACIÓN\n" +
                         $"├─ ID: {mantenimiento.Id}\n" +
                         $"└─ Placa del Carro: {mantenimiento.PlacaCarro}\n\n" +
                         $"🔧 SERVICIO\n" +
                         $"├─ Tipo: {mantenimiento.TipoMantenimiento}\n" +
                         $"├─ Fecha: {mantenimiento.FechaMantenimiento:dd/MM/yyyy}\n" +
                         $"├─ Kilometraje: {mantenimiento.Kilometraje:N0} km\n" +
                         $"└─ Estado: {estadoTexto}" + proximoMant + "\n\n" +
                         $"💰 COSTO\n" +
                         $"└─ {mantenimiento.Costo:C0}\n\n" +
                         $"📝 DESCRIPCIÓN\n" +
                         $"└─ {mantenimiento.Descripcion}";

            var result = MessageBox.Show(detalles + "\n\n¿Desea actualizar este mantenimiento?",
                "Detalle del Mantenimiento",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                var actualizarForm = new ActualizarMantenimientoForm(mantenimiento.Id.ToString());
                actualizarForm.ShowDialog();
                // Refrescar búsqueda después de actualizar
                if (!string.IsNullOrWhiteSpace(txtPlaca.Text))
                {
                    BtnBuscar_Click(null, null);
                }
            }
        }

        private Panel CreateHeader()
        {
            var header = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1140, 100),
                BackColor = Color.Transparent
            };

            // Botón volver
            var btnVolver = CreateBackButton();
            btnVolver.Location = new Point(0, 10);
            header.Controls.Add(btnVolver);

            // Título
            var lblTitulo = new Label
            {
                Text = "🔍 Buscar Mantenimientos",
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
                Text = "Busque todos los mantenimientos de un vehículo por su placa",
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
                Size = new Size(1110, 120),
                BackColor = Color.White
            };

            panel.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Sombra
                using (var shadowPath = GetRoundedRectangle(new Rectangle(2, 2, 1106, 116), 12))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }

                // Fondo
                using (var path = GetRoundedRectangle(new Rectangle(0, 0, 1109, 119), 12))
                using (var brush = new SolidBrush(Color.White))
                using (var pen = new Pen(ModernUI.Colors.Border, 1))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            // Label
            var lblPlaca = new Label
            {
                Text = "Placa del Vehículo",
                Font = ModernUI.Fonts.BodyBold,
                ForeColor = ModernUI.Colors.Gray700,
                AutoSize = true,
                Location = new Point(25, 25),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(lblPlaca);

            // TextBox Container
            var txtContainer = new Panel
            {
                Location = new Point(25, 50),
                Size = new Size(350, 45),
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

            txtPlaca = new TextBox
            {
                Location = new Point(15, 10),
                Width = 320,
                Height = 25,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = ModernUI.Colors.Gray900,
                CharacterCasing = CharacterCasing.Upper
            };

            txtPlaca.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    BtnBuscar_Click(null, null);
                }
            };

            txtContainer.Controls.Add(txtPlaca);
            panel.Controls.Add(txtContainer);

            // Botón Buscar
            var btnBuscar = ModernUI.CreateButton("🔍 Buscar Mantenimientos", ModernUI.Colors.Info, Color.White, BtnBuscar_Click);
            btnBuscar.Location = new Point(400, 50);
            btnBuscar.Size = new Size(230, 45);
            btnBuscar.Font = ModernUI.Fonts.Button;
            panel.Controls.Add(btnBuscar);

            // Botón Limpiar
            var btnLimpiar = ModernUI.CreateSecondaryButton("🔄 Limpiar", (s, e) => {
                txtPlaca.Clear();
                dgvResultados.DataSource = null;
                mantenimientosEncontrados.Clear();
                lblResultadoInfo.Text = "Ingrese la placa del vehículo (ej: ABC-123)";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Gray600;
                txtPlaca.Focus();
            });
            btnLimpiar.Location = new Point(650, 50);
            btnLimpiar.Size = new Size(150, 45);
            btnLimpiar.Font = ModernUI.Fonts.Button;
            panel.Controls.Add(btnLimpiar);

            // Info helper
            var lblHelper = new Label
            {
                Text = "💡 Doble clic en un mantenimiento para ver detalles",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = ModernUI.Colors.Gray500,
                AutoSize = true,
                Location = new Point(820, 60),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(lblHelper);

            return panel;
        }

        private async void BtnBuscar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPlaca.Text))
                {
                    MessageBox.Show("Por favor, ingrese una placa de vehículo.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPlaca.Focus();
                    return;
                }

                // Mostrar indicador de carga
                this.Cursor = Cursors.WaitCursor;
                lblResultadoInfo.Text = "🔍 Buscando mantenimientos...";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Info;
                dgvResultados.DataSource = null;
                mantenimientosEncontrados.Clear();

                var mantenimientos = await _mantenimientoService.BuscarPorCarroAsync(txtPlaca.Text.Trim());

                this.Cursor = Cursors.Default;

                if (mantenimientos != null && mantenimientos.Any())
                {
                    mantenimientosEncontrados = mantenimientos.ToList();
                    dgvResultados.DataSource = mantenimientos.ToList();

                    // Personalizar columnas
                    if (dgvResultados.Columns.Count > 0)
                    {
                        // Ocultar ID (muy largo)
                        if (dgvResultados.Columns.Contains("Id"))
                        {
                            dgvResultados.Columns["Id"].Visible = false;
                        }

                        dgvResultados.Columns["PlacaCarro"].HeaderText = "Placa";
                        dgvResultados.Columns["PlacaCarro"].Width = 100;

                        dgvResultados.Columns["TipoMantenimiento"].HeaderText = "Tipo";
                        dgvResultados.Columns["TipoMantenimiento"].Width = 150;

                        dgvResultados.Columns["FechaMantenimiento"].HeaderText = "Fecha";
                        dgvResultados.Columns["FechaMantenimiento"].DefaultCellStyle.Format = "dd/MM/yyyy";
                        dgvResultados.Columns["FechaMantenimiento"].Width = 120;

                        dgvResultados.Columns["Kilometraje"].HeaderText = "Km";
                        dgvResultados.Columns["Kilometraje"].DefaultCellStyle.Format = "N0";
                        dgvResultados.Columns["Kilometraje"].Width = 100;
                        dgvResultados.Columns["Kilometraje"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        dgvResultados.Columns["Costo"].HeaderText = "Costo";
                        dgvResultados.Columns["Costo"].DefaultCellStyle.Format = "C0";
                        dgvResultados.Columns["Costo"].Width = 120;
                        dgvResultados.Columns["Costo"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        dgvResultados.Columns["Completado"].HeaderText = "Estado";
                        dgvResultados.Columns["Completado"].Width = 100;

                        dgvResultados.Columns["Descripcion"].HeaderText = "Descripción";
                        dgvResultados.Columns["Descripcion"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgvResultados.Columns["Descripcion"].MinimumWidth = 200;

                        // Ocultar campos menos relevantes
                        if (dgvResultados.Columns.Contains("FechaRegistro"))
                            dgvResultados.Columns["FechaRegistro"].Visible = false;
                        if (dgvResultados.Columns.Contains("ProximoMantenimiento"))
                            dgvResultados.Columns["ProximoMantenimiento"].Visible = false;
                        if (dgvResultados.Columns.Contains("EstadoMantenimiento"))
                            dgvResultados.Columns["EstadoMantenimiento"].Visible = false;
                        if (dgvResultados.Columns.Contains("EsUrgente"))
                            dgvResultados.Columns["EsUrgente"].Visible = false;
                    }

                    var completados = mantenimientos.Count(m => m.Completado);
                    var costoTotal = mantenimientos.Sum(m => m.Costo);

                    lblResultadoInfo.Text = $"✅ {mantenimientos.Count()} mantenimiento(s) encontrado(s) para {txtPlaca.Text} | " +
                                          $"Completados: {completados} | Costo Total: {costoTotal:C0}";
                    lblResultadoInfo.ForeColor = ModernUI.Colors.Success;
                }
                else
                {
                    lblResultadoInfo.Text = $"❌ No se encontraron mantenimientos para el vehículo con placa: {txtPlaca.Text}";
                    lblResultadoInfo.ForeColor = ModernUI.Colors.Danger;
                    dgvResultados.DataSource = null;

                    MessageBox.Show($"No se encontraron mantenimientos para el vehículo:\n\nPlaca: {txtPlaca.Text}\n\n" +
                                  "Verifique que la placa esté correcta o que el vehículo tenga mantenimientos registrados.",
                        "No Encontrado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                lblResultadoInfo.Text = $"❌ Error al buscar: {ex.Message}";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Danger;

                MessageBox.Show($"Error al buscar mantenimientos:\n\n{ex.Message}",
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
