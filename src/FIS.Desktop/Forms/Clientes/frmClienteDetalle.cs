using FIS.Contracts.Clientes;
using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Clientes;

public class frmClienteDetalle : Form
{
    private readonly IFisApiClient _api;
    private readonly ClienteDto? _clienteExistente;
    private readonly SesionUsuario _sesion;

    private ComboBox _cmbTipo = default!;
    private TextBox _txtNombre = default!;
    private TextBox _txtNitCi = default!;
    private TextBox _txtEmail = default!;
    private TextBox _txtTelefono = default!;
    private TextBox _txtDireccion = default!;
    private TextBox _txtCiudad = default!;
    private Button _btnGuardar = default!;
    private Button _btnCancelar = default!;

    public frmClienteDetalle(IFisApiClient api, ClienteDto? cliente, SesionUsuario sesion)
    {
        _api = api;
        _clienteExistente = cliente;
        _sesion = sesion;
        ConstruirUi();
        if (cliente is not null) CargarDatos(cliente);
    }

    private void ConstruirUi()
    {
        Text = _clienteExistente is null ? "Nuevo Cliente" : "Editar Cliente";
        Size = new Size(450, 420);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        int y = 20;
        _cmbTipo = AgregarCombo("Tipo:", new[] { "N - Natural", "J - Jurídico" }, ref y);
        _txtNombre = AgregarTexto("Nombre/Razón Social:", ref y);
        _txtNitCi = AgregarTexto("NIT/CI:", ref y);
        _txtEmail = AgregarTexto("Email:", ref y);
        _txtTelefono = AgregarTexto("Teléfono:", ref y);
        _txtDireccion = AgregarTexto("Dirección:", ref y);
        _txtCiudad = AgregarTexto("Ciudad:", ref y);

        _btnGuardar = new Button { Text = "Guardar", Size = new Size(100, 30), Location = new Point(240, y + 10), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        _btnCancelar = new Button { Text = "Cancelar", Size = new Size(100, 30), Location = new Point(130, y + 10) };
        _btnGuardar.Click += async (_, _) => await GuardarAsync();
        _btnCancelar.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.AddRange(new Control[] { _btnGuardar, _btnCancelar });
    }

    private ComboBox AgregarCombo(string label, string[] items, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true });
        var cmb = new ComboBox { Location = new Point(160, y), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
        cmb.Items.AddRange(items);
        cmb.SelectedIndex = 0;
        Controls.Add(cmb);
        y += 35;
        return cmb;
    }

    private TextBox AgregarTexto(string label, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true });
        var txt = new TextBox { Location = new Point(160, y), Width = 240 };
        Controls.Add(txt);
        y += 35;
        return txt;
    }

    private void CargarDatos(ClienteDto c)
    {
        _cmbTipo.SelectedIndex = c.TipoCliente == "N" ? 0 : 1;
        _txtNombre.Text = c.NombreRazonSocial;
        _txtNitCi.Text = c.NitCi;
        _txtEmail.Text = c.Email;
        _txtTelefono.Text = c.Telefono;
        _txtDireccion.Text = c.Direccion;
        _txtCiudad.Text = c.Ciudad;
        _txtNitCi.Enabled = false;
    }

    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtNombre.Text) || string.IsNullOrWhiteSpace(_txtNitCi.Text))
        {
            MessageBox.Show("Nombre y NIT/CI son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var req = new CrearClienteRequest
        {
            TipoCliente = _cmbTipo.SelectedIndex == 0 ? "N" : "J",
            NombreRazonSocial = _txtNombre.Text.Trim(),
            NitCi = _txtNitCi.Text.Trim(),
            Email = _txtEmail.Text.Trim(),
            Telefono = _txtTelefono.Text.Trim(),
            Direccion = _txtDireccion.Text.Trim(),
            Ciudad = _txtCiudad.Text.Trim()
        };

        try
        {
            _btnGuardar.Enabled = false;
            if (_clienteExistente is null)
                await _api.CrearCliente(req);
            else
                await _api.ActualizarCliente(_clienteExistente.IdCliente, req);

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
