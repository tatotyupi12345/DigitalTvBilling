using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Utils
{
    public static class NumberToString
    {
        private static string wordJoin = "და";
        private static string wordEndi = "ი";
        private static string wordEnda = "ა";
        private static string[] ateulebi = { "ოც", "ორმოც", "სამოც", "ოთხმოც" };
        private static string miliard = "მილიარდი";
        private static string milion = "მილიონი";
        private static string atasi = "ათასი";
        private static string asi = "ას";
        private static string[] ricxvebi = { "ერთ", "ორ", "სამ", "ოთხ", "ხუთ", "ექვს", "შვიდ", "რვ", "ცხრ", "ათ", "თერთმეტ", "თორმეტ", "ცამეტ", "თოთხმეტ", "თხუთმეტ", "თექვსმეტ", "ჩვიდმეტ", "თვრამეტ", "ცხრამეტ" };

        public static string NumberToWords(long number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + string.Format(" {0} ", "მილიონი");
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + string.Format(" {0} ", "ათასი");
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + string.Format(" {0} ", "ასი");
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "და;add";

                var unitsMap = new[] {
                    "ნული", "ერთი", 
                    "ორი", "სამი", "ოთხი", 
                    "ხუთი", "ექვსი", "შვიდი", 
                    "რვა", "ცხრა", "ათი", 
                    "თერთმეტი", "თორმეტი", "ცამეტი",
                    "თოთხმეტი", "თხუთმეტი", "თექვსმეტი",
                    "ჩვიდმეტი", "თვრამეტი", "ცხრამეტი"
                };
                var tensMap = new[] { 
                    "ნული", "ათი", 
                    "ოცი", "ოცდაათი", 
                    "ორმოცი", "ორმოცდაათი", 
                    "სამოცი", "სამოცდაათი",
                    "ოთხმოცი", "ოთხმოცდაათი" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-;add" + unitsMap[number % 10];
                }
            }

            return words;
        }
        public static string GetNumberGeorgianString(decimal number, string decimalSymbol, string currencyName, string coinsName)
        {
            if (string.IsNullOrEmpty(decimalSymbol) || decimalSymbol.Length == 0 || decimalSymbol.Length > 1)
                return string.Empty;

            if (currencyName == null)
                currencyName = string.Empty;

            if (coinsName == null)
                coinsName = string.Empty;

            string numberString = number.ToString();
            string endAfterPointString = "";
            string endBeforePointString = "";
            string endNumberName = "";

            int pointIndex = 0;

            pointIndex = numberString.IndexOf(decimalSymbol);
            int len = numberString.Length;
            if (pointIndex >= 0 && len - pointIndex == 2)
            {
                numberString = numberString + "0";
            }
            if (pointIndex > 0)
            {
                endAfterPointString = GetCoinsFullName(numberString.Substring(pointIndex + 1, numberString.Length - (pointIndex + 1)));
                numberString = numberString.Substring(0, pointIndex);

            }
            else
                endAfterPointString = "00 ";

            endBeforePointString = GetNumberFullName(Convert.ToInt64(numberString));
            string _currencyName = currencyName;
            string _coinsName = coinsName;
            string _wordJoin = wordJoin;

            endNumberName = string.Format("{0} {1} {2} {3} {4}", endBeforePointString, _currencyName, _wordJoin, endAfterPointString, _coinsName);

            return endNumberName;
        }
        public static string GetNumberGeorgianString(decimal number)
        {
            return GetNumberFullName(Convert.ToInt64(Math.Truncate(number)));

        }

        /// <summary>
        /// Gets the full name of the coins.
        /// </summary>
        /// <param name="numb">The numb.</param>
        /// <returns></returns>
        private static string GetCoinsFullName(string numb)
        {
            int number;
            if (int.TryParse(numb, out number))
            {
                if (number > 0)
                {
                    return number.ToString();
                }
                else
                    return "00";
            }
            else
                return "00";
        }

        /// <summary>
        /// Gets the full name of the number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        private static string GetNumberFullName(long number)
        {
            if (number == 0)
                return "ნული";

            return GetBillionString(number.ToString()).Replace("  ", " ");
        }

        /// <summary>
        /// Gets the ateuli string.
        /// </summary>
        /// <param name="numb">The numb.</param>
        /// <returns></returns>
        private static string GetAteuliString(string numb)
        {
            if (string.IsNullOrEmpty(numb))
                return string.Empty;

            byte number;
            int index;
            string numberAt = string.Empty, numberRicx = string.Empty;
            if (byte.TryParse(numb, out number))
            {
                if (number > 0 && number <= 19)
                {
                    index = number - 1;
                    return getWord("", "", string.Format("{0}{1}", ricxvebi[index], (numb.Substring(numb.Length - 1, 1) == "8" || numb.Substring(numb.Length - 1, 1) == "9") && number < 10 ? wordEnda : wordEndi));
                }

                else if (number == 20)
                    return getWord(string.Format("{0}{1}", ateulebi[0], wordEndi), "", "");
                else if (number > 20 && number <= 39)
                {
                    index = number - 20 - 1;
                    return getWord(ateulebi[0], wordJoin, ricxvebi[number - 20 - 1] + ((numb.Substring(numb.Length - 1, 1) == "8" || numb.Substring(numb.Length - 1, 1) == "9") && number < 30 ? wordEnda : wordEndi));
                }
                else if (number == 40)
                    return getWord(string.Format("{0}{1}", ateulebi[1], wordEndi), "", "");
                else if (number > 40 && number <= 59)
                {
                    index = number - 40 - 1;
                    return getWord(ateulebi[1], wordJoin, ricxvebi[index] + ((numb.Substring(numb.Length - 1, 1) == "8" || numb.Substring(numb.Length - 1, 1) == "9") && number < 50 ? wordEnda : wordEndi));
                }
                else if (number == 60)
                    return getWord(string.Format("{0}{1}", ateulebi[2], wordEndi), "", "");
                else if (number > 60 && number <= 79)
                {
                    index = number - 60 - 1;
                    return getWord(ateulebi[2], wordJoin, ricxvebi[index] + ((numb.Substring(numb.Length - 1, 1) == "8" || numb.Substring(numb.Length - 1, 1) == "9") && number < 70 ? wordEnda : wordEndi));
                }
                else if (number == 80)
                    return getWord(string.Format("{0}{1}", ateulebi[3], wordEndi), "", "");
                else if (number > 80 && number <= 99)
                {
                    index = number - 80 - 1;
                    return getWord(ateulebi[3], wordJoin, ricxvebi[index] + ((numb.Substring(numb.Length - 1, 1) == "8" || numb.Substring(numb.Length - 1, 1) == "9") && number < 90 ? wordEnda : wordEndi));
                }
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }
        private static string getWord(string numberUp, string wodr_join, string numberDown)
        {
            string fullNumber = string.Empty;
            fullNumber = string.Format("{0}{1}{2}", numberUp, wodr_join, numberDown);
            return fullNumber;
        }
        private static string GetAseulebiString(string numb)
        {
            if (string.IsNullOrEmpty(numb))
                return string.Empty;

            if (numb.Length == 2 || numb.Length == 1)
                return GetAteuliString(numb);

            short number;
            string ateulebi = "";
            if (short.TryParse(numb, out number))
            {
                if (numb.StartsWith("0"))
                    return GetAteuliString(numb.Substring(1, numb.Length - 1));
                else if (numb.StartsWith("1"))
                {
                    ateulebi = GetAteuliString(numb.Substring(1, numb.Length - 1));
                    if (ateulebi != string.Empty)
                        return string.Format("{0}{1}", asi, ateulebi);
                    else
                        return string.Format("{0}{1}", asi, wordEndi);
                }
                else if (numb.StartsWith("8") || numb.StartsWith("9"))
                {
                    ateulebi = GetAteuliString(numb.Substring(1, numb.Length - 1));
                    if (ateulebi != string.Empty)
                        return string.Format("{0}{1}", ricxvebi[Convert.ToInt32(numb.Substring(0, 1)) - 1] + wordEnda + asi, ateulebi);
                    else
                        return string.Format("{0}{1}{2}{3}", ricxvebi[Convert.ToInt32(numb.Substring(0, 1)) - 1], wordEnda, asi, wordEndi);
                }
                else
                {
                    ateulebi = GetAteuliString(numb.Substring(1, numb.Length - 1));
                    if (ateulebi != string.Empty)
                        return string.Format("{0}{1}", ricxvebi[Convert.ToInt32(numb.Substring(0, 1)) - 1] + asi, ateulebi);
                    else
                        return string.Format("{0}{1}{2}", ricxvebi[Convert.ToInt32(numb.Substring(0, 1)) - 1], asi, wordEndi);
                }
            }
            else
                return string.Empty;
        }
        /// <summary>
        /// Gets the ataseuli string.
        /// </summary>
        /// <param name="numb">The numb.</param>
        /// <returns></returns>
        private static string GetAtaseuliString(string numb)
        {
            int number;
            if (int.TryParse(numb, out number))
            {
                if (number.ToString().Length <= 3)
                    return GetAseulebiString(numb);
                else if (number.ToString().Length > 3)
                {
                    string ataseuli = GetAseulebiString(number.ToString().Substring(0, number.ToString().Length - 3));
                    string aseuli = GetAseulebiString(number.ToString().Substring(number.ToString().Length - 3, 3));

                    return string.Format("{0} {1} {2}", ataseuli, atasi, aseuli);
                }
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Gets the million string.
        /// </summary>
        /// <param name="numb">The numb.</param>
        /// <returns></returns>
        private static string GetMillionString(string numb)
        {
            long number = 0;

            if (long.TryParse(numb, out number))
            {
                int length = number.ToString().Length;

                if (length <= 6)
                    return GetAtaseuliString(number.ToString());
                else if (length > 6 && length <= 9)
                {
                    string milionString = GetAseulebiString(number.ToString().Substring(0, length - 6));
                    string atasiString = GetAtaseuliString(number.ToString().Substring(length - 6, 6));

                    return string.Format("{0} {1} {2}", milionString, milion, atasiString);
                }
                else
                    return string.Empty;

            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Gets the billion string.
        /// </summary>
        /// <param name="numb">The numb.</param>
        /// <returns></returns>
        private static string GetBillionString(string numb)
        {
            long number = 0;

            if (long.TryParse(numb, out number))
            {
                int length = number.ToString().Length;

                if (length <= 9)
                    return GetMillionString(number.ToString());
                else if (length > 9 && length <= 12)
                {
                    string billionString = GetAseulebiString(number.ToString().Substring(0, length - 9));
                    string millionString = GetMillionString(number.ToString().Substring(length - 9, 9));

                    return string.Format("{0} {1} {2}", billionString, miliard, millionString);
                }
                else
                    return string.Empty;

            }
            else
                return string.Empty;
        }
    }
}