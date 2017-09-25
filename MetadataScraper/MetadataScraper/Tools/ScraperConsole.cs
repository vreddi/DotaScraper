using System;

namespace MetadataScraper
{
    class ScraperConsole
    {
        public enum ExceptionType {
            Default,
            NotFound
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Logs info message with some key information in the end of the message
        /// </summary>
        /// <param name="message">Text to log</param>
        /// <param name="key">Key text to log in the end</param>
        /// <param name="keyColor">Color fo the key info</param>
        public static void LogKeyInfo(string message, string key, ConsoleColor keyColor = ConsoleColor.Cyan) {
            Console.Write(message);
            Console.ForegroundColor = keyColor;
            Console.WriteLine(key);
            Console.ResetColor();
        }

        /// <summary>
        /// Logs an info message on the console with app's pre configured settings
        /// </summary>
        /// <param name="message">Text to log</param>
        public static void LogInfo(string message) {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Logs an error message on the console with app's pre configured settings
        /// </summary>
        /// <param name="message">Text to log</param>
        public static void LogError(string message, ExceptionType exception = ExceptionType.Default)
        {
            string exceptionType = "";

            if (exception != ExceptionType.Default) {
                exceptionType = exception.ToString();
            }

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(string.Format("EXCEPTION {0}: {1}", exceptionType, message));
            Console.ResetColor();
        }
    }
}
