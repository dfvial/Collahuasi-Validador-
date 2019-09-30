using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimioAPI;
using SimioAPI.Extensions;
using Vectores;
using System.IO;

namespace ActualiazarPalas23
{
    class UserStep1Definition : IStepDefinition
    {
        #region IStepDefinition Members

        /// <summary>
        /// Property returning the full name for this type of step. The name should contain no spaces.
        /// </summary>
        public string Name
        {
            get { return "ActualizarparaPl"; }
        }

        /// <summary>
        /// Property returning a short description of what the step does.
        /// </summary>
        public string Description
        {
            get { return "Actualiza Info para PL"; }
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
        static readonly Guid MY_ID = new Guid("{07505016-38a7-47c7-b7dc-89a3b7083f99}");

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

            pd = schema.AddStateProperty("Pala0");
            pd.Description = "0 para tiempos, 1 para filas, 3 para pasadas";

            pd = schema.AddStateProperty("Pala1");
            pd.Description = "Recibe el tiempo en horas";

            pd = schema.AddStateProperty("Pala2");
            pd.Description = "Siguiente pala";

            pd = schema.AddStateProperty("Pala3");
            pd.Description = "Camino i para despues";

            pd = schema.AddStateProperty("Pala4");
            pd.Description = "Numero id camion";

            pd = schema.AddStateProperty("Pala5");
            pd.Description = "Numero de camiones";

            pd = schema.AddStateProperty("Pala6");
            pd.Description = "Need time";

            pd = schema.AddStateProperty("Pala7");
            pd.Description = "Numero de camiones";

            pd = schema.AddStateProperty("Pala8");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala9");
            pd.Description = "Recibe el tiempo en horas";

            pd = schema.AddStateProperty("Pala10");
            pd.Description = "Siguiente pala";

            pd = schema.AddStateProperty("Pala11");
            pd.Description = "Camino i para despues";

            pd = schema.AddStateProperty("Pala12");
            pd.Description = "Numero id camion";

            pd = schema.AddStateProperty("Pala13");
            pd.Description = "Numero de camiones";

            pd = schema.AddStateProperty("Pala14");
            pd.Description = "Need time";

            pd = schema.AddStateProperty("Pala15");
            pd.Description = "Numero de camiones";

            pd = schema.AddStateProperty("Pala16");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala17");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala18");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala19");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala20");
            pd.Description = "Need time chancador";
          
        }

        /// <summary>
        /// Method called to create a new instance of this step type to place in a process.
        /// Returns an instance of the class implementing the IStep interface.
        /// </summary>
        public IStep CreateStep(IPropertyReaders properties)
        {
            return new UserStep1(properties);
        }

        #endregion
    }

    class UserStep1 : IStep
    {
        IPropertyReaders _properties;
        IStateProperty _propPala0;
        IStateProperty _propPala1;
        IStateProperty _propPala2;
        IStateProperty _propPala3;
        IStateProperty _propPala4;
        IStateProperty _propPala5;
        IStateProperty _propPala6;
        IStateProperty _propPala7;
        IStateProperty _propPala8;
        IStateProperty _propPala9;
        IStateProperty _propPala10;
        IStateProperty _propPala11;
        IStateProperty _propPala12;
        IStateProperty _propPala13;
        IStateProperty _propPala14;
        IStateProperty _propPala15;
        IStateProperty _propPala16;
        IStateProperty _propPala17;
        IStateProperty _propPala18;
        IStateProperty _propPala19;
        IStateProperty _propPala20;

        Vect vectores;

        public UserStep1(IPropertyReaders properties)
        {
            _properties = properties;
            _propPala0 = (IStateProperty)_properties.GetProperty("Pala0");
            _propPala1 = (IStateProperty)_properties.GetProperty("Pala1");
            _propPala2 = (IStateProperty)_properties.GetProperty("Pala2");
            _propPala3 = (IStateProperty)_properties.GetProperty("Pala3");
            _propPala4 = (IStateProperty)_properties.GetProperty("Pala4");
            _propPala5 = (IStateProperty)_properties.GetProperty("Pala5");
            _propPala6 = (IStateProperty)_properties.GetProperty("Pala6");
            _propPala7 = (IStateProperty)_properties.GetProperty("Pala7");
            _propPala8 = (IStateProperty)_properties.GetProperty("Pala8");
            _propPala9 = (IStateProperty)_properties.GetProperty("Pala9");
            _propPala10 = (IStateProperty)_properties.GetProperty("Pala10");
            _propPala11 = (IStateProperty)_properties.GetProperty("Pala11");
            _propPala12 = (IStateProperty)_properties.GetProperty("Pala12");
            _propPala13 = (IStateProperty)_properties.GetProperty("Pala13");
            _propPala14 = (IStateProperty)_properties.GetProperty("Pala14");
            _propPala15 = (IStateProperty)_properties.GetProperty("Pala15");
            _propPala16 = (IStateProperty)_properties.GetProperty("Pala16");
            _propPala17 = (IStateProperty)_properties.GetProperty("Pala17");
            _propPala18 = (IStateProperty)_properties.GetProperty("Pala18");
            _propPala19 = (IStateProperty)_properties.GetProperty("Pala19");
            _propPala20 = (IStateProperty)_properties.GetProperty("Pala20");
            vectores = new Vectores.Vect();
        }

        #region IStep Members

        /// <summary>
        /// Method called when a process token executes the step.
        /// </summary>
        public ExitType Execute(IStepExecutionContext context)
        {
            IState _pala0 = _propPala0.GetState(context);
            double pala0 = Convert.ToDouble(_pala0.StateValue);
            IState _pala1 = _propPala1.GetState(context);
            double pala1 = Convert.ToDouble(_pala1.StateValue);
            IState _pala2 = _propPala2.GetState(context);
            double pala2 = Convert.ToDouble(_pala2.StateValue);
            IState _pala3 = _propPala3.GetState(context);
            double pala3 = Convert.ToDouble(_pala3.StateValue);
            IState _pala4 = _propPala4.GetState(context);
            double pala4 = Convert.ToDouble(_pala4.StateValue);
            IState _pala5 = _propPala5.GetState(context);
            double pala5 = Convert.ToDouble(_pala5.StateValue);
            IState _pala6 = _propPala6.GetState(context);
            double pala6 = Convert.ToDouble(_pala6.StateValue);
            IState _pala7 = _propPala7.GetState(context);
            double pala7 = Convert.ToDouble(_pala7.StateValue);
            IState _pala8 = _propPala8.GetState(context);
            double pala8 = Convert.ToDouble(_pala8.StateValue);
            IState _pala9 = _propPala9.GetState(context);
            double pala9 = Convert.ToDouble(_pala9.StateValue);
            IState _pala10 = _propPala10.GetState(context);
            double pala10 = Convert.ToDouble(_pala10.StateValue);
            IState _pala11 = _propPala11.GetState(context);
            double pala11 = Convert.ToDouble(_pala11.StateValue);
            IState _pala12 = _propPala12.GetState(context);
            double pala12 = Convert.ToDouble(_pala12.StateValue);
            IState _pala13 = _propPala13.GetState(context);
            double pala13 = Convert.ToDouble(_pala13.StateValue);
            IState _pala14 = _propPala14.GetState(context);
            double pala14 = Convert.ToDouble(_pala14.StateValue);
            IState _pala15 = _propPala15.GetState(context);
            double pala15 = Convert.ToDouble(_pala15.StateValue);
            IState _pala16 = _propPala16.GetState(context);
            double pala16 = Convert.ToDouble(_pala16.StateValue);
            IState _pala17 = _propPala17.GetState(context);
            double pala17 = Convert.ToDouble(_pala17.StateValue);
            IState _pala18 = _propPala18.GetState(context);
            double pala18 = Convert.ToDouble(_pala18.StateValue);
            IState _pala19 = _propPala19.GetState(context);
            double pala19 = Convert.ToDouble(_pala19.StateValue);
            IState _pala20 = _propPala20.GetState(context);
            double pala20 = Convert.ToDouble(_pala20.StateValue);

            

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
                vectores.Lastrealocado = v.Lastrealocado;
            }

            if (pala0 == 0)
            {
                for (int x = 0; x < 30; x = x + 1)
                {
                    vectores.Din1[x, 4] = pala1;
                    vectores.Din1[x + 30, 4] = pala2;
                    vectores.Din1[x + 60, 4] = pala3;
                    vectores.Din1[x + 90, 4] = pala4;
                    vectores.Din1[x + 120, 4] = pala5;
                    vectores.Din1[x + 150, 4] = pala6;
                    vectores.Din1[x + 180, 4] = pala7;
                    vectores.Din1[x + 210, 4] = pala8;
                    vectores.Din1[x + 240, 4] = pala9;
                    vectores.Din1[x + 270, 4] = pala10;
                    vectores.Din1[x + 300, 4] = pala11;
                    vectores.Din1[x + 330, 4] = pala12;
                    vectores.Din1[x + 360, 4] = pala13;
                    vectores.Din1[x + 390, 4] = pala14;
                    vectores.Din1[x + 420, 4] = pala15;
                    vectores.Din1[x + 450, 4] = pala16;
                    vectores.Din1[x + 480, 4] = pala17;
                    vectores.Din1[x + 510, 4] = pala18;
                    vectores.Din1[x + 540, 4] = pala19;
                    vectores.Din1[x + 570, 4] = pala20;
                }
            }


            //filas
            else if (pala0 == 1)
            {
                vectores.Fila[0] = pala1;
                vectores.Fila[1] = pala2;
                vectores.Fila[2] = pala3;
                vectores.Fila[3] = pala4;
                vectores.Fila[4] = pala5;
                vectores.Fila[5] = pala6;
                vectores.Fila[6] = pala7;
                vectores.Fila[7] = pala8;
                vectores.Fila[8] = pala9;
                vectores.Fila[9] = pala10;
                vectores.Fila[10] = pala11;
                vectores.Fila[11] = pala12;
                vectores.Fila[12] = pala13;
                vectores.Fila[13] = pala14;
                vectores.Fila[14] = pala15;
                vectores.Fila[15] = pala16;
                vectores.Fila[16] = pala17;
                vectores.Fila[17] = pala18;
                vectores.Fila[18] = pala19;
                vectores.Fila[19] = pala20;
            }



            //numero alocados

            else if (pala0 == 3)
            {
                for (int x = 0; x < 30; x = x + 1)
                {

                    vectores.Din1[x, 5] = pala1;
                    vectores.Din1[x + 30, 5] = pala2;
                    vectores.Din1[x + 60, 5] = pala3;
                    vectores.Din1[x + 90, 5] = pala4;
                    vectores.Din1[x + 120, 5] = pala5;
                    vectores.Din1[x + 150, 5] = pala6;
                    vectores.Din1[x + 180, 5] = pala7;
                    vectores.Din1[x + 210, 5] = pala8;
                    vectores.Din1[x + 240, 5] = pala9;
                    vectores.Din1[x + 270, 5] = pala10;
                    vectores.Din1[x + 300, 5] = pala11;
                    vectores.Din1[x + 330, 5] = pala12;
                    vectores.Din1[x + 360, 5] = pala13;
                    vectores.Din1[x + 390, 5] = pala14;
                    vectores.Din1[x + 420, 5] = pala15;
                    vectores.Din1[x + 450, 5] = pala16;
                    vectores.Din1[x + 480, 5] = pala17;
                    vectores.Din1[x + 510, 5] = pala18;
                    vectores.Din1[x + 540, 5] = pala19;
                    vectores.Din1[x + 570, 5] = pala20;

                }
            }

            else if (pala0 == 4) //Si es de mineral 0, si es de esteril 1
            {
                vectores.Mine[0] = pala1;
                vectores.Mine[1] = pala2;
                vectores.Mine[2] = pala3;
                vectores.Mine[3] = pala4;
                vectores.Mine[4] = pala5;
                vectores.Mine[5] = pala6;
                vectores.Mine[6] = pala7;
                vectores.Mine[7] = pala8;
                vectores.Mine[8] = pala9;
                vectores.Mine[9] = pala10;
                vectores.Mine[10] = pala11;
                vectores.Mine[11] = pala12;
                vectores.Mine[12] = pala13;
                vectores.Mine[13] = pala14;
                vectores.Mine[14] = pala15;
                vectores.Mine[15] = pala16;
                vectores.Mine[16] = pala17;
                vectores.Mine[17] = pala18;
                vectores.Mine[18] = pala19;
                vectores.Mine[19] = pala20;
            }

            else if (pala0 == 5) //numero de camiones que salieron de cada pala hacia el chancador
            {
                vectores.Mineralocado[0] = pala1;
                vectores.Mineralocado[1] = pala2;
                vectores.Mineralocado[2] = pala3;
                vectores.Mineralocado[3] = pala4;
                vectores.Mineralocado[4] = pala5;
                vectores.Mineralocado[5] = pala6;
                vectores.Mineralocado[6] = pala7;
                vectores.Mineralocado[7] = pala8;
                vectores.Mineralocado[8] = pala9;
                vectores.Mineralocado[9] = pala10;
                vectores.Mineralocado[10] = pala11;
                vectores.Mineralocado[11] = pala12;
                vectores.Mineralocado[12] = pala13;
                vectores.Mineralocado[13] = pala14;
                vectores.Mineralocado[14] = pala15;
                vectores.Mineralocado[15] = pala16;
                vectores.Mineralocado[16] = pala17;
                vectores.Mineralocado[17] = pala18;
                vectores.Mineralocado[18] = pala19;
                vectores.Mineralocado[19] = pala20;
            }

            else if (pala0 == 6) //numero de camiones que salieron de cada pala el esteril
            {
                vectores.Lastrealocado[0] = pala1 * vectores.Mine[0];
                vectores.Lastrealocado[1] = pala2 * vectores.Mine[1];
                vectores.Lastrealocado[2] = pala3 * vectores.Mine[2];
                vectores.Lastrealocado[3] = pala4 * vectores.Mine[3];
                vectores.Lastrealocado[4] = pala5 * vectores.Mine[4];
                vectores.Lastrealocado[5] = pala6 * vectores.Mine[5];
                vectores.Lastrealocado[6] = pala7 * vectores.Mine[6];
                vectores.Lastrealocado[7] = pala8 * vectores.Mine[7];
                vectores.Lastrealocado[8] = pala9 * vectores.Mine[8];
                vectores.Lastrealocado[9] = pala10 * vectores.Mine[9];
                vectores.Lastrealocado[10] = pala11 * vectores.Mine[10];
                vectores.Lastrealocado[11] = pala12 * vectores.Mine[11];
                vectores.Lastrealocado[12] = pala13 * vectores.Mine[12];
                vectores.Lastrealocado[13] = pala14 * vectores.Mine[13];
                vectores.Lastrealocado[14] = pala15 * vectores.Mine[14];
                vectores.Lastrealocado[15] = pala16 * vectores.Mine[15];
                vectores.Lastrealocado[16] = pala17 * vectores.Mine[16];
                vectores.Lastrealocado[17] = pala18 * vectores.Mine[17];
                vectores.Lastrealocado[18] = pala19 * vectores.Mine[18];
                vectores.Lastrealocado[19] = pala20 * vectores.Mine[19];
            }


            sr.serializa(vectores);
            sr.closeStream();
            return ExitType.FirstExit;
        }
    }
}
#endregion