using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace JudgeClient.Definition
{
    public class Factory
    {
        public static T CreateAndConfigure<T>(IProfile Profile)
        {
            try
            {
                IModule res = Activator.CreateInstance(System.Type.GetType(Profile.Type)) as IModule;
                if (res == null)
                    ExceptionManager.Throw(new CreateAndConfigureModuleException("Factory: Could not create " + Profile.Type, null));
                res.Configure(Profile);
                return (T)(res as object);
            }
            catch (Exception ex)
            {
                ExceptionManager.Throw(new CreateAndConfigureModuleException("Factory: Could not create or configure or cast " + Profile.Type, ex));
                return default(T);
            }
        }
    }
}
