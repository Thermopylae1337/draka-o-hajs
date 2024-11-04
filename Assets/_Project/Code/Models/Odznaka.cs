public class Odznaka
{
    public string Nazwa { get; private set; }
    public bool StatusOdblokowania { get; private set; }
    public string WarunekOdblokowania { get; private set; }

    public Odznaka(string nazwa, string warunekOdblokowania)
    {
        Nazwa = nazwa;
        WarunekOdblokowania = warunekOdblokowania;
        StatusOdblokowania = false;
    }

    public void Odblokuj()
    {
        StatusOdblokowania = true;
    }

    public bool CzyOdblokowana()
    {
        return StatusOdblokowania;
    }
}
