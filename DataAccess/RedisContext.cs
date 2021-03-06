﻿using DataAccess.Abstract;
using DataAccess.Concrete;
using Model;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public class RedisContext : CacheHelper, ICacheManager
    {
        private static IDatabase _db;
        private static readonly string host = "localhost";
        private static readonly int port = 6379;

        public RedisContext()
        {
            CreateRedisDB();
        }

        private static IDatabase CreateRedisDB()
        {
            if (null == _db)
            {
                ConfigurationOptions option = new ConfigurationOptions();
                option.Ssl = false;
                option.EndPoints.Add(host, port);
                var connect = ConnectionMultiplexer.Connect(option);
                _db = connect.GetDatabase();
            }

            return _db;
        }

        public void Clear()
        {
            var server = _db.Multiplexer.GetServer(host, port);
            foreach (var item in server.Keys())
                _db.KeyDelete(item);
        }

        public T Get<T>(string key)
        {
            var rValue = _db.SetMembers(key);
            if (rValue.Length == 0)
                return default(T);

            var result = Deserialize<T>(rValue.ToStringArray());
            return result;
        }

        public bool IsSet(string key)
        {
            return _db.KeyExists(key);
        }

        public bool RemoveValue(string key, object data)
        {
            if (data == null)
                return false;

            var entryBytes = Serialize(data);
            return _db.SetRemove(key, entryBytes);
        }

        public bool Remove(string key)
        {
            return _db.KeyDelete(key);        
        }

        public void RemoveByPattern(string pattern)
        {
            var server = _db.Multiplexer.GetServer(host, port);
            foreach (var item in server.Keys(pattern: "*" + pattern + "*"))
                _db.KeyDelete(item);
        }

        public void Set(string key, object data)
        {
            if (data == null)
                return;

            var entryBytes = Serialize(data);
            _db.SetAdd(key, entryBytes);


           //  expire ;
           // var expiresIn = TimeSpan.FromMinutes(cacheTime);

            //if (cacheTime > 0)
            //    _db.KeyExpire(key, expiresIn);
        }


    }
}
