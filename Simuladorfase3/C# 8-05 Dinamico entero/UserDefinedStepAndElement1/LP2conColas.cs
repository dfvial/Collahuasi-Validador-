using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurobi;
using SimioAPI;
using SimioAPI.Extensions;
using Vectores;
using System.IO;

namespace LP2conColas
{
    class UserStep1Definition : IStepDefinition
    {
        #region IStepDefinition Members

        /// <summary>
        /// Property returning the full name for this type of step. The name should contain no spaces.
        /// </summary>
        public string Name
        {
            get { return "LP2concColas"; }
        }

        /// <summary>
        /// Property returning a short description of what the step does.
        /// </summary>
        public string Description
        {
            get { return "Calcula Flujos por T periodos"; }
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
        static readonly Guid MY_ID = new Guid("{8badb731-f9e5-40f3-80d5-a80521427547}");

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

        Vect vectores;

        public Cargabien(IPropertyReaders properties)
        {
            _properties = properties;
            _propTimenow = (IStateProperty)_properties.GetProperty("Timenow");
            vectores = new Vectores.Vect();
        }


        /// <summary>
        /// Method called when a process token executes the step.
        /// </summary>
        public ExitType Execute(IStepExecutionContext context)
        {


            IState _timenow = _propTimenow.GetState(context);
            double timenow = Convert.ToDouble(_timenow.StateValue);

            int periodos = 13 - Convert.ToInt32(Math.Truncate(timenow));
            int periodos2 = Convert.ToInt32(Math.Truncate(timenow)) + 1;

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


            //_____________________________________________________________________________________________________________
            //PARÁMETROS INICIALES_________________________________________________________________________________________

            //POLÍTICAS--------------------------------------------------------
            int CAMPAÑA = 0;    //1 Activar, 0 Desactivar modo campaña
            int B_REMANENTE = 0;  //1 Activar, 0 Desactivar beneficios según remanentes

            //-----------------------------------------------------------------
            int L = 300;                   //Capacidad camiones
            int Lmin = 293;
            double Ttot = 13; //TOTAL PERÍODOS DE TODA LA SIMULACIÓN

            //Requerimientos de mineral pala j
            double[] PM0 = new double[20] { 0, 0, 0, 15000, 30000, 40000, 0, 0, 0, 12000, 0, 0, 0, 0, 0, 0, 10000, 5000, 0, 0 };

            //Plan Esteril de cada Fase (15-12 turno B)
            /*            double[] PF0 = new double[] { 105187 * 0.5, 170765 * 0.5, (115343 +154426 )* 0.5 };*/      //Plan Esteril (por fase) %

            //Palas de cada fase
            //int NF = 3;  //Número de fases
            //int[,] F = new int[,]
            //{
            //        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, //F11
            //        {0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0 }, //F12
            //        {0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, //F10B
            //};

            //LIBERAR FASES
            double[] PF0 = new double[] { 270000 };
            int NF = 1;  //Número de fases
            int[,] F = new int[,]
            {
                    {1,1,1,0,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0 }, //F11
            };

            int NCH = 3; //número de chancadores 
            //_____________________________________________________________________________________________________________
            //INPUTS SIMIO_________________________________________________________________________________________________

            double t_global = timenow;   //tiempo transcurrido desde el inicio de la simulación (+1)

            //Modo de ejecución { 1 si parte desde 0; 0 si corresponde a un cambio de condiciones de la mina)
            int Mode = 1;
            if (t_global > 0)
            {
                Mode = 0;
            }

            //Descargas de mineral a chancador por pala
            double[,] DMPP = new double[3, 20] { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }; // chancador alocados
            //Cargas de esteril por pala
            double[] CEPP = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // esteril alocados

            CEPP[0] = vectores.Lastrealocado[0] * L;
            CEPP[1] = vectores.Lastrealocado[1] * L;
            CEPP[2] = vectores.Lastrealocado[2] * L;
            CEPP[3] = vectores.Lastrealocado[3] * L;
            CEPP[4] = vectores.Lastrealocado[4] * L;
            CEPP[5] = vectores.Lastrealocado[5] * L;
            CEPP[6] = vectores.Lastrealocado[6] * L;
            CEPP[7] = vectores.Lastrealocado[7] * L;
            CEPP[8] = vectores.Lastrealocado[8] * L;
            CEPP[9] = vectores.Lastrealocado[9] * L;
            CEPP[10] = vectores.Lastrealocado[10] * L;
            CEPP[11] = vectores.Lastrealocado[11] * L;
            CEPP[12] = vectores.Lastrealocado[12] * L;
            CEPP[13] = vectores.Lastrealocado[13] * L;
            CEPP[14] = vectores.Lastrealocado[14] * L;
            CEPP[15] = vectores.Lastrealocado[15] * L;
            CEPP[16] = vectores.Lastrealocado[16] * L;
            CEPP[17] = vectores.Lastrealocado[17] * L;
            CEPP[18] = vectores.Lastrealocado[18] * L;
            CEPP[19] = vectores.Lastrealocado[19] * L;


            //Palas disponibles
            double[] PDisp = new double[20] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            //Destinos operativos
            double[] DDisp = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            //Flujos actuales 
            double[,] ATcij = new double[30, 20];
            double[,] ATvij = new double[30, 20];

            for (int x = 0; x < 20; x++) // solo el chancador
            {
                DMPP[0, x] = vectores.Mineralocado[x] * Lmin;
                PDisp[x] = vectores.Uj[x];
            }

            for (int a = 0; a < 30; a++) // solo el chancador
            {
                DDisp[a] = vectores.Ui[a];

                for (int b = 0; b < 20; b = b + 1)
                {
                    ATcij[a, b] = vectores.PlTc[a, b];
                    ATvij[a, b] = vectores.PlTv[a, b];
                }
            }

            //_____________________________________________________________________________________________________________
            //_____________________________________________________________________________________________________________


            //PARÁMETROS DINÁMICOS________________________________________________________________________________________
            double T1 = Ttot - Math.Truncate(t_global);
            double dT1 = t_global - Math.Truncate(t_global);   //tiempo restante del primer período
            int T = Convert.ToInt32(T1);                        //Horizonte restante problema (+1)

            //vector que corrige los tiempos-------------------
            double[] Vdt = new double[T];
            Vdt[0] = 0;
            Vdt[1] = 1 - dT1;
            if (Mode == 1)
            {
                Vdt[1] = 1;
            }
            for (int t = 2; t < T; t = t + 1)
            {
                Vdt[t] = 1;
            }

            //Corregir requerimientos de mineral fases--------
            double[] PM = new double[20];  //Mineral
            for (int i = 0; i < 20; i = i + 1)
            {
                double sumai = 0;
                double originali = PM0[i];
                for (int c = 0; c < NCH; c = c + 1)
                {
                    sumai = sumai + DMPP[c, i];
                }
                PM[i] = originali - sumai;
            }

            double[] PF = new double[NF];  //Fases Estéril
            for (int f = 0; f < NF; f = f + 1)
            {
                double sumaf = 0;
                double originalf = PF0[f];
                for (int i = 0; i < 20; i = i + 1)
                {
                    {
                        sumaf = sumaf + CEPP[i] * F[f, i];
                    }
                }
                PF[f] = originalf - sumaf;
            }

            //Si se corre desde el principio, llenar las matrices con 0
            if (Mode == 1)
            {
                for (int i = 0; i > 30; i = i + 1)
                {
                    for (int j = 0; j > 20; i = j + 1)
                    {
                        ATcij[i, j] = 0;
                        ATvij[i, j] = 0;
                    }
                }
            }

            double[] VCS = new double[50];       //Servidores caidos
            double[] VCO = new double[50];       //Servidores operativos
            for (int i = 0; i < 30; i = i + 1)
            {
                VCS[i] = 1 - DDisp[i];
                VCO[i] = DDisp[i];
            }
            for (int j = 30; j < 50; j = j + 1)
            {
                VCS[j] = 1 - PDisp[j - 30];
                VCO[j] = PDisp[j - 30];
            }

            //LEYES Y CARACTERÍSTICAS DE MINERAL_________________________________________________________________________

            double LeyObj = 1.46;

            //LEY DE CADA PALA (POR MIENTRAS PROMEDIO PONDERADO)
            double[] LeyP = new double[] { 0, 0, 0, 1.19100591718726, 1.28076520685417, 1.68963285916421, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0.72938715036178, 1.4466121790279, 0, 0 };
            //CONJUNTOS___________________________________________________________________________________________________
            int[] S1 = new int[] { 33, 34, 35, 39, 46, 47 };
            int[] S2 = new int[] { 0, 1, 2 };
            int[] S3 = new int[] { 17 };
            int[] S4 = new int[] { 30, 31, 32, 36, 37, 40, 41 };
            int[] S5 = new int[] { 3, 4, 5, 6, 7 };

            //CONSTRUIR COMBINACIONES DE CONJUNTOS----------------------------------------------
            //Todos los destinos
            int[] S2uS3uS5 = new int[S2.GetLength(0) + S3.GetLength(0) + S5.GetLength(0)];
            int cont = 0;
            foreach (int i in S2)
            {
                S2uS3uS5[cont] = i;
                cont = cont + 1;
            }
            foreach (int i in S3)
            {
                S2uS3uS5[cont] = i;
                cont = cont + 1;
            }
            foreach (int i in S5)
            {
                S2uS3uS5[cont] = i;
                cont = cont + 1;
            }

            //Destinos Mineral
            int[] S2uS3 = new int[S2.GetLength(0) + S3.GetLength(0)];
            cont = 0;
            foreach (int i in S2)
            {
                S2uS3[cont] = i;
                cont = cont + 1;
            }
            foreach (int i in S3)
            {
                S2uS3[cont] = i;
                cont = cont + 1;
            }

            //Todas las Palas
            int[] S1uS4 = new int[S1.GetLength(0) + S4.GetLength(0)];
            cont = 0;
            foreach (int i in S1)
            {
                S1uS4[cont] = i;
                cont = cont + 1;
            }
            foreach (int i in S4)
            {
                S1uS4[cont] = i;
                cont = cont + 1;
            }

            //Todo menos los chancadores
            int[] S1uS3uS4uS5 = new int[S1.GetLength(0) + S3.GetLength(0) + S4.GetLength(0) + S5.GetLength(0)];
            cont = 0;
            foreach (int i in S1)
            {
                S1uS3uS4uS5[cont] = i;
                cont = cont + 1;
            }
            foreach (int i in S3)
            {
                S1uS3uS4uS5[cont] = i;
                cont = cont + 1;
            }
            foreach (int i in S4)
            {
                S1uS3uS4uS5[cont] = i;
                cont = cont + 1;
            }
            foreach (int i in S5)
            {
                S1uS3uS4uS5[cont] = i;
                cont = cont + 1;
            }
            //--------------------------------------------------------------------------------------------

            //PARÁMETROS___________________________________________________________________________________________________
            int fil = 30;                     //Destinos
            int col = 20;                     //Palas
            int dim = fil + col;              //Todos los servidores 
            int NC = 60;                      //Número máximo de camiones
            double alpha = 0.2;               //Factor suavización de flujo (individual)
            double beta = 0.2;                //Factor de suavización flujo (total)
            int CCam = 20;                    //Costo de operación de cada camión
            double M = 9999;                  //Gran m
            double M2 = 999999;               //Gran m 2
            int alphaPM = 99999999;           //Coste Penaliz por incumplir plan mineral (palas)
            int alphaPF = 500;                //Coste Penaliz por incumplir plan estéril (fases)
            double alphaSlack = 9999999999;   //Penalidad por infactibilidad camiones



            //Beneficio por período de extraer mineral
            double BM_Vini = 100;
            double BM_dv = 5;
            double[,] B_Min = new double[col, T];
            for (int i = 0; i < col; i++)
            {
                B_Min[i, 0] = 0;
                for (int t = 1; t < T; t = t + 1)
                {
                    B_Min[i, t] = BM_Vini - BM_dv * (t - 1);

                }

            }
            //Beneficio por período de extraer esteril
            double[,] B_Est = new double[NF, T];
            double BE_Vini = 20;
            double BE_dv = 0;
            for (int f = 0; f < NF; f++)
            {
                B_Est[f, 0] = 0;
                for (int t = 1; t < T; t = t + 1)
                {
                    B_Est[f, t] = BE_Vini - BE_dv * (t - 1);
                }

            }









            //Tiempos de viaje
            double[,] TV = new double[50, 50] { { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.23, 0.24, 0.21, 0.26, 0.29, 0.25, 0.29, 0.28, 0.21, 0.34, 0.21, 0.26, 99, 99, 99, 99, 0.4, 0.27, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.25, 0.27, 0.29, 0.26, 0.39, 0.33, 0.21, 0.22, 0.28, 0.42, 0.29, 0.18, 99, 99, 99, 99, 0.13, 0.18, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.18, 0.24, 0.25, 99, 0.33, 0.34, 0.26, 99, 0.27, 0.43, 0.25, 0.23, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.25, 0.28, 0.28, 0.16, 0.36, 0.33, 0.34, 0.36, 0.27, 0.42, 0.28, 0.31, 99, 99, 99, 99, 0.2, 0.22, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.2, 0.24, 0.21, 99, 0.29, 0.26, 0.26, 0.26, 0.21, 0.35, 0.21, 0.23, 99, 99, 99, 99, 0.15, 0.42, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 0.35, 99, 0.27, 99, 0.32, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 0.36, 99, 0.33, 99, 0.37, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 0.26, 99, 99, 0.39, 99, 99, 99, 0.34, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.31, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 0.09, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.15, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 0.41, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.45, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 0.43, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.46, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 0.21, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 0.23, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 0.42, 99, 0.3, 99, 0.35, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 0.5, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 0.54, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.18, 99.09, 99.06, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09, 99.09 }, { 0.26, 99, 99, 0.39, 99, 99, 99, 0.34, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.31, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 98.97, 98.97, 98.97, 0.18, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 99.06, 98.97, 98.94, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97, 98.97 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 0.11, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.13, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 0.06, 99, 99, 0.36, 99, 0.29, 99, 0.33, 99, 99, 99, 99, 99, 99, 99, 99, 99, 0.13, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 }, { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99.09, 99, 98.97, 99, 99, 99, 99, 99, 99, 99, 99 } };


            //Tiempos de carga %j
            double[] TC = new double[] { 0.06, 0.07, 0.06, 0.15, 0.09, 0.12, 0.08, 0.09, 99, 0.12, 0.12, 0.12, 99, 99, 99, 99, 0.14, 0.13, 99, 99 };
            //Tiempos de descarga %i
            double[] TD = { 0.07, 999999, 999999, 0.02, 999999, 999999, 999999, 0.01, 999999, 999999, 999999, 999999, 999999, 999999, 999999, 999999, 999999, 0.02, 999999, 999999, 999999, 999999, 999999, 999999, 999999, 999999, 999999, 999999, 999999, 999999 };
            //Capacidad de palas %j en %t
            //double[] CP = new double[] { 4117.54, 5417.58, 3933.07, 1708.91, 2263.85, 3306.26, 5069.83, 3913.43, 0, 0, 0, 0, 0, 0, 0, 0, 1379.92, 1294.04, 0, 0 };
            //double[] CP = new double[20] { 4592,4514,3545,2500,4500,4500,4022,3585, 0, 0, 0, 0, 0, 0, 0, 0, 1379.92, 1294.04, 0, 0 };
            double[] CP = new double[20] { 4592, 4514, 3545, 2500, 4500, 4500, 4022, 3585, 0, 0, 0, 0, 0, 0, 0, 0, 1379.92, 1294.04, 0, 0 };
            //double[] CP = new double[20] { 6292, 5714, 6045, 3516, 4289, 4006, 4122, 3985, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            //Cap nominales
            //double[] CP = new double[] { 12672, 13996, 13148, 4842, 9413, 10692, 10549, 8911, 0, 0, 0, 0, 0, 0, 0, 0, 4190, 4302, 0, 0 };
            ////intento desesperado
            //double[] CP = new double[] { 12672, 13996, 13148, 1708.91, 2263.85, 3306.26, 10549, 8911, 0, 0, 0, 0, 0, 0, 0, 0, 1379.92, 1294.04, 0, 0 };

            //Capacidad destino %i en %t
            double[] CD = new double[] { 12500, 0, 0, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999, 99999 };

            //Penalizadores por desvíos (función convexa)---------------------------------------------
            int Nk = 3;
            //double pond = 9999999999999;
            //double[] Ck = new double[] { 0.84*pond, 1.7360 * pond, 2.6320 * pond };
            //double[] dk = new double[] {-1764 * pond, -7534 * pond, -17318 * pond };


            /*double pond = 1;
            double[] Ck = new double[] { 40 * pond, 1320 * pond, 2600 * pond };
            double[] dk = new double[] { -400 * pond, -435600 * pond, -1690000 * pond };*/


            // Relajar penalización ley
            double[] Ck = new double[] { 0, 0, 0 };
            double[] dk = new double[] { 0, 0, 0 };

            //----------------------------------------------------------------------------------------


            //#######################################################################################################################
            //CÓDIGO CAMPAÑA
            /*
            if (CAMPAÑA == 1)
            {

                //Beneficio por período de extraer mineral
                for (int t = 1; t < T; t = t + 1)
                {
                    B_Min[t] =35;
                }

                //CAP palas
                CP = new double[] { 4117.54, 5417.58, 5000,4200 , 3300, 4100, 4869.83, 4500.43, 0, 0, 0, 0, 0, 0, 0, 0, 1379.92, 1294.04, 0, 0 };
               
                //nominales
                //double[] CP = new double[] { 12672, 13996, 13148, 4842, 9413, 10692, 10549, 8911, 0, 0, 0, 0, 0, 0, 0, 0, 4190, 4302, 0, 0 };
                
                //Rescatar cuanto min se ha sacado
                double Sacado = 0;
                foreach (int i in S1)
                {
                    Sacado = Sacado + DMPP[0, i-fil]/Lmin;
                }

                //Obtener tonelaje objetivo
                double sumaPM0 = 0;
                foreach (int i in S1)
                {
                    sumaPM0 = sumaPM0 + PM0[i - fil];
                }

                //Botar todo si se alcanzó la meta
                if (Sacado > 290)
                {
                    for (int i = 0; i < col; i = i + 1)
                    {
                        PM0[i] = 0;
                    }
                    for (int t = 1; t < T; t = t + 1)
                    {
                        B_Min[t] = 0;
                    }
                    foreach (int i in S1)
                    {
                        CP[i - fil] = 0;
                    }
                }
            }*/
            //_______________________________________________________________________________________________________________________


            //CODIGO DE PENALIZACION EN BASE A HOLGURAS (REMANENTE)__________________________________________________________________

            if (B_REMANENTE == 1)
            {
                double FPond = 50; //Factor de escala holgura
                double FMin = 1.3; //factor de correccion mineral
                //MINERAL
                double[,] B_Min2 = new double[col, T];
                foreach (int i in S1)
                {
                    double Remanente = Math.Max(0, PM0[i - fil] - DMPP[0, i - fil]);
                    for (int t = 0; t < T; t = t + 1)
                    {
                        B_Min2[i - fil, t] = FPond * FMin * Remanente / CP[i - fil];
                        B_Min[i - fil, t] = B_Min[i - fil, t] + B_Min2[i - fil, t];
                        //Console.WriteLine("B_MIN(" + i + "," + t + "): " + B_Min[i-fil, t]);
                    }
                }

                //ESTERIL
                double[,] B_Est2 = new double[NF, T];
                for (int f = 0; f < NF; f = f + 1)
                {
                    double CapPalasFase = 0;
                    double Remanente = PF[f];
                    for (int j = 0; j < col; j = j + 1)
                    {
                        CapPalasFase = CapPalasFase + CP[j] * F[f, j];
                    }
                    for (int t = 0; t < T; t = t + 1)
                    {
                        B_Est2[f, t] = FPond * Remanente / CapPalasFase;
                        B_Est[f, t] = B_Est[f, t] + B_Est2[f, t];
                        //Console.WriteLine("B_EST(" + f +","+t +"): "+ B_Est[f, t]);
                    }

                }
            }

            //_______________________________________________________________________________________________________________________



            //Umbrales
            int NU = 3;
            double[] FlujoUmbral = new double[] { 4000, 5500, 7000 };
            double[] TiempoAdicional = new double[] { 0.1, 0.1, 0.1 };






            //_________________________________________________________________________________________________________
            //                                 EVALUAR FACTIBILIDAD DE RESTRICCIONES
            //_________________________________________________________________________________________________________
            //1.CUMPLIMIENTO DE PLAN MINERAL
            double[] VfPM = new double[col];     //Vector de factibilidad de restricción (cumplimiento de mineral por pala)
            double sumareqmin = 0; //suma de los requerimientos de minerales
            for (int i = 0; i < col; i = i + 1)
            {
                sumareqmin = sumareqmin + PM[i];
                VfPM[i] = 1;      //Corregir por disponibilidad de pala
                if ((T - 2 + Vdt[1]) * CP[i] * PDisp[i] < PM[i])
                {

                    //Console.WriteLine("Infactible Pala Mineral: " + i + ", la restricción ha sido relajada ");
                }
            }

            //1.2 Chequear capacidad de destinos
            double sumacapchan = 0;
            for (int i = 0; i < NCH; i = i + 1)
            {
                sumacapchan = sumacapchan + CD[i];
            }
            if (sumacapchan < sumareqmin)
            {
                for (int i = 0; i < col; i = i + 1)
                {
                    VfPM[i] = 1;
                }
            }

            //2.CUMPLIMIENTO DE PLAN ESTERIL
            double[] VfPF = new double[col];    //Vector de factibilidad de restricción (cumplimiento de mineral por fase)
            for (int f = 0; f < NF; f = f + 1)
            {
                //***************CAMBIAR DESPUÉS************************+
                VfPF[f] = 1;

                double suma_cap = 0;
                for (int i = 0; i < col; i = i + 1)
                {
                    suma_cap = suma_cap + F[f, i] * CP[i] * PDisp[i]; //Solo sumar las capacidades de las palas que pertenecen a la fase
                }

                if (suma_cap * (T - 2 + Vdt[1]) < PF[f])
                {
                    VfPF[f] = 1;
                    //Console.WriteLine("Infactible Fase de Esteril: " + f + ", la restricción ha sido relajada ");
                }
            }




            //____________________________________________________________________________________________________________
            //                                                  MODELO
            //____________________________________________________________________________________________________________


            GRBEnv envMIP = new GRBEnv();
            GRBModel MIP = new GRBModel(envMIP);


            //VARIABLES
            //Caminos
            GRBVar[,,] X = new GRBVar[dim, dim, T];       //Flujo de (i,j) en t
            GRBVar[,,] Y = new GRBVar[dim, dim, T];       //Camiones hora asignados a camino (i,j) en t 

            //Desvío de ley de mineral
            GRBVar[,] DESV = new GRBVar[NCH, T];        //V.Abs de la desviación de la ley de mineral por período, por chancador
            GRBVar[,] PENDESV = new GRBVar[NCH, T];     //Penaliz de la desviación de la ley de mineral por período, por chancador

            //Slack Nro Camiones
            GRBVar[] Ys = new GRBVar[T];

            //Remanentes
            GRBVar[] REM_PM = new GRBVar[col];
            GRBVar[] REM_PF = new GRBVar[NF];


            //Colas
            GRBVar[,,] UMB = new GRBVar[dim, T, NU];
            GRBVar[,,] TA = new GRBVar[dim, T, NU];



            for (int i = 0; i < dim; i = i + 1)
            {
                for (int j = 0; j < dim; j = j + 1)
                {
                    for (int t = 0; t < T; t = t + 1)
                    {
                        X[i, j, t] = MIP.AddVar(0.0, 1000000, 0.0, GRB.CONTINUOUS, "X" + i + "_" + j + "_" + t);
                        Y[i, j, t] = MIP.AddVar(0.0, 100000, 0.0, GRB.CONTINUOUS, "Y" + i + "_" + j + "_" + t);
                    }
                }
            }

            for (int i = 0; i < dim; i = i + 1)
            {
                for (int t = 0; t < T; t = t + 1)
                {

                    for (int k = 0; k < NU; k++)
                    {
                        UMB[i, t, k] = MIP.AddVar(0.0, 1, 0.0, GRB.BINARY, "U" + i + "_" + t);
                        TA[i, t, k] = MIP.AddVar(0.0, 100, 0.0, GRB.CONTINUOUS, "TA" + i + "_" + t + "_" + k);
                    }
                }
            }


            //Desvío Ley Mineral
            foreach (int i in S2)
            {
                for (int t = 0; t < T; t = t + 1)
                {
                    DESV[i, t] = MIP.AddVar(0.0, 100000, 0.0, GRB.CONTINUOUS, "DESV" + t);
                    PENDESV[i, t] = MIP.AddVar(0.0, 100000, 0.0, GRB.CONTINUOUS, "PEN_DESV" + t);
                }
            }

            for (int t = 0; t < T; t = t + 1)
            {
                Ys[t] = MIP.AddVar(0.0, 100000, 0.0, GRB.CONTINUOUS, "Y slack" + t);
            }

            for (int i = 0; i < col; i++)
            {
                REM_PM[i] = MIP.AddVar(0.0, 100000, 0.0, GRB.CONTINUOUS, "Remanente Plan Mineral Pala" + i);
            }

            for (int f = 0; f < NF; f++)
            {
                REM_PF[f] = MIP.AddVar(0.0, 100000, 0.0, GRB.CONTINUOUS, "Remanente Plan Esteril Fase" + f);
            }


            MIP.Update();



            //------------------------------------------   
            //    RESTRICCIONES
            //------------------------------------------

            //0.Evitar flujos extraños
            foreach (int i in S1)
            {
                foreach (int j in S5)
                {
                    for (int t = 0; t < T; t = t + 1)
                    {
                        MIP.AddConstr(X[i, j, t], GRB.EQUAL, 0, "FlujosRaros1");
                    }
                }
            }


            //0.Evitar flujos extraños
            foreach (int i in S4)
            {
                foreach (int j in S2uS3)
                {
                    for (int t = 0; t < T; t = t + 1)
                    {
                        MIP.AddConstr(X[i, j, t], GRB.EQUAL, 0, "FlujosRaros2");
                    }
                }
            }

            //1. Cumplir plan mineral pala j (se desactiva si no es factible)
            foreach (int i in S1)
            {
                GRBLinExpr suma = 0;
                foreach (int j in S2)
                {
                    for (int t = 0; t < T; t = t + 1)
                    {
                        suma = suma + X[i, j, t] * Vdt[t];
                    }
                }
                MIP.AddConstr(PM[i - fil] - M2 * VfPM[i - 30], GRB.LESS_EQUAL, suma, "Cumplir plan mineral");
            }

            ////1. Cumplir plan mineral pala j (v2)
            //foreach (int i in S1)
            //{
            //    GRBLinExpr suma = 0;
            //    foreach (int j in S2)
            //    {
            //        for (int t = 0; t < T; t = t + 1)
            //        {
            //            suma = suma + X[i, j, t] * Vdt[t];
            //        }
            //    }
            //    MIP.AddConstr(PM[i - 30] - M2 * VfPM[i - 30], GRB.LESS_EQUAL, suma, "Cumplir plan mineral");
            //}



            //2. Cumplir plan de cada fase
            for (int f = 0; f < NF; f = f + 1)
            {
                GRBLinExpr suma = 0;
                foreach (int i in S4)
                {
                    foreach (int j in S5)
                    {
                        for (int t = 0; t < T; t = t + 1)
                        {
                            suma = suma + X[i, j, t] * F[f, i - 30] * Vdt[t];
                        }
                    }
                }
                MIP.AddConstr(PF[f] - M2 * VfPF[f], GRB.LESS_EQUAL, suma, "Cumplir demanda fases de esteril");
            }

            //3.1  Respetar capacidad de palas
            foreach (int i in S1) //Mineral
            {
                for (int t = 0; t < T; t = t + 1)
                {
                    GRBLinExpr suma = 0;
                    foreach (int j in S2uS3)
                    {
                        suma = suma + X[i, j, t];
                    }
                    MIP.AddConstr(suma, GRB.LESS_EQUAL, CP[i - 30] * VCO[i] * PDisp[i - 30], "Capacidad Palas Mineral");
                }
            }
            foreach (int i in S4) //Esteril
            {
                for (int t = 0; t < T; t = t + 1)
                {
                    GRBLinExpr suma = 0;
                    foreach (int j in S5)
                    {
                        suma = suma + X[i, j, t];
                    }
                    MIP.AddConstr(suma, GRB.LESS_EQUAL, CP[i - 30] * VCO[i] * PDisp[i - 30], "Capacidad Palas Esteril");
                }
            }

            //3.2 Respetar capacidad servidores
            //foreach (int i in S2uS3uS5)
            for (int i = 0; i < fil; i = i + 1)
            {
                for (int t = 0; t < T; t = t + 1)
                {
                    GRBLinExpr suma = 0;
                    //foreach (int j in S1uS4)
                    for (int j = fil; j < dim; j = j + 1)
                    {
                        suma = suma + X[i, j, t];
                    }
                    MIP.AddConstr(suma, GRB.LESS_EQUAL, CD[i] * DDisp[i], "Capacidad Servidores");
                }
            }

            //4. Conservación de flujo (lo que entra es igual a lo que sale en cada servidor)
            foreach (int j in S1uS4) //Palas
            {
                for (int t = 0; t < T; t = t + 1)
                {
                    GRBLinExpr suma1 = 0;
                    GRBLinExpr suma2 = 0;
                    foreach (int i in S2uS3uS5)
                    {
                        suma1 = suma1 + X[i, j, t];
                        suma2 = suma2 + X[j, i, t];
                    }
                    MIP.AddConstr(suma1, GRB.EQUAL, suma2, "Continuidad palas");
                }
            }
            foreach (int j in S2uS3uS5) //Destinos
            {
                for (int t = 0; t < T; t = t + 1)
                {
                    GRBLinExpr suma1 = 0;
                    GRBLinExpr suma2 = 0;
                    foreach (int i in S1uS4)
                    {
                        suma1 = suma1 + X[i, j, t];
                        suma2 = suma2 + X[j, i, t];
                    }
                    MIP.AddConstr(suma1, GRB.EQUAL, suma2, "Continuidad destinos");
                }
            }



            //6. Relación variables (camiones)----------------------------------------------------------------------------------

            //Camiones Cargados
            foreach (int i in S1uS4)
            {
                foreach (int j in S2uS3uS5)
                {
                    for (int t = 0; t < T; t = t + 1)
                    {
                        MIP.AddConstr(Y[i, j, t] * L, GRB.GREATER_EQUAL, X[i, j, t] * (TV[i, j] + TC[i - 30]), "Definición camiones- Cargados");
                    }
                }
            }
            //Camiones Vacíos
            foreach (int i in S2uS3uS5)
            {
                foreach (int j in S1uS4)
                {
                    for (int t = 0; t < T; t = t + 1)
                    {
                        MIP.AddConstr(Y[i, j, t] * L, GRB.GREATER_EQUAL, X[i, j, t] * (TV[i, j] + TD[i]), "Definición camiones- Vacíos");
                    }
                }
            }


            //COLAS:

            //UMBRALES
            for (int t = 0; t < T; t = t + 1)
            {
                foreach (int i in S1uS4)
                {
                    GRBLinExpr suma = 0;
                    for (int j = 0; j < dim; j++)
                    {
                        suma = suma + X[i, j, t];
                    }
                    for (int k = 0; k < NU; k++)
                    {
                        MIP.AddConstr(M * UMB[i, t, k], GRB.GREATER_EQUAL, suma - FlujoUmbral[k], "Umbrales" + i + "_" + t + "_" + k);
                    }
                }
            }

            //Tiempo Adicional
            for (int t = 0; t < T; t = t + 1)
            {
                foreach (int i in S1uS4)
                {
                    GRBLinExpr suma = 0;
                    for (int j = 0; j < dim; j++)
                    {
                        suma = suma + X[i, j, t];
                    }
                    for (int k = 0; k < NU; k++)
                    {
                        MIP.AddConstr(TA[i, t, k], GRB.LESS_EQUAL, M * UMB[i, t, k], "TA1-" + i + "_" + t + "_" + k);
                        MIP.AddConstr(TA[i, t, k], GRB.LESS_EQUAL, TiempoAdicional[k] * suma, "TA2-" + i + "_" + t + "_" + k);
                        MIP.AddConstr(TA[i, t, k], GRB.GREATER_EQUAL, TiempoAdicional[k] * suma - M * (1 - UMB[i, t, k]), "TA2-" + i + "_" + t + "_" + k);

                    }
                }
            }


            //9.Disponibilidad camiones
            for (int t = 0; t < T; t = t + 1)
            {
                GRBLinExpr suma1 = 0;
                GRBLinExpr suma2 = 0;
                for (int i = 0; i < dim; i = i + 1)
                {
                    for (int j = 0; j < dim; j = j + 1)
                    {
                        suma1 = suma1 + Y[i, j, t];
                    }
                }
                foreach (int i in S1uS4)
                {
                    for (int k = 0; k < NU; k++)
                    {
                        suma2 = suma2 + TA[i, t, k] / L;
                    }
                }
                MIP.AddConstr(suma1 + suma2, GRB.LESS_EQUAL, NC + Ys[t], "usar todos los camiones en t");
            }

            //11.VALOR ABSOLUTO DESVIACIÓN LEY
            foreach (int j in S2)
            {
                for (int t = 1; t < T; t = t + 1)
                {
                    GRBLinExpr suma_ley = 0;
                    GRBLinExpr suma_flujo = 0;
                    foreach (int i in S1)
                    {
                        suma_ley = suma_ley + X[i, j, t] * LeyP[i - 30];
                        suma_flujo = suma_flujo + X[i, j, t];
                    }
                    MIP.AddConstr(DESV[j, t] >= (LeyObj * suma_flujo - suma_ley), "VAbs Desvío ley 1");
                    MIP.AddConstr(DESV[j, t] >= (suma_ley - LeyObj * suma_flujo), "VAbs Desvío ley 2");
                }
            }

            //12.Penalización por desviación

            for (int t = 1; t < T; t = t + 1)
            {
                foreach (int j in S2)
                {
                    for (int k = 0; k < Nk; k = k + 1)
                    {
                        MIP.AddConstr(PENDESV[j, t] >= DESV[j, t] * Ck[k] + dk[k], "Penaliz ley");
                    }
                }
            }

            //Remanente Plan mineral
            foreach (int i in S1)
            {
                GRBLinExpr suma = 0;
                for (int t = 0; t < T; t++)
                {
                    foreach (int j in S2)
                    {
                        suma = suma + X[i, j, t] * Vdt[t];
                    }
                }
                MIP.AddConstr(REM_PM[i - fil] >= PM[i - fil] - suma, "");
            }



            MIP.Update();

            //FUNCIÓN OBJETIVO

            GRBLinExpr SDesv = 0;  //Suma de las desviaciones por período durante turno completo
            GRBLinExpr SMin = 0;   //Beneficio por mineral extraído durante turno completo
            GRBLinExpr SEst = 0;   //Beneficio estéril extraído durante turno completo
            GRBLinExpr SPenPM = 0; //Penalización por no cumplir plan de mineral (pala)
            GRBLinExpr SPenPF = 0; //Penalización por no cumplir plan de estéril (fase)
            GRBLinExpr SPslack = 0; //Penalización por no cumplir plan de estéril (fase)





            //foreach (int i in S2)
            //{
            //    for (int t = 1; t < T; t = t + 1)
            //    {
            //        SDesv = SDesv + DESV[i, t];
            //    }
            //}

            //Penalización por desvío
            foreach (int i in S2)
            {
                for (int t = 1; t < T; t = t + 1)
                {
                    SDesv = SDesv + PENDESV[i, t];
                }
            }

            //Mineral extraído
            for (int t = 1; t < T; t = t + 1)
            {
                foreach (int i in S1)
                {
                    foreach (int j in S2)
                    {
                        SMin = SMin + X[i, j, t] * B_Min[i - fil, t];
                    }
                }
            }

            ////Esteril extraído
            //for (int t = 1; t < T; t = t + 1)
            //{
            //    foreach (int i in S4)
            //    {
            //        foreach (int j in S5)
            //        {
            //            SEst = SEst + X[i, j, t] * B_Est[t];
            //        }
            //    }
            //}

            for (int t = 1; t < T; t = t + 1)
            {
                for (int f = 1; f < NF; f = f + 1)
                {
                    foreach (int i in S4)
                    {
                        foreach (int j in S5)
                        {
                            SEst = SEst + X[i, j, t] * B_Est[i, t] * F[f, i];
                        }
                    }
                }
            }




            //Penaliz Cumplir plan mineral pala j (se activa si no es factible)
            foreach (int i in S1)
            {
                SPenPM = SPenPM + REM_PM[i - fil];

            }

            for (int t = 0; t < T; t = t + 1)
            {
                SPslack = SPslack + Ys[t];
            }



            //Penaliz  no cumplir plan de cada fase
            for (int f = 0; f < NF; f = f + 1)
            {
                foreach (int i in S4)
                {
                    foreach (int j in S5)
                    {
                        for (int t = 0; t < T; t = t + 1)
                        {
                            SPenPF = SPenPF + (PF[f] - X[i, j, t] * F[f, i - 30] * Vdt[t]) * VfPF[f];
                        }
                    }
                }
            }





            //Multiplicar por costes(función objetivo)
            MIP.SetObjective(SDesv - SMin - SEst + alphaPM * SPenPM + alphaPF * SPenPF + SPslack * alphaSlack, GRB.MINIMIZE);
            //MIP.SetObjective(SDesv - SMin - SEst , GRB.MINIMIZE);
            //MIP.SetObjective(SDesv , GRB.MINIMIZE);
            MIP.Update();



            //CORRER MIP--------------------------------------
            MIP.Optimize();
            ////RELAJACION LINEAL-----------------------------
            //GRBModel MIPR = new GRBModel(envMIP);
            //MIPR = MIP.Relax();
            //MIPR.Optimize();
            ////----------------------------------------------










            //___________________________________________________________________________________
            //                         CONVERTIR A FORMATO REQUERIDO
            //___________________________________________________________________________________

            double[,,] Tcij = new double[30, 20, T];
            double[,,] Ycij = new double[30, 20, T];
            for (int i = 30; i < 50; i = i + 1)
            {
                for (int j = 0; j < 30; j = j + 1)
                {
                    for (int t = 0; t < T; t = t + 1)
                    {
                        if (Math.Truncate(X[i, j, t].X) > 300)
                        {
                            Tcij[j, i - 30, t] = Math.Truncate(X[i, j, t].X);
                        }
                        Ycij[j, i - 30, t] = Math.Truncate(Y[i, j, t].X);
                    }
                }
            }


            //Crear matriz de flujos cargados
            double[,,] Tvij = new double[30, 20, T];
            double[,,] Yvij = new double[30, 20, T];
            for (int i = 0; i < 30; i = i + 1)
            {
                for (int j = 30; j < 50; j = j + 1)
                {
                    for (int t = 0; t < T; t = t + 1)
                    {
                        Tvij[i, j - 30, t] = Math.Truncate(X[i, j, t].X);
                        Yvij[i, j - 30, t] = Math.Truncate(Y[i, j, t].X);
                    }
                }
            }


            //#############################################################################################################
            //#############################################################################################################

            double sumayc = 0;
            double sumayv = 0;
            double sumatc = 0;
            double sumatv = 0;

            for (int t = 0; t < T; t = t + 1)
            {

                for (int a = 0; a < 30; a = a + 1)
                {

                    double sumajyc = 0;
                    double sumajyv = 0;
                    double sumajtc = 0;
                    double sumajtv = 0;
                    for (int b = 0; b < 20; b = b + 1)
                    {

                        vectores.MipYc[a, b, Convert.ToInt32(Ttot - T + t)] = Convert.ToDouble(Ycij[a, b, t]);
                        vectores.MipYv[a, b, Convert.ToInt32(Ttot - T + t)] = Convert.ToDouble(Yvij[a, b, t]);
                        vectores.MipTc[a, b, Convert.ToInt32(Ttot - T + t)] = Convert.ToDouble(Tcij[a, b, t]);
                        vectores.MipTv[a, b, Convert.ToInt32(Ttot - T + t)] = Convert.ToDouble(Tvij[a, b, t]);
                        sumayc = sumayc + vectores.MipYc[a, b, Convert.ToInt32(Ttot - T + t)]; //suma todos los flujos llenos
                        sumayv = sumayv + vectores.MipYv[a, b, Convert.ToInt32(Ttot - T + t)]; //suma todos los flujos vacios
                        sumatc = sumatc + vectores.MipTc[a, b, Convert.ToInt32(Ttot - T + t)]; //suma todos los camiones llenos
                        sumatv = sumatv + vectores.MipTv[a, b, Convert.ToInt32(Ttot - T + t)]; //suma todos los camiones vacios
                        sumajyc = sumajyc + Convert.ToDouble(Ycij[a, b, t]); //suma todo el flujo cargado al botadero "a"
                        sumajyv = sumajyv + Convert.ToDouble(Yvij[a, b, t]); //suma todo el flujo vacio al botadero "a"
                        sumajtc = sumajtc + Convert.ToDouble(Tcij[a, b, t]); //suma todos los camiones cargados yendo a botadero "a"
                        sumajtv = sumajtv + Convert.ToDouble(Tvij[a, b, t]); //suma todos los camiones vacios yendo a botadero "a"

                    }

                    vectores.MipYc[a, 20, Convert.ToInt32(Ttot - T + t)] = sumajyc; // deja total
                    vectores.MipYv[a, 20, Convert.ToInt32(Ttot - T + t)] = sumajyv; //
                    vectores.MipTc[a, 20, Convert.ToInt32(Ttot - T + t)] = sumajtc; //
                    vectores.MipTv[a, 20, Convert.ToInt32(Ttot - T + t)] = sumajtv; //
                }

                for (int b = 0; b < 20; b = b + 1)
                {

                    double sumaiyc = 0;
                    double sumaiyv = 0;
                    double sumaitc = 0;
                    double sumaitv = 0;

                    for (int a = 0; a < 30; a = a + 1)
                    {

                        sumaiyc = sumaiyc + Convert.ToDouble(Ycij[a, b, t]);
                        sumaiyv = sumaiyv + Convert.ToDouble(Yvij[a, b, t]);
                        sumaitc = sumaitc + Convert.ToDouble(Tcij[a, b, t]);
                        sumaitv = sumaitv + Convert.ToDouble(Tvij[a, b, t]);

                    }

                    vectores.MipYc[30, b, Convert.ToInt32(Ttot - T + t)] = sumaiyc;
                    vectores.MipYv[30, b, Convert.ToInt32(Ttot - T + t)] = sumaiyv;
                    vectores.MipTc[30, b, Convert.ToInt32(Ttot - T + t)] = sumaitc;
                    vectores.MipTv[30, b, Convert.ToInt32(Ttot - T + t)] = sumaitv;
                }

                vectores.MipYc[30, 20, Convert.ToInt32(Ttot - T + t)] = sumayc;
                vectores.MipYv[30, 20, Convert.ToInt32(Ttot - T + t)] = sumayv;
                vectores.MipTc[30, 20, Convert.ToInt32(Ttot - T + t)] = sumatc;
                vectores.MipTv[30, 20, Convert.ToInt32(Ttot - T + t)] = sumatv;
            }

            for (int b = 0; b < 21; b = b + 1)
            {
                for (int a = 0; a < 31; a = a + 1)
                {

                    vectores.PlYc[a, b] = vectores.MipYc[a, b, periodos2];
                    vectores.PlYv[a, b] = vectores.MipYv[a, b, periodos2];
                    vectores.PlTc[a, b] = vectores.MipTc[a, b, periodos2];
                    vectores.PlTv[a, b] = vectores.MipTv[a, b, periodos2];
                    //*******
                }
            }
            string numerito;
            for (int a = 0; a < 31; a = a + 1)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("important.csv", true))
                {
                    numerito = Convert.ToString(vectores.PlTc[a, 0]) + "," + Convert.ToString(vectores.PlTc[a, 1]) + "," + Convert.ToString(vectores.PlTc[a, 2]) + "," + Convert.ToString(vectores.PlTc[a, 3]) + "," + Convert.ToString(vectores.PlTc[a, 4]) + "," + Convert.ToString(vectores.PlTc[a, 5]) + "," + Convert.ToString(vectores.PlTc[a, 6]) + "," + Convert.ToString(vectores.PlTc[a, 7]) + "," + Convert.ToString(vectores.PlTc[a, 8]) + "," + Convert.ToString(vectores.PlTc[a, 9]) + "," + Convert.ToString(vectores.PlTc[a, 10]) + "," + Convert.ToString(vectores.PlTc[a, 11]) + "," + Convert.ToString(vectores.PlTc[a, 12]) + "," + Convert.ToString(vectores.PlTc[a, 13]) + "," + Convert.ToString(vectores.PlTc[a, 14]) + "," + Convert.ToString(vectores.PlTc[a, 15]) + "," + Convert.ToString(vectores.PlTc[a, 16]) + "," + Convert.ToString(vectores.PlTc[a, 17]) + "," + Convert.ToString(vectores.PlTc[a, 18]) + "," + Convert.ToString(vectores.PlTc[a, 19]) + "," + Convert.ToString(vectores.PlTc[a, 20]) + "," + Convert.ToString(timenow);
                    file.WriteLine(numerito);
                }

            }

            //_timenow.StateValue = vectores.MipTc[0, 20, 1];


            MIP.Dispose();
            envMIP.Dispose();

            sr.serializa(vectores);
            sr.closeStream();
            return ExitType.FirstExit;

        }
    }
}







