using System;

namespace IAECS.Core
{
    public class BaseCom
    {
        [NonSerialized]
        private string entityUid = "";

        //为了序列化，运行时不可设置
        public bool isActive = true;
        public bool IsActive { get => isActive;}

        public string EntityUid { get => entityUid; }

        public string GetComName()
        {
            return this.GetType().Name;
        }

        //初始化（首次添加调用）
        public void Init(Entity pEntity)
        {
            this.entityUid = pEntity.Uid;
            OnInit(pEntity);

            if (isActive)
                OnEnable();
            else
                OnDisable();
        }

        /// <summary>
        /// 初始化，这时候其他组件可能尚未添加
        /// </summary>
        /// <param name="pEntity"></param>
        protected virtual void OnInit(Entity pEntity)
        {

        }

        public void Awake(Entity pEntity)
        {
            OnAwake(pEntity);
        }

        /// <summary>
        /// 组件启动，这时候所有组件都被添加完毕
        /// </summary>
        /// <param name="pEntity"></param>
        protected virtual void OnAwake(Entity pEntity)
        {

        }

        public void Enable(bool pSystemCheck = true)
        {
            isActive = true;
            OnEnable();
            //if (pSystemCheck)
                //ECSLocate.ECS.CheckEntityInSystem(EntityUid);
        }

        protected virtual void OnEnable()
        {

        }

        public void Disable(bool pSystemCheck = true)
        {
            isActive = false;
            OnDisable();
            //if (pSystemCheck)
                //ECSLocate.ECS.CheckEntityInSystem(EntityUid);
        }

        protected virtual void OnDisable()
        {

        }

        public void Destroy(bool pSystemCheck = true)
        {
            Disable(pSystemCheck);
            OnDestroy();
        }

        protected virtual void OnDestroy()
        {

        }

        #region Editor

        public virtual void OnDrawGizmosSelected()
        {

        }

        public virtual void OnDrawGizmos()
        {

        } 

        #endregion
    }
}
