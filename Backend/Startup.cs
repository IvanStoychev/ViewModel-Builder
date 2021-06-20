using Backend.Options;
using Microsoft.Extensions.Configuration;

namespace Backend
{
    /// <summary>
    /// Handles operations and tasks that need to be performed immediately at the
    /// start of the program, before anything else has initialized.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Reads the configuration files, loads their data in application memory, sets any default settings and sets up shutdown hooks.
        /// </summary>
        public static void InitConfig()
        {
            var typeData = TypeData.instance;
            var mergeFieldCatalog = MergeFieldCatalog.instance;
            var templateCatalog = TemplateCatalog.instance;

            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();
            configuration.GetSection(nameof(TypeData)).Bind(typeData);
            configuration.GetSection(nameof(MergeFieldCatalog)).Bind(mergeFieldCatalog);
            configuration.GetSection(nameof(TemplateCatalog)).Bind(templateCatalog);

            TypeData.ReplaceICommandType();
        }
    }
}
