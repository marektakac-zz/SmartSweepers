using System;

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
