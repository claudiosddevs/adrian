using FIS.Contracts.Usuarios;
using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Usuarios;

public class frmUsuarios : Form
{
    private readonly IFisApiClient _api;
    private DataGridView _grid = default!;
    private Button _btnNuevo = default!;
    private Label _lblStatus = default!;

    public frmUsuarios(IFisApiClient api)
    {
        _api = api;
        ConstruirUi();
        Load += async (_, _) => await CargarAsync();
    }

    private void ConstruirUi()
    {
        Text = "Gestión de Usuarios y Roles (RF16)";
        Size = new Size(900, 500);
        StartPosition = FormStartPosition.CenterScreen;

        var toolbar = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };
        _btnNuevo = new Button { Text = "+ Nuevo Usuario", Location = new Point(10, 10), Size = new Size(130, 28), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        _btnNuevo.Click += AbrirNuevo;
        toolbar.Controls.Add(_btnNuevo);

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
    }

    private async Task CargarAsync()
    {
        try
        {
            var resp = await _api.ListarUsuarios();
            if (resp.Success && resp.Data is not null)
            {
                _grid.DataSource = resp.Data.ToList();
                _lblStatus.Text = $"{resp.Data.Count} usuarios.";
            }
        }
        catch (ApiException ex) { _lblStatus.Text = $"Error HTTP {(int)ex.StatusCode}"; }
    }

    private void AbrirNuevo(object? sender, EventArgs e)
    {
        using var frm = new frmUsuarioNuevo(_api);
        if (frm.ShowDialog() == DialogResult.OK) _ = CargarAsync();
    }
}
