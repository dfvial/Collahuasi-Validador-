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
    [Serializable]
    public class UserStepDefinition : IStepDefinition
    {

        #region IStepDefinition Members

        /// <summary>
        /// Property returning the full name for this type of step. The name should contain no spaces. 
        /// </summary>
        public string Name
        {
            get { return "Vectores"; }
        }

        /// <summary>
        /// Property returning a short description of what the step does.  
        /// </summary>
        public string Description
        {
            get { return "Escribe de Vectores del modelo en un archivo externo"; }
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
        static readonly Guid MY_ID = new Guid("{3b89479b-86dd-4063-930d-72d92e361e69}");

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
            IPropertyDefinition pd;
            // Hoja
            pd = schema.AddStateProperty("TipoVector");
            pd.Description = "(1)PL,(2)Dinamico1,(3)Dinamico2,(4)TiempoCVij";

            // Fila
            pd = schema.AddStateProperty("Fila");
            pd.Description = "Fila en que escribe";
            
            // Columna
            pd = schema.AddStateProperty("Columna");
            pd.Description = "Columna en que escribe";

            // Palas
            pd = schema.AddStateProperty("Numerofilas");
            pd.Description = "Numero de filas de la matriz";

            // Camiones
            pd = schema.AddStateProperty("Numerocolumnas");
            pd.Description = "Numero de columnas de la matriz";

            // NuevoValor
            pd = schema.AddStateProperty("NuevoValor");
            pd.Description = "Nuevo valor para la celda";

            // LeeoEscribe
            pd = schema.AddStateProperty("LeeoEscribe");
            pd.Description = "1 para entrar a Simio, 0 para Entregar a C#";
        }

        /// <summary>
        /// Method called to create a new instance of this step type to place in a process. 
        /// Returns an instance of the class implementing the IStep interface.
        /// </summary>
        public IStep CreateStep(IPropertyReaders properties)
        {
            return new Vector(properties);
        }

        #endregion
    }


    class Vector : IStep
    {
        IPropertyReaders _properties;
        IStateProperty _propTipo;
        IStateProperty _propFila;
        IStateProperty _propColumna;
        IStateProperty _propPalas;
        IStateProperty _propCamiones;
        IStateProperty _propNV;
        IStateProperty _propLE;
        Vect vectores;



        public Vector(IPropertyReaders properties)
        {
            _properties = properties;
            _propTipo = (IStateProperty)_properties.GetProperty("TipoVector");
            _propFila = (IStateProperty)_properties.GetProperty("Fila");
            _propColumna = (IStateProperty)_properties.GetProperty("Columna");
            _propPalas = (IStateProperty)_properties.GetProperty("Numerofilas");
            _propCamiones = (IStateProperty)_properties.GetProperty("Numerocolumnas");
            _propNV = (IStateProperty)_properties.GetProperty("NuevoValor");
            _propLE = (IStateProperty)_properties.GetProperty("LeeoEscribe");
            vectores = new Vect();
            //
        }

        #region IStep Members

        /// <summary>
        /// Method called when a process token executes the step.
        /// </summary>
        public ExitType Execute(IStepExecutionContext context)
        {
            // Recupera el valor de las variables


            IState _tipo = _propTipo.GetState(context);
            int tipo = Convert.ToInt32(_tipo.StateValue);

            IState _fila = _propFila.GetState(context);
            int fila = Convert.ToInt32(_fila.StateValue) - 1;

            IState _columna = _propColumna.GetState(context);
            int columna = Convert.ToInt32(_columna.StateValue) - 1;

            IState _palas = _propPalas.GetState(context);
            int palas = Convert.ToInt32(_palas.StateValue);

            IState _camiones = _propCamiones.GetState(context);
            int camiones = Convert.ToInt32(_camiones.StateValue);

            IState _NV = _propNV.GetState(context);
            double NV = Convert.ToDouble(_NV.StateValue);

            IState _le = _propLE.GetState(context);
            int le = Convert.ToInt32(_le.StateValue);

            /***********************************************************/

            SerializarElement sr = new SerializarElement();

            if (File.Exists(sr.SerializationFile))
            {
                Vect v = sr.deserializa();
                if (v != null)
                {
                    //MIP ######################################
                    vectores.MipYc = v.MipYc;
                    vectores.MipYv = v.MipYv;
                    vectores.MipTc = v.MipTc;
                    vectores.MipTv = v.MipTv;

                    //
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

                //
            }
            else
            {
                vectores.Mine = new double[20];
                vectores.MipYc = new double[31, 21, 13];
                vectores.MipYv = new double[31, 21, 13];
                vectores.MipTc = new double[31, 21, 13];
                vectores.MipTv = new double[31, 21, 13];
                vectores.PlYc = new double[31, 21];
                vectores.PlYv = new double[31, 21];
                vectores.PlTc = new double[31, 21];
                vectores.PlTv = new double[31, 21];
                vectores.Din1 = new double[600, 8];
                vectores.DesCam = new double[200, 8];
                vectores.Fila = new double[20];
                vectores.TCV = new double[30, 20];
                vectores.TCC = new double[30, 20];
                vectores.TCj = new double[20];
                vectores.TDi = new double[30];
                vectores.Destij = new double[30, 20];
                vectores.Dj = new double[20];
                vectores.Uj = new double[20];
                vectores.Ui = new double[30];
                vectores.RLi = new double[30];
                vectores.RUi = new double[30];
                vectores.PlTcmalo = new double[30, 20];
                vectores.Mineralocado = new double[20];
                vectores.Lastrealocado = new double[20];

                for (int x = 0; x < 200; x++)
                {
                    vectores.DesCam[x, 0] = x + 1;
                }


                int aux = 1;
                int aux2 = 1;
                for (int x = 0; x < 600; x++)
                {
                    vectores.Din1[x, 2] = aux;
                    aux = aux + 1;
                    vectores.Din1[x, 3] = aux2;
                    if (aux == 31)
                    {
                        aux = 1;
                        aux2 = aux2 + 1;
                    }
                }

                vectores.TDi = new double[30] { 0.07, 99, 99, 0.02, 99, 99, 99, 0.01, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.02, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }; // Tiempos de descarga
                vectores.TCV = new double[,] { { 0.37, 0.37, 0.46, 0.09, 0.38, 0.37, 0.56, 0.53, 99, 99, 99, 99, 99, 99, 99, 99, 0.08, 0.08, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 0.58, 0.46, 0.68, 0.2, 0.62, 0.72, 0.27, 0.34, 99, 99, 99, 99, 99, 99, 99, 99, 0.81, 1.3, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 0.44, 0.4, 0.4, 0.35, 0.46, 0.47, 0.62, 0.74, 99, 99, 99, 99, 99, 99, 99, 99, 1.3, 0.57, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 0.26, 0.19, 0.3, 0.07, 0.39, 0.3, 99, 0.37, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 } };  //Tiempo camion vacio entre i e j
                vectores.TCC = new double[,] { { 0.23, 99, 99, 0.1, 0.41, 0.45, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.03, 0.05, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 0.38, 99, 99, 99, 99, 99, 0.22, 0.23, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 0.33, 0.35, 0.35, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 0.22, 99, 99, 0.2, 0.47, 0.47, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.09, 0.18, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 } }; //Tiempo camion cargado entre i e j
                vectores.PlTc = new double[,] { { 0, 0, 0, 1708.91, 2263.85, 2353.29, 0, 0, 0, 2353.29, 0, 0, 0, 0, 0, 0, 1379.92, 1294.04, 0, 0, 9000 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 5069.83, 3913.43, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8983.26 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 4117.54, 5417.58, 3933.07, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 13468.19 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 952.97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 952.97 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 4117.54, 5417.58, 3933.07, 1708.91, 2263.85, 3306.26, 5069.83, 3913.43, 0, 0, 0, 0, 0, 0, 0, 0, 1379.92, 1294.04, 0, 0, 32404.43 } };
                vectores.PlTv = new double[,] { { 0, 0, 0, 1708.91, 1310.88, 3306.26, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1379.92, 1294.04, 0, 0, 9000 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 5069.83, 3913.43, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8983.26 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 4117.54, 4464.61, 3933.07, 0, 952.97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 13468.19 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 952.97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 952.97 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 4117.54, 5417.58, 3933.07, 1708.91, 2263.85, 3306.26, 5069.83, 3913.43, 0, 0, 0, 0, 0, 0, 0, 0, 1379.92, 1294.04, 0, 0, 32404.43 } };
                vectores.PlYc = new double[,] { { 0, 0, 0, 1.41, 3.64, 4.27, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.82, 0.83, 0, 0, 10.97 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 4.94, 4.17, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9.11 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 5.17, 7.37, 5.13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 17.68 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 1.8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.8 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 5.17, 7.37, 5.13, 1.41, 3.64, 6.07, 4.94, 4.17, 0, 0, 0, 0, 0, 0, 0, 0, 0.82, 0.83, 0, 0, 39.56 } };
                vectores.PlYv = new double[,] { { 0, 0, 0, 0.86, 1.83, 4.49, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.64, 0.51, 0, 0, 8.33 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 4.59, 4.4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 5.85, 5.83, 5.06, 0, 1.4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 18.15 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0.61, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.61 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 5.85, 6.45, 5.06, 0.86, 3.23, 4.49, 4.59, 4.4, 0, 0, 0, 0, 0, 0, 0, 0, 0.64, 0.51, 0, 0, 36.09 } };
                vectores.TCj = new double[20] { 0.06, 0.07, 0.06, 0.15, 0.09, 0.12, 0.08, 0.09, 99, 0.09, 99, 99, 99, 99, 99, 99, 0.14, 0.13, 99, 99 };
                vectores.Destij = new double[30, 20] { { 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }; //matriz de puntos inicio llegada
                vectores.Dj = new double[20] { 4354, 5729, 4160, 1401.3, 1856.35, 2711.13, 5361, 4138, 0, 5361, 5361, 5361, 0, 0, 0, 0, 1131.530, 1061.11, 0, 0 }; // Tasa de extraccion de pala, toneladas por hora 
                //vectores.Dj = new double[20] { 4117.54, 5417.58, 3933.07, 1708.91, 2263.85, 3306.26, 5069.83, 3913.43, 0, 0, 0, 0, 0, 0, 0, 0, 1379.92, 1294.04, 0, 0 }; // Tasa de extraccion de pala, toneladas por hora 
                vectores.Uj = new double[20] { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0 }; // Factor ocupacion de la pala o chancador
                vectores.Ui = new double[30] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }; // Factor ocupacion de la pala o chancador
                vectores.RLi = new double[30] { 8160, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // minimo
                vectores.RUi = new double[30] { 10500, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999 }; // maximo
                vectores.PlTcmalo = new double[,] { { 0, 0, 0, 1401.3, 1856.35, 2709.171, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1131.53, 1061.11, 0, 0, 8160 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 5069.83, 3913.43, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8983.26 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 4117.54, 5417.58, 3933.07, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 13468.19 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 952.97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 952.97 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 4117.54, 5417.58, 3933.07, 1708.91, 2263.85, 3306.26, 5069.83, 3913.43, 0, 0, 0, 0, 0, 0, 0, 0, 1379.92, 1294.04, 0, 0, 32404.43 } };

            }

            /**/

            if (tipo == 1)
            {
                if (le == 0)
                {
                    //vectores.Pl[fila, columna] = NV;
                }
                else if (le == 1)
                {
                    //_NV.StateValue = vectores.Pl[fila, columna];
                }
            }

            else if (tipo == 2)
            {
                if (le == 0)
                {
                    if (columna == 1)
                    {

                        for (int x = 0; x < 30; x++)
                        {
                            vectores.Din1[x + 30 * (palas - 1), 1] = NV;
                        }

                    }
                    if (columna == 4)
                    {
                       
                        for (int x = 0; x < 30; x++)
                        {
                            vectores.Din1[x + 30 * (palas - 1),4] = NV;
                        }

                    }
                    else if (columna == 5)
                    {
                        for (int x = 0; x < 30; x++)
                        {
                            vectores.Din1[x + 30 * (palas - 1),5] = NV;
                        }
                    }
                    else
                    {
                        vectores.Din1[fila, columna] = NV;
                        int x = 1 / le;
                    }
                    
                }
                else if (le == 1)
                {
                    _NV.StateValue = vectores.Din1[fila, columna];
                    int x = 1 / (le-1);
                }
            }
/*            else if (tipo == 3)
            {
                if (le == 0)
                {
                    vectores.Din2[fila, columna] = NV;
                }
                else if (le == 1)
                {
                    _NV.StateValue = vectores.Din2[fila, columna];
                }
            }
*/
            else if (tipo == 4)
            {
                if (le == 0)
                {
                    vectores.TCV[fila, columna] = NV;
                }
                else if (le == 1)
                {
                    _NV.StateValue = vectores.TCV[fila, columna];
                }
            }
            else if (tipo == 5)
            {
                if (le == 0)
                {
                    vectores.TCC[fila, columna] = NV;
                }
                else if (le == 1)
                {
                    _NV.StateValue = vectores.TCC[fila, columna];
                }
            }
            else if (tipo == 6)
            {
                if (le == 0)
                {
                    vectores.DesCam[fila, columna] = NV;
                }
                else if (le == 1)
                {
                    _NV.StateValue = vectores.DesCam[fila, columna];
                }
            }
            else if (tipo == 7)
            {
                if (le == 0)
                {
                    vectores.Fila[fila] = NV;
                }
                else if (le == 1)
                {
                    _NV.StateValue = vectores.Fila[fila];
                }
            }
            else if (tipo == 8) //se cae chancador o palas
            {
                if (le == 0) //botaderos
                {

                    if (columna == 0)
                    {
                        vectores.Ui[fila] = NV;
                    }

                    else if (columna == 1)
                    {
                        vectores.Uj[fila] = NV;
                    }
                                    
                }

                else if (le == 1)

                    if (columna == 0)
                    {
                        _NV.StateValue = vectores.Ui[fila];
                    }

                    else if (columna == 1)
                    {
                        _NV.StateValue = vectores.Uj[fila];
                    }

            }

            sr.serializa(vectores);
            sr.closeStream();
            return ExitType.FirstExit;
        }


        #endregion
    }

    [Serializable]
    public class Vect
    {

        double[,,] mipyc;
        double[,,] mipyv;
        double[,,] miptc;
        double[,,] miptv;
        double[,] plyc;
        double[,] plyv;
        double[,] pltc;
        double[,] pltv;
        double[,] pltcmalo;
        double[,] din1;
        double[,] tcv;
        double[,] tcc;
        double[,] descam;
        double[] fila;
        double[] tcj;
        double[] tdi;
        double[,] destij;
        double[] dj;
        double[] uj;
        double[] ui;
        double[] rli;
        double[] rui;
        double[] mine;
        double[] mineralocado;
        double[] lastrealocado;

        public Vect()

        {


        }

        public double[,,] MipYc { get { return mipyc; } set { mipyc = value; } }
        public double[,,] MipYv { get { return mipyv; } set { mipyv = value; } }
        public double[,,] MipTc { get { return miptc; } set { miptc = value; } }
        public double[,,] MipTv { get { return miptv; } set { miptv = value; } }

        public double[,] PlYc { get { return plyc; } set { plyc = value; } }
        public double[,] PlYv { get { return plyv; } set { plyv = value; } }
        public double[,] PlTc { get { return pltc; } set { pltc = value; } }
        public double[,] PlTv { get { return pltv; } set { pltv = value; } }
        public double[,] Din1 { get { return din1; } set { din1 = value; } }
        public double[,] PlTcmalo { get { return pltcmalo; } set { pltcmalo = value; } }
        public double[,] TCV { get { return tcv; } set { tcv = value; } }
        public double[,] TCC { get { return tcc; } set { tcc = value; } }
        public double[,] DesCam { get { return descam; } set { descam = value; } }
        public double[] Fila { get { return fila; } set { fila = value; } }
        public double[] TCj { get { return tcj; } set { tcj = value; } }
        public double[] TDi { get { return tdi; } set { tdi = value; } }
        public double[,] Destij { get { return destij; } set { destij = value; } }
        public double[] Ui { get { return ui; } set { ui = value; } }
        public double[] Uj { get { return uj; } set { uj = value; } }
        public double[] Dj { get { return dj; } set { dj = value; } }
        public double[] RLi { get { return rli; } set { rli = value; } }
        public double[] RUi { get { return rui; } set { rui = value; } }
        public double[] Mine { get { return mine; } set { mine = value; } }
        public double[] Mineralocado { get { return mineralocado; } set { mineralocado = value; } }
        public double[] Lastrealocado { get { return lastrealocado; } set { lastrealocado = value; } }
    }

}
