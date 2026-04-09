public static class CurrencyFormatter
{
    public static string ToGameCurrency(this int amount)
    {
        if (amount < 1000) 
            return amount.ToString();
            
        return string.Format(new System.Globalization.CultureInfo("el-GR"), "{0:N0}", amount);
    }
}