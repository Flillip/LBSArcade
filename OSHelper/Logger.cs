namespace OSHelper
{
    internal static class Logger
    {
        public static EasyLogPlus.Logger log = new();

        public static void Debug(object Content)
        {
#if DEBUG
            InitLogger();

            log.Debug(Content);
#endif
        }

        public static void Log(object Content)
        {
            InitLogger();

            log.Info(Content);
        }

        public static void Error(object Content)
        {
            InitLogger();

            log.Error(Content);
        }

        private static void InitLogger()
        {
            if (log.isInit)
                return;

            EasyLogPlus.Config config = new();

            config.LogPath = Environment.CurrentDirectory + @"\Helper.log";
            config.ShowDate = true;
            config.SeperateCriticalLogs = true;

#if DEBUG
            config.Console = true;
#endif

            log.cfg = config;
            log.InitLogger();
        }
    }
}
