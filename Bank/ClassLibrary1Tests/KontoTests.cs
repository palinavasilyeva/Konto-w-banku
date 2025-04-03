using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassLibrary1;
using System;

namespace ClassLibrary1.Tests
{
    [TestClass()]
    public class KontoTests
    {
        [TestMethod()]
        public void TestTworzenieKonta()
        {
            // Arrange
            decimal currentBalance = 1000m;
            var konto = new Konto("Jan Kowalski", currentBalance);

            // Act & Assert
            Assert.AreEqual("Jan Kowalski", konto.Nazwa);
            Assert.AreEqual(currentBalance, konto.Bilans);
            Assert.IsFalse(konto.Zablokowane);
        }

        [TestMethod()]
        public void TestWplata()
        {
            // Arrange
            decimal currentBalance = 1000m;
            var konto = new Konto("Jan Kowalski", currentBalance);
            // Act
            konto.Wplata(500m);
            // Assert
            Assert.AreEqual(currentBalance + 500m, konto.Bilans);
        }

        [TestMethod()]
        public void TestWplataZablokowanegoKonta()
        {
            // Arrange
            var konto = new Konto("Jan Kowalski", 1000m);
            konto.Blokuj();
            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => konto.Wplata(500m));
        }

        [TestMethod()]
        public void TestWyplata()
        {
            // Arrange
            decimal currentBalance = 1000m;
            var konto = new Konto("Jan Kowalski", currentBalance);
            // Act
            konto.Wyplata(500m);
            // Assert
            Assert.AreEqual(currentBalance - 500m, konto.Bilans);
        }

        [TestMethod()]
        public void TestWyplataZaDuza()
        {
            // Arrange
            var konto = new Konto("Jan Kowalski", 1000m);
            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => konto.Wyplata(1500m));
        }

        [TestMethod()]
        public void TestWyplataZablokowanegoKonta()
        {
            // Arrange
            var konto = new Konto("Jan Kowalski", 1000m);
            konto.Blokuj();
            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => konto.Wyplata(500m));
        }

        [TestMethod()]
        public void TestBlokowanieKonta()
        {
            // Arrange
            var konto = new Konto("Jan Kowalski", 1000m);
            // Act
            konto.Blokuj();
            // Assert
            Assert.IsTrue(konto.Zablokowane);
        }

        [TestMethod()]
        public void TestOdBlokowaniaKonta()
        {
            // Arrange
            var konto = new Konto("Jan Kowalski", 1000m);
            konto.Blokuj();
            // Act
            konto.Blokuj();
            // Assert
            Assert.IsTrue(konto.Zablokowane);
        }

        [TestMethod()]
        public void TestWyplataNegatywnaKwota()
        {
            // Arrange
            var konto = new Konto("Jan Kowalski", 1000m);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => konto.Wyplata(-500m));
            Assert.ThrowsException<ArgumentException>(() => konto.Wyplata(0m));
        }

        [TestMethod()]
        public void TestUstawienieNowegoBilansu()
        {
            // Arrange
            var konto = new Konto("Jan Kowalski", 1000m);

            // Act
            konto.UstawBilans(500m);

            // Assert
            Assert.AreEqual(500m, konto.Bilans);
        }

        [TestMethod()]
        public void TestWyplataBrakSrodkow()
        {
            // Arrange
            var konto = new Konto("Jan Kowalski", 1000m);

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => konto.Wyplata(2000m));  // Insufficient funds
        }


    }

    [TestClass()]
    public class KontoPlusTests
    {
        [TestMethod()]
        public void TestWyplataZBilansu()
        {
            // Arrange
            var konto = new KontoPlus("Jan Kowalski", 1000m, 500m);

            // Act
            konto.WyplataKontoPlus(500m);

            // Assert
            Assert.AreEqual(500m, konto.EffectiveBilans);
            Assert.IsFalse(konto.Zablokowane);
        }

        [TestMethod()]
        public void TestWyplataZDebetem()
        {
            // Arrange
            var konto = new KontoPlus("Jan Kowalski", 1000m, 500m);

            // Act
            konto.WyplataKontoPlus(1500m);

            // Assert
            Assert.AreEqual(0m, konto.EffectiveBilans);
            Assert.IsTrue(konto.Zablokowane);
        }

        [TestMethod()]
        public void TestWplataPoDebecie()
        {
            // Arrange
            var konto = new KontoPlus("Jan Kowalski", 1000m, 500m);
            konto.WyplataKontoPlus(1500m);

            // Act
            konto.WplataKontoPlus(1000m);

            // Assert
            Assert.AreEqual(1000m, konto.EffectiveBilans);
            Assert.IsFalse(konto.Zablokowane);
        }

        [TestMethod()]
        public void TestPrzekroczenieLimituDebetowego()
        {
            // Arrange
            var konto = new KontoPlus("Jan Kowalski", 1000m, 500m);

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => konto.WyplataKontoPlus(2001m));
        }

        [TestMethod()]
        public void TestWplataPoDebecieNaKontoPlus()
        {
            // Arrange
            var konto = new KontoPlus("Jan Kowalski", 1000m, 500m);
            konto.WyplataKontoPlus(1500m); 

            // Act
            konto.WplataKontoPlus(2000m);

            // Assert
            Assert.AreEqual(2000m, konto.EffectiveBilans);
            Assert.IsFalse(konto.Zablokowane);
        }

        [TestMethod()]
        public void TestResetowanieDebetuPoWplacie()
        {
            // Arrange
            var konto = new KontoPlus("Jan Kowalski", 1000m, 500m);
            konto.WyplataKontoPlus(1500m); // Use overdraft

            // Act
            konto.WplataKontoPlus(1000m); // Deposit should clear the overdraft

            // Assert
            Assert.AreEqual(1000m, konto.EffectiveBilans);  // Effective balance should be back to the deposited amount
            Assert.IsFalse(konto.Zablokowane);  // The account should be unblocked
        }

    }

    [TestClass()]
    public class KontoLimitTests
    {
        [TestMethod()]
        public void TestWyplataZBilansu()
        {
            // Arrange
            var konto = new KontoLimit("Jan Kowalski", 1000m, 500m);

            // Act
            konto.WyplataKontoLimit(500m);

            // Assert
            Assert.AreEqual(500m, konto.Bilans);
            Assert.IsFalse(konto.Zablokowane);
        }

        [TestMethod()]
        public void TestWyplataZDebetem()
        {
            // Arrange
            var konto = new KontoLimit("Jan Kowalski", 1000m, 500m);

            // Act
            konto.WyplataKontoLimit(1500m);

            // Assert
            Assert.AreEqual(0m, konto.Bilans);
            Assert.IsTrue(konto.Zablokowane);
        }

        [TestMethod()]
        public void TestPrzekroczenieLimituDebetowegoKontoLimit()
        {
            // Arrange
            var konto = new KontoLimit("Jan Kowalski", 1000m, 500m);

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => konto.WyplataKontoLimit(2001m));
        }

        [TestMethod()]
        public void TestWyplataZKontemLimitBezUzyciaDebetu()
        {
            // Arrange
            var konto = new KontoLimit("Jan Kowalski", 1000m, 500m);

            // Act
            konto.WyplataKontoLimit(1000m);  // Should use balance, not the overdraft

            // Assert
            Assert.AreEqual(0m, konto.Bilans); // Balance should be zero
            Assert.IsFalse(konto.Zablokowane); // The account should not be blocked since no overdraft was used
        }

    }
}
