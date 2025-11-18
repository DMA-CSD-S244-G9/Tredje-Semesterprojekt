using System;
using System.Drawing;
using System.Windows.Forms;

namespace InfiniteInfluence.WinFormsApp.Components;

public class ColorScrollBar : Control
{
    public enum Orientation { Vertical, Horizontal }
    public Orientation Direction { get; set; } = Orientation.Vertical;

    // Farver
    public Color TrackColor { get; set; } = Color.FromArgb(40, 36, 50);
    public Color ThumbColor { get; set; } = Color.FromArgb(88, 80, 110);
    public Color ThumbHoverColor { get; set; } = Color.FromArgb(110, 100, 140);


    // Range
    public int Minimum { get; set; } = 0;
    public int Maximum { get; set; } = 100;
    public int Value
    {
        get => _value;
        set
        {
            var v = Math.Max(Minimum, Math.Min(Maximum, value));
            if (v != _value) { _value = v; Invalidate(); ValueChanged?.Invoke(this, EventArgs.Empty); }
        }
    }
    public int LargeChange { get; set; } = 10;
    public event EventHandler? ValueChanged;

    private bool _dragging = false;
    private Rectangle _thumbRect;
    private int _dragOffset;
    private int _value;

    public ColorScrollBar()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        Width = 12; Height = 12;
        Cursor = Cursors.Hand;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        using var track = new SolidBrush(TrackColor);
        g.FillRectangle(track, ClientRectangle);

        var thumb = GetThumbRect();
        _thumbRect = thumb;

        bool hover = _thumbRect.Contains(PointToClient(MousePosition));
        using var th = new SolidBrush(hover ? ThumbHoverColor : ThumbColor);
        g.FillRectangle(th, thumb);
        using var pen = new Pen(Color.FromArgb(40, Color.Black), 1);
        g.DrawRectangle(pen, Rectangle.Inflate(thumb, -1, -1));
    }

    private Rectangle GetThumbRect()
    {
        const int minThumb = 20;
        if (Direction == Orientation.Vertical)
        {
            int trackLen = Height;
            int thumbLen = Math.Max(minThumb, (int)(trackLen * (LargeChange / (float)Math.Max(1, (Maximum - Minimum + LargeChange)))));
            int range = Math.Max(1, Maximum - Minimum);
            int pos = (int)((trackLen - thumbLen) * ((Value - Minimum) / (float)range));
            return new Rectangle(0, pos, Width, thumbLen);
        }
        else
        {
            int trackLen = Width;
            int thumbLen = Math.Max(minThumb, (int)(trackLen * (LargeChange / (float)Math.Max(1, (Maximum - Minimum + LargeChange)))));
            int range = Math.Max(1, Maximum - Minimum);
            int pos = (int)((trackLen - thumbLen) * ((Value - Minimum) / (float)range));
            return new Rectangle(pos, 0, thumbLen, Height);
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (_thumbRect.Contains(e.Location))
        {
            _dragging = true;
            _dragOffset = Direction == Orientation.Vertical ? e.Y - _thumbRect.Y : e.X - _thumbRect.X;
        }
    }
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (!_dragging) { Invalidate(); return; }

        if (Direction == Orientation.Vertical)
        {
            int trackLen = Height;
            int thumbLen = _thumbRect.Height;
            int newY = Math.Max(0, Math.Min(trackLen - thumbLen, e.Y - _dragOffset));
            float t = newY / (float)Math.Max(1, (trackLen - thumbLen));
            Value = Minimum + (int)(t * Math.Max(1, Maximum - Minimum));
        }
        else
        {
            int trackLen = Width;
            int thumbLen = _thumbRect.Width;
            int newX = Math.Max(0, Math.Min(trackLen - thumbLen, e.X - _dragOffset));
            float t = newX / (float)Math.Max(1, (trackLen - thumbLen));
            Value = Minimum + (int)(t * Math.Max(1, Maximum - Minimum));
        }
    }
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _dragging = false;
    }
}