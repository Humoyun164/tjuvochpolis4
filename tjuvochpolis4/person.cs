using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tjuvochpolis4
{
    // Basklass för alla personer i simuleringen
    public class Person
    {
        public int X { get; set; }  // Personens position i X-led
        public int Y { get; set; }  // Personens position i Y-led
        public int XDirection { get; set; }  // Rörelseriktning i X-led (kan vara 1 eller -1)
        public int YDirection { get; set; }  // Rörelseriktning i Y-led (kan vara 1 eller -1)
        public List<string> Inventory { get; set; } = new List<string>();  // Lista med personens föremål
        public int PrevX { get; set; }  // Föregående X-position, för att rensa gamla positioner på skärmen
        public int PrevY { get; set; }  // Föregående Y-position

        // sätter startposition och riktning
        public Person(int x, int y, int xDirection, int yDirection)
        {
            X = x;
            Y = y;
            XDirection = xDirection;
            YDirection = yDirection;
            PrevX = x;
            PrevY = y;
        }

        // Flytta personen i staden, med hänsyn till stadens bredd och höjd
        public virtual void Move(int width, int height)
        {
            PrevX = X;  // Spara tidigare position för att kunna rensa den senare
            PrevY = Y;
            X = (X + XDirection + width) % width;  // Flytta i X-led och se till att positionen stannar inom gränserna
            Y = (Y + YDirection + height) % height;  // Flytta i Y-led
        }

        // Byt riktning slumpmässigt
        public void ChangeDirection(Random rand)
        {
            XDirection = rand.Next(2) == 0 ? 1 : -1;  // Ny riktning i X-led
            YDirection = rand.Next(2) == 0 ? 1 : -1;  // Ny riktning i Y-led
        }
        //medborgare, subklass av Person
        public class Citizen : Person
        {
            // Initiera medborgaren med föremål i inventory
            public Citizen(int x, int y, int xDirection, int yDirection)
                : base(x, y, xDirection, yDirection)
            {
                // Ge medborgaren några föremål att bli rånad på
                Inventory.AddRange(new[] { "Nycklar", "Mobiltelefon", "Pengar", "Klocka" });
            }
        }
        //tjuv, subklass av Person
        public class Thief : Person
        {
            // Initiera tjuven med position och riktning
            public Thief(int x, int y, int xDirection, int yDirection)
                : base(x, y, xDirection, yDirection) { }
        }
        //polis, subklass av Person
        public class Police : Person
        {
            // Initiera polisen med position och riktning
            public Police(int x, int y, int xDirection, int yDirection)
                : base(x, y, xDirection, yDirection) { }
        }

    }
}
