using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Services;

namespace EmpresarialesClienteCSharp.Forms
{
    public partial class EliminarCarroForm : Form
    {
        private readonly CarroService _carroService;
        private TextBox txtPlaca = null!;
        private Panel panelInfo = null!;
        private Button btnEliminar = null!;
        private Label lblMarca = null!, lblModelo = null!, lblColor = null!, lblAnio = null!, lblPrecio = null!;

        public EliminarCarroForm()
        {
            _carroService = new CarroService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Eliminar Carro";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(249, 250, 251);
            this.MinimumSize = new Size(700, 600);
            this.MaximumSize = new Size(700, 600);

            // Modern title bar with close button
            var titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(220, 38, 38)
            };
            titleBar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            };

            var lblTitulo = new Label
            {
                Text = "🗑️ Eliminar Vehículo",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(25, 18),
                AutoSize = true
            };

            var btnCerrar = new Button
            {
                Text = "✕",
                Size = new Size(35, 35),
                Location = new Point(640, 17),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.FlatAppearance.MouseOverBackColor = Color.FromArgb(185, 28, 28);
            btnCerrar.Click += (s, e) => this.Close();

            titleBar.Controls.AddRange(new Control[] { lblTitulo, btnCerrar });

            // Main content panel
            var mainPanel = new Panel
            {
                Location = new Point(30, 100),
                Size = new Size(640, 460),
                BackColor = Color.White
            };
            mainPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectangle(mainPanel.ClientRectangle, 15))
                {
                    e.Graphics.FillPath(new SolidBrush(Color.White), path);
                    e.Graphics.DrawPath(new Pen(Color.FromArgb(229, 231, 235), 1), path);
                }
            };

            // Search section
            var lblInstruccion = new Label
            {
                Text = "🔍 Paso 1: Buscar Vehículo por Placa",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(30, 30),
                AutoSize = true
            };

            var lblPlaca = new Label
            {
                Text = "Placa del Vehículo:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(30, 70),
                AutoSize = true
            };

            txtPlaca = new TextBox
            {
                Location = new Point(30, 95),
                Size = new Size(350, 35),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtPlaca.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    BtnBuscar_Click(s, e);
                }
            };

            var btnBuscar = CreateModernButton("🔎 Buscar", Color.FromArgb(59, 130, 246), 400, 92, 210, 42);
            btnBuscar.Click += BtnBuscar_Click;

            // Info panel (hidden by default)
            panelInfo = new Panel
            {
                Location = new Point(30, 160),
                Size = new Size(580, 190),
                BackColor = Color.FromArgb(254, 242, 242),
                Visible = false
            };
            panelInfo.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectangle(panelInfo.ClientRectangle, 10))
                {
                    e.Graphics.FillPath(new SolidBrush(Color.FromArgb(254, 242, 242)), path);
                    e.Graphics.DrawPath(new Pen(Color.FromArgb(252, 165, 165), 2), path);
                }
            };

            var lblInfoTitulo = new Label
            {
                Text = "⚠️ Información del Vehículo a Eliminar",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(153, 27, 27),
                Location = new Point(20, 15),
                AutoSize = true
            };

            lblMarca = CreateInfoLabel("Marca:", "", 20, 50);
            lblModelo = CreateInfoLabel("Modelo:", "", 20, 80);
            lblColor = CreateInfoLabel("Color:", "", 20, 110);
            lblAnio = CreateInfoLabel("Año:", "", 320, 50);
            lblPrecio = CreateInfoLabel("Precio:", "", 320, 80);

            var lblAdvertencia = new Label
            {
                Text = "⚠️ ADVERTENCIA: Esta acción es permanente y no se puede deshacer",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38),
                Location = new Point(20, 150),
                Size = new Size(540, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(254, 226, 226)
            };

            panelInfo.Controls.AddRange(new Control[] { lblInfoTitulo, lblMarca, lblModelo, lblColor, lblAnio, lblPrecio, lblAdvertencia });

            // Action buttons
            btnEliminar = CreateModernButton("🗑️ Eliminar Vehículo", Color.FromArgb(220, 38, 38), 30, 380, 280, 50);
            btnEliminar.Enabled = false;
            btnEliminar.Click += BtnEliminar_Click;

            var btnCancelar = CreateModernButton("❌ Cancelar", Color.FromArgb(107, 114, 128), 330, 380, 280, 50);
            btnCancelar.Click += (s, e) => this.Close();

            mainPanel.Controls.AddRange(new Control[] { lblInstruccion, lblPlaca, txtPlaca, btnBuscar, panelInfo, btnEliminar, btnCancelar });
            this.Controls.AddRange(new Control[] { titleBar, mainPanel });
        }

        private Button CreateModernButton(string text, Color bgColor, int x, int y, int width, int height)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = bgColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            Color hoverColor = ControlPaint.Dark(bgColor, 0.1f);
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = bgColor;

            return btn;
        }

        private Label CreateInfoLabel(string caption, string value, int x, int y)
        {
            return new Label
            {
                Text = $"{caption} {value}",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(x, y),
                AutoSize = true
            };
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private async void BtnBuscar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPlaca.Text))
                {
                    MessageBox.Show("⚠️ Por favor, ingrese una placa.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var carro = await _carroService.BuscarPorPlacaAsync(txtPlaca.Text.ToUpper());

                if (carro != null)
                {
                    lblMarca.Text = $"🚗 Marca: {carro.Marca}";
                    lblModelo.Text = $"📦 Modelo: {carro.Modelo}";
                    lblColor.Text = $"🎨 Color: {carro.Color}";
                    lblAnio.Text = $"📅 Año: {carro.Anio}";
                    lblPrecio.Text = $"💰 Precio: ${carro.Precio:N0}";

                    panelInfo.Visible = true;
                    btnEliminar.Enabled = true;
                }
                else
                {
                    MessageBox.Show("❌ No se encontró ningún vehículo con esa placa.", "No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    panelInfo.Visible = false;
                    btnEliminar.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error al buscar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnEliminar_Click(object? sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    $"⚠️ ¿Está COMPLETAMENTE SEGURO que desea eliminar el vehículo con placa {txtPlaca.Text.ToUpper()}?\n\n" +
                    "⛔ Esta acción es PERMANENTE y NO se puede deshacer.\n\n" +
                    "Se perderán todos los datos asociados al vehículo.",
                    "⚠️ Confirmar Eliminación Permanente",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2
                );

                if (result == DialogResult.Yes)
                {
                    await _carroService.EliminarCarroAsync(txtPlaca.Text.ToUpper());
                    MessageBox.Show("✅ Vehículo eliminado exitosamente del inventario.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error al eliminar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
