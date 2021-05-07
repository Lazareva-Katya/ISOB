namespace Lab2_Client
{
    public static class Config
    {
        public static readonly byte[] C_Key = Helper.ExtendKey("C_Key");

        public static readonly int C_port = 1233;
        public static readonly int AS_port = 1234;
        public static readonly int SS_port = 1235;
        public static readonly int TGS_port = 1236;

        public static readonly string tgs = "TGS";
        public static readonly string id_SS = "id_SS";
    }
}
