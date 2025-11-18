using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;




namespace InfiniteInfluence.WinFormsApp.Components;



[DefaultEvent("TextChanged")]
public class RoundedTextBox : UserControl
{
    private readonly TextBox _textBox = new TextBox();

    private int _cornerRadius = 8;
    private int _borderThickness = 3;
    private Color _borderColor = Color.FromArgb(134, 129, 158);
    private Color _borderFocusColor = Color.FromArgb(109, 84, 181);

    private bool _isPasswordField = false;
    private bool _showingPlaceholder => string.IsNullOrEmpty(Text) && !_textBox.Focused;


    
    private readonly Label _placeholderLabel = new Label();

    private string _placeholderText = string.Empty;
    private Color _placeholderColor = Color.FromArgb(150, 144, 170);



    public RoundedTextBox()
    {
        // EVENTS (hold placeholder i sync)
        _textBox.TextChanged += (s, e) => { UpdatePlaceholder(); Invalidate(); OnTextChanged(e); };
        _textBox.GotFocus += (s, e) => { UpdatePlaceholder(); Invalidate(); OnGotFocus(e); };
        _textBox.LostFocus += (s, e) => { UpdatePlaceholder(); Invalidate(); OnLostFocus(e); };

        // PLACEHOLDER-LABEL (ligger ovenpå textbox)
        _placeholderLabel.AutoSize = false;
        _placeholderLabel.Text = _placeholderText;
        _placeholderLabel.ForeColor = _placeholderColor;
        _placeholderLabel.BackColor = Color.Transparent;
        _placeholderLabel.Cursor = Cursors.IBeam;
        _placeholderLabel.TextAlign = ContentAlignment.MiddleLeft;
        _placeholderLabel.Click += (s, e) => _textBox.Focus();

        // VIGTIG Z-ORDER: tilføj label EFTER textbox, så den ligger øverst
        Controls.Add(_textBox);
        Controls.Add(_placeholderLabel);
        _placeholderLabel.BringToFront();

        // Første layout/synlighed
        UpdateTextboxBounds();
        UpdatePlaceholder();




        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint, true);

        // Default: dark, subtle purple like the screenshot
        BackColor = Color.FromArgb(60, 54, 76);   // fill (RGB 60,54,76)
        ForeColor = Color.FromArgb(134, 129, 158);  // text (RGB 134,129,158)                   // text
        Font = new Font("Segoe UI", 10f);
        Size = new Size(235, 32);
        Padding = new Padding(12, 8, 12, 8);

        // Border colors; normal vs focus glow-ish
        _borderColor = Color.FromArgb(134, 129, 158);           // match text color
        _borderFocusColor = Color.FromArgb(190, 170, 255);      // brighter on focus
        _placeholderColor = Color.FromArgb(150, 144, 170);      // soft gray/purple
        _borderThickness = 1;

        // Inner TextBox setup
        _textBox.BorderStyle = BorderStyle.None;
        _textBox.BackColor = BackColor;
        _textBox.ForeColor = ForeColor;
        _textBox.Font = Font;
        _textBox.Location = new Point(Padding.Left, Padding.Top);
        _textBox.Width = Width - Padding.Horizontal;
        _textBox.UseSystemPasswordChar = _isPasswordField;

        _textBox.TextChanged += (s, e) =>
        {
            Invalidate();
            OnTextChanged(e);
        };
        _textBox.GotFocus += (s, e) =>
        {
            if (_isPasswordField)
                _textBox.UseSystemPasswordChar = true;
            Invalidate();
            OnGotFocus(e);
        };
        _textBox.LostFocus += (s, e) =>
        {
            if (_isPasswordField)
                _textBox.UseSystemPasswordChar = true; // keep masking
            Invalidate();
            OnLostFocus(e);
        };
        _textBox.KeyDown += (s, e) => OnKeyDown(e);
        _textBox.KeyUp += (s, e) => OnKeyUp(e);

