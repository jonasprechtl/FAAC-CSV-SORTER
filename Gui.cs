using System;
using System.Windows.Forms;

public class MainForm : Form
{
    private TextBox inputFolderPathTextBox;
    private TextBox outputFilePathTextBox;
    private Button updateCredentialsButton;
    private DateTimePicker executionTimePicker;
    private CheckBox continuousRunCheckBox;
    
    public MainForm()
    {
        // Eingabeordner TextBox
        inputFolderPathTextBox = new TextBox { Left = 20, Top = 20, Width = 200 };
        Controls.Add(inputFolderPathTextBox);
        
        // Ausgabedatei TextBox
        outputFilePathTextBox = new TextBox { Left = 20, Top = 50, Width = 200 };
        Controls.Add(outputFilePathTextBox);
        
        // Anmeldedaten aktualisieren Button
        updateCredentialsButton = new Button { Left = 20, Top = 80, Text = "Anmeldedaten Aktualisieren" };
        updateCredentialsButton.Click += UpdateCredentialsButtonClick;
        Controls.Add(updateCredentialsButton);
        
        // Ausführung um DateTimePicker
        executionTimePicker = new DateTimePicker
        {
            Left = 20,
            Top = 110,
            Format = DateTimePickerFormat.Time,
            Width = 200
        };
        Controls.Add(executionTimePicker);
        
        // Fortlaufend ausführen CheckBox
        continuousRunCheckBox = new CheckBox { Left = 20, Top = 140, Width = 200, Text = "Fortlaufend ausführen" };
        Controls.Add(continuousRunCheckBox);
    }

    private void UpdateCredentialsButtonClick(object sender, EventArgs e)
    {
        MessageBox.Show("Anmeldedaten aktualisieren...");
    }
    
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
