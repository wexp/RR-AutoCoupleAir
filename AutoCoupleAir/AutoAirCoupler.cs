using HarmonyLib;
using Railloader;
using UI.Builder;

namespace AutoCoupleAir
{
    internal class Settings
    {
        public bool isEnabled = true;
        public bool affectHandbrakes = true;
        public bool affectOnlyAi = false;
        public Settings()
        {
        }
    }

    public class AutoAirCoupler : PluginBase, IModTabHandler
    {
        private readonly IModDefinition self;
        private readonly IModdingContext moddingContext;
        internal static Settings Settings;
        public AutoAirCoupler(IModDefinition self, IModdingContext moddingContext)
        {
            AutoAirCoupler.Settings = moddingContext.LoadSettingsData<Settings>(self.Id) ?? new Settings();
            this.self = self;
            this.moddingContext = moddingContext;
            new Harmony("wexp.AutoCoupleAir").PatchAll(GetType().Assembly);
        }

        public void ModTabDidClose()
        {
            this.moddingContext.SaveSettingsData<Settings>(this.self.Id, AutoAirCoupler.Settings);
        }

        public void ModTabDidOpen(UIPanelBuilder builder)
        {
            builder.AddLabel("Auto attach hoses & open anglecocks on couple");
            builder.AddField("Enabled ", builder.AddToggle(() => AutoAirCoupler.Settings.isEnabled, delegate (bool q)
                {
                    AutoAirCoupler.Settings.isEnabled = q;
                }));
            builder.AddField("Affect only yard AI ", builder.AddToggle(() => AutoAirCoupler.Settings.affectOnlyAi, delegate (bool q)
            {
                AutoAirCoupler.Settings.affectOnlyAi = q;
            }));
            builder.AddField("Turn off handbrakes:", builder.AddToggle(() => AutoAirCoupler.Settings.affectHandbrakes, delegate (bool q)
            {
                AutoAirCoupler.Settings.affectHandbrakes = q;
            }));
            
        }
    }
}
