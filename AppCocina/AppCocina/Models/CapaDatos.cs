using AppCocina.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AppCocina.Models
{
    public class CapaDatos : IDisposable
    {
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
        public async Task<DataTable> getIngredientes()
        {
            try
            {
                DataTable dt = new DataTable();
                var qry = "select * from Ingredientes";
                Dictionary<string, string> pars = new Dictionary<string, string>();
//pars.Add("@StatId", statid);
                using (var db = new SQLServer())
                {
                    dt = await db.getQueryResultAsync(qry, pars, "text");
                }
                return dt;
            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<DataTable> getPlatillos()
        {
            try
            {
                DataTable dt = new DataTable();
                var qry = "select * from Platillos";
                Dictionary<string, string> pars = new Dictionary<string, string>();
                //pars.Add("@StatId", statid);
                using (var db = new SQLServer())
                {
                    dt = await db.getQueryResultAsync(qry, pars, "text");
                }
                return dt;
            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<bool> updateStock(string ingredientes)
        {
            try
            {
                DataTable dt = new DataTable();
                string[] idIngredientes = ingredientes.Split(',');
                foreach (string idIngrediente in idIngredientes)
                {
                    var qry = "update Ingredientes set cantidad = cantidad - 1 where idIngrediente = @idIngrediente";
                    Dictionary<string, string> pars = new Dictionary<string, string>();
                    pars.Add("@idIngrediente", idIngrediente);
                    using (var db = new SQLServer())
                    {
                        dt = await db.getQueryResultAsync(qry, pars, "text");
                    }
                }                
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }


    }
}