using System;

namespace Lab2_Server
{
    public static class Config
    {
        public static readonly int C_port = 1233;
        public static readonly int AS_port = 1234;
        public static readonly int SS_port = 1235;
        public static readonly int TGS_port = 1236;

        public static readonly byte[] C_Key = Helper.ExtendKey("C_Key");
        public static readonly byte[] C_To_TGS_Key = Helper.ExtendKey("C_To_TGS_Key");
        public static readonly byte[] C_To_SS_Key = Helper.ExtendKey("C_To_SS_Key");
        public static readonly byte[] AS_To_TGS_Key = Helper.ExtendKey("AS_To_TGS_Key");
        public static readonly byte[] TGS_To_SS_Key = Helper.ExtendKey("TGS_To_SS_Key");

        public static readonly TimeSpan AS_Duration = new TimeSpan(24, 0, 0);
        public static readonly TimeSpan TGS_Duration = new TimeSpan(12, 0, 0);

        public static readonly string TGS = "TGS";
        public static readonly string id_SS = "id_SS";
    }
}
