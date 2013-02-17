HTML to XML
===========

Informal conversion of HTML to XML. Useful for screen-scraping with XPath.

Build using Visual Studio 2012. Note that the log4net logging library is installed via NuGet.

Use the static ToXml method of the XmlExtractor class to convert an HTML string to an XML string.

    String xml = TagParser.XmlExtractor.ToXml("<html><body>Hello world</body></html>");
