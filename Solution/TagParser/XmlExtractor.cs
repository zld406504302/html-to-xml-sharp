using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TagParser.Tokens;
using log4net;

namespace TagParser
{
    public class XmlExtractor
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Mapping entity names to decimal values, to be used in converting entities not supported in XML.
        /// </summary>
        private static readonly Dictionary<string, int> EntityMappings;

        /// <summary>
        /// These elements are always empty elements. End-tag is redundant for these.
        /// </summary>
        private static readonly List<string> EmptyElements;

        /// <summary>
        /// These elements appear only once in an HTML document.
        /// </summary>
        private static readonly List<string> SingleElements;

        /// <summary>
        /// These entities are the only entities supported by default in XML.
        /// </summary>
        private static readonly List<string> XmlEntities;

        static XmlExtractor()
        {
            EntityMappings = new Dictionary<string, int>();
            MapHtmlEntities();
            EmptyElements = new List<string> { "br", "meta", "link", "base", "input", "img", "area" };
            SingleElements = new List<string> { "html", "head", "body" };
            XmlEntities = new List<string> { "amp", "lt", "gt", "quot", "apos" };
        }

        /// <summary>
        /// This method maps all the known HTML entities with decimal code, for conversion purposes.
        /// </summary>
        private static void MapHtmlEntities()
        {
            // Latin-1 Entities
            // http://htmlhelp.com/reference/html40/entities/latin1.html
            MapHtmlEntity("nbsp", 160);     // no-break space = non-breaking space
            MapHtmlEntity("iexcl", 161);    // inverted exclamation mark
            MapHtmlEntity("cent", 162);     // cent sign
            MapHtmlEntity("pound", 163);    // pound sign
            MapHtmlEntity("curren", 164);   // currency sign
            MapHtmlEntity("yen", 165);      // yen sign = yuan sign
            MapHtmlEntity("brvbar", 166);   // broken bar = broken vertical bar
            MapHtmlEntity("sect", 167);     // section sign
            MapHtmlEntity("uml", 168);      // diaeresis = spacing diaeresis
            MapHtmlEntity("copy", 169);     // copyright sign
            MapHtmlEntity("ordf", 170);     // feminine ordinal indicator
            MapHtmlEntity("laquo", 171);    // left-pointing double angle quotation mark = left pointing guillemet
            MapHtmlEntity("not", 172);      // not sign
            MapHtmlEntity("shy", 173);      // soft hyphen = discretionary hyphen
            MapHtmlEntity("reg", 174);      // registered sign = registered trade mark sign
            MapHtmlEntity("macr", 175);     // macron = spacing macron = overline = APL overbar
            MapHtmlEntity("deg", 176);      // degree sign
            MapHtmlEntity("plusmn", 177);   // plus-minus sign = plus-or-minus sign
            MapHtmlEntity("sup2", 178);     // superscript two = superscript digit two = squared
            MapHtmlEntity("sup3", 179);     // superscript three = superscript digit three = cubed
            MapHtmlEntity("acute", 180);    // acute accent = spacing acute
            MapHtmlEntity("micro", 181);    // micro sign
            MapHtmlEntity("para", 182);     // pilcrow sign = paragraph sign
            MapHtmlEntity("middot", 183);   // middle dot = Georgian comma = Greek middle dot
            MapHtmlEntity("cedil", 184);    // cedilla = spacing cedilla
            MapHtmlEntity("sup1", 185);     // superscript one = superscript digit one
            MapHtmlEntity("ordm", 186);     // masculine ordinal indicator
            MapHtmlEntity("raquo", 187);    // right-pointing double angle quotation mark = right pointing guillemet
            MapHtmlEntity("frac14", 188);   // vulgar fraction one quarter = fraction one quarter
            MapHtmlEntity("frac12", 189);   // vulgar fraction one half = fraction one half
            MapHtmlEntity("frac34", 190);   // vulgar fraction three quarters = fraction three quarters
            MapHtmlEntity("iquest", 191);   // inverted question mark = turned question mark
            MapHtmlEntity("Agrave", 192);   // Latin capital letter A with grave = Latin capital letter A grave
            MapHtmlEntity("Aacute", 193);   // Latin capital letter A with acute
            MapHtmlEntity("Acirc", 194);    // Latin capital letter A with circumflex
            MapHtmlEntity("Atilde", 195);   // Latin capital letter A with tilde
            MapHtmlEntity("Auml", 196);     // Latin capital letter A with diaeresis
            MapHtmlEntity("Aring", 197);    // Latin capital letter A with ring above = Latin capital letter A ring
            MapHtmlEntity("AElig", 198);    // Latin capital letter AE = Latin capital ligature AE
            MapHtmlEntity("Ccedil", 199);   // Latin capital letter C with cedilla
            MapHtmlEntity("Egrave", 200);   // Latin capital letter E with grave
            MapHtmlEntity("Eacute", 201);   // Latin capital letter E with acute
            MapHtmlEntity("Ecirc", 202);    // Latin capital letter E with circumflex
            MapHtmlEntity("Euml", 203);     // Latin capital letter E with diaeresis
            MapHtmlEntity("Igrave", 204);   // Latin capital letter I with grave
            MapHtmlEntity("Iacute", 205);   // Latin capital letter I with acute
            MapHtmlEntity("Icirc", 206);    // Latin capital letter I with circumflex
            MapHtmlEntity("Iuml", 207);     // Latin capital letter I with diaeresis
            MapHtmlEntity("ETH", 208);      // Latin capital letter ETH
            MapHtmlEntity("Ntilde", 209);   // Latin capital letter N with tilde
            MapHtmlEntity("Ograve", 210);   // Latin capital letter O with grave
            MapHtmlEntity("Oacute", 211);   // Latin capital letter O with acute
            MapHtmlEntity("Ocirc", 212);    // Latin capital letter O with circumflex
            MapHtmlEntity("Otilde", 213);   // Latin capital letter O with tilde
            MapHtmlEntity("Ouml", 214);     // Latin capital letter O with diaeresis
            MapHtmlEntity("times", 215);    // multiplication sign
            MapHtmlEntity("Oslash", 216);   // Latin capital letter O with stroke = Latin capital letter O slash
            MapHtmlEntity("Ugrave", 217);   // Latin capital letter U with grave
            MapHtmlEntity("Uacute", 218);   // Latin capital letter U with acute
            MapHtmlEntity("Ucirc", 219);    // Latin capital letter U with circumflex
            MapHtmlEntity("Uuml", 220);     // Latin capital letter U with diaeresis
            MapHtmlEntity("Yacute", 221);   // Latin capital letter Y with acute
            MapHtmlEntity("THORN", 222);    // Latin capital letter THORN
            MapHtmlEntity("szlig", 223);    // Latin small letter sharp s = ess-zed
            MapHtmlEntity("agrave", 224);   // Latin small letter a with grave = Latin small letter a grave
            MapHtmlEntity("aacute", 225);   // Latin small letter a with acute
            MapHtmlEntity("acirc", 226);    // Latin small letter a with circumflex
            MapHtmlEntity("atilde", 227);   // Latin small letter a with tilde
            MapHtmlEntity("auml", 228);     // Latin small letter a with diaeresis
            MapHtmlEntity("aring", 229);    // Latin small letter a with ring above = Latin small letter a ring
            MapHtmlEntity("aelig", 230);    // Latin small letter ae = Latin small ligature ae
            MapHtmlEntity("ccedil", 231);   // Latin small letter c with cedilla
            MapHtmlEntity("egrave", 232);   // Latin small letter e with grave
            MapHtmlEntity("eacute", 233);   // Latin small letter e with acute
            MapHtmlEntity("ecirc", 234);    // Latin small letter e with circumflex
            MapHtmlEntity("euml", 235);     // Latin small letter e with diaeresis
            MapHtmlEntity("igrave", 236);   // Latin small letter i with grave
            MapHtmlEntity("iacute", 237);   // Latin small letter i with acute
            MapHtmlEntity("icirc", 238);    // Latin small letter i with circumflex
            MapHtmlEntity("iuml", 239);     // Latin small letter i with diaeresis
            MapHtmlEntity("eth", 240);      // Latin small letter eth
            MapHtmlEntity("ntilde", 241);   // Latin small letter n with tilde
            MapHtmlEntity("ograve", 242);   // Latin small letter o with grave
            MapHtmlEntity("oacute", 243);   // Latin small letter o with acute
            MapHtmlEntity("ocirc", 244);    // Latin small letter o with circumflex
            MapHtmlEntity("otilde", 245);   // Latin small letter o with tilde
            MapHtmlEntity("ouml", 246);     // Latin small letter o with diaeresis
            MapHtmlEntity("divide", 247);   // division sign
            MapHtmlEntity("oslash", 248);   // Latin small letter o with stroke = Latin small letter o slash
            MapHtmlEntity("ugrave", 249);   // Latin small letter u with grave
            MapHtmlEntity("uacute", 250);   // Latin small letter u with acute
            MapHtmlEntity("ucirc", 251);    // Latin small letter u with circumflex
            MapHtmlEntity("uuml", 252);     // Latin small letter u with diaeresis
            MapHtmlEntity("yacute", 253);   // Latin small letter y with acute
            MapHtmlEntity("thorn", 254);    // Latin small letter thorn
            MapHtmlEntity("yuml", 255);     // Latin small letter y with diaeresis

            // Entities for Symbols and Greek Letters.
            // http://htmlhelp.com/reference/html40/entities/symbols.html
            MapHtmlEntity("fnof", 402);     // Latin small f with hook = function = florin
            MapHtmlEntity("Alpha", 913);    // Greek capital letter alpha
            MapHtmlEntity("Beta", 914);     // Greek capital letter beta
            MapHtmlEntity("Gamma", 915);    // Greek capital letter gamma
            MapHtmlEntity("Delta", 916);    // Greek capital letter delta
            MapHtmlEntity("Epsilon", 917);  // Greek capital letter epsilon
            MapHtmlEntity("Zeta", 918);     // Greek capital letter zeta
            MapHtmlEntity("Eta", 919);      // Greek capital letter eta
            MapHtmlEntity("Theta", 920);    // Greek capital letter theta
            MapHtmlEntity("Iota", 921);     // Greek capital letter iota
            MapHtmlEntity("Kappa", 922);    // Greek capital letter kappa
            MapHtmlEntity("Lambda", 923);   // Greek capital letter lambda
            MapHtmlEntity("Mu", 924);       // Greek capital letter mu
            MapHtmlEntity("Nu", 925);       // Greek capital letter nu
            MapHtmlEntity("Xi", 926);       // Greek capital letter xi
            MapHtmlEntity("Omicron", 927);  // Greek capital letter omicron
            MapHtmlEntity("Pi", 928);       // Greek capital letter pi
            MapHtmlEntity("Rho", 929);      // Greek capital letter rho
            MapHtmlEntity("Sigma", 931);    // Greek capital letter sigma
            MapHtmlEntity("Tau", 932);      // Greek capital letter tau
            MapHtmlEntity("Upsilon", 933);  // Greek capital letter upsilon
            MapHtmlEntity("Phi", 934);      // Greek capital letter phi
            MapHtmlEntity("Chi", 935);      // Greek capital letter chi
            MapHtmlEntity("Psi", 936);      // Greek capital letter psi
            MapHtmlEntity("Omega", 937);    // Greek capital letter omega
            MapHtmlEntity("alpha", 945);    // Greek small letter alpha
            MapHtmlEntity("beta", 946);     // Greek small letter beta
            MapHtmlEntity("gamma", 947);    // Greek small letter gamma
            MapHtmlEntity("delta", 948);    // Greek small letter delta
            MapHtmlEntity("epsilon", 949);  // Greek small letter epsilon
            MapHtmlEntity("zeta", 950);     // Greek small letter zeta
            MapHtmlEntity("eta", 951);      // Greek small letter eta
            MapHtmlEntity("theta", 952);    // Greek small letter theta
            MapHtmlEntity("iota", 953);     // Greek small letter iota
            MapHtmlEntity("kappa", 954);    // Greek small letter kappa
            MapHtmlEntity("lambda", 955);   // Greek small letter lambda
            MapHtmlEntity("mu", 956);       // Greek small letter mu
            MapHtmlEntity("nu", 957);       // Greek small letter nu
            MapHtmlEntity("xi", 958);       // Greek small letter xi
            MapHtmlEntity("omicron", 959);  // Greek small letter omicron
            MapHtmlEntity("pi", 960);       // Greek small letter pi
            MapHtmlEntity("rho", 961);      // Greek small letter rho
            MapHtmlEntity("sigmaf", 962);   // Greek small letter final sigma
            MapHtmlEntity("sigma", 963);    // Greek small letter sigma
            MapHtmlEntity("tau", 964);      // Greek small letter tau
            MapHtmlEntity("upsilon", 965);  // Greek small letter upsilon
            MapHtmlEntity("phi", 966);      // Greek small letter phi
            MapHtmlEntity("chi", 967);      // Greek small letter chi
            MapHtmlEntity("psi", 968);      // Greek small letter psi
            MapHtmlEntity("omega", 969);    // Greek small letter omega
            MapHtmlEntity("thetasym", 977); // Greek small letter theta symbol
            MapHtmlEntity("upsih", 978);    // Greek upsilon with hook symbol
            MapHtmlEntity("piv", 982);      // Greek pi symbol
            MapHtmlEntity("bull", 8226);    // bullet = black small circle
            MapHtmlEntity("hellip", 8230);  // horizontal ellipsis = three dot leader
            MapHtmlEntity("prime", 8242);   // prime = minutes = feet
            MapHtmlEntity("Prime", 8243);   // double prime = seconds = inches
            MapHtmlEntity("oline", 8254);   // overline = spacing overscore
            MapHtmlEntity("weierp", 8472);  // script capital P = power set = Weierstrass p
            MapHtmlEntity("image", 8465);   // blackletter capital I = imaginary part
            MapHtmlEntity("real", 8476);    // blackletter capital R = real part symbol
            MapHtmlEntity("trade", 8482);   // trade mark sign
            MapHtmlEntity("alefsym", 8501); // alef symbol = first transfinite cardinal
            MapHtmlEntity("larr", 8592);    // leftwards arrow
            MapHtmlEntity("uarr", 8593);    // upwards arrow
            MapHtmlEntity("rarr", 8594);    // rightwards arrow
            MapHtmlEntity("darr", 8595);    // downwards arrow
            MapHtmlEntity("harr", 8596);    // left right arrow
            MapHtmlEntity("crarr", 8629);   // downwards arrow with corner leftwards = carriage return
            MapHtmlEntity("lArr", 8656);    // leftwards double arrow
            MapHtmlEntity("uArr", 8657);    // upwards double arrow
            MapHtmlEntity("rArr", 8658);    // rightwards double arrow
            MapHtmlEntity("dArr", 8659);    // downwards double arrow
            MapHtmlEntity("hArr", 8660);    // left right double arrow
            MapHtmlEntity("forall", 8704);  // for all
            MapHtmlEntity("part", 8706);    // partial differential
            MapHtmlEntity("exist", 8707);   // there exists
            MapHtmlEntity("empty", 8709);   // empty set = null set = diameter
            MapHtmlEntity("nabla", 8711);   // nabla = backward difference
            MapHtmlEntity("isin", 8712);    // element of
            MapHtmlEntity("notin", 8713);   // not an element of
            MapHtmlEntity("ni", 8715);      // Contains as member
            MapHtmlEntity("prod", 8719);    // n-ary product = product sign
            MapHtmlEntity("sum", 8721);     // n-ary sumation
            MapHtmlEntity("minus", 8722);   // minus sign
            MapHtmlEntity("lowast", 8727);  // asterisk operator
            MapHtmlEntity("radic", 8730);   // square root = radical sign
            MapHtmlEntity("prop", 8733);    // proportional to
            MapHtmlEntity("infin", 8734);   // infinity
            MapHtmlEntity("ang", 8736);     // angle
            MapHtmlEntity("and", 8743);     // logical and = wedge
            MapHtmlEntity("or", 8744);      // logical or = vee
            MapHtmlEntity("cap", 8745);     // intersection = cap
            MapHtmlEntity("cup", 8746);     // union = cup
            MapHtmlEntity("int", 8747);     // integral
            MapHtmlEntity("there4", 8756);  // therefore
            MapHtmlEntity("sim", 8764);     // tilde operator = varies with = similar to
            MapHtmlEntity("cong", 8773);    // approximately equal to
            MapHtmlEntity("asymp", 8776);   // almost equal to = asymptotic to
            MapHtmlEntity("ne", 8800);      // not equal to
            MapHtmlEntity("equiv", 8801);   // identical to
            MapHtmlEntity("le", 8804);      // less-than or equal to
            MapHtmlEntity("ge", 8805);      // greater-than or equal to
            MapHtmlEntity("sub", 8834);     // subset of
            MapHtmlEntity("sup", 8835);     // superset of
            MapHtmlEntity("nsub", 8836);    // not a subset of
            MapHtmlEntity("sube", 8838);    // subset of or equal to
            MapHtmlEntity("supe", 8839);    // superset of or equal to
            MapHtmlEntity("oplus", 8853);   // circled plus = direct sum
            MapHtmlEntity("otimes", 8855);  // circled times = vector product
            MapHtmlEntity("perp", 8869);    // up tack = orthogonal to = perpendicular
            MapHtmlEntity("sdot", 8901);    // dot operator
            MapHtmlEntity("lceil", 8968);   // left ceiling = APL upstile
            MapHtmlEntity("rceil", 8969);   // right ceiling
            MapHtmlEntity("lfloor", 8970);  // left floor = APL downstile
            MapHtmlEntity("rfloor", 8971);  // right floor
            MapHtmlEntity("lang", 9001);    // left-pointing angle bracket = bra
            MapHtmlEntity("rang", 9002);    // right-pointing angle bracket = ket
            MapHtmlEntity("loz", 9674);     // lozenge
            MapHtmlEntity("spades", 9824);  // black spade suit
            MapHtmlEntity("clubs", 9827);   // black club suit = shamrock
            MapHtmlEntity("hearts", 9829);  // black heart suit = valentine
            MapHtmlEntity("diams", 9830);   // black diamond suit

            // Entities for other special characters.
            // http://htmlhelp.com/reference/html40/entities/special.html
            MapHtmlEntity("quot", 34);  	// quotation mark = APL quote
            MapHtmlEntity("amp", 38); 	    // ampersand
            MapHtmlEntity("lt", 60); 	    // less-than sign
            MapHtmlEntity("gt", 62); 	    // greater-than sign
            MapHtmlEntity("OElig", 338); 	// Latin capital ligature OE
            MapHtmlEntity("oelig", 339); 	// Latin small ligature oe
            MapHtmlEntity("Scaron", 352); 	// Latin capital letter S with caron
            MapHtmlEntity("scaron", 353); 	// Latin small letter s with caron
            MapHtmlEntity("Yuml", 376); 	// Latin capital letter Y with diaeresis
            MapHtmlEntity("circ", 710);     // modifier letter circumflex accent
            MapHtmlEntity("tilde", 732);    // small tilde
            MapHtmlEntity("ensp", 8194);    // en space
            MapHtmlEntity("emsp", 8195);    // em space
            MapHtmlEntity("thinsp", 8201);  // thin space
            MapHtmlEntity("zwnj", 8204);    // zero width non-joiner
            MapHtmlEntity("zwj", 8205);     // zero width joiner
            MapHtmlEntity("lrm", 8206);     // left-to-right mark
            MapHtmlEntity("rlm", 8207);     // right-to-left mark
            MapHtmlEntity("ndash", 8211);   // en dash
            MapHtmlEntity("mdash", 8212);   // em dash
            MapHtmlEntity("lsquo", 8216);   // left single quotation mark
            MapHtmlEntity("rsquo", 8217);   // right single quotation mark
            MapHtmlEntity("sbquo", 8218);   // single low-9 quotation mark
            MapHtmlEntity("ldquo", 8220);   // left double quotation mark
            MapHtmlEntity("rdquo", 8221);   // right double quotation mark
            MapHtmlEntity("bdquo", 8222);   // double low-9 quotation mark
            MapHtmlEntity("dagger", 8224);  // dagger
            MapHtmlEntity("Dagger", 8225);  // double dagger
            MapHtmlEntity("permil", 8240);  // per mille sign
            MapHtmlEntity("lsaquo", 8249);  // single left-pointing angle quotation mark
            MapHtmlEntity("rsaquo", 8250);  // single right-pointing angle quotation mark
            MapHtmlEntity("euro", 8364);    // euro sign
        }

