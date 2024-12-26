using IAECS.Core;
using IAECS.Data;
using UnityEngine;

namespace IAECS.Server.Player
{
    /// <summary>
    /// 玩家服务
    /// </summary>
    public interface IPlayerServer
    {
        void SetPlayerEntity(Entity entity);

        Entity GetPlayerEntity();

        EntityWorkData GetPlayerWorkData();

        void PushPlayerReq(RequestId reqId, ParamData param);
    }
}
