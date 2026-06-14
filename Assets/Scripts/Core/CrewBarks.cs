public enum CrewMember
{
    Korey,
    Eliana,
    Buford,
    Mlody
}

public enum BarkEvent
{
    SectorEnter,
    Combat,
    LowFuel,
    Debt
}

public static class CrewBarks
{
    private static readonly string[][] Barks = new string[][]
    {
        // Korey barks
        new string[] {
            "Korey: 'Scanning local signals. Keep it clean.'",
            "Korey: 'Shields are holding, but don't get cocky.'",
            "Korey: 'Fuel levels are dropping. We should top up.'",
            "Korey: 'Debts keep piling. We need more titanium.'"
        },
        // Eliana barks
        new string[] {
            "Eliana: 'Rubieze doesn't forgive mistakes. Keep eyes open.'",
            "Eliana: 'Hostile locking on! Evasive maneuvers!'",
            "Eliana: 'Energy reserves critical. Switch to eco mode!'",
            "Eliana: 'Creditors are calling. I hate the corporate sector.'"
        },
        // Buford barks
        new string[] {
            "Buford: 'Quiet sector. Too quiet for my taste.'",
            "Buford: 'Eat kinetic metal, space scum!'",
            "Buford: 'Engines are thirstier than me after a dry run.'",
            "Buford: 'We are in the red again. Time to mine heavier rocks.'"
        },
        // Mlody barks
        new string[] {
            "Młody: 'Wow, look at the dust lanes here!'",
            "Młody: 'Are we being shot at?! Help!'",
            "Młody: 'Warning light is blinking red! Fuel low!'",
            "Młody: 'If we don't pay the bank, they'll seize our ship.'"
        }
    };

    public static string GetBark(CrewMember member, BarkEvent barkEvent)
    {
        int memberIndex = (int)member;
        int eventIndex = (int)barkEvent;
        
        // Return event specific bark or fallback
        if (memberIndex >= 0 && memberIndex < Barks.Length)
        {
            var options = Barks[memberIndex];
            if (eventIndex >= 0 && eventIndex < options.Length)
            {
                return options[eventIndex];
            }
        }
        return "Crew: 'Nominal readings.'";
    }
}
