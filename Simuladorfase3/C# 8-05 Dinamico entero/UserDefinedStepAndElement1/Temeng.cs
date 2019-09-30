using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurobi;
using SimioAPI;
using SimioAPI.Extensions;
using Vectores;
using System.IO;
using Gurobi;

namespace Temeng
{
    class UserStep1Definition : IStepDefinition
    {
        #region IStepDefinition Members

        /// <summary>
        /// Property returning the full name for this type of step. The name should contain no spaces.
        /// </summary>
        public string Name
        {
            get { return "Temeng"; }
        }

        /// <summary>
        /// Property returning a short description of what the step does.
        /// </summary>
        public string Description
        {
            get { return "manda al camion a cualquier lado con un PL"; }
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
        static readonly Guid MY_ID = new Guid("{46d59294-1645-4ce4-8f6f-d1e08378f885}");

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

            //i Botaderos. j Palas. k Camiones.

            double[,] Din12 = (double[,])vectores.Din1.Clone();

            double[,] Tij = (double[,])vectores.PlTc.Clone();//I Tonelaje por turno


            double[,] xij = new double[30, 20];//I Tonelaje cumplido hasta el momento de activación del dinámico 
            double[,] yij = new double[30, 20];//Tonelaje requerido para alcazar R
            double[,] Rij = new double[30, 20];//Porcentaje de cumplimienteo del tonelaje
            double[,] dij = new double[30, 20];//Desviación del porcentaje de cumplimiento ideal en el momento de activación
            double[,] Mij = new double[30, 20];//Demanda de camiones por ruta 
            double[,] rij = new double[30, 20];//I Matriz de tiempos de viajes

            double[] tk = new double[81];//I Tiempo que le toma al camión k llegar al botadero/chancador al que va (si ya está en un botadero este tiempo es 0)
            double[] dk = new double[81];//I Tiempo esperado de espera del camión k en el botadero/chancador al que va
            double[] ej = new double[30];//I Tiempo promedio de descarga en el botadero j
            double[] Di = new double[20];//Demanda de camiones por pala (int?)
            double[] Li = new double[20];//I Tiempo promedio de carga en la pala i
            double[] Ni = new double[20];//I Número de camiones en la pala i (int?)
            double[] Ei = new double[20];//I Camiones en ruta a la pala i (int?)
            double[] Sk = new double[81];//I Arreglo de variables binarias que indican si el camión tiene una tarea asignada
            int[] Bk = new int[81];//Botadero al que está llendo el camión k

            int numcam = 81; //Número total de camiones
            double capacidad = 300; //Capacidad de los camiones
            int numPalas = 20; //Palas activas?
            int numBotaderos = 30; //Botaderos o chancador activos?
            double R = 0;

            //Se cálcula el porcentaje de cumplimiento de cuota
            for (int i = 0; i < numBotaderos; i++)
            {
                for (int j = 0; j < numPalas; j++)
                {
                    if (Tij[i, j] != 0)
                    {
                        Rij[i, j] = xij[i, j] / Tij[i, j];
                        R = R + Rij[i, j];
                    }


                }

            }

            //Se calcula de déficit o superávit 
            R = R / (numPalas * numBotaderos);
            for (int i = 0; i < numBotaderos; i++)
            {
                for (int j = 0; j < numPalas; j++)
                {
                    dij[i, j] = Rij[i, j] - R;
                }

            }
            //Se calcula la demanda adeudada para alcanzar R
            double mij_aux;
            int mij;
            for (int i = 0; i < numBotaderos; i++)
            {
                for (int j = 0; j < numPalas; j++)
                {
                    yij[i, j] = R * Tij[i, j] - xij[i, j];
                    mij_aux = yij[i, j] / capacidad;
                    mij = Convert.ToInt32(mij_aux);
                    Mij[i, j] = mij + 1;
                }

            }
            double DTotal = 0;
            double di;
            for (int j = 0; j < numPalas; j++)
            {
                di = 0;
                for (int i = 0; i < numPalas; i++)
                {
                    di = Mij[i, j] + di;
                }

                Di[j] = di;
                DTotal = DTotal + Di[j];
            }
            //En teoría se debería hacer un chequeo para que la demanda total de camiones no supere 
            //el total disponible. De ocurrir, se deben limitar los flujos del PL (*).

            //Criterio de asignación de camiones
            //Se crea el arreglo Wik que contiene los tiempos que demora al camion k llegar a ser
            //atendido por la pala i. El tamaño de este arreglo depende de los camiones disponibles.
            double[,] Wik = new double[numPalas, numcam];//Matriz de tiempos esperados del camión k para ser atendido por la pala i

            for (int k = 0; k < numcam; k++)
            {
                if (Sk[k] != 0) //Sólo para los camiones sin tareas asignadas
                {
                    for (int i = 0; i < numPalas; i++)
                    {
                        Wik[i, k] = Li[i] * (Ni[i] + Ei[i]) - (tk[k] + dk[k] + ej[Bk[k]] + rij[Bk[k], i]);
                    }
                }
            }

            //Modelo
            GRBEnv env = new GRBEnv();
            GRBModel model = new GRBModel(env);

            model.ModelName = "Asignación Dinámica";

            GRBVar[,] X = new GRBVar[numPalas, numcam]; //Binaria que define si el camión k se asigna a la pala i

            for (int i = 0; i < numPalas; i++)
            {
                for (int k = 0; k < numcam; k++)
                {
                    X[i, k] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, "X" + i + "_" + k);
                }
            }

            model.Update();

            //Restricciones

            for (int k = 0; k < numcam; k++)
            {
                GRBLinExpr suma_Xik = 0;

                for (int i = 0; i < numPalas; i++)
                {
                    suma_Xik = suma_Xik + X[i, k];
                }
                model.AddConstr(suma_Xik, GRB.LESS_EQUAL, Sk[k], "S" + k);
            }

            for (int i = 0; i < numPalas; i++)
            {
                GRBLinExpr suma_Xik = 0;

                for (int k = 0; k < numcam; k++)
                {
                    suma_Xik = suma_Xik + X[i, k];
                }
                model.AddConstr(suma_Xik, GRB.GREATER_EQUAL, Di[i], "D" + i);
            }

            model.Update();

            GRBLinExpr FO = 0;

            for (int i = 0; i < numPalas; i++)
            {
                for (int k = 0; k < numcam; k++)
                {
                    FO = FO + Wik[i, k] * X[i, k];
                }
            }
            model.SetObjective(FO, GRB.MINIMIZE);

            model.Update();

            model.Optimize();

            sr.serializa(vectores);

            sr.closeStream();
            return ExitType.FirstExit;

        }
    }
}
