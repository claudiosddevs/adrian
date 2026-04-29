using FIS.Contracts.Auth;
using FIS.Desktop.Forms.Dashboard;
using FIS.Desktop.Services;
using Refit;

namespace FIS.Desktop.Forms.Auth;

/// <summary>
/// Pantalla de inicio de sesión (HU01 — login estándar; el biométrico se añade
/// en una iteración posterior). Diseño programático para facilitar el bootstrap.
/// </summary>
public class frmLogin : Form
{
    private readonly IFisApiClient _api;
    private readonly TokenStore _tokens;
    private readonly SesionUsuario _sesion;
    private readonly IServiceProvider _services;

    private TextBox _txtUsername = default!;
    private TextBox _txtPassword = default!;
    private Button _btnLogin = default!;
    private Label _lblStatus = default!;

    public frmLogin(
        IFisApiClient api, TokenStore tokens, SesionUsuario sesion,
        IServiceProvider services)
    {
        _api = api;
        _tokens = tokens;
        _sesion = sesion;
        _services = services;

        ConstruirUi();
    }

    private void ConstruirUi()
    {
        Text = "Full Internet Services — Iniciar Sesión";
        Size = new Size(420, 320);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        var lblTitulo = new Label
        {
            Text = "Iniciar Sesión",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            Location = new Point(40, 25),
            AutoSize = true,
            ForeColor = Color.FromArgb(31, 41, 55)
        };

        var lblUsername = new Label
        {
            Text = "Usuario",
            Location = new Point(40, 80),
            AutoSize = true,
            Font = new Font("Segoe UI", 9F)
        };
        _txtUsername = new TextBox
        {
            Location = new Point(40, 100),
            Size = new Size(330, 25),
            Font = new Font("Segoe UI", 10F)
        };

        var lblPassword = new Label
        {
            Text = "Contraseña",
            Location = new Point(40, 140),
            AutoSize = true,
            Font = new Font("Segoe UI", 9F)
        };
        _txtPassword = new TextBox
        {
            Location = new Point(40, 160),
            Size = new Size(330, 25),
            UseSystemPasswordChar = true,
            Font = new Font("Segoe UI", 10F)
        };

        _btnLogin = new Button
        {
            Text = "Ingresar",
            Location = new Point(40, 205),
            Size = new Size(330, 38),
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };
        _btnLogin.FlatAppearance.BorderSize = 0;
        _btnLogin.Click += async (_, _) => await IntentarLoginAsync();
        AcceptButton = _btnLogin;

        _lblStatus = new Label
        {
            Location = new Point(40, 250),
            Size = new Size(330, 20),
            ForeColor = Color.Crimson,
            Font = new Font("Segoe UI", 9F)
        };

        Controls.AddRange(new Control[]
        {
            lblTitulo, lblUsername, _txtUsername, lblPassword,
            _txtPassword, _btnLogin, _lblStatus
        });
    }

    private async Task IntentarLoginAsync()
    {
        _lblStatus.Text = string.Empty;
        _btnLogin.Enabled = false;

        try
        {
            var req = new LoginRequest
            {
                Username = _txtUsername.Text.Trim(),
                Password = _txtPassword.Text
            };
            var resp = await _api.Login(req);

            if (resp.Success && resp.Data is not null)
            {
                _tokens.Save(
                    resp.Data.AccessToken,
                    resp.Data.RefreshToken,
                    resp.Data.ExpiresAt);
                _sesion.Establecer(resp.Data.User);

                var dashboard = (frmDashboard)_services
                    .GetService(typeof(frmDashboard))!;
                Hide();
                dashboard.FormClosed += (_, _) => Close();
                dashboard.Show();
            }
            else
            {
                _lblStatus.Text = resp.Message ?? "Credenciales inválidas.";
            }
        }
        catch (ApiException ex)
        {
            _lblStatus.Text = $"Error {(int)ex.StatusCode}: {ex.ReasonPhrase}";
        }
        catch (Exception ex)
        {
            _lblStatus.Text = $"Error de red: {ex.Message}";
        }
        finally
        {
            _btnLogin.Enabled = true;
        }
    }
}
