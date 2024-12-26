
namespace IAECS.Core.Tree.Base
{
    /// <summary>
    /// 节点前提----------单链表
    /// </summary>
    public class NodePremise
    {
        //前提关系
        public PremiseType premiseType;

        public string nodeUid;

        //什么才是真
        public bool checkValue = true;

        //下一个前提
        public NodePremise otherPremise;

        public NodePremise()
        {

        }

        //添加前提
        public void AddOtherPrecondition(NodePremise premise)
        {
            otherPremise = premise;
        }

        //检测方法
        public bool IsTrue(NodeData wData)
        {
            bool resValue = OnMakeTrue(wData);
            resValue = checkValue == resValue ? true : false;
            if (otherPremise != null)
            {
                switch (premiseType)
                {
                    case PremiseType.AND:
                        return resValue && otherPremise.IsTrue(wData);
                    case PremiseType.OR:
                        return resValue || otherPremise.IsTrue(wData);
                    case PremiseType.XOR:
                        return resValue ^ otherPremise.IsTrue(wData);
                }
                return resValue && otherPremise.IsTrue(wData);
            }
            else
            {
                return resValue;
            }
        }

        //子类重写
        public virtual bool OnMakeTrue(NodeData wData)
        {
            return true;
        }
    }
}
