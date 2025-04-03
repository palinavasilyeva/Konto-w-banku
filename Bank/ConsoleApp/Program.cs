using ClassLibrary1;
using static System.Net.Mime.MediaTypeNames;

namespace ClassLibrary1
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("--- Symulacja działania kont ---\n");

            Konto konto = new Konto("Jan Kowalski", 1000m);
            Console.WriteLine($"Utworzono konto dla {konto.Nazwa}, bilans: {konto.Bilans} zł");

            konto.Wplata(500m);
            Console.WriteLine($"Po wpłacie 500 zł, bilans: {konto.Bilans} zł");

            konto.Wyplata(300m);
            Console.WriteLine($"Po wypłacie 300 zł, bilans: {konto.Bilans} zł");

            konto.Blokuj();
            Console.WriteLine("Konto zostało zablokowane.");

            try { konto.Wplata(200m); }
            catch (Exception e) { Console.WriteLine($"Błąd: {e.Message}"); }

            Console.WriteLine("\n--- KontoPlus ---");
            KontoPlus kontoPlus = new KontoPlus("Anna Nowak", 1000m, 500m);
            Console.WriteLine($"Utworzono KontoPlus dla {kontoPlus.Nazwa}, bilans: {kontoPlus.Bilans} zł, debet: {kontoPlus.JednorazowyLimitDebetowy} zł");

            kontoPlus.WyplataKontoPlus(1200m);
            Console.WriteLine($"Po wypłacie 1200 zł, bilans: {kontoPlus.EffectiveBilans} zł, zablokowane: {kontoPlus.Zablokowane}");

            kontoPlus.WplataKontoPlus(500m);
            Console.WriteLine($"Po wpłacie 500 zł, bilans: {kontoPlus.EffectiveBilans} zł, zablokowane: {kontoPlus.Zablokowane}");

            Console.WriteLine("\n--- KontoLimit ---");
            KontoLimit kontoLimit = new KontoLimit("Piotr Wiśniewski", 1000m, 500m);
            Console.WriteLine($"Utworzono KontoLimit dla {kontoLimit.Nazwa}, bilans: {kontoLimit.Bilans} zł, debet: {kontoLimit.JednorazowyLimitDebetowy} zł");

            kontoLimit.WyplataKontoLimit(1300m);
            Console.WriteLine($"Po wypłacie 1300 zł, bilans: {kontoLimit.Bilans} zł, zablokowane: {kontoLimit.Zablokowane}");

            kontoLimit.WplataKontoLimit(600m);
            Console.WriteLine($"Po wpłacie 600 zł, bilans: {kontoLimit.Bilans} zł, zablokowane: {kontoLimit.Zablokowane}");
        }
    }
}