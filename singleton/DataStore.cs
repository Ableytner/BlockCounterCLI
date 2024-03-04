namespace BlockCounterCLI
{
    internal class DataStore
    {
        private DataStore() { }

        private static DataStore instance = null;

        public static DataStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataStore();
                }
                return instance;
            }
        }
    }
}
