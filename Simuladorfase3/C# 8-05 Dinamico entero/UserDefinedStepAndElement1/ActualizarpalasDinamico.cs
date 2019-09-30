using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimioAPI;
using SimioAPI.Extensions;
using Vectores;
using System.IO;

namespace ActualiazarPalas2
{
    class UserStep1Definition : IStepDefinition
    {
        #region IStepDefinition Members

        /// <summary>
        /// Property returning the full name for this type of step. The name should contain no spaces.
        /// </summary>
        public string Name
        {
            get { return "ActualizarPalas2"; }
        }

        /// <summary>
        /// Property returning a short description of what the step does.
        /// </summary>
        public string Description
        {
            get { return "Actualiza asignaciones por caminos/pala"; }
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
        static readonly Guid MY_ID = new Guid("{35f4dfc5-4e68-4f6a-a58c-9b513927a4fe}");

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

            pd = schema.AddStateProperty("Pala21");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala22");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala23");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala24");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala25");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala26");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala27");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala28");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala29");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala30");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala31");
            pd.Description = "Recibe el tiempo en horas";

            pd = schema.AddStateProperty("Pala32");
            pd.Description = "Siguiente Pala3";

            pd = schema.AddStateProperty("Pala33");
            pd.Description = "Camino i para despues";

            pd = schema.AddStateProperty("Pala34");
            pd.Description = "Numero id camion";

            pd = schema.AddStateProperty("Pala35");
            pd.Description = "Numero de camiones";

            pd = schema.AddStateProperty("Pala36");
            pd.Description = "Need time";

            pd = schema.AddStateProperty("Pala37");
            pd.Description = "Numero de camiones";

            pd = schema.AddStateProperty("Pala38");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala39");
            pd.Description = "Recibe el tiempo en horas";

            pd = schema.AddStateProperty("Pala40");
            pd.Description = "Siguiente Pala3";

            pd = schema.AddStateProperty("Pala41");
            pd.Description = "Camino i para despues";

            pd = schema.AddStateProperty("Pala42");
            pd.Description = "Numero id camion";

            pd = schema.AddStateProperty("Pala43");
            pd.Description = "Numero de camiones";

            pd = schema.AddStateProperty("Pala44");
            pd.Description = "Need time";

            pd = schema.AddStateProperty("Pala45");
            pd.Description = "Numero de camiones";

            pd = schema.AddStateProperty("Pala46");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala47");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala48");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala49");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala50");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala51");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala52");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala53");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala54");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala55");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala56");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala57");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala58");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala59");
            pd.Description = "Need time chancador";

            pd = schema.AddStateProperty("Pala60");
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
        IStateProperty _propPala21;
        IStateProperty _propPala22;
        IStateProperty _propPala23;
        IStateProperty _propPala24;
        IStateProperty _propPala25;
        IStateProperty _propPala26;
        IStateProperty _propPala27;
        IStateProperty _propPala28;
        IStateProperty _propPala29;
        IStateProperty _propPala30;
        IStateProperty _propPala31;
        IStateProperty _propPala32;
        IStateProperty _propPala33;
        IStateProperty _propPala34;
        IStateProperty _propPala35;
        IStateProperty _propPala36;
        IStateProperty _propPala37;
        IStateProperty _propPala38;
        IStateProperty _propPala39;
        IStateProperty _propPala40;
        IStateProperty _propPala41;
        IStateProperty _propPala42;
        IStateProperty _propPala43;
        IStateProperty _propPala44;
        IStateProperty _propPala45;
        IStateProperty _propPala46;
        IStateProperty _propPala47;
        IStateProperty _propPala48;
        IStateProperty _propPala49;
        IStateProperty _propPala50;
        IStateProperty _propPala51;
        IStateProperty _propPala52;
        IStateProperty _propPala53;
        IStateProperty _propPala54;
        IStateProperty _propPala55;
        IStateProperty _propPala56;
        IStateProperty _propPala57;
        IStateProperty _propPala58;
        IStateProperty _propPala59;
        IStateProperty _propPala60;

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
            _propPala21 = (IStateProperty)_properties.GetProperty("Pala21");
            _propPala22 = (IStateProperty)_properties.GetProperty("Pala22");
            _propPala23 = (IStateProperty)_properties.GetProperty("Pala23");
            _propPala24 = (IStateProperty)_properties.GetProperty("Pala24");
            _propPala25 = (IStateProperty)_properties.GetProperty("Pala25");
            _propPala26 = (IStateProperty)_properties.GetProperty("Pala26");
            _propPala27 = (IStateProperty)_properties.GetProperty("Pala27");
            _propPala28 = (IStateProperty)_properties.GetProperty("Pala28");
            _propPala29 = (IStateProperty)_properties.GetProperty("Pala29");
            _propPala30 = (IStateProperty)_properties.GetProperty("Pala30");
            _propPala31 = (IStateProperty)_properties.GetProperty("Pala31");
            _propPala32 = (IStateProperty)_properties.GetProperty("Pala32");
            _propPala33 = (IStateProperty)_properties.GetProperty("Pala33");
            _propPala34 = (IStateProperty)_properties.GetProperty("Pala34");
            _propPala35 = (IStateProperty)_properties.GetProperty("Pala35");
            _propPala36 = (IStateProperty)_properties.GetProperty("Pala36");
            _propPala37 = (IStateProperty)_properties.GetProperty("Pala37");
            _propPala38 = (IStateProperty)_properties.GetProperty("Pala38");
            _propPala39 = (IStateProperty)_properties.GetProperty("Pala39");
            _propPala40 = (IStateProperty)_properties.GetProperty("Pala40");
            _propPala41 = (IStateProperty)_properties.GetProperty("Pala41");
            _propPala42 = (IStateProperty)_properties.GetProperty("Pala42");
            _propPala43 = (IStateProperty)_properties.GetProperty("Pala43");
            _propPala44 = (IStateProperty)_properties.GetProperty("Pala44");
            _propPala45 = (IStateProperty)_properties.GetProperty("Pala45");
            _propPala46 = (IStateProperty)_properties.GetProperty("Pala46");
            _propPala47 = (IStateProperty)_properties.GetProperty("Pala47");
            _propPala48 = (IStateProperty)_properties.GetProperty("Pala48");
            _propPala49 = (IStateProperty)_properties.GetProperty("Pala49");
            _propPala50 = (IStateProperty)_properties.GetProperty("Pala50");
            _propPala51 = (IStateProperty)_properties.GetProperty("Pala51");
            _propPala52 = (IStateProperty)_properties.GetProperty("Pala52");
            _propPala53 = (IStateProperty)_properties.GetProperty("Pala53");
            _propPala54 = (IStateProperty)_properties.GetProperty("Pala54");
            _propPala55 = (IStateProperty)_properties.GetProperty("Pala55");
            _propPala56 = (IStateProperty)_properties.GetProperty("Pala56");
            _propPala57 = (IStateProperty)_properties.GetProperty("Pala57");
            _propPala58 = (IStateProperty)_properties.GetProperty("Pala58");
            _propPala59 = (IStateProperty)_properties.GetProperty("Pala59");
            _propPala60 = (IStateProperty)_properties.GetProperty("Pala60");
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
            IState _pala21 = _propPala21.GetState(context);
            double pala21 = Convert.ToDouble(_pala21.StateValue);
            IState _pala22 = _propPala22.GetState(context);
            double pala22 = Convert.ToDouble(_pala22.StateValue);
            IState _pala23 = _propPala23.GetState(context);
            double pala23 = Convert.ToDouble(_pala23.StateValue);
            IState _pala24 = _propPala24.GetState(context);
            double pala24 = Convert.ToDouble(_pala24.StateValue);
            IState _pala25 = _propPala25.GetState(context);
            double pala25 = Convert.ToDouble(_pala25.StateValue);
            IState _pala26 = _propPala26.GetState(context);
            double pala26 = Convert.ToDouble(_pala26.StateValue);
            IState _pala27 = _propPala27.GetState(context);
            double pala27 = Convert.ToDouble(_pala27.StateValue);
            IState _pala28 = _propPala28.GetState(context);
            double pala28 = Convert.ToDouble(_pala28.StateValue);
            IState _pala29 = _propPala29.GetState(context);
            double pala29 = Convert.ToDouble(_pala29.StateValue);
            IState _pala30 = _propPala30.GetState(context);
            double pala30 = Convert.ToDouble(_pala30.StateValue);
            IState _pala31 = _propPala31.GetState(context);
            double pala31 = Convert.ToDouble(_pala31.StateValue);
            IState _pala32 = _propPala32.GetState(context);
            double pala32 = Convert.ToDouble(_pala32.StateValue);
            IState _pala33 = _propPala33.GetState(context);
            double pala33 = Convert.ToDouble(_pala33.StateValue);
            IState _pala34 = _propPala34.GetState(context);
            double pala34 = Convert.ToDouble(_pala34.StateValue);
            IState _pala35 = _propPala35.GetState(context);
            double pala35 = Convert.ToDouble(_pala35.StateValue);
            IState _pala36 = _propPala36.GetState(context);
            double pala36 = Convert.ToDouble(_pala36.StateValue);
            IState _pala37 = _propPala37.GetState(context);
            double pala37 = Convert.ToDouble(_pala37.StateValue);
            IState _pala38 = _propPala38.GetState(context);
            double pala38 = Convert.ToDouble(_pala38.StateValue);
            IState _pala39 = _propPala39.GetState(context);
            double pala39 = Convert.ToDouble(_pala39.StateValue);
            IState _pala40 = _propPala40.GetState(context);
            double pala40 = Convert.ToDouble(_pala40.StateValue);
            IState _pala41 = _propPala41.GetState(context);
            double pala41 = Convert.ToDouble(_pala41.StateValue);
            IState _pala42 = _propPala42.GetState(context);
            double pala42 = Convert.ToDouble(_pala42.StateValue);
            IState _pala43 = _propPala43.GetState(context);
            double pala43 = Convert.ToDouble(_pala43.StateValue);
            IState _pala44 = _propPala44.GetState(context);
            double pala44 = Convert.ToDouble(_pala44.StateValue);
            IState _pala45 = _propPala45.GetState(context);
            double pala45 = Convert.ToDouble(_pala45.StateValue);
            IState _pala46 = _propPala46.GetState(context);
            double pala46 = Convert.ToDouble(_pala46.StateValue);
            IState _pala47 = _propPala47.GetState(context);
            double pala47 = Convert.ToDouble(_pala47.StateValue);
            IState _pala48 = _propPala48.GetState(context);
            double pala48 = Convert.ToDouble(_pala48.StateValue);
            IState _pala49 = _propPala49.GetState(context);
            double pala49 = Convert.ToDouble(_pala49.StateValue);
            IState _pala50 = _propPala50.GetState(context);
            double pala50 = Convert.ToDouble(_pala50.StateValue);
            IState _pala51 = _propPala51.GetState(context);
            double pala51 = Convert.ToDouble(_pala51.StateValue);
            IState _pala52 = _propPala52.GetState(context);
            double pala52 = Convert.ToDouble(_pala52.StateValue);
            IState _pala53 = _propPala53.GetState(context);
            double pala53 = Convert.ToDouble(_pala53.StateValue);
            IState _pala54 = _propPala54.GetState(context);
            double pala54 = Convert.ToDouble(_pala54.StateValue);
            IState _pala55 = _propPala55.GetState(context);
            double pala55 = Convert.ToDouble(_pala55.StateValue);
            IState _pala56 = _propPala56.GetState(context);
            double pala56 = Convert.ToDouble(_pala56.StateValue);
            IState _pala57 = _propPala57.GetState(context);
            double pala57 = Convert.ToDouble(_pala57.StateValue);
            IState _pala58 = _propPala58.GetState(context);
            double pala58 = Convert.ToDouble(_pala58.StateValue);
            IState _pala59 = _propPala59.GetState(context);
            double pala59 = Convert.ToDouble(_pala59.StateValue);
            IState _pala60 = _propPala60.GetState(context);
            double pala60 = Convert.ToDouble(_pala60.StateValue);
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
                vectores.Lastrealocado = v.Lastrealocado;
            }
            /*
            //tiempos

            vectores.Din1[0, 7] = pala0;
            vectores.Din1[30, 7] = pala1;
            vectores.Din1[60, 7] = pala2;
            vectores.Din1[90, 7] = pala3;
            vectores.Din1[120, 7] = pala4;
            vectores.Din1[150, 7] = pala5;
            vectores.Din1[180, 7] = pala6;
            vectores.Din1[210, 7] = pala7;
            vectores.Din1[480, 7] = pala8;
            vectores.Din1[510, 7] = pala9;

            vectores.Din1[3, 7] = pala10;
            vectores.Din1[33, 7] = pala11;
            vectores.Din1[63, 7] = pala12;
            vectores.Din1[93, 7] = pala13;
            vectores.Din1[123, 7] = pala14;
            vectores.Din1[153, 7] = pala15;
            vectores.Din1[183, 7] = pala16;
            vectores.Din1[213, 7] = pala17;
            vectores.Din1[483, 7] = pala18;
            vectores.Din1[513, 7] = pala19;

            vectores.Din1[7, 7] = pala20;
            vectores.Din1[37, 7] = pala31;
            vectores.Din1[67, 7] = pala32;
            vectores.Din1[97, 7] = pala33;
            vectores.Din1[127, 7] = pala34;
            vectores.Din1[157, 7] = pala35;
            vectores.Din1[187, 7] = pala36;
            vectores.Din1[217, 7] = pala37;
            vectores.Din1[487, 7] = pala38;
            vectores.Din1[517, 7] = pala39;

            vectores.Din1[17, 7] = pala40;
            vectores.Din1[47, 7] = pala41;
            vectores.Din1[77, 7] = pala42;
            vectores.Din1[107, 7] = pala43;
            vectores.Din1[137, 7] = pala44;
            vectores.Din1[167, 7] = pala45;
            vectores.Din1[197, 7] = pala46;
            vectores.Din1[227, 7] = pala47;
            vectores.Din1[497, 7] = pala48;
            vectores.Din1[527, 7] = pala49;
            */
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
            vectores.Fila[0] = pala21;
            vectores.Fila[1] = pala22;
            vectores.Fila[2] = pala23;
            vectores.Fila[3] = pala24;
            vectores.Fila[4] = pala25;
            vectores.Fila[5] = pala26;
            vectores.Fila[6] = pala27;
            vectores.Fila[7] = pala28;
            vectores.Fila[8] = pala29;
            vectores.Fila[9] = pala30;
            vectores.Fila[10] = pala31;
            vectores.Fila[11] = pala32;
            vectores.Fila[12] = pala33;
            vectores.Fila[13] = pala34;
            vectores.Fila[14] = pala35;
            vectores.Fila[15] = pala36;
            vectores.Fila[16] = pala37;
            vectores.Fila[17] = pala38;
            vectores.Fila[18] = pala39;
            vectores.Fila[19] = pala40;

            for (int x = 0; x < 30; x = x + 1)
            {

                vectores.Din1[x, 5] = pala41;
                vectores.Din1[x + 30, 5] = pala42;
                vectores.Din1[x + 60, 5] = pala43;
                vectores.Din1[x + 90, 5] = pala44;
                vectores.Din1[x + 120, 5] = pala45;
                vectores.Din1[x + 150, 5] = pala46;
                vectores.Din1[x + 180, 5] = pala47;
                vectores.Din1[x + 210, 5] = pala48;
                vectores.Din1[x + 240, 5] = pala49;
                vectores.Din1[x + 270, 5] = pala50;
                vectores.Din1[x + 300, 5] = pala51;
                vectores.Din1[x + 330, 5] = pala52;
                vectores.Din1[x + 360, 5] = pala53;
                vectores.Din1[x + 390, 5] = pala54;
                vectores.Din1[x + 420, 5] = pala55;
                vectores.Din1[x + 450, 5] = pala56;
                vectores.Din1[x + 480, 5] = pala57;
                vectores.Din1[x + 510, 5] = pala58;
                vectores.Din1[x + 540, 5] = pala59;
                vectores.Din1[x + 570, 5] = pala60;

            }


            sr.serializa(vectores);
            sr.closeStream();
            return ExitType.FirstExit;
        }
    }
}
#endregion