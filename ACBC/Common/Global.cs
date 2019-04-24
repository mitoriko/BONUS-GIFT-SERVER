using ACBC.Buss;
using ACBC.Dao;
using Com.ACBC.Framework.Database;
using Newtonsoft.Json;
using Senparc.CO2NET.Cache.Redis;
using StackExchange.Redis;
using System;

namespace ACBC.Common
{
    public class Global
    {

#if DEBUG
        public const string ENV = "DEV";
#else
        public const string ENV = "PRO";
#endif
        public const string GROUP = "Gift-Server";

        public const string CONFIG_TOPIC = "ConfigServerTopic";

        public const string TOPIC_MESSAGE = "update";

        public const string ROUTE_PX = "/api/gift";
        public const string NAMESPACE = "com.a-cubic.gift";

        public const int REDIS_NO = 1;
        public const int REDIS_EXPIRY_H = 0;
        public const int REDIS_EXPIRY_M = 1;
        public const int REDIS_EXPIRY_S = 0;

        public const int SESSION_EXPIRY_H = 1;
        public const int SESSION_EXPIRY_M = 0;
        public const int SESSION_EXPIRY_S = 0;

        public const string SMS_CODE_URL = "http://v.juhe.cn/sms/send?mobile={3}&tpl_id={1}&tpl_value=%23code%23%3D{2}&dtype=&key={0}";
        public const string EXCHANGE_URL = "http://op.juhe.cn/onebox/exchange/query?key=08940f90d07501ace3f535e32968cf94";

        static Action<ChannelMessage> action = new Action<ChannelMessage>(onMessageHandle);


        /// <summary>
        /// 基础业务处理类对象
        /// </summary>
        public static BaseBuss BUSS = new BaseBuss();

        /// <summary>
        /// 初始化启动预加载
        /// </summary>
        public static void StartUp()
        {
            GetConfig(true);
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
            RedisManager.ConfigurationOption = REDIS;
        }

        static void Subscribe()
        {
            var redis = RedisManager.Manager;
            var queue = redis.GetSubscriber().Subscribe(CONFIG_TOPIC + "." + ENV + "." + GROUP);

            queue.OnMessage(action);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "> " + "已订阅" + CONFIG_TOPIC + "." + ENV + "." + GROUP + "配置更新");
        }

        public static void onMessageHandle(ChannelMessage channelMessage)
        {
            if (channelMessage.Message.ToString() == TOPIC_MESSAGE)
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "> " + "收到配置更新通知");
                GetConfig(false);
            }
        }

        static void GetConfig(bool isFirst)
        {
            string url = "http://ConfigServer/api/config/Config/Open";
#if DEBUG
            url = "http://" + ConfigServer + "/api/config/Config/Open";
#endif
            ConfigParam configParam = new ConfigParam
            {
                env = ENV,
                group = GROUP
            };
            RequestParam requestParam = new RequestParam
            {
                method = "GetConfig",
                param = configParam
            };
            string body = JsonConvert.SerializeObject(requestParam);
            try
            {
                string resp = Utils.PostHttp(url, body, "application/json");
                ResponseObj responseObj = JsonConvert.DeserializeObject<ResponseObj>(resp);

                foreach (ConfigItem item in responseObj.data)
                {
                    Environment.SetEnvironmentVariable(item.key, item.value);
                }

                DatabaseOperation.TYPE = new DBManager();
                RedisManager.ConfigurationOption = REDIS;
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "> " + "加载配置信息完成");
                if (isFirst)
                {
                    Subscribe();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(url);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "> " + "加载配置信息失败");
            }
        }

        public static string SMS_CODE
        {
            get
            {
                var redis = System.Environment.GetEnvironmentVariable("SmsCode");
                return redis;
            }
        }

        public static string SMS_TPL
        {
            get
            {
                var redis = System.Environment.GetEnvironmentVariable("SmsTpl");
                return redis;
            }
        }

        public static string REDIS
        {
            get
            {
                var redis = System.Environment.GetEnvironmentVariable("redis");
                return redis;
            }
        }

        public static string ConfigServer
        {
            get
            {
                return Environment.GetEnvironmentVariable("ConfigServer");
            }
        }

        #region 小程序相关

        /// <summary>
        /// 小程序APPID
        /// </summary>
        public static string APPID
        {
            get
            {
                var appId = System.Environment.GetEnvironmentVariable("WxAppId");
                return appId;
            }
        }

        /// <summary>
        /// 小程序APPSECRET
        /// </summary>
        public static string APPSECRET
        {
            get
            {
                var appSecret = System.Environment.GetEnvironmentVariable("WxAppSecret");
                return appSecret;
            }
        }

        /// <summary>
        /// 小程序APPID
        /// </summary>
        public static string STOREAPPID
        {
            get
            {
                var appId = System.Environment.GetEnvironmentVariable("WxStoreAppId");
                return appId;
            }
        }

        /// <summary>
        /// 小程序APPSECRET
        /// </summary>
        public static string STOREAPPSECRET
        {
            get
            {
                var appSecret = System.Environment.GetEnvironmentVariable("WxStoreAppSecret");
                return appSecret;
            }
        }


        #endregion

        #region OSS相关

        /// <summary>
        /// AccessId
        /// </summary>
        public static string AccessId
        {
            get
            {
                var accessId = System.Environment.GetEnvironmentVariable("ossAccessId");
                return accessId;
            }
        }
        /// <summary>
        /// AccessKey
        /// </summary>
        public static string AccessKey
        {
            get
            {
                var accessKey = System.Environment.GetEnvironmentVariable("ossAccessKey");
                return accessKey;
            }
        }
        /// <summary>
        /// OssHttp
        /// </summary>
        public static string OssHttp
        {
            get
            {
                var ossHttp = System.Environment.GetEnvironmentVariable("ossHttp");
                return ossHttp;
            }
        }
        /// <summary>
        /// OssBucket
        /// </summary>
        public static string OssBucket
        {
            get
            {
                var ossBucket = System.Environment.GetEnvironmentVariable("ossBucket");
                return ossBucket;
            }
        }
        /// <summary>
        /// ossUrl
        /// </summary>
        public static string OssUrl
        {
            get
            {
                var ossUrl = System.Environment.GetEnvironmentVariable("ossUrl");
                return ossUrl;
            }
        }
        /// <summary>
        /// OssDir
        /// </summary>
        public static string OssDir
        {
            get
            {
                var ossDir = System.Environment.GetEnvironmentVariable("ossDir");
                return ossDir;
            }
        }
    }
    #endregion
}
