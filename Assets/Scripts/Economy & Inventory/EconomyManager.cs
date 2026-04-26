using System;
using UnityEngine;

public class EconomyManager : MonoBehaviour {

    // Singleton, bo bêdzie tylko jedna taka klasa
    public static EconomyManager Instance { get; private set; }

    // G³ówna zmienna, mo¿na j¹ bez przypa³u pobraæ, ale nie ustawiaæ
    public int Credits { get; private set; }


    // Tak samo to ¿eby by³ Singleton
    private void Awake() {
        Instance = this;
    }

    // Tworzenie kodów do konsoli deweloperskiej
    private void Start() {
        DeveloperConsole.Instance.AddCommand("set_credits", SetMoneyCommand);
        DeveloperConsole.Instance.AddCommand("get_credits", GetMoneyCommand);
    }


    // Dodawanie Kredytów
    public void AddCredits(int amount) {
        // Mo¿na dodaæ tylko wiêcej ni¿ 0 Kredytów
        if (amount < 0) {
            Debug.Log("Nie mo¿esz dodaæ mniej ni¿ 0");
        } else {
            Credits += amount;
            Debug.Log("Poprawnie dodano " + amount + " kredytów");
        }
    }

    // Wydawanie Kredytów
    public void SpendCredits(int amount) {
        // Nie mo¿na wydaæ wiêcej ni¿ mamy na koncie, oraz nie mo¿na wydaæ mniej ni¿ 0
        if (Credits < amount || amount < 0) {
            Debug.Log("Nie mo¿esz wydaæ takiej kwoty");
        } else {
            Credits -= amount;
            Debug.Log("Poprawnie pobrano " + amount + " kredytów");
        }
    }

    // Ustawianie Kredytów dla konsoli, chyba sie przyda pod LoadGame
    public void SetCredits(int amount) {
        Credits = amount;
        Debug.Log("Poprawnie przypisano " + amount + " kredytów");
    }

    // Komenda do ustawiania iloœci Kredytów
    public void SetMoneyCommand(string[] args) {
        // Sprawdzanie czy wpisano coœ po s³owach kluczowych
        if (args.Length > 0) {
            int amount = 0;
            // Parsowanie ze stringa na inta, jak nie jest liczba po s³owie kluczowym, idzie do else
            if (Int32.TryParse(args[0], out amount)) {
                SetCredits(amount);
            } else {
                Debug.Log("Coœ posz³o nie tak, Ÿle wpisa³eœ komende");
            }
        }  
    }


    // Komenda do wyœwietlania iloœci Kredytów
    public void GetMoneyCommand(string[] args) {
        Debug.Log("Aktualny stan konta wynosi: " + Credits + " kredytów");
    }
}