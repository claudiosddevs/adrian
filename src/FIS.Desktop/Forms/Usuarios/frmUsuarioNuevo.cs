using FIS.Contracts.Usuarios;
using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Usuarios;

public class frmUsuarioNuevo : Form
{
    private readonly IFisApiClient _api;
    private TextBox _txtUsername = default!;
    private TextBox _txtNombreCompleto = default!;
    private TextBox _txtEmail = default!;
    private TextBox _txtPassword = default!;
    private ComboBox _cmbRol = default!;
    private Button _btnGuardar = default!;

    public frmUsuarioNuevo(IFisApiClient api) { _api = api; ConstruirUi(); }

    private void ConstruirUi()
    {
        Text = "Crear Nuevo Usuario";
        Size = new Size(420, 360);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        int y = 20;
        _txtUsername = AgregarTexto("Username:", ref y);
        _txtNombreCompleto = AgregarTexto("Nombre completo:", ref y);
        _txtEmail = AgregarTexto("Email:", ref y);
        _txtPassword = AgregarTexto("Contraseña:", ref y, isPassword: true);
        _cmbRol = AgregarComboRol("Rol:", ref y);

        _btnGuardar = new Button { Text = "Crear", Size = new Size(100, 30), Location = new Point(290, y + 10), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        var btnCancelar = new Button { Text = "Cancelar", Size = new Size(100, 30), Location = new Point(180, y + 10) };
        _btnGuardar.Click += async (_, _) => await GuardarAsync();
        btnCancelar.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        Controls.AddRange(new Control[] { _btnGuardar, btnCancelar });
    }

    private TextBox AgregarTexto(string label, ref int y, bool isPassword = false)
    {
        Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true });
        var txt = new TextBox { Location = new Point(160, y), Width = 220, UseSystemPasswordChar = isPassword };
        Controls.Add(txt);
        y += 35;
        return txt;
    }

    private ComboBox AgregarComboRol(string label, ref int y)
    {
        Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true });
        var cmb = new ComboBox { Location = new Point(160, y), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
        cmb.Items.AddRange(new object[]
        {
            new { Text = "Administrador", Value = 1 },
            new { Text = "Cajero", Value = 2 },
            new { Text = "Tecnico", Value = 3 }
        });
        cmb.DisplayMember = "Text";
        cmb.ValueMember = "Value";
        cmb.SelectedIndex = 1;
        Controls.Add(cmb);
        y += 35;
        return cmb;
    }

    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtUsername.Text) || string.IsNullOrWhiteSpace(_txtPassword.Text))
        {
            MessageBox.Show("Username y contraseña son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selectedRol = (dynamic)_cmbRol.SelectedItem!;

        try
        {
            _btnGuardar.Enabled = false;
            await _api.CrearUsuario(new CrearUsuarioRequest
            {
                NombreUsuario = _txtUsername.Text.Trim(),
                NombreCompleto = _txtNombreCompleto.Text.Trim(),
                Email = _txtEmail.Text.Trim(),
                Password = _txtPassword.Text,
                RolId = selectedRol.Value
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
