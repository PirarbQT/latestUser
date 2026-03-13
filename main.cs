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
            Text = "เวลาเข้าจอด (HH:mm)"
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
            Text = "เวลาออก (HH:mm)"
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
            Text = "ค่าจอดรถต่อชั่วโมง (บาท)"
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
            Text = "คำนวณ"
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
            ShowValidationError("กรุณากรอกเวลาเข้าจอดในรูปแบบ HH:mm", entryTimeTextBox);
            return;
        }

        if (!TryParseTime(exitTimeTextBox.Text, out TimeSpan exitTime))
        {
            ShowValidationError("กรุณากรอกเวลาออกในรูปแบบ HH:mm", exitTimeTextBox);
            return;
        }

        if (!TryParseDecimal(hourlyRateTextBox.Text, out decimal hourlyRate) || hourlyRate < 0)
        {
            ShowValidationError("กรุณากรอกค่าจอดรถต่อชั่วโมงเป็นตัวเลขที่มากกว่าหรือเท่ากับ 0", hourlyRateTextBox);
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
            $"เวลาเข้าจอด: {entryTime:hh\\:mm}{Environment.NewLine}" +
            $"เวลาออก: {exitTime:hh\\:mm}{Environment.NewLine}" +
            $"ระยะเวลาจอดจริง: {FormatDuration(parkingDuration)}{Environment.NewLine}" +
            $"ชั่วโมงที่คิดเงิน: {billedHours:N0} ชั่วโมง{Environment.NewLine}" +
            $"อัตราค่าจอด: {hourlyRate:N2} บาท/ชั่วโมง{Environment.NewLine}{Environment.NewLine}" +
            $"ค่าที่จอดรถรวม: {totalFee:N2} บาท";
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
        resultTextBox.Text = "กรอกเวลาเข้า เวลาออก และค่าจอดรถต่อชั่วโมง แล้วกดปุ่ม คำนวณ";
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
        return $"{hours} ชั่วโมง {minutes} นาที";
    }

    private void ShowValidationError(string message, Control target)
    {
        MessageBox.Show(this, message, "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Error);
        target.Focus();
        if (target is TextBox textBox)
        {
            textBox.SelectAll();
        }
    }
}
