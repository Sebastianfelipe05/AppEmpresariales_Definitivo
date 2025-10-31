using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Models;
using EmpresarialesClienteCSharp.Services;
using EmpresarialesClienteCSharp.Utils;

namespace EmpresarialesClienteCSharp.Forms
{
    public partial class ActualizarCarroForm : Form
    {
        private readonly CarroService _carroService;
        private TextBox txtPlacaBuscar = null!, txtMarca = null!, txtColor = null!, txtModelo = null!, txtAnio = null!, txtPuertas = null!, txtPrecio = null!;
        private ComboBox cmbCombustible = null!, cmbEstado = null!, cmbTransmision = null!;
        private CheckBox chkAireAcondicionado = null!;
        private Panel panelEdicion = null!;
        private Label lblEstado;
        private Carro? carroActual;

        public ActualizarCarroForm()
        {
            _carroService = new CarroService();
            InitializeComponent();
            ModernUI.MakeResponsive(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Actualizar Vehículo";
            this.Size = new Size(950, 1000);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ModernUI.Colors.Background;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.MinimumSize = new Size(850, 900);

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

            // Search Card
            var searchCard = CreateSearchCard();
            searchCard.Location = new Point(30, 120);
            mainPanel.Controls.Add(searchCard);

            // Estado label
            lblEstado = new Label
            {
                Text = "Busque un vehículo para editar su información",
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Gray600,
                AutoSize = true,
                Location = new Point(30, 260),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(lblEstado);

            // Edit Panel (initially hidden)
            panelEdicion = CreateEditPanel();
            panelEdicion.Location = new Point(30, 290);
            panelEdicion.Visible = false;
            mainPanel.Controls.Add(panelEdicion);

            this.Controls.Add(mainPanel);
        }

        private Panel CreateHeader()
        {
            var header = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(890, 100),
                BackColor = Color.Transparent
            };

            // Botón volver
            var btnVolver = CreateBackButton();
            btnVolver.Location = new Point(0, 10);
            header.Controls.Add(btnVolver);

            // Título
            var lblTitulo = new Label
            {
                Text = "✏️ Actualizar Vehículo",
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
                Text = "Busque y actualice la información de un vehículo existente",
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
                ForeColor = ModernUI.Colors.Warning,
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

        private Panel CreateSearchCard()
        {
            var card = new Panel
            {
                Size = new Size(890, 120),
                BackColor = Color.White
            };

            card.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (var shadowPath = GetRoundedRectangle(new Rectangle(2, 2, 886, 116), 12))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }

                using (var path = GetRoundedRectangle(new Rectangle(0, 0, 889, 119), 12))
                using (var brush = new SolidBrush(Color.White))
                using (var pen = new Pen(ModernUI.Colors.Border, 1))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            // Label
            var lblBuscar = new Label
            {
                Text = "1️⃣ Buscar Vehículo por Placa",
                Font = ModernUI.Fonts.BodyBold,
                ForeColor = ModernUI.Colors.Gray700,
                AutoSize = true,
                Location = new Point(25, 25),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblBuscar);

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

            txtPlacaBuscar = new TextBox
            {
                Location = new Point(15, 10),
                Width = 420,
                Height = 25,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = ModernUI.Colors.Gray900,
                CharacterCasing = CharacterCasing.Upper,
                MaxLength = 10
            };

            txtPlacaBuscar.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    BtnBuscar_Click(null, null);
                }
            };

            txtContainer.Controls.Add(txtPlacaBuscar);
            card.Controls.Add(txtContainer);

            // Botón Buscar
            var btnBuscar = ModernUI.CreatePrimaryButton("🔍 Buscar", BtnBuscar_Click);
            btnBuscar.Location = new Point(500, 50);
            btnBuscar.Size = new Size(150, 45);
            btnBuscar.Font = ModernUI.Fonts.Button;
            card.Controls.Add(btnBuscar);

            // Botón Limpiar
            var btnLimpiar = ModernUI.CreateSecondaryButton("🔄 Limpiar", (s, e) => {
                txtPlacaBuscar.Clear();
                panelEdicion.Visible = false;
                lblEstado.Text = "Busque un vehículo para editar su información";
                lblEstado.ForeColor = ModernUI.Colors.Gray600;
                carroActual = null;
                txtPlacaBuscar.Focus();
            });
            btnLimpiar.Location = new Point(670, 50);
            btnLimpiar.Size = new Size(150, 45);
            btnLimpiar.Font = ModernUI.Fonts.Button;
            card.Controls.Add(btnLimpiar);

            return card;
        }

