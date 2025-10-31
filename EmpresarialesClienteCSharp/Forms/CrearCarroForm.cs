using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Models;
using EmpresarialesClienteCSharp.Services;
using EmpresarialesClienteCSharp.Utils;

namespace EmpresarialesClienteCSharp.Forms
{
    public partial class CrearCarroForm : Form
    {
        private readonly CarroService _carroService;
        private TextBox txtMarca = null!, txtColor = null!, txtPlaca = null!, txtModelo = null!, txtAnio = null!, txtPuertas = null!, txtPrecio = null!;
        private ComboBox cmbCombustible = null!, cmbEstado = null!, cmbTransmision = null!;
        private CheckBox chkAireAcondicionado = null!;

        public CrearCarroForm()
        {
            _carroService = new CarroService();
            InitializeComponent();
            ModernUI.MakeResponsive(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Registrar Nuevo Vehículo";
            this.Size = new Size(900, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ModernUI.Colors.Background;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.MinimumSize = new Size(800, 800);

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
                Text = "➕ Registrar Vehículo",
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
                Text = "Complete el formulario para agregar un nuevo vehículo al inventario",
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
                ForeColor = ModernUI.Colors.Success,
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
                Size = new Size(840, 680),
                BackColor = Color.White
            };

            card.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Sombra
                using (var shadowPath = GetRoundedRectangle(new Rectangle(2, 2, 836, 676), 12))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }

                // Fondo
                using (var path = GetRoundedRectangle(new Rectangle(0, 0, 839, 679), 12))
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
            // Placa
            var lblPlaca = CreateLabel("Placa (ABC-123)", leftCol, y);
            card.Controls.Add(lblPlaca);
            txtPlaca = CreateModernTextBox(card, leftCol, y + 25, fieldWidth);
            txtPlaca.CharacterCasing = CharacterCasing.Upper;
            txtPlaca.MaxLength = 10;
            y += 75;

            // Marca
            var lblMarca = CreateLabel("Marca", leftCol, y);
            card.Controls.Add(lblMarca);
            txtMarca = CreateModernTextBox(card, leftCol, y + 25, fieldWidth);
            y += 75;

            // Modelo
            var lblModelo = CreateLabel("Modelo", leftCol, y);
            card.Controls.Add(lblModelo);
            txtModelo = CreateModernTextBox(card, leftCol, y + 25, fieldWidth);
            y += 75;

            // Color
            var lblColor = CreateLabel("Color", leftCol, y);
            card.Controls.Add(lblColor);
            txtColor = CreateModernTextBox(card, leftCol, y + 25, fieldWidth);
            y += 75;

            // Año
            var lblAnio = CreateLabel("Año (1950-2030)", leftCol, y);
            card.Controls.Add(lblAnio);
            txtAnio = CreateModernTextBox(card, leftCol, y + 25, fieldWidth);
            txtAnio.MaxLength = 4;

            // COLUMNA DERECHA
            y = 30;

            // Precio
            var lblPrecio = CreateLabel("Precio", rightCol, y);
            card.Controls.Add(lblPrecio);
            txtPrecio = CreateModernTextBox(card, rightCol, y + 25, fieldWidth);
            y += 75;

            // Número de Puertas
            var lblPuertas = CreateLabel("Número de Puertas (2-5)", rightCol, y);
            card.Controls.Add(lblPuertas);
            txtPuertas = CreateModernTextBox(card, rightCol, y + 25, fieldWidth);
            txtPuertas.MaxLength = 1;
            y += 75;

            // Combustible
            var lblCombustible = CreateLabel("Combustible", rightCol, y);
            card.Controls.Add(lblCombustible);
            cmbCombustible = CreateModernComboBox(rightCol, y + 25, fieldWidth);
            cmbCombustible.Items.AddRange(new[] { "GASOLINA", "DIESEL", "HIBRIDO", "ELECTRICO" });
            cmbCombustible.SelectedIndex = 0;
            card.Controls.Add(cmbCombustible);
            y += 75;

            // Estado
            var lblEstado = CreateLabel("Estado", rightCol, y);
            card.Controls.Add(lblEstado);
            cmbEstado = CreateModernComboBox(rightCol, y + 25, fieldWidth);
            cmbEstado.Items.AddRange(new[] { "NUEVO", "USADO", "EXCELENTE", "BUENO", "REGULAR" });
            cmbEstado.SelectedIndex = 0;
            card.Controls.Add(cmbEstado);
            y += 75;

            // Transmisión
            var lblTransmision = CreateLabel("Transmisión", rightCol, y);
            card.Controls.Add(lblTransmision);
            cmbTransmision = CreateModernComboBox(rightCol, y + 25, fieldWidth);
            cmbTransmision.Items.AddRange(new[] { "MANUAL", "AUTOMATICA" });
            cmbTransmision.SelectedIndex = 0;
            card.Controls.Add(cmbTransmision);

            // Checkbox Aire Acondicionado (spanning both columns)
            y = 405;
            chkAireAcondicionado = new CheckBox
            {
                Text = "  ❄️ Tiene Aire Acondicionado",
                Location = new Point(leftCol, y),
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Gray700,
                AutoSize = true,
                Checked = false,
                BackColor = Color.Transparent
            };
            card.Controls.Add(chkAireAcondicionado);

            // Botones
            y = 580;
            var btnGuardar = ModernUI.CreateSuccessButton("💾 Guardar Vehículo", BtnGuardar_Click);
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

        private async void BtnGuardar_Click(object? sender, EventArgs e)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(txtPlaca.Text) ||
                    string.IsNullOrWhiteSpace(txtMarca.Text) ||
                    string.IsNullOrWhiteSpace(txtModelo.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos obligatorios.", "Error de Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtAnio.Text, out int anio) || anio < 1950 || anio > 2030)
                {
                    MessageBox.Show("El año debe ser un número entre 1950 y 2030.", "Error de Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtPuertas.Text, out int puertas) || puertas < 2 || puertas > 5)
                {
                    MessageBox.Show("El número de puertas debe estar entre 2 y 5.", "Error de Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtPrecio.Text, out double precio) || precio <= 0)
                {
                    MessageBox.Show("El precio debe ser un número mayor que 0.", "Error de Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Crear objeto Carro
                var carro = new Carro
                {
                    Placa = txtPlaca.Text.ToUpper(),
                    Marca = txtMarca.Text.ToUpper(),
                    Modelo = txtModelo.Text.ToUpper(),
                    Color = txtColor.Text.ToUpper(),
                    Anio = anio,
                    NumeroPuertas = puertas,
                    Precio = precio,
                    Combustible = cmbCombustible.SelectedItem?.ToString() ?? "GASOLINA",
                    Estado = cmbEstado.SelectedItem?.ToString() ?? "NUEVO",
                    TipoTransmision = cmbTransmision.SelectedItem?.ToString() ?? "MANUAL",
                    TieneAireAcondicionado = chkAireAcondicionado.Checked
                };

                // Guardar en el servicio
                await _carroService.CrearCarroAsync(carro);

                MessageBox.Show("Carro creado exitosamente!", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear el carro: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
