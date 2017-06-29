using System;
using System.IO;

namespace NetCoreLab
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = Console.ReadLine();
            while (!input.Equals("exit"))
            {
                if (input.Equals("template"))
                {
                    using (var f = File.OpenRead(@"C:\template.txt"))
                    {
                        TemplateInterpreter interpreter = new TemplateInterpreter(f);
                        Console.WriteLine(interpreter.ResolveTemplate());
                    }
                }

                input = Console.ReadLine();
            }
        }
    }
}