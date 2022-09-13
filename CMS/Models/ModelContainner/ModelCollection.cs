using System.Collections.Generic;

namespace CMS.Models.ModelContainner
{
    public class ModelCollection
    {
        private readonly Dictionary<string, object> models = new Dictionary<string, object>();

        public void AddModel<T>(string key, T t)
        {
            models.Add(key, t);
        }

        public T GetModel<T>(string key)
        {
            return (T)models[key];
        }
    }
}
