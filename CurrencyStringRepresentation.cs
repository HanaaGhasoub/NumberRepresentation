using System.Text;

namespace CurrencyStringConverter;

public static class CurrencyStringRepresentation
{
    private const int PartLength = 3;

    public static string GetStringRepresentation(string input)
    {
        if (!decimal.TryParse(input, out _))
            return string.Empty;

        var inputHasDecimal = input.Contains('.');

        var beforeDecimalPoint = inputHasDecimal ? input.Split('.')[0] : input;
        var integerRepresentation = GetRepresentationOfIntegerPart(beforeDecimalPoint);

        var integerNumber = int.Parse(beforeDecimalPoint);

        if (!inputHasDecimal)
            return integerNumber == 1 ? $"{integerRepresentation} dollar" : $"{integerRepresentation} dollars";

        var afterDecimalPoint = input.Split('.')[1];
        var decimalRepresentation = GetRepresentationOfDecimalPart(afterDecimalPoint);

        return integerNumber == 1
            ? $"{integerRepresentation} dollar and {decimalRepresentation}"
            : $"{integerRepresentation} dollars and {decimalRepresentation}";
    }

    private static string GetRepresentationOfIntegerPart(string integerNumber)
    {
        if (integerNumber.Length > 9) return string.Empty;

        if (!int.TryParse(integerNumber, out _)) return string.Empty;

        switch (integerNumber.Length)
        {
            case 1:
            {
                return NumbersBook.GetStringRepresentationOfNumber(int.Parse(integerNumber));
            }
            //99
            case 2:
            {
                return GetNumberRepresentationOfTwoDigits(integerNumber);
            }
            //999
            case 3:
            {
                var thirdDigitPresentation =
                    NumbersBook.GetStringRepresentationOfNumber(int.Parse(integerNumber[0].ToString()));

                var firstAndSecondDigits = integerNumber.Substring(1, 2);
                if (firstAndSecondDigits == "00") return $"{thirdDigitPresentation} hundred";

                var firstAndSecondDigitsRepresentation = GetNumberRepresentationOfTwoDigits(firstAndSecondDigits);

                return $"{thirdDigitPresentation} hundred {firstAndSecondDigitsRepresentation}";
            }
            default:
            {
                var numberParts = DivideNumberIntoThreeDigitsParts(integerNumber);

                var finalNumberRepresentation = new StringBuilder();

                // to format the presentation with keywords: "hundred, thousand, million"
                var zeroDigits = numberParts.Count;

                foreach (var part in numberParts)
                {
                    var suffix = zeroDigits switch
                    {
                        3 => "million",
                        2 => "thousand",
                        _ => "hundred"
                    };

                    var representation = GetNumberRepresentation(part);
                    switch (part.Length)
                    {
                        case 3:
                        {
                            var thirdDigitRepresentation =
                                NumbersBook.GetStringRepresentationOfNumber(int.Parse(part[0].ToString()));

                            var firstAndSecondDigits = part.Substring(1, 2);
                            if (firstAndSecondDigits == "00")
                            {
                                finalNumberRepresentation.Append(zeroDigits switch
                                {
                                    3 =>
                                        $"{thirdDigitRepresentation} hundred million ",
                                    2 =>
                                        $"{thirdDigitRepresentation} hundred thousand ",
                                    _ => $"{thirdDigitRepresentation} hundred  "
                                });
                            }
                            else
                            {
                                var firstAndSecondDigitsRepresentation =
                                    GetNumberRepresentationOfTwoDigits(firstAndSecondDigits);

                                finalNumberRepresentation.Append(zeroDigits switch
                                {
                                    3 =>
                                        $"{thirdDigitRepresentation} hundred {firstAndSecondDigitsRepresentation} million ",
                                    2 =>
                                        $"{thirdDigitRepresentation} hundred {firstAndSecondDigitsRepresentation} thousand ",
                                    _ => $"{thirdDigitRepresentation} hundred {firstAndSecondDigitsRepresentation} "
                                });
                            }

                            break;
                        }
                        default:
                        {
                            finalNumberRepresentation.Append($"{representation} {suffix} ");
                            break;
                        }
                    }

                    zeroDigits--;
                }

                return finalNumberRepresentation.ToString().Trim();
            }
        }
    }

