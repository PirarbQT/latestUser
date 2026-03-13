using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace FuelCost;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FuelCostForm());
    }
}

internal sealed class FuelCostForm : Form
{
    private readonly TextBox distanceTextBox;
    private readonly TextBox efficiencyTextBox;
    private readonly TextBox pricePerLiterTextBox;
    private readonly TextBox resultTextBox;

    internal FuelCostForm()
    {
        Font uiFont = new("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);

        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.Control;
        ClientSize = new Size(680, 420);
        Font = uiFont;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Fuel Cost Calculator";

        Label distanceLabel = new()
        {
            AutoSize = true,
            Location = new Point(24, 32),
            Text = "ระยะทาง (กม.)"
        };

        distanceTextBox = new TextBox
        {
            Location = new Point(240, 28),
            Size = new Size(300, 32)
        };

        Label efficiencyLabel = new()
        {
            AutoSize = true,
            Location = new Point(24, 82),
            Text = "อัตราสิ้นเปลือง (กม./ลิตร)"
        };

        efficiencyTextBox = new TextBox
        {
            Location = new Point(240, 78),
            Size = new Size(300, 32)
        };

        Label priceLabel = new()
        {
            AutoSize = true,
            Location = new Point(24, 132),
            Text = "ราคาน้ำมัน (บาท/ลิตร)"
        };

        pricePerLiterTextBox = new TextBox
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

        Controls.Add(distanceLabel);
        Controls.Add(distanceTextBox);
        Controls.Add(efficiencyLabel);
        Controls.Add(efficiencyTextBox);
        Controls.Add(priceLabel);
        Controls.Add(pricePerLiterTextBox);
        Controls.Add(resultTextBox);
        Controls.Add(calculateButton);
        Controls.Add(resetButton);

        AcceptButton = calculateButton;
        CancelButton = resetButton;

        ResetFields();
    }

    private void CalculateButton_Click(object? sender, EventArgs e)
    {
        if (!TryParseDecimal(distanceTextBox.Text, out decimal distanceKm) || distanceKm <= 0)
        {
            ShowValidationError("กรุณากรอกระยะทางเป็นตัวเลขที่มากกว่า 0", distanceTextBox);
            return;
        }

        if (!TryParseDecimal(efficiencyTextBox.Text, out decimal kmPerLiter) || kmPerLiter <= 0)
        {
            ShowValidationError("กรุณากรอกอัตราสิ้นเปลืองเป็นตัวเลขที่มากกว่า 0", efficiencyTextBox);
            return;
        }

        if (!TryParseDecimal(pricePerLiterTextBox.Text, out decimal pricePerLiter) || pricePerLiter < 0)
        {
            ShowValidationError("กรุณากรอกราคาน้ำมันเป็นตัวเลขที่มากกว่าหรือเท่ากับ 0", pricePerLiterTextBox);
            return;
        }

        decimal litersUsed = distanceKm / kmPerLiter;
        decimal totalCost = litersUsed * pricePerLiter;

        resultTextBox.Text =
            $"ระยะทาง: {distanceKm:N2} กม.{Environment.NewLine}" +
            $"อัตราสิ้นเปลือง: {kmPerLiter:N2} กม./ลิตร{Environment.NewLine}" +
            $"ราคาน้ำมัน: {pricePerLiter:N2} บาท/ลิตร{Environment.NewLine}{Environment.NewLine}" +
            $"ใช้น้ำมัน: {litersUsed:N2} ลิตร{Environment.NewLine}" +
            $"ค่าน้ำมันรวม: {totalCost:N2} บาท";
    }

    private void ResetButton_Click(object? sender, EventArgs e)
    {
        ResetFields();
    }

    private void ResetFields()
    {
        distanceTextBox.Text = string.Empty;
        efficiencyTextBox.Text = string.Empty;
        pricePerLiterTextBox.Text = string.Empty;
        resultTextBox.Text = "กรอกระยะทาง อัตราสิ้นเปลือง และราคาน้ำมัน แล้วกดปุ่ม คำนวณ";
        distanceTextBox.Focus();
    }

    private static bool TryParseDecimal(string? value, out decimal result)
    {
        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out result) ||
               decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
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
