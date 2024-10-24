namespace tjuvochpolis4
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            // börja staden med 10 tjuvar, 20 medborgare och 5 poliser
            City city = new City(20, 10, 5);
            city.RunSimulation();  // Starta simuleringen
        }
    }
}
