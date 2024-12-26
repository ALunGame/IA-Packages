using IAECS.Server;
using IAECS.Server.ECS;
using IAECS.Server.Player;

namespace IAECS
{
    /// <summary>
    /// ECS服务定位器
    /// </summary>
    public static class ECSLocate
    {
        /// <summary>
        /// ECS中心
        /// </summary>
        public static ECSCenter Center { get; private set; }

        /// <summary>
        /// 决策中心
        /// </summary>
        public static DecisionCenter DecCenter { get; private set; }

        /// <summary>
        /// 日志
        /// </summary>
        public static ECSLogServer Log { get; set; }
        public static ECSServer ECS { get; set; }
        public static IPlayerServer Player { get; set; }

        public static void InitServer()
        {
            Log         = new ECSLogServer();
            ECS         = new ECSServer();
            //Player      = new PlayerServer();
        }

        public static void InitECSCenter(ECSCenter center)
        {
            Center = center;
        }

        public static void InitDecCenter(DecisionCenter decCenter)
        {
            DecCenter = decCenter;
        }

        public static void Clear()
        {
            //Center.Clear();
            DecCenter.Clear();
            Log       = null;
            ECS       = null;
            Player    = null;
        }
    }
}
