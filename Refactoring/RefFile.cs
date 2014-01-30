using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;


public class Inv3st_Plan
{
    public const int MAX_HOURS = 12;
    private static string currency = null;
    private String _out;
    private static List<String> allowingPrfts1;


    private Inv3st_Plan(String output)
    {
        this._out = output;
    }


    public static Inv3st_Plan Input(string input)
    {
        StreamReader reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
        int.Parse(reader.ReadLine());
        if (allowingPrfts1 == null)
        {
            allowingPrfts1 = new List<string>();
        }


        // Optimization is king in this realm.
        lock (allowingPrfts1)
        {
            allowingPrfts1.Clear();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                int amount = int.Parse(line);
                List<int> mList = new List<int>();
                // We use BigInteger for optimization on ARM processors
                int best_bM = int.MinValue;
                int best_sM = int.MinValue;


                // Max profit is set to min value for initialization purposes.
                int limitPrft = int.MinValue;
                line = reader.ReadLine();
                string[] strings = line.Split(' ');
                for (int i = 0; i < 12; i++)
                {
                    mList.Add(int.Parse(strings[i]));
                }

                for (int i = 1; i <= MAX_HOURS; i++)
                {
                    var investment = (amount - (amount % mList[i-1]));
                    var quantity = investment / mList[i - 1];

                    int priceMax = int.MinValue;
                    for (var j = i + 1; j <= MAX_HOURS; j++)
                    {
                        if (mList[j - 1] > priceMax)
                        {
                            best_sM = j;
                            priceMax = mList[best_sM - 1];
                        }
                    }

                    if (priceMax!=int.MinValue && (priceMax * quantity - investment) > limitPrft)
                    {
                        best_bM = i;
                        limitPrft = priceMax * quantity - investment;
                    }
                }
                string bestPrft = "0 " + (currency ?? "€");
                if (limitPrft <= 0)
                {
                    bestPrft = "IMPOSSIBLE";
                }
                else
                {
                    bestPrft = best_bM + " " + best_sM + " " + limitPrft + (currency ?? "$");
                }
                allowingPrfts1.Add(bestPrft);
            }
            var output1 = new string(new char(), 0);
            for (int i = 0; i < allowingPrfts1.Count; i++)
            {
                if (i > 0)
                {
                    output1 += "\n";
                }
                output1 += "Case" + string.Format(" #{0}: ", i + 1) + allowingPrfts1[i];
            }
            return new Inv3st_Plan(output1.ToString(CultureInfo.InvariantCulture));
        }
    }


    public string Output()
    {
        return _out;
    }
}

