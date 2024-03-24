﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Ursa.Demo.ViewModels;

public class MultiComboBoxDemoViewModel: ObservableObject
{
    public ObservableCollection<string> Items { get; set; }

    public MultiComboBoxDemoViewModel()
    {
        Items = new ObservableCollection<string>()
        {
            "Item 1",
            "Item 2",
            "Item 3",
            "Item 4",
            "Item 5",
            "Item 6",
            "Item 7",
            "Item 8",
            "Illinois",
            "Indiana",
            "Iowa",
            "Kansas",
            "Kentucky",
            "Louisiana",
        };
    }
}