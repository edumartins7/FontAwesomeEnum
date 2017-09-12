using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace FontAwesomeEnum
{
    class Program
    {
        private static string InputPath = $"{Environment.CurrentDirectory}\\variables.less";
        private static string EnumOutputPath = $"{Environment.CurrentDirectory}\\FontAwesomeEnum.cs";
        private static string ConstsOutputPath = $"{Environment.CurrentDirectory}\\FontAwesomeConstants.cs";

        static void Main(string[] args)
        {
            try
            {
                SafeMain(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
        }

        static void SafeMain(string[] args)
        {
            Console.WriteLine("FontAwesomeEnum - Version 1.2.0 Copyright (C) Simon Mourier 2013-" + DateTime.Now.Year + ". All rights reserved.");
            Console.WriteLine("");


            Console.WriteLine("Input file path: " + InputPath);
            Console.WriteLine("Enum output file path: " + EnumOutputPath);
            Console.WriteLine("Constants output file path: " + ConstsOutputPath);

            var enums = new List<Tuple<string, string>>();
            using (var reader = new StreamReader(InputPath, Encoding.Default))
            {
                do
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        break;

                    const string varToken = "@fa-var-";
                    line = line.Trim();
                    if (!line.StartsWith(varToken))
                        continue;

                    int valuePos = line.IndexOf(':');
                    if (valuePos < 0)
                        continue;

                    string varName = "fa_" + line.Substring(varToken.Length, valuePos - varToken.Length);
                    string value = line.Substring(valuePos + 1).Trim();

                    if (!value.StartsWith("\"\\") || !value.EndsWith("\";"))
                        continue;

                    value = value.Substring(2, value.Length - 2 - 2);

                    enums.Add(new Tuple<string, string>(varName, value));
                }
                while (true);
            }

            Console.WriteLine();

            if (enums.Count == 0)
            {
                Console.WriteLine("No variable was found.");
                return;
            }
            Console.WriteLine("Variables detected: " + enums.Count);

            Console.WriteLine();

            WriteEnumFile(enums);
            WriteConstsFile(enums);
        }
    
        static void WriteEnumFile(IList<Tuple<string, string>> enums)
        {

            using (var writer = new StreamWriter(EnumOutputPath, false))
            {
                writer.WriteLine("\tpublic enum FontAwesomeEnum");
                writer.WriteLine("\t{");
                for (int i = 0; i < enums.Count; i++)
                {
                    var kv = enums[i];
  
                    writer.Write('\t');
                    writer.Write('\t');
                    writer.Write(GetValidIdentifier(kv.Item1));
                    writer.Write(" = 0x");
                    writer.Write(kv.Item2);

                    if (i < (enums.Count - 1))
                    {
                        writer.WriteLine(',');
                    }
                    writer.WriteLine();
                }
                writer.WriteLine("\t}");
                writer.WriteLine("");
            }

            Console.WriteLine("Enum file was successfully written.");
        }

        static void WriteConstsFile(IList<Tuple<string, string>> enums)
        {
            using (var writer = new StreamWriter(ConstsOutputPath, false))
            {
                WriteHeader(writer);
                writer.WriteLine("\tpublic static partial class FontAwesomeResource");
                writer.WriteLine("\t{");
                for (int i = 0; i < enums.Count; i++)
                {
                    var kv = enums[i];
                    //writer.WriteLine("\t\t/// <summary>");
                    //writer.WriteLine("\t\t/// fa-" + kv.Item1 + " glyph (" + kv.Item2 + ").");
                    //writer.WriteLine("\t\t/// </summary>");
                    writer.WriteLine("\t\tpublic const char " + GetValidIdentifier(kv.Item1) + " = '\\u" + kv.Item2 + "';");
                    if (i < (enums.Count - 1))
                    {
                        writer.WriteLine();
                    }
                }
                writer.WriteLine("\t}");
                writer.WriteLine("}");
            }

            Console.WriteLine("Constants file was successfully written.");
        }

        static void WriteHeader(StreamWriter writer)
        {
            writer.WriteLine("//------------------------------------------------------------------------------");
            writer.WriteLine("// <auto-generated>");
            writer.WriteLine("//     This code was generated by a tool.");
            //writer.WriteLine("//     Runtime Version:" + Environment.Version);
            writer.WriteLine("//");
            writer.WriteLine("//     Changes to this file may cause incorrect behavior and will be lost if");
            writer.WriteLine("//     the code is regenerated.");
            writer.WriteLine("// </auto-generated>");
            writer.WriteLine("//------------------------------------------------------------------------------");
            writer.WriteLine();

            writer.WriteLine("namespace FontAwesome");
            writer.WriteLine("{");
            writer.WriteLine("\t/// <summary>");
            writer.WriteLine("\t/// Font Awesome Resources.");
            writer.WriteLine("\t/// </summary>");
        }

        static string GetValidIdentifier(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));

            int start = 0;
            var sb = new StringBuilder(text.Length);
            if (IsValidIdentifierStart(text[0]))
            {
                sb.Append(text[0]);
                start = 1;
            }
            else
            {
                sb.Append('_');
            }

            bool nextUpper = false;
            for (int i = start; i < text.Length; i++)
            {
                if (IsValidIdentifierPart(text[i]))
                {
                    if (nextUpper)
                    {
                        sb.Append(char.ToUpper(text[i], CultureInfo.CurrentCulture));
                        nextUpper = false;
                    }
                    else
                    {
                        sb.Append(text[i]);
                    }
                }
                else
                {
                    if (text[i] == ' ')
                    {
                        nextUpper = true;
                    }
                    else
                    {
                        sb.Append('_');
                    }
                }
            }
            return sb.ToString();
        }

        static bool IsValidIdentifierStart(char character)
        {
            if (character == '_')
                return true;

            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            switch (category)
            {
                case UnicodeCategory.UppercaseLetter://Lu
                case UnicodeCategory.LowercaseLetter://Ll
                case UnicodeCategory.TitlecaseLetter://Lt
                case UnicodeCategory.ModifierLetter://Lm
                case UnicodeCategory.OtherLetter://Lo
                case UnicodeCategory.LetterNumber://Nl
                    return true;

                default:
                    return false;
            }
        }

        static bool IsValidIdentifierPart(char character)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            switch (category)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.LetterNumber:
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.SpacingCombiningMark:
                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.ConnectorPunctuation:
                case UnicodeCategory.Format:
                    return true;

                default:
                    return false;
            }
        }
    }
}