        private static void MapHtmlEntity(string entity, int code)
        {
            EntityMappings.Add(entity, code);
        }

        public static string ToXml(string html)
        {
            StringBuilder result = new StringBuilder();

            // Standard XML file header, including entities that are likely to be used.
            result.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");

            ParseReader reader = new ParseReader(html);
            TagParser parser = new TagParser(reader);
            Stack<string> nestingStack = new Stack<string>();

            try
            {
                ParseToken token = parser.GetNextToken();

                // Ignore leading white-space.
                while (token is SpacesToken || token is NewlineToken || token is DoctypeToken)
                    token = parser.GetNextToken();

                while (!(token is EOFToken))
                {
                    Log.DebugFormat("Token = {0}", token);
                    if (token is TagToken)
                    {
                        TagToken t = (TagToken)token;
                        if (!t.Tag.IsEndTag)
                        {

                            // Deal with start-tag. Typically this will be new element nesting.
                            Tag startTag = t.Tag;
                            if (startTag is EmptyElement)
                            {
                                result.Append(startTag.ToString());
                            }
                            else
                            {

                                // Tags that are always empty elements are converted to empty elements here.
                                // Element names are pushed onto the stack to balance elements with missing end-tag.
                                string startTagName = startTag.Name.ToLower();
                                Log.DebugFormat("startTagName = {0}", startTagName);
                                if (EmptyElements.Contains(startTagName))
                                {
                                    result.Append(new EmptyElement(startTag));
                                }
                                else
                                {
                                    result.Append(startTag.ToString());
                                    nestingStack.Push(startTagName);
                                }
                            }
                        }
                        else
                        {

                            // Deal with end-tag.
                            Tag endTag = t.Tag;

                            // Remove the '/' from beginning of the tag-name for comparison.
                            string endTagName = endTag.Name.Substring(1).ToLower();
                            Log.DebugFormat("endTagName = {0}", endTagName);

                            // Ignore some end-tags for empty elements that are handled with or without empty element syntax.
                            if (EmptyElements.Contains(endTagName))
                            {
                                Log.InfoFormat("Ignoring redundant end-tag: {0}", endTagName);
                            }
                            else
                            {

                                // Keep element tags matched appropriately.
                                string peek = nestingStack.Peek();
                                if (peek == null)
                                {
                                    Log.WarnFormat("Ignoring extra content at end of document! </{0}> ({1})", endTagName, parser.GetCharacterPosition());
                                }
                                else
                                {
                                    if (peek.Equals(endTagName))
                                    {
                                        nestingStack.Pop();
                                    }
                                    else
                                    {

                                        // Pair all the previous unmatched tags for these important structural elements.
                                        // These elements appear only once, so should never be automatically closed.
                                        if (SingleElements.Contains(endTagName))
                                        {

                                            while (peek != endTagName)
                                            {
                                                StringBuilder endtag = (new StringBuilder()).Append("</").Append(peek).Append('>');
                                                Log.WarnFormat("Adding a missing end-tag! {0} ({1})", endtag, parser.GetCharacterPosition());
                                                result.Append(endtag);
                                                nestingStack.Pop();
                                                peek = nestingStack.Peek();
                                            }

                                            // Remove the current item from the stack, as it has been paired now.
                                            nestingStack.Pop();

                                        }
                                        else
                                        {

                                            // Insert a matching start-tag before the unbalanced end-tag found.
                                            StringBuilder startTag = (new StringBuilder()).Append("<").Append(endTagName).Append('>');
                                            Log.WarnFormat("Adding a missing start-tag! {0} ({1})", startTag, parser.GetCharacterPosition());
                                            result.Append(startTag);
                                        }
                                    }

                                    // Write the current element end-tag.
                                    result.Append("</").Append(endTagName).Append('>');
                                }
                            }
                        }
                    }
                    else if (token is WordToken)
                    {
                        WordToken t = (WordToken)token;
                        result.Append(t.Word);
                    }
                    else if (token is SpacesToken)
                    {
                        SpacesToken t = (SpacesToken)token;
                        result.Append(t.Spaces);
                    }
                    else if (token is NumberToken)
                    {
                        NumberToken t = (NumberToken)token;
                        result.Append(t.Number);
                    }
                    else if (token is EntityReferenceToken)
                    {
                        EntityReferenceToken t = (EntityReferenceToken)token;
                        result.Append(XmlEntity(t.Name));
                    }
                    else if (token is PunctuationToken)
                    {
                        PunctuationToken t = (PunctuationToken)token;
                        result.Append(t.Character);
                    }
                    else if (token is CharacterEntityToken)
                    {
                        CharacterEntityToken t = (CharacterEntityToken)token;
                        result.Append(t.Character);
                    }
                    else if (token is NewlineToken)
                    {
                        result.Append('\n');
                    }
                    else if (token is ScriptToken)
                    {
                        ScriptToken t = (ScriptToken)token;
                        if (t.Script.Length > 0)
                        {
							// Script element contents are often empty.
                            // NOTE: Removing any prior use of CDATA section in script, to avoid conflict.
                            string script = t.Script.Replace("<![CDATA[", "").Replace("]]>", "");
                            result.Append("/*<![CDATA[*/").Append(script).Append("/*]]>*/");
                        }
                    }
                    else if (token is CDataToken)
                    {
                        CDataToken t = (CDataToken)token;
                        result.Append("<![CDATA[").Append(t.Data).Append("]]>");
                    }
                    else if (token is CommentToken)
                    {
                        CommentToken t = (CommentToken)token;
                        result.Append("<!--").Append(t.Comment).Append("-->");
                    }
                    else if (token is DoctypeToken)
                    {
                        // Ignore.
                    }
                    else if (token is ProcessingInstructionToken)
                    {
                        // Ignore.
                    }
                    else
                    {
                        Log.WarnFormat("Unexpected token! {0}", token);
                    }
                    token = parser.GetNextToken();
                }

                Log.Info(parser.GetCompletionReport());
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                result = null;
            }

            return result == null ? null : result.ToString();
        }

        /// <summary>
        /// Convert entity reference for valid XML text.
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        private static string XmlEntity(string entityName)
        {
            if (XmlEntities.Contains(entityName))
                return (new StringBuilder())
                    .Append('&').Append(entityName).Append(';')
                    .ToString();

            if (!EntityMappings.ContainsKey(entityName))
                throw new Exception("Unsupported entity name: " + entityName);

            return (new StringBuilder())
                .Append("&#").Append(EntityMappings[entityName]).Append(';')
                .ToString();
        }
    }
}
