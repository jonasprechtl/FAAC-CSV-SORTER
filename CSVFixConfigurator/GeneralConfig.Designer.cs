using Config;
namespace CSVFixConfigurator;

partial class Form1
{
    private System.Windows.Forms.TextBox txtInputFile;
    private System.Windows.Forms.TextBox txtOutputFile;
    private System.Windows.Forms.Label lblInputFile;
    private System.Windows.Forms.Label lblOutputFile;
    private System.Windows.Forms.ComboBox cmbExecutionTime;
    private System.Windows.Forms.Label lblExecutionTime;
    private System.Windows.Forms.Button btnSave;

    // ... other member variables and methods ...

    private void InitializeComponent()
    {
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(438, 340);
        this.Text = "FAAC - Kennzeichenkorrektur";

        // Initialize input directory label and text box
        this.lblInputFile = new System.Windows.Forms.Label();
        this.lblInputFile.Text = "Eingabeordner";
        this.lblInputFile.Location = new System.Drawing.Point(10, 10); // Example location
        this.lblInputFile.Anchor = System.Windows.Forms.AnchorStyles.Left; // Example anchor

        this.txtInputFile = new System.Windows.Forms.TextBox();
        this.txtInputFile.Location = new System.Drawing.Point(120, 10); // Example location
        this.txtInputFile.Size = new System.Drawing.Size(308, 20); // Example size

        // Initialize output file label and text box
        this.lblOutputFile = new System.Windows.Forms.Label();
        this.lblOutputFile.Text = "Ausgabedatei";
        this.lblOutputFile.Location = new System.Drawing.Point(10, 40); // Example location

        this.txtOutputFile = new System.Windows.Forms.TextBox();
        this.txtOutputFile.Location = new System.Drawing.Point(120, 40); // Example location
        this.txtOutputFile.Size = new System.Drawing.Size(308, 20); // Example size

        // Initialize execution time label and combo box
        this.lblExecutionTime = new System.Windows.Forms.Label();
        this.lblExecutionTime.Text = "Ausführung um";
        this.lblExecutionTime.Location = new System.Drawing.Point(10, 70); // Example location

        this.cmbExecutionTime = new System.Windows.Forms.ComboBox();
        this.cmbExecutionTime.Location = new System.Drawing.Point(120, 70); // Example location
        this.cmbExecutionTime.Size = new System.Drawing.Size(120, 20); // Example size
        this.cmbExecutionTime.DropDownStyle = ComboBoxStyle.DropDownList; // Make the combo box read-only
        // Add some example times to the combo box

        // Add times to the combo box in 15 minute intervals
        for (int hour = 0; hour <= 23; hour++)
        {
            for (int minute = 0; minute <= 45; minute += 15)
            {
                string time = $"{hour:D2}:{minute:D2}";
                this.cmbExecutionTime.Items.Add(time);
            }
        }

        // Initialize the save button
        this.btnSave = new System.Windows.Forms.Button();
        this.btnSave.Location = new System.Drawing.Point(10, 100); // Example location
        this.btnSave.Size = new System.Drawing.Size(75, 23); // Example size
        this.btnSave.Text = "Save";
        this.btnSave.UseVisualStyleBackColor = true;
        this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

        // Add the controls to the form's controls
        this.Controls.Add(this.lblInputFile);
        this.Controls.Add(this.txtInputFile);
        this.Controls.Add(this.lblOutputFile);
        this.Controls.Add(this.txtOutputFile);
        this.Controls.Add(this.lblExecutionTime);
        this.Controls.Add(this.cmbExecutionTime);
        this.Controls.Add(this.btnSave);
    }

    // ... other methods ...

    private void btnSave_Click(object sender, EventArgs e)
    {
        RegConfig.updateValues(this.txtInputFile.Text, this.txtOutputFile.Text, this.cmbExecutionTime.SelectedItem.ToString());
        string[] newValues = RegConfig.getValues();
        this.txtInputFile.Text = newValues[0];
        this.txtOutputFile.Text = newValues[1];
        this.cmbExecutionTime.SelectedItem = newValues[2];

        MessageBox.Show("Die Werte wurden erfolgreich gespeichert", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
