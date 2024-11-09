using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

using System.Text;

public class Team
{
    //zadeklarowanie nazwa zmiennych
    private string name;
    private int money= 10000;
    private int cluesUsed=0;
    private int inactiveRounds = 0; //licznik rund bierności w licytacji
    private int bid = 0;
    private ulong id;
    //powerupy zmienione z listy stringów na inta. Po prostu będziemy mieć listę powerupów gdzieśtam w kodzie wpisaną, a żeby sprawdzić
    //czy drużyna ma danego powerupa trzeba bd zrobić 
    /*
      if( (powerUps&powerup)!=0)){
    do thing
    }
     */
    //działa tak samo a mniej zabawy z serializacją listy w liście x.x
    //no i zamiast lecieć przez listę mamy jednego ifa
    private int powerUps = 0;
    //możliwość odczytania danych
    public string Names => name;
    public int Money => money;
    public int CluesUsed => cluesUsed;
    public int InactiveRounds => inactiveRounds;
    public int PowerUps => powerUps;

    public int Bid => bid;
    public ulong ID => id;
    //dodawanie pieniędzy
    public void AddMoney(int balance)
    {
        if (balance >= 0)
        {
            money += balance; //dodawanie pieniędzy
        }
        else
        {
            Debug.LogWarning("Nie można dodać ujemnej kwotry."); //wyświetlanie informacji że nie można dodać ujemnej kwoty
        }
    }
    //odejmowanie pieniędzy
    public void SubstractMoney(int balance)
    {
        if (balance >= 0)
        {
            money -= balance; //odjemowanie pieniędzy
            if (money <= 0)
            {
                money = 0; //jeżeli pieniądze po odjęciu zejdą poniżej zera ustawiam pieniądzeNaKoncie na 0
            }
        }
        else
        {
            Debug.LogWarning("Nie można odjąć ujemnej kwoty."); //wyświetlanie informacji że nie można odjąć ujemnej kwoty
        }
    }


    public void Raise_Bid(int amount)
    {

        bid += amount;
        SubstractMoney(amount);
    }

    public  Team(string name, ulong id) 
    {
        this.name = name;
        this.id = id;
    }

    public Team(string name, ulong id, int money, int bid, int clues_used, int inactive_rounds, int powerups)
      {
        this.name = name;
        this.id = id;
        this.money = money;
        this.bid = bid;
        this.cluesUsed = cluesUsed;
    }

    public string Serialize() 
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(name);
        sb.Append(";");
        sb.Append(id );
        sb.Append(";");
        sb.Append(money);
        sb.Append(";");
        sb.Append(bid);
         sb.Append(";");
        sb.Append(cluesUsed);
        sb.Append(";");
        sb.Append(inactiveRounds);
        sb.Append(";");
        sb.Append(powerUps);
        sb.Append(";");


        return sb.ToString();
    }
    public static Team Deserialize(string source) 
    {
        //vars to find (do add them here (and to the Serialize() method), should you add any)
        string name="";

        ulong id = 0;
        int money = 10000;
        int bid = 0;
        int cluesUsed = 0;
        int inactiveRounds = 0; 
        int powerups=0;
        //vars for deserialization purposes. i is self explanatory, vari (var index) is for checking which variable we're writing to, and var is for the variable itself
        int i = 0;
        int vari = 0;
        StringBuilder var = new StringBuilder();
        while (i < source.Length) 
        {
            if (source[i] == ';')
            {
                //is this an elegant solution? Not really
                //can I think of a better one? Not really
                switch (vari)
                {
                    case 0:
                        name = var.ToString();
                        var.Clear();
                        break;
                    case 1:
                         
                        id = ulong.Parse(var.ToString());
                        break;
                    case 2:
                        money = int.Parse(var.ToString());
                        break;
                    case 3:
                        bid = int.Parse(var.ToString());
                        break;
                    case 4:
                        cluesUsed =   int.Parse(var.ToString());
                        break;
                    case 5:
                        inactiveRounds = int.Parse(var.ToString());
                        break;
                    case 6:
                        powerups = int.Parse(var.ToString());
                        break;
                }

                vari += 1;
            }
            else 
            {
                var.Append(source[i]);

            }
            i += 1;
        }
        return new Team(name,id, money, bid, cluesUsed, inactiveRounds,powerups );
 }


    //dodawanie podpowiedzi
    public void UseClue()
    {
        cluesUsed++; //dodawanie podpowiedzi
    }
    //zwiększanie czasu bierności w licytacji
    public void AddInactiveRounds()
    {
        inactiveRounds++; //dodaje czas bierności w licytacji
    }
    //resetowanie czasu bierności w licytacji
    public void ResetInactiveRounds()
    {
        inactiveRounds = 0; // resetowanie (zerowanie) czasu bierności w licytacji
    }
    //dodawanie powerUpa
    public void AddPowerUp(int powerUp)
    {
        if((powerUp&powerUps) == 0) {
            powerUps += powerUp;
        }

         //dodawnie powerUpa do listy powerUpów
    }
    //wykorzystywanie powerUpa
    public bool UsePowerUp(int powerUp)
    {
         
            if ((powerUps& powerUp)!=0) //znajdywanie powerUpa
            {
                powerUps-= powerUp; //usuwanie powerUpa
                return true; //użycie powerUpa
            }
         
        Debug.LogWarning("Nie posiadasz takiego powerUpa");
        return false;
    }
}