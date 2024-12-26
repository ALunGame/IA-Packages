using IAECS.Data;
using IAECS.Layer.Info;
using IAToolkit;
using System;
using System.Collections.Generic;
using IAEngine;
using IAServer;
using UnityEngine;

namespace IAECS.Server.Layer
{
    public class InfoServer : BaseServer
    {
        private Dictionary<int, ISensor> SensorDict = new Dictionary<int, ISensor>();
        private Dictionary<string, EntityWorkData> WorkDataDict = new Dictionary<string, EntityWorkData>();

        private void RegAllSensor()
        {
            //世界信息
            foreach (Type type in ReflectionHelper.GetChildTypes<ISensor>())
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;
                if (AttributeHelper.TryGetTypeAttribute(type, out WorldSensorAttribute attr))
                {
                    ISensor sensor = ReflectionHelper.CreateInstance(type) as ISensor;
                    SensorDict.Add((int)attr.InfoKey, sensor);
                }
                else
                {
                    ECSLocate.Log.LogR("有世界信息没有加入特性 >>>>>>", type.Name);
                    return;
                }
            }
        }
        
        private T GetSensor<T>(int key) where T : ISensor
        {
            if (!SensorDict.ContainsKey(key))
            {
                ECSLocate.Log.LogError("没有找到对应的世界信息》》》》" + key);
                return default ;
            }
            return (T)SensorDict[key];
        }
        
        public override void OnInit()
        {
            RegAllSensor();
        }

        public T GetSensor<T>(SensorType key) where T : ISensor
        {
            return GetSensor<T>((int) key);
        }
        
        public void AddEntityWorkData(string uid, EntityWorkData data)
        {
            if (WorkDataDict.ContainsKey(uid))
            {
                return;
            }
            WorkDataDict.Add(uid, data);
        }

        public EntityWorkData GetEntityWorkData(string uid)
        {
            if (WorkDataDict.ContainsKey(uid))
            {
                return WorkDataDict[uid];
            }
            return null;
        }

        public void RemoveEntityWorkData(string uid)
        {
            if (WorkDataDict.ContainsKey(uid))
            {
                WorkDataDict.Remove(uid);
            }
        }
    }
}
