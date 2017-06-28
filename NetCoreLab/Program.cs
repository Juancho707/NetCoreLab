using System;

namespace NetCoreLab
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = Console.ReadLine();
            while (!input.Equals("exit"))
            {                 
                TemplateInterpreter interpreter = new TemplateInterpreter(input);
                //Console.WriteLine(interpreter.ApplyTemplate());
                interpreter.GetIfStatement(input);
                input = Console.ReadLine();
            }
        }
    }
}