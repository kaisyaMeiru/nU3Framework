using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace nU3.Core.UI.Components.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(SignatureControl))]
    public partial class SignatureControl : XtraUserControl
    {
        private PanelControl? _signaturePanel;
        private SimpleButton? _clearButton;
        private SimpleButton? _saveButton;
        private SimpleButton? _undoButton;
        private CheckEdit? _confirmCheckEdit;
        private LabelControl? _statusLabel;
        private MemoEdit? _commentEdit;
        private PanelControl? _buttonPanel;
        private PanelControl? _commentPanel;
        private LabelControl? _commentLabel;

        private Bitmap? _signatureBitmap;
        private Graphics? _signatureGraphics;
        private Point _lastPoint;
        private bool _isDrawing;
        private bool _hasSignature;
        private List<Point>? _currentStroke;
        private List<List<Point>> _strokes = new();

        public event EventHandler<SignatureEventArgs>? SignatureSigned;
        public event EventHandler? SignatureCleared;

        [Category("Data")]
        public byte[]? SignatureData
        {
            get => GetSignatureData();
            set => LoadSignatureData(value);
        }

        [Category("Data")]
        public bool IsSigned => _hasSignature && (_confirmCheckEdit?.Checked ?? false);

        [Category("Data")]
        public string? Comment
        {
            get => _commentEdit?.Text;
            set
            {
                if (_commentEdit != null)
                {
                    _commentEdit.Text = value ?? "";
                }
            }
        }

        [Category("Behavior")]
        public bool RequireConfirmation { get; set; } = true;

        [Category("Behavior")]
        public bool AllowComment { get; set; } = true;

        [Category("Behavior")]
        public bool AllowUndo { get; set; } = true;

        [Category("Appearance")]
        public Color PenColor { get; set; } = Color.Black;

        [Category("Appearance")]
        public int PenWidth { get; set; } = 2;

        [Category("Appearance")]
        public string? SignerName { get; set; }

        public SignatureControl()
        {
            InitializeComponent();
            InitializeSignatureCanvas();
            AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            if (_signaturePanel != null)
            {
                _signaturePanel.Paint += OnSignaturePanelPaint;
                _signaturePanel.MouseDown += OnSignaturePanelMouseDown;
                _signaturePanel.MouseMove += OnSignaturePanelMouseMove;
                _signaturePanel.MouseUp += OnSignaturePanelMouseUp;
                _signaturePanel.MouseLeave += OnSignaturePanelMouseLeave;
            }

            if (_clearButton != null)
            {
                _clearButton.Click += OnClear;
            }

            if (_undoButton != null)
            {
                _undoButton.Click += OnUndo;
            }

            if (_saveButton != null)
            {
                _saveButton.Click += OnSave;
            }
        }

        private void InitializeSignatureCanvas()
        {
            if (_signaturePanel == null) return;

            _signatureBitmap = new Bitmap(_signaturePanel.Width, _signaturePanel.Height);
            _signatureGraphics = Graphics.FromImage(_signatureBitmap);
            _signatureGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            ClearCanvas();
        }

        private void ClearCanvas()
        {
            if (_signatureGraphics != null)
            {
                _signatureGraphics.Clear(Color.White);
            }
            _strokes.Clear();
            _hasSignature = false;
            UpdateStatus();
            if (_signaturePanel != null)
            {
                _signaturePanel.Invalidate();
            }
        }

        private void OnSignaturePanelPaint(object? sender, PaintEventArgs e)
        {
            if (_signatureBitmap == null) return;

            e.Graphics.DrawImage(_signatureBitmap, 0, 0);
        }

        private void OnSignaturePanelMouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            _isDrawing = true;
            _lastPoint = e.Location;
            _currentStroke = new List<Point> { _lastPoint };
        }

        private void OnSignaturePanelMouseMove(object? sender, MouseEventArgs e)
        {
            if (!_isDrawing) return;

            var currentPoint = e.Location;

            if (_currentStroke != null)
            {
                _currentStroke.Add(currentPoint);
            }

            if (_signatureGraphics != null)
            {
                using var pen = new Pen(PenColor, PenWidth);
                _signatureGraphics.DrawLine(pen, _lastPoint, currentPoint);
            }

            _lastPoint = currentPoint;
            _hasSignature = true;
            _signaturePanel?.Invalidate();
        }

        private void OnSignaturePanelMouseUp(object? sender, MouseEventArgs e)
        {
            if (_isDrawing && _currentStroke != null)
            {
                _strokes.Add(_currentStroke);
            }

            _isDrawing = false;
            _currentStroke = null;
            UpdateStatus();
        }

        private void OnSignaturePanelMouseLeave(object? sender, EventArgs e)
        {
            _isDrawing = false;
            _currentStroke = null;
        }

        private void OnClear(object? sender, EventArgs e)
        {
            ClearCanvas();
            SignatureCleared?.Invoke(this, EventArgs.Empty);
        }

        private void OnUndo(object? sender, EventArgs e)
        {
            if (_strokes.Count == 0) return;

            _strokes.RemoveAt(_strokes.Count - 1);

            RedrawCanvas();

            _hasSignature = _strokes.Count > 0;
            UpdateStatus();
        }

        private void RedrawCanvas()
        {
            if (_signatureGraphics == null) return;

            _signatureGraphics.Clear(Color.White);

            foreach (var stroke in _strokes)
            {
                if (stroke.Count < 2) continue;

                using var pen = new Pen(PenColor, PenWidth);
                for (int i = 1; i < stroke.Count; i++)
                {
                    _signatureGraphics.DrawLine(pen, stroke[i - 1], stroke[i]);
                }
            }

            _signaturePanel?.Invalidate();
        }

        private void OnSave(object? sender, EventArgs e)
        {
            if (RequireConfirmation && !(_confirmCheckEdit?.Checked ?? false))
            {
                XtraMessageBox.Show("서명 확인 체크박스를 선택하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_hasSignature)
            {
                XtraMessageBox.Show("서명을 입력하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var data = GetSignatureData();
            OnSignatureSigned(data ?? Array.Empty<byte>());
        }

        private byte[]? GetSignatureData()
        {
            if (_signatureBitmap == null) return null;

            using var ms = new System.IO.MemoryStream();
            _signatureBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        private void LoadSignatureData(byte[]? data)
        {
            if (data == null || data.Length == 0)
            {
                ClearCanvas();
                return;
            }

            try
            {
                using var ms = new System.IO.MemoryStream(data);
                var loadedBitmap = new Bitmap(ms);

                if (_signatureGraphics != null)
                {
                    _signatureGraphics.DrawImage(loadedBitmap, 0, 0);
                }

                _hasSignature = true;
                _signaturePanel?.Invalidate();
                UpdateStatus();
            }
            catch
            {
                ClearCanvas();
            }
        }

        private void UpdateStatus()
        {
            if (_statusLabel == null) return;

            if (!_hasSignature)
            {
                _statusLabel.Text = "서명 필드에 서명하세요";
                _statusLabel.Appearance.ForeColor = Color.Gray;
            }
            else if (RequireConfirmation && !(_confirmCheckEdit?.Checked ?? false))
            {
                _statusLabel.Text = "서명 완료 - 확인 체크 필요";
                _statusLabel.Appearance.ForeColor = Color.Orange;
            }
            else
            {
                _statusLabel.Text = $"서명 완료 - {SignerName ?? ""}";
                _statusLabel.Appearance.ForeColor = Color.Green;
            }
        }

        protected virtual void OnSignatureSigned(byte[] data)
        {
            SignatureSigned?.Invoke(this, new SignatureEventArgs
            {
                SignatureData = data,
                Comment = Comment,
                SignerName = SignerName,
                Timestamp = DateTime.Now
            });
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_signaturePanel != null && (_signatureBitmap == null || 
                _signatureBitmap.Width != _signaturePanel.Width || 
                _signatureBitmap.Height != _signaturePanel.Height))
            {
                InitializeSignatureCanvas();
            }
        }
    }
}
