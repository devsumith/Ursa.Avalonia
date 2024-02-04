using Avalonia.Controls.Primitives;

namespace Ursa.Controls;

public class CustomDrawerControl: DrawerControlBase
{
    internal bool IsCloseButtonVisible { get; set; }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_closeButton is not null)
        {
            _closeButton.IsVisible = IsCloseButtonVisible;
        }
    }

    public override void Close()
    {
        if (DataContext is IDialogContext context)
        {
            context.Close();
        }
        else
        {
            OnElementClosing(this, null);
        }
    }
}