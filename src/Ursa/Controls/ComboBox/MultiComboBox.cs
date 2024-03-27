﻿using System.Collections;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Irihi.Avalonia.Shared.Helpers;
using Irihi.Avalonia.Shared.Contracts;

namespace Ursa.Controls;

[TemplatePart(PART_BackgroundBorder, typeof(Border))]
[PseudoClasses(PC_DropDownOpen, PC_Empty)]
public class MultiComboBox: SelectingItemsControl, IInnerContentControl
{
    public const string PART_BackgroundBorder = "PART_BackgroundBorder";
    public const string PC_DropDownOpen = ":dropdownopen";
    public const string PC_Empty = ":selection-empty";
    
    private Border? _rootBorder;
    
    private static ITemplate<Panel?> _defaultPanel = new FuncTemplate<Panel?>(() => new VirtualizingStackPanel());

    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        ComboBox.IsDropDownOpenProperty.AddOwner<MultiComboBox>();
    
    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    public static readonly StyledProperty<double> MaxDropdownHeightProperty = AvaloniaProperty.Register<MultiComboBox, double>(
        nameof(MaxDropdownHeight));

    public double MaxDropdownHeight
    {
        get => GetValue(MaxDropdownHeightProperty);
        set => SetValue(MaxDropdownHeightProperty, value);
    }

    public static readonly StyledProperty<double> MaxSelectionBoxHeightProperty = AvaloniaProperty.Register<MultiComboBox, double>(
        nameof(MaxSelectionBoxHeight));

    public double MaxSelectionBoxHeight
    {
        get => GetValue(MaxSelectionBoxHeightProperty);
        set => SetValue(MaxSelectionBoxHeightProperty, value);
    }

    public new static readonly StyledProperty<IList?> SelectedItemsProperty = AvaloniaProperty.Register<MultiComboBox, IList?>(
        nameof(SelectedItems), new AvaloniaList<object>());

    public new IList? SelectedItems
    {
        get => GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    public static readonly StyledProperty<object?> InnerLeftContentProperty = AvaloniaProperty.Register<MultiComboBox, object?>(
        nameof(InnerLeftContent));

    public object? InnerLeftContent
    {
        get => GetValue(InnerLeftContentProperty);
        set => SetValue(InnerLeftContentProperty, value);
    }

    public static readonly StyledProperty<object?> InnerRightContentProperty = AvaloniaProperty.Register<MultiComboBox, object?>(
        nameof(InnerRightContent));

    public object? InnerRightContent
    {
        get => GetValue(InnerRightContentProperty);
        set => SetValue(InnerRightContentProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> SelectedItemTemplateProperty = AvaloniaProperty.Register<MultiComboBox, IDataTemplate?>(
        nameof(SelectedItemTemplate));

    public IDataTemplate? SelectedItemTemplate
    {
        get => GetValue(SelectedItemTemplateProperty);
        set => SetValue(SelectedItemTemplateProperty, value);
    }
    
    static MultiComboBox()
    {
        FocusableProperty.OverrideDefaultValue<MultiComboBox>(true);
        ItemsPanelProperty.OverrideDefaultValue<MultiComboBox>(_defaultPanel);
        IsDropDownOpenProperty.AffectsPseudoClass<MultiComboBox>(PC_DropDownOpen);
        SelectedItemsProperty.Changed.AddClassHandler<MultiComboBox, IList?>((box, args) => box.OnSelectedItemsChanged(args));
    }

    public MultiComboBox()
    {
        if (SelectedItems is INotifyCollectionChanged c)
        {
            c.CollectionChanged+=OnSelectedItemsCollectionChanged;
        }
    }

    private void OnSelectedItemsChanged(AvaloniaPropertyChangedEventArgs<IList?> args)
    {
        if (args.OldValue.Value is INotifyCollectionChanged old)
        {
            old.CollectionChanged-=OnSelectedItemsCollectionChanged;
        }
        if (args.NewValue.Value is INotifyCollectionChanged @new)
        {
            @new.CollectionChanged += OnSelectedItemsCollectionChanged;
        }
    }

    private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        PseudoClasses.Set(PC_Empty, SelectedItems?.Count == 0);
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = item;
        return item is not MultiComboBoxItem;
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new MultiComboBoxItem();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        PointerPressedEvent.RemoveHandler(OnBackgroundPointerPressed, _rootBorder);
        _rootBorder = e.NameScope.Find<Border>(PART_BackgroundBorder);
        PointerPressedEvent.AddHandler(OnBackgroundPointerPressed, _rootBorder);
        PseudoClasses.Set(PC_Empty, SelectedItems?.Count == 0);
    }

    private void OnBackgroundPointerPressed(object sender, PointerPressedEventArgs e)
    {
        SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
    }

    internal void ItemFocused(MultiComboBoxItem dropDownItem)
    {
        if (IsDropDownOpen && dropDownItem.IsFocused && dropDownItem.IsArrangeValid)
        {
            dropDownItem.BringIntoView();
        }
    }

    public void Remove(object? o)
    {
        if (o is StyledElement s)
        {
            var data = s.DataContext;
            this.SelectedItems?.Remove(data);
            var item = this.Items.FirstOrDefault(a => ReferenceEquals(a, data));
            if (item is not null)
            {
                var container = ContainerFromItem(item);
                if (container is MultiComboBoxItem t)
                {
                    t.IsSelected = false;
                }
            }
        }
        
    }

    public void Clear()
    {
        // this.SelectedItems?.Clear();
        var containers = Presenter?.Panel?.Children;
        if(containers is null) return;
        foreach (var container in containers)
        {
            if (container is MultiComboBoxItem t)
            {
                t.IsSelected = false;
            }
        }
    }
    
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        if (SelectedItems is INotifyCollectionChanged c)
        {
            c.CollectionChanged-=OnSelectedItemsCollectionChanged;
        }
    }
}