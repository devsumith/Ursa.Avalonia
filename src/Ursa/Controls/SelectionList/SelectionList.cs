using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
using Irihi.Avalonia.Shared.Helpers;

namespace Ursa.Controls;

[TemplatePart(PART_Indicator, typeof(ContentPresenter))]
public class SelectionList: SelectingItemsControl
{
    public const string PART_Indicator = "PART_Indicator";
    private static readonly FuncTemplate<Panel?> DefaultPanel = new(() => new StackPanel());
    
    private ImplicitAnimationCollection? _implicitAnimations;
    private ContentPresenter? _indicator;

    public static readonly StyledProperty<Control?> IndicatorProperty = AvaloniaProperty.Register<SelectionList, Control?>(
        nameof(Indicator));

    public Control? Indicator
    {
        get => GetValue(IndicatorProperty);
        set => SetValue(IndicatorProperty, value);
    }
    
    static SelectionList()
    {
        SelectionModeProperty.OverrideMetadata<SelectionList>(
            new StyledPropertyMetadata<SelectionMode>(
            defaultValue: SelectionMode.Single, 
            coerce: (o, mode) => SelectionMode.Single)
            );
        SelectedItemProperty.Changed.AddClassHandler<SelectionList, object?>((list, args) =>
            list.OnSelectedItemChanged(args));
    }

    private void OnSelectedItemChanged(AvaloniaPropertyChangedEventArgs<object?> args)
    {
        var newValue = args.NewValue.Value;
        if (newValue is null)
        {
            OpacityProperty.SetValue(0d, _indicator);
            return;
        }
        var container = ContainerFromItem(newValue);
        if (container is null)
        {
            OpacityProperty.SetValue(0d, _indicator);
            return;
        }
        OpacityProperty.SetValue(1d, _indicator);
        InvalidateMeasure();
        InvalidateArrange();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if(_indicator is not null && SelectedItem is not null)
        {
            var container = ContainerFromItem(SelectedItem);
            if (container is null) return size;
            _indicator.Arrange(container.Bounds);
        }
        return size;
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<SelectionListItem>(item, out recycleKey);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new SelectionListItem();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _indicator= e.NameScope.Find<ContentPresenter>(PART_Indicator);
        EnsureIndicatorAnimation();
    }

    private void EnsureIndicatorAnimation()
    {
        if (_indicator is not null)
        {
            _indicator.Opacity = 0;
            SetUpAnimation();
            if (ElementComposition.GetElementVisual(_indicator) is { } v)
            {
                v.ImplicitAnimations = _implicitAnimations;
            }
        }
    }

    internal void SelectByIndex(int index)
    {
        using var operation = Selection.BatchUpdate();
        Selection.Clear();
        Selection.Select(index);
    }
    
    private void SetUpAnimation()
    {
        if (_implicitAnimations != null) return;
        var compositorVisual = ElementComposition.GetElementVisual(this);
        if (compositorVisual is null) return;
        var compositor = ElementComposition.GetElementVisual(this)!.Compositor;
        var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
        offsetAnimation.Target = nameof(CompositionVisual.Offset);
        offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
        offsetAnimation.Duration = TimeSpan.FromSeconds(0.3);
        var sizeAnimation = compositor.CreateVector2KeyFrameAnimation();
        sizeAnimation.Target = nameof(CompositionVisual.Size);
        sizeAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
        sizeAnimation.Duration = TimeSpan.FromSeconds(0.3);
        var opacityAnimation = compositor.CreateScalarKeyFrameAnimation();
        opacityAnimation.Target = nameof(CompositionVisual.Opacity);
        opacityAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
        opacityAnimation.Duration = TimeSpan.FromSeconds(0.3);

        _implicitAnimations = compositor.CreateImplicitAnimationCollection();
        _implicitAnimations[nameof(CompositionVisual.Offset)] = offsetAnimation;
        _implicitAnimations[nameof(CompositionVisual.Size)] = sizeAnimation;
        _implicitAnimations[nameof(CompositionVisual.Opacity)] = opacityAnimation;
    }
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
        var hotkeys = Application.Current!.PlatformSettings?.HotkeyConfiguration;
        
        if (e.Key.ToNavigationDirection() is { } direction &&  direction.IsDirectional())
        {
            e.Handled |= MoveSelection(direction, WrapSelection);
        }
        base.OnKeyDown(e);
    }
}
