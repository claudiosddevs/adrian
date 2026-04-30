using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Reportes;

public class frmReportes : Form
{
    private readonly IFisApiClient _api;

    private TabControl _tabs = default!;
    private DataGridView _gridMora = default!;
    private DataGridView _gridVentas = default!;
    private DataGridView _gridTecnicos = default!;
    private NumericUpDown _nudAnio = default!;

    public frmReportes(IFisApiClient api)
    {
        _api = api;
        ConstruirUi();
        Load += async (_, _) => await CargarTodosAsync();
    }

    private void ConstruirUi()
    {
        Text = "Reportes del Sistema (RF14, RF15)";
        Size = new Size(1100, 600);
        StartPosition = FormStartPosition.CenterScreen;

        _tabs = new TabControl { Dock = DockStyle.Fill };

        var tabMora = new TabPage("Clientes en Mora (RF08)");
        _gridMora = CrearGrid();
        tabMora.Controls.Add(_gridMora);

        var tabVentas = new TabPage("Ventas por Mes");
        var ventasPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };
        ventasPanel.Controls.Add(new Label { Text = "Año:", Location = new Point(10, 15), AutoSize = true });
        _nudAnio = new NumericUpDown { Location = new Point(40, 12), Width = 80, Minimum = 2020, Maximum = 2099, Value = DateTime.Now.Year };
        var btnCargarVentas = new Button { Text = "Cargar", Location = new Point(130, 10), Size = new Size(80, 28) };
        btnCargarVentas.Click += async (_, _) => await CargarVentasAsync();
        ventasPanel.Controls.AddRange(new Control[] { _nudAnio, btnCargarVentas });
        _gridVentas = CrearGrid();
        tabVentas.Controls.Add(_gridVentas);
        tabVentas.Controls.Add(ventasPanel);

        var tabTecnicos = new TabPage("Desempeño Técnicos (RF14)");
        var btnRefrescarTecnicos = new Button { Text = "Actualizar", Dock = DockStyle.Top, Height = 30 };
        btnRefrescarTecnicos.Click += async (_, _) => await CargarTecnicosAsync();
        _gridTecnicos = CrearGrid();
        tabTecnicos.Controls.Add(_gridTecnicos);
        tabTecnicos.Controls.Add(btnRefrescarTecnicos);

        _tabs.TabPages.AddRange(new[] { tabMora, tabVentas, tabTecnicos });
        Controls.Add(_tabs);
    }

    private static DataGridView CrearGrid() => new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        BackgroundColor = Color.White
    };

    private async Task CargarTodosAsync()
    {
        await Task.WhenAll(CargarMoraAsync(), CargarVentasAsync(), CargarTecnicosAsync());
    }

    private async Task CargarMoraAsync()
    {
        try
        {
            var resp = await _api.ReporteMora();
            if (resp.Success && resp.Data is not null)
                _gridMora.DataSource = resp.Data.ToList();
        }
        catch (ApiException ex) { MessageBox.Show($"Error mora: HTTP {(int)ex.StatusCode}"); }
    }

    private async Task CargarVentasAsync()
    {
        try
        {
            var resp = await _api.ReporteVentas((int)_nudAnio.Value);
            if (resp.Success && resp.Data is not null)
                _gridVentas.DataSource = resp.Data.ToList();
        }
        catch (ApiException ex) { MessageBox.Show($"Error ventas: HTTP {(int)ex.StatusCode}"); }
    }

    private async Task CargarTecnicosAsync()
    {
        try
        {
            var resp = await _api.ReporteTecnicos();
            if (resp.Success && resp.Data is not null)
                _gridTecnicos.DataSource = resp.Data.ToList();
        }
        catch (ApiException ex) { MessageBox.Show($"Error técnicos: HTTP {(int)ex.StatusCode}"); }
    }
}
