using IAECS.Config;
using IAECS.Data;
using IAECS.Layer.Request;
using IAEngine;
using IAServer;
using System;
using System.Collections.Generic;

namespace IAECS.Server.Layer
{
    public class RequestServer : BaseServer
    {
        private Dictionary<RequestId, IRequest> RequestDict = new Dictionary<RequestId, IRequest>();
        
        private void RegAllRequest()
        {
            foreach (Type type in ReflectionHelper.GetChildTypes<IRequest>())
            {
                if (AttributeHelper.TryGetTypeAttribute(type, out RequestAttribute attr))
                {
                    IRequest request = ReflectionHelper.CreateInstance<IRequest>(type.FullName);
                    RequestDict.Add(attr.ReqId, request);
                }
                else
                {
                    ECSLocate.Log.Log("有请求没有加特性 走权重 >>>>>>", type.Name);
                    return;
                }
            }
        }
        
        private IRequest GetEntityRequest(RequestId key)
        {
            if (!RequestDict.ContainsKey(key))
                return null;
            return RequestDict[key];
        }
        
        public override void OnInit()
        {
            RegAllRequest();
        }
        
        public int GetRequestSort(RequestId reqId)
        {
            RequestSort sort = ECSLocate.Center.GetRequestSort(reqId);
            if (sort == null)
                return 0;
            return sort.sort;
        }
        
        public void PushRequest(string uid, RequestData requestData)
        {
            RequestId reqId = requestData.ReqId;
            //数据
            EntityWorkData workData = ECSLayerLocate.Info.GetEntityWorkData(uid);
            if (workData == null)
            {
                return;
            }

            if (workData.CurrReqId == reqId)
            {
                workData.AddParam(requestData);
                return;
            }

            //请求
            IRequest pushRequest = GetEntityRequest(reqId);
            RequestId oldReqId   = workData.CurrReqId;

            //请求不需要自身处理
            if (pushRequest == null)
            {
                ChangeRequest(workData, reqId, requestData);
            }
            else
            {
                RequestId selfSwId = reqId;
                //请求内部置换
                int rule = pushRequest.SwitchRequest(reqId, ref selfSwId);
                //只需要自身判断
                if (rule == ECSDefinition.RESwithRuleSelf)
                {
                    if (workData.CurrReqId != selfSwId)
                    {
                        workData.ChangeRequestId(reqId);
                        workData.AddParam(requestData);
                    }
                }
                else
                {
                    //自身判断也需要权重置换规则
                    ChangeRequest(workData, reqId, requestData);
                }
            }

            if (workData.CurrReqId == oldReqId)
                return;

            //执行请求
            ECSLayerLocate.Behavior.ReqBev(workData, oldReqId);
        }

        private void ChangeRequest(EntityWorkData workData, RequestId reqId, RequestData requestData)
        {
            if (workData.CurrReqId == RequestId.None)
            {
                workData.ChangeRequestId(reqId);
                workData.AddParam(requestData);
            }
            else
            {
                int pushSort = GetRequestSort(reqId);
                int curSort = GetRequestSort(workData.CurrReqId);

                //强制置换（覆盖）
                if (pushSort == ECSDefinition.REForceSwithWeight)
                {
                    workData.ChangeRequestId(reqId);
                    workData.AddParam(requestData);
                }
                else
                {
                    //判断当前的
                    if (pushSort < curSort)
                    {
                        workData.ChangeRequestId(reqId);
                        workData.AddParam(requestData);
                    }
                }
            }
        }
    }
}
