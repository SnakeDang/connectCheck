using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SignalRClient
{
  public static class LogAndReadFile
  {
    public static string ReadContentFile(string filePath)
    {
      try
      {
        string content = File.ReadAllText(filePath);
        return content.Split(";")[1];
      }
      catch (IOException e)
      {
        Console.WriteLine($"An error occurred: {e.Message}");
        return "error";
      }

    }

    public static void LogErrorToFile(string fileName, string errorMessage)
    {
      try
      {
        string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
        string logFilePath = Path.Combine(directoryPath, fileName);
        // Kiểm tra xem tệp lỗi đã tồn tại chưa
        if (!File.Exists(logFilePath))
        {
          // Nếu không tồn tại, tạo một tệp mới
          using (StreamWriter writer = File.CreateText(logFilePath))
          {
            writer.WriteLine($"[{DateTime.Now}] Error: {errorMessage}");
          }
        }
        else
        {
          // Nếu tệp đã tồn tại, ghi lỗi vào cuối tệp
          using (StreamWriter writer = new StreamWriter(logFilePath, true))
          {
            writer.WriteLine($"[{DateTime.Now}] Error: {errorMessage}");
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine($"Failed to write to log file: {e.Message}");
      }
    }
  }
}
