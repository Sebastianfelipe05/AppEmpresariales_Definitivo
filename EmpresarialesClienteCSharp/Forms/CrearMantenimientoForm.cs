using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Models;
using EmpresarialesClienteCSharp.Services;
using EmpresarialesClienteCSharp.Utils;

namespace EmpresarialesClienteCSharp.Forms
{
    public partial class CrearMantenimientoForm : Form
    {
        private readonly MantenimientoService _mantenimientoService;
        private TextBox txtPlacaCarro = null!;
        private DateTimePicker dtpFechaMantenimiento = null!;
        private NumericUpDown nudKilometraje = null!;
        private ComboBox cboTipoMantenimiento = null!;
        private NumericUpDown nudCosto = null!;
        private TextBox txtDescripcion = null!;
        private DateTimePicker dtpProximoMantenimiento = null!;
        private CheckBox chkProximoMantenimiento = null!;
        private CheckBox chkCompletado = null!;

        public CrearMantenimientoForm()
        {
            _mantenimientoService = new MantenimientoService();
            InitializeComponent();
            ModernUI.MakeResponsive(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Registrar Nuevo Mantenimiento";
            this.Size = new Size(900, 950);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ModernUI.Colors.Background;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.MinimumSize = new Size(800, 850);

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

            // Form Card
            var formCard = CreateFormCard();
            formCard.Location = new Point(30, 120);
            mainPanel.Controls.Add(formCard);

            this.Controls.Add(mainPanel);
        }

        private Panel CreateHeader()
        {
            var header = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(840, 100),
                BackColor = Color.Transparent
            };

            // Botón volver
            var btnVolver = CreateBackButton();
            btnVolver.Location = new Point(0, 10);
            header.Controls.Add(btnVolver);

            // Título
            var lblTitulo = new Label
            {
                Text = "🔧 Registrar Mantenimiento",
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
                Text = "Complete el formulario para registrar un nuevo mantenimiento vehicular",
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

        private Panel CreateFormCard()
        {
            var card = new Panel
            {
                Size = new Size(840, 730),
                BackColor = Color.White
            };

            card.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Sombra
                using (var shadowPath = GetRoundedRectangle(new Rectangle(2, 2, 836, 726), 12))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }

                // Fondo
                using (var path = GetRoundedRectangle(new Rectangle(0, 0, 839, 729), 12))
                using (var brush = new SolidBrush(Color.White))
                using (var pen = new Pen(ModernUI.Colors.Border, 1))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            int y = 30;
            int leftCol = 30;
            int rightCol = 440;
            int fieldWidth = 360;

            // COLUMNA IZQUIERDA
            // Placa del Carro
            var lblPlaca = CreateLabel("Placa del Carro *", leftCol, y);
            card.Controls.Add(lblPlaca);
            txtPlacaCarro = CreateModernTextBox(card, leftCol, y + 25, fieldWidth);
            txtPlacaCarro.CharacterCasing = CharacterCasing.Upper;
            txtPlacaCarro.MaxLength = 10;
            y += 75;

            // Fecha de Mantenimiento
            var lblFecha = CreateLabel("Fecha de Mantenimiento *", leftCol, y);
            card.Controls.Add(lblFecha);
            dtpFechaMantenimiento = CreateModernDateTimePicker(leftCol, y + 25, fieldWidth);
            dtpFechaMantenimiento.Format = DateTimePickerFormat.Custom;
            dtpFechaMantenimiento.CustomFormat = "dd/MM/yyyy HH:mm";
            card.Controls.Add(dtpFechaMantenimiento);
            y += 75;

            // Kilometraje
            var lblKilometraje = CreateLabel("Kilometraje (km) *", leftCol, y);
            card.Controls.Add(lblKilometraje);
            nudKilometraje = CreateModernNumericUpDown(leftCol, y + 25, fieldWidth);
            nudKilometraje.Maximum = 1000000;
            nudKilometraje.ThousandsSeparator = true;
            card.Controls.Add(nudKilometraje);
            y += 75;

            // Tipo de Mantenimiento
            var lblTipo = CreateLabel("Tipo de Mantenimiento *", leftCol, y);
            card.Controls.Add(lblTipo);
            cboTipoMantenimiento = CreateModernComboBox(leftCol, y + 25, fieldWidth);
            cboTipoMantenimiento.Items.AddRange(new object[]
            {
                "PREVENTIVO",
                "CORRECTIVO",
                "REVISION",
                "CAMBIO_ACEITE",
                "CAMBIO_LLANTAS",
                "OTROS"
            });
            cboTipoMantenimiento.SelectedIndex = 0;
            card.Controls.Add(cboTipoMantenimiento);

            // COLUMNA DERECHA
            y = 30;

            // Costo
            var lblCosto = CreateLabel("Costo ($) *", rightCol, y);
            card.Controls.Add(lblCosto);
            nudCosto = CreateModernNumericUpDown(rightCol, y + 25, fieldWidth);
            nudCosto.Maximum = 100000000;
            nudCosto.ThousandsSeparator = true;
            card.Controls.Add(nudCosto);
            y += 75;

            // Descripción (spanning both columns)
            y = 330;
            var lblDescripcion = CreateLabel("Descripción del Mantenimiento *", leftCol, y);
            card.Controls.Add(lblDescripcion);

            var txtDescContainer = new Panel
            {
                Location = new Point(leftCol, y + 25),
                Size = new Size(770, 120),
                BackColor = Color.White
            };

            txtDescContainer.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectangle(txtDescContainer.ClientRectangle, 8))
                using (var pen = new Pen(ModernUI.Colors.Border, 2))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };

