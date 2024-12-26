using System;

namespace IAECS.Layer.Request
{
    /// <summary>
    /// 实体请求 特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RequestAttribute : Attribute
    {
        private RequestId reqId = 0;
        /// <summary>
        /// 世界信息键
        /// </summary>
        public RequestId ReqId
        {
            get { return reqId; }
            set { reqId = value; }
        }

        public RequestAttribute(RequestId reqId)
        {
            this.reqId = reqId;
        }
    }

    public interface IRequest
    {
        /// <summary>
        /// 自身置换规则
        /// </summary>
        /// <param name="swId">请求置换Id</param>
        /// <param name="resId">自身置换后的Id</param>
        /// <returns>置换规则  不需要自身处理直接 Return ECSDefinition.RESwithRuleSelf</returns>
        int SwitchRequest(RequestId swId, ref RequestId resId);
    }
}
