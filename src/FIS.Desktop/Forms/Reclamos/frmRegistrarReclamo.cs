using FIS.Contracts.Reclamos;
using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Reclamos;

public class frmRegistrarReclamo : Form
{
    private readonly IFisApiClient _api;
    private TextBox _txtClienteId = default!;
    private TextBox _txtDescripcion = default!;
    private ComboBox _cmbClasificacion = default!;
    private Button _btnGuardar = default!;

    public frmRegistrarReclamo(IFisApiClient api) { _api = api; ConstruirUi(); }

    private void ConstruirUi()
    {
        Text = "Registrar Reclamo";
        Size = new Size(420, 280);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        int y = 20;
        _txtClienteId = AgregarTexto("ID del Cliente:", ref y);
        _txtDescripcion = AgregarTexto("Descripción:", ref y, height: 80);
        _cmbClasificacion = AgregarCombo("Clasificación:", new[] { "Leve", "Medio", "Complejo" }, ref y);

        _btnGuardar = new Button { Text = "Registrar", Size = new Size(100, 30), Location = new Point(290, y + 10), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        var btnCancelar = new Button { Text = "Cancelar", Size = new Size(100, 30), Location = new Point(180, y + 10) };
        _btnGuardar.Click += async (_, _) => await GuardarAsync();
        btnCancelar.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        Controls.AddRange(new Control[] { _btnGuardar, btnCancelar });
    }

    private TextBox AgregarTexto(string label, ref int y, int height = 24)
    {
        Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true });
        var txt = new TextBox { Location = new Point(160, y), Width = 220, Height = height, Multiline = height > 24 };
        Controls.Add(txt);
        y += height + 11;
        return txt;
    }

    private ComboBox AgregarCombo(string label, string[] items, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true });
        var cmb = new ComboBox { Location = new Point(160, y), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
        cmb.Items.AddRange(items);
        cmb.SelectedIndex = 0;
        Controls.Add(cmb);
        y += 35;
        return cmb;
    }

    private async Task GuardarAsync()
    {
        if (!int.TryParse(_txtClienteId.Text, out var clienteId) || string.IsNullOrWhiteSpace(_txtDescripcion.Text))
        {
            MessageBox.Show("ID de cliente y descripción son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _btnGuardar.Enabled = false;
            await _api.RegistrarReclamo(new RegistrarReclamoRequest
            {
                ClienteId = clienteId,
                Descripcion = _txtDescripcion.Text.Trim(),
                Clasificacion = _cmbClasificacion.Text
            });
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
