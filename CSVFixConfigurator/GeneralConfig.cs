using Config;
namespace CSVFixConfigurator;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        string[] vals = RegConfig.getValues();

        this.Icon =  this.Icon = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("CSVFixConfigurator.logo.ico"));

        this.txtInputFile.Text = vals[0];
        this.txtOutputFile.Text = vals[1];
        this.cmbExecutionTime.SelectedItem = vals[2];
    }
}
