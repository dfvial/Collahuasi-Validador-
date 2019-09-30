using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimioAPI;
using SimioAPI.Extensions;
using System.IO;


namespace Vectores
{
    class SerializarDefinition : IElementDefinition
    {
        #region IElementDefinition Members

        /// <summary>
        /// Property returning the full name for this type of element. The name should contain no spaces. 
        /// </summary>
        public string Name
        {
            get { return "Serializar"; }
        }

        /// <summary>
        /// Property returning a short description of what the element does.  
        /// </summary>
        public string Description
        {
            get { return "Create and read file C:\temp"; }
        }

        /// <summary>
        /// Property returning an icon to display for the element in the UI. 
        /// </summary>
        public System.Drawing.Image Icon
        {
            get { return null; }
        }

        /// <summary>
        /// Property returning a unique static GUID for the element.  
        /// </summary>
        public Guid UniqueID
        {
            get { return MY_ID; }
        }
        // We need to use this ID in the element reference property of the Read/Write steps, so we make it public
        public static readonly Guid MY_ID = new Guid("{5c716ae0-aae4-42a4-86c5-572bfa384fa0}");

        /// <summary>
        /// Method called that defines the property, state, and event schema for the element.
        /// </summary>
        public void DefineSchema(IElementSchema schema)
        {
            IPropertyDefinition pd = schema.PropertyDefinitions.AddStringProperty("Nada", String.Empty);

        }

        /// <summary>
        /// Method called to add a new instance of this element type to a model. 
        /// Returns an instance of the class implementing the IElement interface.
        /// </summary>
        public IElement CreateElement(IElementData data)
        {
            return new SerializarElement();
        }

        #endregion
    }

    class SerializarElement : IElement, IDisposable
    {
        string dir;
        string serializationFile;
        Stream stream;

        public SerializarElement()
        {

            dir = @"C:\Users\Usuario\Desktop\Collahuasi\Basura";
            serializationFile = Path.Combine(dir, "vector.bin");

        }


        public string SerializationFile { get { return serializationFile; } }
        public void serializa(Vect v)
        {

            using (stream = File.Create(serializationFile))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bformatter.Serialize(stream, v);

            }

        }
        public Vect deserializa()
        {
            Vect v;
            if (File.Exists(serializationFile))
            {
                using (stream = File.Open(serializationFile, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    stream.Position = 0;
                    v = (Vect)bformatter.Deserialize(stream);
                }
                return v;
            }
            return null;
        }
        public void closeStream()
        {
            stream.Close();
            stream.Dispose();

        }

        #region IElement Members

        /// <summary>
        /// Method called when the simulation run is initialized.
        /// </summary>
        public void Initialize()
        {

        }

        /// <summary>
        /// Method called when the simulation run is terminating.
        /// </summary>
        public void Shutdown()
        {
            File.Delete(serializationFile);
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Shutdown();
        }

        #endregion
    }

}