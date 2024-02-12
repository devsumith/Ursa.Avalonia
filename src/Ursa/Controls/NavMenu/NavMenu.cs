﻿using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace Ursa.Controls;

public class NavMenu: ItemsControl
{
    public static readonly StyledProperty<object?> SelectedItemProperty = AvaloniaProperty.Register<NavMenu, object?>(
        nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly StyledProperty<IBinding?> IconBindingProperty = AvaloniaProperty.Register<NavMenu, IBinding?>(
        nameof(IconBinding));

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IBinding? IconBinding
    {
        get => GetValue(IconBindingProperty);
        set => SetValue(IconBindingProperty, value);
    }

    public static readonly StyledProperty<IBinding?> HeaderBindingProperty = AvaloniaProperty.Register<NavMenu, IBinding?>(
        nameof(HeaderBinding));

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IBinding? HeaderBinding
    {
        get => GetValue(HeaderBindingProperty);
        set => SetValue(HeaderBindingProperty, value);
    }

    public static readonly StyledProperty<IBinding?> SubMenuBindingProperty = AvaloniaProperty.Register<NavMenu, IBinding?>(
        nameof(SubMenuBinding));

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IBinding? SubMenuBinding
    {
        get => GetValue(SubMenuBindingProperty);
        set => SetValue(SubMenuBindingProperty, value);
    }

    public static readonly StyledProperty<IBinding?> CommandBindingProperty = AvaloniaProperty.Register<NavMenu, IBinding?>(
        nameof(CommandBinding));

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IBinding? CommandBinding
    {
        get => GetValue(CommandBindingProperty);
        set => SetValue(CommandBindingProperty, value);
    }

    static NavMenu()
    {
        SelectedItemProperty.Changed.AddClassHandler<NavMenu, object?>((o, e) => o.OnSelectedItemChange(e));
    }

    private void OnSelectedItemChange(AvaloniaPropertyChangedEventArgs<object?> args)
    {
        Debug.WriteLine(args.NewValue.Value);
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<NavMenuItem>(item, out recycleKey);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new NavMenuItem();
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is NavMenuItem navMenuItem)
        {
            if (IconBinding is not null)
            {
                navMenuItem[!NavMenuItem.IconProperty] = IconBinding;
            }
            if (HeaderBinding is not null)
            {
                navMenuItem[!HeaderedItemsControl.HeaderProperty] = HeaderBinding;
            }
            if (SubMenuBinding is not null)
            {
                navMenuItem[!ItemsSourceProperty] = SubMenuBinding;
            }
            if (CommandBinding is not null)
            {
                navMenuItem[!NavMenuItem.CommandProperty] = CommandBinding;
            }
        }
    }

    internal void SelectItem(NavMenuItem item, NavMenuItem parent)
    {
        // if (item.IsSelected) return;
        foreach (var child in LogicalChildren)
        {
            if (child == parent)
            {
                continue;
            }
            else
            {
                if (child is NavMenuItem navMenuItem)
                {
                    navMenuItem.ClearSelection();
                }
            }
        }
        if (item.DataContext is not null && item.DataContext != this.DataContext)
        {
            SelectedItem = item.DataContext;
        }
        else
        {
            SelectedItem = item;
        }
        item.IsSelected = true;
    }
}