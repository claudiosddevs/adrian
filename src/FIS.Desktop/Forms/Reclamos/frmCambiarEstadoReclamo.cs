using FIS.Contracts.Reclamos;
using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Reclamos;

public class frmCambiarEstadoReclamo : Form
{
    private readonly IFisApiClient _api;
    private readonly ReclamoDto _reclamo;
    private ComboBox _cmbEstado = default!;
    private TextBox _txtObservacion = default!;
    private Button _btnGuardar = default!;

    public frmCambiarEstadoReclamo(IFisApiClient api, ReclamoDto reclamo)
    {
        _api = api;
        _reclamo = reclamo;
        ConstruirUi();
    }

    private void ConstruirUi()
    {
        Text = $"Cambiar Estado — Reclamo #{_reclamo.IdReclamo}";
        Size = new Size(420, 280);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        int y = 20;
        Controls.Add(new Label { Text = $"Estado actual: {_reclamo.Estado}", Location = new Point(20, y), AutoSize = true });
        y += 35;

        Controls.Add(new Label { Text = "Nuevo estado:", Location = new Point(20, y), AutoSize = true });
        _cmbEstado = new ComboBox { Location = new Point(150, y), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbEstado.Items.AddRange(new object[] { "Recepcionado", "En Proceso", "Observado", "Cerrado" });
        _cmbEstado.Text = _reclamo.Estado;
        Controls.Add(_cmbEstado);
        y += 35;

        Controls.Add(new Label { Text = "Observación:", Location = new Point(20, y), AutoSize = true });
        _txtObservacion = new TextBox { Location = new Point(150, y), Width = 220, Height = 80, Multiline = true };
        Controls.Add(_txtObservacion);
        y += 95;

        _btnGuardar = new Button { Text = "Guardar", Size = new Size(100, 30), Location = new Point(290, y + 5), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        var btnCancelar = new Button { Text = "Cancelar", Size = new Size(100, 30), Location = new Point(180, y + 5) };
        _btnGuardar.Click += async (_, _) => await GuardarAsync();
        btnCancelar.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        Controls.AddRange(new Control[] { _btnGuardar, btnCancelar });
    }

    private async Task GuardarAsync()
    {
        try
        {
            _btnGuardar.Enabled = false;
            await _api.CambiarEstadoReclamo(_reclamo.IdReclamo, new CambiarEstadoReclamoRequest
            {
                NuevoEstado = _cmbEstado.Text,
                Observacion = _txtObservacion.Text.Trim()
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
