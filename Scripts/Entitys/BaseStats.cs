using Godot;
using System;
using System.Runtime.Serialization;


public class CombinedStats {
    public Stats stats;
    public Defences defences;
}

public class Stats {
    public Stat Health, Mana, Speed, Luck, Crit_Damage, Crit_Chance;
}

public class Defences {
    public Stat Armor, Barrier, Defence, Magic_Resistance, Magic_Defense;

}

public class Stat {
    public int Current;//the uise value
    public int Max; //the current high point of the value 
    public int Top; //the highest value that the value can ever reach
    public int Min; //the current low point of the value
    public int Bottom; //the lowest value that the value can ever reach
}
