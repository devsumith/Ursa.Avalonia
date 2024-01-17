using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Utilities;

namespace Ursa.Controls;

/// <summary>
/// 1. Notice that this is not used in ScrollBar, so ViewportSize related feature is not necessary.
/// 2. Maximum, Minimum, MaxValue and MinValue are coerced there.
/// </summary>
[PseudoClasses(PC_Horizontal, PC_Vertical)]
public class RangeTrack: Control
{
    public const string PC_Horizontal = ":horizontal";
    public const string PC_Vertical = ":vertical";
    private double _density;
    private Vector _lastDrag;
    
    public static readonly StyledProperty<double> MinimumProperty = AvaloniaProperty.Register<RangeTrack, double>(
        nameof(Minimum), coerce: CoerceMinimum);

    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly StyledProperty<double> MaximumProperty = AvaloniaProperty.Register<RangeTrack, double>(
        nameof(Maximum), coerce: CoerceMaximum);

    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }
    
    public static readonly StyledProperty<double> LowerValueProperty = AvaloniaProperty.Register<RangeTrack, double>(
        nameof(LowerValue), coerce: CoerceLowerValue);
    
    public double LowerValue
    {
        get => GetValue(LowerValueProperty);
        set => SetValue(LowerValueProperty, value);
    }
    
    public static readonly StyledProperty<double> UpperValueProperty = AvaloniaProperty.Register<RangeTrack, double>(
        nameof(UpperValue), coerce: CoerceUpperValue);
    
    public double UpperValue
    {
        get => GetValue(UpperValueProperty);
        set => SetValue(UpperValueProperty, value);
    }

    public static readonly StyledProperty<Orientation> OrientationProperty = AvaloniaProperty.Register<RangeTrack, Orientation>(
        nameof(Orientation));

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly StyledProperty<Button?> UpperButtonProperty = AvaloniaProperty.Register<RangeTrack, Button?>(
        nameof(UpperButton));

    public Button? UpperButton
    {
        get => GetValue(UpperButtonProperty);
        set => SetValue(UpperButtonProperty, value);
    }

    public static readonly StyledProperty<Button?> LowerButtonProperty = AvaloniaProperty.Register<RangeTrack, Button?>(
        nameof(LowerButton));

    public Button? LowerButton
    {
        get => GetValue(LowerButtonProperty);
        set => SetValue(LowerButtonProperty, value);
    }

    public static readonly StyledProperty<Button?> InnerButtonProperty = AvaloniaProperty.Register<RangeTrack, Button?>(
        nameof(InnerButton));

    public Button? InnerButton
    {
        get => GetValue(InnerButtonProperty);
        set => SetValue(InnerButtonProperty, value);
    }
    
    public static readonly StyledProperty<Thumb?> UpperThumbProperty = AvaloniaProperty.Register<RangeTrack, Thumb?>(
        nameof(UpperThumb));
    
    public Thumb? UpperThumb
    {
        get => GetValue(UpperThumbProperty);
        set => SetValue(UpperThumbProperty, value);
    }
    
    public static readonly StyledProperty<Thumb?> LowerThumbProperty = AvaloniaProperty.Register<RangeTrack, Thumb?>(
        nameof(LowerThumb));
    
    public Thumb? LowerThumb
    {
        get => GetValue(LowerThumbProperty);
        set => SetValue(LowerThumbProperty, value);
    }

    public static readonly StyledProperty<bool> IsDirectionReversedProperty = AvaloniaProperty.Register<RangeTrack, bool>(
        nameof(IsDirectionReversed));

    public bool IsDirectionReversed
    {
        get => GetValue(IsDirectionReversedProperty);
        set => SetValue(IsDirectionReversedProperty, value);
    }

    public static readonly RoutedEvent<RangeValueChangedEventArgs> ValueChangedEvent =
        RoutedEvent.Register<RangeTrack, RangeValueChangedEventArgs>(nameof(ValueChanged), RoutingStrategies.Bubble);
    
    public event EventHandler<RangeValueChangedEventArgs> ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    } 
    
    static RangeTrack()
    {
        OrientationProperty.Changed.AddClassHandler<RangeTrack, Orientation>((o, e) => o.OnOrientationChanged(e));
        LowerThumbProperty.Changed.AddClassHandler<RangeTrack, Thumb?>((o, e) => o.OnThumbChanged(e));
        UpperThumbProperty.Changed.AddClassHandler<RangeTrack, Thumb?>((o, e) => o.OnThumbChanged(e));
        LowerButtonProperty.Changed.AddClassHandler<RangeTrack, Button?>((o, e) => o.OnButtonChanged(e));
        UpperButtonProperty.Changed.AddClassHandler<RangeTrack, Button?>((o, e) => o.OnButtonChanged(e));
        InnerButtonProperty.Changed.AddClassHandler<RangeTrack, Button?>((o, e) => o.OnButtonChanged(e));
        MinimumProperty.Changed.AddClassHandler<RangeTrack, double>((o, e) => o.OnMinimumChanged(e));
        MaximumProperty.Changed.AddClassHandler<RangeTrack, double>((o, e) => o.OnMaximumChanged(e));
        LowerValueProperty.Changed.AddClassHandler<RangeTrack, double>((o, e) => o.OnValueChanged(e, true));
        UpperValueProperty.Changed.AddClassHandler<RangeTrack, double>((o, e) => o.OnValueChanged(e, false));
        AffectsArrange<RangeTrack>(
            MinimumProperty, 
            MaximumProperty, 
            LowerValueProperty, 
            UpperValueProperty, 
            OrientationProperty, 
            IsDirectionReversedProperty);
    }

    private void OnValueChanged(AvaloniaPropertyChangedEventArgs<double> args, bool isLower)
    {
        var oldValue = args.OldValue.Value;
        var newValue = args.NewValue.Value;
        if (oldValue != newValue)
        {
            RaiseEvent(new RangeValueChangedEventArgs(ValueChangedEvent, this, oldValue, newValue, isLower));
        }
    }

    private void OnMinimumChanged(AvaloniaPropertyChangedEventArgs<double> avaloniaPropertyChangedEventArgs)
    {
        if (IsInitialized)
        {
            CoerceValue(MaximumProperty);
            CoerceValue(LowerValueProperty);
            CoerceValue(UpperValueProperty);
        }
    }
    
    private void OnMaximumChanged(AvaloniaPropertyChangedEventArgs<double> avaloniaPropertyChangedEventArgs)
    {
        if (IsInitialized)
        {
            CoerceValue(LowerValueProperty);
            CoerceValue(UpperValueProperty);
        }
    }

    private void OnButtonChanged(AvaloniaPropertyChangedEventArgs<Button?> args)
    {
        var oldButton = args.OldValue.Value;
        var newButton = args.NewValue.Value;
        if (oldButton is not null)
        {
            LogicalChildren.Remove(oldButton);
            VisualChildren.Remove(oldButton);
        }
        if (newButton is not null)
        {
            LogicalChildren.Add(newButton);
            VisualChildren.Add(newButton);
        }
    }

    private void OnThumbChanged(AvaloniaPropertyChangedEventArgs<Thumb?> args)
    {
        var oldThumb = args.OldValue.Value;
        var newThumb = args.NewValue.Value;
        if(oldThumb is not null)
        {
            oldThumb.DragDelta -= OnThumbDragDelta;
            LogicalChildren.Remove(oldThumb);
            VisualChildren.Remove(oldThumb);
        }
        if (newThumb is not null)
        {
            newThumb.DragDelta += OnThumbDragDelta;
            LogicalChildren.Add(newThumb);
            VisualChildren.Add(newThumb);
        }
    }

    private void OnThumbDragDelta(object sender, VectorEventArgs e)
    { 
        if(sender is not Thumb thumb) return;
        bool lower = thumb == LowerThumb;
        double scale = IsDirectionReversed ? -1 : 1;
        double originalValue = lower ? LowerValue : UpperValue;
        double value;
        if (Orientation == Orientation.Horizontal)
        {
            value = scale * e.Vector.X * _density;
        }
        else
        {
            value = -1 * scale * e.Vector.Y * _density;
        }
        var factor = e.Vector / value;
        if (lower)
        {
            SetCurrentValue(LowerValueProperty, MathUtilities.Clamp(originalValue + value, Minimum, UpperValue));
        }
        else
        {
            SetCurrentValue(UpperValueProperty, MathUtilities.Clamp(originalValue + value, LowerValue, Maximum));
        }
        
    }

    private void OnOrientationChanged(AvaloniaPropertyChangedEventArgs<Orientation> args)
    {
        Orientation o = args.NewValue.Value;
        PseudoClasses.Set(PC_Horizontal, o == Orientation.Horizontal);
        PseudoClasses.Set(PC_Vertical, o == Orientation.Vertical);
    }

    private static double CoerceMaximum(AvaloniaObject sender, double value)
    {
        return ValidateDouble(value)
            ? Math.Max(value, sender.GetValue(MinimumProperty))
            : sender.GetValue(MinimumProperty);
    }

    private static double CoerceMinimum(AvaloniaObject sender, double value)
    {
        return ValidateDouble(value) ? value : sender.GetValue(MaximumProperty);
    }

    private static double CoerceLowerValue(AvaloniaObject sender, double value)
    {
        if (!ValidateDouble(value)) return sender.GetValue(LowerValueProperty);
        value = MathUtilities.Clamp(value, sender.GetValue(MinimumProperty), sender.GetValue(MaximumProperty));
        value = MathUtilities.Clamp(value, sender.GetValue(MinimumProperty), sender.GetValue(UpperValueProperty));
        return value;
    }

    private static double CoerceUpperValue(AvaloniaObject sender, double value)
    {
        if (!ValidateDouble(value)) return sender.GetValue(UpperValueProperty);
        value = MathUtilities.Clamp(value, sender.GetValue(MinimumProperty), sender.GetValue(MaximumProperty));
        value = MathUtilities.Clamp(value, sender.GetValue(LowerValueProperty), sender.GetValue(MaximumProperty));
        return value;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        CoerceValue(MaximumProperty);
        CoerceValue(LowerValueProperty);
        CoerceValue(UpperValueProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var desiredSize = new Size();
        if (LowerThumb is not null && UpperThumb is not null)
        {
            LowerThumb.Measure(availableSize);
            UpperThumb.Measure(availableSize);
            if (Orientation == Orientation.Horizontal)
            {
                desiredSize = new Size(LowerThumb.DesiredSize.Width + UpperThumb.DesiredSize.Width,
                    Math.Max(LowerThumb.DesiredSize.Height, UpperThumb.DesiredSize.Height));
            }
            else
            {
                desiredSize = new Size(Math.Max(LowerThumb.DesiredSize.Width, UpperThumb.DesiredSize.Width),
                    LowerThumb.DesiredSize.Height + UpperThumb.DesiredSize.Height);
            }
        }
        return desiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var vertical = Orientation == Orientation.Vertical;
        double lowerButtonLength, innerButtonLength, upperButtonLength, lowerThumbLength, upperThumbLength;
        ComputeSliderLengths(finalSize, vertical, out lowerButtonLength, out innerButtonLength, out upperButtonLength,
            out lowerThumbLength, out upperThumbLength);
        var offset = new Point();
        var pieceSize = finalSize;
        if (vertical)
        {
            CoerceLength(ref lowerButtonLength, finalSize.Height);
            CoerceLength(ref innerButtonLength, finalSize.Height);
            CoerceLength(ref upperButtonLength, finalSize.Height);
            CoerceLength(ref lowerThumbLength, finalSize.Height);
            CoerceLength(ref upperThumbLength, finalSize.Height);
            if (IsDirectionReversed)
            {
                pieceSize = pieceSize.WithHeight(upperButtonLength);
                UpperButton?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithY(offset.Y + upperButtonLength);
                pieceSize = pieceSize.WithHeight(upperThumbLength);
                UpperThumb?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithY(offset.Y + upperThumbLength);
                pieceSize = pieceSize.WithHeight(innerButtonLength);
                InnerButton?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithY(offset.Y + innerButtonLength);
                pieceSize = pieceSize.WithHeight(lowerThumbLength);
                LowerThumb?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithY(offset.Y + lowerThumbLength);
                pieceSize = pieceSize.WithHeight(lowerButtonLength);
                LowerButton?.Arrange(new Rect(offset, pieceSize));
            }
            else
            {
                pieceSize = pieceSize.WithHeight(lowerButtonLength);
                LowerButton?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithY(offset.Y + lowerButtonLength);
                pieceSize = pieceSize.WithHeight(lowerThumbLength);
                LowerThumb?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithY(offset.Y + lowerThumbLength);
                pieceSize = pieceSize.WithHeight(innerButtonLength);
                InnerButton?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithY(offset.Y + innerButtonLength);
                pieceSize = pieceSize.WithHeight(upperThumbLength);
                UpperThumb?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithY(offset.Y + upperThumbLength);
                pieceSize = pieceSize.WithHeight(upperButtonLength);
                UpperButton?.Arrange(new Rect(offset, pieceSize));
            }
        }
        else
        {
            CoerceLength(ref lowerButtonLength, finalSize.Width);
            CoerceLength(ref innerButtonLength, finalSize.Width);
            CoerceLength(ref upperButtonLength, finalSize.Width);
            CoerceLength(ref lowerThumbLength, finalSize.Width);
            CoerceLength(ref upperThumbLength, finalSize.Width);
            if (IsDirectionReversed)
            {
                pieceSize = pieceSize.WithWidth(upperButtonLength);
                UpperButton?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithX(offset.X + upperButtonLength);
                pieceSize = pieceSize.WithWidth(upperThumbLength);
                UpperThumb?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithX(offset.X + upperThumbLength);
                pieceSize = pieceSize.WithWidth(innerButtonLength);
                InnerButton?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithX(offset.X + innerButtonLength);
                pieceSize = pieceSize.WithWidth(lowerThumbLength);
                LowerThumb?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithX(offset.X + lowerThumbLength);
                pieceSize = pieceSize.WithWidth(lowerButtonLength);
                LowerButton?.Arrange(new Rect(offset, pieceSize));
            }
            else
            {
                pieceSize = pieceSize.WithWidth(lowerButtonLength);
                LowerButton?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithX(offset.X + lowerButtonLength);
                pieceSize = pieceSize.WithWidth(lowerThumbLength);
                LowerThumb?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithX(offset.X + lowerThumbLength);
                pieceSize = pieceSize.WithWidth(innerButtonLength);
                InnerButton?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithX(offset.X + innerButtonLength);
                pieceSize = pieceSize.WithWidth(upperThumbLength);
                UpperThumb?.Arrange(new Rect(offset, pieceSize));
                offset = offset.WithX(offset.X + upperThumbLength);
                pieceSize = pieceSize.WithWidth(upperButtonLength);
                UpperButton?.Arrange(new Rect(offset, pieceSize));
                
            }
        }
        return finalSize;
    }
    
    private void ComputeSliderLengths(
        Size arrangeSize, 
        bool isVertical, 
        out double lowerButtonLength, 
        out double innerButtonLength, 
        out double upperButtonLength, 
        out double lowerThumbLength, 
        out double upperThumbLength)
    {
        double range = Math.Max(0, Maximum - Minimum);
        double lowerOffset = Math.Min(range, LowerValue - Minimum);
        double upperOffset = Math.Min(range, UpperValue - Minimum);

        double trackLength;
        if (isVertical)
        {
            trackLength = arrangeSize.Height;
            lowerThumbLength = LowerThumb?.DesiredSize.Height ?? 0;
            upperThumbLength = UpperThumb?.DesiredSize.Height ?? 0;
        }
        else
        {
            trackLength = arrangeSize.Width;
            lowerThumbLength = LowerThumb?.DesiredSize.Width ?? 0;
            upperThumbLength = UpperThumb?.DesiredSize.Width ?? 0;
        }
        
        CoerceLength(ref lowerThumbLength, trackLength);
        CoerceLength(ref upperThumbLength, trackLength);
        
        double remainingLength = trackLength -lowerThumbLength - upperThumbLength;
        
        lowerButtonLength = remainingLength * lowerOffset / range;
        upperButtonLength = remainingLength * (range-upperOffset) / range;
        innerButtonLength = remainingLength - lowerButtonLength - upperButtonLength;

        _density = range / remainingLength;
    }

    private static void CoerceLength(ref double componentLength, double trackLength)
    {
        if (componentLength < 0)
        {
            componentLength = 0.0;
        }
        else if (componentLength > trackLength || double.IsNaN(componentLength))
        {
            componentLength = trackLength;
        }
    }

    private static bool ValidateDouble(double value)
    {
        return !double.IsInfinity(value) && !double.IsNaN(value);
    }
}