        private Panel CreateEditPanel()
        {
            var card = new Panel
            {
                Size = new Size(890, 630),
                BackColor = Color.White
            };

            card.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (var shadowPath = GetRoundedRectangle(new Rectangle(2, 2, 886, 626), 12))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }

                using (var path = GetRoundedRectangle(new Rectangle(0, 0, 889, 629), 12))
                using (var brush = new SolidBrush(Color.White))
                using (var pen = new Pen(ModernUI.Colors.Border, 1))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            // Title
            var lblEditar = new Label
            {
                Text = "2️⃣ Editar Información del Vehículo",
                Font = ModernUI.Fonts.BodyBold,
                ForeColor = ModernUI.Colors.Gray700,
                AutoSize = true,
                Location = new Point(25, 25),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblEditar);

            int y = 60;
            int leftCol = 30;
            int rightCol = 470;
            int fieldWidth = 380;

            // COLUMNA IZQUIERDA
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
            var lblAnio = CreateLabel("Año", leftCol, y);
            card.Controls.Add(lblAnio);
            txtAnio = CreateModernTextBox(card, leftCol, y + 25, fieldWidth);
            txtAnio.MaxLength = 4;

            // COLUMNA DERECHA
            y = 60;

            // Precio
            var lblPrecio = CreateLabel("Precio", rightCol, y);
            card.Controls.Add(lblPrecio);
            txtPrecio = CreateModernTextBox(card, rightCol, y + 25, fieldWidth);
            y += 75;

            // Número de Puertas
            var lblPuertas = CreateLabel("Número de Puertas", rightCol, y);
            card.Controls.Add(lblPuertas);
            txtPuertas = CreateModernTextBox(card, rightCol, y + 25, fieldWidth);
            txtPuertas.MaxLength = 1;
            y += 75;

            // Combustible
            var lblCombustible = CreateLabel("Combustible", rightCol, y);
            card.Controls.Add(lblCombustible);
            cmbCombustible = CreateModernComboBox(rightCol, y + 25, fieldWidth);
            cmbCombustible.Items.AddRange(new[] { "GASOLINA", "DIESEL", "HIBRIDO", "ELECTRICO" });
            card.Controls.Add(cmbCombustible);
            y += 75;

            // Estado
            var lblEstado = CreateLabel("Estado", rightCol, y);
            card.Controls.Add(lblEstado);
            cmbEstado = CreateModernComboBox(rightCol, y + 25, fieldWidth);
            cmbEstado.Items.AddRange(new[] { "NUEVO", "USADO", "EXCELENTE", "BUENO", "REGULAR" });
            card.Controls.Add(cmbEstado);

            // Transmisión (below estado)
            y = 360;
            var lblTransmision = CreateLabel("Transmisión", leftCol, y);
            card.Controls.Add(lblTransmision);
            cmbTransmision = CreateModernComboBox(leftCol, y + 25, fieldWidth);
            cmbTransmision.Items.AddRange(new[] { "MANUAL", "AUTOMATICA" });
            card.Controls.Add(cmbTransmision);

            // Checkbox Aire Acondicionado
            y = 460;
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
            y = 530;
            var btnActualizar = ModernUI.CreateButton("💾 Guardar Cambios", ModernUI.Colors.Warning, Color.White, BtnActualizar_Click);
            btnActualizar.Location = new Point(leftCol, y);
            btnActualizar.Size = new Size(380, 50);
            btnActualizar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            card.Controls.Add(btnActualizar);

