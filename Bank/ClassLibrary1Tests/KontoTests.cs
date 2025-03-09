using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Assert.AreEqual(500m, konto.Bilans);
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
            Assert.AreEqual(0m, konto.Bilans);
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
            Assert.AreEqual(1000m, konto.Bilans); 
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
    }

    [TestClass()]
    public class KontoLimitTests
    {
        // Test 1: Sprawdzenie, czy wypłata w ramach dostępnego salda działa poprawnie.
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

        // Test 2: Sprawdzenie wypłaty, która wykorzystuje limit debetowy.
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

        // Test 3: Sprawdzenie działania metody wpłaty po wykorzystaniu debetu (odblokowanie konta).
        [TestMethod()]
        public void TestWplataPoDebecie()
        {
            // Arrange
            var konto = new KontoLimit("Jan Kowalski", 1000m, 500m);
            konto.WyplataKontoLimit(1500m);

            // Act
            konto.WplataKontoLimit(1000m);

            // Assert
            Assert.AreEqual(1000m, konto.Bilans); 
            Assert.IsFalse(konto.Zablokowane);  
        }
    }


}
