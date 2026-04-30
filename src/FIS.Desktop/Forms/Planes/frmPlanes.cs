using FIS.Contracts.Planes;
using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Planes;

public class frmPlanes : Form
{
    private readonly IFisApiClient _api;
    private readonly SesionUsuario _sesion;

    private DataGridView _grid = default!;
    private Button _btnNuevo = default!;
    private Button _btnEditar = default!;
    private Button _btnDesactivar = default!;
    private CheckBox _chkSoloActivos = default!;
    private Label _lblStatus = default!;

    public frmPlanes(IFisApiClient api, SesionUsuario sesion)
    {
        _api = api;
        _sesion = sesion;
        ConstruirUi();
        Load += async (_, _) => await CargarAsync();
    }

    private void ConstruirUi()
    {
        Text = "Gestión de Planes de Servicio (RF03, RF04)";
        Size = new Size(900, 500);
        StartPosition = FormStartPosition.CenterScreen;

        var toolbar = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };
        _chkSoloActivos = new CheckBox { Text = "Solo activos", Location = new Point(10, 14), Checked = true };
        _chkSoloActivos.CheckedChanged += async (_, _) => await CargarAsync();
        _btnNuevo = new Button { Text = "+ Nuevo Plan", Location = new Point(120, 10), Size = new Size(110, 28), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        _btnEditar = new Button { Text = "Editar", Location = new Point(240, 10), Size = new Size(80, 28) };
        _btnDesactivar = new Button { Text = "Desactivar", Location = new Point(330, 10), Size = new Size(90, 28), BackColor = Color.FromArgb(220, 38, 38), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

        _btnNuevo.Click += AbrirNuevo;
        _btnEditar.Click += AbrirEditar;
        _btnDesactivar.Click += async (_, _) => await DesactivarAsync();
        toolbar.Controls.AddRange(new Control[] { _chkSoloActivos, _btnNuevo, _btnEditar, _btnDesactivar });

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

        _btnNuevo.Enabled = _btnEditar.Enabled = _btnDesactivar.Enabled = _sesion.EsAdministrador;
    }

    private async Task CargarAsync()
    {
        try
        {
            var resp = await _api.ListarPlanes(_chkSoloActivos.Checked);
            if (resp.Success && resp.Data is not null)
            {
                _grid.DataSource = resp.Data.ToList();
                _lblStatus.Text = $"{resp.Data.Count} planes.";
            }
        }
        catch (ApiException ex) { _lblStatus.Text = $"Error HTTP {(int)ex.StatusCode}"; }
    }

    private void AbrirNuevo(object? sender, EventArgs e)
    {
        using var frm = new frmPlanDetalle(_api, null);
        if (frm.ShowDialog() == DialogResult.OK) _ = CargarAsync();
    }

    private void AbrirEditar(object? sender, EventArgs e)
    {
        if (_grid.CurrentRow?.DataBoundItem is not PlanDto dto) return;
        using var frm = new frmPlanDetalle(_api, dto);
        if (frm.ShowDialog() == DialogResult.OK) _ = CargarAsync();
    }

    private async Task DesactivarAsync()
    {
        if (_grid.CurrentRow?.DataBoundItem is not PlanDto dto) return;
        if (MessageBox.Show($"¿Desactivar plan '{dto.NombrePlan}'?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        try { await _api.DesactivarPlan(dto.IdPlan); await CargarAsync(); }
        catch (ApiException ex) { MessageBox.Show($"Error HTTP {(int)ex.StatusCode}"); }
    }
}
