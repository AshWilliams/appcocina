using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AppCocina.Utilidades
{
    public class SQLServer : IDisposable
    {
        private SqlConnection myConnection = null;
        private string myConnectionString;
        private SqlCommand myCommand = null;
        private bool status = false;       

        #region "Dispose"
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.ReRegisterForFinalize(this);
            }

        }

        /// <summary>
        /// Realiza tareas definidas por la aplicación asociadas a la liberación o al restablecimiento de recursos no administrados.
        /// </summary>
        public void Dispose()
        {
            //  Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        #endregion

        /// <summary>
        /// Obtengo la cadena de conexion a la base de datos o bien la seteo.
        /// </summary>   
        /// <author>Robert Rozas Navarro</author>
        public string getConn
        {
            get
            {
                return myConnectionString;
            }
            set
            {
                myConnectionString = value;
            }
        }

        /// <summary>
        /// Constructor de la instalacia de la Clase MySQL.
        /// </summary>
        /// <author>Robert Rozas Navarro</author>
        public SQLServer()
        {
            try
            {
                string conexion = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]; ;
                this.getConn = conexion;
                this.myConnection = new SqlConnection();
                this.myConnection.ConnectionString = this.getConn;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en el constructor MySQL: " + ex.Message);
                throw;
            }
        }

        public SqlConnection getConnection()
        {
            return this.myConnection;
        }

        /// <summary>
        /// Abre la conexion a la BD.
        /// </summary>
        /// <author>Robert Rozas Navarro</author>
        public void Open()
        {
            try
            {
                if (!this.status)
                {
                    this.myConnection.Open();
                    this.status = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en el void Open: " + ex.Message);
                throw;
            }

        }

        /// <summary>
        /// Cierra la conexion con la BD
        /// </summary>
        /// <author>Robert Rozas Navarro</author>
        public void Close()
        {
            try
            {
                if (this.status)
                {
                    this.myConnection.Close();
                    this.status = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en el void Close: " + ex.Message);
                throw;
            }

        }

        /// <summary>
        /// Creo comando Mysql custom.
        /// </summary>
        /// <param name="qry">Consulta Sql</param>
        /// <returnsMysqlCommand></returns>
        /// <author>Robert Rozas Navarro</author>
        public SqlCommand Command(string qry)
        {
            try
            {
                if (this.status)
                {
                    this.myCommand = new SqlCommand(qry, this.myConnection);
                }
                return this.myCommand;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en el método Command: " + ex.Message);
                throw;
            }

        }


        /// <summary>
        /// Obtengo resultado dinámico de una consulta en base a n parámetros
        /// </summary>
        /// <param name="qry">Consulta</param>
        /// <param name="parametros">Los Parámetros</param>
        /// <returns>DataTable con resultados</returns>
        /// <author>Robert Rozas Navarro</author>
        public DataTable getQueryResult(string qry, Dictionary<string, string> parametros, string tipo = "SP")
        {
            try
            {
                DataTable dt = new DataTable();
                SQLServer db = null;
                SqlCommand myComando = null;
                SqlDataAdapter myAdapter = new SqlDataAdapter();

                using (db = new SQLServer())
                {
                    db.Open();

                    using (myComando = db.Command(qry))
                    {
                        myComando.CommandType = tipo == "SP" ? CommandType.StoredProcedure : CommandType.Text;
                        foreach (var item in parametros)
                        {
                            int entero;
                            bool res = int.TryParse(item.Value, out entero);
                            if (res)
                            {
                                myComando.Parameters.AddWithValue(item.Key, entero);
                            }
                            else
                            {
                                myComando.Parameters.AddWithValue(item.Key, item.Value);
                            }
                        }
                        myAdapter.SelectCommand = myComando;
                        myAdapter.Fill(dt);
                    }

                }

                return dt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en getResult: " + ex.Message);
                throw;
            }
        }


        public async Task<DataTable> getQueryResultAsync(string qry, Dictionary<string, string> parametros, string tipo = "SP")
        {
            try
            {
                DataTable dt = new DataTable();
                SQLServer db = null;
                SqlCommand myComando = null;
                SqlDataAdapter myAdapter = new SqlDataAdapter();
                SqlDataReader myReader;
                using (db = new SQLServer())
                {
                    db.Open();

                    using (myComando = db.Command(qry))
                    {
                        myComando.CommandType = tipo == "SP" ? CommandType.StoredProcedure : CommandType.Text;
                        foreach (var item in parametros)
                        {
                            int entero;
                            bool res = int.TryParse(item.Value, out entero);
                            if (res)
                            {
                                myComando.Parameters.AddWithValue(item.Key, entero);
                            }
                            else
                            {
                                myComando.Parameters.AddWithValue(item.Key, item.Value);
                            }
                        }
                        myReader = await myComando.ExecuteReaderAsync();
                        dt.Load(myReader);
                        //myAdapter.SelectCommand = myComando;                       
                        //myAdapter.Fill(dt);
                    }

                }

                return dt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en getResult: " + ex.Message);
                throw;
            }
        }
    }
}