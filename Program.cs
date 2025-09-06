using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

class Program
{
    

    static void Main()
    {
        string filePath = "input.csv";

        while (true)
        {
            string[] lines; //reinstantiate array size to allow for added characters 
            lines = File.ReadAllLines(filePath); //update lines each time to ensure current data

            Console.WriteLine("Menu:");
            Console.WriteLine("1. Display Characters");
            Console.WriteLine("2. Add Character");
            Console.WriteLine("3. Level Up Character");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    DisplayAllCharacters(lines);
                    break;
                case "2":
                    AddCharacter(lines);
                    break;
                case "3":
                    LevelUpCharacter(lines);
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            Console.WriteLine();
        }
    }

    static void DisplayAllCharacters(string[] lines)
    {
        // Skip the header row
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];

            string name;

            int commaIndex;

            // Check if the name is quoted
            if (line.StartsWith("\""))
            {
                // TODO: Find the closing quote and the comma right after 
                commaIndex = line.Substring(line.IndexOf(",")).IndexOf(",", 1) + (line.IndexOf(",")); //retrieves the index of the comma after the name/quotations
                // TODO: Remove quotes from the name if present and parse the name
                name = line.Substring(1, line.IndexOf('"', 1));
                // name = "John, Brave"
                // name = ...
                name = $"{name.Substring(name.IndexOf(",") + 2).Trim('"')} {name.Substring(0, name.IndexOf(","))}"; //reverse name/title and remove comma 

                
            }
            else
            {
                // TODO: Name is not quoted, so store the name up to the first comma
                // name =
                commaIndex = line.IndexOf(","); //retrieves index of comma after name
                name = line.Substring(0, commaIndex);                 
            }

            // TODO: Parse characterClass, level, hitPoints, and equipment
            // string characterClass = ...
            // int level = ...
            // int hitPoints = ...

            string[] fields = line.Substring(commaIndex+1).Split(",");
            string characterClass = fields[0];
            int level = int.Parse(fields[1]);
            int hitPoints = int.Parse(fields[2]);

            // TODO: Parse equipment noting that it contains multiple items separated by '|'
            // string[] equipment = ...
            string[] equipment = fields[3].Split('|');

            // Display character information
            Console.WriteLine($"Name: {name}, Class: {characterClass}, Level: {level}, HP: {hitPoints}, Equipment: {string.Join(", ", equipment)}");

        }
    }

    static void AddCharacter(string[] lines)
    {
        // TODO: Implement logic to add a new character
        // Prompt for character details (name, class, level, hit points, equipment)
        // DO NOT just ask the user to enter a new line of CSV data or enter the pipe-separated equipment string
        // Append the new character to the lines array
        Console.Write("Enter your character's name with any titles coming first (i.e. Sir Andrel): ");
        string name = Console.ReadLine(); //Sir Andrel
        string formattedName = "";

        if (name.Contains(" ")) //check for spaces to correctly format name
        {
            formattedName = $"\"{name.Substring(name.IndexOf(" ") + 1)}, {name.Substring(0, name.IndexOf(" "))}\"";
        }
        else
            formattedName = name;

        Console.Write("Enter your character's class: ");
        string characterClass = Console.ReadLine();

        Console.Write("Enter your character's level: ");
        int level = int.Parse(Console.ReadLine());

        Console.Write("Enter your character's Hitpoints: ");
        int hitpoints = int.Parse(Console.ReadLine());


        Console.Write("Does your character have any equipment? (Y/N): ");
        string response = Console.ReadLine();
        bool hasEquipment = yesNoValidation(ref response);        
        

        //use list for variable size inventory 
        List<string> equipment = new List<string>();

        while (hasEquipment)
        {
            Console.Write("Enter your item: ");
            equipment.Add(Console.ReadLine());

            Console.Write("Do you have more equipment? (Y/N): ");
            response = Console.ReadLine();

            hasEquipment = yesNoValidation(ref response);
        }

        Console.WriteLine($"\nWelcome, {name} the {characterClass}! You are level {level} and your equipment includes: {string.Join(", ", equipment)}.\n");
        string newCharacter = $"{formattedName},{characterClass},{level},{hitpoints},{string.Join("|", equipment)}";

        using (StreamWriter writer = new StreamWriter("input.csv", true))
        {
            writer.WriteLine(newCharacter);
        }

    }

    static void LevelUpCharacter(string[] lines)
    {
        Random rand = new Random();
        Console.Write("Enter the name of the character to level up: ");
        string nameToLevelUp = Console.ReadLine();

        // Loop through characters to find the one to level up
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] fields;
            int commaIndex;
            string name;
            string formattedName; 

            //if contains name
            if (line.Contains(nameToLevelUp))
            {
                //check for quotation handling - move into method or class in the future
                if (line.StartsWith("\""))
                {
                    commaIndex = line.Substring(line.IndexOf(",")).IndexOf(",", 1) + (line.IndexOf(",")); //retrieves the index of the comma after the name/quotations
                    name = line.Substring(1, line.IndexOf('"', 1));
                    formattedName = line.Substring(0, commaIndex);
                    name = $"{name.Substring(name.IndexOf(",") + 2).Trim('"')} {name.Substring(0, name.IndexOf(","))}"; //reverse name/title and remove comma 
                }
                //otherwise normal processing
                else
                {
                    commaIndex = line.IndexOf(","); //retrieves index of comma after name
                    formattedName = line.Substring(0, commaIndex);
                    name = line.Substring(0, commaIndex);
                }

                //split off non-name fields
                fields = line.Substring(commaIndex + 1).Split(",");

                //store and update level
                int level = int.Parse(fields[1]);
                level++;
                Console.WriteLine($"Character {name} leveled up to level {level}!");
                int hpGain = rand.Next(1, 7);
                Console.WriteLine($"They gained {hpGain} maximum hitpoints!");
                int hitpoints = int.Parse(fields[2]) + hpGain;

                //rewrite current line 
                lines[i] = $"{formattedName},{fields[0]},{level},{hitpoints},{fields[3]}";
                break; //break out of loop 
            }
        }

        //rewrite .csv file
        using (StreamWriter writer = new StreamWriter("input.csv", false))
        {
            for (int i = 0; i < lines.Length; i++)
            {
                writer.WriteLine(lines[i]);
            }
        }

    }

    static bool yesNoValidation(ref string response)
        {
            //checks if input is Y or N, else looping to get a valid response. 
            while (true)
            {
                if (response.Equals("Y", StringComparison.OrdinalIgnoreCase))
                    return true;
                else if (response.Equals("N", StringComparison.OrdinalIgnoreCase))
                    return false;
                else
                {
                    Console.Write("Invalid input. Please enter 'Y' or 'N': ");
                    response = Console.ReadLine();
                }
            }
        }
}