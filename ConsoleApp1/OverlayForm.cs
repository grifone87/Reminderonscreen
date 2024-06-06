using ImageOverlayApp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class OverlayForm : Form
{
    private List<(Rectangle rectangle, string message, string textPosition, bool isFlagged)> _rectangles;
    private Form1 _parentForm;

    public OverlayForm(Form1 parentForm)
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.Manual;
        this.ShowInTaskbar = false;
        this.TopMost = true;
        this.BackColor = Color.LimeGreen;
        this.TransparencyKey = Color.LimeGreen;
        this.Opacity = 0.5;
        this.DoubleBuffered = true;
        this.WindowState = FormWindowState.Maximized;
        _parentForm = parentForm;
    }

    public void UpdateRectangles((Rectangle rectangle, string message, string textPosition, bool isFlagged)[] newRectangles)
    {
        _rectangles = newRectangles.ToList();
        this.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (_rectangles != null)
        {
            foreach (var (rectangle, message, textPosition, isFlagged) in _rectangles)
            {
                e.Graphics.DrawRectangle(Pens.Red, rectangle);
                DrawText(e.Graphics, message, rectangle, textPosition);
            }
        }
    }

    private void DrawText(Graphics g, string message, Rectangle rectangle, string textPosition)
    {
        if (string.IsNullOrEmpty(message))
            return;

        using (Font font = new Font("Arial", 11, FontStyle.Bold))
        {
            SizeF textSize = g.MeasureString(message, font);
            PointF textLocation = GetTextLocation(rectangle, textSize, textPosition);

            using (SolidBrush backgroundBrush = new SolidBrush(Color.Orange)) // Set background color
            {
                g.FillRectangle(backgroundBrush, new RectangleF(textLocation, textSize)); // Fill background
            }

            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                g.DrawString(message, font, textBrush, textLocation);
            }
        }
    }

    private PointF GetTextLocation(Rectangle rectangle, SizeF textSize, string textPosition)
    {
        PointF location = rectangle.Location;

        switch (textPosition.ToLower())
        {
            case "above":
                location = new PointF(rectangle.Left + (rectangle.Width - textSize.Width) / 2, rectangle.Top - textSize.Height);
                break;
            case "below":
                location = new PointF(rectangle.Left + (rectangle.Width - textSize.Width) / 2, rectangle.Bottom);
                break;
            case "left":
                location = new PointF(rectangle.Left - textSize.Width, rectangle.Top + (rectangle.Height - textSize.Height) / 2);
                break;
            case "right":
                location = new PointF(rectangle.Right, rectangle.Top + (rectangle.Height - textSize.Height) / 2);
                break;
            default:
                location = new PointF(rectangle.Left, rectangle.Top);
                break;
        }

        return location;
    }
}