    private static IReadOnlyList<string> DivideNumberIntoThreeDigitsParts(string integerNumber)
    {
        var numberParts = new List<string>();

        //we will divide the number into parts of 3-digits, so we can easy read it.
        //in some cases we will have number like 99-999 ,
        //which have 2-parts one with 3-digits and one with 2-digit.
        //So our start part is [99] which has 2-digits, for that we will need
        //generic calculations for the start point of our number and to get that as the line below.
        // ex: number=99-999, then number-length(5)%part-length(3) = 2 [99] "part is less than 3-digits".
        var noOfNotFullyParts = integerNumber.Length % PartLength;

        var partStartIndex = 0;
        var partEndIndex = noOfNotFullyParts > 0 ? noOfNotFullyParts : PartLength;

        while (partStartIndex != integerNumber.Length)
        {
            numberParts.Add(integerNumber.Substring(partStartIndex, partEndIndex));

            partStartIndex += partEndIndex;
            partEndIndex = PartLength;
        }

        return numberParts;
    }

    private static string GetRepresentationOfDecimalPart(string decimalNumber)
    {
        if (decimalNumber.Length == 1) decimalNumber = $"{decimalNumber}0";

        var twoDigitsNumberRepresentation = GetNumberRepresentationOfTwoDigits(decimalNumber);

        return twoDigitsNumberRepresentation == "one"
            ? $"{twoDigitsNumberRepresentation} cent"
            : $"{twoDigitsNumberRepresentation} cents";

        //switch (decimalNumber.Length)
        //{
        //    case 1:
        //    {
        //        var number = int.Parse(decimalNumber);
        //        var numberRepresentation = NumbersBook.GetStringRepresentationOfNumber(number);

        //        return number == 1 ? $"{numberRepresentation} cent" : $"{numberRepresentation} cents";
        //    }
        //    case 2:
        //    {
        //        var twoDigitsNumberRepresentation = GetNumberRepresentationOfTwoDigits(decimalNumber);

        //        return $"{twoDigitsNumberRepresentation} cents";
        //    }
        //    default:
        //        return string.Empty;
        //}
    }

    private static string GetNumberRepresentation(string part)
    {
        if (!int.TryParse(part, out var number))
            return string.Empty;

        switch (part.Length)
        {
            case 1:
                return NumbersBook.GetStringRepresentationOfNumber(number);
            case 2:
                return GetNumberRepresentationOfTwoDigits(part);
            default:
                var thirdDigitRepresentation = NumbersBook.GetStringRepresentationOfNumber(part[2]);

                var hundredsPart = part.Substring(1, 2);
                if (!int.TryParse(hundredsPart, out _))
                    return thirdDigitRepresentation;

                var firstAndSecondDigitsRepresentation = GetNumberRepresentationOfTwoDigits(hundredsPart);

                return $"{thirdDigitRepresentation} hundred {firstAndSecondDigitsRepresentation}";
        }
    }

    /// <summary>
    ///     get string representation of number of two digits.
    /// </summary>
    /// <param name="number">number as string</param>
    /// <returns>string representation of the number, ex: 24 will return twenty-four</returns>
    private static string GetNumberRepresentationOfTwoDigits(string number)
    {
        var partRepresentation = NumbersBook.GetStringRepresentationOfNumber(int.Parse(number));

        if (!string.IsNullOrEmpty(partRepresentation)) return partRepresentation;

        var partTensRepresentation = NumbersBook.GetStringRepresentationOfNumber(int.Parse(number[0].ToString()) * 10);
        var partOnesRepresentation = NumbersBook.GetStringRepresentationOfNumber(int.Parse(number[1].ToString()));

        return $"{partTensRepresentation}-{partOnesRepresentation}";
    }
}