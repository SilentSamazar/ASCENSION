using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CatalogDropTables : MonoBehaviour
{
    public Dictionary<string, List<string[]>> Tables = new Dictionary<string, List<string[]>>() { }; // This is all our drop tables. It is filled with every tsv in the DropTables folder.
   
    public void CreateDictionary()
    {
        string[] info = Directory.GetFiles("Assets/Resources/DropTables", "*.tsv");
        foreach (string s in info)
        {
            string[,] Table = LoadTsv(s); //Load tsv file into a 2D array of strings
            
            List<string[]> DropList = new List<string[]>(); //Dummy list to fill with part strings

            for (int c = 0; c < Table.GetLength(0); c++) // Once per row in the table
            {
                string[] ParticularPartInfo = new string[Table.GetLength(1)]; //Initialize placeholder for one specific part's info. Must be in the loop to work.
                for (int h = 0; h < Table.GetLength(1); h++)
                {
                    ParticularPartInfo[h] = Table[c, h]; //Make PPI into the part's info
                                                         //Debug.Log(ParticularPartInfo[h]);
                                                         //Debug.Log(Table[c, h]);
                }
                DropList.Add(ParticularPartInfo); //Add this array to the drop table's list
            }

            

            Tables.Add(Path.GetFileNameWithoutExtension(s), DropList); //Add the list to the dictionary, named after the file. (No extension)
            //Debug.Log("Component table now contains " + Path.GetFileNameWithoutExtension(s) + " with first entries " + DropList[0][0] + DropList[1][0] + DropList[2][0]);
            //Debug.Log("With each entry " + DropList[0].Length + " items wide");
            //Debug.Log("and a total list of " + DropList.Count + " entries.");
            //Debug.Log("The KEY for this table is " + Path.GetFileNameWithoutExtension(s));
            //Debug.Log("The Component Tables Dictionary now has " + Tables.Count + " tables stored within it." );

        }

        //Debug.Log("Current Drop Tables are: ");
        //foreach (KeyValuePair<string, List<string[]>> table in Tables)
        //{
        //    Debug.Log(table.Key);
        //}

    }

    void GrabTable(string Filename)
    {
        string[,] Table = LoadTsv(Filename);
        string[] ParticularPartInfo = new string[Table.GetLength(1)];

        for (int c = 0; c < Table.GetLength(0); c++)
        {
            int[] Atts = new int[Table.GetLength(1)];
            for (int h = 0; h < Table.GetLength(1); h++)
            {
                    Atts[h] = int.Parse(Table[c, h]);
            }
        }
    }

    private string[,] LoadTsv(string filename) //Use @ASSETS/whatever
    {
        // Get the file's text.
        string whole_file = System.IO.File.ReadAllText(filename);

        // Split into lines.
        whole_file = whole_file.Replace("\n", "");
        string[] lines = whole_file.Split(new char[] { '\r' });

        // See how many rows and columns there are.
        int num_rows = lines.Length;
        int num_cols = lines[0].Split('\t').Length;

        // Allocate the data array.
        string[,] values = new string[num_rows, num_cols];

        // Load the array.
        for (int r = 0; r < num_rows; r++)
        {
            string[] line_r = lines[r].Split('\t');
            for (int c = 0; c < num_cols; c++)
            {
                values[r, c] = line_r[c];
            }
        }

        // Return the values.

        return values;
    }
}
