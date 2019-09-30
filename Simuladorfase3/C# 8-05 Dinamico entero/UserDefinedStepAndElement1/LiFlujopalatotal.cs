using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurobi;
using SimioAPI;
using SimioAPI.Extensions;
using Vectores;
using System.IO;

namespace LiFlujopalatotal
{
    class UserStep1Definition : IStepDefinition
    {
        #region IStepDefinition Members

        /// <summary>
        /// Property returning the full name for this type of step. The name should contain no spaces.
        /// </summary>
        public string Name
        {
            get { return "Liflujopalatotal"; }
        }

        /// <summary>
        /// Property returning a short description of what the step does.
        /// </summary>
        public string Description
        {
            get { return "manda al camion a cualquier lado"; }
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
        static readonly Guid MY_ID = new Guid("{6adcacd9-7e04-422d-af9d-fa9f55385d66}");

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

            pd = schema.AddStateProperty("Timenow");
            pd.Description = "Recibe el tiempo en horas";

            pd = schema.AddStateProperty("Salida");
            pd.Description = "Siguiente pala";

            pd = schema.AddStateProperty("Aux");
            pd.Description = "Camino i para despues";
        }

        /// <summary>
        /// Method called to create a new instance of this step type to place in a process.
        /// Returns an instance of the class implementing the IStep interface.
        /// </summary>
        public IStep CreateStep(IPropertyReaders properties)
        {
            return new Cargabien(properties);
        }

        #endregion
    }

    class Cargabien : IStep
    {
        IPropertyReaders _properties;
        IStateProperty _propTimenow;
        IStateProperty _propSalida;
        IStateProperty _propAux;
        Vect vectores;

        public Cargabien(IPropertyReaders properties)
        {
            _properties = properties;
            _propTimenow = (IStateProperty)_properties.GetProperty("Timenow");
            _propSalida = (IStateProperty)_properties.GetProperty("Salida");
            _propAux = (IStateProperty)_properties.GetProperty("Aux");
            vectores = new Vectores.Vect();
        }


        /// <summary>
        /// Method called when a process token executes the step.
        /// </summary>
        public ExitType Execute(IStepExecutionContext context)
        {


            IState _timenow = _propTimenow.GetState(context);
            double timenow = Convert.ToDouble(_timenow.StateValue);
            IState _salida = _propSalida.GetState(context);
            double salida = Convert.ToDouble(_salida.StateValue);
            IState _aux = _propAux.GetState(context);
            double aux = Convert.ToDouble(_aux.StateValue);

            SerializarElement sr = new SerializarElement();

            if (File.Exists(sr.SerializationFile))
            {

                Vect v = sr.deserializa();
                vectores = sr.deserializa();

                vectores.MipYc = v.MipYc;
                vectores.MipYv = v.MipYv;
                vectores.MipTc = v.MipTc;
                vectores.MipTv = v.MipTv;

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
                vectores.PlYc = v.PlYc;
                vectores.PlYv = v.PlYv;
                vectores.PlTc = v.PlTc;
                vectores.PlTv = v.PlTv;
                vectores.PlTcmalo = v.PlTcmalo;
                vectores.Mine = v.Mine;
                vectores.Mineralocado = v.Mineralocado;
                vectores.Lastrealocado = v.Lastrealocado;
            }


            /* 
                            vectores.PlTc[30, 0]; #flujo pala 1
                            vectores.PlTc[30, 1]; #flujo pala 2
                            etc etc

                            vectores.Din1[x, 4] = pala1; #Ultimo tiempo que pala 1 tuvo una asignacion
                            vectores.Din1[x + 30, 4] = pala2; #Ultimo tiempo que pala 2 tuvo una asignacion
                            vectores.Din1[x + 60, 4] = pala3; etc
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

                            timenow; #tiempo actual



            */
            int Npalas = 20;
            int NSitios = 30;
            double botadero;
            double[] tij_op = new double[20];
            double[] desv_tij = new double[20];
            double[] Puntaje = new double[20];
            double Pmax = -10000000000;
            double Destino = 0;
            for (int j = 0; j < Npalas; j++)
            {
                if (vectores.Uj[j] > 0)
                {
                    double FlujoCamionesCargadoTotal = 0;

                    for (int i = 0; i < NSitios; i++)
                    {
                        FlujoCamionesCargadoTotal = FlujoCamionesCargadoTotal + vectores.PlYc[i, j];
                    }
                    tij_op[j] = 1 / FlujoCamionesCargadoTotal;
                    desv_tij[j] = (timenow - vectores.Din1[30 * j, 4]) - tij_op[j];
                    Puntaje[j] = desv_tij[j] / tij_op[j];
                    if (Puntaje[j] > Pmax)
                    {
                        Pmax = Puntaje[j];
                        Destino = j + 1;
                    }
                }
            }

            if (Destino == 4 || Destino == 5 || Destino == 6 || Destino == 10 || Destino == 17 || Destino == 18)
            {
                botadero = 1; 
            }

            else if (Destino == 7 || Destino == 8 || Destino == 12)
            {
                botadero = 4;
            }

            else if (Destino == 1 || Destino == 2 || Destino == 3 || Destino == 11)
            {
                botadero = 8;
            }

            else
            {
                Destino = 0;
                botadero = 3;
                
            }
            _salida.StateValue = Destino;
            _aux.StateValue = botadero;

            sr.serializa(vectores);

            sr.closeStream();
            return ExitType.FirstExit;

        }
    }
}