using FIS.Contracts.Planes;
using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Planes;

public class frmPlanDetalle : Form
{
    private readonly IFisApiClient _api;
    private readonly PlanDto? _planExistente;

    private TextBox _txtNombre = default!;
    private ComboBox _cmbTipo = default!;
    private TextBox _txtBajada = default!;
    private TextBox _txtSubida = default!;
    private TextBox _txtPrecio = default!;
    private Button _btnGuardar = default!;

    public frmPlanDetalle(IFisApiClient api, PlanDto? plan)
    {
        _api = api;
        _planExistente = plan;
        ConstruirUi();
        if (plan is not null) CargarDatos(plan);
    }

    private void ConstruirUi()
    {
        Text = _planExistente is null ? "Nuevo Plan" : "Editar Plan";
        Size = new Size(420, 340);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        int y = 20;
        _txtNombre = AgregarTexto("Nombre del plan:", ref y);
        _cmbTipo = AgregarCombo("Tipo de servicio:", new[] { "Internet", "Hosting", "Dominio", "Fibra", "Otro" }, ref y);
        _txtBajada = AgregarTexto("Velocidad bajada (Mbps):", ref y);
        _txtSubida = AgregarTexto("Velocidad subida (Mbps):", ref y);
        _txtPrecio = AgregarTexto("Precio mensual (Bs.):", ref y);

        _btnGuardar = new Button { Text = "Guardar", Size = new Size(100, 30), Location = new Point(290, y + 10), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        var btnCancelar = new Button { Text = "Cancelar", Size = new Size(100, 30), Location = new Point(180, y + 10) };
        _btnGuardar.Click += async (_, _) => await GuardarAsync();
        btnCancelar.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        Controls.AddRange(new Control[] { _btnGuardar, btnCancelar });
    }

    private TextBox AgregarTexto(string label, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true });
        var txt = new TextBox { Location = new Point(200, y), Width = 180 };
        Controls.Add(txt);
        y += 35;
        return txt;
    }

    private ComboBox AgregarCombo(string label, string[] items, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true });
        var cmb = new ComboBox { Location = new Point(200, y), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
        cmb.Items.AddRange(items);
        cmb.SelectedIndex = 0;
        Controls.Add(cmb);
        y += 35;
        return cmb;
    }

    private void CargarDatos(PlanDto p)
    {
        _txtNombre.Text = p.NombrePlan;
        _cmbTipo.Text = p.TipoServicio;
        _txtBajada.Text = p.VelocidadBajadaMbps?.ToString() ?? string.Empty;
        _txtSubida.Text = p.VelocidadSubidaMbps?.ToString() ?? string.Empty;
        _txtPrecio.Text = p.PrecioMensual.ToString();
    }

    private async Task GuardarAsync()
    {
        if (!decimal.TryParse(_txtPrecio.Text, out var precio) || precio <= 0)
        {
            MessageBox.Show("El precio debe ser un número positivo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        decimal.TryParse(_txtBajada.Text, out var bajada);
        decimal.TryParse(_txtSubida.Text, out var subida);

        var req = new CrearPlanRequest
        {
            NombrePlan = _txtNombre.Text.Trim(),
            TipoServicio = _cmbTipo.Text,
            VelocidadBajadaMbps = bajada > 0 ? bajada : null,
            VelocidadSubidaMbps = subida > 0 ? subida : null,
            PrecioMensual = precio
        };

        try
        {
            _btnGuardar.Enabled = false;
            if (_planExistente is null) await _api.CrearPlan(req);
            else await _api.ActualizarPlan(_planExistente.IdPlan, req);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (ApiException ex)
        {
            MessageBox.Show($"Error HTTP {(int)ex.StatusCode}: {ex.Content}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _btnGuardar.Enabled = true;
        }
    }
}
