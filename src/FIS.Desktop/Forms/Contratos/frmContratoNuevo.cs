using FIS.Contracts.Contratos;
using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Contratos;

public class frmContratoNuevo : Form
{
    private readonly IFisApiClient _api;

    private TextBox _txtClienteId = default!;
    private ComboBox _cmbPlan = default!;
    private DateTimePicker _dtpInicio = default!;
    private DateTimePicker _dtpFin = default!;
    private ComboBox _cmbMetodoPago = default!;
    private Button _btnGuardar = default!;

    public frmContratoNuevo(IFisApiClient api)
    {
        _api = api;
        ConstruirUi();
        Load += async (_, _) => await CargarPlanesAsync();
    }

    private void ConstruirUi()
    {
        Text = "Registrar Nuevo Contrato";
        Size = new Size(420, 380);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        int y = 20;
        _txtClienteId = AgregarTexto("ID del Cliente:", ref y);
        _cmbPlan = AgregarCombo("Plan de servicio:", Array.Empty<string>(), ref y);
        Controls.Add(new Label { Text = "Fecha inicio:", Location = new Point(20, y), AutoSize = true });
        _dtpInicio = new DateTimePicker { Location = new Point(170, y), Width = 200, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
        Controls.Add(_dtpInicio);
        y += 35;
        Controls.Add(new Label { Text = "Fecha fin:", Location = new Point(20, y), AutoSize = true });
        _dtpFin = new DateTimePicker { Location = new Point(170, y), Width = 200, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddYears(1) };
        Controls.Add(_dtpFin);
        y += 35;
        _cmbMetodoPago = AgregarCombo("Método de pago:", new[] { "Mensual", "Anual" }, ref y);

        _btnGuardar = new Button { Text = "Guardar", Size = new Size(100, 30), Location = new Point(290, y + 10), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        var btnCancelar = new Button { Text = "Cancelar", Size = new Size(100, 30), Location = new Point(180, y + 10) };
        _btnGuardar.Click += async (_, _) => await GuardarAsync();
        btnCancelar.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        Controls.AddRange(new Control[] { _btnGuardar, btnCancelar });
    }

    private TextBox AgregarTexto(string label, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true });
        var txt = new TextBox { Location = new Point(170, y), Width = 200 };
        Controls.Add(txt);
        y += 35;
        return txt;
    }

    private ComboBox AgregarCombo(string label, string[] items, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true });
        var cmb = new ComboBox { Location = new Point(170, y), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        cmb.Items.AddRange(items);
        if (items.Length > 0) cmb.SelectedIndex = 0;
        Controls.Add(cmb);
        y += 35;
        return cmb;
    }

    private async Task CargarPlanesAsync()
    {
        try
        {
            var resp = await _api.ListarPlanes(true);
            if (resp.Success && resp.Data is not null)
            {
                _cmbPlan.DisplayMember = "NombrePlan";
                _cmbPlan.ValueMember = "IdPlan";
                _cmbPlan.DataSource = resp.Data.ToList();
            }
        }
        catch { }
    }

    private async Task GuardarAsync()
    {
        if (!int.TryParse(_txtClienteId.Text, out var clienteId))
        {
            MessageBox.Show("Ingrese un ID de cliente válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_cmbPlan.SelectedItem is null)
        {
            MessageBox.Show("Seleccione un plan.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var plan = (FIS.Contracts.Planes.PlanDto)_cmbPlan.SelectedItem;
        var req = new RegistrarContratoRequest
        {
            ClienteId = clienteId,
            PlanId = plan.IdPlan,
            FechaInicio = _dtpInicio.Value,
            FechaFin = _dtpFin.Value,
            MetodoPago = _cmbMetodoPago.Text
        };

        try
        {
            _btnGuardar.Enabled = false;
            await _api.RegistrarContrato(req);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (ApiException ex)
        {
            MessageBox.Show($"Error: {ex.Content}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _btnGuardar.Enabled = true;
        }
    }
}
