using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimioAPI;
using SimioAPI.Extensions;
using Vectores;
using System.IO;

namespace Dinamico
{
    class UserStep1Definition : IStepDefinition
    {
        #region IStepDefinition Members

        /// <summary>
        /// Property returning the full name for this type of step. The name should contain no spaces.
        /// </summary>
        public string Name
        {
            get { return "Dinamico"; }
        }

        /// <summary>
        /// Property returning a short description of what the step does.
        /// </summary>
        public string Description
        {
            get { return "Manda camiones a destino"; }
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
        static readonly Guid MY_ID = new Guid("{993bc1bd-8160-430b-b84c-fb6079db36db}");

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

            pd = schema.AddStateProperty("Salida");
            pd.Description = "Siguiente pala";

            pd = schema.AddStateProperty("Aux");
            pd.Description = "Camino i para despues";

            pd = schema.AddStateProperty("Aux2");
            pd.Description = "Numero id camion";

            pd = schema.AddStateProperty("NumCam");
            pd.Description = "Numero de camiones";

            pd = schema.AddStateProperty("Needtime");
            pd.Description = "Need time";

            pd = schema.AddStateProperty("Losttones");
            pd.Description = "Numero de camiones";

            pd = schema.AddStateProperty("Needch");
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
        IStateProperty _propTimenow;
        IStateProperty _propSalida;
        IStateProperty _propAux;
        IStateProperty _propAux2;
        IStateProperty _propNumCam;
        IStateProperty _propNeedtime;
        IStateProperty _propLosttones;
        IStateProperty _propNeedch;
        Vect vectores;

        public UserStep1(IPropertyReaders properties)
        {
            _properties = properties;
            _propTimenow = (IStateProperty)_properties.GetProperty("Timenow");
            _propSalida = (IStateProperty)_properties.GetProperty("Salida");
            _propAux = (IStateProperty)_properties.GetProperty("Aux");
            _propAux2 = (IStateProperty)_properties.GetProperty("Aux2");
            _propNumCam = (IStateProperty)_properties.GetProperty("NumCam");
            _propNeedtime = (IStateProperty)_properties.GetProperty("Needtime");
            _propLosttones = (IStateProperty)_properties.GetProperty("Losttones");
            _propNeedch = (IStateProperty)_properties.GetProperty("Needch");
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
            IState _salida = _propSalida.GetState(context);
            double salida = Convert.ToDouble(_salida.StateValue);
            IState _aux = _propAux.GetState(context);
            double aux = Convert.ToDouble(_aux.StateValue);
            IState _aux2 = _propAux2.GetState(context);
            double aux2 = Convert.ToDouble(_aux2.StateValue);
            IState _numcam = _propNumCam.GetState(context);
            double numcam = Convert.ToDouble(_numcam.StateValue);
            IState _needtime = _propNeedtime.GetState(context);
            double needtime = Convert.ToDouble(_needtime.StateValue);
            IState _losttones = _propLosttones.GetState(context);
            double losttones = Convert.ToDouble(_losttones.StateValue);
            IState _needch = _propNeedch.GetState(context);
            double needch = Convert.ToDouble(_needch.StateValue);
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

            int y; //para entrar al loop
            int palaneedy = 0; //numero de la pala needy
            double caminoipalaneedy = 0;
            double min; //var aux para encontrar pala needy
            double[,] Din22 = new double[200, 11]; //matriz con datos para encontrar camion con menos lost_tones
            double[] need_time = new double[600]; //matriz para encontrar al camino mas needy


            for (int x = 0; x < numcam; x++) //construye lo que falta de la matriz de los camiones (lo que depende de la pala needy)
            {
                if (vectores.DesCam[x, 3] == 1) // indicador si necesita pala
                {
                    Din22[x, 8] = 1;
                }

            }


            for (int vuelta = 0; vuelta < 20; vuelta++)
            {
                for (int x = 0; x < 30; x++)
                {
                    vectores.Din1[x + 30 * (vuelta), 1] = vectores.PlTc[30, vuelta]; //rescatar el haulage por hora de cada camino
                }

            }

            int desesperacion;
            int desesperacion2;
            desesperacion = 0;
            desesperacion2 = 0;
            for (int x = 0; x < 600; x++) //recupera los tonelajes del PL
            {
                vectores.Din1[x, 0] = vectores.PlTc[desesperacion, desesperacion2];
                desesperacion = desesperacion + 1;
                if (desesperacion > 29)
                {
                    desesperacion2 = desesperacion2 + 1;
                    desesperacion = 0;
                }


            }

            for (int x = 0; x < 200; x++)
            {
                Din22[x, 0] = x + 1; //numero camion
                Din22[x, 1] = 320; //capacidad
                Din22[x, 2] = vectores.PlTc[30, 20]; //total rate shovels
                Din22[x, 3] = (vectores.PlYc[30, 20] + vectores.PlYv[30, 20]) * 320; //total trucks en toneladas
            }

            int newaux = 0;
            int camionpiola = 100;
            min = 999999;//var aux para encontrar pala needy
            double min3;
            double[,] Din12 = (double[,])vectores.Din1.Clone(); //matriz con datos para encontrar camion con menos lost_tones

            do
            {
                min3 = 0;
                min = 999999;
                y = 0;

                int indicemin1 = 900000000; //var aux para encontrar pala needy

                for (int x = 0; x < 600; x++)
                {
                    if (Din12[x, 0] != 0)
                    {

                        need_time[x] = Din12[x, 4] + ((Din12[x, 0] / Din12[x, 1]) * (Din12[x, 5] * 300 - Din12[x, 1]) / (Din12[x, 0]));

                        //need_time[x] = Din12[x, 4] + ((Din12[x, 0] / Din12[x, 1]) * (Din12[x, 5] * 300 - Din12[x, 1] * timenow)) / (Din12[x, 7] * 300 / (timenow));

                        if (need_time[x] < min) //encuentra el menor y lo guarda en palaneedy
                        {

                            indicemin1 = x;

                            palaneedy = Convert.ToInt32(Din12[x, 3]);

                            min = need_time[x];

                            caminoipalaneedy = Din12[x, 2];

                        }

                    }


                }

                //if (min < timenow)
                //{
                    //Din12[indicemin1, 7] = Din12[indicemin1, 7] + 1;


                    for (int x = 0; x < numcam; x++) //construye lo que falta de la matriz de los camiones (lo que depende de la pala needy)

                    {
                    /*
                     * sacar la fila bien 
                     *
                     */
                        
                        vectores.Ui[0] = vectores.Ui[0];

                        Din22[x, 4] = vectores.Fila[palaneedy - 1] * vectores.TCj[palaneedy - 1] + vectores.DesCam[x, 4];

                        if (Din22[x, 8] == 1)
                        {
                            Din22[x, 5] = vectores.TCV[Convert.ToInt32(vectores.DesCam[x, 2] - 1), palaneedy - 1]; //recoje tiempo de donde esta a la pala needy
                        }

                        Din22[x, 6] = vectores.PlTc[30, palaneedy - 1]; //tonelaje total asignado a palaneedy

                        if (vectores.Fila[palaneedy - 1] == 0) //si no hay fila
                        {
                            if (Din22[x, 8] == 1)
                            {
                                Din22[x, 7] = vectores.TCV[Convert.ToInt32(vectores.DesCam[x, 2] - 1), palaneedy - 1]; //el shovel idle rate es Tij

                            }
                        }


                    }


                    double min2 = 90000000;
                    camionpiola = 100; //que sea mayor que la ultima linea
                    double[] lost_tones = new double[200];
                    newaux = 0;

                    for (int x = 0; x < numcam; x++) //calcula lost_tones para los importantes
                    {

                        if (Din22[x, 8] == 1)
                        {

                            lost_tones[x] = ((Din22[x, 1] * Din22[x, 2] / Din22[x, 3]) * (Din22[x, 4] + Din22[x, 5]) + (Din22[x, 6] * Din22[x, 7]));
                            //lost_tones[x] = (Din22[x, 2] / Din22[x, 3]) * (Din22[x, 3] + Din22[x, 4]) + (Din22[x, 5] * Din22[x, 6]);

                            if (lost_tones[x] < min2) //recupera el menor y actualiza el valor
                            {

                                min2 = lost_tones[x];

                                //camionpiola = Convert.ToInt32(Din22[x, 0]);

                                camionpiola = x + 1;

                            }

                        }

                        else

                        {

                            newaux = newaux + 1;

                        }

                    }


                    Din22[camionpiola - 1, 9] = palaneedy; // asignar camino a camion
                    Din22[camionpiola - 1, 8] = 0; //lo saca de la lista
                    Din22[camionpiola - 1, 10] = caminoipalaneedy;

                    for (int x = 0; x < numcam; x++)
                    {

                        if (Din22[x, 8] == 1)
                        {
                            y = 1;
                        }

                    }

                    for (int x = 0; x < 30; x++)
                    {
                        Din12[x + (palaneedy - 1) * 30, 4] = timenow;
                        Din12[x + (palaneedy - 1) * 30, 5] = Din12[x + (palaneedy - 1) * 30, 5] + 1;

                    }
                    newaux = indicemin1;
//                }

                /*else
                {
                    min = 959;
                    int palapiola = 100; //que sea mayor que la ultima linea
                    double min2 = 90000000;
                    for (int palanedy = 1; palanedy < 20; palanedy++)
                    {

                        if (vectores.PlTc[30, palanedy - 1] > 0)
                        {

                            Din22[Convert.ToInt32(aux2 - 1), 4] = vectores.Fila[palanedy - 1] * vectores.TCj[palanedy - 1] + vectores.DesCam[Convert.ToInt32(aux2 - 1), 4];

                            Din22[Convert.ToInt32(aux2 - 1), 5] = vectores.TCV[Convert.ToInt32(vectores.DesCam[Convert.ToInt32(aux2 - 1), 2] - 1), palanedy - 1]; //recoje tiempo de donde esta a la pala needy

                            Din22[Convert.ToInt32(aux2 - 1), 6] = vectores.PlTc[30, palanedy - 1]; //tonelaje total asignado a palaneedy

                            Din22[Convert.ToInt32(aux2 - 1), 7] = 0;

                            if (vectores.Fila[palanedy - 1] == 0) //si no hay fila
                            {
                                Din22[Convert.ToInt32(aux2 - 1), 7] = vectores.TCV[Convert.ToInt32(vectores.DesCam[Convert.ToInt32(aux2 - 1), 2] - 1), palanedy - 1]; //el shovel idle rate es Tij                            }
                            }

                            double lost_tones;

                            lost_tones = ((Din22[Convert.ToInt32(aux2 - 1), 1] * Din22[Convert.ToInt32(aux2 - 1), 2] / Din22[Convert.ToInt32(aux2 - 1), 3]) * (Din22[Convert.ToInt32(aux2 - 1), 4] + Din22[Convert.ToInt32(aux2 - 1), 5]) + (Din22[Convert.ToInt32(aux2 - 1), 6] * Din22[Convert.ToInt32(aux2 - 1), 7]));
                            //lost_tones[x] = (Din22[x, 2] / Din22[x, 3]) * (Din22[x, 3] + Din22[x, 4]) + (Din22[x, 5] * Din22[x, 6]);

                            if (lost_tones < min2) //recupera el menor y actualiza el valor
                            {

                                min2 = lost_tones;

                                palapiola = palanedy;

                            }
                        }
                    }

                    double min32 = 0;

                    for (int l = 0; l < 30; l++)
                    {
                        double last_tones = 0;
                        if (vectores.PlTc[l, palapiola - 1] > 0)
                        {
                            last_tones = vectores.PlTc[l, palapiola - 1] / (Din12[(palapiola - 1) * 30 + l, 7] * 300 / timenow);
                            if (last_tones > min32)
                            {

                                min32 = last_tones;
                                Din22[Convert.ToInt32(aux2 - 1), 9] = palapiola;
                                Din22[Convert.ToInt32(aux2 - 1), 10] = l + 1;

                            }
                        }
                        else
                        {
                            if (min2 == 0)
                            {
                                Din22[Convert.ToInt32(aux2 - 1), 9] = 21;
                            }
                        }


                    }
                    y = 0;
                }*/

            }

            while (y == 1);

            //for (int x = 0; x < numcam; x++) //DesCam suelta el dato como importante
            //{
            //    vectores.DesCam[x, 3] = 0;
            //}

            _salida.StateValue = Din22[Convert.ToInt32(aux2 - 1), 9];
            if (Din22[Convert.ToInt32(aux2 - 1), 9] == 4 || Din22[Convert.ToInt32(aux2 - 1), 9] == 5 || Din22[Convert.ToInt32(aux2 - 1), 9] == 6 || Din22[Convert.ToInt32(aux2 - 1), 9] == 10 || Din22[Convert.ToInt32(aux2 - 1), 9] == 17 || Din22[Convert.ToInt32(aux2 - 1), 9] == 18)
            {
                Din22[Convert.ToInt32(aux2 - 1), 10] = 1;
            }

            else if (Din22[Convert.ToInt32(aux2 - 1), 9] == 7 || Din22[Convert.ToInt32(aux2 - 1), 9] == 8  || Din22[Convert.ToInt32(aux2 - 1), 9] == 12)
            {
                Din22[Convert.ToInt32(aux2 - 1), 10] = 4;
            }

            else if (Din22[Convert.ToInt32(aux2 - 1), 9] == 1 || Din22[Convert.ToInt32(aux2 - 1), 9] == 2 || Din22[Convert.ToInt32(aux2 - 1), 9] == 3 || Din22[Convert.ToInt32(aux2 - 1), 9] == 11)
            {
                Din22[Convert.ToInt32(aux2 - 1), 10] = 8;
            }

            _aux.StateValue = Din22[Convert.ToInt32(aux2 - 1), 10];//
            //_aux2.StateValue = camionpiola;
            _needtime.StateValue = min;//
            _losttones.StateValue = ((Din12[33, 0] / Din12[33, 1]) * (Din12[33, 5] * 300 - Din12[33, 1] * timenow)) / (Din12[33, 7] * 300 / timenow);//(Din12[33, 7] * 300 / timenow);//
            _needch.StateValue = ((Din12[37, 0] / Din12[37, 1]) * (Din12[37, 5] * 300 - Din12[37, 1] * timenow)) / (Din12[37, 7] * 300 / timenow);//

            sr.serializa(vectores);
            sr.closeStream();
            return ExitType.FirstExit;

        }

    }

}
#endregion
