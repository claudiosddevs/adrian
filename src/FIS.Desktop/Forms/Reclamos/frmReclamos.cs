using FIS.Contracts.Reclamos;
using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Reclamos;

public class frmReclamos : Form
{
    private readonly IFisApiClient _api;
    private readonly SesionUsuario _sesion;

    private DataGridView _grid = default!;
    private ComboBox _cmbEstado = default!;
    private Button _btnBuscar = default!;
    private Button _btnNuevo = default!;
    private Button _btnAsignar = default!;
    private Button _btnCambiarEstado = default!;
    private Label _lblStatus = default!;

    public frmReclamos(IFisApiClient api, SesionUsuario sesion)
    {
        _api = api;
        _sesion = sesion;
        ConstruirUi();
        Load += async (_, _) => await CargarAsync();
    }

    private void ConstruirUi()
    {
        Text = "Soporte Técnico — Reclamos (RF09-RF13)";
        Size = new Size(1200, 600);
        StartPosition = FormStartPosition.CenterScreen;

        var toolbar = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };
        Controls.Add(new Label { Text = "Estado:", Location = new Point(10, 16), AutoSize = true, Parent = toolbar });

        _cmbEstado = new ComboBox
        {
            Location = new Point(70, 13),
            Width = 130,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Parent = toolbar
        };
        _cmbEstado.Items.AddRange(new object[] { "(Todos)", "Recepcionado", "En Proceso", "Observado", "Cerrado" });
        _cmbEstado.SelectedIndex = 0;

        _btnBuscar = new Button { Text = "Filtrar", Location = new Point(210, 12), Size = new Size(70, 26), Parent = toolbar };
        _btnNuevo = new Button { Text = "+ Registrar", Location = new Point(290, 12), Size = new Size(110, 26), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Parent = toolbar };
        _btnAsignar = new Button { Text = "Asignar Técnico", Location = new Point(410, 12), Size = new Size(120, 26), BackColor = Color.Orange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Parent = toolbar };
        _btnCambiarEstado = new Button { Text = "Cambiar Estado", Location = new Point(540, 12), Size = new Size(120, 26), BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Parent = toolbar };

        _btnBuscar.Click += async (_, _) => await CargarAsync();
        _btnNuevo.Click += AbrirNuevoReclamo;
        _btnAsignar.Click += AbrirAsignarTecnico;
        _btnCambiarEstado.Click += AbrirCambiarEstado;

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

        _btnAsignar.Enabled = _sesion.EsAdministrador;
    }

    private async Task CargarAsync()
    {
        try
        {
            var estado = _cmbEstado.SelectedIndex == 0 ? null : _cmbEstado.Text;
            var resp = await _api.ListarReclamos(estado, null);
            if (resp.Success && resp.Data is not null)
            {
                _grid.DataSource = resp.Data.Items.ToList();
                _lblStatus.Text = $"{resp.Data.Total} reclamos.";
            }
        }
        catch (ApiException ex) { _lblStatus.Text = $"Error HTTP {(int)ex.StatusCode}"; }
    }

    private void AbrirNuevoReclamo(object? sender, EventArgs e)
    {
        using var frm = new frmRegistrarReclamo(_api);
        if (frm.ShowDialog() == DialogResult.OK) _ = CargarAsync();
    }

    private void AbrirAsignarTecnico(object? sender, EventArgs e)
    {
        if (_grid.CurrentRow?.DataBoundItem is not ReclamoDto dto) return;
        var idStr = Microsoft.VisualBasic.Interaction.InputBox("Ingrese el ID del técnico a asignar:", "Asignar Técnico");
        if (!int.TryParse(idStr, out var tecnicoId)) return;

        _ = AsignarTecnicoAsync(dto.IdReclamo, tecnicoId);
    }

    private async Task AsignarTecnicoAsync(int reclamoId, int tecnicoId)
    {
        try
        {
            await _api.AsignarTecnico(reclamoId, new AsignarTecnicoRequest { TecnicoId = tecnicoId });
            await CargarAsync();
        }
        catch (ApiException ex) { MessageBox.Show($"Error HTTP {(int)ex.StatusCode}"); }
    }

    private void AbrirCambiarEstado(object? sender, EventArgs e)
    {
        if (_grid.CurrentRow?.DataBoundItem is not ReclamoDto dto) return;
        using var frm = new frmCambiarEstadoReclamo(_api, dto);
        if (frm.ShowDialog() == DialogResult.OK) _ = CargarAsync();
    }
}
