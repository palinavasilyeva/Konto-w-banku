using System;

namespace ClassLibrary1
{
    public class Konto
    {
        private string klient; 
        private decimal bilans; 
        private bool zablokowane = false; 

        public bool Zablokowane => zablokowane;

        public Konto(string klient, decimal bilansNaStart = 0)
        {
            this.klient = klient;
            this.bilans = bilansNaStart;
        }

        public string Nazwa => klient;
        public decimal Bilans => bilans;

        public void Wplata(decimal kwota)
        {
            if (zablokowane)
            {
                throw new InvalidOperationException("Konto zablokowane.");
            }
            if (kwota <= 0)
            {
                throw new ArgumentException("Kwota musi być dodatnia.");
            }
            bilans += kwota;
        }

        public void Wyplata(decimal kwota)
        {
            if (zablokowane)
            {
                throw new InvalidOperationException("Konto zablokowane.");
            }
            if (kwota <= 0)
            {
                throw new ArgumentException("Kwota musi być dodatnia.");
            }

            if (bilans < kwota)
            {
                throw new InvalidOperationException("Brak środków na koncie.");
            }
            bilans -= kwota;
        }

        public void Blokuj()
        {
            zablokowane = true;
        }

        public void Odblokuj()
        {
            zablokowane = false;
        }

        protected void SetBilans(decimal nowyBilans)
        {
            bilans = nowyBilans;
        }

        public void UstawBilans(decimal nowyBilans)
        {
            bilans = nowyBilans;
        }
    }

    public class KontoPlus : Konto
    {
        private decimal jednorazowyLimitDebetowy; 
        private bool debetWykorzystany = false;

        public decimal JednorazowyLimitDebetowy
        {
            get { return jednorazowyLimitDebetowy; }
            private set { jednorazowyLimitDebetowy = value; }
        }

        public KontoPlus(string klient, decimal bilansNaStart = 0, decimal limitDebetowy = 0) : base(klient, bilansNaStart)
        {
            this.JednorazowyLimitDebetowy = limitDebetowy;
        }

        // Metoda wpłaty na KontoPlus
        public void WplataKontoPlus(decimal kwota)
        {
            if (Zablokowane && !debetWykorzystany) 
            {
                throw new InvalidOperationException("Konto zablokowane.");
            }

            if (debetWykorzystany)
            {
                Odblokuj(); 
                debetWykorzystany = false;
            }

            base.Wplata(kwota);
        }

        // Metoda wypłaty z KontaPlus
        public void WyplataKontoPlus(decimal kwota)
        {
            if (Zablokowane)
            {
                throw new InvalidOperationException("Konto zablokowane.");
            }

            if (kwota <= 0)
            {
                throw new ArgumentException("Kwota musi być dodatnia.");
            }

            if (Bilans >= kwota)
            {
                base.Wyplata(kwota);
            }
            else if (kwota - Bilans <= JednorazowyLimitDebetowy)
            {
                jednorazowyLimitDebetowy -= (kwota - Bilans);
                SetBilans(0);
                debetWykorzystany = true;
                Blokuj();
            }
            else
            {
                throw new InvalidOperationException("Przekroczono dostępny debet.");
            }
        }

        public decimal EffectiveBilans
        {
            get
            {
                return debetWykorzystany ? base.Bilans - JednorazowyLimitDebetowy : base.Bilans;
            }
        }
    }

    public class KontoLimit
    {
        private Konto konto;
        private decimal jednorazowyLimitDebetowy;
        private bool debetWykorzystany = false;

        public decimal JednorazowyLimitDebetowy
        {
            get { return jednorazowyLimitDebetowy; }
            private set { jednorazowyLimitDebetowy = value; }
        }

        public bool Zablokowane => konto.Zablokowane;

        public KontoLimit(string klient, decimal bilansNaStart = 0, decimal limitDebetowy = 0)
        {
            this.konto = new Konto(klient, bilansNaStart);
            this.JednorazowyLimitDebetowy = limitDebetowy;
        }

        public string Nazwa => konto.Nazwa;

        public decimal Bilans
        {
            get
            {
                if (debetWykorzystany)
                {
                    return konto.Bilans - JednorazowyLimitDebetowy;
                }
                else
                {
                    return konto.Bilans;
                }
            }
        }

        public void WplataKontoLimit(decimal kwota)
        {
            if (Zablokowane)
            {
                throw new InvalidOperationException("Konto zablokowane.");
            }

            konto.Wplata(kwota);

            if (konto.Bilans > 0 && debetWykorzystany)
            {
                debetWykorzystany = false;
                konto.Odblokuj();
            }
            else if (konto.Bilans == 0 && debetWykorzystany)
            {
                debetWykorzystany = false;
                konto.Odblokuj();
            }
        }

        public void WyplataKontoLimit(decimal kwota)
        {
            if (Zablokowane)
            {
                throw new InvalidOperationException("Konto zablokowane.");
            }

            if (kwota <= 0)
            {
                throw new ArgumentException("Kwota musi być dodatnia.");
            }

            if (konto.Bilans >= kwota)
            {
                konto.Wyplata(kwota);
            }
            else if (kwota - konto.Bilans <= JednorazowyLimitDebetowy)
            {
                JednorazowyLimitDebetowy -= (kwota - konto.Bilans);
                konto.UstawBilans(0);
                debetWykorzystany = true;
                konto.Blokuj();
            }
            else
            {
                throw new InvalidOperationException("Przekroczono dostępny debet.");
            }
        }
    }

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
