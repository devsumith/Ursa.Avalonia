using Avalonia.Controls;

namespace Ursa.Controls;

public class OverflowStackPanel: StackPanel
{
    public Panel? OverflowPanel { get; set; }
    public void MoveChildrenToOverflowPanel()
    {
        var children = this.Children.ToList();
        foreach (var child in children)
        {
            Children.Remove(child);
            OverflowPanel?.Children.Add(child);
        }
    }
    
    public void MoveChildrenToMainPanel()
    {
        var children = this.OverflowPanel?.Children.ToList();
        if (children != null && children.Count > 0)
        {
            foreach (var child in children)
            {
                OverflowPanel?.Children.Remove(child);
                Children.Add(child);
            }
        }
    }
}