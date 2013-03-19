using System.Collections.Generic;

namespace coUnity.WindowsAzure.MobileServices
{
    public interface IMobileServiceTable<T> where T : class
    {
        /// <summary>
        /// Executes a query against the table
        /// </summary>
        /// <param name="id">The query to be executed. See http://msdn.microsoft.com/en-us/library/windowsazure/jj677199.aspx 
        /// and http://www.odata.org/documentation/uri-conventions#SystemQueryOptions for more details on how to query data.</param>
        /// <returns></returns>
        IEnumerable<T> Read(string query);
        /// <summary>
        /// Gets a single object from the table
        /// </summary>
        /// <param name="id">Id of the object to be returned</param>
        /// <returns></returns>
        T Get(object id);
        /// <summary>
        /// Inserts a new object into the table. The provided instance is updated with the id
        /// </summary>
        /// <param name="instance">The object to be stored</param>
        void Insert(T instance);
        /// <summary>
        /// Updates an existing object in the table.
        /// </summary>
        /// <param name="instance">The object to be updated</param>
        void Update(T instance);
        /// <summary>
        /// Deletes an existing object from the table
        /// </summary>
        /// <param name="instance">The object to be deleted</param>
        void Delete(T instance);
    }
}
