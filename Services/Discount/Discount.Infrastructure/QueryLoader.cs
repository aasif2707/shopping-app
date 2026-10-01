using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Discount.Infrastructure
{
    public class QueryLoader
    {
        private static XDocument _document;
        private static IEnumerable<XElement> xElements;

        static QueryLoader()
        {
            var assemby = Assembly.GetExecutingAssembly();
            var resourcename = $"{assemby.GetName().Name}.Discount_Query.xml";
            using (var stream = assemby.GetManifestResourceStream(resourcename))
            using (var reader = new StreamReader(stream))
            {
                _document = XDocument.Load(reader);
            }

            xElements = Document.Descendants("query");
        }

        public static XDocument Document
        {
            get
            {
                return _document;
            }
            set
            {
                _document = value;
            }
        }

        public static IEnumerable<XElement> XElements
        {
            get { return xElements; }
            set { xElements = value; }

        }


        public static string GetSqlQuery(string name)
        {
            var query = XElements.FirstOrDefault(x => x.Attribute("name").Value == name);
            return query?.Value?.Trim();
        }
    }
}
