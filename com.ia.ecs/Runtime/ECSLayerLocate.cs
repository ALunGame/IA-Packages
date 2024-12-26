using IAECS.Server.Layer;

namespace IAECS
{
    /// <summary>
    /// ECS层级定位器
    /// </summary>
    public class ECSLayerLocate
    {
        public static InfoServer Info { get; set; }
        public static RequestServer Request { get; set; }
        public static BehaviorServer Behavior { get; set; }

        public static void InitLayerServer()
        {
            Info = new InfoServer();
            Info.Init();
            Request = new RequestServer();
            Request.Init();
            Behavior = new BehaviorServer();
            Behavior.Init();
        }
    }
}
