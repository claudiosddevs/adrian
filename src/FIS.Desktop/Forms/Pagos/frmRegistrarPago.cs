using FIS.Contracts.Pagos;
using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Pagos;

public class frmRegistrarPago : Form
{
    private readonly IFisApiClient _api;
    private TextBox _txtContratoId = default!;
    private TextBox _txtMonto = default!;
    private ComboBox _cmbMetodo = default!;
    private Button _btnGuardar = default!;

    public frmRegistrarPago(IFisApiClient api)
    {
        _api = api;
        ConstruirUi();
    }

    private void ConstruirUi()
    {
        Text = "Registrar Pago";
        Size = new Size(360, 250);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        int y = 20;
        _txtContratoId = AgregarTexto("ID del Contrato:", ref y);
        _txtMonto = AgregarTexto("Monto (Bs.):", ref y);
        _cmbMetodo = AgregarCombo("Método de pago:", new[] { "Efectivo", "Transferencia", "QR", "Tarjeta" }, ref y);

        _btnGuardar = new Button { Text = "Registrar", Size = new Size(100, 30), Location = new Point(230, y + 10), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        var btnCancelar = new Button { Text = "Cancelar", Size = new Size(100, 30), Location = new Point(120, y + 10) };
        _btnGuardar.Click += async (_, _) => await GuardarAsync();
        btnCancelar.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        Controls.AddRange(new Control[] { _btnGuardar, btnCancelar });
    }

    private TextBox AgregarTexto(string label, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true });
        var txt = new TextBox { Location = new Point(160, y), Width = 160 };
        Controls.Add(txt);
        y += 35;
        return txt;
    }

    private ComboBox AgregarCombo(string label, string[] items, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true });
        var cmb = new ComboBox { Location = new Point(160, y), Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
        cmb.Items.AddRange(items);
        cmb.SelectedIndex = 0;
        Controls.Add(cmb);
        y += 35;
        return cmb;
    }

    private async Task GuardarAsync()
    {
        if (!int.TryParse(_txtContratoId.Text, out var contratoId) ||
            !decimal.TryParse(_txtMonto.Text, out var monto) || monto <= 0)
        {
            MessageBox.Show("ID de contrato y monto son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _btnGuardar.Enabled = false;
            await _api.RegistrarPago(new RegistrarPagoRequest { ContratoId = contratoId, Monto = monto, MetodoPago = _cmbMetodo.Text });
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
