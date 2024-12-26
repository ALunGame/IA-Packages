using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAECS
{
    /// <summary>
    /// 信息类型
    /// </summary>
    public enum SensorType
    {
        /// <summary>
        /// 全局信息
        /// </summary>
        Global,

        /// <summary>
        /// 玩家信息
        /// </summary>
        Player,                  

        /// <summary>
        /// 地图信息
        /// </summary>
        Map,                     

        /// <summary>
        /// 实体信息
        /// </summary>
        Entity,                  

        /// <summary>
        /// 技能Aoe信息
        /// </summary>
        Skill_Aoe,                   

        /// <summary>
        /// 技能Bullet信息
        /// </summary>
        Skill_Bullet,    
    }
}
