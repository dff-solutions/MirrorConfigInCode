using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorConfigInCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigSettings();
            foreach (var target in config.DeployTargets)
            {
                Console.WriteLine(string.Format("Target Name: {0},\t Is Active: {1},\t ServerName: {2}", target.name, target.isactive, target.servername));
            }
            Console.WriteLine("Press Escape to Exit");
            if (Console.ReadKey() != null)
            {
                return;
            }
        }
    }
}
