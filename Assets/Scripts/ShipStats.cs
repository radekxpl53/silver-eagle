using System;
using UnityEngine;
using UnityEngine.Events;

public class ShipStats : MonoBehaviour {
    public float CurrentHP { get; private set; }
    public float CurrentEnergy { get; private set; }
    public float CurrentCargo { get; private set; }
    [SerializeField] private float MaxHP;
    [SerializeField] private float MaxEnergy;
    [SerializeField] private float MaxCargo;
    [SerializeField] private float BaseMass;

    [Header("--- SMIERĆ STATKU ---")]
    public UnityEvent OnDestroyed;                    // ← podłącz tutaj okno porażki
    public bool IsDestroyed { get; private set; }

    [Header("--- SKRYPTY STERUJĄCE DO ZABLOKOWANIA ---")]
    [SerializeField] private MonoBehaviour[] controlScriptsToDisable;

    public void Start() {
        CurrentHP = MaxHP;
        CurrentEnergy = MaxEnergy;
        DeveloperConsole.Instance.AddCommand("set_hp", SetHPCommand);
        DeveloperConsole.Instance.AddCommand("set_max_hp", SetMaxHPCommand);
        DeveloperConsole.Instance.AddCommand("set_energy", SetEnergyCommand);
        DeveloperConsole.Instance.AddCommand("set_max_energy", SetMaxEnergyCommand);
        DeveloperConsole.Instance.AddCommand("get_hp", GetHPCommand);
        DeveloperConsole.Instance.AddCommand("get_energy", GetEnergyCommand);
    }

    public void TakeDamage(float damage)
    {
        if (damage > 0f)
        {
            CurrentHP -= damage;
            if (CurrentHP <= 0)
            {
                CurrentHP = 0;
                if (!IsDestroyed)
                {
                    IsDestroyed = true;
                    HandleDestruction();
                }
                Debug.Log("Statek zniszczony!");
            }
            Debug.Log("Ustawiono wartość HP na: " + CurrentHP);
        }
        else
        {
            Debug.Log("Nie możesz zadać statkowi mniej niż 0 dmg");
        }
    }

    private void HandleDestruction()
    {
        Debug.Log("<color=red>STATEK ZNISZCZONY!</color>");

        // 1. Blokada sterowania
        foreach (var script in controlScriptsToDisable)
        {
            if (script != null) script.enabled = false;
        }

        // 2. Globalny event (okno porażki)
        OnDestroyed?.Invoke();
    }

    public void Heal(float amount) {
        if (amount > 0f) {
            if (CurrentHP + amount > MaxHP) {
                CurrentHP = MaxHP;
                Debug.Log("Statek naprawiony!");
            }
            else {
                CurrentHP += amount;
            }
            Debug.Log("Ustawiono wartość HP na: " + CurrentHP);
        }
        else {
            Debug.Log("Nie możesz uleczyć statku za mniej niż 0HP");
        }
    }

    public void UseEnergy(float amount) {
        if (amount > 0f) {
            if (CurrentEnergy < amount) {
                CurrentEnergy = 0;
                Debug.Log("Statek nie ma paliwa!");
            }
            else {
                CurrentEnergy -= amount;
            }
            Debug.Log("Ustawiono wartość Paliwa na: " + CurrentEnergy);
        }
        else {
            Debug.Log("Nie możesz spalić mniej niż 0 jednostek paliwa");
        }
    }

    public void AddEnergy(float amount) {
        if (amount > 0f) {
            if (CurrentEnergy + amount > MaxEnergy) {
                CurrentEnergy = MaxEnergy;
                Debug.Log("Statek zatankowany na full!");
            }
            else {
                CurrentEnergy += amount;
            }
            Debug.Log("Ustawiono wartość Paliwa na: " + CurrentEnergy);
        }
        else {
            Debug.Log("Nie możesz zatankować statku za mniej niż 0 jednostek paliwowych");
        }
    }

    public bool AddCargo(float amount) {
        if (amount > 0) {
            if (CurrentCargo + amount > MaxCargo) {
                return false;
            } else {
                CurrentCargo += amount;
                return true;
            }
        } else {
            return false;
        }
    }

    public float GetTotalMass() {
        return BaseMass + CurrentCargo;
    }

    public void SetHP(float amount) {
        CurrentHP = amount;
        Debug.Log("Poprawnie przypisano " + amount + " HP");
    }
    public void SetMaxHP(float amount) {
        MaxHP = amount;
        Debug.Log("Poprawnie przypisano " + amount + " MaxHP");
    }
    public void SetEnergy(float amount) {
        CurrentEnergy = amount;
        Debug.Log("Poprawnie przypisano " + amount + " Paliwa");
    }
    public void SetMaxEnergy(float amount) {
        MaxEnergy = amount;
        Debug.Log("Poprawnie przypisano " + amount + " MaxPaliwa");
    }
    

    public void SetHPCommand(string[] args) {
        if (args.Length > 0) {
            int amount = 0;
            // Parsowanie ze stringa na inta, jak nie jest liczba po słowie kluczowym, idzie do else
            if (Int32.TryParse(args[0], out amount)) {
                SetHP(amount);
            }
            else {
                Debug.Log("Coś poszło nie tak, źle wpisałeś komende");
            }
        }
    }
    public void SetMaxHPCommand(string[] args) {
        if (args.Length > 0) {
            int amount = 0;
            if (Int32.TryParse(args[0], out amount)) {
                SetMaxHP(amount);
            }
            else {
                Debug.Log("Coś poszło nie tak, źle wpisałeś komende");
            }
        }
    }
    public void SetEnergyCommand(string[] args) {
        if (args.Length > 0) {
            int amount = 0;
            if (Int32.TryParse(args[0], out amount)) {
                SetEnergy(amount);
            }
            else {
                Debug.Log("Coś poszło nie tak, źle wpisałeś komende");
            }
        }
    }
    public void SetMaxEnergyCommand(string[] args) {
        if (args.Length > 0) {
            int amount = 0;
            if (Int32.TryParse(args[0], out amount)) {
                SetMaxEnergy(amount);
            }
            else {
                Debug.Log("Coś poszło nie tak, źle wpisałeś komende");
            }
        }
    }
    public void GetHPCommand(string[] args) {
        Debug.Log("Aktualny stan HP wynosi: " + CurrentHP + "/" + MaxHP);
    }
    public void GetEnergyCommand(string[] args) {
        Debug.Log("Aktualny stan paliwa wynosi: " + CurrentEnergy + "/" + MaxEnergy);
    }
}