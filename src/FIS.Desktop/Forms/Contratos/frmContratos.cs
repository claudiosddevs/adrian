using FIS.Contracts.Contratos;
using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Contratos;

public class frmContratos : Form
{
    private readonly IFisApiClient _api;
    private readonly SesionUsuario _sesion;

    private DataGridView _grid = default!;
    private Button _btnNuevo = default!;
    private Button _btnSuspender = default!;
    private Button _btnReactivar = default!;
    private Label _lblStatus = default!;

    public frmContratos(IFisApiClient api, SesionUsuario sesion)
    {
        _api = api;
        _sesion = sesion;
        ConstruirUi();
        Load += async (_, _) => await CargarAsync();
    }

    private void ConstruirUi()
    {
        Text = "Gestión de Contratos (RF05)";
        Size = new Size(1100, 550);
        StartPosition = FormStartPosition.CenterScreen;

        var toolbar = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };
        _btnNuevo = new Button { Text = "+ Nuevo Contrato", Location = new Point(10, 10), Size = new Size(130, 28), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        _btnSuspender = new Button { Text = "Suspender", Location = new Point(150, 10), Size = new Size(90, 28), BackColor = Color.Orange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        _btnReactivar = new Button { Text = "Reactivar", Location = new Point(250, 10), Size = new Size(90, 28), BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

        _btnNuevo.Click += AbrirNuevo;
        _btnSuspender.Click += async (_, _) => await CambiarEstadoAsync("suspender");
        _btnReactivar.Click += async (_, _) => await CambiarEstadoAsync("reactivar");
        toolbar.Controls.AddRange(new Control[] { _btnNuevo, _btnSuspender, _btnReactivar });

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

        _btnNuevo.Enabled = _btnSuspender.Enabled = _btnReactivar.Enabled = _sesion.EsAdministrador;
    }

    private async Task CargarAsync()
    {
        try
        {
            var resp = await _api.ListarContratos();
            if (resp.Success && resp.Data is not null)
            {
                _grid.DataSource = resp.Data.Items.ToList();
                _lblStatus.Text = $"{resp.Data.Total} contratos.";
            }
        }
        catch (ApiException ex) { _lblStatus.Text = $"Error HTTP {(int)ex.StatusCode}"; }
    }

    private void AbrirNuevo(object? sender, EventArgs e)
    {
        using var frm = new frmContratoNuevo(_api);
        if (frm.ShowDialog() == DialogResult.OK) _ = CargarAsync();
    }

    private async Task CambiarEstadoAsync(string accion)
    {
        if (_grid.CurrentRow?.DataBoundItem is not ContratoDto dto) return;
        try { await _api.CambiarEstadoContrato(dto.IdContrato, accion); await CargarAsync(); }
        catch (ApiException ex) { MessageBox.Show($"Error HTTP {(int)ex.StatusCode}"); }
    }
}
