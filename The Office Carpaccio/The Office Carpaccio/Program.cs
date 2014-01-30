namespace The_Office_Carpaccio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using NFluent;

    using NUnit.Framework;

    class Program
    {
        static void Main(string[] args)
        {
            var mycalc = new DunderMifflinPriceCalculator();
            Console.WriteLine("Welcome to the Dunder Mifflin Paper Co wonderful price calculator");
            Console.WriteLine();

            Console.WriteLine("Input (gross) unit price in EUR:");
            var input = Console.ReadLine();
            mycalc.SetUnitPrice(input);

            Console.WriteLine("Input desired quantity:");
            input = Console.ReadLine();
            mycalc.SetQuantity(input);

            Console.WriteLine("Input shipping country (ES, FR, IT, PL)");
            input = Console.ReadLine();
            mycalc.SetCountry(input);

            MessageBox.Show(mycalc.TextResult, "Result");

        }
    }

    [TestFixture]
    internal class TestOffice
    {
        [Test]
        public void Dummy()
        {
            var calc = new DunderMifflinPriceCalculator();
            calc.SetUnitPrice("5");

            Check.That(calc.UnitPrice).IsEqualTo(5M);

            calc.SetQuantity("100");

 

            calc.SetCountry("ES");
            Check.That(calc.Total).IsEqualTo(605m);
            Check.That(calc.TextResult).IsEqualTo("5 x 100 = 605,00 € TTC (in ES)");

            calc.SetQuantity("10");

            calc.SetUnitPrice("12,10");
            Check.That(calc.TextResult).IsEqualTo("12,10 x 10 = 146,41 € TTC (in ES)");
            calc.SetQuantity("1");
            Check.That(calc.Total).IsEqualTo(14.64M);

            calc.SetUnitPrice("1");
            calc.SetQuantity("200");
            calc.SetCountry("FR");

            Check.That(calc.Total).IsEqualTo(228M);

            calc.SetQuantity("500");
            Check.That(calc.Total).IsEqualTo(558M);

            Check.That(() => calc.SetUnitPrice("-1")).Throws<ApplicationException>();
            Check.That(() => calc.SetQuantity("-1")).Throws<ApplicationException>();
        }
        
    }

    internal class DunderMifflinPriceCalculator
    {
        private decimal unitPrice;

        private int quantity;

        private decimal tva;

        private string country;

        public decimal UnitPrice
        {
            get
            {
                return unitPrice;
            }
        }

        public string TextResult
        {
            get
            {
                return string.Format("{0} x {2} = {1:C} TTC (in {3})", unitPrice, Total, quantity, country);
            }
        }

        public decimal Total 
        {
            get
            {
                var factor = 1M;
                if (this.quantity > 100)
                {
                    factor = .95M;
                    if (this.quantity > 400)
                    {
                        factor = .93M;
                    }
                }
                return Math.Round(factor * unitPrice * (1M + this.tva) * this.quantity, 2);
            }
        }

        public void SetUnitPrice(string s)
        {
            if (!decimal.TryParse(s, out this.unitPrice))
                Fails();
            if (this.unitPrice <= 0)
            {
                Fails();
            }
        }

        public void SetQuantity(string s)
        {
            if (!int.TryParse(s, out this.quantity))
                Fails();
            if (this.quantity <= 0)
            {
                Fails();
            }
        }

        private static void Fails()
        {
            Console.WriteLine("Bad input, exiting");
            Console.ReadKey();
            Environment.Exit(-1);
        }

        public void SetCountry(string es)
        {
            switch (es)
            {
                case "ES":
                    this.tva = .21M;
                    break;
                case "FR":
                    this.tva = .20M;
                    break;
                case "IT":
                    this.tva = .22M;
                    break;
                case "PL":
                    this.tva = .23M;
                    break;
                default:
                    Fails();
                    break;
            }
            this.country = es;
        }
    }
}
