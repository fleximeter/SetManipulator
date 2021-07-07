using System;
using System.Collections.Generic;
using System.Text;
using MusicTheory;

namespace SetManipulator
{
    class RowTool
    {
        Row _row = new Row();
        Matrix _matrix = new Matrix();
        bool _haveMatrix = false;

        public void RowMenu()
        {
            string input;
            DisplayMenu();
            do
            {
                Console.Write("> ");
                input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        EnterRow();
                        Console.WriteLine();
                        break;
                    case "2":
                        LoadRandomRow();
                        Console.WriteLine();
                        break;
                    case "3":
                        LoadAIR();
                        Console.WriteLine();
                        break;
                    case "4":
                        DisplayMatrix();
                        Console.WriteLine();
                        break;
                    case "5":
                        DisplayForm();
                        Console.WriteLine();
                        break;
                    case "6":
                        SearchForms();
                        Console.WriteLine();
                        break;
                    case "7":
                        ContinuousSearch();
                        break;
                    case "m":
                    case "M":
                        DisplayMenu();
                        break;
                    case "r":
                    case "R":
                        break;
                    default:
                        Console.WriteLine("Invalid entry");
                        break;
                }
            } while (input != "r");
        }

        void DisplayMenu()
        {
            Console.Write("\n1) Enter row\n2) Load random row\n3) Load random all interval row\n4) Display matrix\n5) Display row form(s)\n" +
                    "6) Search row forms\n7) Continuous row search mode\nm) Display this menu\nr) Return\n");
        }

        void EnterRow()
        {
            string input;
            List<PitchClass> pc;
            Console.Write("Enter row\n> ");
            input = Console.ReadLine();
            pc = Functions.ConvertPCList(input);
            if (Row.IsValidRow(pc))
            {
                _row.ImportRow(pc);
                _matrix.ImportRow(_row);
                _haveMatrix = true;
            }
            else
                Console.WriteLine("Invalid row");
        }

        void LoadRandomRow()
        {
            _row.LoadRandomRow();
            _matrix.ImportRow(_row);
            Console.WriteLine("Row: " + FormatRow(_row));
            _haveMatrix = true;
        }

        void LoadAIR()
        {
            System.IO.StreamReader sr = new System.IO.StreamReader("eleven_interval.txt");
            System.Random rand = new System.Random();
            PitchClass pc = new PitchClass();
            int index = rand.Next(0, 3856);
            int[] rGen = new int[11];
            int rGenIndex = 0;
            string line = "";
            for (int i = 0; i < index; i++)
                line = sr.ReadLine();
            foreach (char c in line)
            {
                if (PitchClass.IsValidPitchClassChar(c))
                {
                    pc.PitchClassChar = c;
                    rGen[rGenIndex] = pc.PitchClassInteger;
                    rGenIndex++;
                }
            }
            _row.LoadRowFromRGen(rGen);
            _matrix.ImportRow(_row);
            Console.WriteLine("Row: " + FormatRow(_row));
            _haveMatrix = true;
        }

