using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public static class DebugLogger
{
    private static string filePathDeathData;
    private static string filePathPopulation;
    private static string filePathFoodPopulation;

    public static void setLogPath()
    {
        if(filePathDeathData == null)   // TODO: This is just a quickfix, we're gonna need sg better than this --> SINGLETON-sg?
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string dataDirectory = Path.Combine(projectRoot, ".logs");

            Directory.CreateDirectory(dataDirectory);
            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            filePathDeathData = Path.Combine(dataDirectory, $"{timestamp}_DeathData.csv");
            filePathPopulation = Path.Combine(dataDirectory, $"{timestamp}_PopulationData.csv");
            filePathFoodPopulation = Path.Combine(dataDirectory, $"{timestamp}_FoodPopulationData.csv");

            using (StreamWriter writer = new StreamWriter(filePathDeathData))
            {
                writer.WriteLine("Step;Species;DeathCause;Age;Speed;Sight;ReproductiveUrge;LifeSpan;Charm;PregnancyDuration;Status;Starving;Drying");
            }

            using (StreamWriter writer = new StreamWriter(filePathPopulation))
            {
                writer.WriteLine("Time;BunnyPop;FoxPop");
            }

            using (StreamWriter writer = new StreamWriter(filePathFoodPopulation))
            {
                writer.WriteLine("Time;Food");
            }

            Info(filePathPopulation);
            Info(filePathFoodPopulation);
            Info(filePathDeathData);
        }
    }
    public enum LogLevel { Off, Error, Warn, Notice, Info }
    public static LogLevel CurrentLevel = LogLevel.Info;
    public static void Log(string msg, string level_str, LogLevel level = LogLevel.Info)
    {
        if (level <= CurrentLevel)
        {
            string color = GetColorForLevel(level);
            string coloredMessage = $"<color={color}>{level_str + msg}</color>";
            Debug.Log(coloredMessage);
        }
    }
    public static void Error(string message) => Log(msg: message, level_str: "Error: ", level: LogLevel.Error);
    public static void Warning(string message) => Log(msg: message, level_str: "Warning: ", level: LogLevel.Warn);
    public static void Info(string message) => Log(msg: message, level_str: "Notice: ", level: LogLevel.Info);
    public static void Notice(string message) => Log(msg: message, level_str: "Info: ", level: LogLevel.Notice);
    private static string GetColorForLevel(LogLevel level)
    {
        switch (level)
        {
            case LogLevel.Error: return "#FF5151";
            case LogLevel.Warn: return "#FFA300";
            case LogLevel.Notice: return "#00AEFF";
            case LogLevel.Info: return "white";
            default: return "white";
        }
    }
    public static void RegisterPopulation(int step, int counterBunny, int counterFox)
    {
        using (StreamWriter writer = new StreamWriter(filePathPopulation, true))
        {
            writer.WriteLine($"{step};{counterBunny};{counterFox}");
        }
    }
    public static void RegisterFood(int cnt, int step = 1)
    {
        using (StreamWriter writer = new StreamWriter(filePathFoodPopulation, true))
        {
            writer.WriteLine($"{step};{cnt}");
        }
    }

    public static void RegisterDeath(int step, Animal animal)
    {
        using (StreamWriter writer = new StreamWriter(filePathDeathData, true))
        {
            string dataLine = $"{step};" +
                $"{animal.species.ToPrint()};" +
                $"{animal.cause.ToPrint()};" +
                $"{animal.aging.currentAge}" +
                $";{animal.movement.moveSpeed};" +
                $"{animal.sensor.radius};" +
                $"{animal.reproduction.reproductiveUrge};" +
                $"{animal.aging.lifeSpan};{animal.mating.charm};" +
                $"{animal.reproduction.pregnancyDuration};" +
                $"{animal.prevStatus};" +
                $"{animal.eat.critical};" +
                $"{animal.drink.critical}";
            writer.WriteLine(dataLine);
        }
    }
}
