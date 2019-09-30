using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimioAPI;
using SimioAPI.Extensions;
using Vectores;
using System.IO;


namespace Cambiohora
{
    class UserStep1Definition : IStepDefinition
    {
        #region IStepDefinition Members

        /// <summary>
        /// Property returning the full name for this type of step. The name should contain no spaces.
        /// </summary>
        public string Name
        {
            get { return "Cambiohora"; }
        }

        /// <summary>
        /// Property returning a short description of what the step does.
        /// </summary>
        public string Description
        {
            get { return "actualiza horas"; }
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
            get { return MY_ID; }
        }
        static readonly Guid MY_ID = new Guid("{978bc1bd-8160-430b-b84c-fb6079db36db}");

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
            // Example of how to add a property definition to the step.
            IPropertyDefinition pd;
            pd = schema.AddStateProperty("Timenow");
            pd.Description = "Recibe el tiempo en horas";

        }

        /// <summary>
        /// Method called to create a new instance of this step type to place in a process.
        /// Returns an instance of the class implementing the IStep interface.
        /// </summary>
        public IStep CreateStep(IPropertyReaders properties)
        {
            return new Cambiohora(properties);
        }

        #endregion
    }

    class Cambiohora : IStep
    {
        IPropertyReaders _properties;
        IStateProperty _propTimenow;
        Vect vectores;

        public Cambiohora(IPropertyReaders properties)
        {
            _properties = properties;
            _propTimenow = (IStateProperty)_properties.GetProperty("Timenow");
            vectores = new Vectores.Vect();
        }

        #region IStep Members

        /// <summary>
        /// Method called when a process token executes the step.
        /// </summary>
        public ExitType Execute(IStepExecutionContext context)
        {

            IState _timenow = _propTimenow.GetState(context);
            double timenow = Convert.ToDouble(_timenow.StateValue);

            // Example of how to get the value of a step property.


            SerializarElement sr = new SerializarElement();

            if (File.Exists(sr.SerializationFile))
            {

                Vect v = sr.deserializa();
                vectores = sr.deserializa();

                vectores.MipYc = v.MipYc;
                vectores.MipYv = v.MipYv;
                vectores.MipTc = v.MipTc;
                vectores.MipTv = v.MipTv;
                vectores.PlYc = v.PlYc;
                vectores.PlYv = v.PlYv;
                vectores.PlTc = v.PlTc;
                vectores.PlTv = v.PlTv;
                vectores.PlTcmalo = v.PlTcmalo;

                //Dinamico
                vectores.Din1 = v.Din1;
                vectores.DesCam = v.DesCam;
                vectores.Fila = v.Fila;

                //LP
                vectores.TCV = v.TCV;
                vectores.TCC = v.TCC;
                vectores.TCj = v.TCj;
                vectores.TDi = v.TDi;
                vectores.Destij = v.Destij;
                vectores.Dj = v.Dj;
                vectores.Uj = v.Uj;
                vectores.Ui = v.Ui;
                vectores.RLi = v.RLi;
                vectores.RUi = v.RUi;
                vectores.Mine = v.Mine;
                vectores.Mineralocado = v.Mineralocado;
            }

            

            int periodos = 13-Convert.ToInt32(Math.Truncate(timenow));

            for (int b = 0; b < 21; b = b + 1)
            {

                for (int a = 0; a < 31; a = a + 1)
                {
                    vectores.PlYc[a,b] = vectores.MipYc[a, b, periodos];
                    vectores.PlYv[a,b] = vectores.MipYv[a, b, periodos];
                    vectores.PlTc[a,b] = vectores.MipTc[a, b, periodos];
                    vectores.PlTv[a,b] = vectores.MipTv[a, b, periodos];
                }

            }

            return ExitType.FirstExit;
        }
    }
}
#endregion