            txtDescripcion = new TextBox
            {
                Location = new Point(10, 10),
                Width = 750,
                Height = 100,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = ModernUI.Colors.Gray900,
                Multiline = true,
                MaxLength = 500,
                ScrollBars = ScrollBars.Vertical
            };

            txtDescContainer.Controls.Add(txtDescripcion);
            card.Controls.Add(txtDescContainer);

            // Checkboxes
            y = 475;
            chkCompletado = new CheckBox
            {
                Text = "  ✅ Mantenimiento Completado",
                Location = new Point(leftCol, y),
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Gray700,
                AutoSize = true,
                Checked = false,
                BackColor = Color.Transparent
            };
            card.Controls.Add(chkCompletado);

            y += 30;
            chkProximoMantenimiento = new CheckBox
            {
                Text = "  📅 Programar Próximo Mantenimiento",
                Location = new Point(leftCol, y),
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Gray700,
                AutoSize = true,
                Checked = false,
                BackColor = Color.Transparent
            };
            chkProximoMantenimiento.CheckedChanged += (s, e) => {
                dtpProximoMantenimiento.Enabled = chkProximoMantenimiento.Checked;
            };
            card.Controls.Add(chkProximoMantenimiento);

            // Fecha Próximo Mantenimiento
            y += 30;
            var lblProximoMant = CreateLabel("Fecha del Próximo Mantenimiento", leftCol, y);
            card.Controls.Add(lblProximoMant);
            dtpProximoMantenimiento = CreateModernDateTimePicker(leftCol, y + 25, 360);
            dtpProximoMantenimiento.Format = DateTimePickerFormat.Custom;
            dtpProximoMantenimiento.CustomFormat = "dd/MM/yyyy";
            dtpProximoMantenimiento.Enabled = false;
            card.Controls.Add(dtpProximoMantenimiento);

            // Nota de campos obligatorios
            y = 615;
            var lblNota = new Label
            {
                Text = "* Campos obligatorios",
                Location = new Point(leftCol, y),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = ModernUI.Colors.Danger,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblNota);

            // Botones
            y = 655;
            var btnGuardar = ModernUI.CreateSuccessButton("💾 Guardar Mantenimiento", BtnGuardar_Click);
            btnGuardar.Location = new Point(leftCol, y);
            btnGuardar.Size = new Size(360, 50);
            btnGuardar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            card.Controls.Add(btnGuardar);

            var btnCancelar = ModernUI.CreateDangerButton("✖️ Cancelar", (s, e) => this.Close());
            btnCancelar.Location = new Point(rightCol, y);
            btnCancelar.Size = new Size(360, 50);
            btnCancelar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            card.Controls.Add(btnCancelar);

            return card;
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Font = ModernUI.Fonts.BodyBold,
                ForeColor = ModernUI.Colors.Gray700,
                AutoSize = true,
                BackColor = Color.Transparent
            };
        }

        private TextBox CreateModernTextBox(Panel parent, int x, int y, int width)
        {
            var container = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, 40),
                BackColor = Color.White
            };

            container.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectangle(container.ClientRectangle, 8))
                using (var pen = new Pen(ModernUI.Colors.Border, 2))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };

            var txt = new TextBox
            {
                Location = new Point(10, 8),
                Width = width - 20,
                Height = 24,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = ModernUI.Colors.Gray900
            };

            container.Controls.Add(txt);
            parent.Controls.Add(container);

            return txt;
        }

        private NumericUpDown CreateModernNumericUpDown(int x, int y, int width)
        {
            var nud = new NumericUpDown
            {
                Location = new Point(x, y),
                Size = new Size(width, 40),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Minimum = 0,
                Maximum = 999999,
                BackColor = Color.White,
                ForeColor = ModernUI.Colors.Gray900
            };

            return nud;
        }

        private ComboBox CreateModernComboBox(int x, int y, int width)
        {
            var cmb = new ComboBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 40),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = ModernUI.Colors.Gray900,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            return cmb;
        }

        private DateTimePicker CreateModernDateTimePicker(int x, int y, int width)
        {
            var dtp = new DateTimePicker
            {
                Location = new Point(x, y),
                Size = new Size(width, 40),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Format = DateTimePickerFormat.Long,
                CalendarForeColor = ModernUI.Colors.Gray900,
                CalendarMonthBackground = Color.White
            };

            return dtp;
        }

        private async void BtnGuardar_Click(object? sender, EventArgs e)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(txtPlacaCarro.Text))
                {
                    MessageBox.Show("La placa del carro es obligatoria.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPlacaCarro.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDescripcion.Text) || txtDescripcion.Text.Length < 10)
                {
                    MessageBox.Show("La descripción debe tener al menos 10 caracteres.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescripcion.Focus();
                    return;
                }

                // Crear objeto Mantenimiento
                var mantenimiento = new Mantenimiento
                {
                    PlacaCarro = txtPlacaCarro.Text.Trim().ToUpper(),
                    FechaMantenimiento = dtpFechaMantenimiento.Value,
                    Kilometraje = (int)nudKilometraje.Value,
                    TipoMantenimiento = cboTipoMantenimiento.SelectedItem?.ToString() ?? "PREVENTIVO",
                    Costo = (double)nudCosto.Value,
                    Descripcion = txtDescripcion.Text.Trim(),
                    ProximoMantenimiento = chkProximoMantenimiento.Checked ? dtpProximoMantenimiento.Value : null,
                    Completado = chkCompletado.Checked
                };

                this.Cursor = Cursors.WaitCursor;

                // Guardar en el servicio
                await _mantenimientoService.CrearMantenimientoAsync(mantenimiento);

                this.Cursor = Cursors.Default;

                MessageBox.Show("✅ Mantenimiento registrado exitosamente!", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al registrar el mantenimiento:\n\n{ex.Message}", "Error",
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
