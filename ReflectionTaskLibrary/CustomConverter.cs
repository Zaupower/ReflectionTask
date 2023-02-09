using ReflectionTask;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ReflectionTaskLibrary
{
    public class CustomConverter
    {
        private int spacer = -1;
        public string Serialize(object model)
        {
                      
            var hasMoreThanTwoProperties = model.GetType().GetProperties().Count() > 2;

            if (!hasMoreThanTwoProperties)
            {
                return SerializeString(model);
            }
            string result = Serialize2(model);
            result = result.Substring(0, result.Length - 2);
            return result;
        }

        private string SerializeString(object model)
        {
            return model.ToString();
        }

        public string Serialize2(object model)
        {
            IncrementSpacer();

            StringBuilder sb = new StringBuilder();
            sb.Append(GetSpacer() + "[section.begin]");
            sb.AppendLine();
            // Get all properties in the model
            var properties = model.GetType().GetProperties();

            foreach (var property in properties)
            {
                if(property.GetValue(model) != null)
                {
                    // Check if property has the CustomSerializeAttribute attribute
                    var customSerializeAttributes = property.GetCustomAttributes().Select(i => (CustomSerializeAttribute)i).ToList();

                    if (customSerializeAttributes.Count > 0)
                    {
                        // Get the Name attribute parameter value
                        string name = string.IsNullOrEmpty(customSerializeAttributes[0].Name) ? property.Name : customSerializeAttributes[0].Name;
                        object value = property.GetValue(model);

                        // Serialize the property value
                        if(value != null)
                            sb.AppendFormat(GetSpacer() + "{0} = {1}", name, property.PropertyType.IsClass && property.PropertyType != typeof(string) ? null : value);
                        sb.AppendLine();
                        // If the property value is a complex entity with nested fields, serialize them recursively
                        if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                        {
                            sb.Append(Serialize2(property.GetValue(model)));
                        }
                    }
                }
            }

            sb.Append(GetSpacer() + "[section.end]");
            sb.AppendLine();
            DecrementSpacer();
            return sb.ToString();
        }

        public string GetSpacer()
        {
            string spacerReturn = "";
            if (this.spacer > 0)
            {
                for(int i =0; i< this.spacer; i++)
                {
                    spacerReturn += "          ";
                }                
            }
            return spacerReturn;
        }

        public void IncrementSpacer()
        {
            this.spacer += 1;
        }

        public void DecrementSpacer()
        {
            this.spacer -= 1;
        }
    }
}
