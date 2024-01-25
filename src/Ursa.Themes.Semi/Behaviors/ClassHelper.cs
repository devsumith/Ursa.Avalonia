﻿using Avalonia;

namespace Ursa.Themes.Semi;

internal class ClassHelper: AvaloniaObject
{
    static ClassHelper()
    {
        ClassesProperty.Changed.AddClassHandler<StyledElement>(OnClassesChanged);
    }
    
    public static readonly AttachedProperty<string> ClassesProperty =
        AvaloniaProperty.RegisterAttached<ClassHelper, StyledElement, string>("Classes");

    public static void SetClasses(AvaloniaObject obj, string value) => obj.SetValue(ClassesProperty, value);
    public static string GetClasses(AvaloniaObject obj) => obj.GetValue(ClassesProperty);
    
    private static void OnClassesChanged(StyledElement sender, AvaloniaPropertyChangedEventArgs value)
    {
        IEnumerable<string> classes = value.GetNewValue<IEnumerable<string>>();
        sender.Classes.Clear();
        sender.Classes.AddRange(classes);
    }
}