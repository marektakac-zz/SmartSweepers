using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSweepersSlimDX
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SmartSweepers2D smartSweepers2D = new SmartSweepers2D())
            {
                smartSweepers2D.Run();
            }
        }
    }
}
