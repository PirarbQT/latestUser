using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ParkingFee;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new ParkingFeeForm());
    }
}

internal sealed class ParkingFeeForm : Form
{
    private readonly TextBox entryTimeTextBox;
    private readonly TextBox exitTimeTextBox;
    private readonly TextBox hourlyRateTextBox;
    private readonly TextBox resultTextBox;

    internal ParkingFeeForm()
    {
        Font uiFont = new("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);

        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.Control;
        ClientSize = new Size(680, 420);
        Font = uiFont;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Parking Fee Calculator";

        Label entryTimeLabel = new()
        {
            AutoSize = true,
            Location = new Point(24, 32),
            Text = "Entry Time (HH:mm)"
        };

        entryTimeTextBox = new TextBox
        {
            Location = new Point(240, 28),
            Size = new Size(300, 32)
        };

        Label exitTimeLabel = new()
        {
            AutoSize = true,
            Location = new Point(24, 82),
            Text = "Exit Time (HH:mm)"
        };

        exitTimeTextBox = new TextBox
        {
            Location = new Point(240, 78),
            Size = new Size(300, 32)
        };

        Label hourlyRateLabel = new()
        {
            AutoSize = true,
            Location = new Point(24, 132),
            Text = "Hourly Rate (THB)"
        };

        hourlyRateTextBox = new TextBox
        {
            Location = new Point(240, 128),
            Size = new Size(300, 32)
        };

        resultTextBox = new TextBox
        {
            Location = new Point(24, 190),
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Size = new Size(628, 150)
        };

        Button calculateButton = new()
        {
            Location = new Point(190, 360),
            Size = new Size(120, 38),
            Text = "Calculate"
        };
        calculateButton.Click += CalculateButton_Click;

        Button resetButton = new()
        {
            Location = new Point(340, 360),
            Size = new Size(120, 38),
            Text = "Reset"
        };
        resetButton.Click += ResetButton_Click;

        Controls.Add(entryTimeLabel);
        Controls.Add(entryTimeTextBox);
        Controls.Add(exitTimeLabel);
        Controls.Add(exitTimeTextBox);
        Controls.Add(hourlyRateLabel);
        Controls.Add(hourlyRateTextBox);
        Controls.Add(resultTextBox);
        Controls.Add(calculateButton);
        Controls.Add(resetButton);

        AcceptButton = calculateButton;
        CancelButton = resetButton;

        ResetFields();
    }

    private void CalculateButton_Click(object? sender, EventArgs e)
    {
        if (!TryParseTime(entryTimeTextBox.Text, out TimeSpan entryTime))
        {
            ShowValidationError("Please enter the entry time in HH:mm format.", entryTimeTextBox);
            return;
        }

        if (!TryParseTime(exitTimeTextBox.Text, out TimeSpan exitTime))
        {
            ShowValidationError("Please enter the exit time in HH:mm format.", exitTimeTextBox);
            return;
        }

        if (!TryParseDecimal(hourlyRateTextBox.Text, out decimal hourlyRate) || hourlyRate < 0)
        {
            ShowValidationError("Please enter a valid hourly rate greater than or equal to 0.", hourlyRateTextBox);
            return;
        }

        DateTime entryDateTime = DateTime.Today.Add(entryTime);
        DateTime exitDateTime = DateTime.Today.Add(exitTime);
        if (exitDateTime <= entryDateTime)
        {
            exitDateTime = exitDateTime.AddDays(1);
        }

        TimeSpan parkingDuration = exitDateTime - entryDateTime;
        decimal billedHours = Math.Ceiling((decimal)parkingDuration.TotalHours);
        decimal totalFee = billedHours * hourlyRate;

        resultTextBox.Text =
            $"Entry Time: {entryTime:hh\\:mm}{Environment.NewLine}" +
            $"Exit Time: {exitTime:hh\\:mm}{Environment.NewLine}" +
            $"Actual Duration: {FormatDuration(parkingDuration)}{Environment.NewLine}" +
            $"Billed Hours: {billedHours:N0} {FormatUnit((int)billedHours, "hour", "hours")}{Environment.NewLine}" +
            $"Hourly Rate: {hourlyRate:N2} THB/hour{Environment.NewLine}{Environment.NewLine}" +
            $"Total Parking Fee: {totalFee:N2} THB";
    }

    private void ResetButton_Click(object? sender, EventArgs e)
    {
        ResetFields();
    }

    private void ResetFields()
    {
        entryTimeTextBox.Text = string.Empty;
        exitTimeTextBox.Text = string.Empty;
        hourlyRateTextBox.Text = string.Empty;
        resultTextBox.Text = "Enter the entry time, exit time, and hourly rate, then click Calculate.";
        entryTimeTextBox.Focus();
    }

    private static bool TryParseDecimal(string? value, out decimal result)
    {
        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out result) ||
               decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
    }

    private static bool TryParseTime(string? value, out TimeSpan result)
    {
        return TimeSpan.TryParseExact(value, ["h\\:mm", "hh\\:mm"], CultureInfo.InvariantCulture, out result);
    }

    private static string FormatDuration(TimeSpan duration)
    {
        int hours = (int)duration.TotalHours;
        int minutes = duration.Minutes;
        return $"{hours} {FormatUnit(hours, "hour", "hours")} {minutes} {FormatUnit(minutes, "minute", "minutes")}";
    }

    private static string FormatUnit(int value, string singular, string plural)
    {
        return value == 1 ? singular : plural;
    }

    private void ShowValidationError(string message, Control target)
    {
        MessageBox.Show(this, message, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
        target.Focus();
        if (target is TextBox textBox)
        {
            textBox.SelectAll();
        }
    }
}
