
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace WeatherAlertSystem
{
    public partial class Form1 : Form
    {
        private TextBox txtName = new TextBox();
        private TextBox txtPhone = new TextBox();
        private ComboBox cmbArea = new ComboBox();
        private DataGridView grid = new DataGridView();
        private TextBox txtWarning = new TextBox();
        private Label lblStatus = new Label();
        private CheckBox chkConsent = new CheckBox();

        public Form1()
        {
            Database.Initialize();
                this.Shown += async (sender, e) =>
                {
                await UpdateService.CheckForUpdatesAsync();
                };
                
            BuildForm();
        }

        private void BuildForm()
        {
            Text = "Weather Warning and Storm Signal SMS Alert System";
            Size = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.WhiteSmoke;

            Label title = new Label
            {
                Text = "WEATHER WARNING & SMS ALERT SYSTEM",
                Font = new Font("Arial", 17, FontStyle.Bold),
                Location = new Point(20, 15),
                Size = new Size(900, 35)
            };
            Controls.Add(title);

            AddLabel("Subscriber Name:", 25, 75);
            txtName.SetBounds(170, 72, 220, 28);
            Controls.Add(txtName);

            AddLabel("Mobile Number:", 25, 115);
            txtPhone.SetBounds(170, 112, 220, 28);
            txtPhone.PlaceholderText = "+639XXXXXXXXX";
            Controls.Add(txtPhone);

            AddLabel("Area:", 25, 155);
            cmbArea.SetBounds(170, 152, 220, 28);
            cmbArea.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbArea.Items.AddRange(new object[]
            {
                "Select Area",
                "NCR",
                "Region 4",
                "Region 3"
            });
            cmbArea.SelectedIndex = 0;
            Controls.Add(cmbArea);

            Button btnRegister = new Button
            {
                Text = "Register Subscriber",
                Location = new Point(170, 195),
                Size = new Size(220, 35),
                BackColor = Color.LightBlue
            };

            chkConsent.Text =
            "I have permission to register this person for weather SMS alerts.";
             chkConsent.Location = new Point(170, 235);
            chkConsent.Size = new Size(450, 40);
            chkConsent.Checked = false;
            Controls.Add(chkConsent);
            btnRegister.Click += RegisterSubscriber;
            Controls.Add(btnRegister);
        

            Label subscriberTitle = new Label
            {
                Text = "REGISTERED SUBSCRIBERS (CURRENT SESSION)",
                Location = new Point(25, 275),
                Size = new Size(500, 25),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            Controls.Add(subscriberTitle);

            grid.Location = new Point(25, 300);
            grid.Size = new Size(600, 250);
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            grid.Columns.Add("Name", "Name");
            grid.Columns.Add("Phone", "Mobile Number");
            grid.Columns.Add("Area", "Area");
            foreach (var subscriber in Database.GetSubscribers())
            {
             grid.Rows.Add(
             subscriber.Name,
             subscriber.Phone,
             subscriber.Area);
            }
            Controls.Add(grid);

            Label warningTitle = new Label
            {
                Text = "WEATHER WARNING MESSAGE",
                Location = new Point(650, 75),
                Size = new Size(300, 25),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            Controls.Add(warningTitle);

            txtWarning.Multiline = true;
            txtWarning.ScrollBars = ScrollBars.Vertical;
            txtWarning.SetBounds(650, 110, 300, 200);
            txtWarning.Text =
                "WEATHER WARNING\r\n\r\n" +
                "Official bulletin: [Enter verified details]\r\n" +
                "Area: [Enter affected area]\r\n" +
                "Signal: [Verify official PAGASA bulletin]\r\n\r\n" +
                "Please follow official safety instructions.";
            Controls.Add(txtWarning);

            Button btnPreview = new Button
            {
                Text = "Preview Warning",
                Location = new Point(650, 325),
                Size = new Size(140, 35)
            };
            btnPreview.Click += (s, e) =>
            {
                MessageBox.Show(
                    txtWarning.Text,
                    "Warning Preview",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };
            Controls.Add(btnPreview);

          
        Button btnSend = new Button
        {
        Text = "Simulate SMS",
        Location = new Point(800, 325),
        Size = new Size(150, 35),
        BackColor = Color.LightBlue
        };

        Button btnSmsHistory = new Button
        {
        Text = "View SMS History",
        Location = new Point(650, 380),
        Size = new Size(150, 35)
        };

        btnSmsHistory.Click += (sender, e) => ShowSmsHistory();
        Controls.Add(btnSmsHistory);

        
btnSend.Click += async (s, e) =>
{
    if (grid.Rows.Count == 0)
    {
        MessageBox.Show(
            "Register a test subscriber first.",
            "No Subscribers",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
        return;
    }

    if (string.IsNullOrWhiteSpace(txtWarning.Text))
    {
        MessageBox.Show("Enter a warning message first.");
        return;
    }

    var confirm = MessageBox.Show(
        "This is a simulation only.\n" +
        "No real SMS will be sent.\n\n" +
        "Simulate a message for the first subscriber?",
        "Confirm SMS Simulation",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

    if (confirm != DialogResult.Yes)
        return;

    string recipient =
        Convert.ToString(grid.Rows[0].Cells["Phone"].Value) ?? "";

    try
    {
        string result = await SmsService.SendAsync(
            recipient, txtWarning.Text);

        Database.AddSmsLog(
            recipient,
            txtWarning.Text,
            "SIMULATED - NOT SENT");

        MessageBox.Show(
            result,
            "SMS Simulation Result",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show(
            ex.Message,
            "Simulation Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
};

Controls.Add(btnSend);


lblStatus.Text =
    "Status: Demo mode — subscriber data is saved in MySQL.";
lblStatus.Location = new Point(25, 570);
lblStatus.Size = new Size(900, 30);
lblStatus.ForeColor = Color.DarkBlue;
Controls.Add(lblStatus);
}


        private void AddLabel(string text, int x, int y)
        {
            Controls.Add(new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(140, 25)
            });
        }

        private void RegisterSubscriber(object? sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(phone) ||
                cmbArea.SelectedIndex <= 0)
            {
                MessageBox.Show(
                    "Please enter the name, mobile number, and area.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            
            if (!chkConsent.Checked)
                {
                 MessageBox.Show(
                "Please confirm permission before registering for SMS alerts.",
                 "Consent Required",
                MessageBoxButtons.OK,
                 MessageBoxIcon.Warning);
                 return;
            }

            try
        {
        Database.AddSubscriber(name, phone, cmbArea.Text);

         grid.Rows.Insert(0, name, phone, cmbArea.Text);
        }
        catch (Exception ex)
        {
         MessageBox.Show(
        "Could not save subscriber: " + ex.Message);
        return;
}

            txtName.Clear();
            txtPhone.Clear();
            cmbArea.SelectedIndex = 0;

            MessageBox.Show(
                "Subscriber added to the current session.",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ShowSmsHistory()
{
    var logs = Database.GetSmsLogs();

    string history = "";

    foreach (var log in logs)
    {
        history +=
            "Recipient: " + log.Recipient + "\n" +
            "Message: " + log.Message + "\n" +
            "Status: " + log.Status + "\n" +
            "Date: " + log.CreatedAt + "\n" +
            "------------------------------\n";
    }

    if (string.IsNullOrWhiteSpace(history))
        history = "No SMS history found.";

    MessageBox.Show(
        history,
        "SMS History",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information);
}
    }
}
