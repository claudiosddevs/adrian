using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Dashboard;

/// <summary>
/// Panel principal post-login. Demuestra cómo la UI cambia según el rol y
/// consume endpoints protegidos del backend.
/// </summary>
public class frmDashboard : Form
{
    private readonly IFisApiClient _api;
    private readonly TokenStore _tokens;
    private readonly SesionUsuario _sesion;

    private Label _lblBienvenida = default!;
    private Label _lblRol = default!;
    private Button _btnListarClientes = default!;
    private Button _btnSoloAdmin = default!;
    private Button _btnCerrarSesion = default!;
    private DataGridView _grid = default!;
    private Label _lblResultado = default!;

    public frmDashboard(IFisApiClient api, TokenStore tokens, SesionUsuario sesion)
    {
        _api = api;
        _tokens = tokens;
        _sesion = sesion;

        ConstruirUi();
        Load += (_, _) => AjustarPorRol();
    }

    private void ConstruirUi()
    {
        Text = "Full Internet Services — Panel Administrativo";
        Size = new Size(1100, 650);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.WhiteSmoke;
        MinimumSize = new Size(900, 550);

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = Color.FromArgb(37, 99, 235)
        };
        var lblTitulo = new Label
        {
            Text = "FIS — Panel Administrativo",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(20, 20),
            AutoSize = true
        };
        _lblBienvenida = new Label
        {
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.White,
            Location = new Point(20, 50),
            AutoSize = true
        };
        header.Controls.AddRange(new Control[] { lblTitulo, _lblBienvenida });

        var sidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 220,
            BackColor = Color.FromArgb(31, 41, 55)
        };
        _lblRol = new Label
        {
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(251, 191, 36),
            Location = new Point(20, 20),
            AutoSize = true
        };
        _btnListarClientes = CrearBotonMenu("Clientes", 60);
        _btnListarClientes.Click += async (_, _) => await ListarClientesAsync();
        _btnSoloAdmin = CrearBotonMenu("Solo Administrador", 110);
        _btnSoloAdmin.Click += async (_, _) => await ProbarRbacAsync();
        _btnCerrarSesion = CrearBotonMenu("Cerrar Sesión", 480);
        _btnCerrarSesion.BackColor = Color.FromArgb(220, 38, 38);
        _btnCerrarSesion.Click += (_, _) => CerrarSesion();
        sidebar.Controls.AddRange(new Control[]
        {
            _lblRol, _btnListarClientes, _btnSoloAdmin, _btnCerrarSesion
        });

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            AutoGenerateColumns = true,
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        _lblResultado = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 30,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(20, 0, 0, 0),
            Font = new Font("Segoe UI", 9F)
        };

        Controls.Add(_grid);
        Controls.Add(_lblResultado);
        Controls.Add(sidebar);
        Controls.Add(header);
    }

    private static Button CrearBotonMenu(string texto, int top)
    {
        var btn = new Button
        {
            Text = texto,
            Location = new Point(20, top),
            Size = new Size(180, 38),
            BackColor = Color.FromArgb(55, 65, 81),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(10, 0, 0, 0),
            Font = new Font("Segoe UI", 9F)
        };
        btn.FlatAppearance.BorderSize = 0;
        return btn;
    }

    private void AjustarPorRol()
    {
        var u = _sesion.Actual;
        if (u is null)
        {
            CerrarSesion();
            return;
        }
        _lblBienvenida.Text = $"Bienvenido(a), {u.NombreCompleto}";
        _lblRol.Text = $"ROL: {u.Rol.ToUpperInvariant()}";

        // Comportamiento por rol: el botón "Solo Administrador"
        // solo está habilitado para esa role.
        _btnSoloAdmin.Enabled = _sesion.EsAdministrador;
    }

    private async Task ListarClientesAsync()
    {
        try
        {
            _lblResultado.Text = "Consultando clientes...";
            var resp = await _api.ListarClientes(filtro: null);

            if (resp.Success && resp.Data is not null)
            {
                _grid.DataSource = resp.Data.Items.ToList();
                _lblResultado.Text =
                    $"Total: {resp.Data.Total} clientes — Página {resp.Data.Page}/{resp.Data.TotalPages}";
            }
            else
            {
                _lblResultado.Text = resp.Message ?? "Error al consultar.";
            }
        }
        catch (ApiException ex)
        {
            _lblResultado.Text = $"HTTP {(int)ex.StatusCode}: {ex.ReasonPhrase}";
        }
        catch (Exception ex)
        {
            _lblResultado.Text = $"Error: {ex.Message}";
        }
    }

    private async Task ProbarRbacAsync()
    {
        try
        {
            var resp = await _api.SoloAdmin();
            MessageBox.Show(
                resp.Data ?? "Sin datos",
                "Endpoint protegido por rol",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (ApiException ex)
        {
            MessageBox.Show(
                $"Acceso denegado (HTTP {(int)ex.StatusCode}).",
                "RBAC",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void CerrarSesion()
    {
        _tokens.Clear();
        _sesion.Limpiar();
        Close();
        Application.Exit();
    }
}
