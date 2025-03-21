using NLog;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Reflection;
using System.Text.Json;
string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

// deserialize mario json from file into List<Mario>
string marioFileName = "mario.json";
string DKFileName = "dk.json";
string SFFileName = "sf2.json";
List<Mario> marios = getCharacters<Mario>(marioFileName);
List<Mario> DKCharacters = getCharacters<Mario>(DKFileName);
List<Mario> SFCharacters = getCharacters<Mario>(SFFileName);

//Helper function to grab characters from files.
List<T> getCharacters<T>(string fileName)
{
  List<T> characters = [];
  // check if file exists
  if (File.Exists(fileName))
  {
    characters = JsonSerializer.Deserialize<List<T>>(File.ReadAllText(fileName))!;
    logger.Info($"File deserialized {fileName}");
    return characters;
  }
  else return characters;
}

//Helper function to add characters to appropriete file
void addCharacter(Character character, string fileName)
{
  // Add Character
  Type type = character.GetType();
  if (type == typeof(Mario))
  {
    Mario mario = new();
    mario.Id = marios.Count == 0 ? 1 : marios.Max(c => c.Id) + 1;
    InputCharacter(mario);

    // Add Character
    marios.Add(mario);
    File.WriteAllText(marioFileName, JsonSerializer.Serialize(marios));
    logger.Info($"Character added: {mario.Name}");
  }
  if (type == typeof(StreetFighter))
  {
    //TODO: add to SF file
  }
  if (type == typeof(DonkeyKong))
  {
    //TODO: add to DK file
  }
}


do
{
  // display choices to user
  Console.WriteLine("1) Display Mario Characters");
  Console.WriteLine("2) Add Mario Character");
  Console.WriteLine("3) Remove Mario Character");
  Console.WriteLine("Enter to quit");

  // input selection
  string? choice = Console.ReadLine();
  logger.Info("User choice: {Choice}", choice);

  if (choice == "1")
  {
    Console.WriteLine("\nFrom which game do you want?");
    Console.WriteLine("1) Mario");
    Console.WriteLine("2) Donkey Kong");
    Console.WriteLine("3) Street Figher");
    choice = Console.ReadLine();
    // Display Mario Characters
    if (choice == "1")
    {
      foreach (var c in marios)
      {
        Console.WriteLine(c.Display());
      }
    }
    if (choice == "2")
    {
      foreach (var c in DKCharacters)
      {
        Console.WriteLine(c.Display());
      }
    }
    if (choice == "3")
    {
      foreach (var c in SFCharacters)
      {
        Console.WriteLine(c.Display());
      }
    }
  }
  else if (choice == "2")
  {
    addCharacter(new Mario(), marioFileName);
  }
  else if (choice == "3")
  {
    // Remove Mario Character
    Console.WriteLine("Enter the Id of the character to remove:");
    if (UInt32.TryParse(Console.ReadLine(), out UInt32 Id))
    {
      Mario? character = marios.FirstOrDefault(c => c.Id == Id);
      if (character == null)
      {
        logger.Error($"Character Id {Id} not found");
      }
      else
      {
        marios.Remove(character);
        // serialize list<marioCharacter> into json file
        File.WriteAllText(marioFileName, JsonSerializer.Serialize(marios));
        logger.Info($"Character Id {Id} removed");
      }
    }
    else
    {
      logger.Error("Invalid Id");
    }
  }
  else if (string.IsNullOrEmpty(choice))
  {
    break;
  }
  else
  {
    logger.Info("Invalid choice");
  }
} while (true);

logger.Info("Program ended");

static void InputCharacter(Character character)
{
  Type type = character.GetType();
  PropertyInfo[] properties = type.GetProperties();
  var props = properties.Where(p => p.Name != "Id");
  foreach (PropertyInfo prop in props)
  {
    if (prop.PropertyType == typeof(string))
    {
      Console.WriteLine($"Enter {prop.Name}:");
      prop.SetValue(character, Console.ReadLine());
    }
    else if (prop.PropertyType == typeof(List<string>))
    {
      List<string> list = [];
      do
      {
        Console.WriteLine($"Enter {prop.Name} or (enter) to quit:");
        string response = Console.ReadLine()!;
        if (string.IsNullOrEmpty(response))
        {
          break;
        }
        list.Add(response);
      } while (true);
      prop.SetValue(character, list);
    }
  }
}