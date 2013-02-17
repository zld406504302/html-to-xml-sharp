using System;
using System.Reflection;
using TagParser;
using log4net;

namespace TagParserTest
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            try
            {
                XmlExtractor.ToXml("<html><body>Hello world</body></html>");
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
            }

            Console.Out.Write("Press any key");
            Console.ReadKey();
        }
    }
}
