using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using OpenCvSharp;
using DrawingPoint = System.Drawing.Point; // Alias per System.Drawing.Point
using DrawingSize = System.Drawing.Size; // Alias per System.Drawing.Size

namespace ImageOverlayApp
{
    public partial class Form1 : Form
    {
        private Dictionary<string, Bitmap> _referenceImages;
        private List<(Rectangle rectangle, string message, string textPosition, bool isFlagged)> _rectangles;
        private System.Windows.Forms.Timer _timer;
        private OverlayForm _overlayForm;
        private List<ImageConfig> imageConfigs;
        private const string WindowTitle = "WinPer 11.0"; // Cambia questo con il titolo della tua finestra

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized; // Riduce a icona la finestra principale
            this.Load += (sender, e) => { this.Hide(); }; // Nasconde la finestra principale
            _rectangles = new List<(Rectangle rectangle, string message, string textPosition, bool isFlagged)>();
            InitializeOverlay();
            LoadReferenceImages();
        }

        private void InitializeOverlay()
        {
            try
            {
                _overlayForm = new OverlayForm(this); // Pass the parent form
                _overlayForm.Show();

                _timer = new System.Windows.Forms.Timer();
                _timer.Interval = 1000; // Controlla ogni secondo
                _timer.Tick += OnTimerTick;
                _timer.Start();
                Console.WriteLine("Overlay initialized.");
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void LoadReferenceImages()
        {
            try
            {
                _referenceImages = new Dictionary<string, Bitmap>();

                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string imagesPath = Path.Combine(basePath, "images");
                string configPath = Path.Combine(basePath, "imagesConfig.json");

                if (!File.Exists(configPath))
                {
                    MessageBox.Show($"Configuration file not found: {configPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                imageConfigs = JsonConvert.DeserializeObject<List<ImageConfig>>(File.ReadAllText(configPath));

                foreach (var config in imageConfigs)
                {
                    string imagePath = Path.Combine(imagesPath, config.ImagePath);
                    if (File.Exists(imagePath))
                    {
                        _referenceImages[config.Name] = new Bitmap(imagePath);
                    }
                    else
                    {
                        MessageBox.Show($"File not found: {imagePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                Console.WriteLine("Images loaded successfully.");
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            try
            {
                CheckImages();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void CheckImages()
        {
            _rectangles.Clear();

            foreach (var referenceImage in _referenceImages)
            {
                var template = OpenCvSharp.Extensions.BitmapConverter.ToMat(referenceImage.Value);
                Cv2.CvtColor(template, template, ColorConversionCodes.BGR2GRAY);

                var screenCapture = CaptureScreen();
                if (screenCapture == null)
                {
                    continue;
                }
                Cv2.CvtColor(screenCapture, screenCapture, ColorConversionCodes.BGR2GRAY);

                using (var result = new Mat())
                {
                    Cv2.MatchTemplate(screenCapture, template, result, TemplateMatchModes.CCoeffNormed);
                    result.MinMaxLoc(out _, out OpenCvSharp.Point maxLoc);

                    if (result.At<float>(maxLoc.Y, maxLoc.X) >= 0.8)
                    {
                        var config = imageConfigs.FirstOrDefault(c => c.Name == referenceImage.Key);
                        if (config != null)
                        {
                            _rectangles.Add((new Rectangle(new DrawingPoint(maxLoc.X, maxLoc.Y), new DrawingSize(template.Width, template.Height)), config.Message, config.TextPosition, config.IsFlagged));
                            config.IsFlagged = true; // Imposta il flag
                        }
                    }
                }
            }

            _overlayForm.UpdateRectangles(_rectangles.ToArray());
        }

        private Mat CaptureScreen()
        {
            try
            {
                var screenBitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                using (var g = Graphics.FromImage(screenBitmap))
                {
                    g.CopyFromScreen(0, 0, 0, 0, screenBitmap.Size);
                }
                return OpenCvSharp.Extensions.BitmapConverter.ToMat(screenBitmap);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }

        static void LogException(Exception ex)
        {
            MessageBox.Show($"Unhandled exception: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public class ImageConfig
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Message { get; set; }
        public string TextPosition { get; set; }
        public bool IsFlagged { get; set; } // Aggiunto flag
    }
}
