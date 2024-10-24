using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static tjuvochpolis4.Person;

namespace tjuvochpolis4
{
    // Hanterar staden och alla personer i den
    public class City
    {
        private const int Width = 100;  // Stadens bredd
        private const int Height = 25;  // Stadens höjd

        public List<Citizen> citizens = new List<Citizen>();  // Lista över alla medborgare
        public List<Thief> thieves = new List<Thief>();  // Lista över alla tjuvar
        public List<Police> policeOfficers = new List<Police>();  // Lista över alla poliser

        public int robbedCitizens = 0;  // Räknare för rånade medborgare
        public int caughtThieves = 0;  // Räknare för gripna tjuvar

        private Random rand = new Random();  // Slumpgenerator för att bestämma riktningar

        // Initiera staden med ett visst antal medborgare, tjuvar och poliser
        public City(int citizenCount, int thiefCount, int policeCount)
        {
            // Skapa medborgare
            for (int i = 0; i < citizenCount; i++)
            {
                citizens.Add(CreatePerson<Citizen>(Width, Height));
            }

            // Skapa tjuvar
            for (int i = 0; i < thiefCount; i++)
            {
                thieves.Add(CreatePerson<Thief>(Width, Height));
            }

            // Skapa poliser
            for (int i = 0; i < policeCount; i++)
            {
                policeOfficers.Add(CreatePerson<Police>(Width, Height));
            }
        }

        // Skapa en person av en viss typ (medborgare, tjuv eller polis)
        public T CreatePerson<T>(int width, int height) where T : Person
        {
            // Bestäm slumpmässigt om personen ska röra sig i X-led, Y-led eller diagonalt
            int directionType = rand.Next(3);
            int xDirection = 0;
            int yDirection = 0;

            if (directionType == 0)
            {
                // Rörelse endast i X-led
                xDirection = rand.Next(2) == 0 ? 1 : -1;
                yDirection = 0;
            }
            else if (directionType == 1)
            {
                // Rörelse endast i Y-led
                xDirection = 0;
                yDirection = rand.Next(2) == 0 ? 1 : -1;
            }
            else
            {
                // Diagonal rörelse
                xDirection = rand.Next(2) == 0 ? 1 : -1;
                yDirection = rand.Next(2) == 0 ? 1 : -1;
            }


            return (T)Activator.CreateInstance(typeof(T), new object[] { rand.Next(width), rand.Next(height), xDirection, yDirection });
        }

        // Kör simuleringen och låt den uppdateras kontinuerligt
        public void RunSimulation()
        {
            while (true)
            {
                MovePeople();  // Flytta alla personer
                CheckInteractions();  // Kontrollera om några interaktioner sker
                UpdatePositions();  // Uppdatera positioner och status på skärmen
                System.Threading.Thread.Sleep(700);  // Pausa så att simuleringen inte går för fort
            }
        }

        // Flytta alla personer i staden
        public void MovePeople()
        {
            foreach (var citizen in citizens) citizen.Move(Width, Height);
            foreach (var thief in thieves) thief.Move(Width, Height);
            foreach (var police in policeOfficers) police.Move(Width, Height);
        }

        // Kontrollera om två personer är nära varandra
        public bool AreClose(Person a, Person b)
        {
            return Math.Abs(a.X - b.X) <= 1 && Math.Abs(a.Y - b.Y) <= 1;
        }

        // Kontrollera interaktioner mellan medborgare, tjuvar och poliser
        public void CheckInteractions()
        {
            List<Thief> thievesToRemove = new List<Thief>();
            List<Citizen> citizensToRemove = new List<Citizen>();

            // Kontrollera interaktion mellan tjuvar och medborgare
            foreach (var thief in thieves)
            {
                foreach (var citizen in citizens)
                {
                    if (AreClose(thief, citizen) && citizen.Inventory.Count > 0)
                    {
                        // Tjuven rånar medborgaren och tar ett föremål
                        string stolenItem = citizen.Inventory[0];  // Ta första objektet
                        citizen.Inventory.Remove(stolenItem);
                        thief.Inventory.Add(stolenItem);
                        robbedCitizens++;
                        Console.SetCursorPosition(0, Height + 1);
                        Console.WriteLine("Tjuv rånar medborgare och tar " + stolenItem);

                        // Byt riktning
                        thief.ChangeDirection(rand);
                        citizen.ChangeDirection(rand);

                        if (citizen.Inventory.Count == 0)
                        {
                            citizensToRemove.Add(citizen);  // Ta bort medborgaren om han blivit av med alla föremål
                        }
                    }
                }

                // Kontrollera interaktion mellan tjuvar och poliser
                foreach (var police in policeOfficers)
                {
                    if (AreClose(thief, police) && thief.Inventory.Count > 0)
                    {
                        // Polisen griper tjuven och beslagtar alla föremål
                        string stolenGoods = string.Join(", ", thief.Inventory);
                        Console.SetCursorPosition(0, Height + 1);
                        Console.WriteLine("Polis griper tjuven och beslagtar stöldgodset: " + stolenGoods);

                        police.Inventory.AddRange(thief.Inventory);  // Lägg till tjuvens föremål till polisens inventarie
                        thief.Inventory.Clear();  // Töm tjuvens inventarie
                        caughtThieves++;
                        police.ChangeDirection(rand);
                        thievesToRemove.Add(thief);  // Ta bort tjuven efter gripandet
                    }
                }
            }

            // Ta bort alla tjuvar som blivit gripna
            foreach (var thief in thievesToRemove)
            {
                thieves.Remove(thief);
            }

            // Ta bort alla medborgare som blivit rånade på alla sina föremål
            foreach (var citizen in citizensToRemove)
            {
                citizens.Remove(citizen);
            }
        }

        // Uppdatera positionerna och skriv ut status på skärmen
        public void UpdatePositions()
        {
            foreach (var citizen in citizens) ClearPreviousPosition(citizen.PrevX, citizen.PrevY);
            foreach (var thief in thieves) ClearPreviousPosition(thief.PrevX, thief.PrevY);
            foreach (var police in policeOfficers) ClearPreviousPosition(police.PrevX, police.PrevY);

            foreach (var citizen in citizens) DrawPerson(citizen.X, citizen.Y, 'M', ConsoleColor.Green);
            foreach (var thief in thieves) DrawPerson(thief.X, thief.Y, 'T', ConsoleColor.Red);
            foreach (var police in policeOfficers) DrawPerson(police.X, police.Y, 'P', ConsoleColor.Blue);

            Console.SetCursorPosition(0, Height + 2);
            Console.WriteLine($"Antal rånade medborgare: {robbedCitizens}");
            Console.WriteLine($"Antal gripna tjuvar: {caughtThieves}");
        }

        // Ritar personen på skärmen
        public void DrawPerson(int x, int y, char symbol, ConsoleColor color)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = color;
            Console.Write(symbol);
            Console.ResetColor();
        }

        // Rensa föregående position på skärmen
        public void ClearPreviousPosition(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(' ');  // Rensar genom att skriva ett blanksteg
        }
    }
}
