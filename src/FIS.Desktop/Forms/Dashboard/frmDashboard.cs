using FIS.Desktop.Forms.Clientes;
using FIS.Desktop.Forms.Contratos;
using FIS.Desktop.Forms.Pagos;
using FIS.Desktop.Forms.Planes;
using FIS.Desktop.Forms.Reclamos;
using FIS.Desktop.Forms.Reportes;
using FIS.Desktop.Forms.Usuarios;
using FIS.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FIS.Desktop.Forms.Dashboard;

public class frmDashboard : Form
{
    private readonly IFisApiClient _api;
    private readonly TokenStore _tokens;
    private readonly SesionUsuario _sesion;
    private readonly IServiceProvider _sp;

    private Label _lblBienvenida = default!;
    private Label _lblRol = default!;
    private Button _btnCerrarSesion = default!;

    // botones del sidebar
    private readonly List<(Button Btn, Action Click)> _menuItems = new();

    public frmDashboard(IFisApiClient api, TokenStore tokens, SesionUsuario sesion, IServiceProvider sp)
    {
        _api = api;
        _tokens = tokens;
        _sesion = sesion;
        _sp = sp;

        ConstruirUi();
        Load += (_, _) => AjustarPorRol();
    }

    private void ConstruirUi()
    {
        Text = "Full Internet Services — Panel Administrativo";
        Size = new Size(1100, 680);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.WhiteSmoke;
        MinimumSize = new Size(900, 580);

        // Header
        var header = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.FromArgb(37, 99, 235) };
        var lblTitulo = new Label { Text = "FIS — Panel Administrativo", Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 18), AutoSize = true };
        _lblBienvenida = new Label { Font = new Font("Segoe UI", 10F), ForeColor = Color.LightBlue, Location = new Point(20, 52), AutoSize = true };
        header.Controls.AddRange(new Control[] { lblTitulo, _lblBienvenida });

        // Sidebar
        var sidebar = new Panel { Dock = DockStyle.Left, Width = 230, BackColor = Color.FromArgb(31, 41, 55) };
        _lblRol = new Label { Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(251, 191, 36), Location = new Point(20, 15), AutoSize = true };
        sidebar.Controls.Add(_lblRol);

        // Main content (Label de bienvenida)
        var lblMain = new Label
        {
            Text = "Seleccione un módulo del menú lateral →",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 14F),
            ForeColor = Color.Gray
        };

        // Módulos del menú
        int btnY = 45;
        void AddMenuBtn(string texto, bool soloAdmin, Action accion)
        {
            var btn = CrearBotonMenu(texto, btnY);
            btn.Tag = soloAdmin;
            btn.Click += (_, _) => accion();
            sidebar.Controls.Add(btn);
            _menuItems.Add((btn, accion));
            btnY += 44;
        }

        AddMenuBtn("Clientes", false, AbrirClientes);
        AddMenuBtn("Planes de Servicio", true, AbrirPlanes);
        AddMenuBtn("Contratos", true, AbrirContratos);
        AddMenuBtn("Pagos", false, AbrirPagos);
        AddMenuBtn("Soporte / Reclamos", false, AbrirReclamos);
        AddMenuBtn("Usuarios y Roles", true, AbrirUsuarios);
        AddMenuBtn("Reportes", true, AbrirReportes);

        _btnCerrarSesion = CrearBotonMenu("Cerrar Sesión", btnY + 20);
        _btnCerrarSesion.BackColor = Color.FromArgb(220, 38, 38);
        _btnCerrarSesion.Click += (_, _) => CerrarSesion();
        sidebar.Controls.Add(_btnCerrarSesion);

        Controls.Add(lblMain);
        Controls.Add(sidebar);
        Controls.Add(header);
    }

    private static Button CrearBotonMenu(string texto, int top) => new()
    {
        Text = texto,
        Location = new Point(15, top),
        Size = new Size(200, 36),
        BackColor = Color.FromArgb(55, 65, 81),
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat,
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(8, 0, 0, 0),
        Font = new Font("Segoe UI", 9F)
    };

    private void AjustarPorRol()
    {
        var u = _sesion.Actual;
        if (u is null) { CerrarSesion(); return; }
        _lblBienvenida.Text = $"Bienvenido(a), {u.NombreCompleto}";
        _lblRol.Text = $"ROL: {u.Rol.ToUpperInvariant()}";

        foreach (var (btn, _) in _menuItems)
        {
            var soloAdmin = (bool)(btn.Tag ?? false);
            if (soloAdmin)
                btn.Enabled = _sesion.EsAdministrador;
        }
    }

    private void AbrirClientes()
    {
        using var frm = new frmClientes(_api, _sesion);
        frm.ShowDialog(this);
    }

    private void AbrirPlanes()
    {
        using var frm = new frmPlanes(_api, _sesion);
        frm.ShowDialog(this);
    }

    private void AbrirContratos()
    {
        using var frm = new frmContratos(_api, _sesion);
        frm.ShowDialog(this);
    }

    private void AbrirPagos()
    {
        using var frm = new frmPagos(_api, _sesion);
        frm.ShowDialog(this);
    }

    private void AbrirReclamos()
    {
        using var frm = new frmReclamos(_api, _sesion);
        frm.ShowDialog(this);
    }

    private void AbrirUsuarios()
    {
        using var frm = new frmUsuarios(_api);
        frm.ShowDialog(this);
    }

    private void AbrirReportes()
    {
        using var frm = new frmReportes(_api);
        frm.ShowDialog(this);
    }

    private void CerrarSesion()
    {
        _tokens.Clear();
        _sesion.Limpiar();
        Close();
        Application.Exit();
    }
}
