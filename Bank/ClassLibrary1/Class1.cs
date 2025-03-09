﻿namespace ClassLibrary1
{
    public class Konto
    {
        private string klient;  //nazwa klienta
        private decimal bilans;  //aktualny stan środków na koncie
        private bool zablokowane = false; //stan konta

        private Konto() { }

        public Konto(string klient, decimal bilansNaStart = 0)
        {
            if (string.IsNullOrWhiteSpace(klient))
            {
                throw new ArgumentException("Nazwa klienta nie może być pusta.");
            }

            this.klient = klient;
            this.bilans = bilansNaStart;
        }

        public string Nazwa => klient;  // nazwa klienta
        public decimal Bilans => bilans;  // aktualny bilans
        public bool Zablokowane => zablokowane;  // status konta

        public void Wplata(decimal kwota)
        {
            if (kwota <= 0)
            {
                throw new ArgumentException("Kwota musi być dodatnia.");
            }
            if (zablokowane)
            {
                throw new InvalidOperationException("Konto zablokowane.");
            }
            bilans += kwota;
        }

        public void Wyplata(decimal kwota)
        {
            if (kwota <= 0)
            {
                throw new ArgumentException("Kwota musi być dodatnia.");
            }
            if (zablokowane)
            {
                throw new InvalidOperationException("Konto zablokowane.");
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


    }
}
