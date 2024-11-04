public class ListaOdznak
{
    private List<Odznaka> odznaki;

    public ListaOdznak()
    {
        odznaki = new List<Odznaka>();
    }

    public void DodajOdznake(Odznaka odznaka)
    {
        odznaki.Add(odznaka);
    }

    public Odznaka WyszukajOdznake(string nazwa)
    {
        if (odznaki.Count > 0)
        {
            return odznaki.Find(odznaka => odznaka.Nazwa.Equals(nazwa, StringComparison.OrdinalIgnoreCase));
        }
        return null;
    }

    public bool CzyOdznakaOdblokowana(string nazwa)
    {
        var odznaka = WyszukajOdznake(nazwa);
        return odznaka != null && odznaka.CzyOdblokowana();
    }
}
