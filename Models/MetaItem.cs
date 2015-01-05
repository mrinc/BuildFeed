﻿using NServiceKit.DataAnnotations;
using NServiceKit.DesignPatterns.Model;
using NServiceKit.Redis;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Required = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace BuildFeed.Models
{
    [DataObject]
    public class MetaItem : IHasId<MetaItemKey>
    {
        [Key]
        [Index]
        [@Required]
        public MetaItemKey Id { get; set; }

        [DisplayName("Page Content")]
        [AllowHtml]
        public string PageContent { get; set; }

        [DisplayName("Meta Description")]
        public string MetaDescription { get; set; }



        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static IEnumerable<MetaItem> Select()
        {
            using (RedisClient rClient = new RedisClient(DatabaseConfig.Host, DatabaseConfig.Port, db: DatabaseConfig.Database))
            {
                var client = rClient.As<MetaItem>();
                return client.GetAll();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public static IEnumerable<MetaItem> SelectByType(MetaType type)
        {
            using (RedisClient rClient = new RedisClient(DatabaseConfig.Host, DatabaseConfig.Port, db: DatabaseConfig.Database))
            {
                var client = rClient.As<MetaItem>();
                return from t in client.GetAll()
                       where t.Id.Type == type
                       select t;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static MetaItem SelectById(MetaItemKey id)
        {
            using (RedisClient rClient = new RedisClient(DatabaseConfig.Host, DatabaseConfig.Port, db: DatabaseConfig.Database))
            {
                var client = rClient.As<MetaItem>();
                return client.GetById(id);
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static IEnumerable<string> SelectUnusedLabs()
        {

            using (RedisClient rClient = new RedisClient(DatabaseConfig.Host, DatabaseConfig.Port, db: DatabaseConfig.Database))
            {
                var client = rClient.As<MetaItem>();
                var labs = Build.SelectBuildLabs();

                var usedLabs = from u in client.GetAll()
                               where u.Id.Type == MetaType.Lab
                               select u;

                return from l in labs
                       where !usedLabs.Any(ul => ul.Id.Value as string == l)
                       select l;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public static void Insert(MetaItem item)
        {
            using (RedisClient rClient = new RedisClient(DatabaseConfig.Host, DatabaseConfig.Port, db: DatabaseConfig.Database))
            {
                var client = rClient.As<MetaItem>();
                client.Store(item);
            }
        }

        [DataObjectMethod(DataObjectMethodType.Update, true)]
        public static void Update(MetaItem item)
        {
            using (RedisClient rClient = new RedisClient(DatabaseConfig.Host, DatabaseConfig.Port, db: DatabaseConfig.Database))
            {
                var client = rClient.As<MetaItem>();
                client.Store(item);
            }
        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public static void InsertAll(IEnumerable<MetaItem> items)
        {
            using (RedisClient rClient = new RedisClient(DatabaseConfig.Host, DatabaseConfig.Port, db: DatabaseConfig.Database))
            {
                var client = rClient.As<MetaItem>();
                client.StoreAll(items);
            }
        }

        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public static void DeleteById(long id)
        {
            using (RedisClient rClient = new RedisClient(DatabaseConfig.Host, DatabaseConfig.Port, db: DatabaseConfig.Database))
            {
                var client = rClient.As<MetaItem>();
                client.DeleteById(id);
            }
        }
    }

    public struct MetaItemKey
    {
        public object Value { get; set; }
        public MetaType Type { get; set; }
    }

    public enum MetaType
    {
        Lab,
        Version,
        Source
    }
}