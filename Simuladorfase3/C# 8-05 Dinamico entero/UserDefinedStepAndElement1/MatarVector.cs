using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimioAPI;
using SimioAPI.Extensions;

using System.IO;

namespace CloseFile
{

    public class UserStepDefinition : IStepDefinition
    {

        #region IStepDefinition Members

        /// <summary>
        /// Property returning the full name for this type of step. The name should contain no spaces. 
        /// </summary>
        public string Name
        {
            get { return "CloseFile"; }
        }

        /// <summary>
        /// Property returning a short description of what the step does.  
        /// </summary>
        public string Description
        {
            get { return "Borra archivo temp"; }
        }

        /// <summary>
        /// Property returning an icon to display for the step in the UI. 
        /// </summary>
        public System.Drawing.Image Icon
        {
            get { return null; }
        }

        /// <summary>
        /// Property returning a unique static GUID for the step.  
        /// </summary>
        public Guid UniqueID
        {
            get { return MY_IDFile; }
        }
        static readonly Guid MY_IDFile = new Guid("{627c9c6f-cb52-43bd-ae34-5dd8a3596451}");

        /// <summary>
        /// Property returning the number of exits out of the step. Can return either 1 or 2. 
        /// </summary>
        public int NumberOfExits
        {
            get { return 1; }
        }

        /// <summary>
        /// Method called that defines the property schema for the step.
        /// </summary>
        public void DefineSchema(IPropertyDefinitions schema)
        {

        }

        /// <summary>
        /// Method called to create a new instance of this step type to place in a process. 
        /// Returns an instance of the class implementing the IStep interface.
        /// </summary>
        public IStep CreateStep(IPropertyReaders properties)
        {
            return new CloseFile(properties);
        }

        #endregion
    }


    class CloseFile : IStep
    {
        string dir;
        string serializationFile;


        public CloseFile(IPropertyReaders properties)
        {
            dir = @"C:\Users\Usuario\Desktop\Collahuasi\Basura";
            serializationFile = Path.Combine(dir, "vector.bin");

        }

        #region IStep Members

        /// <summary>
        /// Method called when a process token executes the step.
        /// </summary>
        public ExitType Execute(IStepExecutionContext context)
        {
            File.Delete(serializationFile);
            return ExitType.FirstExit;
        }


        #endregion
    }


}
