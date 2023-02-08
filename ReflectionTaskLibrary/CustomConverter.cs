using ReflectionTask;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ReflectionTaskLibrary
{
    public class CustomConverter
    {
        private int spacer = -1;
        public string Serialize(object model)
        {
            //IncrementSpacer();

            //StringBuilder sb = new StringBuilder();
            //sb.Append(GetSpacer() + "[section.begin]");
            //sb.AppendLine();
            //// Get all properties in the model
            //var properties  = model.GetType().GetProperties();

            //foreach (var property in properties)
            //{
            //    // Check if property has the CustomSerializeAttribute attribute
            //    //var customSerializeAttributes = property.GetCustomAttributes(typeof(CustomSerializeAttribute), true);
            //    var customSerializeAttributes = property.GetCustomAttributes().Select(i=> (CustomSerializeAttribute)i).ToList();
            //    if (customSerializeAttributes.Count > 0)
            //    {
            //        // Get the Name attribute parameter value
            //        string name = customSerializeAttributes[0].Name;
            //        object value = property.GetValue(model);

            //        if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            //        {
            //            value = null;
            //        }                        

            //        if (string.IsNullOrEmpty(name))
            //        {
            //            name = property.Name;
            //        }                  

            //        // Serialize the property value
            //        sb.AppendFormat(GetSpacer()+"{0} = {1}", name, value);
            //        sb.AppendLine();
            //        // If the property value is a complex entity with nested fields, serialize them recursively
            //        if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            //        {
            //            sb.Append(Serialize(property.GetValue(model)));
            //        }
            //    }
            //}
            //sb.Append(GetSpacer() + "[section.end]");
            //sb.AppendLine();
            //DecrementSpacer();
            string result = "";
            //switch (model.GetType())
            //{
            //    case :
            //        result = SerializeString(model);
            //        break;
            //}
            if(model.GetType() == typeof(string)
                || model.GetType() == typeof(DayOfWeek)
                || model.GetType() == typeof(int)
                || model.GetType() == typeof(double))
            {
               return SerializeString(model);
            }

            var leType = model.GetType();
            Console.WriteLine(leType.Name);
            result = Serialize2(model);
            result = result.Substring(0, result.Length - 2);
            return result;
        }

        private string SerializeString(object model)
        {
            return model.ToString();
        }

        //TODO:
        //Handle null values; If the propertie does not have value(null) dont add it
        //
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

            private StringBuilder MethodBuidler(StringBuilder sb, PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                sb.Append("[section.begin]\n");
                sb.Append(property.Name + " = " + properties.GetValue(0).ToString());
                sb.Append("[section.end]\n");
            }

            return sb;
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
