﻿using System;

namespace BlazorComponent.Doc.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                return new CliWorker().Execute(args);
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred. {e.ToString()}");
                return 1;
            }
        }
    }
}
