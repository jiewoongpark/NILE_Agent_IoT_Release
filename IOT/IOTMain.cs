using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using IOT.Properties;

namespace IOT
{
    
    [SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
    [SuppressMessage("ReSharper", "ConvertToConstant.Local")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    [SuppressMessage("ReSharper", "BuiltInTypeReferenceStyle")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
    [SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "RedundantExplicitParamsArrayCreation")]
    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    [SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
    [SuppressMessage("ReSharper", "RedundantDelegateCreation")]
    [SuppressMessage("ReSharper", "RedundantNameQualifier")]
    [SuppressMessage("ReSharper", "RedundantCast")]
    [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
    [SuppressMessage("ReSharper", "InvertIf")]
    [SuppressMessage("ReSharper", "NotAccessedVariable")]
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "UnusedVariable")]
    [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
    [SuppressMessage("ReSharper", "RedundantBoolCompare")]
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
    
    
    public partial class frmIOTMain : Form
    {
        
        public frmIOTMain()
        {
            InitializeComponent();
        }

        public enum ConnectionStatus
        {
            Ready,
            Connected,
            Disconnected
        }

        private void UpdateConnectionStatusUI(ConnectionStatus status)
        {
            Color statusColor;
            string statusText;

            switch (status)
            {
                case ConnectionStatus.Ready:
                    statusColor = Color.Orange;
                    statusText = "Ready";
                    break;
                case ConnectionStatus.Connected:
                    statusColor = Color.Green;
                    statusText = "Connected";
                    break;
                case ConnectionStatus.Disconnected:
                    statusColor = Color.Red;
                    statusText = "Disconnected";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), Resources.UnknownConnectionStatus);
            }

            // Update connection indicator label
            lbConnectionIndicator.Text = statusText;

            // Update the connection status circle
            PaintConnectionStatusCircle(statusColor);

            // Optionally log the status change
            appendLog($"Status: {statusText}");
        }

        private void PaintConnectionStatusCircle(Color color)
        {
            var graphics = this.CreateGraphics();
            int x = 10;
            int y = 435;
            int diameter = 15;
            using (Brush brush = new SolidBrush(color))
            {
                graphics.FillEllipse(brush, x, y, diameter, diameter);
            }
        }

        
        private readonly string masterPasswd = "hsnc123!";
        private string IOT_LogPath =  @"C:\NILE_Agent_logs\";
        private string IOT_LogFileName = "AGENT_DATE.log";
        private uint log_line_cnt = 0;
        private uint MAX_LOG_LINES = 100;

        private bool defaultCleanSession = false;
        private int defaultTimeout = 120;
        private int defaultMqttPort = 1883;
        
        private bool logAppendedForMqtt = false;
        private bool logAppendedForCoap = false;
        private bool logAppendedForRedis = false;
        private bool logAppendedForInflux = false;
        private bool logAppendedForTimeScale = false;

        private Timer _coapTimer;

        private int _lastLoggedDay = DateTime.Now.Day;
        private Dictionary<string, clsMQTTBrokerProtocol> mqttConnections = new Dictionary<string, clsMQTTBrokerProtocol>();
        private ClsCoApProtocol coapProtocol;

        
        
        
        
        /*TextBox*/
        private string getText(TextBox txtb )
        {
            return txtb.Text;
        }

        private delegate void SafeCallDelegate_Text(TextBox txt, String strContent);

        private void setText(TextBox txtb, string strContent)
        {
            if (txtLog.InvokeRequired)
            {
                var d = new SafeCallDelegate_Text(setText);
                txtb.Invoke(d, new object[] { txtb, strContent });
            }
            else
            {
                txtb.Text = strContent;
            }
        
        }

        /*Label*/
        private string getText(Label lbl)
        {
            return lbl.Text;
        }

        private delegate void SafeCallDelegate_lblText(Label lbl, String strContent);
        public void setText(Label lbl, string strContent)
        {
            if (txtLog.InvokeRequired)
            {
                var d = new SafeCallDelegate_lblText(setText);
                lbl.Invoke(d, new object[] { lbl, strContent });
            }
            else
            {
                lbl.Text = strContent;
            }

        }

        /*CheckBox*/
        public bool getValue(CheckBox ChkBx)
        {
            return ChkBx.Checked;
        }

        private delegate void SafeCallDelegate_cValue(CheckBox ChkBx, Boolean chk);
        public void setValue(CheckBox ChkBx, Boolean chk)
        {
            if (ChkBx.InvokeRequired)
            {
                var d = new SafeCallDelegate_cValue(setValue);
                ChkBx.Invoke(d, new object[] { ChkBx, chk });
            }
            else
            {
                ChkBx.Checked = chk;
            }
        }

        
        
        
        
        private void StartSecTimer()
        {
            this.txtLogPath.Text = IOT_LogPath;
            System.Windows.Forms.Timer secTimer = new System.Windows.Forms.Timer();
            secTimer.Tick += new EventHandler(PerSecEvent);
            secTimer.Interval = (int)1000;
            secTimer.Start();
        }
        
        private void PerSecEvent(Object myObject, EventArgs myEventArgs)
        {
            DateTime currentTime = DateTime.Now;
            setText(lbCurrentTime, currentTime.ToString());

            if (currentTime.Day != _lastLoggedDay)
            {
                IOT_LogFileName = $"NILE_Agent_{currentTime:yyyy_MM_dd}.log";
                _lastLoggedDay = currentTime.Day;
            }
        }

        public static class PromptPW
        {
            public static string ShowPWDialog()
            {
                Form prompt = new Form()
                {
                    Width = 400,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = Resources.PasswordPrompt,
                    StartPosition = FormStartPosition.CenterScreen
                };
                Label textLabel = new Label() { Left = 50, Top = 20, Text = Resources.PasswordPrompt };
                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 200, PasswordChar = '*' };
                Button confirmation = new Button() { Text = Resources.ApplyText, Left = 300, Width = 50, Top = 50, DialogResult = DialogResult.OK };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            }
        }

        private void load_all_default_params()
        {
            txtFilePath.Text = filePathSaveCheckBox.Checked ? Properties.Settings.Default.FILE_PATH : "";
            txtLogPath.Text = LogSaveCheckBox.Checked ? Properties.Settings.Default.LOG_PATH : "";
            filePathSaveCheckBox.Checked = Properties.Settings.Default.SAVE_FILE_PATH;
            LogSaveCheckBox.Checked = Properties.Settings.Default.SAVE_LOG_PATH;
        }

        private void frmAgentMain_Load(object sender, EventArgs e)
        {
            appendLog("System Loading...");
            IOT_LogFileName = $"NILE_Agent_{DateTime.Now:yyyy_MM_dd}.log";
            load_all_default_params();
            StartSecTimer();
            appendLog("System Loading Complete");
            UpdateConnectionStatusUI(ConnectionStatus.Ready);
            this.Invalidate();
        }

        private void toggleUIEnabled(bool enabled)
        {
            //LogGroupBox.Enabled = enabled;
            txtFilePath.Enabled = enabled;
            txtLogPath.Enabled = enabled;
            btnFilePath.Enabled = enabled;
            filePathSaveCheckBox.Enabled = enabled;
            LogSaveCheckBox.Enabled = enabled;
            CoapTimer.Enabled = enabled;
        }

        private async void SetupCoAP(string coapUrl, string coapResourcePath)
        {
            coapProtocol = new ClsCoApProtocol(coapUrl);
            coapProtocol.MessageReceived += CoapProtocol_MessageReceived;
            await coapProtocol.RetrieveMessageAsync(coapResourcePath);
        }

        private void CoapProtocol_MessageReceived(string resourcePath, string message)
        {
            Invoke((MethodInvoker)delegate
            {
                appendLog($"[CoAP] {resourcePath}: {message}");
            });
        }
        
        private void StopAndDisposeCoapTimer()
        {
            if (_coapTimer != null)
            {
                _coapTimer.Stop();
                _coapTimer.Dispose();
                _coapTimer = null;
            }
        }
        
        private async void btnSubscribe_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text))
            {
                MessageBox.Show(Resources.SelectConfigFile, Resources.ErrorText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } 

            string inputPassword = PromptPW.ShowPWDialog();
            if (inputPassword != masterPasswd)
            {
                MessageBox.Show(Resources.IncorrectPasswordText, Resources.ErrorText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (btnSubscribe.Text == Resources.SubscribeText)
            {
                UpdateConnectionStatusUI(ConnectionStatus.Ready);
                string configFileContent = File.ReadAllText(txtFilePath.Text);
                appendLog(configFileContent);
                var configurations = JsonConvert.DeserializeObject<List<RtdbConfiguration>>(configFileContent);
                if (configurations == null || configurations.Count == 0)
                {
                    MessageBox.Show(Resources.InvalidConfigFileOrEmpty, Resources.ErrorText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                await ProcessConfigurations(configurations);
                if (logAppendedForMqtt)
                {
                    appendLog("MQTT Connected Successfully");
                }
                
                if (logAppendedForCoap)
                {
                    appendLog("CoAP Connected Successfully");
                }
                
                if (logAppendedForRedis)
                {
                    appendLog("Redis Connected Successfully");
                }
                
                if (logAppendedForInflux)
                {
                    appendLog("Influx Connected Successfully");
                }
                
                if (logAppendedForTimeScale)
                {
                    appendLog("TimeScale Connected Successfully");
                }
                
                btnSubscribe.Text = Resources.UnsubscribeText;
                UpdateConnectionStatusUI(ConnectionStatus.Connected);
                toggleUIEnabled(false);
            }
            else
            {
                mqttConnections.Clear();
                StopAndDisposeCoapTimer();
                btnSubscribe.Text = Resources.SubscribeText;
                toggleUIEnabled(true);
                UpdateConnectionStatusUI(ConnectionStatus.Disconnected);
            }
        }

        
        
        
        
        private async Task ProcessConfigurations(List<RtdbConfiguration> configurations)
        {
            appendLog("Job Received. Processing ...");
            logAppendedForMqtt = logAppendedForRedis = logAppendedForInflux = logAppendedForTimeScale = logAppendedForCoap = false;

            foreach (var config in configurations)
            {
                switch (config.RtdbType.ToLower())
                {

                    case "redis":
                        var redisConfig = JsonConvert.DeserializeObject<Redis>(config.RtdbConnectionInfo.ToString());

                        bool isClusterMode = redisConfig.ClusterMode;
                        bool isSingleNodeInsert = redisConfig.SingleNodeInsert;

                        if (isClusterMode && isSingleNodeInsert)
                        {
                            await ProcessRedisClusterSingleInsertConfiguration(config, redisConfig);
                        }
                        else if (isClusterMode && !isSingleNodeInsert)
                        {
                            await ProcessRedisClusterMultipleInsertConfiguration(config, redisConfig);
                        }
                        else
                        {
                            await ProcessRedisConfiguration(config, redisConfig);

                        }
                        break;
                    case "influxdb":
                        var influxConfig = JsonConvert.DeserializeObject<InfluxDB>(config.RtdbConnectionInfo.ToString());
                        await ProcessInfluxDBConfiguration(config, influxConfig);
                        break;
                    case "timescaledb":
                        var timescaleConfig = JsonConvert.DeserializeObject<TimeScaleDB>(config.RtdbConnectionInfo.ToString());
                        await ProcessTimeScaleDBConfiguration(config, timescaleConfig);
                        break;
                }
            }
        }

        private async Task ProcessRedisClusterSingleInsertConfiguration(RtdbConfiguration config, Redis redisConfig)
        {
            if (!string.IsNullOrEmpty(config.MqttUrl) && string.IsNullOrEmpty(config.CoapIp))
            {
                if (config.RtdbType.ToLower().Equals("redis"))
                {
                    var mqttProtocol = new clsMQTTBrokerProtocol(
                        config.MqttUrl,
                        config.MqttPort ?? defaultMqttPort,
                        config.MqttClientId,
                        config.MqttUsername,
                        config.MqttPassword,
                        config.MqttCleanSession ?? defaultCleanSession,
                        config.MqttTimeout ?? defaultTimeout);
                    await mqttProtocol.ConnectAndSubscribeAsync(config.MqttTopic);

                    mqttProtocol.MessageReceived += async (topic, message) =>
                    {
                        OperationResult initResult = new OperationResult(true, "Redis client initialized.");
                        if (!ClsRtdbRedisProtocol.RedisIsInitialized)
                        { 
                            initResult = ClsRtdbRedisProtocol.Initialize(redisConfig.ConnectionString, redisConfig.Database);
                        }
                        
                        var redisProtocol = new ClsRtdbRedisProtocol();
                        var appendResult = await redisProtocol.AppendDataToListAsync(topic, message);
                        
                    };
                    
                    logAppendedForRedis = true;
                    logAppendedForMqtt = true;

                }
            }
            else if (string.IsNullOrEmpty(config.MqttUrl) && !string.IsNullOrEmpty(config.CoapIp))
            {
                if (config.RtdbType.ToLower().Equals("redis"))
                {
                    string coapUrl = $"coap://{config.CoapIp}:{config.CoapPort}/{config.CoapTopic}";

                    SetupCoAP(coapUrl, config.CoapTopic);

                    int intervalInSeconds = Convert.ToInt32(CoapTimer.Value);
                    _coapTimer?.Dispose();
                    _coapTimer = new Timer
                    {
                        Interval = intervalInSeconds * 1000
                    };
                    
                    coapProtocol.MessageReceived += (topic, message) =>
                    {
                        _coapTimer.Tick += async (sender, e) =>
                        {
                            OperationResult initResult = new OperationResult(true, "Redis client initialized.");
                            if (!ClsRtdbRedisProtocol.RedisIsInitialized)
                            { 
                                initResult = ClsRtdbRedisProtocol.Initialize(redisConfig.ConnectionString, redisConfig.Database);
                            }
                       
                            if (!initResult.RedisSuccess)
                            {
                                appendLog(initResult.RedisMessage);
                            }
                            else
                            {
                                var redisProtocol = new ClsRtdbRedisProtocol();
                                var appendResult = await redisProtocol.AppendDataToListAsync(topic, message);
                                appendLog(!appendResult.RedisSuccess
                                    ? appendResult.RedisMessage
                                    : "[CoAP] messages sent to [REDIS]");
                            }
                          
                        };
                    };
                    logAppendedForRedis = true;
                    logAppendedForCoap = true;
                    _coapTimer.Start();
                }
            }
            else
            {
                appendLog("MQTT/CoAP Message is Empty. Please check your configuration.");
            }

        }
        private async Task ProcessRedisClusterMultipleInsertConfiguration(RtdbConfiguration config, Redis redisConfig)
        {
            if (!string.IsNullOrEmpty(config.MqttUrl) && string.IsNullOrEmpty(config.CoapIp))
            {
                if (config.RtdbType.ToLower().Equals("redis"))
                {
                    var mqttProtocol = new clsMQTTBrokerProtocol(
                        config.MqttUrl,
                        config.MqttPort ?? defaultMqttPort,
                        config.MqttClientId,
                        config.MqttUsername,
                        config.MqttPassword,
                        config.MqttCleanSession ?? defaultCleanSession,
                        config.MqttTimeout ?? defaultTimeout);
                    await mqttProtocol.ConnectAndSubscribeAsync(config.MqttTopic);

                    mqttProtocol.MessageReceived += async (topic, message) =>
                    {
                        OperationResult initResult = new OperationResult(true, "Redis client initialized.");
                        if (!ClsRtdbRedisProtocol.RedisIsInitializedManyNodes)
                        { 
                            initResult = ClsRtdbRedisProtocol.InitializeManyNodes(redisConfig.ConnectionStringManyNodes, redisConfig.Database);
                        }
                        
                        var redisProtocol = new ClsRtdbRedisProtocol();
                        var appendResult = await redisProtocol.AppendDataToManyNodesListAsync(topic, message);
                        
                    };
                    
                    logAppendedForRedis = true;
                    logAppendedForMqtt = true;

                }
            }
            else if (string.IsNullOrEmpty(config.MqttUrl) && !string.IsNullOrEmpty(config.CoapIp))
            {
                if (config.RtdbType.ToLower().Equals("redis"))
                {
                    string coapUrl = $"coap://{config.CoapIp}:{config.CoapPort}/{config.CoapTopic}";

                    SetupCoAP(coapUrl, config.CoapTopic);

                    int intervalInSeconds = Convert.ToInt32(CoapTimer.Value);
                    _coapTimer?.Dispose();
                    _coapTimer = new Timer
                    {
                        Interval = intervalInSeconds * 1000
                    };
                    
                    coapProtocol.MessageReceived += (topic, message) =>
                    {
                        _coapTimer.Tick += async (sender, e) =>
                        {
                            OperationResult initResult = new OperationResult(true, "Redis client initialized.");
                            if (!ClsRtdbRedisProtocol.RedisIsInitializedManyNodes)
                            { 
                                initResult = ClsRtdbRedisProtocol.InitializeManyNodes(redisConfig.ConnectionStringManyNodes, redisConfig.Database);
                            }
                       
                            if (!initResult.RedisSuccess)
                            {
                                appendLog(initResult.RedisMessage);
                            }
                            else
                            {
                                var redisProtocol = new ClsRtdbRedisProtocol();
                                var appendResult = await redisProtocol.AppendDataToManyNodesListAsync(topic, message);
                                appendLog(!appendResult.RedisSuccess
                                    ? appendResult.RedisMessage
                                    : "[CoAP] messages sent to [REDIS]");
                            }
                          
                        };
                    };
                    logAppendedForRedis = true;
                    logAppendedForCoap = true;
                    _coapTimer.Start();
                }
            }
            else
            {
                appendLog("MQTT/CoAP Message is Empty. Please check your configuration.");
            }

        }
        private async Task ProcessRedisConfiguration(RtdbConfiguration config, Redis redisConfig)
        {
            
            if (!string.IsNullOrEmpty(config.MqttUrl) && string.IsNullOrEmpty(config.CoapIp))
            {
                if (config.RtdbType.ToLower().Equals("redis"))
                {
                    var mqttProtocol = new clsMQTTBrokerProtocol(
                        config.MqttUrl,
                        config.MqttPort ?? defaultMqttPort,
                        config.MqttClientId,
                        config.MqttUsername,
                        config.MqttPassword,
                        config.MqttCleanSession ?? defaultCleanSession,
                        config.MqttTimeout ?? defaultTimeout);
                    await mqttProtocol.ConnectAndSubscribeAsync(config.MqttTopic);

                    mqttProtocol.MessageReceived += async (topic, message) =>
                    {
                        var redisProtocol = new ClsRtdbRedisStandaloneProtocol(redisConfig.ConnectionString, redisConfig.Database);
                        await redisProtocol.AppendDataToListAsync(topic, message);
                    };
                    
                    logAppendedForRedis = true;
                    logAppendedForMqtt = true;

                }
            }
            else if (string.IsNullOrEmpty(config.MqttUrl) && !string.IsNullOrEmpty(config.CoapIp))
            {
                if (config.RtdbType.ToLower().Equals("redis"))
                {
                    string coapUrl = $"coap://{config.CoapIp}:{config.CoapPort}/{config.CoapTopic}";

                    SetupCoAP(coapUrl, config.CoapTopic);

                    int intervalInSeconds = Convert.ToInt32(CoapTimer.Value);
                    _coapTimer?.Dispose();
                    _coapTimer = new Timer
                    {
                        Interval = intervalInSeconds * 1000
                    };
                    
                    coapProtocol.MessageReceived += (topic, message) =>
                    {
                        _coapTimer.Tick += async (sender, e) =>
                        {
                            var redisProtocol = new ClsRtdbRedisStandaloneProtocol(redisConfig.ConnectionString, redisConfig.Database);
                            await redisProtocol.AppendDataToListAsync(topic, message);
                            appendLog($"[CoAP] messages sent to [Redis]");
                        };
                    };
                    logAppendedForRedis = true;
                    logAppendedForCoap = true;
                    _coapTimer.Start();
                }
            }
            else
            {
                appendLog("MQTT/CoAP Message is Empty. Please check your configuration.");
            }

        }

        
        private async Task ProcessInfluxDBConfiguration(RtdbConfiguration config, InfluxDB influxConfig)
        {
            if (!string.IsNullOrEmpty(config.MqttUrl) && string.IsNullOrEmpty(config.CoapIp))
            {
                if (config.RtdbType.ToLower().Equals("influxdb"))
                {
                    var mqttProtocol = new clsMQTTBrokerProtocol(
                        config.MqttUrl,
                        config.MqttPort ?? defaultMqttPort,
                        config.MqttClientId,
                        config.MqttUsername,
                        config.MqttPassword,
                        config.MqttCleanSession ?? defaultCleanSession,
                        config.MqttTimeout ?? defaultTimeout);
                    await mqttProtocol.ConnectAndSubscribeAsync(config.MqttTopic);

                    mqttProtocol.MessageReceived += async (topic, message) =>
                    {
                        var influxProtocol = new clsRTDBInfluxDBProtocol(
                            influxConfig.url,
                            influxConfig.token,
                            influxConfig.org,
                            influxConfig.bucket);
                        
                        await influxProtocol.WriteData(influxConfig.measurement, topic, message);
                    };
                    
                    logAppendedForInflux = true;
                    logAppendedForMqtt = true;
                }
            } 
            else if(string.IsNullOrEmpty(config.MqttUrl) && !string.IsNullOrEmpty(config.CoapIp))
            {
                if (config.RtdbType.ToLower().Equals("influxdb"))
                {
                    string coapUrl = $"coap://{config.CoapIp}:{config.CoapPort}/{config.CoapTopic}";

                    SetupCoAP(coapUrl, config.CoapTopic);
                    
                    int intervalInSeconds = Convert.ToInt32(CoapTimer.Value);
                    _coapTimer?.Dispose();
                    _coapTimer = new Timer
                    {
                        Interval = intervalInSeconds * 1000
                    };
                    
                    coapProtocol.MessageReceived += (topic, message) =>
                    {
                        _coapTimer.Tick += async (sender, e) =>
                        {
                            var influxProtocol = new clsRTDBInfluxDBProtocol(
                                influxConfig.url,
                                influxConfig.token,
                                influxConfig.org,
                                influxConfig.bucket);
                            await influxProtocol.WriteData(influxConfig.measurement, topic, message);
                            appendLog("[CoAP] messages sent to [InfluxDB]");
                        };
                    };
                    logAppendedForInflux = true;
                    logAppendedForCoap = true;
                    _coapTimer.Start();
                }
            }
            else
            {
                appendLog("MQTT/CoAP Message is Empty. Please check your configuration.");
            }

        }

        private async Task ProcessTimeScaleDBConfiguration(RtdbConfiguration config, TimeScaleDB timescaleConfig)
        {
            if (!string.IsNullOrEmpty(config.MqttUrl) && string.IsNullOrEmpty(config.CoapIp))
            {
                if (config.RtdbType.ToLower().Equals("timescaledb"))
                {
                    var mqttProtocol = new clsMQTTBrokerProtocol(
                        config.MqttUrl,
                        config.MqttPort ?? defaultMqttPort,
                        config.MqttClientId,
                        config.MqttUsername,
                        config.MqttPassword,
                        config.MqttCleanSession ?? defaultCleanSession,
                        config.MqttTimeout ?? defaultTimeout);
                    await mqttProtocol.ConnectAndSubscribeAsync(config.MqttTopic);

                    mqttProtocol.MessageReceived += async (topic, message) =>
                    {
                        var timescaleProtocol = new ClsRtdbTimeScaleDbProtocol(timescaleConfig.ConnectionString);
                        await timescaleProtocol.WriteDataAsync(timescaleConfig.table, topic, message);
                    };
                    
                    logAppendedForTimeScale = true;
                    logAppendedForMqtt = true;
                }
            }
            else if (string.IsNullOrEmpty(config.MqttUrl) && !string.IsNullOrEmpty(config.CoapIp))
            {
                if (config.RtdbType.ToLower().Equals("timescaledb"))
                {
                    string coapUrl = $"coap://{config.CoapIp}:{config.CoapPort}/{config.CoapTopic}";

                    SetupCoAP(coapUrl, config.CoapTopic);
                    int intervalInSeconds = Convert.ToInt32(CoapTimer.Value);
                    _coapTimer?.Dispose();
                    _coapTimer = new Timer
                    {
                        Interval = intervalInSeconds * 1000
                    };

                    coapProtocol.MessageReceived += (topic, message) =>
                    {
                        _coapTimer.Tick += async (sender, e) =>
                        {
                            var timescaleProtocol = new ClsRtdbTimeScaleDbProtocol(timescaleConfig.ConnectionString);
                            await timescaleProtocol.WriteDataAsync(timescaleConfig.table, topic, message);
                            appendLog("[CoAP] messages sent to [TimeScaleDB]");
                        };
                        logAppendedForTimeScale = true;
                        logAppendedForCoap = true;
                        _coapTimer.Start();
                    };
                }
            }
            else
            {
                appendLog("MQTT/CoAP Message is Empty. Please check your configuration.");
            }
        }

        private delegate void SafeCallDelegate_appendLog(String strContent, bool newline = true, bool Inc_time = true);

        private void appendLog(String strContent, bool newline = true, bool Inc_time = true) //Thread Safe
        {
            DateTime localDate = DateTime.Now;

            if (true)
            {
                if (txtLog.InvokeRequired)
                {
                    var d = new SafeCallDelegate_appendLog(appendLog);
                    txtLog.Invoke(d, new object[] { strContent, newline, Inc_time });
                }
                else
                {
                    if (newline && Inc_time)
                        this.txtLog.AppendText("[" + localDate.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture) + "] " + strContent + "\r\n");
                    else if (newline == false && Inc_time == true)
                        this.txtLog.AppendText("[" + localDate.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture) + "] " + strContent);
                    else if (newline == true && Inc_time == false)
                        this.txtLog.AppendText(strContent + "\r\n");
                    else
                        this.txtLog.AppendText(strContent);

                    if (newline)
                    {
                        string exp_st = null;
                        if (++log_line_cnt > MAX_LOG_LINES)
                        {
                            log_line_cnt = 0;

                            if (this.LogSaveCheckBox.Checked == true)
                            {
                                try
                                {
                                    if (!Directory.Exists(IOT_LogPath))
                                    {
                                        Directory.CreateDirectory(IOT_LogPath);
                                    }
                                    File.AppendAllText(IOT_LogPath + IOT_LogFileName, txtLog.Text);
                                }
                                catch (Exception exp)
                                {
                                    exp_st = exp.ToString();
                                }
                            }

                            this.txtLog.Clear();
                            if (exp_st != null)
                            {
                                this.txtLog.AppendText("\r\n" + exp_st + "\r\n");
                                log_line_cnt++;
                            }
                        }
                    }
                }
            }
        }
        
        
        private void frmAgentMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            appendLog("App. 강제 종료 !\r\n");
            IOT_LogFileName = $"NILE_Agent_{DateTime.Now:yyyy_MM_dd}.log";

            if (LogSaveCheckBox.Checked)
            {
                Properties.Settings.Default.SAVE_LOG_PATH = LogSaveCheckBox.Checked;
                Properties.Settings.Default.LOG_PATH = txtLog.Text;
                Properties.Settings.Default.Save();
            }
            if (filePathSaveCheckBox.Checked)
            {
                Properties.Settings.Default.SAVE_FILE_PATH = filePathSaveCheckBox.Checked;
                Properties.Settings.Default.FILE_PATH = txtFilePath.Text;
                Properties.Settings.Default.Save();
            }
            
            mqttConnections.Clear();
            StopAndDisposeCoapTimer();
            try
            {
                if (!Directory.Exists(txtLogPath.Text))
                {
                    Directory.CreateDirectory(txtLogPath.Text);
                }
                File.AppendAllText(Path.Combine(txtLogPath.Text, IOT_LogFileName), txtLog.Text);
            }
            catch (Exception exp)
            {
                File.AppendAllText("NILE_Agent_ERR.txt", exp.ToString());
            }

        }

        private void LogSaveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (LogSaveCheckBox.Checked)
            {
                this.txtLogPath.Text = IOT_LogPath;
                Properties.Settings.Default.SAVE_LOG_PATH = this.LogSaveCheckBox.Checked;
                Properties.Settings.Default.Save();
            }
        }

        private void txtFilePath_TextChanged(object sender, EventArgs e)
        {
            filePathSaveCheckBox.Checked = false;
        }

        private void btnFilePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = Resources.FileFilterConfig;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFileDialog.FileName;
            }
        }
        
        private void filePathSaveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (filePathSaveCheckBox.Checked)
            {
                Properties.Settings.Default.SAVE_FILE_PATH = this.filePathSaveCheckBox.Checked;
                Properties.Settings.Default.Save();
            }
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {

        }
    }

    
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class RtdbConfiguration
    {
        public string MqttUrl { get; set; }
        public int? MqttPort { get; set; }
        public string MqttClientId { get; set; }
        public int? MqttTimeout { get; set; }
        public bool? MqttCleanSession { get; set; }
        public string MqttUsername { get; set; }
        public string MqttPassword { get; set; }
        public string MqttTopic { get; set; }
        
        public int? MqttQos { get; set; }
        public string CoapIp { get; set; }
        public string CoapPort { get; set; }
        
        public string CoapClientId { get; set; }
        
        public string CoapUsername { get; set; }
        
        public string CoapPassword { get; set; }
        
        public string CoapMessageId { get; set; }
        
        public string CoapToken { get; set; }
        public string CoapTopic { get; set; }
        
        public int? CoapTimeout { get; set; }
        public string RtdbType { get; set; }
        public dynamic RtdbConnectionInfo { get; set; }
    }

    
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class Redis
    {
        public bool ClusterMode { get; set; }
        public bool SingleNodeInsert { get; set; }
        public string ConnectionString { get; set; }
        public string[] ConnectionStringManyNodes { get; set; }
        public int Database { get; set; }
    }

    
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class InfluxDB
    {
        public string url { get; set; }
        public string token { get; set; }
        public string org { get; set; }
        public string bucket { get; set; }
        public string measurement { get; set; }
    }
    
    
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class TimeScaleDB
    {
        public string ConnectionString { get; set; }
        public string table { get; set; }
    }

  
}