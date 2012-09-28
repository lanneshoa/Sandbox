using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace BBEEGInteger
{
    public static class Serialize
    {
        public static string ToJson(object value)
        {
            Newtonsoft.Json.JsonSerializer json = new Newtonsoft.Json.JsonSerializer()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Auto,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            };

            StringWriter sw = new StringWriter();
            Newtonsoft.Json.JsonTextWriter writer = new JsonTextWriter(sw);

            writer.Formatting = Formatting.None;

            //writer.QuoteChar = '"';

            json.Serialize(writer, value);


            string output = sw.ToString();
            writer.Close();
            sw.Close();

            return output;


            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //return jss.Serialize(value);
        }
    }
}
