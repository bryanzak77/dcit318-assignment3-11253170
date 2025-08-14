using System;
using System.Collections.Generic;
using System.IO;

namespace StudentGradingSystem
{
    // a. Student Class
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80) return "A";
            if (Score >= 70) return "B";
            if (Score >= 60) return "C";
            if (Score >= 50) return "D";
            return "F";
        }
    }

    // b. Custom Exception: InvalidScoreFormatException
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    // c. Custom Exception: MissingFieldException
    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // d. StudentResultProcessor Class
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            List<Student> students = new List<Student>();
            using (StreamReader sr = new StreamReader(inputFilePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] fields = line.Split(',');
                    if (fields.Length < 3)
                    {
                        throw new MissingFieldException($"Incomplete record found: '{line}'. Expected 3 fields, got {fields.Length}.");
                    }

                    int id;
                    string fullName;
                    int score;

                    // It's good practice to try to parse the Id as well
                    if (!int.TryParse(fields[0].Trim(), out id))
                    {
                        // Handle potential error for ID format
                        // This wasn't explicitly asked but is a good practice.
                        // We'll proceed with the score format validation as per the prompt.
                    }

                    fullName = fields[1].Trim();

                    if (!int.TryParse(fields[2].Trim(), out score))
                    {
                        throw new InvalidScoreFormatException($"Invalid score format for student '{fullName}'. Score value was: '{fields[2]}'.");
                    }

                    students.Add(new Student(id, fullName, score));
                }
            }
            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (StreamWriter sw = new StreamWriter(outputFilePath))
            {
                foreach (Student student in students)
                {
                    sw.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }
    }

    // e. Main Application Flow
    public class Program
    {
        public static void Main(string[] args)
        {
            string inputFilePath = "students.txt";
            string outputFilePath = "results.txt";

            // Create a sample input file for testing purposes.
            File.WriteAllText(inputFilePath,
                "101,Ama Addo,84\n" +
                "102,Kwaku Asamoah,71\n" +
                "103,Emmanuel Aboagye,65\n" +
                "104,Diana Asamoah,58\n" +
                "105,Serwaa Asante,49\n" +
                "106,Frank Addy,invalid_score\n" + // Invalid score for testing
                "107,Grace,88" // Missing field for testing
            );

            try
            {
                Console.WriteLine("Reading student data from file...");
                StudentResultProcessor processor = new StudentResultProcessor();
                List<Student> students = processor.ReadStudentsFromFile(inputFilePath);

                Console.WriteLine("Generating a summary report...");
                processor.WriteReportToFile(students, outputFilePath);

                Console.WriteLine($"Process completed successfully. Report saved to '{outputFilePath}'.");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: The input file was not found. Details: {ex.Message}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Data Error: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Data Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}