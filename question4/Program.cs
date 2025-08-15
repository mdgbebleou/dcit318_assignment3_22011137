using System;
using System.Collections.Generic;
using System.IO;

// =============================
// 1. Student Class
// =============================
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

    // Determine grade based on score
    public string GetGrade()
    {
        return Score switch
        {
            >= 80 and <= 100 => "A",
            >= 70 => "B",
            >= 60 => "C",
            >= 50 => "D",
            < 50 => "F",
            _ => "F" // fallback
        };
    }

    public override string ToString()
    {
        return $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
    }
}

// =============================
// 2. Custom Exceptions
// =============================
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// =============================
// 3. StudentResultProcessor - Handles file I/O and processing
// =============================
public class StudentResultProcessor
{
    // Read students from input file
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();

        using (var reader = new StreamReader(inputFilePath))
        {
            string line;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                line = line.Trim();
                if (string.IsNullOrEmpty(line)) continue; // Skip empty lines

                var parts = line.Split(',');
                if (parts.Length < 3)
                {
                    throw new MissingFieldException($"Line {lineNumber}: Expected 3 fields, found {parts.Length}. Content: '{line}'");
                }

                string idStr = parts[0].Trim();
                string fullName = parts[1].Trim();
                string scoreStr = parts[2].Trim();

                if (string.IsNullOrEmpty(idStr) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(scoreStr))
                {
                    throw new MissingFieldException($"Line {lineNumber}: One or more fields are empty.");
                }

                if (!int.TryParse(idStr, out int id))
                {
                    throw new FormatException($"Line {lineNumber}: Invalid Student ID format: '{idStr}'");
                }

                if (!int.TryParse(scoreStr, out int score))
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Score is not a valid integer: '{scoreStr}'");
                }

                if (score < 0 || score > 100)
                {
                    Console.WriteLine($"Warning: Score {score} for {fullName} is outside typical range (0–100), but accepted.");
                }

                students.Add(new Student(id, fullName, score));
            }
        }

        return students;
    }

    // Write formatted report to output file
    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            writer.WriteLine("=== STUDENT GRADE REPORT ===\n");
            foreach (var student in students)
            {
                writer.WriteLine(student.ToString());
            }
            writer.WriteLine($"\nTotal Students Processed: {students.Count}");
        }
    }
}

// =============================
// 4. Main Application
// =============================
class Program
{
    static void Main()
    {
        var processor = new StudentResultProcessor();
        string inputPath = "grades_input.txt";
        string outputPath = "grades_report.txt";

        try
        {
            Console.WriteLine("🔍 Reading student data from file...");
            var students = processor.ReadStudentsFromFile(inputPath);

            Console.WriteLine($"✅ Successfully loaded {students.Count} students.");
            Console.WriteLine("📝 Generating report...");

            processor.WriteReportToFile(students, outputPath);

            Console.WriteLine($"✅ Report saved to '{outputPath}'");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"❌ Error: Input file '{inputPath}' not found. Please make sure the file exists.");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"❌ Invalid Score Format: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"❌ Missing Data: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"❌ Format Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ An unexpected error occurred: {ex.Message}");
        }
    }
}