        void DisplayMatrix()
        {
            if (_haveMatrix)
            {
                Console.WriteLine(string.Format("    {0,-5}{1,-5}{2,-5}{3,-5}{4,-5}{5,-5}{6,-5}{7,-5}{8,-5}{9,-5}{10,-5}{11,-5}",
                    _matrix.GetTopColumnNameString(0), _matrix.GetTopColumnNameString(1), _matrix.GetTopColumnNameString(2),
                    _matrix.GetTopColumnNameString(3), _matrix.GetTopColumnNameString(4), _matrix.GetTopColumnNameString(5),
                    _matrix.GetTopColumnNameString(6), _matrix.GetTopColumnNameString(7), _matrix.GetTopColumnNameString(8),
                    _matrix.GetTopColumnNameString(9), _matrix.GetTopColumnNameString(10), _matrix.GetTopColumnNameString(11)));
                Console.WriteLine();

                for (int i = 0; i < 12; i++)
                {
                    Console.WriteLine(string.Format("{0,-4}{1,-5}{2,-5}{3,-5}{4,-5}{5,-5}{6,-5}{7,-5}{8,-5}{9,-5}{10,-5}{11,-5}{12,-5}{13,-4}",
                        _matrix.GetLeftRowNameString(i), _matrix[i, 0].PitchClassChar, _matrix[i, 1].PitchClassChar, _matrix[i, 2].PitchClassChar,
                        _matrix[i, 3].PitchClassChar, _matrix[i, 4].PitchClassChar, _matrix[i, 5].PitchClassChar, _matrix[i, 6].PitchClassChar,
                        _matrix[i, 7].PitchClassChar, _matrix[i, 8].PitchClassChar, _matrix[i, 9].PitchClassChar, _matrix[i, 10].PitchClassChar,
                        _matrix[i, 11].PitchClassChar, _matrix.GetRightRowNameString(i)));
                    Console.WriteLine();
                }

                Console.WriteLine(string.Format("    {0,-5}{1,-5}{2,-5}{3,-5}{4,-5}{5,-5}{6,-5}{7,-5}{8,-5}{9,-5}{10,-5}{11,-5}",
                    _matrix.GetBottomColumnNameString(0), _matrix.GetBottomColumnNameString(1), _matrix.GetBottomColumnNameString(2),
                    _matrix.GetBottomColumnNameString(3), _matrix.GetBottomColumnNameString(4), _matrix.GetBottomColumnNameString(5),
                    _matrix.GetBottomColumnNameString(6), _matrix.GetBottomColumnNameString(7), _matrix.GetBottomColumnNameString(8),
                    _matrix.GetBottomColumnNameString(9), _matrix.GetBottomColumnNameString(10), _matrix.GetBottomColumnNameString(11)));
            }
            else
                Console.WriteLine("You must enter a row before displaying the matrix.");
        }

        void DisplayForm()
        {
            string input;
            string[] forms;
            Row secondaryForm;
            Console.Write("Enter row form(s)\n> ");
            input = Console.ReadLine().Trim();
            if (input.Contains(','))
                forms = input.Split(',');
            else
                forms = input.Split(' ');
            foreach (string form in forms)
            {
                string trimForm = form.Trim();
                if (trimForm.Length > 0)
                {
                    if (PcSeg.IsValidTransformation(trimForm))
                    {
                        secondaryForm = _row.Transform(trimForm);
                        Console.WriteLine(string.Format("{0,-6}{1}", trimForm + ": ", FormatRow(secondaryForm)));
                    }
                    else
                        Console.WriteLine(string.Format("{0,-6}{1}", trimForm + ": ", "Invalid row form"));
                }
            }
        }

        void SearchForms()
        {
            string input;
            List<PitchClass> pc;
            List<Pair<string, Row>> rowForms;
            Console.Write("Enter pitch classes\n> ");
            input = Console.ReadLine();
            pc = Functions.ConvertPCList(input);
            rowForms = _row.GetSecondaryForms(pc);
            foreach (Pair<string, Row> rowForm in rowForms)
            {
                Console.Write(string.Format("{0,-6}", rowForm.Item1 + ":"));
                Console.WriteLine(FormatRow(rowForm.Item2));
            }
            if (rowForms.Count == 0)
                Console.WriteLine("No row forms match your search term.");
        }

        void ContinuousSearch()
        {
            string input;
            List<PitchClass> pc;
            List<Pair<string, Row>> rowForms;
            Console.Write("Enter pitch classes (q to quit)\n> ");
            input = Console.ReadLine();
            while (input != "q" && input != "Q")
            {
                pc = Functions.ConvertPCList(input);
                rowForms = _row.GetSecondaryForms(pc);
                foreach (Pair<string, Row> rowForm in rowForms)
                {
                    Console.Write(string.Format("{0,-6}", rowForm.Item1 + ":"));
                    Console.WriteLine(FormatRow(rowForm.Item2));
                }
                if (rowForms.Count == 0)
                    Console.WriteLine("No row forms match your search term.");
                Console.Write("Enter pitch classes (q to quit)\n> ");
                input = Console.ReadLine();
            }
        }

        string FormatRow(Row row)
        {
            string sRow = "";
            for (int i = 0; i < 11; i++)
                sRow = sRow + row[i].PitchClassChar + ' ';
            sRow += row[11].PitchClassChar;
            return sRow;
        }
    }
}
