public class StreetFighter : Character
{
  public string[] Moves { get; set; }

  public override string Display()
  {
    return $"Id: {Id}\nName: {Name}\nDescription: {Description}\nSpecies: {Moves}\n";
  }
}