        Controls.Add(_textBox);
        UpdateTextboxBounds();
    }





    #region Exposed Properties




    [Category("Behavior")]
    [DefaultValue("")]
    public string PlaceholderText
    {
        get => _placeholderText;
        set { _placeholderText = value ?? string.Empty; UpdatePlaceholder(); Invalidate(); }
    }



    [Category("Appearance")]
    [DefaultValue(typeof(Color), "Gray")]
    public Color PlaceholderColor
    {
        get => _placeholderColor;
        set { _placeholderColor = value; UpdatePlaceholder(); Invalidate(); }
    }





    [Category("Appearance")]
    [DefaultValue(12)]
    public int CornerRadius
    {
        get => _cornerRadius;
        set { _cornerRadius = Math.Max(1, value); UpdateRegion(); Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue(1)]
    public int BorderThickness
    {
        get => _borderThickness;
        set { _borderThickness = Math.Max(1, value); Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue(typeof(Color), "Silver")]
    public Color BorderColor
    {
        get => _borderColor;
        set { _borderColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue(typeof(Color), "DodgerBlue")]
    public Color BorderFocusColor
    {
        get => _borderFocusColor;
        set { _borderFocusColor = value; Invalidate(); }
    }

    /*
    [Category("Behavior")]
    [DefaultValue("")]
    public string PlaceholderText
    {
        get => _placeholderText;
        set { _placeholderText = value ?? string.Empty; Invalidate(); }
    }

    [Category("Appearance")]
    [DefaultValue(typeof(Color), "Gray")]
    public Color PlaceholderColor
    {
        get => _placeholderColor;
        set { _placeholderColor = value; Invalidate(); }
    }
    */

    [Category("Behavior")]
    [DefaultValue(false)]
    public bool IsPasswordField
    {
        get => _isPasswordField;
        set
        {
            _isPasswordField = value;
            _textBox.UseSystemPasswordChar = _isPasswordField && (Focused || _textBox.Focused);
            Invalidate();
        }
    }

    [Category("Appearance")]
    public override Color BackColor
    {
        get => base.BackColor;
        set { base.BackColor = value; if (_textBox != null) _textBox.BackColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    public override Color ForeColor
    {
        get => base.ForeColor;
        set { base.ForeColor = value; if (_textBox != null) _textBox.ForeColor = value; Invalidate(); }
    }

    [Category("Appearance")]
    public override Font Font
    {
        get => base.Font;
        set { base.Font = value; if (_textBox != null) _textBox.Font = value; UpdateTextboxBounds(); Invalidate(); }
    }

    [Browsable(true)]
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override string Text
    {
        get => _textBox?.Text ?? string.Empty;
        set { if (_textBox != null) _textBox.Text = value; Invalidate(); }
    }

    [Category("Layout")]
    public new Padding Padding
    {
        get => base.Padding;
        set { base.Padding = value; UpdateTextboxBounds(); Invalidate(); }
    }

    [Category("Behavior")]
    [DefaultValue(HorizontalAlignment.Left)]
    public HorizontalAlignment TextAlign
    {
        get => _textBox.TextAlign;
        set { _textBox.TextAlign = value; Invalidate(); }
    }
    #endregion

    #region Layout & Painting
/*
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateRegion();
        UpdateTextboxBounds();
    }
*/
    private void UpdateTextboxBounds()
    {
        if (_textBox == null) return;
        var rect = GetInnerRect();
        _textBox.Location = new Point(rect.X, rect.Y);
        _textBox.Size = new Size(rect.Width, rect.Height);
    }

    private Rectangle GetInnerRect()
    {
        // Inner rect leaves room for the border and padding
        int pad = Math.Max(_borderThickness, 1);
        return new Rectangle(
            Padding.Left,
            Padding.Top,
            Math.Max(0, Width - Padding.Horizontal),
            Math.Max(0, Height - Padding.Vertical)
        );
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Base rounded fill and border
        using (GraphicsPath path = GetRoundRectPath(ClientRectangle, _cornerRadius))
        using (SolidBrush bgBrush = new SolidBrush(BackColor))
        using (Pen borderPen = new Pen((_textBox.Focused ? _borderFocusColor : _borderColor), _borderThickness))
        {
            e.Graphics.FillPath(bgBrush, path);
            e.Graphics.DrawPath(borderPen, path);
        }

        // Subtle inner top highlight to mimic the reference style
        using (GraphicsPath innerPath = GetRoundRectPath(Rectangle.Inflate(ClientRectangle, -1, -1), _cornerRadius))
        using (Pen highlight = new Pen(Color.FromArgb(35, 255, 255, 255), 1))
        {
            e.Graphics.DrawPath(highlight, innerPath);
        }

        // Draw placeholder if needed
        if (_showingPlaceholder && !string.IsNullOrEmpty(_placeholderText))
        {
            var rect = GetInnerRect();
            var textFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            if (_textBox.TextAlign == HorizontalAlignment.Left) textFormat |= TextFormatFlags.Left;
            else if (_textBox.TextAlign == HorizontalAlignment.Center) textFormat |= TextFormatFlags.HorizontalCenter;
            else textFormat |= TextFormatFlags.Right;

//            TextRenderer.DrawText(e.Graphics, _placeholderText, Font, rect, _placeholderColor, textFormat);
        }
    }



    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateRegion();
        UpdateTextboxBounds();
        UpdatePlaceholder(); // ← vigtigt
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        UpdateRegion();
        UpdateTextboxBounds();
        UpdatePlaceholder(); // ← vigtigt ved Designer/opstart
    }


    private void UpdatePlaceholder()
    {
        bool show = string.IsNullOrEmpty(_textBox.Text) && !_textBox.Focused;
        _placeholderLabel.Visible = show && !string.IsNullOrEmpty(_placeholderText);
        _placeholderLabel.Text = _placeholderText;
        _placeholderLabel.ForeColor = _placeholderColor;

        // Match horisontal justering med TextBox
        _placeholderLabel.TextAlign = _textBox.TextAlign switch
        {
            HorizontalAlignment.Left => ContentAlignment.MiddleLeft,
            HorizontalAlignment.Center => ContentAlignment.MiddleCenter,
            HorizontalAlignment.Right => ContentAlignment.MiddleRight,
            _ => ContentAlignment.MiddleLeft
        };

        // Placering: dæk præcis tekst-området
        var r = GetInnerRect();       // du har allerede denne i din kontrol
        _placeholderLabel.SetBounds(r.X, r.Y, r.Width, r.Height);
    }






    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        if (_textBox != null) _textBox.BackColor = BackColor;
        Invalidate();
    }

    protected override void OnForeColorChanged(EventArgs e)
    {
        base.OnForeColorChanged(e);
        if (_textBox != null) _textBox.ForeColor = ForeColor;
        Invalidate();
    }

    protected override void OnEnter(EventArgs e)
    {
        base.OnEnter(e);
        _textBox.Focus();
        Invalidate();
    }

    private void UpdateRegion()
    {
        using (var path = GetRoundRectPath(ClientRectangle, _cornerRadius))
        {
            Region?.Dispose();
            Region = new Region(path);
        }
    }

    private static GraphicsPath GetRoundRectPath(Rectangle bounds, int radius)
    {
        int r = Math.Max(1, radius) * 2;
        int w = Math.Max(0, bounds.Width - 1);
        int h = Math.Max(0, bounds.Height - 1);

        var path = new GraphicsPath();
        if (r > 0)
        {
            path.AddArc(0, 0, r, r, 180, 90);
            path.AddArc(w - r, 0, r, r, 270, 90);
            path.AddArc(w - r, h - r, r, r, 0, 90);
            path.AddArc(0, h - r, r, r, 90, 90);
            path.CloseFigure();
        }
        else
        {
            path.AddRectangle(bounds);
        }
        return path;
    }
    #endregion



    #region Public API helpers
    public void SelectAllText() => _textBox.SelectAll();
    public void SetSelection(int start, int length) => _textBox.Select(start, length);
    #endregion

    
    
    #region Design-time support

    /*
    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        UpdateRegion();
        UpdateTextboxBounds();
    }
    */

    // Make the control clickable/focusable anywhere inside
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _textBox.Focus();
    }
    #endregion
}