            var btnCancelar = ModernUI.CreateDangerButton("✖️ Cancelar", (s, e) => this.Close());
            btnCancelar.Location = new Point(rightCol, y);
            btnCancelar.Size = new Size(380, 50);
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

        private async void BtnBuscar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPlacaBuscar.Text))
                {
                    MessageBox.Show("Por favor, ingrese una placa para buscar.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPlacaBuscar.Focus();
                    return;
                }

                this.Cursor = Cursors.WaitCursor;
                lblEstado.Text = "🔍 Buscando vehículo...";
                lblEstado.ForeColor = ModernUI.Colors.Primary;

                carroActual = await _carroService.BuscarPorPlacaAsync(txtPlacaBuscar.Text.Trim().ToUpper());

                this.Cursor = Cursors.Default;

                if (carroActual != null)
                {
                    // Rellenar los campos
                    txtMarca.Text = carroActual.Marca;
                    txtModelo.Text = carroActual.Modelo;
                    txtColor.Text = carroActual.Color;
                    txtAnio.Text = carroActual.Anio.ToString();
                    txtPuertas.Text = carroActual.NumeroPuertas.ToString();
                    txtPrecio.Text = carroActual.Precio.ToString();
                    cmbCombustible.SelectedItem = carroActual.Combustible;
                    cmbEstado.SelectedItem = carroActual.Estado;
                    cmbTransmision.SelectedItem = carroActual.TipoTransmision;
                    chkAireAcondicionado.Checked = carroActual.TieneAireAcondicionado;

                    panelEdicion.Visible = true;
                    lblEstado.Text = $"✅ Vehículo encontrado: {carroActual.Marca} {carroActual.Modelo} - {carroActual.Placa}";
                    lblEstado.ForeColor = ModernUI.Colors.Success;
                }
                else
                {
                    panelEdicion.Visible = false;
                    lblEstado.Text = $"❌ No se encontró ningún vehículo con la placa: {txtPlacaBuscar.Text}";
                    lblEstado.ForeColor = ModernUI.Colors.Danger;

                    MessageBox.Show($"No se encontró ningún vehículo con la placa: {txtPlacaBuscar.Text}",
                        "No Encontrado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                lblEstado.Text = $"❌ Error al buscar: {ex.Message}";
                lblEstado.ForeColor = ModernUI.Colors.Danger;

                MessageBox.Show($"Error al buscar vehículo:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnActualizar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (carroActual == null)
                {
                    MessageBox.Show("No hay un vehículo seleccionado para actualizar.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validaciones
                if (!int.TryParse(txtAnio.Text, out int anio) || anio < 1950 || anio > 2030)
                {
                    MessageBox.Show("El año debe ser un número entre 1950 y 2030.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtPuertas.Text, out int puertas) || puertas < 2 || puertas > 5)
                {
                    MessageBox.Show("El número de puertas debe estar entre 2 y 5.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(txtPrecio.Text, out double precio) || precio <= 0)
                {
                    MessageBox.Show("El precio debe ser un número mayor que 0.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Actualizar objeto
                carroActual.Marca = txtMarca.Text.ToUpper();
                carroActual.Modelo = txtModelo.Text.ToUpper();
                carroActual.Color = txtColor.Text.ToUpper();
                carroActual.Anio = anio;
                carroActual.NumeroPuertas = puertas;
                carroActual.Precio = precio;
                carroActual.Combustible = cmbCombustible.SelectedItem?.ToString() ?? "GASOLINA";
                carroActual.Estado = cmbEstado.SelectedItem?.ToString() ?? "NUEVO";
                carroActual.TipoTransmision = cmbTransmision.SelectedItem?.ToString() ?? "MANUAL";
                carroActual.TieneAireAcondicionado = chkAireAcondicionado.Checked;

                this.Cursor = Cursors.WaitCursor;

                await _carroService.ActualizarCarroAsync(carroActual.Placa, carroActual);

                this.Cursor = Cursors.Default;

                MessageBox.Show("✅ Vehículo actualizado exitosamente!", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al actualizar el vehículo:\n\n{ex.Message}", "Error",
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
