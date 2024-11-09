using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

public class Team
{
    //zadeklarowanie nazwa zmiennych
    private string name;
    private int money;
    private int cluesUsed;
    private int inactiveRounds = 0; //licznik rund bierności w licytacji
    private List<string> powerUps;
    //możliwość odczytania danych
    public string Names => name;
    public int Money => money;
    public int CluesUsed => cluesUsed;
    public int InactiveRounds => inactiveRounds;
    public List<string> PowerUps => powerUps;
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
    public void AddPowerUp(string powerUp)
    {
        powerUps.Add(powerUp); //dodawnie powerUpa do listy powerUpów
    }
    //wykorzystywanie powerUpa
    public bool UsePowerUp(string powerUp)
    {   
        for (int i = 0; i < PowerUps.Count; i++)
        {
            if (PowerUps[i] == powerUp) //znajdywanie powerUpa
            {
                PowerUps.RemoveAt(i); //usuwanie powerUpa
                return true; //użycie powerUpa
            }
        }
        Debug.LogWarning("Nie posiadasz takiego powerUpa");
        return false;
    }
}