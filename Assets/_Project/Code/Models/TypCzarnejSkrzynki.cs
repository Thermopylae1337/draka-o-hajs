
using System;

namespace Assets
{
    public enum TypCzarnejSkrzynki
    {
        WYDŁUŻENIE_ODPOWIEDZI,
        PONOWNE_WYLOSOWANIE_KATEGORII,
        USUNIĘCIE_KATEGORII_Z_RUNDY
    }
    public static class TypCzarnejSkrzynkiExtensions
    {
        public static string FriendlyNazwa(this TypCzarnejSkrzynki skrzynka)
        {
            switch (skrzynka)
            {
                case TypCzarnejSkrzynki.WYDŁUŻENIE_ODPOWIEDZI:
                    return "Wydłużenie Odpowiedzi";
                case TypCzarnejSkrzynki.PONOWNE_WYLOSOWANIE_KATEGORII:
                    return "Podwojona Nagroda";
                case TypCzarnejSkrzynki.USUNIĘCIE_KATEGORII_Z_RUNDY:
                    return "Usunięcie Kategorii z Rundy";
                default:
                    return null;
            }
        }
    }
}
