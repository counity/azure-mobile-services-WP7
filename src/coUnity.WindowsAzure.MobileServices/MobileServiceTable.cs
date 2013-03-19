using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using ServiceStack.Text;
using coUnity.WindowsAzure.MobileServices.Util;

namespace coUnity.WindowsAzure.MobileServices
{
    public class MobileServiceTable
    {
        /// <summary>
        /// Name of the reserved Mobile Services ID member.
        /// </summary>
        /// <remarks>
        /// Note: This value is used by other areas like serialiation to find
        /// the name of the reserved ID member.
        /// </remarks>
        internal const string IdPropertyName = "id";

        /// <summary>
        /// The route separator used to denote the table in a uri like
        /// .../{app}/tables/{coll}.
        /// </summary>
        internal const string TableRouteSeperatorName = "tables";

        public MobileServiceClient ServiceClient { get; protected set; }

        public string TableName { get; protected set; }
        /// <summary>
        /// Get a uri fragment representing the resource corresponding to the
        /// table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>A URI fragment representing the resource.</returns>
        protected static string GetUriFragment(string tableName)
        {
            return Path.Combine(MobileServiceTable.TableRouteSeperatorName, tableName);
        }

        /// <summary>
        /// Get a uri fragment representing the resource corresponding to the
        /// given id in the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="id">The id of the instance.</param>
        /// <returns>A URI fragment representing the resource.</returns>
        protected static string GetUriFragment(string tableName, object id)
        {
            string uriFragment = GetUriFragment(tableName);
            return Path.Combine(uriFragment, id.ToString());
        }
    }

    public class MobileServiceTable<T> : MobileServiceTable, IMobileServiceTable<T> where T : class
    {
        public MobileServiceTable(MobileServiceClient client)
        {
            ServiceClient = client;
            TableName = (typeof (T)).GetTableName();
        }

        public T Get(object id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            string uriFragment = GetUriFragment(TableName, id);

            try
            {
                var jsonResult = ServiceClient.PerformRequest("GET", uriFragment, null);
                return jsonResult.FromJson<T>();
            }
            catch (WebException e)
            {
                if (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.NotFound)
                    return null;

                throw e;
            }
        }

        public IEnumerable<T> Read(string query)
        {
            string uriFragment = GetUriFragment(this.TableName);
            if (!string.IsNullOrEmpty(query))
                uriFragment += '?' + query.TrimStart('?');

            var jsonResult = ServiceClient.PerformRequest("GET", uriFragment, null);
            var retVal = jsonResult.FromJson<List<T>>();
            return retVal;
        }

        public void Insert(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException();

            //check if id is set
            if (instance.GetIdValue() != null)
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Cannot insert if the {0} member is already set.",
                        MobileServiceTable.IdPropertyName),
                    "instance");

            string uriFragment = GetUriFragment(TableName);
            var jsonResult = ServiceClient.PerformRequest("POST", uriFragment, instance.ToJson(true));
            var result = jsonResult.FromJson<T>();
            instance.Patch(result);
        }

        public void Update(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException();

            string uriFragment = GetUriFragment(TableName, instance.GetIdValue());
            var jsonResult = ServiceClient.PerformRequest("PATCH", uriFragment, instance.ToJson());
            var result = jsonResult.FromJson<T>();
            instance.Patch(result);
        }

        public void Delete(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException();

            string uriFragment = GetUriFragment(TableName, instance.GetIdValue());
            ServiceClient.PerformRequest("DELETE", uriFragment, instance.ToJson());
        }
    }
}
