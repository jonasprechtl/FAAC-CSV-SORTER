

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
    private System.Windows.Forms.CheckBox chkUseAuth;
    private System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.Label lblUsername;
    private System.Windows.Forms.Label lblPassword;
    private System.Windows.Forms.Button btnManualRun;

    // ... other member variables and methods ...

    private void InitializeComponent()
    {

        this.fileBrowserDialog = new System.Windows.Forms.OpenFileDialog();


        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1030, 300);

        this.Text = "FAAC - Kennzeichenkorrektur";

        // Initialize input directory label and text box
        this.lblInputFile = new System.Windows.Forms.Label();
        this.lblInputFile.Text = "Eingabeordner";
        this.lblInputFile.Location = new System.Drawing.Point(10, 20); // Example location
        this.lblInputFile.Anchor = System.Windows.Forms.AnchorStyles.Left; // Example anchor
        this.lblInputFile.AutoSize = true;


        this.txtInputFile = new System.Windows.Forms.TextBox();
        this.txtInputFile.Location = new System.Drawing.Point(210, 20); // Example location
        this.txtInputFile.Size = new System.Drawing.Size(770, 20); // Example size

        // Initialize the input directory select button
        this.btnInputFile = new System.Windows.Forms.Button();
        this.btnInputFile.Location = new System.Drawing.Point(990, 20); // Example location
        this.btnInputFile.Size = new System.Drawing.Size(30, 20); // Example size
        this.btnInputFile.Text = "...";
        this.btnInputFile.UseVisualStyleBackColor = true;
        this.btnInputFile.Click += new System.EventHandler(this.btnInputFile_Click);


        // Initialize output file label and text box
        this.lblOutputFile = new System.Windows.Forms.Label();
        this.lblOutputFile.Text = "Ausgabedatei";
        this.lblOutputFile.Location = new System.Drawing.Point(10, 70); // Example location
        this.lblOutputFile.AutoSize = true;


        this.txtOutputFile = new System.Windows.Forms.TextBox();
        this.txtOutputFile.Location = new System.Drawing.Point(210, 70); // Example location
        this.txtOutputFile.Size = new System.Drawing.Size(770, 20); // Example size

        // Initialize the output directory select button
        this.btnOutputFile = new System.Windows.Forms.Button();
        this.btnOutputFile.Location = new System.Drawing.Point(990, 70); // Example location
        this.btnOutputFile.Size = new System.Drawing.Size(30, 20); // Example size
        this.btnOutputFile.Text = "...";
        this.btnOutputFile.UseVisualStyleBackColor = true;
        this.btnOutputFile.Click += new System.EventHandler(this.btnOutputFile_Click);


        // Initialize execution time label and combo box
        this.lblExecutionTime = new System.Windows.Forms.Label();
        this.lblExecutionTime.Text = "Ausführung um";
        this.lblExecutionTime.Location = new System.Drawing.Point(10, 120); // Example location
        this.lblExecutionTime.AutoSize = true;


        this.cmbExecutionTime = new System.Windows.Forms.ComboBox();
        this.cmbExecutionTime.Location = new System.Drawing.Point(210, 120); // Example location
        this.cmbExecutionTime.Size = new System.Drawing.Size(210, 20); // Example size
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
        
        //Button for manual run
        this.btnManualRun = new System.Windows.Forms.Button();
        this.btnManualRun.Location = new System.Drawing.Point(440, 120); // Example location
        this.btnManualRun.Size = new System.Drawing.Size(150, 24); // Example size
        this.btnManualRun.Text = "Manuellen Lauf starten";
        this.btnManualRun.Click += new System.EventHandler(this.btnManualRun_Click);


        //Checkbox for USEAUTH
        this.chkUseAuth = new System.Windows.Forms.CheckBox();
        this.chkUseAuth.Text = "Authentifizierung verwenden";
        this.chkUseAuth.Location = new System.Drawing.Point(10, 160); // Example location
        this.chkUseAuth.AutoSize = true;
        this.chkUseAuth.CheckedChanged += new System.EventHandler(this.chkUseAuth_CheckedChanged);

        //Input for PASSWORD
        this.lblUsername = new System.Windows.Forms.Label();
        this.lblUsername.Text = "Benutzername";
        this.lblUsername.Location = new System.Drawing.Point(10, 190); // Example location
        this.lblUsername.AutoSize = true;
        if(this.AuthEnabledOnLoad){
            lblUsername.Text = "Benutzername (Leer lassen um nicht zu ändern)";
        }

        this.txtUsername = new System.Windows.Forms.TextBox();
        this.txtUsername.Location = new System.Drawing.Point(10, 210); // Example location
        this.txtUsername.Size = new System.Drawing.Size(300, 20); // Example size
        
        //Input for USERNAME
        this.lblPassword = new System.Windows.Forms.Label();
        this.lblPassword.Text = "Passwort";
        this.lblPassword.Location = new System.Drawing.Point(340, 190); // Example location
        this.lblPassword.AutoSize = true;
        if(this.AuthEnabledOnLoad){
            lblPassword.Text = "Passwort (Leer lassen um nicht zu ändern)";
        }

        this.txtPassword = new System.Windows.Forms.TextBox();
        this.txtPassword.Location = new System.Drawing.Point(340, 210); // Example location
        this.txtPassword.Size = new System.Drawing.Size(300, 20); // Example size
        this.txtPassword.PasswordChar = '*'; // Set the password character



        // Initialize the save button
        this.btnSave = new System.Windows.Forms.Button();
        this.btnSave.Location = new System.Drawing.Point(10, 250); // Example location
        this.btnSave.Size = new System.Drawing.Size(150, 40); // Example size
        this.btnSave.Text = "Speichern";
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
        this.Controls.Add(this.btnInputFile);
        this.Controls.Add(this.btnOutputFile);
        this.Controls.Add(this.chkUseAuth);
        this.Controls.Add(this.txtUsername);
        this.Controls.Add(this.txtPassword);
        this.Controls.Add(this.lblUsername);
        this.Controls.Add(this.lblPassword);
        this.Controls.Add(this.btnManualRun);
    }

    // ... other methods ...

    private void btnSave_Click(object sender, EventArgs e)
    {

        //If the user has checked the checkbox and password and username are empty, do not change the password (password stays the same)

        //if only one of the two fields is empty, show an error message
       
        if (this.chkUseAuth.Checked && !this.AuthEnabledOnLoad && (this.txtUsername.Text == "" || this.txtPassword.Text == ""))
        {
            MessageBox.Show("Zum Aktivieren der Authentifizierung geben sie bitte den Benutzernamen und das Passwort ein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

         if (this.chkUseAuth.Checked && (this.txtUsername.Text == "" && this.txtPassword.Text != "" || this.txtUsername.Text != "" && this.txtPassword.Text == ""))
        {
            MessageBox.Show("Zum Ändern der Anmeldedaten geben Sie bitte den Benutzernamen UND das Passwort ein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if(this.chkUseAuth.Checked && this.txtUsername.Text != "" && this.txtPassword.Text != ""){
            RegConfig.updateValues(this.txtInputFile.Text, this.txtOutputFile.Text, this.cmbExecutionTime.SelectedItem.ToString(), this.chkUseAuth.Checked ? 1 : 0, this.txtUsername.Text, this.txtPassword.Text);
        } else {
            RegConfig.updateValues(this.txtInputFile.Text, this.txtOutputFile.Text, this.cmbExecutionTime.SelectedItem.ToString(), this.chkUseAuth.Checked ? 1 : 0);
        }
        string[] newValues = RegConfig.getValues();
        this.txtInputFile.Text = newValues[0];
        this.txtOutputFile.Text = newValues[1];
        this.cmbExecutionTime.SelectedItem = newValues[2];
        this.chkUseAuth.Checked = newValues[3] == "1";

        //After saving, the AuthEnabledOnLoad variable has to be updated 
        //(the user does not have to enter the password and username again if it was already set on last update)
        //(The User also does have to enter the password and username again if the Authentication was disabled on last update)
        this.AuthEnabledOnLoad = newValues[3] == "1";
        setPWDandUSRLabels();
        clearPWDandUSR();

        MessageBox.Show("Die Werte wurden erfolgreich gespeichert", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }


    private System.Windows.Forms.Button btnInputFile;
    private System.Windows.Forms.Button btnOutputFile;
    private System.Windows.Forms.OpenFileDialog fileBrowserDialog;

    private void btnInputFile_Click(object sender, EventArgs e)
    {
        if (fileBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            this.txtInputFile.Text = fileBrowserDialog.FileName;
        }
    }

    private void btnOutputFile_Click(object sender, EventArgs e)
    {
        if (fileBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            this.txtOutputFile.Text = fileBrowserDialog.FileName;
        }
    }

    //This enables or disables the username and password input fields depending on the checkbox
    private void chkUseAuth_CheckedChanged(object sender, EventArgs e)
    {
        this.txtUsername.Enabled = this.chkUseAuth.Checked;
        this.txtPassword.Enabled = this.chkUseAuth.Checked;
    }

    private void setPWDandUSRLabels(){
        if(this.AuthEnabledOnLoad){
            lblUsername.Text = "Benutzername (Leer lassen um nicht zu ändern)";
            lblPassword.Text = "Passwort (Leer lassen um nicht zu ändern)";
        } else {
            lblUsername.Text = "Benutzername";
            lblPassword.Text = "Passwort";
        }
    }

    private void clearPWDandUSR(){
        this.txtUsername.Text = "";
        this.txtPassword.Text = "";
    }

    private void btnManualRun_Click(object sender, EventArgs e)
    {
        RegConfig.initiateManualRun();
        MessageBox.Show("Der manuelle Lauf wurde gestartet", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

}


