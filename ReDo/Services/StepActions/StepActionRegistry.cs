using System.Collections.Generic;
using System.Linq;

namespace ReDo.Services.StepActions
{
    /// <summary>Central registry of add-step actions. Register providers to extend the menu.</summary>
    public static class StepActionRegistry
    {
        private static readonly List<IStepActionProvider> Providers = new List<IStepActionProvider>();
        private static bool _initialized;

        public static IReadOnlyList<IStepActionProvider> All
        {
            get
            {
                EnsureInitialized();
                return Providers.AsReadOnly();
            }
        }

        public static void Register(IStepActionProvider provider)
        {
            if (provider == null) return;
            EnsureInitialized();
            if (Providers.Any(p => p.Id == provider.Id))
                return;
            Providers.Add(provider);
        }

        public static IStepActionProvider Get(string id)
        {
            EnsureInitialized();
            return Providers.FirstOrDefault(p => p.Id == id);
        }

        private static void EnsureInitialized()
        {
            if (_initialized) return;
            _initialized = true;
            BuiltInStepActions.RegisterAll();
        }
    }
}
