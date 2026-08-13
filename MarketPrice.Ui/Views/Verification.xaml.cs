using MarketPrice.Ui.ViewModels;
namespace MarketPrice.Ui.Views;

public partial class Verification : ContentPage
{
	public Verification()
	{
		InitializeComponent();
        BindingContext = new VerificationViewModel();
    }

	private void OnDigitTextChanged(object sender, TextChangedEventArgs e)
	{
		var entry = sender as Entry;
		if (entry is null) return;

		if (!string.IsNullOrEmpty(e.NewTextValue))
		{
			GetNextEntry(entry)?.Focus();
		}
		else if (string.IsNullOrEmpty(e.NewTextValue) && !string.IsNullOrEmpty(e.OldTextValue))
        {
			GetPreviousEntry(entry)?.Focus();
        }
	}

	private Entry GetNextEntry(Entry current)
	{
		if (current == digit1Entry) return digit2Entry;
		if (current == digit2Entry) return digit3Entry;
		if (current == digit3Entry) return digit4Entry;
		if (current == digit4Entry) return digit5Entry;
		if (current == digit5Entry) return digit6Entry;
		return null;
	}

	private Entry GetPreviousEntry(Entry current)
	{
        if (current == digit1Entry) return digit2Entry;
        if (current == digit2Entry) return digit3Entry;
        if (current == digit3Entry) return digit4Entry;
        if (current == digit4Entry) return digit5Entry;
        if (current == digit5Entry) return digit6Entry;
        return null;
    }




}