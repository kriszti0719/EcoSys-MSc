using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Globalization;


public static class DebugLogger
{
    private static string filePathDeath;
    private static string filePathPopulation;
    private static string filePathFood;
    private static string filePathUtility;
    private static string filePathFCM;
    private static string timestamp;

    public static void setLogPath()
    {
        if(filePathDeath == null)   // TODO: This is just a quickfix, we're gonna need sg better than this --> SINGLETON-sg?
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string dataDirectory = Path.Combine(projectRoot, ".logs");

            Directory.CreateDirectory(dataDirectory);
            timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            filePathDeath = Path.Combine(dataDirectory, $"{timestamp}_DeathData.csv");
            filePathPopulation = Path.Combine(dataDirectory, $"{timestamp}_PopulationData.csv");
            filePathFood = Path.Combine(dataDirectory, $"{timestamp}_FoodData.csv");
            
            using (StreamWriter writer = new StreamWriter(filePathDeath))
            {
                writer.WriteLine("Step;Name;Generation;Species;IsMale;DeathCause;Age;Speed;Sight;LifeSpan;Charm;PregnancyDuration;Status;Starving;Drying;triedForBaby;GaveBirth;Kids");
            }
            using (StreamWriter writer = new StreamWriter(filePathPopulation))
            {
                writer.WriteLine("Time;BunnyPop;FoxPop");
            }
            using (StreamWriter writer = new StreamWriter(filePathFood))
            {
                writer.WriteLine("Time;Food");
            }

            Info(filePathPopulation);
            Info(filePathFood);
            Info(filePathDeath);

            if (DecisionController.GlobalMode == DecisionMode.Utility)
            {
                filePathUtility = Path.Combine(dataDirectory, $"{timestamp}_Utility.csv");
                using (StreamWriter writer = new StreamWriter(filePathUtility))
                {
                    writer.WriteLine("Animal;Generation;Hunger;Thirst;Energy;Mate");
                }
                Info(filePathUtility);
            }

            if (DecisionController.GlobalMode == DecisionMode.FCM)
            {
                filePathFCM = Path.Combine(dataDirectory, $"{timestamp}_FCM.csv");
                using (StreamWriter writer = new StreamWriter(filePathFCM))
                {
                    writer.WriteLine("Animal;Generation;Weights");
                }
                Info(filePathFCM);
            }
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
    public static void ShowNotification(string message) => Notifier.Show( message );
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
    public static void RegisterPopulationNumbers(int step, int counterBunny, int counterFox)
    {
        using (StreamWriter writer = new StreamWriter(filePathPopulation, true))
        {
            writer.WriteLine($"{step};{counterBunny};{counterFox}");
        }
    }
    public static void RegisterFood(int cnt, int step = 1)
    {
        using (StreamWriter writer = new StreamWriter(filePathFood, true))
        {
            writer.WriteLine($"{step};{cnt}");
        }
    }
    public static void RegisterDeath(int step, Animal animal, bool influxDB)
    {
        using (StreamWriter writer = new StreamWriter(filePathDeath, true))
        {
            string dataLine = $"{step};" +
                              $"{animal.name};" +
                              $"{animal.generation};" + 
                              $"{animal.species.ToString()};" + 
                              $"{animal.isMale};" + 
                              $"{animal.cause.ToString()};" + 
                              $"{animal.aging.currentAge};" + 
                              $"{animal.movement.moveSpeed};" + 
                              $"{animal.sensor.radius};" + 
                              $"{animal.aging.lifeSpan};" + 
                              $"{animal.mating.charm};" + 
                              $"{animal.reproduction.pregnancyDuration};" + 
                              $"{animal.prevStatus};" + 
                              $"{animal.eat.critical};" + 
                              $"{animal.drink.critical};" + 
                              $"{animal.triedForBaby};" + 
                              $"{animal.gaveBirth};" + 
                              $"{animal.kids}"
                              ;
            writer.WriteLine(dataLine);
            
            if(influxDB) RegisterDeathToDb(step, animal);
        }

        if (DecisionController.GlobalMode == DecisionMode.Utility)
        {
            using (StreamWriter wirter = new StreamWriter(filePathUtility, true))
            {
                string dataLine = $"{animal.name};" +
                                  $"{animal.generation};" +
                                  $"{animal.utilityGenome.hungerWeight};" +
                                  $"{animal.utilityGenome.thirstWeight};" +
                                  $"{animal.utilityGenome.energyWeight};" +
                                  $"{animal.utilityGenome.mateWeight}"
                                  ;
                wirter.WriteLine(dataLine);
            }
        }

        if (DecisionController.GlobalMode == DecisionMode.FCM)
        {
            using (StreamWriter wirter = new StreamWriter(filePathFCM, true))
            {
                string dataLine = $"{animal.name};" +
                                  $"{animal.generation};" +
                                  $"{string.Join(",", animal.fcmGenome.weights)}"
                                  ;
                wirter.WriteLine(dataLine);
            }
        }

    }
    public static void RegisterDeathToDb(int step, Animal animal)
    {
        string F(float v) => v.ToString(CultureInfo.InvariantCulture);

        StringBuilder sb = new StringBuilder();
    
        sb.Append($"death,run_id={timestamp},ai_mode={DecisionController.GlobalMode.ToString()},species={animal.species.ToString()},cause={animal.cause.ToString()} ");
        sb.Append($"age={F(animal.aging.currentAge)},");
        sb.Append($"speed={F(animal.movement.moveSpeed)},");
        sb.Append($"sight={F(animal.sensor.radius)},");
        sb.Append($"lifeSpan={F(animal.aging.lifeSpan)},");
        sb.Append($"charm={F(animal.mating.charm)},");
        sb.Append($"pregnancyDuration={F(animal.reproduction.pregnancyDuration)},");
        sb.Append($"starving={F(animal.eat.critical)},");
        sb.Append($"drying={F(animal.drink.critical)},");
        sb.Append($"triedForBaby={(animal.triedForBaby)},");
        sb.Append($"gaveBirth={(animal.gaveBirth)},");
        sb.Append($"kids={animal.kids},");
        sb.Append($"generation={animal.generation},");  
        sb.Append($"step={step}");

        if (DecisionController.GlobalMode == DecisionMode.Utility && animal.utilityGenome != null)
        {
            sb.Append($",hungerWeight={F(animal.utilityGenome.hungerWeight)}");
            sb.Append($",thirstWeight={F(animal.utilityGenome.thirstWeight)}");
            sb.Append($",energyWeight={F(animal.utilityGenome.energyWeight)}");
            sb.Append($",mateWeight={F(animal.utilityGenome.mateWeight)}");
        }
        else if (DecisionController.GlobalMode == DecisionMode.FCM && animal.fcmGenome != null)
        {
            for (int i = 0; i < animal.fcmGenome.weights.Length; i++)
            {
                sb.Append($",fcm_w_{i}={F(animal.fcmGenome.weights[i])}");
            }
        }

        string line = sb.ToString();

        InfluxLogger.Log(line);
        Info(line);
    } 
    public static void RegisterSnapshotToDb(int step, List<Animal> livingAnimals) 
    {
        string F(float v) => v.ToString(CultureInfo.InvariantCulture);
        
        var bunnies = livingAnimals.Where(a => a.species == Species.BUNNY).ToList();
        var foxes = livingAnimals.Where(a => a.species == Species.FOX).ToList();

        StringBuilder sb = new StringBuilder();
        
        sb.Append($"snapshot,run_id={timestamp},ai_mode={DecisionController.GlobalMode.ToString()} ");
        sb.Append($"bunny_count={bunnies.Count},");
        sb.Append($"fox_count={foxes.Count},");
        sb.Append($"step={step}");

        foreach (var stateGroup in bunnies.GroupBy(b => b.prevStatus.ToString()))
        {
            sb.Append($",bunny_state_{stateGroup.Key}={stateGroup.Count()}");
        }
        foreach (var stateGroup in foxes.GroupBy(f => f.prevStatus.ToString()))
        {
            sb.Append($",fox_state_{stateGroup.Key}={stateGroup.Count()}");
        }

        if (bunnies.Count > 0)
        {
            sb.Append($",bunny_avg_age={F(bunnies.Average(b => b.aging.currentAge))}");
            sb.Append($",bunny_avg_speed={F(bunnies.Average(b => b.movement.moveSpeed))}");
            sb.Append($",bunny_max_generation={bunnies.Max(b => b.generation)}");

            if (DecisionController.GlobalMode == DecisionMode.Utility)
            {
                sb.Append($",bunny_avg_w_hunger={F(bunnies.Average(b => b.utilityGenome?.hungerWeight ?? 0))}");
                sb.Append($",bunny_avg_w_thirst={F(bunnies.Average(b => b.utilityGenome?.thirstWeight ?? 0))}");
                sb.Append($",bunny_avg_w_energy={F(bunnies.Average(b => b.utilityGenome?.energyWeight ?? 0))}");
                sb.Append($",bunny_avg_w_mate={F(bunnies.Average(b => b.utilityGenome?.mateWeight ?? 0))}");
            }
            else if (DecisionController.GlobalMode == DecisionMode.FCM && bunnies.Any(b => b.fcmGenome != null))
            {
                int linkCount = bunnies.First(b => b.fcmGenome != null).fcmGenome.weights.Length;
                for (int i = 0; i < linkCount; i++)
                {
                    int index = i;
                    float avgWeight = bunnies.Average(b => b.fcmGenome.weights[index]);
                    sb.Append($",bunny_avg_fcm_w_{index}={F(avgWeight)}");
                }
            }
        }

        if (foxes.Count > 0)
        {
            sb.Append($",fox_avg_age={F(foxes.Average(f => f.aging.currentAge))}");
            sb.Append($",fox_avg_speed={F(foxes.Average(f => f.movement.moveSpeed))}");
            sb.Append($",fox_max_generation={foxes.Max(f => f.generation)}");

            if (DecisionController.GlobalMode == DecisionMode.Utility)
            {
                sb.Append($",fox_avg_w_hunger={F(foxes.Average(f => f.utilityGenome?.hungerWeight ?? 0))}");
                sb.Append($",fox_avg_w_thirst={F(foxes.Average(f => f.utilityGenome?.thirstWeight ?? 0))}");
                sb.Append($",fox_avg_w_energy={F(foxes.Average(f => f.utilityGenome?.energyWeight ?? 0))}");
                sb.Append($",fox_avg_w_mate={F(foxes.Average(f => f.utilityGenome?.mateWeight ?? 0))}");
            }
            else if (DecisionController.GlobalMode == DecisionMode.FCM && foxes.Any(f => f.fcmGenome != null))
            {
                int linkCount = foxes.First(f => f.fcmGenome != null).fcmGenome.weights.Length;
                for (int i = 0; i < linkCount; i++)
                {
                    int index = i;
                    float avgWeight = foxes.Average(f => f.fcmGenome.weights[index]);
                    sb.Append($",fox_avg_fcm_w_{index}={F(avgWeight)}");
                }
            }
        }

        string line = sb.ToString();
        InfluxLogger.Log(line);
    }
}


public static class Notifier
{
    public static void Show(string message)
    {
        System.Diagnostics.Process.Start("powershell",
            $"-Command \"Add-Type -AssemblyName PresentationFramework;[System.Windows.MessageBox]::Show('{message}')\"");
    }
}