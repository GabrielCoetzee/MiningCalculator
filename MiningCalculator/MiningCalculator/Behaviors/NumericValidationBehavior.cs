namespace MiningCalculator.Behaviors
{
    public class NumericValidationBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnTextChanged;
            base.OnDetachingFrom(entry);
        }

        private void OnTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender == null)
                return;

            if (!string.IsNullOrEmpty(e.NewTextValue) && !e.NewTextValue.All(x => char.IsDigit(x) || x == '.'))
            {
                ((Entry)sender).Text = new string([.. e.NewTextValue.Where(char.IsDigit)]);
            }
        }
    }
}
