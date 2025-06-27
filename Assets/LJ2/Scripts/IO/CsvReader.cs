using IO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IO 
{
    public class CsvReader
    {
        public static string BasePath
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath + Path.DirectorySeparatorChar;
#else
                return Application.persistentDataPath + Path.DirectorySeparatorChar;
#endif
            }
        }

        public static void Read(Csv csv)
        {
            if (!IsValidPath(csv) ||
                !IsValidEmpty(csv, out string[] lines))
                return;

            bool isReadSuccessful;
            switch (csv)
            {
                case CsvTable table:
                    isReadSuccessful = ReadToTable(table, lines);
                    break;
                default:
                    isReadSuccessful = false;
                    break;
            }
            PrintResult(csv, isReadSuccessful);
        }

        private static bool ReadToTable(CsvTable csv, string[] lines)
        {
            string[] firstLineFields = lines[0].Split(csv.SplitSymbol);
            int rows = lines.Length;
            int columns = firstLineFields.Length;

            csv.Table = new string[rows, columns];

            for (int r = 0; r < rows; r++)
            {
                string[] fields = lines[r].Split(csv.SplitSymbol);
                if (fields.Length < columns)
                {
                    return false;
                }

                for (int c = 0; c < columns; c++)
                {
                    csv.Table[r, c] = fields[c];
                }
            }

            return true;
        }

        private static bool IsValidPath(Csv csv)
        {
            if (!File.Exists(csv.FilePath))
            {
#if UNITY_EDITOR
                Debug.LogError($"Error: CSV file not found at path: {csv.FilePath}");
#endif
                return false;
            }
            return true;
        }

        private static bool IsValidEmpty(Csv csv, out string[] lines)
        {
            lines = File.ReadAllLines(csv.FilePath);

            if (lines.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogError($"Error: CSV file at path {csv.FilePath} is empty.");
#endif
                return false;
            }
            return true;
        }

        private static void PrintResult(Csv csv, bool result)
        {
#if UNITY_EDITOR
            if (result)
            {
                Debug.Log($"Successfully loaded CSV file from path: {csv.FilePath}");
            }
            else
            {
                Debug.LogError($"Error: CSV file at path {csv.FilePath} has inconsistent column lengths.");
            }
#endif
        }
    }
}
