using FIS.Contracts.Pagos;
using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Pagos;

public class frmPagos : Form
{
    private readonly IFisApiClient _api;
    private readonly SesionUsuario _sesion;

    private DataGridView _grid = default!;
    private Button _btnNuevoPago = default!;
    private Button _btnAnular = default!;
    private Label _lblStatus = default!;

    public frmPagos(IFisApiClient api, SesionUsuario sesion)
    {
        _api = api;
        _sesion = sesion;
        ConstruirUi();
        Load += async (_, _) => await CargarAsync();
    }

    private void ConstruirUi()
    {
        Text = "Registro de Pagos (RF06, RF07)";
        Size = new Size(1000, 550);
        StartPosition = FormStartPosition.CenterScreen;

        var toolbar = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };
        _btnNuevoPago = new Button { Text = "+ Registrar Pago", Location = new Point(10, 10), Size = new Size(130, 28), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        _btnAnular = new Button { Text = "Anular Pago", Location = new Point(150, 10), Size = new Size(110, 28), BackColor = Color.FromArgb(220, 38, 38), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

        _btnNuevoPago.Click += AbrirNuevoPago;
        _btnAnular.Click += AbrirAnularPago;
        toolbar.Controls.AddRange(new Control[] { _btnNuevoPago, _btnAnular });

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

        _btnAnular.Enabled = _sesion.EsAdministrador;
    }

    private async Task CargarAsync()
    {
        try
        {
            var resp = await _api.ListarPagos();
            if (resp.Success && resp.Data is not null)
            {
                _grid.DataSource = resp.Data.Items.ToList();
                _lblStatus.Text = $"{resp.Data.Total} pagos registrados.";
            }
        }
        catch (ApiException ex) { _lblStatus.Text = $"Error HTTP {(int)ex.StatusCode}"; }
    }

    private void AbrirNuevoPago(object? sender, EventArgs e)
    {
        using var frm = new frmRegistrarPago(_api);
        if (frm.ShowDialog() == DialogResult.OK) _ = CargarAsync();
    }

    private void AbrirAnularPago(object? sender, EventArgs e)
    {
        if (_grid.CurrentRow?.DataBoundItem is not PagoDto dto) return;
        var motivo = Microsoft.VisualBasic.Interaction.InputBox("Ingrese el motivo de anulación:", "Anular Pago");
        if (string.IsNullOrWhiteSpace(motivo)) return;

        _ = AnularPagoAsync(dto.IdPago, motivo);
    }

    private async Task AnularPagoAsync(int id, string motivo)
    {
        try
        {
            await _api.AnularPago(new AnularPagoRequest { IdPago = id, Motivo = motivo });
            await CargarAsync();
        }
        catch (ApiException ex)
        {
            MessageBox.Show($"Error HTTP {(int)ex.StatusCode}: {ex.Content}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
