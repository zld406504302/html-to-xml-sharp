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
                Log.InfoFormat("XML:\n{0}", XmlExtractor.ToXml("<html><body>Hello world</body></html>"));
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
            }
        }
    }
}
