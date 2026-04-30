using FIS.Contracts.Clientes;
using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Clientes;

public class frmClientes : Form
{
    private readonly IFisApiClient _api;
    private readonly SesionUsuario _sesion;

    private DataGridView _grid = default!;
    private TextBox _txtFiltro = default!;
    private Button _btnBuscar = default!;
    private Button _btnNuevo = default!;
    private Button _btnEditar = default!;
    private Button _btnDesactivar = default!;
    private Label _lblStatus = default!;

    public frmClientes(IFisApiClient api, SesionUsuario sesion)
    {
        _api = api;
        _sesion = sesion;
        ConstruirUi();
        Load += async (_, _) => await CargarAsync();
    }

    private void ConstruirUi()
    {
        Text = "Gestión de Clientes (RF02)";
        Size = new Size(1000, 600);
        StartPosition = FormStartPosition.CenterScreen;

        var toolbar = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };
        _txtFiltro = new TextBox { PlaceholderText = "Buscar por nombre o NIT/CI...", Width = 250, Location = new Point(10, 12) };
        _btnBuscar = new Button { Text = "Buscar", Location = new Point(270, 10), Size = new Size(80, 28) };
        _btnNuevo = new Button { Text = "+ Nuevo", Location = new Point(370, 10), Size = new Size(90, 28), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        _btnEditar = new Button { Text = "Editar", Location = new Point(470, 10), Size = new Size(80, 28) };
        _btnDesactivar = new Button { Text = "Desactivar", Location = new Point(560, 10), Size = new Size(90, 28), BackColor = Color.FromArgb(220, 38, 38), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

        _btnBuscar.Click += async (_, _) => await CargarAsync();
        _btnNuevo.Click += AbrirNuevoCliente;
        _btnEditar.Click += AbrirEditarCliente;
        _btnDesactivar.Click += async (_, _) => await DesactivarClienteAsync();

        toolbar.Controls.AddRange(new Control[] { _txtFiltro, _btnBuscar, _btnNuevo, _btnEditar, _btnDesactivar });

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };

        _lblStatus = new Label { Dock = DockStyle.Bottom, Height = 25, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0) };

        Controls.Add(_grid);
        Controls.Add(_lblStatus);
        Controls.Add(toolbar);

        _btnNuevo.Enabled = _sesion.EsAdministrador;
        _btnEditar.Enabled = _sesion.EsAdministrador;
        _btnDesactivar.Enabled = _sesion.EsAdministrador;
    }

    private async Task CargarAsync()
    {
        try
        {
            _lblStatus.Text = "Cargando...";
            var resp = await _api.ListarClientes(_txtFiltro.Text.Trim(), 1, 100);
            if (resp.Success && resp.Data is not null)
            {
                _grid.DataSource = resp.Data.Items.ToList();
                _lblStatus.Text = $"{resp.Data.Total} clientes encontrados.";
            }
        }
        catch (ApiException ex)
        {
            _lblStatus.Text = $"Error HTTP {(int)ex.StatusCode}";
        }
        catch (Exception ex)
        {
            _lblStatus.Text = $"Error: {ex.Message}";
        }
    }

    private void AbrirNuevoCliente(object? sender, EventArgs e)
    {
        using var frm = new frmClienteDetalle(_api, null, _sesion);
        if (frm.ShowDialog() == DialogResult.OK)
            _ = CargarAsync();
    }

    private void AbrirEditarCliente(object? sender, EventArgs e)
    {
        if (_grid.CurrentRow?.DataBoundItem is not ClienteDto dto) return;
        using var frm = new frmClienteDetalle(_api, dto, _sesion);
        if (frm.ShowDialog() == DialogResult.OK)
            _ = CargarAsync();
    }

    private async Task DesactivarClienteAsync()
    {
        if (_grid.CurrentRow?.DataBoundItem is not ClienteDto dto) return;
        var confirmacion = MessageBox.Show(
            $"¿Desactivar cliente '{dto.NombreRazonSocial}'?",
            "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirmacion != DialogResult.Yes) return;

        try
        {
            await _api.DesactivarCliente(dto.IdCliente);
            await CargarAsync();
        }
        catch (ApiException ex)
        {
            MessageBox.Show($"Error: HTTP {(int)ex.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
