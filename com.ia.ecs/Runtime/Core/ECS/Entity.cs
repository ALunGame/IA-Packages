using System.Collections.Generic;

namespace IAECS.Core
{
    public class Entity
    {
        public string Uid { get; protected set; } = "";

        public int EntityId;

        public bool isActive = false;
        public bool IsActive { 
            get 
            { 
                return isActive; 
            }
            set
            {
                if (value)
                {
                    Enable();
                }
                else
                {
                    Disable();
                }
            } 
        }

        private Dictionary<string, BaseCom> coms = new Dictionary<string, BaseCom>();

        public Entity(string uid)
        {
            Uid = uid;
        }

        #region 生命周期

        public void Init()
        {
            OnInit();
        }

        /// <summary>
        /// 所有组件添加完毕
        /// </summary>
        protected virtual void OnInit() { }

        public void Enable()
        {
            isActive = true;
            foreach (BaseCom com in coms.Values)
            {
                com.Enable(false);
            }
            //ECSLocate.ECS.CheckEntityInSystem(Uid);
            OnEnable();
        }

        protected virtual void OnEnable() { }

        public void Disable()
        {
            isActive = false;
            foreach (BaseCom com in coms.Values)
            {
                com.Disable(false);
            }
            //ECSLocate.ECS.CheckEntityInSystem(Uid);
            OnDisable();
        }

        protected virtual void OnDisable() { }

        public void Destroy()
        {
            //禁用
            Disable();
            
            //销毁
            foreach (BaseCom com in coms.Values)
            {
                com.Destroy(false);
            }
            
            OnDestroy();
        }

        protected virtual void OnDestroy() { }

        #endregion

        #region Override

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is Entity)
            {
                Entity other = (Entity)obj;
                return other.Uid == Uid;
            }
            return false;
        }

        #endregion

        #region 组件相关

        public IEnumerable<BaseCom> GetComs()
        {
            foreach (var item in coms.Values)
            {
                yield return item;
            }
        }

        public bool HasCom(string comFullName)
        {
            return coms.ContainsKey(comFullName);
        }

        public bool HasCom(BaseCom checkCom)
        {
            return coms.ContainsKey(checkCom.GetType().Name);
        }

        public T GetOrCreateCom<T>() where T : BaseCom, new()
        {
            string typeName = typeof(T).Name;
            if (!HasCom(typeName))
            {
                T newCom = new T();
                newCom.Init(this);
                coms.Add(typeName, newCom);
            }
            return coms[typeName] as T;
        }

        public BaseCom GetCom(string comName)
        {
            if (!coms.ContainsKey(comName))
                return null;
            return coms[comName];
        }

        public T GetCom<T>() where T : BaseCom
        {
            string typeName = typeof(T).Name;
            if (!coms.ContainsKey(typeName))
                return null;
            return coms[typeName] as T;
        }

        public bool GetCom<T>(out T outCom) where T : BaseCom
        {
            string typeName = typeof(T).Name;
            if (!coms.ContainsKey(typeName))
            {
                outCom = null;
                return false;
            }
            outCom = (T)coms[typeName];
            return true;
        }

        public T AddCom<T>(T com) where T : BaseCom
        {
            string typeName = com.GetType().Name;

            if (HasCom(typeName))
                return GetCom<T>();

            //保存数据
            coms.Add(typeName, com);
            com.Init(this);
            return com;
        }

        public void RemoveCom<T>() where T : BaseCom
        {
            string typeName = typeof(T).Name;
            if (!HasCom(typeName))
                return;

            //调用函数
            BaseCom com = coms[typeName];
            com.Destroy();

            //清除数据
            coms.Remove(typeName);
        }

        public void CoverCom(BaseCom com)
        {
            BaseCom oldCom = GetCom(com.GetType().Name);
            if (oldCom == null)
            {
                //ECSLocate.Log.LogError("覆盖实体组件失败，没有对应组件>>>>>>>", com);
                return;
            }
            coms[com.GetType().Name] = com;
        }

        #endregion
    }
}
