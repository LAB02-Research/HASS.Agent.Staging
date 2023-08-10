using HASS.Agent.API;
using HASS.Agent.Functions;
using HASS.Agent.Managers;
using HASS.Agent.Properties;
using HASS.Agent.Resources.Localization;
using HASS.Agent.Sensors;
using HASS.Agent.Settings;
using Serilog;
using Syncfusion.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace HASS.Agent.Forms.ChildApplications
{
    public partial class CompatibilityTask : MetroForm
    {
        public CompatibilityTask()
        {
            InitializeComponent();
        }

        private void CompatibilityTask_Load(object sender, EventArgs e) => ProcessCompatibilityTask();

        /// <summary>
        /// Performs compatibility task
        /// </summary>
        private async void ProcessCompatibilityTask()
        {
            // set busy indicator
            PbStep1CompatTask.Image = Properties.Resources.small_loader_32;

            await Task.Delay(TimeSpan.FromSeconds(2));


            var taskDone = await NameComaptibilityTaskAsync();
            PbStep1CompatTask.Image = taskDone ? Properties.Resources.done_32 : Properties.Resources.failed_32;

            if (!taskDone)
            {
                MessageBoxAdv.Show(this, Languages.PortReservation_ProcessPostUpdate_MessageBox1, Variables.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // wait a bit to show the 'completed' check
                await Task.Delay(750);
            }

            Log.CloseAndFlush();

            Environment.Exit(taskDone ? 0 : -1);
        }

        /// <summary>
        /// Processes the sensor name compatibility task
        /// </summary>
        /// <returns></returns>
        public async Task<bool> NameComaptibilityTaskAsync()
        {
            try
            {
                Log.Information("[COMPATTASK] Sensor name compatibility task started");

                Log.Information("[COMPATTASK] Loading stored sensors");
                var loaded = await StoredSensors.LoadAsync();
                if (!loaded)
                {
                    Log.Error("[COMPATTASK] Error loading sensors");

                    return false;
                }

                var sensors = Variables.SingleValueSensors;
                foreach(var sensor in sensors)
                {
                    sensor.Name = sensor.Name.Replace("","");
                }

                Log.Information("[COMPATTASK] Storing modified sensors");
                var saved = StoredSensors.Store();
                if (!saved)
                {
                    Log.Error("[COMPATTASK] Error saving sensors");

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[COMPATTASK] Error performing sensor name compatibility task: {err}", ex.Message);
                return false;
            }
        }

        private void CompatibilityTask_ResizeEnd(object sender, EventArgs e)
        {
            if (Variables.ShuttingDown) return;
            if (!IsHandleCreated) return;
            if (IsDisposed) return;

            try
            {
                Refresh();
            }
            catch
            {
                // best effort
            }
        }
    }
}
