// using IAECS.Core;
// using IAECS.Data;
// using UnityEngine;
//
// namespace IAECS.Server.Player
// {
//     public class PlayerServer : IPlayerServer
//     {
//         private Entity playerEntity;
//         private EntityWorkData playerWorkData;
//
//         public void SetPlayerEntity(Entity entity)
//         {
//             playerEntity = entity;
//             playerWorkData = ECSLayerLocate.Info.GetEntityWorkData(entity.Uid);
//         }
//
//         public Entity GetPlayerEntity()
//         {
//             return playerEntity;
//         }
//
//         public EntityWorkData GetPlayerWorkData()
//         {
//             return playerWorkData;
//         }
//
//         //请求
//         public void PushPlayerReq(RequestId reqId, ParamData param)
//         {
//             //ECSLayerLocate.Request.PushRequest(playerEntity.Uid, reqId, param);
//         }
//     }
// }
