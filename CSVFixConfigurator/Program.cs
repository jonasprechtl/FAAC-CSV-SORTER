using Config;
using System.Security.Principal;


namespace CSVFixConfigurator;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {

        if (!IsUserAdministrator())
        {
            MessageBox.Show("Diese Anwendung benötigt Administratorrechte", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        RegConfig.loadValuesFromRegistry();

        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
        
    }    


    static bool IsUserAdministrator()
    {
        bool isAdmin;
        try
        {
            WindowsIdentity user = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(user);
            isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch (UnauthorizedAccessException ex)
        {
            isAdmin = false;
        }
        catch (Exception ex)
        {
            isAdmin = false;
        }
        return isAdmin;
    }
}