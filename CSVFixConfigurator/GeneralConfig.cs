using Config;
namespace CSVFixConfigurator;

public partial class Form1 : Form
{
    bool AuthEnabledOnLoad;

    public Form1()
    {
        string[] vals = RegConfig.getValues();
        /*
            If Authentication was already enabled on Load, the User does not have to enter the password and username again
            If however Authentication was not enabled on Load, the User has to enter the password and username to avoid not having credentials stored
        */
        this.AuthEnabledOnLoad = vals[3] == "1";


        InitializeComponent();

        this.Icon =  this.Icon = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("CSVFixConfigurator.logo.ico"));

        this.txtInputFile.Text = vals[0];
        this.txtOutputFile.Text = vals[1];
        this.cmbExecutionTime.SelectedItem = vals[2];
        this.chkUseAuth.Checked = vals[3] == "1";
        
        //This enables or disables the username and password input fields depending on the checkbox
        this.txtUsername.Enabled = this.chkUseAuth.Checked;
        this.txtPassword.Enabled = this.chkUseAuth.Checked;

    }
}
